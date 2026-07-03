<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="AccessDenied.aspx.vb" Inherits="AccessDenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Akses Ditolak</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex items-center justify-center min-h-64">
        <div class="bg-white rounded-2xl border border-slate-200 shadow-sm p-10 max-w-md w-full text-center">
            <div class="w-16 h-16 bg-red-100 rounded-2xl flex items-center justify-center mx-auto mb-4">
                <svg class="w-8 h-8 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
                </svg>
            </div>
            <h2 class="text-lg font-semibold text-slate-900 mb-2">Akses Ditolak</h2>
            <p class="text-sm text-slate-500 mb-1">Akun kamu tidak memiliki izin untuk mengakses halaman ini.</p>
            <p class="text-xs text-slate-400 mb-6">Hubungi Administrator untuk mengatur ulang role akun kamu.</p>
            <a href="~/Default.aspx" runat="server" class="btn btn-primary">
                Kembali ke Dashboard
            </a>
        </div>
    </div>
</asp:Content>
