<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="StockMovements.aspx.vb" Inherits="Inventory_StockMovements" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Mutasi Stok</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card" class="max-w-xl">
        <h2>Catat Mutasi Stok (Barang Masuk / Keluar)</h2>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Item *</label>
                <asp:DropDownList ID="ddlItem" runat="server" />
            </div>
            <div class="form-row">
                <label>Gudang *</label>
                <asp:DropDownList ID="ddlWarehouse" runat="server" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Tipe Mutasi *</label>
                <asp:DropDownList ID="ddlMovementType" runat="server">
                    <asp:ListItem Text="Barang Masuk (IN)" Value="IN" />
                    <asp:ListItem Text="Barang Keluar (OUT)" Value="OUT" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Jumlah *</label>
                <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQuantity"
                    ErrorMessage="Jumlah wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>

        <div class="form-row">
            <label>Referensi / Catatan</label>
            <asp:TextBox ID="txtNotes" runat="server" placeholder="contoh: pembelian dari supplier, atau No. SO" />
        </div>

        <asp:Literal ID="litMessage" runat="server" />

        <asp:Button ID="btnSubmit" runat="server" Text="Simpan Mutasi" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
    </div>

    <div class="card">
        <h2>Riwayat Mutasi Terakhir</h2>
        <asp:GridView ID="gvMovements" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada mutasi stok.">
            <Columns>
                <asp:BoundField DataField="MovementDate" HeaderText="Tanggal" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                <asp:BoundField DataField="ItemName" HeaderText="Item" />
                <asp:BoundField DataField="WarehouseName" HeaderText="Gudang" />
                <asp:BoundField DataField="MovementType" HeaderText="Tipe" />
                <asp:BoundField DataField="Quantity" HeaderText="Jumlah" />
                <asp:BoundField DataField="Notes" HeaderText="Catatan" />
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
