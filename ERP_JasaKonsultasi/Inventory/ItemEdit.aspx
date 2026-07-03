<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="ItemEdit.aspx.vb" Inherits="Inventory_ItemEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form Item</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-2xl"><div class="card" class="max-w-2xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="Item Baru" /></h2>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Kode Item *</label>
                <asp:TextBox ID="txtItemCode" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtItemCode"
                    ErrorMessage="Kode item wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Nama Item *</label>
                <asp:TextBox ID="txtItemName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtItemName"
                    ErrorMessage="Nama item wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Tipe</label>
                <asp:DropDownList ID="ddlItemType" runat="server">
                    <asp:ListItem Text="Goods (Barang)" Value="Goods" />
                    <asp:ListItem Text="Service (Jasa)" Value="Service" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Kategori</label>
                <asp:TextBox ID="txtCategory" runat="server" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Satuan</label>
                <asp:TextBox ID="txtUnit" runat="server" Text="Pcs" />
            </div>
            <div class="form-row">
                <label>Harga Satuan (Rp)</label>
                <asp:TextBox ID="txtUnitPrice" runat="server" TextMode="Number" Text="0" />
            </div>
        </div>

        <div class="form-row" >
            <label>Reorder Level (batas minimal stok)</label>
            <asp:TextBox ID="txtReorderLevel" runat="server" TextMode="Number" Text="0" />
            <div class="hint">Khusus untuk item bertipe Goods. Isi 0 jika tidak perlu diawasi stoknya.</div>
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Simpan" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a class="btn btn-outline" href="Items.aspx">Batal</a>
    </div>
    </div>

</asp:Content>