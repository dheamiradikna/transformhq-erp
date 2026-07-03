Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Web.Security

Partial Public Class SiteMaster
    Inherits System.Web.UI.MasterPage

    ' Deklarasi control (Protected WithEvents) untuk halaman ini ada di
    ' Site.master.designer.vb (file terpisah, mengikuti konvensi standar VS2022
    ' supaya Visual Studio tidak membuat deklarasi dobel saat membuka file ini di editor).

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        ' Tampilkan nama user yang sedang login
        ' (Catatan: MasterPage tidak punya properti "User" langsung seperti Page,
        ' jadi harus diakses lewat "Page.User")
        If Page.User IsNot Nothing AndAlso Page.User.Identity.IsAuthenticated Then
            litUserName.Text = "Halo, " & Server.HtmlEncode(Page.User.Identity.Name) &
                " (" & Server.HtmlEncode(AuthHelper.CurrentRole(Page)) & ")"
        End If

        ' ===== Role-Based Access Control: sembunyikan menu yang tidak boleh diakses role ini =====
        ' Aturan: Staff -> hanya CRM, Sales, Inventory, Projects.
        '         Manager -> semua modul KECUALI Manajemen User.
        '         Admin -> semua modul.
        Dim role As String = AuthHelper.CurrentRole(Page)
        pnlFinanceGroup.Visible = (role = "Admin" OrElse role = "Manager")
        pnlHRGroup.Visible = (role = "Admin" OrElse role = "Manager")
        pnlReportsGroup.Visible = (role = "Admin" OrElse role = "Manager")
        pnlAdminGroup.Visible = (role = "Admin")

        ' Tandai menu sidebar yang aktif sesuai halaman yang sedang dibuka
        Dim path As String = Request.Url.AbsolutePath.ToLower()

        If path.Contains("/default.aspx") Then navDashboard.Attributes("class") = "nav-link active"
        If path.Contains("/customers.aspx") Or path.Contains("/customeredit.aspx") Then navCustomers.Attributes("class") = "nav-link active"
        If path.Contains("/salesorders.aspx") Or path.Contains("/salesorderedit.aspx") Then navSalesOrders.Attributes("class") = "nav-link active"
        If path.Contains("/items.aspx") Or path.Contains("/itemedit.aspx") Then navItems.Attributes("class") = "nav-link active"
        If path.Contains("/warehouses.aspx") Or path.Contains("/warehouseedit.aspx") Then navWarehouses.Attributes("class") = "nav-link active"
        If path.Contains("/stockmovements.aspx") Then navStock.Attributes("class") = "nav-link active"
        If path.Contains("/invoices.aspx") Or path.Contains("/invoiceedit.aspx") Or path.Contains("/invoicepayments.aspx") Or path.Contains("/invoiceprint.aspx") Then navInvoices.Attributes("class") = "nav-link active"
        If path.Contains("/chartofaccounts.aspx") Then navChartOfAccounts.Attributes("class") = "nav-link active"
        If path.Contains("/journalentries.aspx") Or path.Contains("/journalentryedit.aspx") Or path.Contains("/journalentryview.aspx") Then navJournalEntries.Attributes("class") = "nav-link active"
        If path.Contains("/trialbalance.aspx") Then navTrialBalance.Attributes("class") = "nav-link active"
        If path.Contains("/incomestatement.aspx") Then navIncomeStatement.Attributes("class") = "nav-link active"
        If path.Contains("/balancesheet.aspx") Then navBalanceSheet.Attributes("class") = "nav-link active"
        If path.Contains("/employees.aspx") Or path.Contains("/employeeedit.aspx") Then navEmployees.Attributes("class") = "nav-link active"
        If path.Contains("/payrollperiods.aspx") Or path.Contains("/payrollperiodedit.aspx") Or path.Contains("/payrollrun.aspx") Then navPayroll.Attributes("class") = "nav-link active"
        If path.Contains("/projects.aspx") Or path.Contains("/projectedit.aspx") Or path.Contains("/projecttasks.aspx") Then navProjects.Attributes("class") = "nav-link active"
        If path.Contains("/salesreport.aspx") Then navSalesReport.Attributes("class") = "nav-link active"
        If path.Contains("/stockreport.aspx") Then navStockReport.Attributes("class") = "nav-link active"
        If path.Contains("/users.aspx") Or path.Contains("/useredit.aspx") Then navUsers.Attributes("class") = "nav-link active"

        LoadNotificationBell(role)

    End Sub

    ''' <summary>Mengisi ikon lonceng di topbar dengan jumlah & daftar notifikasi aktif (invoice jatuh tempo,
    ''' stok menipis, task proyek mendekati deadline). Dipanggil di setiap halaman lewat Site.master.</summary>
    Private Sub LoadNotificationBell(role As String)
        Dim items = NotificationHelper.GetNotifications(role)

        If items.Count = 0 Then
            litNotifCount.Text = ""
            litNotifDropdown.Text = "<div class='notif-empty'>Tidak ada notifikasi aktif. Semua aman!</div>"
            Return
        End If

        litNotifCount.Text = "<span class='notif-count'>" & items.Count & "</span>"

        Dim sb As New System.Text.StringBuilder()
        ' Tampilkan maksimal 6 notifikasi terbaru di dropdown, sisanya lihat di halaman penuh
        For Each item In items.Take(6)
            sb.Append("<a class='notif-item " & item.Severity & "' href='" & ResolveUrl(item.Url) & "'>")
            sb.Append("<span class='notif-dot'></span>" & Server.HtmlEncode(item.Message))
            sb.Append("</a>")
        Next
        sb.Append("<a class='notif-footer' href='" & ResolveUrl("~/Notifications.aspx") & "'>Lihat Semua Notifikasi</a>")

        litNotifDropdown.Text = sb.ToString()
    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        FormsAuthentication.SignOut()
        Session.Clear()
        Response.Redirect("~/Login.aspx")
    End Sub

End Class
