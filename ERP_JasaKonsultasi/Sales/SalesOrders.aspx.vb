Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class Sales_SalesOrders
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT so.SalesOrderID, so.OrderNo, c.CompanyName, so.OrderDate, so.Status, so.TotalAmount " &
                             "FROM dbo.SalesOrders so " &
                             "INNER JOIN dbo.Customers c ON so.CustomerID = c.CustomerID " &
                             "ORDER BY so.OrderDate DESC"
        gvSalesOrders.DataSource = DBHelper.ExecuteQuery(sql)
        gvSalesOrders.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim soId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' SalesOrderItems akan ikut terhapus otomatis karena FK ON DELETE CASCADE
        DBHelper.ExecuteNonQuery("DELETE FROM dbo.SalesOrders WHERE SalesOrderID = @id",
            New SqlParameter("@id", soId))

        BindGrid()
    End Sub

End Class
