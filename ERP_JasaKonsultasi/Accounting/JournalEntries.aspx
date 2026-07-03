<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="JournalEntries.aspx.vb" Inherits="Accounting_JournalEntries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Jurnal Umum</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="JournalEntryEdit.aspx">+ Jurnal Manual Baru</a>
        </div>

        <asp:GridView ID="gvEntries" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada jurnal." DataKeyNames="JournalEntryID">
            <Columns>
                <asp:BoundField DataField="EntryNo" HeaderText="No. Jurnal" />
                <asp:BoundField DataField="EntryDate" HeaderText="Tanggal" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="Description" HeaderText="Deskripsi" />
                <asp:TemplateField HeaderText="Sumber">
                    <ItemTemplate>
                        <span class='<%# "badge " & If(Eval("SourceType").ToString() = "Manual", "badge-draft", "badge-active") %>'>
                            <%# Eval("SourceType") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TotalDebit" HeaderText="Total Debit" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "JournalEntryView.aspx?id=" & Eval("JournalEntryID") %>'>Lihat Detail</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" Visible='<%# Eval("SourceType").ToString() = "Manual" %>'
                            CommandArgument='<%# Eval("JournalEntryID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("EntryNo") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
