<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="SalesReport.aspx.vb" Inherits="Reports_SalesReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Laporan Penjualan</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <div class="form-grid-2" class="mb-0">
                <div class="form-row" class="mb-0">
                    <label>Dari Tanggal</label>
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="Filter_Changed" />
                </div>
                <div class="form-row" class="mb-0">
                    <label>Sampai Tanggal</label>
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="Filter_Changed" />
                </div>
            </div>
            <asp:Button ID="btnExport" runat="server" Text="Export Excel (CSV)" CssClass="btn btn-outline" OnClick="btnExport_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="kpi-grid">
        <div class="kpi-card accent-primary">
            <div class="kpi-label">Total Penjualan (Confirmed/Completed)</div>
            <div class="kpi-value"><asp:Literal ID="litTotalSales" runat="server" /></div>
        </div>
        <div class="kpi-card accent-success">
            <div class="kpi-label">Jumlah Sales Order</div>
            <div class="kpi-value"><asp:Literal ID="litOrderCount" runat="server" /></div>
        </div>
        <div class="kpi-card accent-warning">
            <div class="kpi-label">Rata-rata per Order</div>
            <div class="kpi-value"><asp:Literal ID="litAvgOrder" runat="server" /></div>
        </div>
    </div>

    <div class="card">
        <h2>Penjualan per Customer</h2>
        <asp:GridView ID="gvByCustomer" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Tidak ada data penjualan pada periode ini.">
            <Columns>
                <asp:BoundField DataField="CompanyName" HeaderText="Customer" />
                <asp:BoundField DataField="JumlahOrder" HeaderText="Jumlah Order" />
                <asp:BoundField DataField="TotalPenjualan" HeaderText="Total Penjualan" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="card">
        <h2>Item Terlaris</h2>
        <asp:GridView ID="gvByItem" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Tidak ada data penjualan pada periode ini.">
            <Columns>
                <asp:BoundField DataField="ItemName" HeaderText="Item" />
                <asp:BoundField DataField="TotalQty" HeaderText="Total Qty Terjual" />
                <asp:BoundField DataField="TotalPenjualan" HeaderText="Total Penjualan" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
