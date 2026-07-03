Imports System.Data.SqlClient

Partial Public Class HR_PayrollPeriodEdit
    Inherits System.Web.UI.Page


    Private ReadOnly MonthNames As String() = {
        "", "Januari", "Februari", "Maret", "April", "Mei", "Juni",
        "Juli", "Agustus", "September", "Oktober", "November", "Desember"
    }

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString()
            txtYear.Text = DateTime.Now.Year.ToString()
            txtPayDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
        End If
    End Sub

    Protected Sub btnGenerate_Click(sender As Object, e As EventArgs)
        Dim month As Integer = Convert.ToInt32(ddlMonth.SelectedValue)
        Dim year As Integer
        If Not Integer.TryParse(txtYear.Text, year) OrElse year < 2000 OrElse year > 2100 Then
            ShowMessage("Tahun tidak valid.")
            Return
        End If

        ' Cek duplikat periode
        Dim existing = Convert.ToInt32(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM dbo.PayrollPeriods WHERE PeriodMonth = @Month AND PeriodYear = @Year",
            New SqlParameter("@Month", month), New SqlParameter("@Year", year)))

        If existing > 0 Then
            ShowMessage("Periode " & MonthNames(month) & " " & year & " sudah pernah dibuat sebelumnya.")
            Return
        End If

        Dim periodName As String = MonthNames(month) & " " & year
        Dim payDate As Object = If(txtPayDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtPayDate.Text))

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                Dim newPeriodId As Integer
                Using cmdIns As New SqlCommand(
                    "INSERT INTO dbo.PayrollPeriods (PeriodName, PeriodMonth, PeriodYear, PayDate, Status, CreatedBy) " &
                    "VALUES (@Name, @Month, @Year, @PayDate, 'Draft', @CreatedBy); SELECT SCOPE_IDENTITY();", conn, tran)
                    cmdIns.Parameters.AddWithValue("@Name", periodName)
                    cmdIns.Parameters.AddWithValue("@Month", month)
                    cmdIns.Parameters.AddWithValue("@Year", year)
                    cmdIns.Parameters.AddWithValue("@PayDate", payDate)
                    cmdIns.Parameters.AddWithValue("@CreatedBy", User.Identity.Name)
                    newPeriodId = Convert.ToInt32(cmdIns.ExecuteScalar())
                End Using

                ' Generate rincian gaji untuk semua karyawan Active, pakai Gaji Pokok dari Master Karyawan
                Using cmdGen As New SqlCommand(
                    "INSERT INTO dbo.PayrollDetails (PayrollPeriodID, EmployeeID, BaseSalary, Allowance, Deduction) " &
                    "SELECT @PeriodID, EmployeeID, BaseSalary, 0, 0 FROM dbo.Employees WHERE Status = 'Active'", conn, tran)
                    cmdGen.Parameters.AddWithValue("@PeriodID", newPeriodId)
                    cmdGen.ExecuteNonQuery()
                End Using

                tran.Commit()
                Response.Redirect("PayrollRun.aspx?id=" & newPeriodId)

            Catch ex As Exception
                tran.Rollback()
                ShowMessage("Gagal membuat periode payroll: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub ShowMessage(message As String)
        litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
    End Sub

End Class
