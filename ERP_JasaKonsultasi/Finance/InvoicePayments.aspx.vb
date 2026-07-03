Imports System.Data.SqlClient
Imports System.Web.UI.HtmlControls
Imports System.Globalization

Partial Public Class Finance_InvoicePayments
    Inherits System.Web.UI.Page


    Private ReadOnly Property InvoiceId As Integer
        Get
            Return Convert.ToInt32(Request.QueryString("id"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If String.IsNullOrEmpty(Request.QueryString("id")) Then
            Response.Redirect("Invoices.aspx")
            Return
        End If

        If Not IsPostBack Then
            txtPaymentDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            LoadInvoiceSummary()
            BindPaymentsGrid()
        End If
    End Sub

    Private Sub LoadInvoiceSummary()
        Dim sql As String =
            "SELECT i.InvoiceID, i.InvoiceNo, c.CompanyName, i.TotalAmount, i.DueDate, i.Status, " &
            "ISNULL(p.PaidAmount,0) AS PaidAmount " &
            "FROM dbo.Invoices i " &
            "INNER JOIN dbo.Customers c ON i.CustomerID = c.CustomerID " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "WHERE i.InvoiceID = @id"

        Dim dt = DBHelper.ExecuteQuery(sql, New SqlParameter("@id", InvoiceId))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Invoices.aspx")
            Return
        End If

        Dim row = dt.Rows(0)
        Dim total As Decimal = Convert.ToDecimal(row("TotalAmount"))
        Dim paid As Decimal = Convert.ToDecimal(row("PaidAmount"))
        Dim outstanding As Decimal = total - paid
        Dim status As String = row("Status").ToString()
        Dim dueDate As Object = row("DueDate")

        litInvoiceNo.Text = row("InvoiceNo").ToString()
        lnkPrint.HRef = "InvoicePrint.aspx?id=" & InvoiceId
        litCustomer.Text = row("CompanyName").ToString()
        litTotal.Text = "Rp " & total.ToString("N0")
        litPaid.Text = "Rp " & paid.ToString("N0")
        litOutstanding.Text = "Rp " & outstanding.ToString("N0")
        litDueDate.Text = If(dueDate Is DBNull.Value, "-", Convert.ToDateTime(dueDate).ToString("dd-MM-yyyy"))

        Dim displayStatus As String
        If status = "Cancelled" Then
            displayStatus = "Cancelled"
        ElseIf paid >= total Then
            displayStatus = "Paid"
        ElseIf dueDate IsNot DBNull.Value AndAlso Convert.ToDateTime(dueDate) < DateTime.Now.Date AndAlso paid < total Then
            displayStatus = "Overdue"
        ElseIf paid > 0 Then
            displayStatus = "PartiallyPaid"
        Else
            displayStatus = "Unpaid"
        End If

        lblStatusBadge.InnerText = displayStatus
        lblStatusBadge.Attributes("class") = "badge badge-" & displayStatus.ToLower()
    End Sub

    Private Sub BindPaymentsGrid()
        Dim sql As String = "SELECT PaymentID, PaymentDate, Amount, PaymentMethod, Reference, Notes " &
                             "FROM dbo.Payments WHERE InvoiceID = @id ORDER BY PaymentDate DESC"
        gvPayments.DataSource = DBHelper.ExecuteQuery(sql, New SqlParameter("@id", InvoiceId))
        gvPayments.DataBind()
    End Sub

    Protected Sub btnAddPayment_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim amount As Decimal
        If Not Decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, amount) OrElse amount <= 0 Then
            ShowMessage("Jumlah bayar harus berupa angka lebih dari 0.")
            Return
        End If

        ' Validasi: jangan sampai pembayaran melebihi sisa tagihan
        Dim invoiceData = DBHelper.ExecuteQuery(
            "SELECT i.TotalAmount, ISNULL(p.PaidAmount,0) AS PaidAmount " &
            "FROM dbo.Invoices i " &
            "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
            "WHERE i.InvoiceID = @id", New SqlParameter("@id", InvoiceId))

        Dim total As Decimal = Convert.ToDecimal(invoiceData.Rows(0)("TotalAmount"))
        Dim alreadyPaid As Decimal = Convert.ToDecimal(invoiceData.Rows(0)("PaidAmount"))
        Dim outstanding As Decimal = total - alreadyPaid

        If amount > outstanding Then
            ShowMessage("Jumlah bayar (Rp " & amount.ToString("N0") & ") melebihi sisa tagihan (Rp " & outstanding.ToString("N0") & ").")
            Return
        End If

        Dim paymentDate As DateTime = If(txtPaymentDate.Text = "", DateTime.Now, Convert.ToDateTime(txtPaymentDate.Text))
        Dim invoiceNo = DBHelper.ExecuteScalar("SELECT InvoiceNo FROM dbo.Invoices WHERE InvoiceID = @id", New SqlParameter("@id", InvoiceId))

        ' Akun Debit ditentukan dari metode pembayaran: Cash -> Kas, selain itu -> Bank
        Dim debitAccountCode As String = If(ddlMethod.SelectedValue = "Cash", "1-1000", "1-1100")

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                Dim newPaymentId As Integer
                Using cmdIns As New SqlCommand(
                    "INSERT INTO dbo.Payments (InvoiceID, PaymentDate, Amount, PaymentMethod, Reference, Notes, CreatedBy) " &
                    "VALUES (@InvoiceID, @PaymentDate, @Amount, @Method, @Reference, @Notes, @CreatedBy); SELECT SCOPE_IDENTITY();", conn, tran)
                    cmdIns.Parameters.AddWithValue("@InvoiceID", InvoiceId)
                    cmdIns.Parameters.AddWithValue("@PaymentDate", paymentDate)
                    cmdIns.Parameters.AddWithValue("@Amount", amount)
                    cmdIns.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                    cmdIns.Parameters.AddWithValue("@Reference", If(txtReference.Text.Trim() = "", CType(DBNull.Value, Object), txtReference.Text.Trim()))
                    cmdIns.Parameters.AddWithValue("@Notes", If(txtNotes.Text.Trim() = "", CType(DBNull.Value, Object), txtNotes.Text.Trim()))
                    cmdIns.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                    newPaymentId = Convert.ToInt32(cmdIns.ExecuteScalar())
                End Using

                ' ===== Auto-posting jurnal: Kas/Bank (Debit) vs Piutang Usaha (Credit) =====
                JournalHelper.PostSimpleEntry(conn, tran, paymentDate,
                    "Pembayaran Invoice " & If(invoiceNo IsNot Nothing, invoiceNo.ToString(), ""),
                    "Payment", newPaymentId,
                    debitAccountCode, "1-1200", amount, User.Identity.Name)

                tran.Commit()
            Catch ex As Exception
                tran.Rollback()
                ShowMessage("Gagal mencatat pembayaran: " & ex.Message)
                Return
            End Try
        End Using

        txtAmount.Text = ""
        txtReference.Text = ""
        txtNotes.Text = ""
        litMessage.Text = ""

        LoadInvoiceSummary()
        BindPaymentsGrid()
    End Sub

    Protected Sub lnkDeletePayment_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim paymentId As Integer = Convert.ToInt32(btn.CommandArgument)

        DBHelper.ExecuteNonQuery("DELETE FROM dbo.Payments WHERE PaymentID = @id", New SqlParameter("@id", paymentId))

        LoadInvoiceSummary()
        BindPaymentsGrid()
    End Sub

    Private Sub ShowMessage(message As String)
        litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
    End Sub

End Class
