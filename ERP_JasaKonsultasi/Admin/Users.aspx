<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Users.aspx.vb" Inherits="Admin_Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Manajemen User</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <h2 class="m-0">Daftar Akun Login</h2>
            <a class="btn btn-primary" href="UserEdit.aspx">+ User Baru</a>
        </div>

        <asp:GridView ID="gvUsers" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada user." DataKeyNames="UserID">
            <Columns>
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:BoundField DataField="FullName" HeaderText="Nama Lengkap" />
                <asp:BoundField DataField="Role" HeaderText="Role" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='<%# "badge " & If(CBool(Eval("IsActive")), "badge-active", "badge-inactive") %>'>
                            <%# If(CBool(Eval("IsActive")), "Active", "Inactive") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CreatedDate" HeaderText="Dibuat" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "UserEdit.aspx?id=" & Eval("UserID") %>'>Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-outline btn-sm" CommandArgument='<%# Eval("UserID") %>'
                            OnClick="lnkToggleActive_Click" Text='<%# If(CBool(Eval("IsActive")), "Nonaktifkan", "Aktifkan") %>' />
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("UserID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("Username") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
