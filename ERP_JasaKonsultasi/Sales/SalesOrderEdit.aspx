<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="SalesOrderEdit.aspx.vb" Inherits="Sales_SalesOrderEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form Sales Order</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card" class="max-w-3xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="Sales Order Baru" /></h2>

        <div class="form-grid-2">
            <div class="form-row">
                <label>No. Sales Order</label>
                <asp:Label ID="lblOrderNo" runat="server" Text="(otomatis terisi saat disimpan)" CssClass="hint" />
            </div>
            <div class="form-row">
                <label>Customer *</label>
                <asp:DropDownList ID="ddlCustomer" runat="server" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Text="Draft" Value="Draft" />
                    <asp:ListItem Text="Confirmed" Value="Confirmed" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Catatan</label>
                <asp:TextBox ID="txtNotes" runat="server" />
            </div>
        </div>
    </div>

    <div class="card" class="max-w-3xl">
        <h2>Item Pesanan</h2>

        <p class="hint" id="pLockedNotice" runat="server" visible="false">
            [TERKUNCI] Pesanan ini sudah <b>Confirmed/Completed</b> — item tidak bisa diubah lagi
            (stok sudah dipotong otomatis). Ubah Status ke "Cancelled" kalau ingin membatalkan
            &amp; mengembalikan stok.
        </p>

        <asp:Panel ID="pnlAddLine" runat="server">
            <div class="form-grid-2" >
                <div class="form-row">
                    <label>Pilih Item</label>
                    <asp:DropDownList ID="ddlItem" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged" />
                </div>
                <div class="form-row">
                    <label>Harga Satuan</label>
                    <asp:TextBox ID="txtUnitPrice" runat="server" TextMode="Number" />
                </div>
            </div>
            <div class="form-grid-2" >
                <div class="form-row">
                    <label>Qty</label>
                    <asp:TextBox ID="txtQty" runat="server" TextMode="Number" Text="1" />
                </div>
                <div class="form-row">
                    <asp:Button ID="btnAddLine" runat="server" Text="+ Tambah ke Daftar" CssClass="btn btn-outline" OnClick="btnAddLine_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <asp:Literal ID="litMessage" runat="server" />

        <asp:GridView ID="gvLines" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada item. Tambahkan item di atas.">
            <Columns>
                <asp:BoundField DataField="ItemName" HeaderText="Item" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Harga Satuan" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="LineTotal" HeaderText="Subtotal" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Container.DataItemIndex %>'
                            OnClick="lnkRemoveLine_Click" Text="Hapus" CausesValidation="false" Visible='<%# Not IsLocked %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="text-right text-lg font-bold mt-4 text-slate-900">
            Grand Total: <asp:Literal ID="litGrandTotal" runat="server" Text="Rp 0" />
        </div>
    </div>

    <asp:Button ID="btnSave" runat="server" Text="Simpan Sales Order" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    <a class="btn btn-outline" href="SalesOrders.aspx">Batal</a>

</asp:Content>
