Partial Public Class Notifications
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim role As String = AuthHelper.CurrentRole(Me)
        Dim items = NotificationHelper.GetNotifications(role)

        If items.Count = 0 Then
            litNotifications.Text = "<p class='empty-state'>Tidak ada notifikasi aktif saat ini. Semua aman!</p>"
            Return
        End If

        Dim sb As New System.Text.StringBuilder()
        sb.Append("<table class='grid'><thead><tr><th>Status</th><th>Pesan</th><th></th></tr></thead><tbody>")

        For Each item In items
            Dim badgeClass As String = If(item.Severity = "danger", "badge-overdue", "badge-draft")
            Dim badgeText As String = If(item.Severity = "danger", "Urgent", "Perhatian")
            sb.Append("<tr><td><span class='badge " & badgeClass & "'>" & badgeText & "</span></td>")
            sb.Append("<td>" & Server.HtmlEncode(item.Message) & "</td>")
            sb.Append("<td><a class='btn btn-outline btn-sm' href='" & ResolveUrl(item.Url) & "'>Lihat</a></td></tr>")
        Next

        sb.Append("</tbody></table>")
        litNotifications.Text = sb.ToString()
    End Sub

End Class
