Imports System.Data.SqlClient

Partial Public Class Accounting_IncomeStatement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            txtStartDate.Text = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd")
            txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            BindReport()
        End If
    End Sub

    Protected Sub DateFilter_TextChanged(sender As Object, e As EventArgs)
        BindReport()
    End Sub

    ''' <summary>Mengambil total per akun untuk satu AccountType (Revenue/Expense) dalam rentang tanggal.
    ''' Untuk Revenue, "Amount" = Credit - Debit (karena saldo normal Revenue ada di Credit).
    ''' Untuk Expense, "Amount" = Debit - Credit (karena saldo normal Expense ada di Debit).</summary>
    Private Function GetAccountTypeTotals(accountType As String, startDate As DateTime, endDate As DateTime) As DataTable
        Dim amountExpr As String = If(accountType = "Revenue", "SUM(jel.Credit) - SUM(jel.Debit)", "SUM(jel.Debit) - SUM(jel.Credit)")

        Dim sql As String =
            "SELECT coa.AccountName, " & amountExpr & " AS Amount " &
            "FROM dbo.ChartOfAccounts coa " &
            "INNER JOIN dbo.JournalEntryLines jel ON jel.AccountID = coa.AccountID " &
            "INNER JOIN dbo.JournalEntries je ON jel.JournalEntryID = je.JournalEntryID " &
            "WHERE coa.AccountType = @AccountType AND je.EntryDate >= @StartDate AND je.EntryDate <= @EndDate " &
            "GROUP BY coa.AccountName " &
            "HAVING " & amountExpr & " <> 0 " &
            "ORDER BY coa.AccountName"

        Return DBHelper.ExecuteQuery(sql,
            New SqlParameter("@AccountType", accountType),
            New SqlParameter("@StartDate", startDate),
            New SqlParameter("@EndDate", endDate.AddDays(1).AddSeconds(-1))) ' sampai akhir hari endDate
    End Function

    Private Sub BindReport()
        Dim startDate As DateTime = If(txtStartDate.Text = "", DateTime.Now, Convert.ToDateTime(txtStartDate.Text))
        Dim endDate As DateTime = If(txtEndDate.Text = "", DateTime.Now, Convert.ToDateTime(txtEndDate.Text))

        Dim dtRevenue = GetAccountTypeTotals("Revenue", startDate, endDate)
        Dim dtExpense = GetAccountTypeTotals("Expense", startDate, endDate)

        gvRevenue.DataSource = dtRevenue
        gvRevenue.DataBind()
        gvExpense.DataSource = dtExpense
        gvExpense.DataBind()

        Dim totalRevenue As Decimal = 0
        For Each row As DataRow In dtRevenue.Rows
            totalRevenue += Convert.ToDecimal(row("Amount"))
        Next

        Dim totalExpense As Decimal = 0
        For Each row As DataRow In dtExpense.Rows
            totalExpense += Convert.ToDecimal(row("Amount"))
        Next

        litTotalRevenue.Text = "Rp " & totalRevenue.ToString("N0")
        litTotalExpense.Text = "Rp " & totalExpense.ToString("N0")

        Dim netIncome As Decimal = totalRevenue - totalExpense
        litNetIncome.Text = "Rp " & netIncome.ToString("N0")
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim startDate As DateTime = If(txtStartDate.Text = "", DateTime.Now, Convert.ToDateTime(txtStartDate.Text))
        Dim endDate As DateTime = If(txtEndDate.Text = "", DateTime.Now, Convert.ToDateTime(txtEndDate.Text))

        Dim dtRevenue = GetAccountTypeTotals("Revenue", startDate, endDate)
        Dim dtExpense = GetAccountTypeTotals("Expense", startDate, endDate)

        Dim exportDt As New DataTable()
        exportDt.Columns.Add("Kategori")
        exportDt.Columns.Add("Akun")
        exportDt.Columns.Add("Jumlah")

        Dim totalRevenue As Decimal = 0
        For Each row As DataRow In dtRevenue.Rows
            exportDt.Rows.Add("Pendapatan", row("AccountName"), Convert.ToDecimal(row("Amount")).ToString("N0"))
            totalRevenue += Convert.ToDecimal(row("Amount"))
        Next
        exportDt.Rows.Add("", "TOTAL PENDAPATAN", totalRevenue.ToString("N0"))

        Dim totalExpense As Decimal = 0
        For Each row As DataRow In dtExpense.Rows
            exportDt.Rows.Add("Beban", row("AccountName"), Convert.ToDecimal(row("Amount")).ToString("N0"))
            totalExpense += Convert.ToDecimal(row("Amount"))
        Next
        exportDt.Rows.Add("", "TOTAL BEBAN", totalExpense.ToString("N0"))
        exportDt.Rows.Add("", "LABA/RUGI BERSIH", (totalRevenue - totalExpense).ToString("N0"))

        CsvHelper.ExportToCsv(Response, exportDt, "LabaRugi_" & DateTime.Now.ToString("yyyyMMdd") & ".csv")
    End Sub

End Class
