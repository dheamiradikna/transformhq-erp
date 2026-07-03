<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="PayrollRun.aspx.vb" Inherits="HR_PayrollRun" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Proses Payroll</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <div class="toolbar">
            <h2 class="m-0"><asp:Literal ID="litPeriodName" runat="server" /></h2>
            <a class="btn btn-outline btn-sm" href="PayrollPeriods.aspx">&larr; Kembali ke Daftar Periode</a>
        </div>
        <p>
            Status: <span id="lblStatusBadge" runat="server" class="badge" /> &nbsp;|&nbsp;
            Tanggal Bayar: <asp:Literal ID="litPayDate" runat="server" /> &nbsp;|&nbsp;
            Total Take Home Pay: <b><asp:Literal ID="litTotalNet" runat="server" /></b>
        </p>
    </div>

    <div class="card">
        <asp:Literal ID="litMessage" runat="server" />

        <asp:GridView ID="gvDetails" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Tidak ada rincian gaji." DataKeyNames="PayrollDetailID">
            <Columns>
                <asp:BoundField DataField="EmployeeCode" HeaderText="Kode" />
                <asp:BoundField DataField="FullName" HeaderText="Nama Karyawan" />
                <asp:BoundField DataField="Position" HeaderText="Jabatan" />
                <asp:BoundField DataField="BaseSalary" HeaderText="Gaji Pokok" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Tunjangan (Rp)">
                    <ItemTemplate>
                        <asp:TextBox ID="txtAllowance" runat="server" TextMode="Number" Width="110"
                            Text='<%# Eval("Allowance") %>' ReadOnly='<%# Not IsDraft %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Potongan (Rp)">
                    <ItemTemplate>
                        <asp:TextBox ID="txtDeduction" runat="server" TextMode="Number" Width="110"
                            Text='<%# Eval("Deduction") %>' ReadOnly='<%# Not IsDraft %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="NetSalary" HeaderText="Take Home Pay" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Slip Gaji">
                    <ItemTemplate>
                        <a class="btn btn-outline btn-sm" target="_blank"
                           href='<%# "PayslipPrint.aspx?periodId=" & PeriodId & "&employeeId=" & Eval("EmployeeID") %>'>Cetak Slip</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlDraftActions" runat="server" class="mt-4">
            <asp:Button ID="btnSaveChanges" runat="server" Text="Simpan Perubahan" CssClass="btn btn-outline" OnClick="btnSaveChanges_Click" />
            <asp:Button ID="btnFinalize" runat="server" Text="Finalisasi Payroll" CssClass="btn btn-primary" OnClick="btnFinalize_Click"
                OnClientClick="return confirm('Setelah difinalisasi, rincian gaji tidak bisa diubah lagi. Lanjutkan?');" />
        </asp:Panel>
        <p class="text-xs text-amber-600 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2 mt-2" runat="server" id="pFinalizedNote" visible="false">
            Periode ini sudah difinalisasi dan tidak bisa diubah lagi.
        </p>
    </div>

</asp:Content>
