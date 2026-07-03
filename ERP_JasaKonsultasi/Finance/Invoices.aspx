<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Invoices.aspx.vb" Inherits="Finance_Invoices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Invoice &amp; Pembayaran</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="kpi-grid">
        <div class="kpi-card accent-warning">
            <div class="kpi-label">Total Outstanding (belum lunas)</div>
            <div class="kpi-value"><asp:Literal ID="litTotalOutstanding" runat="server" /></div>
        </div>
        <div class="kpi-card accent-danger">
            <div class="kpi-label">Invoice Jatuh Tempo (Overdue)</div>
            <div class="kpi-value"><asp:Literal ID="litOverdueCount" runat="server" /></div>
        </div>
        <div class="kpi-card accent-success">
            <div class="kpi-label">Total Sudah Dibayar</div>
            <div class="kpi-value"><asp:Literal ID="litTotalPaid" runat="server" /></div>
        </div>
    </div>

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="InvoiceEdit.aspx">+ Invoice Baru</a>
        </div>

        <asp:GridView ID="gvInvoices" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada invoice." DataKeyNames="InvoiceID">
            <Columns>
                <asp:BoundField DataField="InvoiceNo" HeaderText="No. Invoice" />
                <asp:BoundField DataField="CompanyName" HeaderText="Customer" />
                <asp:BoundField DataField="InvoiceDate" HeaderText="Tgl Invoice" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="DueDate" HeaderText="Jatuh Tempo" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="PaidAmount" HeaderText="Terbayar" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="Outstanding" HeaderText="Sisa" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("DisplayStatus").ToString().ToLower() %>'><%# Eval("DisplayStatus") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "InvoicePayments.aspx?id=" & Eval("InvoiceID") %>'>Bayar / Detail</a>
                        <a class="btn btn-outline btn-sm" href='<%# "InvoiceEdit.aspx?id=" & Eval("InvoiceID") %>'>Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("InvoiceID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("InvoiceNo") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
