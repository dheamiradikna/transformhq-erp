<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="IncomeStatement.aspx.vb" Inherits="Accounting_IncomeStatement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Laporan Laba Rugi</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <div class="form-grid-2" class="mb-0">
                <div class="form-row" class="mb-0">
                    <label>Dari Tanggal</label>
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="DateFilter_TextChanged" />
                </div>
                <div class="form-row" class="mb-0">
                    <label>Sampai Tanggal</label>
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="DateFilter_TextChanged" />
                </div>
            </div>
            <asp:Button ID="btnExport" runat="server" Text="Export Excel (CSV)" CssClass="btn btn-outline" OnClick="btnExport_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="card" class="max-w-xl">
        <h2>Pendapatan</h2>
        <asp:GridView ID="gvRevenue" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada pendapatan pada periode ini.">
            <Columns>
                <asp:BoundField DataField="AccountName" HeaderText="Akun" />
                <asp:BoundField DataField="Amount" HeaderText="Jumlah" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>
        <p class="text-right font-semibold text-slate-700">Total Pendapatan: <asp:Literal ID="litTotalRevenue" runat="server" /></p>

        <h2>Beban</h2>
        <asp:GridView ID="gvExpense" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada beban pada periode ini.">
            <Columns>
                <asp:BoundField DataField="AccountName" HeaderText="Akun" />
                <asp:BoundField DataField="Amount" HeaderText="Jumlah" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>
        <p class="text-right font-semibold text-slate-700">Total Beban: <asp:Literal ID="litTotalExpense" runat="server" /></p>

        <hr />
        <p class="text-right text-lg font-bold text-slate-900">
            Laba / Rugi Bersih: <asp:Literal ID="litNetIncome" runat="server" />
        </p>
    </div>

</asp:Content>
