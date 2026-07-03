Imports System.Data.SqlClient

Partial Public Class Finance_InvoicePrint
    Inherits System.Web.UI.Page

    ' Lihat catatan di PayslipPrint.aspx.vb - pola yang sama dipakai di sini:
    ' Public Property biasa (bukan kontrol server), supaya tidak butuh .designer.vb.

    Public Property InvoiceNoText As String = ""
    Public Property CustomerNameText As String = ""
    Public Property InvoiceDateText As String = ""
    Public Property DueDateText As String = ""
    Public Property TotalText As String = ""
    Public Property PaidText As String = ""
    Public Property OutstandingText As String = ""
    Public Property StatusText As String = ""
    Public Property NotesText As String = ""

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If String.IsNullOrEmpty(Request.QueryString("id")) Then
            Response.Redirect("Invoices.aspx")
            Return
        End If

        Dim invoiceId As Integer = Convert.ToInt32(Request.QueryString("id"))

        Dim sql As String =
            "SELECT i.InvoiceNo, c.CompanyName, i.InvoiceDate, i.DueDate, i.TotalAmount, i.Status, i.Notes, " &
            "ISNULL(p.PaidAmount,0) AS PaidAmount " &
            "FROM dbo.Invoices i " &
            "INNER JOIN dbo.Customers c ON i.CustomerID = c.CustomerID " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "WHERE i.InvoiceID = @id"

        Dim dt = DBHelper.ExecuteQuery(sql, New SqlParameter("@id", invoiceId))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Invoices.aspx")
            Return
        End If

        Dim row = dt.Rows(0)
        Dim total As Decimal = Convert.ToDecimal(row("TotalAmount"))
        Dim paid As Decimal = Convert.ToDecimal(row("PaidAmount"))

        InvoiceNoText = row("InvoiceNo").ToString()
        CustomerNameText = row("CompanyName").ToString()
        InvoiceDateText = Convert.ToDateTime(row("InvoiceDate")).ToString("dd MMMM yyyy", New Globalization.CultureInfo("id-ID"))
        DueDateText = If(row("DueDate") Is DBNull.Value, "-", Convert.ToDateTime(row("DueDate")).ToString("dd MMMM yyyy", New Globalization.CultureInfo("id-ID")))
        TotalText = "Rp " & total.ToString("N0")
        PaidText = "Rp " & paid.ToString("N0")
        OutstandingText = "Rp " & (total - paid).ToString("N0")
        StatusText = row("Status").ToString()
        NotesText = If(row("Notes") Is DBNull.Value, "-", row("Notes").ToString())
    End Sub

End Class
