Imports System.Data.SqlClient

Partial Public Class Reports_SalesReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            txtStartDate.Text = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd")
            txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            BindReport()
        End If
    End Sub

    Protected Sub Filter_Changed(sender As Object, e As EventArgs)
        BindReport()
    End Sub

    Private Function GetByCustomerTable(startDate As DateTime, endDate As DateTime) As DataTable
        Dim sql As String =
            "SELECT c.CompanyName, COUNT(*) AS JumlahOrder, SUM(so.TotalAmount) AS TotalPenjualan " &
            "FROM dbo.SalesOrders so " &
            "INNER JOIN dbo.Customers c ON so.CustomerID = c.CustomerID " &
            "WHERE so.Status IN ('Confirmed','Completed') AND so.OrderDate >= @StartDate AND so.OrderDate <= @EndDate " &
            "GROUP BY c.CompanyName ORDER BY TotalPenjualan DESC"
        Return DBHelper.ExecuteQuery(sql,
            New SqlParameter("@StartDate", startDate), New SqlParameter("@EndDate", endDate.AddDays(1).AddSeconds(-1)))
    End Function

    Private Function GetByItemTable(startDate As DateTime, endDate As DateTime) As DataTable
        Dim sql As String =
            "SELECT i.ItemName, SUM(soi.Quantity) AS TotalQty, SUM(soi.LineTotal) AS TotalPenjualan " &
            "FROM dbo.SalesOrderItems soi " &
            "INNER JOIN dbo.SalesOrders so ON soi.SalesOrderID = so.SalesOrderID " &
            "INNER JOIN dbo.Items i ON soi.ItemID = i.ItemID " &
            "WHERE so.Status IN ('Confirmed','Completed') AND so.OrderDate >= @StartDate AND so.OrderDate <= @EndDate " &
            "GROUP BY i.ItemName ORDER BY TotalPenjualan DESC"
        Return DBHelper.ExecuteQuery(sql,
            New SqlParameter("@StartDate", startDate), New SqlParameter("@EndDate", endDate.AddDays(1).AddSeconds(-1)))
    End Function

    Private Sub BindReport()
        Dim startDate As DateTime = If(txtStartDate.Text = "", DateTime.Now, Convert.ToDateTime(txtStartDate.Text))
        Dim endDate As DateTime = If(txtEndDate.Text = "", DateTime.Now, Convert.ToDateTime(txtEndDate.Text))

        Dim dtCustomer = GetByCustomerTable(startDate, endDate)
        Dim dtItem = GetByItemTable(startDate, endDate)

        gvByCustomer.DataSource = dtCustomer
        gvByCustomer.DataBind()
        gvByItem.DataSource = dtItem
        gvByItem.DataBind()

        Dim totalSales As Decimal = 0
        Dim orderCount As Integer = 0
        For Each row As DataRow In dtCustomer.Rows
            totalSales += Convert.ToDecimal(row("TotalPenjualan"))
            orderCount += Convert.ToInt32(row("JumlahOrder"))
        Next

        litTotalSales.Text = "Rp " & totalSales.ToString("N0")
        litOrderCount.Text = orderCount.ToString()
        litAvgOrder.Text = "Rp " & If(orderCount > 0, (totalSales / orderCount).ToString("N0"), "0")
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim startDate As DateTime = If(txtStartDate.Text = "", DateTime.Now, Convert.ToDateTime(txtStartDate.Text))
        Dim endDate As DateTime = If(txtEndDate.Text = "", DateTime.Now, Convert.ToDateTime(txtEndDate.Text))
        Dim dtCustomer = GetByCustomerTable(startDate, endDate)

        Dim exportDt As New DataTable()
        exportDt.Columns.Add("Customer")
        exportDt.Columns.Add("Jumlah Order")
        exportDt.Columns.Add("Total Penjualan")

        For Each row As DataRow In dtCustomer.Rows
            exportDt.Rows.Add(row("CompanyName"), row("JumlahOrder"), Convert.ToDecimal(row("TotalPenjualan")).ToString("N0"))
        Next

        CsvHelper.ExportToCsv(Response, exportDt, "LaporanPenjualan_" & DateTime.Now.ToString("yyyyMMdd") & ".csv")
    End Sub

End Class
