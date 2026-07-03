Imports System.Data.SqlClient
Imports System.Globalization

Partial Public Class HR_EmployeeEdit
    Inherits System.Web.UI.Page


    Private ReadOnly Property EmployeeId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            BindUserDropdown()

            If EmployeeId.HasValue Then
                litFormTitle.Text = "Edit Karyawan"
                LoadEmployee(EmployeeId.Value)
            Else
                litFormTitle.Text = "Karyawan Baru"
            End If
        End If
    End Sub

    Private Sub BindUserDropdown()
        ' Tampilkan user dengan format: "FullName (username - Role)"
        ' Filter: hanya tampilkan user yang BELUM dihubungkan ke karyawan lain
        ' (kecuali user yang sudah terhubung ke karyawan yang sedang di-edit ini - supaya bisa di-simpan ulang)
        Dim excludeEmployeeId As Integer = If(EmployeeId.HasValue, EmployeeId.Value, -1)

        Dim dt = DBHelper.ExecuteQuery(
            "SELECT u.UserID, u.Username, u.FullName, u.Role " &
            "FROM dbo.Users u " &
            "WHERE u.IsActive = 1 " &
            "AND (u.UserID NOT IN (SELECT UserID FROM dbo.Employees WHERE UserID IS NOT NULL AND EmployeeID <> @CurrentEmpID) " &
            "OR @CurrentEmpID = -1) " &
            "ORDER BY u.FullName",
            New SqlParameter("@CurrentEmpID", excludeEmployeeId))

        ddlUser.Items.Clear()
        ddlUser.Items.Add(New ListItem("-- Tidak terhubung ke akun login --", ""))

        For Each row As DataRow In dt.Rows
            Dim label As String = row("FullName").ToString() &
                                  " (" & row("Username").ToString() &
                                  " - " & row("Role").ToString() & ")"
            ddlUser.Items.Add(New ListItem(label, row("UserID").ToString()))
        Next
    End Sub

    Private Sub LoadEmployee(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Employees WHERE EmployeeID = @id", New SqlParameter("@id", id))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Employees.aspx")
            Return
        End If
        Dim row = dt.Rows(0)
        txtEmployeeCode.Text = row("EmployeeCode").ToString()
        txtFullName.Text = row("FullName").ToString()
        txtPosition.Text = row("Position").ToString()
        txtDepartment.Text = row("Department").ToString()
        txtEmail.Text = row("Email").ToString()
        txtPhone.Text = row("Phone").ToString()
        txtAddress.Text = row("Address").ToString()
        If row("HireDate") IsNot DBNull.Value Then txtHireDate.Text = Convert.ToDateTime(row("HireDate")).ToString("yyyy-MM-dd")
        ddlStatus.SelectedValue = row("Status").ToString()
        txtBaseSalary.Text = Convert.ToDecimal(row("BaseSalary")).ToString(CultureInfo.InvariantCulture)
        If row("UserID") IsNot DBNull.Value Then ddlUser.SelectedValue = row("UserID").ToString()
        txtBankName.Text = row("BankName").ToString()
        txtBankAccountNo.Text = row("BankAccountNo").ToString()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim hireDate As Object = If(txtHireDate.Text = "", CType(DBNull.Value, Object), Convert.ToDateTime(txtHireDate.Text))
        Dim baseSalary As Decimal = 0
        Decimal.TryParse(txtBaseSalary.Text, NumberStyles.Any, CultureInfo.InvariantCulture, baseSalary)
        Dim userId As Object = If(ddlUser.SelectedValue = "", CType(DBNull.Value, Object), Convert.ToInt32(ddlUser.SelectedValue))

        If EmployeeId.HasValue Then
            DBHelper.ExecuteNonQuery(
                "UPDATE dbo.Employees SET EmployeeCode=@Code, FullName=@Name, Position=@Position, Department=@Dept, " &
                "Email=@Email, Phone=@Phone, Address=@Address, HireDate=@HireDate, Status=@Status, BaseSalary=@Salary, " &
                "UserID=@UserID, BankName=@BankName, BankAccountNo=@BankAccountNo WHERE EmployeeID=@id",
                New SqlParameter("@Code", txtEmployeeCode.Text.Trim()),
                New SqlParameter("@Name", txtFullName.Text.Trim()),
                New SqlParameter("@Position", txtPosition.Text.Trim()),
                New SqlParameter("@Dept", txtDepartment.Text.Trim()),
                New SqlParameter("@Email", txtEmail.Text.Trim()),
                New SqlParameter("@Phone", txtPhone.Text.Trim()),
                New SqlParameter("@Address", txtAddress.Text.Trim()),
                New SqlParameter("@HireDate", hireDate),
                New SqlParameter("@Status", ddlStatus.SelectedValue),
                New SqlParameter("@Salary", baseSalary),
                New SqlParameter("@UserID", userId),
                New SqlParameter("@BankName", txtBankName.Text.Trim()),
                New SqlParameter("@BankAccountNo", txtBankAccountNo.Text.Trim()),
                New SqlParameter("@id", EmployeeId.Value))
        Else
            DBHelper.ExecuteNonQuery(
                "INSERT INTO dbo.Employees (EmployeeCode, FullName, Position, Department, Email, Phone, Address, HireDate, Status, BaseSalary, UserID, BankName, BankAccountNo) " &
                "VALUES (@Code, @Name, @Position, @Dept, @Email, @Phone, @Address, @HireDate, @Status, @Salary, @UserID, @BankName, @BankAccountNo)",
                New SqlParameter("@Code", txtEmployeeCode.Text.Trim()),
                New SqlParameter("@Name", txtFullName.Text.Trim()),
                New SqlParameter("@Position", txtPosition.Text.Trim()),
                New SqlParameter("@Dept", txtDepartment.Text.Trim()),
                New SqlParameter("@Email", txtEmail.Text.Trim()),
                New SqlParameter("@Phone", txtPhone.Text.Trim()),
                New SqlParameter("@Address", txtAddress.Text.Trim()),
                New SqlParameter("@HireDate", hireDate),
                New SqlParameter("@Status", ddlStatus.SelectedValue),
                New SqlParameter("@Salary", baseSalary),
                New SqlParameter("@UserID", userId),
                New SqlParameter("@BankName", txtBankName.Text.Trim()),
                New SqlParameter("@BankAccountNo", txtBankAccountNo.Text.Trim()))
        End If

        Response.Redirect("Employees.aspx")
    End Sub

End Class
