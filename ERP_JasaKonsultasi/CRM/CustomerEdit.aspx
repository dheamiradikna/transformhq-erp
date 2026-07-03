<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="CustomerEdit.aspx.vb" Inherits="CRM_CustomerEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form Customer / Lead</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-2xl"><div class="card" class="max-w-2xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="Customer / Lead Baru" /></h2>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Nama Perusahaan *</label>
                <asp:TextBox ID="txtCompanyName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCompanyName"
                    ErrorMessage="Nama perusahaan wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Nama Kontak</label>
                <asp:TextBox ID="txtContactName" runat="server" />
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
                <label>Industri</label>
                <asp:TextBox ID="txtIndustry" runat="server" />
            </div>
            <div class="form-row">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Text="Lead" Value="Lead" />
                    <asp:ListItem Text="Active" Value="Active" />
                    <asp:ListItem Text="Inactive" Value="Inactive" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="form-row">
            <label>Catatan</label>
            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Simpan" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a class="btn btn-outline" href="Customers.aspx">Batal</a>
    </div>
    </div>

</asp:Content>