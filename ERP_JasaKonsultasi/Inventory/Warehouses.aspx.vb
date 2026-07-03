Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class Inventory_Warehouses
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        gvWarehouses.DataSource = DBHelper.ExecuteQuery(
            "SELECT WarehouseID, WarehouseName, Location FROM dbo.Warehouses WHERE IsActive = 1 ORDER BY WarehouseName")
        gvWarehouses.DataBind()
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        DBHelper.ExecuteNonQuery(
            "INSERT INTO dbo.Warehouses (WarehouseName, Location) VALUES (@Name, @Location)",
            New SqlParameter("@Name", txtWarehouseName.Text.Trim()),
            New SqlParameter("@Location", txtLocation.Text.Trim()))

        txtWarehouseName.Text = ""
        txtLocation.Text = ""
        BindGrid()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim warehouseId As Integer = Convert.ToInt32(btn.CommandArgument)
        DBHelper.ExecuteNonQuery("UPDATE dbo.Warehouses SET IsActive = 0 WHERE WarehouseID = @id",
            New SqlParameter("@id", warehouseId))
        BindGrid()
    End Sub

End Class
