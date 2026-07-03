<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="UserEdit.aspx.vb" Inherits="Admin_UserEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form User</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-lg"><div class="card" class="max-w-xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="User Baru" /></h2>

        <asp:Literal ID="litMessage" runat="server" />

        <div class="form-grid-2">
            <div class="form-row">
                <label>Username *</label>
                <asp:TextBox ID="txtUsername" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Username wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Nama Lengkap *</label>
                <asp:TextBox ID="txtFullName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName"
                    ErrorMessage="Nama wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Role *</label>
            <asp:DropDownList ID="ddlRole" runat="server">
                <asp:ListItem Text="Admin (akses penuh + manajemen user)" Value="Admin" />
                <asp:ListItem Text="Manager (akses penuh kecuali manajemen user)" Value="Manager" />
                <asp:ListItem Text="Staff (CRM, Sales, Inventory, Projects saja)" Value="Staff" />
            </asp:DropDownList>
        </div>

        <div class="form-row">
            <label id="lblPasswordLabel" runat="server">Password *</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
            <div class="hint" id="hintPassword" runat="server">
                Kosongkan kalau tidak ingin mengubah password.
            </div>
        </div>

        <div class="form-row">
            <asp:CheckBox ID="chkIsActive" runat="server" Text=" Aktif (bisa login)" Checked="true" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Simpan" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a class="btn btn-outline" href="Users.aspx">Batal</a>
    </div>
    </div>

</asp:Content>