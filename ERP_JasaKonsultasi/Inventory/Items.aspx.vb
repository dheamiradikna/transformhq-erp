Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class Inventory_Items
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT i.ItemID, i.ItemCode, i.ItemName, i.ItemType, i.Unit, i.UnitPrice, " &
                             "ISNULL((SELECT SUM(QtyOnHand) FROM dbo.StockBalances sb WHERE sb.ItemID = i.ItemID), 0) AS TotalStock " &
                             "FROM dbo.Items i WHERE i.IsActive = 1 ORDER BY i.ItemName"
        gvItems.DataSource = DBHelper.ExecuteQuery(sql)
        gvItems.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim itemId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Soft-delete: set IsActive = 0 supaya histori transaksi (SalesOrderItems, StockMovements) tetap valid
        DBHelper.ExecuteNonQuery("UPDATE dbo.Items SET IsActive = 0 WHERE ItemID = @id",
            New SqlParameter("@id", itemId))

        BindGrid()
    End Sub

End Class
