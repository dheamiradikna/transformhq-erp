<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Default.aspx.vb" Inherits="Default_Dashboard" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Dashboard</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- KPI Grid -->
    <div class="grid grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4 mb-5">

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-4 border-l-4 border-l-blue-500">
            <p class="text-xs font-semibold text-slate-500 uppercase tracking-wide">Total Customer</p>
            <p class="text-2xl font-bold text-slate-900 mt-1"><asp:Literal ID="litTotalCustomers" runat="server" /></p>
        </div>

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-4 border-l-4 border-l-amber-400">
            <p class="text-xs font-semibold text-slate-500 uppercase tracking-wide">Lead Belum Closing</p>
            <p class="text-2xl font-bold text-slate-900 mt-1"><asp:Literal ID="litTotalLeads" runat="server" /></p>
        </div>

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-4 border-l-4 border-l-green-500">
            <p class="text-xs font-semibold text-slate-500 uppercase tracking-wide">SO Bulan Ini</p>
            <p class="text-2xl font-bold text-slate-900 mt-1"><asp:Literal ID="litSOThisMonth" runat="server" /></p>
        </div>

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-4 border-l-4 border-l-blue-500">
            <p class="text-xs font-semibold text-slate-500 uppercase tracking-wide">Proyek Aktif</p>
            <p class="text-2xl font-bold text-slate-900 mt-1"><asp:Literal ID="litActiveProjects" runat="server" /></p>
        </div>

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-4 border-l-4 border-l-red-500">
            <p class="text-xs font-semibold text-slate-500 uppercase tracking-wide">Stok Menipis</p>
            <p class="text-2xl font-bold text-slate-900 mt-1"><asp:Literal ID="litLowStock" runat="server" /></p>
        </div>

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-4 border-l-4 border-l-amber-400">
            <p class="text-xs font-semibold text-slate-500 uppercase tracking-wide">Invoice Outstanding</p>
            <p class="text-lg font-bold text-slate-900 mt-1"><asp:Literal ID="litOutstandingInvoice" runat="server" /></p>
        </div>

    </div>

    <!-- Charts Row -->
    <div class="grid grid-cols-1 lg:grid-cols-5 gap-5 mb-5">

        <div class="lg:col-span-3 bg-white rounded-xl border border-slate-200 shadow-sm p-5">
            <div class="flex items-center justify-between mb-4">
                <h2 class="text-sm font-semibold text-slate-900">Tren Penjualan</h2>
                <span class="text-xs text-slate-400">6 Bulan Terakhir</span>
            </div>
            <canvas id="chartSalesTrend" height="200"></canvas>
        </div>

        <div class="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm p-5">
            <div class="flex items-center justify-between mb-4">
                <h2 class="text-sm font-semibold text-slate-900">Status Invoice</h2>
                <span class="text-xs text-slate-400">Semua waktu</span>
            </div>
            <canvas id="chartInvoiceStatus" height="200"></canvas>
            <div class="mt-3 space-y-1.5">
                <div class="flex items-center justify-between text-xs">
                    <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-amber-400 inline-block"></span>Unpaid</span>
                    <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-blue-400 inline-block"></span>Partial</span>
                    <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-green-500 inline-block"></span>Paid</span>
                    <span class="flex items-center gap-1.5"><span class="w-2.5 h-2.5 rounded-full bg-red-500 inline-block"></span>Overdue</span>
                </div>
            </div>
        </div>

    </div>

    <!-- Laba Bulan Ini -->
    <div class="mb-5">
        <div class="bg-gradient-to-r from-blue-600 to-blue-700 rounded-xl p-5 text-white shadow-sm">
            <p class="text-xs font-semibold text-blue-200 uppercase tracking-wide">Laba Bulan Ini</p>
            <p class="text-3xl font-bold mt-1"><asp:Literal ID="litNetIncomeMonth" runat="server" /></p>
            <p class="text-xs text-blue-300 mt-1">Berdasarkan data jurnal akuntansi</p>
        </div>
    </div>

    <!-- Tables Row -->
    <div class="grid grid-cols-1 xl:grid-cols-2 gap-5">

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
            <h2 class="text-sm font-semibold text-slate-900 mb-4">Sales Order Terbaru</h2>
            <asp:GridView ID="gvRecentSO" runat="server" CssClass="grid" AutoGenerateColumns="false"
                GridLines="None" EmptyDataText="Belum ada Sales Order.">
                <Columns>
                    <asp:BoundField DataField="OrderNo" HeaderText="No. SO" />
                    <asp:BoundField DataField="CompanyName" HeaderText="Customer" />
                    <asp:BoundField DataField="OrderDate" HeaderText="Tanggal" DataFormatString="{0:dd-MM-yyyy}" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:N0}" />
                </Columns>
            </asp:GridView>
        </div>

        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
            <h2 class="text-sm font-semibold text-slate-900 mb-4">Proyek Aktif</h2>
            <asp:GridView ID="gvRecentProjects" runat="server" CssClass="grid" AutoGenerateColumns="false"
                GridLines="None" EmptyDataText="Belum ada proyek aktif.">
                <Columns>
                    <asp:BoundField DataField="ProjectCode" HeaderText="Kode" />
                    <asp:BoundField DataField="ProjectName" HeaderText="Nama Proyek" />
                    <asp:BoundField DataField="ProjectManager" HeaderText="PM" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:BoundField DataField="EndDate" HeaderText="Target" DataFormatString="{0:dd-MM-yyyy}" />
                </Columns>
            </asp:GridView>
        </div>

    </div>

    <asp:Literal ID="litChartScript" runat="server" />

</asp:Content>
