<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="SalesOrders.aspx.vb" Inherits="Sales_SalesOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Sales Order</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="SalesOrderEdit.aspx">+ Sales Order Baru</a>
        </div>

        <asp:GridView ID="gvSalesOrders" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada Sales Order." DataKeyNames="SalesOrderID">
            <Columns>
                <asp:BoundField DataField="OrderNo" HeaderText="No. SO" />
                <asp:BoundField DataField="CompanyName" HeaderText="Customer" />
                <asp:BoundField DataField="OrderDate" HeaderText="Tanggal" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "SalesOrderEdit.aspx?id=" & Eval("SalesOrderID") %>'>Lihat / Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("SalesOrderID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("OrderNo") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
