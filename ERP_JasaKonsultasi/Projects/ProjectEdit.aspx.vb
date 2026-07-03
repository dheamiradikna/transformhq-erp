Imports System.Data.SqlClient
Imports System.Globalization

Partial Public Class Projects_ProjectEdit
    Inherits System.Web.UI.Page


    Private ReadOnly Property ProjectId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindDropdowns()
            If ProjectId.HasValue Then
                litFormTitle.Text = "Edit Proyek"
                LoadProject(ProjectId.Value)
            Else
                litFormTitle.Text = "Proyek Baru"
            End If
        End If
    End Sub

    Private Sub BindDropdowns()
        Dim dtCust = DBHelper.ExecuteQuery("SELECT CustomerID, CompanyName FROM dbo.Customers ORDER BY CompanyName")
        ddlCustomer.DataSource = dtCust
        ddlCustomer.DataTextField = "CompanyName"
        ddlCustomer.DataValueField = "CustomerID"
        ddlCustomer.DataBind()
        ddlCustomer.Items.Insert(0, New ListItem("-- Tidak ada --", ""))

        Dim dtSO = DBHelper.ExecuteQuery("SELECT SalesOrderID, OrderNo FROM dbo.SalesOrders ORDER BY OrderDate DESC")
        ddlSalesOrder.DataSource = dtSO
        ddlSalesOrder.DataTextField = "OrderNo"
        ddlSalesOrder.DataValueField = "SalesOrderID"
        ddlSalesOrder.DataBind()
        ddlSalesOrder.Items.Insert(0, New ListItem("-- Tidak ada --", ""))
    End Sub

    Private Sub LoadProject(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Projects WHERE ProjectID = @id", New SqlParameter("@id", id))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Projects.aspx")
            Return
        End If
        Dim row = dt.Rows(0)
        txtProjectCode.Text = row("ProjectCode").ToString()
        txtProjectName.Text = row("ProjectName").ToString()
        If row("CustomerID") IsNot DBNull.Value Then ddlCustomer.SelectedValue = row("CustomerID").ToString()
        If row("SalesOrderID") IsNot DBNull.Value Then ddlSalesOrder.SelectedValue = row("SalesOrderID").ToString()
        If row("StartDate") IsNot DBNull.Value Then txtStartDate.Text = Convert.ToDateTime(row("StartDate")).ToString("yyyy-MM-dd")
        If row("EndDate") IsNot DBNull.Value Then txtEndDate.Text = Convert.ToDateTime(row("EndDate")).ToString("yyyy-MM-dd")
        ddlStatus.SelectedValue = row("Status").ToString()
        txtProjectManager.Text = row("ProjectManager").ToString()
        txtBudget.Text = Convert.ToDecimal(row("Budget")).ToString(CultureInfo.InvariantCulture)
        txtDescription.Text = row("Description").ToString()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim customerId As Object = If(ddlCustomer.SelectedValue = "", CType(DBNull.Value, Object), Convert.ToInt32(ddlCustomer.SelectedValue))
        Dim salesOrderId As Object = If(ddlSalesOrder.SelectedValue = "", CType(DBNull.Value, Object), Convert.ToInt32(ddlSalesOrder.SelectedValue))
        Dim startDate As Object = If(txtStartDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtStartDate.Text))
        Dim endDate As Object = If(txtEndDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtEndDate.Text))
        Dim budget As Decimal = 0
        Decimal.TryParse(txtBudget.Text, NumberStyles.Any, CultureInfo.InvariantCulture, budget)

        If ProjectId.HasValue Then
            DBHelper.ExecuteNonQuery(
                "UPDATE dbo.Projects SET ProjectCode=@Code, ProjectName=@Name, CustomerID=@CustomerID, SalesOrderID=@SalesOrderID, " &
                "StartDate=@StartDate, EndDate=@EndDate, Status=@Status, ProjectManager=@PM, Budget=@Budget, Description=@Desc " &
                "WHERE ProjectID=@id",
                New SqlParameter("@Code", txtProjectCode.Text.Trim()),
                New SqlParameter("@Name", txtProjectName.Text.Trim()),
                New SqlParameter("@CustomerID", customerId),
                New SqlParameter("@SalesOrderID", salesOrderId),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate),
                New SqlParameter("@Status", ddlStatus.SelectedValue),
                New SqlParameter("@PM", txtProjectManager.Text.Trim()),
                New SqlParameter("@Budget", budget),
                New SqlParameter("@Desc", txtDescription.Text.Trim()),
                New SqlParameter("@id", ProjectId.Value))
        Else
            DBHelper.ExecuteNonQuery(
                "INSERT INTO dbo.Projects (ProjectCode, ProjectName, CustomerID, SalesOrderID, StartDate, EndDate, Status, ProjectManager, Budget, Description) " &
                "VALUES (@Code, @Name, @CustomerID, @SalesOrderID, @StartDate, @EndDate, @Status, @PM, @Budget, @Desc)",
                New SqlParameter("@Code", txtProjectCode.Text.Trim()),
                New SqlParameter("@Name", txtProjectName.Text.Trim()),
                New SqlParameter("@CustomerID", customerId),
                New SqlParameter("@SalesOrderID", salesOrderId),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate),
                New SqlParameter("@Status", ddlStatus.SelectedValue),
                New SqlParameter("@PM", txtProjectManager.Text.Trim()),
                New SqlParameter("@Budget", budget),
                New SqlParameter("@Desc", txtDescription.Text.Trim()))
        End If

        Response.Redirect("Projects.aspx")
    End Sub

End Class
