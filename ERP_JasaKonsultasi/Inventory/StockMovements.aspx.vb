Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data

Partial Public Class Inventory_StockMovements
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindDropdowns()
            BindGrid()
        End If
    End Sub

    Private Sub BindDropdowns()
        Dim dtItems = DBHelper.ExecuteQuery("SELECT ItemID, ItemName FROM dbo.Items WHERE IsActive = 1 AND ItemType = 'Goods' ORDER BY ItemName")
        ddlItem.DataSource = dtItems
        ddlItem.DataTextField = "ItemName"
        ddlItem.DataValueField = "ItemID"
        ddlItem.DataBind()

        Dim dtWarehouses = DBHelper.ExecuteQuery("SELECT WarehouseID, WarehouseName FROM dbo.Warehouses WHERE IsActive = 1 ORDER BY WarehouseName")
        ddlWarehouse.DataSource = dtWarehouses
        ddlWarehouse.DataTextField = "WarehouseName"
        ddlWarehouse.DataValueField = "WarehouseID"
        ddlWarehouse.DataBind()
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT TOP 20 sm.MovementDate, i.ItemName, w.WarehouseName, sm.MovementType, sm.Quantity, sm.Notes " &
                             "FROM dbo.StockMovements sm " &
                             "INNER JOIN dbo.Items i ON sm.ItemID = i.ItemID " &
                             "INNER JOIN dbo.Warehouses w ON sm.WarehouseID = w.WarehouseID " &
                             "ORDER BY sm.MovementDate DESC"
        gvMovements.DataSource = DBHelper.ExecuteQuery(sql)
        gvMovements.DataBind()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return
        If ddlItem.Items.Count = 0 OrElse ddlWarehouse.Items.Count = 0 Then
            ShowMessage("Tambahkan minimal 1 Item (tipe Goods) dan 1 Gudang terlebih dahulu.", True)
            Return
        End If

        Dim itemId As Integer = Convert.ToInt32(ddlItem.SelectedValue)
        Dim warehouseId As Integer = Convert.ToInt32(ddlWarehouse.SelectedValue)
        Dim movementType As String = ddlMovementType.SelectedValue
        Dim qty As Decimal
        If Not Decimal.TryParse(txtQuantity.Text, qty) OrElse qty <= 0 Then
            ShowMessage("Jumlah harus berupa angka lebih dari 0.", True)
            Return
        End If

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                ' 1) Cek saldo stok saat ini untuk validasi OUT & untuk upsert
                Dim currentQty As Decimal = 0
                Using cmdCheck As New SqlCommand(
                    "SELECT QtyOnHand FROM dbo.StockBalances WHERE ItemID=@ItemID AND WarehouseID=@WarehouseID", conn, tran)
                    cmdCheck.Parameters.AddWithValue("@ItemID", itemId)
                    cmdCheck.Parameters.AddWithValue("@WarehouseID", warehouseId)
                    Dim result = cmdCheck.ExecuteScalar()
                    If result IsNot Nothing Then currentQty = Convert.ToDecimal(result)
                End Using

                If movementType = "OUT" AndAlso qty > currentQty Then
                    tran.Rollback()
                    ShowMessage("Stok tidak cukup. Saldo saat ini hanya " & currentQty.ToString("N0") & ".", True)
                    Return
                End If

                ' 2) Insert riwayat mutasi
                Using cmdInsert As New SqlCommand(
                    "INSERT INTO dbo.StockMovements (ItemID, WarehouseID, MovementType, Quantity, Reference, Notes, CreatedBy) " &
                    "VALUES (@ItemID, @WarehouseID, @MovementType, @Quantity, @Reference, @Notes, @CreatedBy)", conn, tran)
                    cmdInsert.Parameters.AddWithValue("@ItemID", itemId)
                    cmdInsert.Parameters.AddWithValue("@WarehouseID", warehouseId)
                    cmdInsert.Parameters.AddWithValue("@MovementType", movementType)
                    cmdInsert.Parameters.AddWithValue("@Quantity", qty)
                    cmdInsert.Parameters.AddWithValue("@Reference", DBNull.Value)
                    cmdInsert.Parameters.AddWithValue("@Notes", If(txtNotes.Text.Trim() = "", CType(DBNull.Value, Object), txtNotes.Text.Trim()))
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                    cmdInsert.ExecuteNonQuery()
                End Using

                ' 3) Upsert StockBalances (tambah jika IN, kurangi jika OUT)
                Dim delta As Decimal = If(movementType = "IN", qty, -qty)

                Using cmdUpsert As New SqlCommand(
                    "MERGE dbo.StockBalances AS target " &
                    "USING (SELECT @ItemID AS ItemID, @WarehouseID AS WarehouseID) AS src " &
                    "ON target.ItemID = src.ItemID AND target.WarehouseID = src.WarehouseID " &
                    "WHEN MATCHED THEN UPDATE SET QtyOnHand = target.QtyOnHand + @Delta " &
                    "WHEN NOT MATCHED THEN INSERT (ItemID, WarehouseID, QtyOnHand) VALUES (@ItemID, @WarehouseID, @Delta);", conn, tran)
                    cmdUpsert.Parameters.AddWithValue("@ItemID", itemId)
                    cmdUpsert.Parameters.AddWithValue("@WarehouseID", warehouseId)
                    cmdUpsert.Parameters.AddWithValue("@Delta", delta)
                    cmdUpsert.ExecuteNonQuery()
                End Using

                tran.Commit()
                ShowMessage("Mutasi stok berhasil disimpan.", False)
                txtQuantity.Text = ""
                txtNotes.Text = ""
                BindGrid()

            Catch ex As Exception
                tran.Rollback()
                ShowMessage("Terjadi kesalahan: " & ex.Message, True)
            End Try
        End Using
    End Sub

    Private Sub ShowMessage(message As String, isError As Boolean)
        Dim cssClass As String = If(isError, "error-msg", "error-msg")
        If Not isError Then
            litMessage.Text = "<div style='background:#dcfce7;color:#166534;padding:10px 12px;border-radius:6px;font-size:13px;margin-bottom:14px;'>" & Server.HtmlEncode(message) & "</div>"
        Else
            litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
        End If
    End Sub

End Class
