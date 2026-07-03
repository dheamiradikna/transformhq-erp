<%@ Page Language="VB" AutoEventWireup="true" CodeBehind="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html>
<html lang="id">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Login - TransformHQ</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
    <script>tailwind.config={theme:{extend:{fontFamily:{sans:['Inter','system-ui','sans-serif']}}}}</script>
    <link rel="stylesheet" href="Content/site.css" />
</head>
<body>
<form id="formLogin" runat="server">
<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 to-slate-800 p-4">
    <div class="w-full max-w-sm">

        <!-- Logo area -->
        <div class="text-center mb-8">
            <h1 class="text-3xl font-bold text-white tracking-tight">
                Transform<span class="text-blue-400">HQ</span>
            </h1>
            <p class="text-slate-400 text-sm mt-1">Enterprise Resource Planning</p>
        </div>

        <!-- Card -->
        <div class="bg-white rounded-2xl shadow-2xl p-8">
            <h2 class="text-lg font-semibold text-slate-900 mb-1">Selamat datang kembali</h2>
            <p class="text-sm text-slate-500 mb-6">Masuk untuk melanjutkan ke dashboard</p>

            <asp:Literal ID="litError" runat="server" Visible="false" />

            <div class="space-y-4">
                <div>
                    <label class="block text-xs font-semibold text-slate-600 mb-1.5">Username</label>
                    <asp:TextBox ID="txtUsername" runat="server"
                        CssClass="w-full px-3.5 py-2.5 border border-slate-300 rounded-xl text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-shadow"
                        placeholder="Masukkan username" />
                </div>
                <div>
                    <label class="block text-xs font-semibold text-slate-600 mb-1.5">Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                        CssClass="w-full px-3.5 py-2.5 border border-slate-300 rounded-xl text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-shadow"
                        placeholder="••••••••" />
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Masuk" OnClick="btnLogin_Click"
                CssClass="w-full mt-5 px-4 py-2.5 bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold rounded-xl transition-colors cursor-pointer border-0 shadow-sm" />

            <div class="mt-5 pt-4 border-t border-slate-100">
                <p class="text-xs text-slate-400 text-center">
                    Demo: <span class="font-mono bg-slate-100 px-1.5 py-0.5 rounded text-slate-600">admin</span>
                    /
                    <span class="font-mono bg-slate-100 px-1.5 py-0.5 rounded text-slate-600">admin123</span>
                </p>
            </div>
        </div>

        <p class="text-center text-xs text-slate-500 mt-6">TransformHQ ERP &copy; 2026</p>
    </div>
</div>
</form>
</body>
</html>
