Imports System.Data.SqlClient
Imports System.Globalization

Partial Public Class Finance_InvoiceEdit
    Inherits System.Web.UI.Page


    Private ReadOnly Property InvoiceId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindDropdowns()

            If InvoiceId.HasValue Then
                litFormTitle.Text = "Edit Invoice"
                LoadInvoice(InvoiceId.Value)
            Else
                litFormTitle.Text = "Invoice Baru"
                txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
                txtDueDate.Text = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd")
            End If
        End If
    End Sub

    Private Sub BindDropdowns()
        Dim dtSO = DBHelper.ExecuteQuery(
            "SELECT SalesOrderID, OrderNo, CustomerID, TotalAmount FROM dbo.SalesOrders ORDER BY OrderDate DESC")
        ddlSalesOrder.DataSource = dtSO
        ddlSalesOrder.DataTextField = "OrderNo"
        ddlSalesOrder.DataValueField = "SalesOrderID"
        ddlSalesOrder.DataBind()
        ddlSalesOrder.Items.Insert(0, New ListItem("-- Tidak ada (invoice manual) --", ""))

        Dim dtCust = DBHelper.ExecuteQuery("SELECT CustomerID, CompanyName FROM dbo.Customers ORDER BY CompanyName")
        ddlCustomer.DataSource = dtCust
        ddlCustomer.DataTextField = "CompanyName"
        ddlCustomer.DataValueField = "CustomerID"
        ddlCustomer.DataBind()
    End Sub

    Protected Sub ddlSalesOrder_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlSalesOrder.SelectedValue = "" Then Return

        Dim soId As Integer = Convert.ToInt32(ddlSalesOrder.SelectedValue)
        Dim dt = DBHelper.ExecuteQuery("SELECT CustomerID, TotalAmount FROM dbo.SalesOrders WHERE SalesOrderID = @id",
            New SqlParameter("@id", soId))

        If dt.Rows.Count > 0 Then
            ddlCustomer.SelectedValue = dt.Rows(0)("CustomerID").ToString()
            txtTotalAmount.Text = Convert.ToDecimal(dt.Rows(0)("TotalAmount")).ToString(CultureInfo.InvariantCulture)
        End If
    End Sub

    Private Sub LoadInvoice(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Invoices WHERE InvoiceID = @id", New SqlParameter("@id", id))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Invoices.aspx")
            Return
        End If
        Dim row = dt.Rows(0)
        lblInvoiceNo.Text = row("InvoiceNo").ToString()
        If row("SalesOrderID") IsNot DBNull.Value Then ddlSalesOrder.SelectedValue = row("SalesOrderID").ToString()
        ddlCustomer.SelectedValue = row("CustomerID").ToString()
        txtTotalAmount.Text = Convert.ToDecimal(row("TotalAmount")).ToString(CultureInfo.InvariantCulture)
        txtInvoiceDate.Text = Convert.ToDateTime(row("InvoiceDate")).ToString("yyyy-MM-dd")
        If row("DueDate") IsNot DBNull.Value Then txtDueDate.Text = Convert.ToDateTime(row("DueDate")).ToString("yyyy-MM-dd")
        ddlStatus.SelectedValue = row("Status").ToString()
        txtNotes.Text = row("Notes").ToString()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return
        If ddlCustomer.Items.Count = 0 Then Return

        Dim salesOrderId As Object = If(ddlSalesOrder.SelectedValue = "", CType(DBNull.Value, Object), Convert.ToInt32(ddlSalesOrder.SelectedValue))
        Dim customerId As Integer = Convert.ToInt32(ddlCustomer.SelectedValue)
        Dim totalAmount As Decimal = 0
        Decimal.TryParse(txtTotalAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, totalAmount)
        Dim invoiceDate As DateTime = If(txtInvoiceDate.Text = "", DateTime.Now, Convert.ToDateTime(txtInvoiceDate.Text))
        Dim dueDate As Object = If(txtDueDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtDueDate.Text))
        Dim notes As String = txtNotes.Text.Trim()

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                If InvoiceId.HasValue Then
                    Using cmd As New SqlCommand(
                        "UPDATE dbo.Invoices SET SalesOrderID=@SalesOrderID, CustomerID=@CustomerID, TotalAmount=@Total, " &
                        "InvoiceDate=@InvoiceDate, DueDate=@DueDate, Status=@Status, Notes=@Notes WHERE InvoiceID=@id", conn, tran)
                        cmd.Parameters.AddWithValue("@SalesOrderID", salesOrderId)
                        cmd.Parameters.AddWithValue("@CustomerID", customerId)
                        cmd.Parameters.AddWithValue("@Total", totalAmount)
                        cmd.Parameters.AddWithValue("@InvoiceDate", invoiceDate)
                        cmd.Parameters.AddWithValue("@DueDate", dueDate)
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        cmd.Parameters.AddWithValue("@Notes", If(notes = "", CType(DBNull.Value, Object), notes))
                        cmd.Parameters.AddWithValue("@id", InvoiceId.Value)
                        cmd.ExecuteNonQuery()
                    End Using
                    ' Catatan: jurnal TIDAK diposting ulang saat edit, hanya saat invoice BARU dibuat,
                    ' supaya tidak terjadi jurnal dobel kalau invoice sekadar dikoreksi datanya.
                Else
                    ' Generate nomor invoice otomatis: INV-{tahun}-{urutan}
                    Dim currentYear As Integer = DateTime.Now.Year
                    Dim countThisYear As Integer
                    Using cmdCount As New SqlCommand("SELECT COUNT(*) FROM dbo.Invoices WHERE YEAR(InvoiceDate) = @Year", conn, tran)
                        cmdCount.Parameters.AddWithValue("@Year", currentYear)
                        countThisYear = Convert.ToInt32(cmdCount.ExecuteScalar())
                    End Using
                    Dim newInvoiceNo As String = "INV-" & currentYear & "-" & (countThisYear + 1).ToString("D4")

                    Dim newInvoiceId As Integer
                    Using cmdIns As New SqlCommand(
                        "INSERT INTO dbo.Invoices (InvoiceNo, SalesOrderID, CustomerID, TotalAmount, InvoiceDate, DueDate, Status, Notes, CreatedBy) " &
                        "VALUES (@InvoiceNo, @SalesOrderID, @CustomerID, @Total, @InvoiceDate, @DueDate, @Status, @Notes, @CreatedBy); SELECT SCOPE_IDENTITY();", conn, tran)
                        cmdIns.Parameters.AddWithValue("@InvoiceNo", newInvoiceNo)
                        cmdIns.Parameters.AddWithValue("@SalesOrderID", salesOrderId)
                        cmdIns.Parameters.AddWithValue("@CustomerID", customerId)
                        cmdIns.Parameters.AddWithValue("@Total", totalAmount)
                        cmdIns.Parameters.AddWithValue("@InvoiceDate", invoiceDate)
                        cmdIns.Parameters.AddWithValue("@DueDate", dueDate)
                        cmdIns.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        cmdIns.Parameters.AddWithValue("@Notes", If(notes = "", CType(DBNull.Value, Object), notes))
                        cmdIns.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                        newInvoiceId = Convert.ToInt32(cmdIns.ExecuteScalar())
                    End Using

                    ' ===== Auto-posting jurnal: Piutang Usaha (Debit) vs Pendapatan Jasa (Credit) =====
                    JournalHelper.PostSimpleEntry(conn, tran, invoiceDate,
                        "Invoice " & newInvoiceNo & " - " & ddlCustomer.SelectedItem.Text,
                        "Invoice", newInvoiceId,
                        "1-1200", "4-1000", totalAmount, User.Identity.Name)
                End If

                tran.Commit()
            Catch ex As Exception
                tran.Rollback()
                litMessage.Text = "<div class='error-msg'>Gagal menyimpan invoice: " & Server.HtmlEncode(ex.Message) & "</div>"
                Return
            End Try
        End Using

        Response.Redirect("Invoices.aspx")
    End Sub

End Class
