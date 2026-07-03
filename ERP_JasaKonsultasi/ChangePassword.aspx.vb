Imports System.Data.SqlClient

Partial Public Class ChangePassword
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Tidak perlu AuthHelper.RequireRole - halaman ini boleh diakses semua role
        ' yang sudah login (cukup terautentikasi, sudah dijamin oleh Web.config).
    End Sub

    Protected Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim currentPassword As String = txtCurrentPassword.Text
        Dim newPassword As String = txtNewPassword.Text
        Dim confirmPassword As String = txtConfirmPassword.Text

        If newPassword.Length < 6 Then
            ShowMessage("Password baru minimal 6 karakter.", True)
            Return
        End If

        If newPassword <> confirmPassword Then
            ShowMessage("Konfirmasi password tidak cocok dengan password baru.", True)
            Return
        End If

        Dim username As String = User.Identity.Name
        Dim currentHash As String = DBHelper.HashPassword(currentPassword)

        Dim matchCount = Convert.ToInt32(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.Users WHERE Username = @Username AND PasswordHash = @Hash",
            New SqlParameter("@Username", username), New SqlParameter("@Hash", currentHash)))

        If matchCount = 0 Then
            ShowMessage("Password saat ini yang kamu masukkan salah.", True)
            Return
        End If

        DBHelper.ExecuteNonQuery(
            "UPDATE dbo.Users SET PasswordHash = @NewHash WHERE Username = @Username",
            New SqlParameter("@NewHash", DBHelper.HashPassword(newPassword)),
            New SqlParameter("@Username", username))

        txtCurrentPassword.Text = ""
        txtNewPassword.Text = ""
        txtConfirmPassword.Text = ""
        ShowMessage("Password berhasil diubah.", False)
    End Sub

    Private Sub ShowMessage(message As String, isError As Boolean)
        If isError Then
            litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
        Else
            litMessage.Text = "<div style='background:#dcfce7;color:#166534;padding:10px 12px;border-radius:6px;font-size:13px;margin-bottom:14px;'>" & Server.HtmlEncode(message) & "</div>"
        End If
    End Sub

End Class
