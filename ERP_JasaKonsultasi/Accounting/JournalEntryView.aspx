<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="JournalEntryView.aspx.vb" Inherits="Accounting_JournalEntryView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Detail Jurnal</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-4xl"><div class="card">
        <div class="toolbar">
            <h2 class="m-0"><asp:Literal ID="litEntryNo" runat="server" /></h2>
            <a class="btn btn-outline btn-sm" href="JournalEntries.aspx">&larr; Kembali ke Jurnal Umum</a>
        </div>
        <p>
            Tanggal: <asp:Literal ID="litEntryDate" runat="server" /> &nbsp;|&nbsp;
            Sumber: <asp:Literal ID="litSourceType" runat="server" /> &nbsp;|&nbsp;
            Dibuat oleh: <asp:Literal ID="litCreatedBy" runat="server" />
        </p>
        <p>Deskripsi: <b><asp:Literal ID="litDescription" runat="server" /></b></p>
    </div>

    <div class="card">
        <asp:GridView ID="gvLines" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Tidak ada baris jurnal.">
            <Columns>
                <asp:BoundField DataField="AccountCode" HeaderText="Kode Akun" />
                <asp:BoundField DataField="AccountName" HeaderText="Nama Akun" />
                <asp:BoundField DataField="Notes" HeaderText="Catatan" />
                <asp:BoundField DataField="Debit" HeaderText="Debit" DataFormatString="{0:N0}" />
                <asp:BoundField DataField="Credit" HeaderText="Credit" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>
    </div>
    </div>

</asp:Content>