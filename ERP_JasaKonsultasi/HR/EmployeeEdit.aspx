<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="EmployeeEdit.aspx.vb" Inherits="HR_EmployeeEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form Karyawan</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-2xl"><div class="card" class="max-w-2xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="Karyawan Baru" /></h2>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Kode Karyawan *</label>
                <asp:TextBox ID="txtEmployeeCode" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmployeeCode"
                    ErrorMessage="Kode karyawan wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Nama Lengkap *</label>
                <asp:TextBox ID="txtFullName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName"
                    ErrorMessage="Nama wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Jabatan</label>
                <asp:TextBox ID="txtPosition" runat="server" />
            </div>
            <div class="form-row">
                <label>Departemen</label>
                <asp:TextBox ID="txtDepartment" runat="server" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" />
            </div>
            <div class="form-row">
                <label>Telepon</label>
                <asp:TextBox ID="txtPhone" runat="server" />
            </div>
        </div>

        <div class="form-row">
            <label>Alamat</label>
            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="2" />
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Tanggal Bergabung</label>
                <asp:TextBox ID="txtHireDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-row">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Text="Active" Value="Active" />
                    <asp:ListItem Text="Resigned" Value="Resigned" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Gaji Pokok (Rp) *</label>
                <asp:TextBox ID="txtBaseSalary" runat="server" TextMode="Number" />
            </div>
            <div class="form-row">
                <label>Hubungkan ke Akun Login (opsional)</label>
                <asp:DropDownList ID="ddlUser" runat="server" />
                <div class="hint">Pilih akun login kalau karyawan ini juga memakai sistem ini.</div>
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Nama Bank</label>
                <asp:TextBox ID="txtBankName" runat="server" />
            </div>
            <div class="form-row">
                <label>No. Rekening</label>
                <asp:TextBox ID="txtBankAccountNo" runat="server" />
            </div>
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Simpan" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a class="btn btn-outline" href="Employees.aspx">Batal</a>
    </div>
    </div>

</asp:Content>