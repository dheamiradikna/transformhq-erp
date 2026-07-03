Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class Projects_Projects
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT p.ProjectID, p.ProjectCode, p.ProjectName, c.CompanyName, p.ProjectManager, p.Status, p.EndDate " &
                             "FROM dbo.Projects p " &
                             "LEFT JOIN dbo.Customers c ON p.CustomerID = c.CustomerID " &
                             "ORDER BY p.CreatedDate DESC"
        gvProjects.DataSource = DBHelper.ExecuteQuery(sql)
        gvProjects.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim projectId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' ProjectTasks akan ikut terhapus otomatis karena FK ON DELETE CASCADE
        DBHelper.ExecuteNonQuery("DELETE FROM dbo.Projects WHERE ProjectID = @id",
            New SqlParameter("@id", projectId))

        BindGrid()
    End Sub

End Class
