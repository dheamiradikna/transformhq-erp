<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="TrialBalance.aspx.vb" Inherits="Accounting_TrialBalance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Neraca Saldo</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-4xl"><div class="card">
        <div class="toolbar">
            <div class="form-row" class="mb-0">
                <label>Per Tanggal</label>
                <asp:TextBox ID="txtAsOfDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txtAsOfDate_TextChanged" />
            </div>
            <asp:Button ID="btnExport" runat="server" Text="Export Excel (CSV)" CssClass="btn btn-outline" OnClick="btnExport_Click" CausesValidation="false" />
        </div>

        <asp:GridView ID="gvTrialBalance" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data jurnal.">
            <Columns>
                <asp:BoundField DataField="AccountCode" HeaderText="Kode" />
                <asp:BoundField DataField="AccountName" HeaderText="Nama Akun" />
                <asp:BoundField DataField="AccountType" HeaderText="Tipe" />
                <asp:BoundField DataField="TotalDebit" HeaderText="Debit" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="TotalCredit" HeaderText="Credit" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>

        <p class="text-right font-bold mt-4 text-slate-900">
            Total Debit: <asp:Literal ID="litGrandTotalDebit" runat="server" /> &nbsp;|&nbsp;
            Total Credit: <asp:Literal ID="litGrandTotalCredit" runat="server" />
            &nbsp;
            <asp:Literal ID="litBalanceCheck" runat="server" />
        </p>
    </div>
    </div>

</asp:Content>