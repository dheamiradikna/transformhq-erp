<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Employees.aspx.vb" Inherits="HR_Employees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Data Karyawan</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <a class="btn btn-primary" href="EmployeeEdit.aspx">+ Karyawan Baru</a>
        </div>

        <asp:GridView ID="gvEmployees" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada data karyawan." DataKeyNames="EmployeeID">
            <Columns>
                <asp:BoundField DataField="EmployeeCode" HeaderText="Kode" />
                <asp:BoundField DataField="FullName" HeaderText="Nama" />
                <asp:BoundField DataField="Position" HeaderText="Jabatan" />
                <asp:BoundField DataField="Department" HeaderText="Departemen" />
                <asp:BoundField DataField="BaseSalary" HeaderText="Gaji Pokok" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "EmployeeEdit.aspx?id=" & Eval("EmployeeID") %>'>Edit</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("EmployeeID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("FullName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
