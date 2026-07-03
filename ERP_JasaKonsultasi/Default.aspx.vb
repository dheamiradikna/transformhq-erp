Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class Default_Dashboard
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadKPIs()
            LoadRecentSalesOrders()
            LoadActiveProjects()
            LoadCharts()
        End If
    End Sub

    Private Sub LoadKPIs()
        litTotalCustomers.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.Customers").ToString()

        litTotalLeads.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.Customers WHERE Status = 'Lead'").ToString()

        litSOThisMonth.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.SalesOrders WHERE MONTH(OrderDate) = MONTH(GETDATE()) AND YEAR(OrderDate) = YEAR(GETDATE())").ToString()

        litActiveProjects.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.Projects WHERE Status IN ('Planning','InProgress')").ToString()

        litLowStock.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.StockBalances sb " &
            "INNER JOIN dbo.Items i ON sb.ItemID = i.ItemID " &
            "WHERE sb.QtyOnHand <= i.ReorderLevel AND i.ReorderLevel > 0").ToString()

        Dim outstanding = DBHelper.ExecuteScalar(
            "SELECT ISNULL(SUM(i.TotalAmount - ISNULL(p.PaidAmount,0)), 0) " &
            "FROM dbo.Invoices i " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "WHERE i.Status <> 'Cancelled' AND i.TotalAmount > ISNULL(p.PaidAmount,0)")
        litOutstandingInvoice.Text = "Rp " & Convert.ToDecimal(outstanding).ToString("N0")

        ' Laba bulan ini dari data jurnal (Revenue - Expense bulan berjalan).
        ' Dibungkus Try/Catch supaya Dashboard tetap jalan normal kalau modul Akuntansi
        ' (script 04_Accounting_Module.sql) belum dijalankan oleh user.
        Try
            Dim startOfMonth As New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            Dim netIncomeSql As String =
                "SELECT " &
                "ISNULL(SUM(CASE WHEN coa.AccountType = 'Revenue' THEN jel.Credit - jel.Debit ELSE 0 END), 0) - " &
                "ISNULL(SUM(CASE WHEN coa.AccountType = 'Expense' THEN jel.Debit - jel.Credit ELSE 0 END), 0) AS NetIncome " &
                "FROM dbo.JournalEntryLines jel " &
                "INNER JOIN dbo.ChartOfAccounts coa ON jel.AccountID = coa.AccountID " &
                "INNER JOIN dbo.JournalEntries je ON jel.JournalEntryID = je.JournalEntryID " &
                "WHERE coa.AccountType IN ('Revenue','Expense') AND je.EntryDate >= @StartOfMonth"

            Dim netIncomeResult = DBHelper.ExecuteScalar(netIncomeSql, New SqlParameter("@StartOfMonth", startOfMonth))
            Dim netIncome As Decimal = If(netIncomeResult IsNot Nothing AndAlso netIncomeResult IsNot DBNull.Value, Convert.ToDecimal(netIncomeResult), 0)
            litNetIncomeMonth.Text = "Rp " & netIncome.ToString("N0")
        Catch
            litNetIncomeMonth.Text = "-"
        End Try
    End Sub

    Private Sub LoadRecentSalesOrders()
        Dim sql As String = "SELECT TOP 5 so.OrderNo, c.CompanyName, so.OrderDate, so.Status, so.TotalAmount " &
                             "FROM dbo.SalesOrders so " &
                             "INNER JOIN dbo.Customers c ON so.CustomerID = c.CustomerID " &
                             "ORDER BY so.OrderDate DESC"
        gvRecentSO.DataSource = DBHelper.ExecuteQuery(sql)
        gvRecentSO.DataBind()
    End Sub

    Private Sub LoadActiveProjects()
        Dim sql As String = "SELECT TOP 5 ProjectCode, ProjectName, ProjectManager, Status, EndDate " &
                             "FROM dbo.Projects " &
                             "WHERE Status IN ('Planning','InProgress') " &
                             "ORDER BY EndDate ASC"
        gvRecentProjects.DataSource = DBHelper.ExecuteQuery(sql)
        gvRecentProjects.DataBind()
    End Sub

    Private Sub LoadCharts()
        ' ===== 1) Tren Penjualan 6 bulan terakhir (SO dengan status Confirmed/Completed) =====
        Dim monthLabels As New List(Of String)
        Dim monthTotals As New List(Of Decimal)
        Dim cultureID As New Globalization.CultureInfo("id-ID")

        For i As Integer = 5 To 0 Step -1
            Dim targetDate As DateTime = DateTime.Now.AddMonths(-i)
            monthLabels.Add(targetDate.ToString("MMM yyyy", cultureID))

            Dim total = DBHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(TotalAmount),0) FROM dbo.SalesOrders " &
                "WHERE Status IN ('Confirmed','Completed') AND MONTH(OrderDate) = @Month AND YEAR(OrderDate) = @Year",
                New SqlParameter("@Month", targetDate.Month), New SqlParameter("@Year", targetDate.Year))
            monthTotals.Add(Convert.ToDecimal(total))
        Next

        ' ===== 2) Breakdown status invoice (logika sama dengan halaman Invoices) =====
        Dim statusCaseSql As String =
            "CASE " &
            "  WHEN i.Status = 'Cancelled' THEN 'Cancelled' " &
            "  WHEN ISNULL(p.PaidAmount,0) >= i.TotalAmount THEN 'Paid' " &
            "  WHEN i.DueDate IS NOT NULL AND i.DueDate < CAST(GETDATE() AS DATE) AND ISNULL(p.PaidAmount,0) < i.TotalAmount THEN 'Overdue' " &
            "  WHEN ISNULL(p.PaidAmount,0) > 0 THEN 'PartiallyPaid' " &
            "  ELSE 'Unpaid' " &
            "END"

        Dim dtStatus = DBHelper.ExecuteQuery(
            "SELECT " & statusCaseSql & " AS DisplayStatus, COUNT(*) AS Jumlah " &
            "FROM dbo.Invoices i " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "GROUP BY " & statusCaseSql)

        Dim statusCounts As New Dictionary(Of String, Integer) From {
            {"Unpaid", 0}, {"PartiallyPaid", 0}, {"Paid", 0}, {"Overdue", 0}, {"Cancelled", 0}
        }
        For Each r As DataRow In dtStatus.Rows
            Dim key = r("DisplayStatus").ToString()
            If statusCounts.ContainsKey(key) Then statusCounts(key) = Convert.ToInt32(r("Jumlah"))
        Next

        ' ===== Render script Chart.js (data digabung langsung sebagai literal JS array - aman karena semua angka/internal, bukan input user) =====
        Dim labelsJs As String = String.Join(",", monthLabels.Select(Function(l) "'" & l & "'"))
        Dim totalsJs As String = String.Join(",", monthTotals.Select(Function(t) t.ToString(Globalization.CultureInfo.InvariantCulture)))

        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script>")
        sb.Append("document.addEventListener('DOMContentLoaded', function () {")

        sb.Append("new Chart(document.getElementById('chartSalesTrend'), { type: 'bar', data: { labels: [")
        sb.Append(labelsJs)
        sb.Append("], datasets: [{ label: 'Total Penjualan', data: [")
        sb.Append(totalsJs)
        sb.Append("], backgroundColor: '#2563eb', borderRadius: 4 }] }, ")
        sb.Append("options: { plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } } });")

        sb.Append("new Chart(document.getElementById('chartInvoiceStatus'), { type: 'doughnut', data: { ")
        sb.Append("labels: ['Unpaid','Partially Paid','Paid','Overdue','Cancelled'], datasets: [{ data: [")
        sb.Append(statusCounts("Unpaid") & "," & statusCounts("PartiallyPaid") & "," & statusCounts("Paid") & "," & statusCounts("Overdue") & "," & statusCounts("Cancelled"))
        sb.Append("], backgroundColor: ['#f59e0b','#3b82f6','#16a34a','#dc2626','#94a3b8'] }] } });")

        sb.Append("});")
        sb.Append("</script>")

        litChartScript.Text = sb.ToString()
    End Sub

End Class
