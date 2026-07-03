<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Projects.aspx.vb" Inherits="Projects_Projects" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Daftar Proyek</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="ProjectEdit.aspx">+ Proyek Baru</a>
        </div>

        <asp:GridView ID="gvProjects" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data proyek." DataKeyNames="ProjectID">
            <Columns>
                <asp:BoundField DataField="ProjectCode" HeaderText="Kode" />
                <asp:BoundField DataField="ProjectName" HeaderText="Nama Proyek" />
                <asp:BoundField DataField="CompanyName" HeaderText="Customer" />
                <asp:BoundField DataField="ProjectManager" HeaderText="PM" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EndDate" HeaderText="Target Selesai" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "ProjectTasks.aspx?projectId=" & Eval("ProjectID") %>'>Tasks</a>
                        <a class="btn btn-outline btn-sm" href='<%# "ProjectEdit.aspx?id=" & Eval("ProjectID") %>'>Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("ProjectID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("ProjectName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
