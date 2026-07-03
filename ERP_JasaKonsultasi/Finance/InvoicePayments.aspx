<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="InvoicePayments.aspx.vb" Inherits="Finance_InvoicePayments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Detail Invoice &amp; Pembayaran</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <h2 class="m-0"><asp:Literal ID="litInvoiceNo" runat="server" /></h2>
            <div>
                <a class="btn btn-outline btn-sm" id="lnkPrint" runat="server" target="_blank">Cetak Invoice</a>
                <a class="btn btn-outline btn-sm" href="Invoices.aspx">&larr; Kembali ke Daftar Invoice</a>
            </div>
        </div>

        <div class="kpi-grid">
            <div class="kpi-card accent-primary">
                <div class="kpi-label">Customer</div>
                <div class="kpi-value text-base"><asp:Literal ID="litCustomer" runat="server" /></div>
            </div>
            <div class="kpi-card accent-primary">
                <div class="kpi-label">Total Tagihan</div>
                <div class="kpi-value"><asp:Literal ID="litTotal" runat="server" /></div>
            </div>
            <div class="kpi-card accent-success">
                <div class="kpi-label">Sudah Dibayar</div>
                <div class="kpi-value"><asp:Literal ID="litPaid" runat="server" /></div>
            </div>
            <div class="kpi-card accent-warning">
                <div class="kpi-label">Sisa Tagihan</div>
                <div class="kpi-value"><asp:Literal ID="litOutstanding" runat="server" /></div>
            </div>
        </div>
        <p>Status: <span id="lblStatusBadge" runat="server" class="badge" /> &nbsp;|&nbsp;
           Jatuh Tempo: <asp:Literal ID="litDueDate" runat="server" /></p>
    </div>

    <div class="card" class="max-w-2xl">
        <h2>Catat Pembayaran Baru</h2>

        <asp:Literal ID="litMessage" runat="server" />

        <div class="form-grid-2">
            <div class="form-row">
                <label>Jumlah Bayar (Rp) *</label>
                <asp:TextBox ID="txtAmount" runat="server" TextMode="Number" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmount"
                    ErrorMessage="Jumlah bayar wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Tanggal Bayar</label>
                <asp:TextBox ID="txtPaymentDate" runat="server" TextMode="Date" />
            </div>
        </div>

        <div class="form-grid-2">
            <div class="form-row">
                <label>Metode Pembayaran</label>
                <asp:DropDownList ID="ddlMethod" runat="server">
                    <asp:ListItem Text="Transfer Bank" Value="Transfer" />
                    <asp:ListItem Text="Cash" Value="Cash" />
                    <asp:ListItem Text="Kartu Kredit" Value="Credit Card" />
                    <asp:ListItem Text="Lainnya" Value="Lainnya" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>No. Referensi</label>
                <asp:TextBox ID="txtReference" runat="server" placeholder="contoh: no. resi transfer" />
            </div>
        </div>

        <div class="form-row">
            <label>Catatan</label>
            <asp:TextBox ID="txtNotes" runat="server" />
        </div>

        <asp:Button ID="btnAddPayment" runat="server" Text="+ Catat Pembayaran" CssClass="btn btn-primary" OnClick="btnAddPayment_Click" />
    </div>

    <div class="card">
        <h2>Riwayat Pembayaran</h2>
        <asp:GridView ID="gvPayments" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada pembayaran untuk invoice ini." DataKeyNames="PaymentID">
            <Columns>
                <asp:BoundField DataField="PaymentDate" HeaderText="Tanggal" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                <asp:BoundField DataField="Amount" HeaderText="Jumlah" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="PaymentMethod" HeaderText="Metode" />
                <asp:BoundField DataField="Reference" HeaderText="Referensi" />
                <asp:BoundField DataField="Notes" HeaderText="Catatan" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("PaymentID") %>'
                            OnClientClick="return confirm('Yakin ingin menghapus riwayat pembayaran ini?');"
                            OnClick="lnkDeletePayment_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
