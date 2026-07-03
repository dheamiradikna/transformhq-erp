Imports System.Data.SqlClient

Partial Public Class Accounting_TrialBalance
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            txtAsOfDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            BindGrid()
        End If
    End Sub

    Protected Sub txtAsOfDate_TextChanged(sender As Object, e As EventArgs)
        BindGrid()
    End Sub

    Private Function GetReportDataTable() As DataTable
        Dim asOfDate As DateTime = If(txtAsOfDate.Text = "", DateTime.Now, Convert.ToDateTime(txtAsOfDate.Text))

        Dim sql As String =
            "SELECT coa.AccountCode, coa.AccountName, coa.AccountType, " &
            "ISNULL(SUM(jel.Debit), 0) AS TotalDebit, ISNULL(SUM(jel.Credit), 0) AS TotalCredit " &
            "FROM dbo.ChartOfAccounts coa " &
            "LEFT JOIN dbo.JournalEntryLines jel ON jel.AccountID = coa.AccountID " &
            "LEFT JOIN dbo.JournalEntries je ON jel.JournalEntryID = je.JournalEntryID AND je.EntryDate <= @AsOfDate " &
            "WHERE coa.IsActive = 1 " &
            "GROUP BY coa.AccountCode, coa.AccountName, coa.AccountType " &
            "HAVING ISNULL(SUM(jel.Debit), 0) <> 0 OR ISNULL(SUM(jel.Credit), 0) <> 0 " &
            "ORDER BY coa.AccountCode"

        Return DBHelper.ExecuteQuery(sql, New SqlParameter("@AsOfDate", asOfDate))
    End Function

    Private Sub BindGrid()
        Dim dt = GetReportDataTable()
        gvTrialBalance.DataSource = dt
        gvTrialBalance.DataBind()

        Dim totalDebit As Decimal = 0
        Dim totalCredit As Decimal = 0
        For Each row As DataRow In dt.Rows
            totalDebit += Convert.ToDecimal(row("TotalDebit"))
            totalCredit += Convert.ToDecimal(row("TotalCredit"))
        Next

        litGrandTotalDebit.Text = "Rp " & totalDebit.ToString("N0")
        litGrandTotalCredit.Text = "Rp " & totalCredit.ToString("N0")

        If totalDebit = totalCredit Then
            litBalanceCheck.Text = "<span class='badge badge-active'>Balance &#10003;</span>"
        Else
            litBalanceCheck.Text = "<span class='badge badge-overdue'>Tidak Balance!</span>"
        End If
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim dt = GetReportDataTable()
        Dim exportDt As New DataTable()
        exportDt.Columns.Add("Kode Akun")
        exportDt.Columns.Add("Nama Akun")
        exportDt.Columns.Add("Tipe")
        exportDt.Columns.Add("Debit")
        exportDt.Columns.Add("Credit")

        For Each row As DataRow In dt.Rows
            exportDt.Rows.Add(row("AccountCode"), row("AccountName"), row("AccountType"),
                Convert.ToDecimal(row("TotalDebit")).ToString("N0"), Convert.ToDecimal(row("TotalCredit")).ToString("N0"))
        Next

        CsvHelper.ExportToCsv(Response, exportDt, "NeracaSaldo_" & DateTime.Now.ToString("yyyyMMdd") & ".csv")
    End Sub

End Class
