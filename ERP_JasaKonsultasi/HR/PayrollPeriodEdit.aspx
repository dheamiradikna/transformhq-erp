<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="PayrollPeriodEdit.aspx.vb" Inherits="HR_PayrollPeriodEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Buat Periode Payroll</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="max-w-xl"><div class="card" class="max-w-xl">
        <h2>Buat Periode Payroll Baru</h2>

        <asp:Literal ID="litMessage" runat="server" />

        <div class="form-grid-2">
            <div class="form-row">
                <label>Bulan *</label>
                <asp:DropDownList ID="ddlMonth" runat="server">
                    <asp:ListItem Text="Januari" Value="1" /><asp:ListItem Text="Februari" Value="2" />
                    <asp:ListItem Text="Maret" Value="3" /><asp:ListItem Text="April" Value="4" />
                    <asp:ListItem Text="Mei" Value="5" /><asp:ListItem Text="Juni" Value="6" />
                    <asp:ListItem Text="Juli" Value="7" /><asp:ListItem Text="Agustus" Value="8" />
                    <asp:ListItem Text="September" Value="9" /><asp:ListItem Text="Oktober" Value="10" />
                    <asp:ListItem Text="November" Value="11" /><asp:ListItem Text="Desember" Value="12" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Tahun *</label>
                <asp:TextBox ID="txtYear" runat="server" TextMode="Number" />
            </div>
        </div>

        <div class="form-row">
            <label>Tanggal Bayar (rencana)</label>
            <asp:TextBox ID="txtPayDate" runat="server" TextMode="Date" />
        </div>

        <div class="hint" class="mb-3">
            Setelah disimpan, sistem akan otomatis membuat rincian gaji untuk
            <b>semua karyawan dengan status Active</b>, menggunakan Gaji Pokok dari Master Karyawan
            (Tunjangan &amp; Potongan default Rp 0 - bisa diubah di halaman proses payroll).
        </div>

        <asp:Button ID="btnGenerate" runat="server" Text="Buat &amp; Generate Rincian Gaji" CssClass="btn btn-primary" OnClick="btnGenerate_Click" />
        <a class="btn btn-outline" href="PayrollPeriods.aspx">Batal</a>
    </div>
    </div>

</asp:Content>