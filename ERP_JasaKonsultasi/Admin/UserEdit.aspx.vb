Imports System.Data.SqlClient
Imports System.Web.UI.HtmlControls

Partial Public Class Admin_UserEdit
    Inherits System.Web.UI.Page


    Private ReadOnly Property UserId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin")

        If Not IsPostBack Then
            If UserId.HasValue Then
                litFormTitle.Text = "Edit User"
                lblPasswordLabel.InnerText = "Password Baru"
                hintPassword.Visible = True
                LoadUser(UserId.Value)
            Else
                litFormTitle.Text = "User Baru"
                lblPasswordLabel.InnerText = "Password *"
                hintPassword.Visible = False
            End If
        End If
    End Sub

    Private Sub LoadUser(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Users WHERE UserID = @id", New SqlParameter("@id", id))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Users.aspx")
            Return
        End If
        Dim row = dt.Rows(0)
        txtUsername.Text = row("Username").ToString()
        txtFullName.Text = row("FullName").ToString()
        ddlRole.SelectedValue = row("Role").ToString()
        chkIsActive.Checked = Convert.ToBoolean(row("IsActive"))
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim username As String = txtUsername.Text.Trim()
        Dim fullName As String = txtFullName.Text.Trim()
        Dim role As String = ddlRole.SelectedValue
        Dim isActive As Boolean = chkIsActive.Checked
        Dim password As String = txtPassword.Text

        If Not UserId.HasValue AndAlso password = "" Then
            ShowMessage("Password wajib diisi untuk user baru.")
            Return
        End If

        ' Cek username sudah dipakai user lain atau belum
        Dim duplicateSql As String = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @Username"
        If UserId.HasValue Then duplicateSql &= " AND UserID <> @id"

        Dim duplicateCount As Integer
        If UserId.HasValue Then
            duplicateCount = Convert.ToInt32(DBHelper.ExecuteScalar(duplicateSql,
                New SqlParameter("@Username", username), New SqlParameter("@id", UserId.Value)))
        Else
            duplicateCount = Convert.ToInt32(DBHelper.ExecuteScalar(duplicateSql, New SqlParameter("@Username", username)))
        End If

        If duplicateCount > 0 Then
            ShowMessage("Username '" & username & "' sudah dipakai. Gunakan username lain.")
            Return
        End If

        If UserId.HasValue Then
            If password <> "" Then
                DBHelper.ExecuteNonQuery(
                    "UPDATE dbo.Users SET Username=@Username, FullName=@FullName, Role=@Role, IsActive=@IsActive, PasswordHash=@Hash WHERE UserID=@id",
                    New SqlParameter("@Username", username),
                    New SqlParameter("@FullName", fullName),
                    New SqlParameter("@Role", role),
                    New SqlParameter("@IsActive", isActive),
                    New SqlParameter("@Hash", DBHelper.HashPassword(password)),
                    New SqlParameter("@id", UserId.Value))
            Else
                DBHelper.ExecuteNonQuery(
                    "UPDATE dbo.Users SET Username=@Username, FullName=@FullName, Role=@Role, IsActive=@IsActive WHERE UserID=@id",
                    New SqlParameter("@Username", username),
                    New SqlParameter("@FullName", fullName),
                    New SqlParameter("@Role", role),
                    New SqlParameter("@IsActive", isActive),
                    New SqlParameter("@id", UserId.Value))
            End If
        Else
            DBHelper.ExecuteNonQuery(
                "INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role, IsActive) VALUES (@Username, @Hash, @FullName, @Role, @IsActive)",
                New SqlParameter("@Username", username),
                New SqlParameter("@Hash", DBHelper.HashPassword(password)),
                New SqlParameter("@FullName", fullName),
                New SqlParameter("@Role", role),
                New SqlParameter("@IsActive", isActive))
        End If

        Response.Redirect("Users.aspx")
    End Sub

    Private Sub ShowMessage(message As String)
        litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
    End Sub

End Class
