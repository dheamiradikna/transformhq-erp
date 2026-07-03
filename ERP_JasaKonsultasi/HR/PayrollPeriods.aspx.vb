Imports System.Data.SqlClient

Partial Public Class HR_PayrollPeriods
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sql As String =
            "SELECT pp.PayrollPeriodID, pp.PeriodName, pp.PayDate, pp.Status, " &
            "COUNT(pd.PayrollDetailID) AS JumlahKaryawan, " &
            "ISNULL(SUM(pd.NetSalary), 0) AS TotalNetSalary " &
            "FROM dbo.PayrollPeriods pp " &
            "LEFT JOIN dbo.PayrollDetails pd ON pd.PayrollPeriodID = pp.PayrollPeriodID " &
            "GROUP BY pp.PayrollPeriodID, pp.PeriodName, pp.PayDate, pp.Status, pp.PeriodMonth, pp.PeriodYear " &
            "ORDER BY pp.PeriodYear DESC, pp.PeriodMonth DESC"

        gvPeriods.DataSource = DBHelper.ExecuteQuery(sql)
        gvPeriods.DataBind()
    End Sub

    Protected Sub lnkDelete_Click(sender As Object, e As EventArgs)
        Dim btn As LinkButton = CType(sender, LinkButton)
        Dim periodId As Integer = Convert.ToInt32(btn.CommandArgument)

        ' PayrollDetails ikut terhapus otomatis karena FK ON DELETE CASCADE
        DBHelper.ExecuteNonQuery("DELETE FROM dbo.PayrollPeriods WHERE PayrollPeriodID = @id",
            New SqlParameter("@id", periodId))

        BindGrid()
    End Sub

End Class
