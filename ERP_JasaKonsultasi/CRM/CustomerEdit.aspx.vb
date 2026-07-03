Imports System.Data.SqlClient

Partial Public Class CRM_CustomerEdit
    Inherits System.Web.UI.Page


    Private ReadOnly Property CustomerId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If CustomerId.HasValue Then
                litFormTitle.Text = "Edit Customer / Lead"
                LoadCustomer(CustomerId.Value)
            Else
                litFormTitle.Text = "Customer / Lead Baru"
            End If
        End If
    End Sub

    Private Sub LoadCustomer(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Customers WHERE CustomerID = @id",
            New SqlParameter("@id", id))

        If dt.Rows.Count = 0 Then
            Response.Redirect("Customers.aspx")
            Return
        End If

        Dim row = dt.Rows(0)
        txtCompanyName.Text = row("CompanyName").ToString()
        txtContactName.Text = row("ContactName").ToString()
        txtEmail.Text = row("Email").ToString()
        txtPhone.Text = row("Phone").ToString()
        txtAddress.Text = row("Address").ToString()
        txtIndustry.Text = row("Industry").ToString()
        ddlStatus.SelectedValue = row("Status").ToString()
        txtNotes.Text = row("Notes").ToString()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        If CustomerId.HasValue Then
            DBHelper.ExecuteNonQuery(
                "UPDATE dbo.Customers SET CompanyName=@CompanyName, ContactName=@ContactName, Email=@Email, " &
                "Phone=@Phone, Address=@Address, Industry=@Industry, Status=@Status, Notes=@Notes " &
                "WHERE CustomerID=@id",
                New SqlParameter("@CompanyName", txtCompanyName.Text.Trim()),
                New SqlParameter("@ContactName", txtContactName.Text.Trim()),
                New SqlParameter("@Email", txtEmail.Text.Trim()),
                New SqlParameter("@Phone", txtPhone.Text.Trim()),
                New SqlParameter("@Address", txtAddress.Text.Trim()),
                New SqlParameter("@Industry", txtIndustry.Text.Trim()),
                New SqlParameter("@Status", ddlStatus.SelectedValue),
                New SqlParameter("@Notes", txtNotes.Text.Trim()),
                New SqlParameter("@id", CustomerId.Value))
        Else
            DBHelper.ExecuteNonQuery(
                "INSERT INTO dbo.Customers (CompanyName, ContactName, Email, Phone, Address, Industry, Status, Notes) " &
                "VALUES (@CompanyName, @ContactName, @Email, @Phone, @Address, @Industry, @Status, @Notes)",
                New SqlParameter("@CompanyName", txtCompanyName.Text.Trim()),
                New SqlParameter("@ContactName", txtContactName.Text.Trim()),
                New SqlParameter("@Email", txtEmail.Text.Trim()),
                New SqlParameter("@Phone", txtPhone.Text.Trim()),
                New SqlParameter("@Address", txtAddress.Text.Trim()),
                New SqlParameter("@Industry", txtIndustry.Text.Trim()),
                New SqlParameter("@Status", ddlStatus.SelectedValue),
                New SqlParameter("@Notes", txtNotes.Text.Trim()))
        End If

        Response.Redirect("Customers.aspx")
    End Sub

End Class
