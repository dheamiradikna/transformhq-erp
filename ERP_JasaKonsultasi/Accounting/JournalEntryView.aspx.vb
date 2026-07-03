Imports System.Data.SqlClient

Partial Public Class Accounting_JournalEntryView
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If String.IsNullOrEmpty(Request.QueryString("id")) Then
            Response.Redirect("JournalEntries.aspx")
            Return
        End If

        Dim journalEntryId As Integer = Convert.ToInt32(Request.QueryString("id"))

        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.JournalEntries WHERE JournalEntryID = @id",
            New SqlParameter("@id", journalEntryId))

        If dt.Rows.Count = 0 Then
            Response.Redirect("JournalEntries.aspx")
            Return
        End If

        Dim row = dt.Rows(0)
        litEntryNo.Text = row("EntryNo").ToString()
        litEntryDate.Text = Convert.ToDateTime(row("EntryDate")).ToString("dd-MM-yyyy")
        litSourceType.Text = row("SourceType").ToString()
        litCreatedBy.Text = If(row("CreatedBy") Is DBNull.Value, "-", row("CreatedBy").ToString())
        litDescription.Text = row("Description").ToString()

        Dim sqlLines As String =
            "SELECT coa.AccountCode, coa.AccountName, jel.Debit, jel.Credit, jel.Notes " &
            "FROM dbo.JournalEntryLines jel " &
            "INNER JOIN dbo.ChartOfAccounts coa ON jel.AccountID = coa.AccountID " &
            "WHERE jel.JournalEntryID = @id ORDER BY jel.LineID"

        gvLines.DataSource = DBHelper.ExecuteQuery(sqlLines, New SqlParameter("@id", journalEntryId))
        gvLines.DataBind()
    End Sub

End Class
