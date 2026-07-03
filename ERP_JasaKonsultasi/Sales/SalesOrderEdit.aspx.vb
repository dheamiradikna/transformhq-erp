Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web.UI.HtmlControls

' Kelas sederhana untuk menyimpan 1 baris item pesanan di ViewState
' selama user masih mengedit form (sebelum disimpan ke database).
<Serializable()>
Public Class OrderLineItem
    Public Property ItemID As Integer
    Public Property ItemName As String
    Public Property ItemType As String   ' "Goods" atau "Service" - dipakai untuk auto stock movement
    Public Property Quantity As Decimal
    Public Property UnitPrice As Decimal
    Public ReadOnly Property LineTotal As Decimal
        Get
            Return Quantity * UnitPrice
        End Get
    End Property
End Class

Partial Public Class Sales_SalesOrderEdit
    Inherits System.Web.UI.Page


    ' Status order SEBELUM disimpan (dipakai untuk deteksi transisi status,
    ' misalnya Draft -> Confirmed, supaya auto stock movement hanya jalan SEKALI).
    Private _oldStatus As String = ""

    ''' <summary>True kalau order sudah Confirmed/Completed - item tidak boleh diubah lagi.
    ''' Dipakai juga di markup (.aspx) lewat data-binding.</summary>
    Protected ReadOnly Property IsLocked As Boolean
        Get
            Return _oldStatus = "Confirmed" OrElse _oldStatus = "Completed"
        End Get
    End Property

    Private ReadOnly Property SalesOrderId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    ' Daftar baris item disimpan di ViewState supaya tetap ada selama postback
    ' (saat user tambah/hapus baris) tanpa perlu ke database dulu.
    Private Property OrderLines As List(Of OrderLineItem)
        Get
            If ViewState("OrderLines") Is Nothing Then
                ViewState("OrderLines") = New List(Of OrderLineItem)()
            End If
            Return CType(ViewState("OrderLines"), List(Of OrderLineItem))
        End Get
        Set(value As List(Of OrderLineItem))
            ViewState("OrderLines") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' _oldStatus diambil ulang di SETIAP request (termasuk postback Add/Hapus baris),
        ' supaya status terkunci tetap konsisten saat GridView di-render ulang.
        If SalesOrderId.HasValue Then
            Dim statusResult = DBHelper.ExecuteScalar("SELECT Status FROM dbo.SalesOrders WHERE SalesOrderID = @id",
                New SqlParameter("@id", SalesOrderId.Value))
            If statusResult IsNot Nothing Then _oldStatus = statusResult.ToString()
        End If

        If Not IsPostBack Then
            BindCustomerDropdown()
            BindItemDropdown()

            If SalesOrderId.HasValue Then
                litFormTitle.Text = "Edit Sales Order"
                LoadSalesOrder(SalesOrderId.Value)
            Else
                litFormTitle.Text = "Sales Order Baru"
                OrderLines = New List(Of OrderLineItem)()
            End If

            BindLinesGrid()
        End If

        ' Terapkan status terkunci ke kontrol UI (dijalankan di setiap request,
        ' termasuk postback, supaya tetap konsisten setelah Add/Hapus baris).
        pnlAddLine.Visible = Not IsLocked
        pLockedNotice.Visible = IsLocked
        ddlCustomer.Enabled = Not IsLocked
    End Sub

    Private Sub BindCustomerDropdown()
        Dim dt = DBHelper.ExecuteQuery("SELECT CustomerID, CompanyName FROM dbo.Customers ORDER BY CompanyName")
        ddlCustomer.DataSource = dt
        ddlCustomer.DataTextField = "CompanyName"
        ddlCustomer.DataValueField = "CustomerID"
        ddlCustomer.DataBind()
    End Sub

    Private Sub BindItemDropdown()
        Dim dt = DBHelper.ExecuteQuery("SELECT ItemID, ItemName, UnitPrice, ItemType FROM dbo.Items WHERE IsActive = 1 ORDER BY ItemName")
        ddlItem.DataSource = dt
        ddlItem.DataTextField = "ItemName"
        ddlItem.DataValueField = "ItemID"
        ddlItem.DataBind()

        If dt.Rows.Count > 0 Then
            txtUnitPrice.Text = Convert.ToDecimal(dt.Rows(0)("UnitPrice")).ToString(CultureInfo.InvariantCulture)
        End If
    End Sub

    Protected Sub ddlItem_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim itemId As Integer = Convert.ToInt32(ddlItem.SelectedValue)
        Dim price = DBHelper.ExecuteScalar("SELECT UnitPrice FROM dbo.Items WHERE ItemID = @id", New SqlParameter("@id", itemId))
        If price IsNot Nothing Then
            txtUnitPrice.Text = Convert.ToDecimal(price).ToString(CultureInfo.InvariantCulture)
        End If
    End Sub

    Private Sub LoadSalesOrder(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.SalesOrders WHERE SalesOrderID = @id", New SqlParameter("@id", id))
        If dt.Rows.Count = 0 Then
            Response.Redirect("SalesOrders.aspx")
            Return
        End If
        Dim row = dt.Rows(0)
        lblOrderNo.Text = row("OrderNo").ToString()
        ddlCustomer.SelectedValue = row("CustomerID").ToString()
        ddlStatus.SelectedValue = row("Status").ToString()
        txtNotes.Text = row("Notes").ToString()

        Dim dtLines = DBHelper.ExecuteQuery(
            "SELECT soi.ItemID, i.ItemName, i.ItemType, soi.Quantity, soi.UnitPrice " &
            "FROM dbo.SalesOrderItems soi INNER JOIN dbo.Items i ON soi.ItemID = i.ItemID " &
            "WHERE soi.SalesOrderID = @id", New SqlParameter("@id", id))

        Dim lines As New List(Of OrderLineItem)()
        For Each r As DataRow In dtLines.Rows
            lines.Add(New OrderLineItem With {
                .ItemID = Convert.ToInt32(r("ItemID")),
                .ItemName = r("ItemName").ToString(),
                .ItemType = r("ItemType").ToString(),
                .Quantity = Convert.ToDecimal(r("Quantity")),
                .UnitPrice = Convert.ToDecimal(r("UnitPrice"))
            })
        Next
        OrderLines = lines
    End Sub

    Protected Sub btnAddLine_Click(sender As Object, e As EventArgs)
        If IsLocked Then
            ShowMessage("Pesanan ini sudah Confirmed/Completed, item tidak bisa ditambah lagi.")
            Return
        End If

        If ddlItem.Items.Count = 0 Then
            ShowMessage("Tambahkan minimal 1 item di Master Barang/Jasa terlebih dahulu.")
            Return
        End If

        Dim qty As Decimal
        Dim price As Decimal
        If Not Decimal.TryParse(txtQty.Text, qty) OrElse qty <= 0 Then
            ShowMessage("Qty harus berupa angka lebih dari 0.")
            Return
        End If
        If Not Decimal.TryParse(txtUnitPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, price) OrElse price < 0 Then
            ShowMessage("Harga satuan tidak valid.")
            Return
        End If

        Dim lines = OrderLines
        Dim itemType = DBHelper.ExecuteScalar("SELECT ItemType FROM dbo.Items WHERE ItemID = @id",
            New SqlParameter("@id", Convert.ToInt32(ddlItem.SelectedValue)))

        lines.Add(New OrderLineItem With {
            .ItemID = Convert.ToInt32(ddlItem.SelectedValue),
            .ItemName = ddlItem.SelectedItem.Text,
            .ItemType = If(itemType IsNot Nothing, itemType.ToString(), "Goods"),
            .Quantity = qty,
            .UnitPrice = price
        })
        OrderLines = lines

        txtQty.Text = "1"
        litMessage.Text = ""
        BindLinesGrid()
    End Sub

    Protected Sub lnkRemoveLine_Click(sender As Object, e As EventArgs)
        If IsLocked Then
            ShowMessage("Pesanan ini sudah Confirmed/Completed, item tidak bisa dihapus lagi.")
            Return
        End If

        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim index As Integer = Convert.ToInt32(btn.CommandArgument)

        Dim lines = OrderLines
        If index >= 0 AndAlso index < lines.Count Then
            lines.RemoveAt(index)
        End If
        OrderLines = lines

        BindLinesGrid()
    End Sub

    Private Sub BindLinesGrid()
        Dim lines = OrderLines
        gvLines.DataSource = lines
        gvLines.DataBind()

        Dim total As Decimal = 0
        For Each l As OrderLineItem In lines
            total += l.LineTotal
        Next
        litGrandTotal.Text = "Rp " & total.ToString("N0", New CultureInfo("id-ID"))
    End Sub

    Private Sub ShowMessage(message As String)
        litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim lines = OrderLines
        If lines.Count = 0 Then
            ShowMessage("Tambahkan minimal 1 item sebelum menyimpan Sales Order.")
            Return
        End If

        Dim customerId As Integer = Convert.ToInt32(ddlCustomer.SelectedValue)
        Dim newStatus As String = ddlStatus.SelectedValue
        Dim notes As String = txtNotes.Text.Trim()
        Dim total As Decimal = 0
        For Each l As OrderLineItem In lines
            total += l.LineTotal
        Next

        ' ===== Deteksi transisi status untuk auto stock movement =====
        ' Draft/Cancelled -> Confirmed : stok barang (Goods) otomatis DIKURANGI (OUT)
        ' Confirmed/Completed -> Cancelled : stok barang otomatis DIKEMBALIKAN (IN)
        Dim needsStockOut As Boolean = (_oldStatus <> "Confirmed" AndAlso _oldStatus <> "Completed") AndAlso (newStatus = "Confirmed")
        Dim needsStockReturn As Boolean = (_oldStatus = "Confirmed" OrElse _oldStatus = "Completed") AndAlso (newStatus = "Cancelled")
        Dim goodsLines = lines.Where(Function(l) l.ItemType = "Goods").ToList()

        Dim defaultWarehouseId As Integer? = Nothing
        If (needsStockOut OrElse needsStockReturn) AndAlso goodsLines.Count > 0 Then
            Dim whResult = DBHelper.ExecuteScalar("SELECT TOP 1 WarehouseID FROM dbo.Warehouses WHERE IsActive = 1 ORDER BY WarehouseID")
            If whResult Is Nothing Then
                ShowMessage("Tidak ada gudang aktif. Buat minimal 1 gudang dulu di menu Inventori sebelum men-Confirm pesanan yang berisi barang.")
                Return
            End If
            defaultWarehouseId = Convert.ToInt32(whResult)
        End If

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                ' ===== Validasi stok TERSEDIA sebelum Confirm (supaya tidak Confirm pesanan yang stoknya tidak cukup) =====
                If needsStockOut AndAlso goodsLines.Count > 0 Then
                    For Each line In goodsLines
                        Dim currentQty As Decimal = 0
                        Using cmdCheck As New SqlCommand(
                            "SELECT QtyOnHand FROM dbo.StockBalances WHERE ItemID=@ItemID AND WarehouseID=@WarehouseID", conn, tran)
                            cmdCheck.Parameters.AddWithValue("@ItemID", line.ItemID)
                            cmdCheck.Parameters.AddWithValue("@WarehouseID", defaultWarehouseId.Value)
                            Dim result = cmdCheck.ExecuteScalar()
                            If result IsNot Nothing Then currentQty = Convert.ToDecimal(result)
                        End Using

                        If line.Quantity > currentQty Then
                            tran.Rollback()
                            ShowMessage("Stok '" & line.ItemName & "' tidak cukup untuk Confirm (saldo saat ini: " & currentQty.ToString("N0") & ", dibutuhkan: " & line.Quantity.ToString("N0") & ").")
                            Return
                        End If
                    Next
                End If

                Dim soId As Integer

                If SalesOrderId.HasValue Then
                    soId = SalesOrderId.Value
                    Using cmd As New SqlCommand(
                        "UPDATE dbo.SalesOrders SET CustomerID=@CustomerID, Status=@Status, Notes=@Notes, TotalAmount=@Total WHERE SalesOrderID=@id", conn, tran)
                        cmd.Parameters.AddWithValue("@CustomerID", customerId)
                        cmd.Parameters.AddWithValue("@Status", newStatus)
                        cmd.Parameters.AddWithValue("@Notes", If(notes = "", CType(DBNull.Value, Object), notes))
                        cmd.Parameters.AddWithValue("@Total", total)
                        cmd.Parameters.AddWithValue("@id", soId)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmdDel As New SqlCommand("DELETE FROM dbo.SalesOrderItems WHERE SalesOrderID=@id", conn, tran)
                        cmdDel.Parameters.AddWithValue("@id", soId)
                        cmdDel.ExecuteNonQuery()
                    End Using
                Else
                    ' Generate nomor SO otomatis: SO-{tahun}-{urutan}
                    Dim currentYear As Integer = DateTime.Now.Year
                    Dim countThisYear As Integer
                    Using cmdCount As New SqlCommand("SELECT COUNT(*) FROM dbo.SalesOrders WHERE YEAR(OrderDate) = @Year", conn, tran)
                        cmdCount.Parameters.AddWithValue("@Year", currentYear)
                        countThisYear = Convert.ToInt32(cmdCount.ExecuteScalar())
                    End Using
                    Dim newOrderNo As String = "SO-" & currentYear & "-" & (countThisYear + 1).ToString("D4")

                    Using cmdIns As New SqlCommand(
                        "INSERT INTO dbo.SalesOrders (OrderNo, CustomerID, Status, TotalAmount, Notes, CreatedBy) " &
                        "VALUES (@OrderNo, @CustomerID, @Status, @Total, @Notes, @CreatedBy); SELECT SCOPE_IDENTITY();", conn, tran)
                        cmdIns.Parameters.AddWithValue("@OrderNo", newOrderNo)
                        cmdIns.Parameters.AddWithValue("@CustomerID", customerId)
                        cmdIns.Parameters.AddWithValue("@Status", newStatus)
                        cmdIns.Parameters.AddWithValue("@Total", total)
                        cmdIns.Parameters.AddWithValue("@Notes", If(notes = "", CType(DBNull.Value, Object), notes))
                        cmdIns.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                        soId = Convert.ToInt32(cmdIns.ExecuteScalar())
                    End Using
                End If

                For Each line As OrderLineItem In lines
                    Using cmdLine As New SqlCommand(
                        "INSERT INTO dbo.SalesOrderItems (SalesOrderID, ItemID, Quantity, UnitPrice, LineTotal) " &
                        "VALUES (@SalesOrderID, @ItemID, @Quantity, @UnitPrice, @LineTotal)", conn, tran)
                        cmdLine.Parameters.AddWithValue("@SalesOrderID", soId)
                        cmdLine.Parameters.AddWithValue("@ItemID", line.ItemID)
                        cmdLine.Parameters.AddWithValue("@Quantity", line.Quantity)
                        cmdLine.Parameters.AddWithValue("@UnitPrice", line.UnitPrice)
                        cmdLine.Parameters.AddWithValue("@LineTotal", line.LineTotal)
                        cmdLine.ExecuteNonQuery()
                    End Using
                Next

                ' ===== Eksekusi auto stock movement (OUT saat Confirm, IN saat Cancel-setelah-Confirm) =====
                If (needsStockOut OrElse needsStockReturn) AndAlso goodsLines.Count > 0 Then
                    Dim movementType As String = If(needsStockOut, "OUT", "IN")
                    Dim referenceText As String = "Auto dari SO #" & soId

                    For Each line In goodsLines
                        Dim delta As Decimal = If(movementType = "IN", line.Quantity, -line.Quantity)

                        Using cmdMove As New SqlCommand(
                            "INSERT INTO dbo.StockMovements (ItemID, WarehouseID, MovementType, Quantity, Reference, Notes, CreatedBy) " &
                            "VALUES (@ItemID, @WarehouseID, @MovementType, @Quantity, @Reference, @Notes, @CreatedBy)", conn, tran)
                            cmdMove.Parameters.AddWithValue("@ItemID", line.ItemID)
                            cmdMove.Parameters.AddWithValue("@WarehouseID", defaultWarehouseId.Value)
                            cmdMove.Parameters.AddWithValue("@MovementType", movementType)
                            cmdMove.Parameters.AddWithValue("@Quantity", line.Quantity)
                            cmdMove.Parameters.AddWithValue("@Reference", referenceText)
                            cmdMove.Parameters.AddWithValue("@Notes", If(needsStockOut, "Otomatis: SO dikonfirmasi", "Otomatis: SO dibatalkan, stok dikembalikan"))
                            cmdMove.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                            cmdMove.ExecuteNonQuery()
                        End Using

                        Using cmdUpsert As New SqlCommand(
                            "MERGE dbo.StockBalances AS target " &
                            "USING (SELECT @ItemID AS ItemID, @WarehouseID AS WarehouseID) AS src " &
                            "ON target.ItemID = src.ItemID AND target.WarehouseID = src.WarehouseID " &
                            "WHEN MATCHED THEN UPDATE SET QtyOnHand = target.QtyOnHand + @Delta " &
                            "WHEN NOT MATCHED THEN INSERT (ItemID, WarehouseID, QtyOnHand) VALUES (@ItemID, @WarehouseID, @Delta);", conn, tran)
                            cmdUpsert.Parameters.AddWithValue("@ItemID", line.ItemID)
                            cmdUpsert.Parameters.AddWithValue("@WarehouseID", defaultWarehouseId.Value)
                            cmdUpsert.Parameters.AddWithValue("@Delta", delta)
                            cmdUpsert.ExecuteNonQuery()
                        End Using
                    Next
                End If

                tran.Commit()
            Catch ex As Exception
                tran.Rollback()
                ShowMessage("Gagal menyimpan: " & ex.Message)
                Return
            End Try
        End Using

        Response.Redirect("SalesOrders.aspx")
    End Sub

End Class
