Imports System.Data
Imports System.Data.SqlClient

''' <summary>Satu item notifikasi/reminder yang ditampilkan di lonceng topbar.</summary>
Public Class NotificationItem
    Public Property Severity As String  ' "danger" (urgent/overdue), "warning" (perlu perhatian)
    Public Property Message As String
    Public Property Url As String
End Class

''' <summary>
''' NotificationHelper - mengumpulkan reminder dari berbagai modul (Invoice jatuh tempo,
''' stok menipis, task proyek mendekati deadline) jadi satu daftar notifikasi.
''' Dipanggil dari Site.master supaya muncul di semua halaman lewat ikon lonceng.
''' </summary>
Public Module NotificationHelper

    ''' <summary>Ambil semua notifikasi aktif untuk ditampilkan. Role menentukan modul mana yang relevan.</summary>
    Public Function GetNotifications(role As String) As List(Of NotificationItem)
        Dim items As New List(Of NotificationItem)()

        ' ===== 1) Invoice jatuh tempo / sudah lewat jatuh tempo (Admin & Manager) =====
        If role = "Admin" OrElse role = "Manager" Then
            Dim sqlInvoice As String =
                "SELECT i.InvoiceID, i.InvoiceNo, i.DueDate, c.CompanyName, " &
                "i.TotalAmount - ISNULL(p.PaidAmount,0) AS Outstanding " &
                "FROM dbo.Invoices i " &
                "INNER JOIN dbo.Customers c ON i.CustomerID = c.CustomerID " &
                "LEFT JOIN (SELECT InvoiceID, SUM(Amount) AS PaidAmount FROM dbo.Payments GROUP BY InvoiceID) p ON p.InvoiceID = i.InvoiceID " &
                "WHERE i.Status <> 'Cancelled' AND i.DueDate IS NOT NULL " &
                "AND i.TotalAmount > ISNULL(p.PaidAmount,0) " &
                "AND i.DueDate <= DATEADD(DAY, 7, CAST(GETDATE() AS DATE)) " &
                "ORDER BY i.DueDate ASC"

            Dim dtInvoice = DBHelper.ExecuteQuery(sqlInvoice)
            For Each row As DataRow In dtInvoice.Rows
                Dim dueDate As DateTime = Convert.ToDateTime(row("DueDate"))
                Dim isOverdue As Boolean = dueDate < DateTime.Now.Date
                Dim daysText As String = If(isOverdue,
                    "sudah lewat " & (DateTime.Now.Date - dueDate).Days & " hari",
                    "jatuh tempo dalam " & (dueDate - DateTime.Now.Date).Days & " hari")

                items.Add(New NotificationItem With {
                    .Severity = If(isOverdue, "danger", "warning"),
                    .Message = "Invoice " & row("InvoiceNo").ToString() & " (" & row("CompanyName").ToString() & ") " & daysText,
                    .Url = "~/Finance/InvoicePayments.aspx?id=" & row("InvoiceID").ToString()
                })
            Next
        End If

        ' ===== 2) Stok menipis (semua role yang punya akses Inventori - Staff, Manager, Admin) =====
        Dim sqlStock As String =
            "SELECT i.ItemName, sb.QtyOnHand, i.ReorderLevel " &
            "FROM dbo.StockBalances sb " &
            "INNER JOIN dbo.Items i ON sb.ItemID = i.ItemID " &
            "WHERE sb.QtyOnHand <= i.ReorderLevel AND i.ReorderLevel > 0"

        Dim dtStock = DBHelper.ExecuteQuery(sqlStock)
        For Each row As DataRow In dtStock.Rows
            items.Add(New NotificationItem With {
                .Severity = "warning",
                .Message = "Stok '" & row("ItemName").ToString() & "' menipis (sisa " & Convert.ToDecimal(row("QtyOnHand")).ToString("N0") & ")",
                .Url = "~/Inventory/Items.aspx"
            })
        Next

        ' ===== 3) Task proyek mendekati deadline / lewat deadline (semua role) =====
        Dim sqlTask As String =
            "SELECT pt.TaskID, pt.TaskName, pt.DueDate, p.ProjectName, p.ProjectID " &
            "FROM dbo.ProjectTasks pt " &
            "INNER JOIN dbo.Projects p ON pt.ProjectID = p.ProjectID " &
            "WHERE pt.Status <> 'Done' AND pt.DueDate IS NOT NULL " &
            "AND pt.DueDate <= DATEADD(DAY, 3, CAST(GETDATE() AS DATE)) " &
            "ORDER BY pt.DueDate ASC"

        Dim dtTask = DBHelper.ExecuteQuery(sqlTask)
        For Each row As DataRow In dtTask.Rows
            Dim dueDate As DateTime = Convert.ToDateTime(row("DueDate"))
            Dim isOverdue As Boolean = dueDate < DateTime.Now.Date
            Dim daysText As String = If(isOverdue, "sudah lewat deadline", "deadline dalam " & Math.Max(0, (dueDate - DateTime.Now.Date).Days) & " hari")

            items.Add(New NotificationItem With {
                .Severity = If(isOverdue, "danger", "warning"),
                .Message = "Task '" & row("TaskName").ToString() & "' (" & row("ProjectName").ToString() & ") " & daysText,
                .Url = "~/Projects/ProjectTasks.aspx?projectId=" & row("ProjectID").ToString()
            })
        Next

        Return items
    End Function

End Module
