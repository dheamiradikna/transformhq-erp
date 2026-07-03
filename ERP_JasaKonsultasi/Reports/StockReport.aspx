<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="StockReport.aspx.vb" Inherits="Reports_StockReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Laporan Stok</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <asp:Button ID="btnExport" runat="server" Text="Export Excel (CSV)" CssClass="btn btn-outline" OnClick="btnExport_Click" />
        </div>
    </div>

    <div class="kpi-grid">
        <div class="kpi-card accent-primary">
            <div class="kpi-label">Total Nilai Persediaan</div>
            <div class="kpi-value"><asp:Literal ID="litTotalValue" runat="server" /></div>
        </div>
        <div class="kpi-card accent-danger">
            <div class="kpi-label">Item Stok Menipis</div>
            <div class="kpi-value"><asp:Literal ID="litLowStockCount" runat="server" /></div>
        </div>
    </div>

    <div class="card">
        <h2>Saldo Stok per Item &amp; Gudang</h2>
        <asp:GridView ID="gvStock" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data stok.">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Kode" />
                <asp:BoundField DataField="ItemName" HeaderText="Item" />
                <asp:BoundField DataField="WarehouseName" HeaderText="Gudang" />
                <asp:BoundField DataField="QtyOnHand" HeaderText="Saldo" />
                <asp:BoundField DataField="ReorderLevel" HeaderText="Batas Minimal" />
                <asp:BoundField DataField="NilaiPersediaan" HeaderText="Nilai (Rp)" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <%# If(Convert.ToDecimal(Eval("QtyOnHand")) <= Convert.ToDecimal(Eval("ReorderLevel")) AndAlso Convert.ToDecimal(Eval("ReorderLevel")) > 0, "<span class='badge badge-overdue'>Menipis</span>", "<span class='badge badge-active'>Aman</span>") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
