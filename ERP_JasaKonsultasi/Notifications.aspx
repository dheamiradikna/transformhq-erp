<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="Notifications.aspx.vb" Inherits="Notifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Notifikasi &amp; Reminder</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="max-w-2xl">
        <div class="card">
            <h2>Semua Notifikasi Aktif</h2>
            <asp:Literal ID="litNotifications" runat="server" />
        </div>
    </div>
</asp:Content>
