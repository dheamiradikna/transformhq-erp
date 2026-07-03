<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="ProjectEdit.aspx.vb" Inherits="Projects_ProjectEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form Proyek</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-2xl"><div class="card" class="max-w-2xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="Proyek Baru" /></h2>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Kode Proyek *</label>
                <asp:TextBox ID="txtProjectCode" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtProjectCode"
                    ErrorMessage="Kode proyek wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Nama Proyek *</label>
                <asp:TextBox ID="txtProjectName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtProjectName"
                    ErrorMessage="Nama proyek wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Customer</label>
                <asp:DropDownList ID="ddlCustomer" runat="server" />
            </div>
            <div class="form-row">
                <label>Terkait Sales Order</label>
                <asp:DropDownList ID="ddlSalesOrder" runat="server" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Tanggal Mulai</label>
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-row">
                <label>Target Selesai</label>
                <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Text="Planning" Value="Planning" />
                    <asp:ListItem Text="In Progress" Value="InProgress" />
                    <asp:ListItem Text="On Hold" Value="OnHold" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Project Manager</label>
                <asp:TextBox ID="txtProjectManager" runat="server" />
            </div>
        </div>

        <div class="form-row" >
            <label>Budget (Rp)</label>
            <asp:TextBox ID="txtBudget" runat="server" TextMode="Number" Text="0" />
        </div>

        <div class="form-row">
            <label>Deskripsi</label>
            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Simpan" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a class="btn btn-outline" href="Projects.aspx">Batal</a>
    </div>
    </div>

</asp:Content>