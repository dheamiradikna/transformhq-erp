<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="BalanceSheet.aspx.vb" Inherits="Accounting_BalanceSheet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Neraca</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <div class="form-row" class="mb-0">
                <label>Per Tanggal</label>
                <asp:TextBox ID="txtAsOfDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="txtAsOfDate_TextChanged" />
            </div>
            <asp:Button ID="btnExport" runat="server" Text="Export Excel (CSV)" CssClass="btn btn-outline" OnClick="btnExport_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="chart-grid">
        <div class="card" class="mb-0">
            <h2>Aset (Asset)</h2>
            <asp:GridView ID="gvAsset" runat="server" CssClass="grid" AutoGenerateColumns="false"
                GridLines="None" EmptyDataText="Belum ada data.">
                <Columns>
                    <asp:BoundField DataField="AccountName" HeaderText="Akun" />
                    <asp:BoundField DataField="Amount" HeaderText="Saldo" DataFormatString="{0:N0}" />
                </Columns>
            </asp:GridView>
            <p class="text-right font-bold text-slate-900">Total Aset: <asp:Literal ID="litTotalAsset" runat="server" /></p>
        </div>

        <div class="card" class="mb-0">
            <h2>Kewajiban (Liability)</h2>
            <asp:GridView ID="gvLiability" runat="server" CssClass="grid" AutoGenerateColumns="false"
                GridLines="None" EmptyDataText="Belum ada data.">
                <Columns>
                    <asp:BoundField DataField="AccountName" HeaderText="Akun" />
                    <asp:BoundField DataField="Amount" HeaderText="Saldo" DataFormatString="{0:N0}" />
                </Columns>
            </asp:GridView>
            <p class="text-right font-semibold text-slate-700">Total Kewajiban: <asp:Literal ID="litTotalLiability" runat="server" /></p>

            <h2>Modal (Equity)</h2>
            <asp:GridView ID="gvEquity" runat="server" CssClass="grid" AutoGenerateColumns="false"
                GridLines="None" EmptyDataText="Belum ada data.">
                <Columns>
                    <asp:BoundField DataField="AccountName" HeaderText="Akun" />
                    <asp:BoundField DataField="Amount" HeaderText="Saldo" DataFormatString="{0:N0}" />
                </Columns>
            </asp:GridView>
            <p class="text-right font-semibold text-slate-700">
                Total Modal (termasuk Laba Berjalan): <asp:Literal ID="litTotalEquity" runat="server" />
            </p>
            <hr />
            <p class="text-right font-bold text-slate-900">Total Kewajiban + Modal: <asp:Literal ID="litTotalLiabilityEquity" runat="server" /></p>
        </div>
    </div>

    <div class="card">
        <asp:Literal ID="litBalanceCheck" runat="server" />
    </div>

</asp:Content>
