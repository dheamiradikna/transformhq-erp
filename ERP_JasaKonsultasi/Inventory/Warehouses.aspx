<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Warehouses.aspx.vb" Inherits="Inventory_Warehouses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Master Gudang</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-xl"><div class="card" class="max-w-lg">
        <h2>Tambah Gudang Baru</h2>
        <div class="form-row">
            <label>Nama Gudang *</label>
            <asp:TextBox ID="txtWarehouseName" runat="server" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtWarehouseName"
                ErrorMessage="Nama gudang wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
        </div>
        <div class="form-row">
            <label>Lokasi</label>
            <asp:TextBox ID="txtLocation" runat="server" />
        </div>
        <asp:Button ID="btnAdd" runat="server" Text="Tambah" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </div>

    <div class="card">
        <h2>Daftar Gudang</h2>
        <asp:GridView ID="gvWarehouses" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data gudang." DataKeyNames="WarehouseID">
            <Columns>
                <asp:BoundField DataField="WarehouseName" HeaderText="Nama Gudang" />
                <asp:BoundField DataField="Location" HeaderText="Lokasi" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("WarehouseID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("WarehouseName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    </div>

</asp:Content>