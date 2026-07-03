Imports System.Data.SqlClient

Partial Public Class Accounting_JournalEntries
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String =
            "SELECT je.JournalEntryID, je.EntryNo, je.EntryDate, je.Description, je.SourceType, " &
            "ISNULL((SELECT SUM(Debit) FROM dbo.JournalEntryLines WHERE JournalEntryID = je.JournalEntryID), 0) AS TotalDebit " &
            "FROM dbo.JournalEntries je ORDER BY je.EntryDate DESC, je.JournalEntryID DESC"
        gvEntries.DataSource = DBHelper.ExecuteQuery(sql)
        gvEntries.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim journalEntryId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Hanya jurnal Manual yang boleh dihapus dari sini (jurnal otomatis dari Invoice/Payment/Payroll
        ' tidak ditampilkan tombol hapusnya - lihat Visible='<%# ... = "Manual" %>' di markup).
        DBHelper.ExecuteNonQuery("DELETE FROM dbo.JournalEntries WHERE JournalEntryID = @id AND SourceType = 'Manual'",
            New SqlParameter("@id", journalEntryId))

        BindGrid()
    End Sub

End Class
