''' <summary>
''' AuthHelper - kumpulan fungsi untuk mengatur Role-Based Access Control (RBAC).
''' Role user disimpan di Session("Role") saat login (lihat Login.aspx.vb).
''' Role yang dipakai di aplikasi ini: "Admin", "Manager", "Staff".
''' </summary>
Public Module AuthHelper

    ''' <summary>Mengambil role user yang sedang login dari Session. Kosong jika belum login.</summary>
    Public Function CurrentRole(page As System.Web.UI.Page) As String
        If page.Session("Role") IsNot Nothing Then
            Return page.Session("Role").ToString()
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Panggil di baris PALING ATAS Page_Load pada halaman yang dibatasi aksesnya.
    ''' Jika role user saat ini TIDAK ada di daftar allowedRoles, user akan dialihkan
    ''' ke halaman AccessDenied.aspx.
    ''' Contoh: AuthHelper.RequireRole(Me, "Admin", "Manager")
    ''' </summary>
    Public Sub RequireRole(page As System.Web.UI.Page, ParamArray allowedRoles() As String)
        Dim role As String = CurrentRole(page)
        Dim isAllowed As Boolean = False

        For Each r In allowedRoles
            If String.Equals(r, role, StringComparison.OrdinalIgnoreCase) Then
                isAllowed = True
                Exit For
            End If
        Next

        If Not isAllowed Then
            page.Response.Redirect("~/AccessDenied.aspx")
        End If
    End Sub

End Module
