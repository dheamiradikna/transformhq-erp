<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="PayrollPeriods.aspx.vb" Inherits="HR_PayrollPeriods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Payroll</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <h2 class="m-0">Daftar Periode Payroll</h2>
            <a class="btn btn-primary" href="PayrollPeriodEdit.aspx">+ Buat Periode Baru</a>
        </div>

        <asp:GridView ID="gvPeriods" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada periode payroll." DataKeyNames="PayrollPeriodID">
            <Columns>
                <asp:BoundField DataField="PeriodName" HeaderText="Periode" />
                <asp:BoundField DataField="PayDate" HeaderText="Tgl Bayar" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="JumlahKaryawan" HeaderText="Jumlah Karyawan" />
                <asp:BoundField DataField="TotalNetSalary" HeaderText="Total Take Home Pay" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" href='<%# "PayrollRun.aspx?id=" & Eval("PayrollPeriodID") %>'>Lihat / Proses</a>
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" Visible='<%# Eval("Status").ToString() = "Draft" %>'
                            CommandArgument='<%# Eval("PayrollPeriodID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("PeriodName") & """);" %>'
                            OnClick="lnkDelete_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
