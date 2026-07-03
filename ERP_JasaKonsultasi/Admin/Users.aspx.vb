Imports System.Data.SqlClient

Partial Public Class Admin_Users
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin")

        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT UserID, Username, FullName, Role, IsActive, CreatedDate FROM dbo.Users ORDER BY CreatedDate"
        gvUsers.DataSource = DBHelper.ExecuteQuery(sql)
        gvUsers.DataBind()
    End Sub

    Protected Sub lnkToggleActive_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim userId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Cegah user menonaktifkan akunnya sendiri (supaya tidak terkunci dari sistem)
        If GetUsernameById(userId) = User.Identity.Name Then
            ShowMessage("Kamu tidak bisa menonaktifkan akun yang sedang kamu pakai untuk login.")
            Return
        End If

        DBHelper.ExecuteNonQuery(
            "UPDATE dbo.Users SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE UserID = @id",
            New SqlParameter("@id", userId))

        BindGrid()
    End Sub

    Private Function GetUsernameById(userId As Integer) As String
        Dim result = DBHelper.ExecuteScalar("SELECT Username FROM dbo.Users WHERE UserID = @id", New SqlParameter("@id", userId))
        Return If(result IsNot Nothing, result.ToString(), "")
    End Function

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim userId As Integer = Convert.ToInt32(btn.CommandArgument)

        If GetUsernameById(userId) = User.Identity.Name Then
            ShowMessage("Kamu tidak bisa menghapus akun yang sedang kamu pakai untuk login.")
            Return
        End If

        ' Cek apakah user ini terhubung ke data Karyawan - kalau iya, jangan hapus permanen
        Dim linkedEmployee = Convert.ToInt32(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.Employees WHERE UserID = @id", New SqlParameter("@id", userId)))

        If linkedEmployee > 0 Then
            DBHelper.ExecuteNonQuery("UPDATE dbo.Users SET IsActive = 0 WHERE UserID = @id", New SqlParameter("@id", userId))
        Else
            DBHelper.ExecuteNonQuery("DELETE FROM dbo.Users WHERE UserID = @id", New SqlParameter("@id", userId))
        End If

        BindGrid()
    End Sub

    Private Sub ShowMessage(message As String)
        ' Pesan sederhana ditaruh di atas grid lewat Response - untuk MVP cukup pakai Page.ClientScript alert
        ClientScript.RegisterStartupScript(Me.GetType(), "msg", "alert('" & message.Replace("'", "") & "');", True)
    End Sub

End Class
