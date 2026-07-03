Imports System.Data.SqlClient

Partial Public Class Accounting_ChartOfAccounts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT AccountID, AccountCode, AccountName, AccountType, NormalBalance " &
                             "FROM dbo.ChartOfAccounts WHERE IsActive = 1 ORDER BY AccountCode"
        gvAccounts.DataSource = DBHelper.ExecuteQuery(sql)
        gvAccounts.DataBind()
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim code As String = txtAccountCode.Text.Trim()
        Dim dupCount = Convert.ToInt32(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.ChartOfAccounts WHERE AccountCode = @Code", New SqlParameter("@Code", code)))

        If dupCount > 0 Then
            litMessage.Text = "<div class='error-msg'>Kode akun '" & Server.HtmlEncode(code) & "' sudah dipakai.</div>"
            Return
        End If

        DBHelper.ExecuteNonQuery(
            "INSERT INTO dbo.ChartOfAccounts (AccountCode, AccountName, AccountType, NormalBalance) VALUES (@Code, @Name, @Type, @Normal)",
            New SqlParameter("@Code", code),
            New SqlParameter("@Name", txtAccountName.Text.Trim()),
            New SqlParameter("@Type", ddlAccountType.SelectedValue),
            New SqlParameter("@Normal", ddlNormalBalance.SelectedValue))

        txtAccountCode.Text = ""
        txtAccountName.Text = ""
        litMessage.Text = ""
        BindGrid()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim accountId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Soft-delete saja: akun yang sudah pernah dipakai di jurnal tidak boleh dihapus permanen,
        ' supaya histori jurnal lama tetap valid.
        DBHelper.ExecuteNonQuery("UPDATE dbo.ChartOfAccounts SET IsActive = 0 WHERE AccountID = @id",
            New SqlParameter("@id", accountId))

        BindGrid()
    End Sub

End Class
