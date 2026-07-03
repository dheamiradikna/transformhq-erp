<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="InvoiceEdit.aspx.vb" Inherits="Finance_InvoiceEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Form Invoice</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-2xl"><div class="card" class="max-w-2xl">
        <h2><asp:Literal ID="litFormTitle" runat="server" Text="Invoice Baru" /></h2>

        <div class="form-row">
            <label>No. Invoice</label>
            <asp:Label ID="lblInvoiceNo" runat="server" Text="(otomatis terisi saat disimpan)" CssClass="hint" />
        </div>

        <div class="form-row">
            <label>Buat dari Sales Order (opsional)</label>
            <asp:DropDownList ID="ddlSalesOrder" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSalesOrder_SelectedIndexChanged" />
            <div class="hint">Pilih Sales Order untuk mengisi otomatis Customer &amp; Total. Atau pilih "-- Tidak ada --" untuk invoice manual.</div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Customer *</label>
                <asp:DropDownList ID="ddlCustomer" runat="server" />
            </div>
            <div class="form-row">
                <label>Total Tagihan (Rp) *</label>
                <asp:TextBox ID="txtTotalAmount" runat="server" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTotalAmount"
                    ErrorMessage="Total tagihan wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Tanggal Invoice</label>
                <asp:TextBox ID="txtInvoiceDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-row">
                <label>Jatuh Tempo</label>
                <asp:TextBox ID="txtDueDate" runat="server" TextMode="Date" />
            </div>
        </div>

        <div class="form-row">
            <label>Status</label>
            <asp:DropDownList ID="ddlStatus" runat="server">
                <asp:ListItem Text="Unpaid" Value="Unpaid" />
                <asp:ListItem Text="PartiallyPaid" Value="PartiallyPaid" />
                <asp:ListItem Text="Paid" Value="Paid" />
                <asp:ListItem Text="Cancelled" Value="Cancelled" />
            </asp:DropDownList>
            <div class="hint">Status di sini diisi manual hanya untuk kasus khusus (misal "Cancelled").
                Status pembayaran sebenarnya dihitung otomatis dari riwayat pembayaran di halaman "Bayar / Detail".</div>
        </div>

        <div class="form-row">
            <label>Catatan</label>
            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="2" />
        </div>

        <asp:Literal ID="litMessage" runat="server" />

        <asp:Button ID="btnSave" runat="server" Text="Simpan" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a class="btn btn-outline" href="Invoices.aspx">Batal</a>
    </div>
    </div>

</asp:Content>