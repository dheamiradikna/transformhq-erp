Imports System.Data.SqlClient
Imports System.Web.UI.HtmlControls
Imports System.Globalization

Partial Public Class HR_PayrollRun
    Inherits System.Web.UI.Page


    Private _periodStatus As String = "Draft"

    ''' <summary>Dipakai di markup (.aspx) untuk menentukan apakah TextBox Tunjangan/Potongan bisa diedit.</summary>
    Protected ReadOnly Property IsDraft As Boolean
        Get
            Return _periodStatus = "Draft"
        End Get
    End Property

    ' HARUS Protected (bukan Private) supaya bisa diakses dari data binding
    ' <%# %> di markup PayrollRun.aspx (untuk link cetak slip gaji per baris).
    Protected ReadOnly Property PeriodId As Integer
        Get
            Return Convert.ToInt32(Request.QueryString("id"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If String.IsNullOrEmpty(Request.QueryString("id")) Then
            Response.Redirect("PayrollPeriods.aspx")
            Return
        End If

        If Not IsPostBack Then
            LoadPeriodInfo()
            BindGrid()
        End If
    End Sub

    Private Sub LoadPeriodInfo()
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.PayrollPeriods WHERE PayrollPeriodID = @id",
            New SqlParameter("@id", PeriodId))

        If dt.Rows.Count = 0 Then
            Response.Redirect("PayrollPeriods.aspx")
            Return
        End If

        Dim row = dt.Rows(0)
        _periodStatus = row("Status").ToString()

        litPeriodName.Text = row("PeriodName").ToString()
        litPayDate.Text = If(row("PayDate") Is DBNull.Value, "-", Convert.ToDateTime(row("PayDate")).ToString("dd-MM-yyyy"))
        lblStatusBadge.InnerText = _periodStatus
        lblStatusBadge.Attributes("class") = "badge badge-" & _periodStatus.ToLower()

        Dim totalNet = DBHelper.ExecuteScalar("SELECT ISNULL(SUM(NetSalary),0) FROM dbo.PayrollDetails WHERE PayrollPeriodID = @id",
            New SqlParameter("@id", PeriodId))
        litTotalNet.Text = "Rp " & Convert.ToDecimal(totalNet).ToString("N0")

        pnlDraftActions.Visible = IsDraft
        pFinalizedNote.Visible = Not IsDraft
    End Sub

    Private Sub BindGrid()
        Dim sql As String =
            "SELECT pd.PayrollDetailID, e.EmployeeID, e.EmployeeCode, e.FullName, e.Position, pd.BaseSalary, pd.Allowance, pd.Deduction, pd.NetSalary " &
            "FROM dbo.PayrollDetails pd " &
            "INNER JOIN dbo.Employees e ON pd.EmployeeID = e.EmployeeID " &
            "WHERE pd.PayrollPeriodID = @id ORDER BY e.FullName"

        gvDetails.DataSource = DBHelper.ExecuteQuery(sql, New SqlParameter("@id", PeriodId))
        gvDetails.DataBind()
    End Sub

    Protected Sub btnSaveChanges_Click(sender As Object, e As EventArgs)
        LoadPeriodInfo()
        If Not IsDraft Then
            ShowMessage("Periode ini sudah difinalisasi, tidak bisa diubah lagi.")
            BindGrid()
            Return
        End If

        For Each row As GridViewRow In gvDetails.Rows
            If row.RowType = DataControlRowType.DataRow Then
                Dim detailId As Integer = Convert.ToInt32(gvDetails.DataKeys(row.RowIndex).Value)
                Dim txtAllowance As TextBox = CType(row.FindControl("txtAllowance"), TextBox)
                Dim txtDeduction As TextBox = CType(row.FindControl("txtDeduction"), TextBox)

                Dim allowance As Decimal = 0
                Dim deduction As Decimal = 0
                Decimal.TryParse(txtAllowance.Text, NumberStyles.Any, CultureInfo.InvariantCulture, allowance)
                Decimal.TryParse(txtDeduction.Text, NumberStyles.Any, CultureInfo.InvariantCulture, deduction)

                DBHelper.ExecuteNonQuery(
                    "UPDATE dbo.PayrollDetails SET Allowance = @Allowance, Deduction = @Deduction WHERE PayrollDetailID = @id",
                    New SqlParameter("@Allowance", allowance),
                    New SqlParameter("@Deduction", deduction),
                    New SqlParameter("@id", detailId))
            End If
        Next

        litMessage.Text = "<div style='background:#dcfce7;color:#166534;padding:10px 12px;border-radius:6px;font-size:13px;margin-bottom:14px;'>Perubahan berhasil disimpan.</div>"
        LoadPeriodInfo()
        BindGrid()
    End Sub

    Protected Sub btnFinalize_Click(sender As Object, e As EventArgs)
        Dim totalNetSalary = Convert.ToDecimal(DBHelper.ExecuteScalar(
            "SELECT ISNULL(SUM(NetSalary),0) FROM dbo.PayrollDetails WHERE PayrollPeriodID = @id",
            New SqlParameter("@id", PeriodId)))
        Dim periodName = DBHelper.ExecuteScalar("SELECT PeriodName FROM dbo.PayrollPeriods WHERE PayrollPeriodID = @id",
            New SqlParameter("@id", PeriodId))

        Using conn As New SqlConnection(DBHelper.GetConnectionString())
            conn.Open()
            Dim tran As SqlTransaction = conn.BeginTransaction()
            Try
                Using cmd As New SqlCommand("UPDATE dbo.PayrollPeriods SET Status = 'Finalized' WHERE PayrollPeriodID = @id", conn, tran)
                    cmd.Parameters.AddWithValue("@id", PeriodId)
                    cmd.ExecuteNonQuery()
                End Using

                ' ===== Auto-posting jurnal: Beban Gaji (Debit) vs Bank (Credit) - asumsi gaji dibayar via transfer =====
                JournalHelper.PostSimpleEntry(conn, tran, DateTime.Now,
                    "Payroll " & If(periodName IsNot Nothing, periodName.ToString(), ""),
                    "Payroll", PeriodId,
                    "5-1000", "1-1100", totalNetSalary, User.Identity.Name)

                tran.Commit()
            Catch ex As Exception
                tran.Rollback()
                ShowMessage("Gagal finalisasi payroll: " & ex.Message)
                Return
            End Try
        End Using

        LoadPeriodInfo()
        BindGrid()
    End Sub

    Private Sub ShowMessage(message As String)
        litMessage.Text = "<div class='error-msg'>" & Server.HtmlEncode(message) & "</div>"
    End Sub

End Class
