Imports System.Data.SqlClient

Partial Public Class Finance_Invoices
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            LoadKPIs()
            BindGrid()
        End If
    End Sub

    Private Sub LoadKPIs()
        Dim outstanding = DBHelper.ExecuteScalar(
            "SELECT ISNULL(SUM(i.TotalAmount - ISNULL(p.PaidAmount,0)), 0) " &
            "FROM dbo.Invoices i " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "WHERE i.Status <> 'Cancelled' AND i.TotalAmount > ISNULL(p.PaidAmount,0)")
        litTotalOutstanding.Text = "Rp " & Convert.ToDecimal(outstanding).ToString("N0")

        Dim overdueCount = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.Invoices i " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "WHERE i.Status <> 'Cancelled' AND i.DueDate IS NOT NULL AND i.DueDate < CAST(GETDATE() AS DATE) " &
            "AND i.TotalAmount > ISNULL(p.PaidAmount,0)")
        litOverdueCount.Text = overdueCount.ToString()

        Dim totalPaid = DBHelper.ExecuteScalar("SELECT ISNULL(SUM(Amount), 0) FROM dbo.Payments")
        litTotalPaid.Text = "Rp " & Convert.ToDecimal(totalPaid).ToString("N0")
    End Sub

    Private Sub BindGrid()
        Dim sql As String =
            "SELECT i.InvoiceID, i.InvoiceNo, c.CompanyName, i.InvoiceDate, i.DueDate, i.TotalAmount, " &
            "ISNULL(p.PaidAmount,0) AS PaidAmount, " &
            "i.TotalAmount - ISNULL(p.PaidAmount,0) AS Outstanding, " &
            "CASE " &
            "  WHEN i.Status = 'Cancelled' THEN 'Cancelled' " &
            "  WHEN ISNULL(p.PaidAmount,0) >= i.TotalAmount THEN 'Paid' " &
            "  WHEN i.DueDate IS NOT NULL AND i.DueDate < CAST(GETDATE() AS DATE) AND ISNULL(p.PaidAmount,0) < i.TotalAmount THEN 'Overdue' " &
            "  WHEN ISNULL(p.PaidAmount,0) > 0 THEN 'PartiallyPaid' " &
            "  ELSE 'Unpaid' " &
            "END AS DisplayStatus " &
            "FROM dbo.Invoices i " &
            "INNER JOIN dbo.Customers c ON i.CustomerID = c.CustomerID " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "ORDER BY i.InvoiceDate DESC"

        gvInvoices.DataSource = DBHelper.ExecuteQuery(sql)
        gvInvoices.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim invoiceId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Payments akan ikut terhapus otomatis karena FK ON DELETE CASCADE
        DBHelper.ExecuteNonQuery("DELETE FROM dbo.Invoices WHERE InvoiceID = @id",
            New SqlParameter("@id", invoiceId))

        LoadKPIs()
        BindGrid()
    End Sub

End Class
