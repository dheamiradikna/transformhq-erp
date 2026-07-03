<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="ChartOfAccounts.aspx.vb" Inherits="Accounting_ChartOfAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Chart of Accounts</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-2xl"><div class="card" class="max-w-2xl">
        <h2>Tambah Akun Baru</h2>
        <div class="form-grid-2">
            <div class="form-row">
                <label>Kode Akun *</label>
                <asp:TextBox ID="txtAccountCode" runat="server" placeholder="contoh: 1-1400" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAccountCode"
                    ErrorMessage="Kode akun wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Nama Akun *</label>
                <asp:TextBox ID="txtAccountName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAccountName"
                    ErrorMessage="Nama akun wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>
        <div class="form-grid-2">
            <div class="form-row">
                <label>Tipe Akun</label>
                <asp:DropDownList ID="ddlAccountType" runat="server">
                    <asp:ListItem Text="Asset (Aset)" Value="Asset" />
                    <asp:ListItem Text="Liability (Kewajiban)" Value="Liability" />
                    <asp:ListItem Text="Equity (Modal)" Value="Equity" />
                    <asp:ListItem Text="Revenue (Pendapatan)" Value="Revenue" />
                    <asp:ListItem Text="Expense (Beban)" Value="Expense" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Saldo Normal</label>
                <asp:DropDownList ID="ddlNormalBalance" runat="server">
                    <asp:ListItem Text="Debit" Value="Debit" />
                    <asp:ListItem Text="Credit" Value="Credit" />
                </asp:DropDownList>
            </div>
        </div>
        <asp:Literal ID="litMessage" runat="server" />
        <asp:Button ID="btnAdd" runat="server" Text="Tambah Akun" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </div>

    <div class="card">
        <h2>Daftar Akun</h2>
        <asp:GridView ID="gvAccounts" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada akun." DataKeyNames="AccountID">
            <Columns>
                <asp:BoundField DataField="AccountCode" HeaderText="Kode" />
                <asp:BoundField DataField="AccountName" HeaderText="Nama Akun" />
                <asp:BoundField DataField="AccountType" HeaderText="Tipe" />
                <asp:BoundField DataField="NormalBalance" HeaderText="Saldo Normal" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("AccountID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("AccountName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Nonaktifkan" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    </div>

</asp:Content>