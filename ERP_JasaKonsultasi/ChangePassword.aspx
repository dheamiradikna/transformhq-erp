<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="ChangePassword.aspx.vb" Inherits="ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Ganti Password</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-lg">
        <div class="card">
            <h2>Ganti Password Saya</h2>

            <asp:Literal ID="litMessage" runat="server" />

            <div class="form-row">
                <label>Password Saat Ini *</label>
                <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCurrentPassword"
                    ErrorMessage="Password saat ini wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>

            <div class="form-row">
                <label>Password Baru *</label>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewPassword"
                    ErrorMessage="Password baru wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
                <span class="hint">Minimal 6 karakter.</span>
            </div>

            <div class="form-row">
                <label>Konfirmasi Password Baru *</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Konfirmasi password wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>

            <asp:Button ID="btnChangePassword" runat="server" Text="Simpan Password Baru"
                CssClass="btn btn-primary" OnClick="btnChangePassword_Click" />
        </div>
    </div>

</asp:Content>
