<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Items.aspx.vb" Inherits="Inventory_Items" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Master Barang &amp; Jasa</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="ItemEdit.aspx">+ Item Baru</a>
        </div>

        <asp:GridView ID="gvItems" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data item." DataKeyNames="ItemID">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Kode" />
                <asp:BoundField DataField="ItemName" HeaderText="Nama Item" />
                <asp:BoundField DataField="ItemType" HeaderText="Tipe" />
                <asp:BoundField DataField="Unit" HeaderText="Satuan" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Harga" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="TotalStock" HeaderText="Total Stok" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "ItemEdit.aspx?id=" & Eval("ItemID") %>'>Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("ItemID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("ItemName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
