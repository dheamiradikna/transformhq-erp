<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="JournalEntryEdit.aspx.vb" Inherits="Accounting_JournalEntryEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Jurnal Manual Baru</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-4xl"><div class="card">
        <h2>Header Jurnal</h2>
        <div class="form-grid-2">
            <div class="form-row">
                <label>Tanggal</label>
                <asp:TextBox ID="txtEntryDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-row">
                <label>Deskripsi *</label>
                <asp:TextBox ID="txtDescription" runat="server" placeholder="contoh: Pembelian ATK tunai" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription"
                    ErrorMessage="Deskripsi wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
        </div>
    </div>

    <div class="card">
        <h2>Baris Jurnal (Debit harus sama dengan Credit)</h2>

        <div class="form-grid-2" >
            <div class="form-row">
                <label>Akun</label>
                <asp:DropDownList ID="ddlAccount" runat="server" />
            </div>
            <div class="form-row">
                <label>Catatan baris (opsional)</label>
                <asp:TextBox ID="txtLineNotes" runat="server" />
            </div>
        </div>
        <div class="form-grid-2" >
            <div class="form-row">
                <label>Debit (Rp)</label>
                <asp:TextBox ID="txtDebit" runat="server" TextMode="Number" Text="0" />
            </div>
            <div class="form-row">
                <label>Credit (Rp)</label>
                <asp:TextBox ID="txtCredit" runat="server" TextMode="Number" Text="0" />
            </div>
        </div>
        <asp:Button ID="btnAddLine" runat="server" Text="+ Tambah Baris" CssClass="btn btn-outline" OnClick="btnAddLine_Click" CausesValidation="false" />

        <asp:Literal ID="litMessage" runat="server" />

        <asp:GridView ID="gvLines" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada baris jurnal." class="mt-4">
            <Columns>
                <asp:BoundField DataField="AccountDisplay" HeaderText="Akun" />
                <asp:BoundField DataField="Notes" HeaderText="Catatan" />
                <asp:BoundField DataField="Debit" HeaderText="Debit" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="Credit" HeaderText="Credit" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Container.DataItemIndex %>'
                            OnClick="lnkRemoveLine_Click" Text="Hapus" CausesValidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <p class="text-right font-bold mt-4 text-slate-900">
            Total Debit: <asp:Literal ID="litTotalDebit" runat="server" Text="Rp 0" /> &nbsp;|&nbsp;
            Total Credit: <asp:Literal ID="litTotalCredit" runat="server" Text="Rp 0" />
        </p>
    </div>

    <asp:Button ID="btnSave" runat="server" Text="Simpan Jurnal" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    <a class="btn btn-outline" href="JournalEntries.aspx">Batal</a>
    </div>

</asp:Content>