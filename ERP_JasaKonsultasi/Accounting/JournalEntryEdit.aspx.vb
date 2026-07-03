Imports System.Data.SqlClient
Imports System.Globalization

''' <summary>Satu baris jurnal manual, disimpan sementara di ViewState selama user mengedit form.</summary>
<Serializable()>
Public Class JournalLineItem
    Public Property AccountID As Integer
    Public Property AccountDisplay As String
    Public Property Debit As Decimal
    Public Property Credit As Decimal
    Public Property Notes As String
End Class

Partial Public Class Accounting_JournalEntryEdit
    Inherits System.Web.UI.Page

    Private Property JournalLines As List(Of JournalLineItem)
        Get
            If ViewState("JournalLines") Is Nothing Then
                ViewState("JournalLines") = New List(Of JournalLineItem)()
            End If
            Return CType(ViewState("JournalLines"), List(Of JournalLineItem))
        End Get
        Set(value As List(Of JournalLineItem))
            ViewState("JournalLines") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindAccountDropdown()
            txtEntryDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            JournalLines = New List(Of JournalLineItem)()
            BindLinesGrid()
        End If
    End Sub

    Private Sub BindAccountDropdown()
        Dim dt = DBHelper.ExecuteQuery("SELECT AccountID, AccountCode, AccountName FROM dbo.ChartOfAccounts WHERE IsActive = 1 ORDER BY AccountCode")
        ddlAccount.DataSource = dt
        ddlAccount.DataTextField = "AccountName"
        ddlAccount.DataValueField = "AccountID"
        ddlAccount.DataBind()

        ' Tampilkan "kode - nama" di dropdown supaya gampang dikenali
        For Each item As ListItem In ddlAccount.Items
            Dim row = dt.Select("AccountID = " & item.Value)(0)
            item.Text = row("AccountCode").ToString() & " - " & row("AccountName").ToString()
        Next
    End Sub

    Protected Sub btnAddLine_Click(sender As Object, e As EventArgs)
        If ddlAccount.Items.Count = 0 Then
            ShowMessage("Tambahkan minimal 1 akun di Chart of Accounts terlebih dahulu.")
            Return
        End If

        Dim debit As Decimal = 0
        Dim credit As Decimal = 0
        Decimal.TryParse(txtDebit.Text, NumberStyles.Any, CultureInfo.InvariantCulture, debit)
        Decimal.TryParse(txtCredit.Text, NumberStyles.Any, CultureInfo.InvariantCulture, credit)

        If debit = 0 AndAlso credit = 0 Then
            ShowMessage("Isi salah satu Debit atau Credit (tidak boleh keduanya nol).")
            Return
        End If
        If debit > 0 AndAlso credit > 0 Then
            ShowMessage("Satu baris hanya boleh diisi Debit ATAU Credit, tidak boleh dua-duanya.")
            Return
        End If

        Dim lines = JournalLines
        lines.Add(New JournalLineItem With {
            .AccountID = Convert.ToInt32(ddlAccount.SelectedValue),
            .AccountDisplay = ddlAccount.SelectedItem.Text,
            .Debit = debit,
            .Credit = credit,
            .Notes = txtLineNotes.Text.Trim()
        })
        JournalLines = lines

        txtDebit.Text = "0"
        txtCredit.Text = "0"
        txtLineNotes.Text = ""
        litMessage.Text = ""
        BindLinesGrid()
    End Sub

    Protected Sub lnkRemoveLine_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim index As Integer = Convert.ToInt32(btn.CommandArgument)

        Dim lines = JournalLines
        If index >= 0 AndAlso index < lines.Count Then lines.RemoveAt(index)
        JournalLines = lines

        BindLinesGrid()
    End Sub

    Private Sub BindLinesGrid()
        Dim lines = JournalLines
        gvLines.DataSource = lines
        gvLines.DataBind()

        Dim totalDebit As Decimal = lines.Sum(Function(l) l.Debit)
        Dim totalCredit As Decimal = lines.Sum(Function(l) l.Credit)
        litTotalDebit.Text = "Rp " & totalDebit.ToString("N0")
        litTotalCredit.Text = "Rp " & totalCredit.ToString("N0")
    End Sub

    Private Sub ShowMessage(message As String)
        litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim lines = JournalLines
        If lines.Count < 2 Then
            ShowMessage("Jurnal minimal harus punya 2 baris (1 Debit, 1 Credit).")
            Return
        End If

        Dim totalDebit As Decimal = lines.Sum(Function(l) l.Debit)
        Dim totalCredit As Decimal = lines.Sum(Function(l) l.Credit)

        If totalDebit <> totalCredit Then
            ShowMessage("Total Debit (Rp " & totalDebit.ToString("N0") & ") harus SAMA DENGAN Total Credit (Rp " & totalCredit.ToString("N0") & ").")
            Return
        End If

        Dim entryDate As DateTime = If(txtEntryDate.Text = "", DateTime.Now, Convert.ToDateTime(txtEntryDate.Text))

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                Dim entryNo As String = JournalHelper.GenerateEntryNo(conn, tran)
                Dim journalEntryId As Integer

                Using cmdHeader As New SqlCommand(
                    "INSERT INTO dbo.JournalEntries (EntryNo, EntryDate, Description, SourceType, CreatedBy) " &
                    "VALUES (@EntryNo, @EntryDate, @Description, 'Manual', @CreatedBy); SELECT SCOPE_IDENTITY();", conn, tran)
                    cmdHeader.Parameters.AddWithValue("@EntryNo", entryNo)
                    cmdHeader.Parameters.AddWithValue("@EntryDate", entryDate)
                    cmdHeader.Parameters.AddWithValue("@Description", txtDescription.Text.Trim())
                    cmdHeader.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                    journalEntryId = Convert.ToInt32(cmdHeader.ExecuteScalar())
                End Using

                For Each line As JournalLineItem In lines
                    Using cmdLine As New SqlCommand(
                        "INSERT INTO dbo.JournalEntryLines (JournalEntryID, AccountID, Debit, Credit, Notes) " &
                        "VALUES (@JEID, @AccID, @Debit, @Credit, @Notes)", conn, tran)
                        cmdLine.Parameters.AddWithValue("@JEID", journalEntryId)
                        cmdLine.Parameters.AddWithValue("@AccID", line.AccountID)
                        cmdLine.Parameters.AddWithValue("@Debit", line.Debit)
                        cmdLine.Parameters.AddWithValue("@Credit", line.Credit)
                        cmdLine.Parameters.AddWithValue("@Notes", If(line.Notes = "", CType(DBNull.Value, Object), line.Notes))
                        cmdLine.ExecuteNonQuery()
                    End Using
                Next

                tran.Commit()
            Catch ex As Exception
                tran.Rollback()
                ShowMessage("Gagal menyimpan jurnal: " & ex.Message)
                Return
            End Try
        End Using

        Response.Redirect("JournalEntries.aspx")
    End Sub

End Class
