Imports System.Web.Security
Imports System.Data.SqlClient

Partial Public Class Login
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Jika sudah login, langsung lempar ke Dashboard
        If User.Identity.IsAuthenticated Then
            Response.Redirect("~/Default.aspx")
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text

        If username = "" OrElse password = "" Then
            ShowError("Username dan password wajib diisi.")
            Return
        End If

        Dim hashed As String = DBHelper.HashPassword(password)

        Dim sql As String = "SELECT UserID, FullName, Role FROM dbo.Users " &
                             "WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1"

        Dim dt = DBHelper.ExecuteQuery(sql,
            New SqlParameter("@Username", username),
            New SqlParameter("@PasswordHash", hashed))

        If dt.Rows.Count = 0 Then
            ShowError("Username atau password salah.")
            Return
        End If

        Dim fullName As String = dt.Rows(0)("FullName").ToString()
        Dim role As String = dt.Rows(0)("Role").ToString()

        ' Buat Forms Authentication ticket
        FormsAuthentication.SetAuthCookie(username, False)
        Session("FullName") = fullName
        Session("Role") = role

        Response.Redirect("~/Default.aspx")
    End Sub

    Private Sub ShowError(message As String)
        litError.Text = "<div class='mb-4 px-4 py-3 bg-red-50 border border-red-200 rounded-xl text-sm text-red-700 flex items-center gap-2'>" &
                        "<svg class='w-4 h-4 flex-shrink-0 text-red-500' fill='none' stroke='currentColor' viewBox='0 0 24 24'><path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z'/></svg>" &
                        Server.HtmlEncode(message) & "</div>"
        litError.Visible = True
    End Sub

End Class
