Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class CRM_Customers
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT CustomerID, CompanyName, ContactName, Email, Phone, Status " &
                             "FROM dbo.Customers ORDER BY CreatedDate DESC"
        gvCustomers.DataSource = DBHelper.ExecuteQuery(sql)
        gvCustomers.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim customerId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Cek apakah customer ini sudah punya Sales Order (untuk mencegah data terhapus tanpa sengaja)
        Dim soCount = Convert.ToInt32(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.SalesOrders WHERE CustomerID = @id",
            New SqlParameter("@id", customerId)))

        If soCount > 0 Then
            ' Tidak hapus, hanya set Inactive supaya histori SO tetap aman
            DBHelper.ExecuteNonQuery("UPDATE dbo.Customers SET Status = 'Inactive' WHERE CustomerID = @id",
                New SqlParameter("@id", customerId))
        Else
            DBHelper.ExecuteNonQuery("DELETE FROM dbo.Customers WHERE CustomerID = @id",
                New SqlParameter("@id", customerId))
        End If

        BindGrid()
    End Sub

End Class
