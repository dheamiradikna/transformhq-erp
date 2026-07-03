Imports System.Data.SqlClient

Partial Public Class HR_Employees
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String = "SELECT EmployeeID, EmployeeCode, FullName, Position, Department, BaseSalary, Status " &
                             "FROM dbo.Employees ORDER BY FullName"
        gvEmployees.DataSource = DBHelper.ExecuteQuery(sql)
        gvEmployees.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim employeeId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' Cek apakah karyawan ini sudah punya riwayat payroll (jangan hapus permanen kalau sudah ada)
        Dim payrollCount = Convert.ToInt32(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.PayrollDetails WHERE EmployeeID = @id",
            New SqlParameter("@id", employeeId)))

        If payrollCount > 0 Then
            DBHelper.ExecuteNonQuery("UPDATE dbo.Employees SET Status = 'Resigned' WHERE EmployeeID = @id",
                New SqlParameter("@id", employeeId))
        Else
            DBHelper.ExecuteNonQuery("DELETE FROM dbo.Employees WHERE EmployeeID = @id",
                New SqlParameter("@id", employeeId))
        End If

        BindGrid()
    End Sub

End Class
