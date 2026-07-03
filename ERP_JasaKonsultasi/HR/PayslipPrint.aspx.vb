Imports System.Data.SqlClient

Partial Public Class HR_PayslipPrint
    Inherits System.Web.UI.Page

    ' ===== Catatan penting =====
    ' Halaman ini SENGAJA tidak memakai kontrol server (asp:Literal dkk) dan TIDAK
    ' memerlukan file .designer.vb sama sekali. Semua data ditampilkan lewat
    ' Public Property biasa yang dipanggil langsung dari markup pakai <%= NamaProperty %>.
    ' Pendekatan ini dipakai khusus di sini supaya halaman ini kebal terhadap masalah
    ' "duplicate declaration" yang sempat berulang kali muncul akibat file .designer.vb
    ' ter-generate ulang secara otomatis oleh Visual Studio / ekstensi tertentu.

    Public Property PeriodNameText As String = ""
    Public Property EmployeeNameText As String = ""
    Public Property PositionText As String = ""
    Public Property EmployeeCodeText As String = ""
    Public Property PayDateText As String = ""
    Public Property BaseSalaryText As String = ""
    Public Property AllowanceText As String = ""
    Public Property DeductionText As String = ""
    Public Property NetSalaryText As String = ""

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If String.IsNullOrEmpty(Request.QueryString("periodId")) OrElse String.IsNullOrEmpty(Request.QueryString("employeeId")) Then
            Response.Redirect("PayrollPeriods.aspx")
            Return
        End If

        Dim periodId As Integer = Convert.ToInt32(Request.QueryString("periodId"))
        Dim employeeId As Integer = Convert.ToInt32(Request.QueryString("employeeId"))

        Dim sql As String =
            "SELECT pp.PeriodName, pp.PayDate, e.FullName, e.Position, e.EmployeeCode, " &
            "pd.BaseSalary, pd.Allowance, pd.Deduction, pd.NetSalary " &
            "FROM dbo.PayrollDetails pd " &
            "INNER JOIN dbo.PayrollPeriods pp ON pd.PayrollPeriodID = pp.PayrollPeriodID " &
            "INNER JOIN dbo.Employees e ON pd.EmployeeID = e.EmployeeID " &
            "WHERE pd.PayrollPeriodID = @PeriodID AND pd.EmployeeID = @EmployeeID"

        Dim dt = DBHelper.ExecuteQuery(sql,
            New SqlParameter("@PeriodID", periodId), New SqlParameter("@EmployeeID", employeeId))

        If dt.Rows.Count = 0 Then
            Response.Redirect("PayrollPeriods.aspx")
            Return
        End If

        Dim row = dt.Rows(0)
        PeriodNameText = row("PeriodName").ToString()
        EmployeeNameText = row("FullName").ToString()
        PositionText = If(row("Position") Is DBNull.Value, "-", row("Position").ToString())
        EmployeeCodeText = row("EmployeeCode").ToString()
        PayDateText = If(row("PayDate") Is DBNull.Value, "-", Convert.ToDateTime(row("PayDate")).ToString("dd MMMM yyyy", New Globalization.CultureInfo("id-ID")))
        BaseSalaryText = "Rp " & Convert.ToDecimal(row("BaseSalary")).ToString("N0")
        AllowanceText = "Rp " & Convert.ToDecimal(row("Allowance")).ToString("N0")
        DeductionText = "Rp " & Convert.ToDecimal(row("Deduction")).ToString("N0")
        NetSalaryText = "Rp " & Convert.ToDecimal(row("NetSalary")).ToString("N0")
    End Sub

End Class
