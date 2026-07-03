<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Customers.aspx.vb" Inherits="CRM_Customers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Customer & Lead</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="CustomerEdit.aspx">+ Customer / Lead Baru</a>
        </div>

        <asp:GridView ID="gvCustomers" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data customer." DataKeyNames="CustomerID">
            <Columns>
                <asp:BoundField DataField="CompanyName" HeaderText="Perusahaan" />
                <asp:BoundField DataField="ContactName" HeaderText="Kontak" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Phone" HeaderText="Telepon" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "CustomerEdit.aspx?id=" & Eval("CustomerID") %>'>Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandName="DeleteCustomer"
                            CommandArgument='<%# Eval("CustomerID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("CompanyName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
