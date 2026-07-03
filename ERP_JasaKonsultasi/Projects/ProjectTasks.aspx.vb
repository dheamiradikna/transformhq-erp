Imports System.Web.UI.WebControls
Imports System.Data.SqlClient

Partial Public Class Projects_ProjectTasks
    Inherits System.Web.UI.Page


    Private ReadOnly Property ProjectId As Integer
        Get
            Return Convert.ToInt32(Request.QueryString("projectId"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("projectId")) Then
            Response.Redirect("Projects.aspx")
            Return
        End If

        If Not IsPostBack Then
            LoadProjectName()
            BindGrid()
        End If
    End Sub

    Private Sub LoadProjectName()
        Dim name = DBHelper.ExecuteScalar("SELECT ProjectName FROM dbo.Projects WHERE ProjectID = @id",
            New SqlParameter("@id", ProjectId))
        litProjectName.Text = "Task untuk Proyek: " & If(name IsNot Nothing, name.ToString(), "")
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT TaskID, TaskName, AssignedTo, DueDate, Priority, Status " &
                             "FROM dbo.ProjectTasks WHERE ProjectID = @id ORDER BY DueDate ASC"
        gvTasks.DataSource = DBHelper.ExecuteQuery(sql, New SqlParameter("@id", ProjectId))
        gvTasks.DataBind()
    End Sub

    Protected Sub btnAddTask_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim startDate As Object = If(txtStartDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtStartDate.Text))
        Dim dueDate As Object = If(txtDueDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtDueDate.Text))

        DBHelper.ExecuteNonQuery(
            "INSERT INTO dbo.ProjectTasks (ProjectID, TaskName, AssignedTo, StartDate, DueDate, Status, Priority) " &
            "VALUES (@ProjectID, @TaskName, @AssignedTo, @StartDate, @DueDate, @Status, @Priority)",
            New SqlParameter("@ProjectID", ProjectId),
            New SqlParameter("@TaskName", txtTaskName.Text.Trim()),
            New SqlParameter("@AssignedTo", txtAssignedTo.Text.Trim()),
            New SqlParameter("@StartDate", startDate),
            New SqlParameter("@DueDate", dueDate),
            New SqlParameter("@Status", ddlStatus.SelectedValue),
            New SqlParameter("@Priority", ddlPriority.SelectedValue))

        txtTaskName.Text = ""
        txtAssignedTo.Text = ""
        txtStartDate.Text = ""
        txtDueDate.Text = ""
        BindGrid()
    End Sub

    Protected Sub lnkMarkDone_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim taskId As Integer = Convert.ToInt32(btn.CommandArgument)
        DBHelper.ExecuteNonQuery("UPDATE dbo.ProjectTasks SET Status = 'Done' WHERE TaskID = @id",
            New SqlParameter("@id", taskId))
        BindGrid()
    End Sub

    Protected Sub lnkDeleteTask_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim taskId As Integer = Convert.ToInt32(btn.CommandArgument)
        DBHelper.ExecuteNonQuery("DELETE FROM dbo.ProjectTasks WHERE TaskID = @id",
            New SqlParameter("@id", taskId))
        BindGrid()
    End Sub

End Class
