Imports System.Data.SqlClient

Partial Public Class Reports_StockReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindReport()
        End If
    End Sub

    Private Function GetStockTable() As DataTable
        Dim sql As String =
            "SELECT i.ItemCode, i.ItemName, w.WarehouseName, sb.QtyOnHand, i.ReorderLevel, " &
            "sb.QtyOnHand * i.UnitPrice AS NilaiPersediaan " &
            "FROM dbo.StockBalances sb " &
            "INNER JOIN dbo.Items i ON sb.ItemID = i.ItemID " &
            "INNER JOIN dbo.Warehouses w ON sb.WarehouseID = w.WarehouseID " &
            "WHERE i.IsActive = 1 " &
            "ORDER BY i.ItemName, w.WarehouseName"
        Return DBHelper.ExecuteQuery(sql)
    End Function

    Private Sub BindReport()
        Dim dt = GetStockTable()
        gvStock.DataSource = dt
        gvStock.DataBind()

        Dim totalValue As Decimal = 0
        Dim lowStockCount As Integer = 0
        For Each row As DataRow In dt.Rows
            totalValue += Convert.ToDecimal(row("NilaiPersediaan"))
            Dim reorderLevel = Convert.ToDecimal(row("ReorderLevel"))
            If reorderLevel > 0 AndAlso Convert.ToDecimal(row("QtyOnHand")) <= reorderLevel Then
                lowStockCount += 1
            End If
        Next

        litTotalValue.Text = "Rp " & totalValue.ToString("N0")
        litLowStockCount.Text = lowStockCount.ToString()
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim dt = GetStockTable()
        Dim exportDt As New DataTable()
        exportDt.Columns.Add("Kode Item")
        exportDt.Columns.Add("Nama Item")
        exportDt.Columns.Add("Gudang")
        exportDt.Columns.Add("Saldo")
        exportDt.Columns.Add("Batas Minimal")
        exportDt.Columns.Add("Nilai Persediaan")

        For Each row As DataRow In dt.Rows
            exportDt.Rows.Add(row("ItemCode"), row("ItemName"), row("WarehouseName"),
                row("QtyOnHand"), row("ReorderLevel"), Convert.ToDecimal(row("NilaiPersediaan")).ToString("N0"))
        Next

        CsvHelper.ExportToCsv(Response, exportDt, "LaporanStok_" & DateTime.Now.ToString("yyyyMMdd") & ".csv")
    End Sub

End Class
