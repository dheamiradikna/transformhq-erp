<div align="center">

# 🏢 TransformHQ ERP

**Enterprise Resource Planning untuk Bisnis Jasa & Konsultasi**

[![ASP.NET Web Forms](https://img.shields.io/badge/ASP.NET-Web%20Forms%204.8-512BD4?logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/aspnet/web-forms/)
[![VB.NET](https://img.shields.io/badge/Language-VB.NET-0078D4?logo=visualstudio&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/visual-basic/)
[![SQL Server 2022](https://img.shields.io/badge/Database-SQL%20Server%202022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Tailwind CSS](https://img.shields.io/badge/UI-Tailwind%20CSS-38BDF8?logo=tailwindcss&logoColor=white)](https://tailwindcss.com/)
[![Visual Studio 2022](https://img.shields.io/badge/IDE-Visual%20Studio%202022-5C2D91?logo=visualstudio&logoColor=white)](https://visualstudio.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-22c55e.svg)](LICENSE)

<br/>

> Sistem ERP lengkap berbasis web untuk bisnis jasa & konsultasi — mencakup CRM, Sales, Inventori, Keuangan, Akuntansi (double-entry), HR & Payroll, Manajemen Proyek, serta Laporan & Analitik, semua dalam satu aplikasi terintegrasi dengan tampilan modern Tailwind CSS.

<br/>

[![Dashboard](https://placehold.co/900x500/0f172a/3b82f6?text=Screenshot+Dashboard+TransformHQ&font=inter)](screenshots/)

</div>

---

## 📋 Daftar Isi

- [Fitur Unggulan](#-fitur-unggulan)
- [Tech Stack](#-tech-stack)
- [Modul Lengkap](#-modul-lengkap)
- [Screenshot](#-screenshot)
- [Instalasi & Setup](#-instalasi--setup)
- [Akun Demo](#-akun-demo)
- [Struktur Project](#-struktur-project)
- [Live Demo](#-live-demo)
- [Deployment ke Azure](#-deployment-ke-azure)
- [Kontribusi](#-kontribusi)
- [Lisensi](#-lisensi)

---

## ✨ Fitur Unggulan

<table>
<tr>
<td width="50%">

**🔄 Integrasi Otomatis**
- Auto stock-out saat Sales Order dikonfirmasi
- Auto-posting jurnal double-entry dari Invoice, Pembayaran & Payroll
- Status invoice dihitung real-time (Unpaid → Overdue → Paid)

</td>
<td width="50%">

**🔐 Role-Based Access Control**
- 3 role: Admin, Manager, Staff
- Menu sidebar otomatis menyesuaikan role
- Server-side guard di setiap halaman sensitif

</td>
</tr>
<tr>
<td width="50%">

**📊 Dashboard Interaktif**
- Grafik tren penjualan 6 bulan (Chart.js)
- Doughnut chart status invoice
- KPI real-time: Customer, SO, Proyek, Stok, Laba

</td>
<td width="50%">

**🔔 Notifikasi & Reminder**
- Bell icon di topbar semua halaman
- Invoice jatuh tempo (7 hari ke depan)
- Stok menipis & task proyek mendekati deadline

</td>
</tr>
<tr>
<td width="50%">

**📚 Akuntansi Double-Entry**
- Chart of Accounts lengkap
- Jurnal otomatis + manual
- Neraca Saldo, Laba Rugi, Neraca

</td>
<td width="50%">

**📤 Export & Cetak**
- Semua laporan bisa di-export ke Excel (CSV)
- Cetak Invoice & Slip Gaji (PDF via browser print)
- DataTables dengan sorting & search built-in

</td>
</tr>
</table>

---

## 🛠 Tech Stack

| Layer | Teknologi |
|-------|-----------|
| **Backend** | ASP.NET Web Forms 4.8, VB.NET |
| **Database** | Microsoft SQL Server 2022 |
| **UI Framework** | Tailwind CSS (Play CDN) |
| **Fonts** | Inter (Google Fonts) |
| **Charts** | Chart.js v4.4 |
| **Tables** | jQuery DataTables 1.13.8 |
| **IDE** | Visual Studio 2022 |
| **Auth** | ASP.NET Forms Authentication |
| **ORM** | ADO.NET (murni, tanpa Entity Framework) |

---

## 📦 Modul Lengkap

```
TransformHQ ERP
├── 🏠  Dashboard          → KPI, grafik tren, proyek & SO terbaru
├── 👥  CRM                → Customer & Lead pipeline
├── 📋  Sales              → Sales Order multi-item + auto stock-out
├── 📦  Inventori          → Master Barang/Jasa, Gudang, Mutasi Stok
├── 💳  Keuangan           → Invoice, Pembayaran, Cetak Invoice
├── 📚  Akuntansi          → Chart of Accounts, Jurnal, Neraca Saldo, Laba Rugi, Neraca
├── 👤  HR & Payroll       → Data Karyawan, Proses Payroll, Slip Gaji
├── 🗂️  Proyek             → Manajemen Proyek & Task (Kanban-style)
├── 📈  Laporan            → Laporan Penjualan & Stok (Export CSV)
├── 🔔  Notifikasi         → Reminder invoice, stok, & deadline task
└── ⚙️  Administrasi       → Manajemen User, Ganti Password
```

---

## 📸 Screenshot

> **Catatan:** Ganti placeholder di bawah ini dengan screenshot nyata dari aplikasi.
> Lihat [Panduan Screenshot](#panduan-screenshot) untuk daftar scene yang disarankan.

<table>
<tr>
<td align="center" width="50%">
<img src="screenshots/01-login.png" alt="Halaman Login" width="100%"/>
<br/><em>Login Page — Modern card dengan gradient gelap</em>
</td>
<td align="center" width="50%">
<img src="screenshots/02-dashboard.png" alt="Dashboard" width="100%"/>
<br/><em>Dashboard — KPI cards, grafik Chart.js, data terbaru</em>
</td>
</tr>
<tr>
<td align="center" width="50%">
<img src="screenshots/03-crm.png" alt="CRM Customer" width="100%"/>
<br/><em>CRM — DataTables dengan search & sort otomatis</em>
</td>
<td align="center" width="50%">
<img src="screenshots/04-sales-order.png" alt="Sales Order" width="100%"/>
<br/><em>Sales Order — Multi-item dengan auto hitung total</em>
</td>
</tr>
<tr>
<td align="center" width="50%">
<img src="screenshots/05-invoice.png" alt="Invoice" width="100%"/>
<br/><em>Invoice & Pembayaran — Status real-time + riwayat</em>
</td>
<td align="center" width="50%">
<img src="screenshots/06-journal.png" alt="Jurnal Akuntansi" width="100%"/>
<br/><em>Jurnal Akuntansi — Double-entry otomatis & manual</em>
</td>
</tr>
<tr>
<td align="center" width="50%">
<img src="screenshots/07-payroll.png" alt="Payroll" width="100%"/>
<br/><em>Payroll — Proses gaji bulanan + cetak slip gaji</em>
</td>
<td align="center" width="50%">
<img src="screenshots/08-reports.png" alt="Laporan" width="100%"/>
<br/><em>Laporan Penjualan — Analitik per customer & item</em>
</td>
</tr>
</table>

### Panduan Screenshot

Berikut scene yang paling direkomendasikan untuk di-screenshot dan diunggah ke folder `screenshots/`:

| No | File | Scene yang diambil | Tips |
|----|------|--------------------|------|
| 01 | `01-login.png` | Halaman login `localhost:44301/Login.aspx` | Resolusi 1280×800, zoom 100% |
| 02 | `02-dashboard.png` | Dashboard setelah login — tampilkan semua KPI & grafik terisi | Pastikan ada data transaksi dulu |
| 03 | `03-crm.png` | Daftar Customer dengan DataTables ter-load | Klik salah satu header kolom untuk tunjukkan sorting |
| 04 | `04-sales-order.png` | Form Sales Order dengan beberapa item sudah ditambah | Pastikan Grand Total terisi |
| 05 | `05-invoice.png` | Halaman detail Invoice dengan status "PartiallyPaid" dan riwayat pembayaran | Tunjukkan badge berwarna |
| 06 | `06-journal.png` | Jurnal Umum — tampilkan mix jurnal otomatis (Invoice, Payment) & manual | Tunjukkan kolom SourceType |
| 07 | `07-payroll.png` | PayrollRun dengan 3+ karyawan, status Draft | Tampilkan kolom Tunjangan & Potongan |
| 08 | `08-reports.png` | Laporan Penjualan — KPI cards + tabel per customer | Ada data bulan ini |
| 09 | `09-notification.png` | Hover ke lonceng — dropdown notifikasi muncul | Ada minimal 2-3 notifikasi |
| 10 | `10-invoice-print.png` | Halaman cetak Invoice — tampilan clean tanpa sidebar | Gunakan Ctrl+P preview |
| 11 | `11-mobile.png` | Dashboard di DevTools mobile view (375px) | F12 → Toggle device toolbar |
| 12 | `12-balance-sheet.png` | Neraca — tampilkan "Neraca Balance ✓" | Ada jurnal yang sudah diposting |

---

## 🚀 Instalasi & Setup

### Prasyarat

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (dengan workload **ASP.NET and web development**)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server) (Express Edition sudah cukup)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup)
- Koneksi internet (untuk Tailwind CDN, jQuery, Chart.js, Inter font)

### Langkah 1 — Setup Database

1. Buka **SSMS**, konek ke instance SQL Server 2022 kamu
2. Buka file `Database/01_Create_Database_And_Schema.sql`
3. Tekan **F5** (Execute) — script ini akan membuat:
   - Database `ERP_JasaKonsultasi`
   - Semua tabel, relasi, & index
   - Data contoh (customers, items, sales order, proyek)
   - 3 akun login demo (admin, manager, staff)

> ⚠️ **Sudah punya database dari versi sebelumnya?**
> Jalankan script tambahan secara berurutan:
> ```
> Database/02_Finance_Module.sql      → tambah tabel Invoice & Payments
> Database/03_HR_Payroll_Module.sql   → tambah tabel Employees & Payroll
> Database/04_Accounting_Module.sql   → tambah tabel Chart of Accounts & Journal
> ```

### Langkah 2 — Konfigurasi Connection String

Buka `ERP_JasaKonsultasi/Web.config` dan sesuaikan:

```xml
<connectionStrings>
  <add name="ERPConnectionString"
       connectionString="Data Source=NAMA_SERVER;Initial Catalog=ERP_JasaKonsultasi;
                         User ID=sa;Password=PASSWORD_KAMU;
                         Integrated Security=False;MultipleActiveResultSets=True;
                         TrustServerCertificate=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Contoh untuk Windows Authentication (tanpa username/password):**
```xml
connectionString="Data Source=localhost;Initial Catalog=ERP_JasaKonsultasi;
                  Integrated Security=True;TrustServerCertificate=True;"
```

### Langkah 3 — Jalankan Aplikasi

```
1. Buka file ERP_JasaKonsultasi.sln di Visual Studio 2022
2. Build → Clean Solution
3. Build → Rebuild Solution
4. Tekan F5 (atau klik tombol IIS Express)
5. Browser akan membuka localhost:44301/Login.aspx
```

---

## 🔑 Akun Demo

| Username | Password | Role | Akses |
|----------|----------|------|-------|
| `admin` | `admin123` | **Admin** | Semua modul + Manajemen User |
| `manager` | `manager123` | **Manager** | Semua modul kecuali Manajemen User |
| `staff` | `staff123` | **Staff** | CRM, Sales, Inventori, Proyek |

> Password di-hash dengan **SHA256** di database. Untuk mengganti password, login sebagai Admin → menu **Manajemen User**, atau user bisa ganti sendiri lewat link **Ganti Password** di topbar.

---

## 📁 Struktur Project

```
TransformHQ-ERP/
├── 📁 Database/
│   ├── 01_Create_Database_And_Schema.sql   ← Instalasi lengkap (fresh install)
│   ├── 02_Finance_Module.sql               ← Tambahan modul Finance
│   ├── 03_HR_Payroll_Module.sql            ← Tambahan modul HR & Payroll
│   └── 04_Accounting_Module.sql            ← Tambahan modul Akuntansi
│
├── 📁 ERP_JasaKonsultasi/                  ← Project utama VS2022
│   ├── 📁 App_Code/
│   │   ├── DBHelper.vb                     ← ADO.NET helper (koneksi & query)
│   │   ├── AuthHelper.vb                   ← Role-Based Access Control
│   │   ├── JournalHelper.vb                ← Auto-posting jurnal double-entry
│   │   ├── CsvHelper.vb                    ← Export DataTable → CSV/Excel
│   │   └── NotificationHelper.vb           ← Reminder (invoice, stok, task)
│   │
│   ├── 📁 CRM/           → Customer & Lead
│   ├── 📁 Sales/         → Sales Order
│   ├── 📁 Inventory/     → Barang, Gudang, Stok
│   ├── 📁 Finance/       → Invoice & Pembayaran
│   ├── 📁 Accounting/    → COA, Jurnal, Neraca Saldo, Laba Rugi, Neraca
│   ├── 📁 HR/            → Karyawan & Payroll
│   ├── 📁 Projects/      → Proyek & Task
│   ├── 📁 Reports/       → Laporan Penjualan & Stok
│   ├── 📁 Admin/         → Manajemen User
│   ├── 📁 Content/       → site.css (Tailwind hybrid)
│   ├── 📁 Scripts/       → site.js (DataTables init)
│   │
│   ├── Site.master       ← Layout utama (sidebar + topbar + notifikasi)
│   ├── Login.aspx        ← Halaman login
│   ├── Default.aspx      ← Dashboard
│   └── Web.config        ← Konfigurasi app & connection string
│
├── 📁 screenshots/       ← Screenshot untuk README (isi manual)
├── ERP_JasaKonsultasi.sln
└── README.md
```

---

## 🌐 Live Demo

> ⚠️ **Penting:** Aplikasi ini menggunakan **ASP.NET Web Forms (.NET Framework 4.8)** dan **Microsoft SQL Server**, sehingga **tidak bisa di-deploy ke GitHub Pages** (yang hanya mendukung static site).
>
> Untuk live demo, gunakan salah satu platform berikut:

### Opsi 1 — Azure App Service (Rekomendasi, Gratis)

Microsoft Azure menyediakan **Free Tier (F1)** yang cocok untuk .NET Framework:

```bash
# Install Azure CLI
# https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

# Login ke Azure
az login

# Buat Resource Group
az group create --name TransformHQ-RG --location southeastasia

# Buat SQL Server di Azure
az sql server create \
  --name transformhq-sqlserver \
  --resource-group TransformHQ-RG \
  --location southeastasia \
  --admin-user sqladmin \
  --admin-password "YourPassword123!"

# Buat Database
az sql db create \
  --resource-group TransformHQ-RG \
  --server transformhq-sqlserver \
  --name ERP_JasaKonsultasi \
  --edition Basic

# Buat App Service Plan (Free)
az appservice plan create \
  --name TransformHQ-Plan \
  --resource-group TransformHQ-RG \
  --sku F1

# Buat Web App
az webapp create \
  --name transformhq-erp \
  --resource-group TransformHQ-RG \
  --plan TransformHQ-Plan

# Deploy via Visual Studio:
# Klik kanan project → Publish → Azure → Azure App Service
```

### Opsi 2 — Deploy Manual via Visual Studio

1. Klik kanan project di Solution Explorer → **Publish**
2. Pilih **Azure** → **Azure App Service (Windows)**
3. Login dengan akun Azure → pilih/buat App Service
4. Klik **Publish**

### Opsi 3 — Windows VPS/Hosting

Untuk hosting berbayar yang mendukung ASP.NET Web Forms:
- [Smarterasp.net](https://www.smarterasp.net/) — khusus ASP.NET, mulai $3/bulan
- [WinHost](https://www.winhost.com/) — Windows hosting dengan SQL Server
- [SmarterASP.NET Free Trial](https://www.smarterasp.net/free_asp_net_hosting) — 60 hari gratis

---

## 🔄 Push ke GitHub

### Setup Repository Baru

```bash
# 1. Clone atau download project ini, masuk ke folder root
cd TransformHQ-ERP

# 2. Inisialisasi git (kalau belum)
git init

# 3. Buat file .gitignore
# (sudah ada di project ini - lihat .gitignore section di bawah)

# 4. Add semua file
git add .

# 5. Commit pertama
git commit -m "feat: initial commit — TransformHQ ERP v1.0"

# 6. Tambahkan remote (ganti dengan URL repo kamu)
git remote add origin https://github.com/USERNAME/transformhq-erp.git

# 7. Push ke GitHub
git push -u origin main
```

### File .gitignore yang Disarankan

```gitignore
# Visual Studio
.vs/
*.user
*.suo
*.userprefs

# Build outputs
bin/
obj/

# Package restore
packages/
*.nupkg

# Database files (jangan commit file .mdf/.ldf)
*.mdf
*.ldf

# Logs
*.log

# OS files
.DS_Store
Thumbs.db

# Web.config (berisi credentials - JANGAN di-commit ke public repo!)
# ERP_JasaKonsultasi/Web.config
# → Ganti dengan Web.config.example dan isi credentials via environment variable

# Screenshots folder bisa di-commit kalau ada isinya
!screenshots/*.png
!screenshots/*.jpg
```

> ⚠️ **PENTING — Keamanan:** Jangan commit `Web.config` ke **public repository** karena berisi username & password database. Gunakan `Web.config.example` (tanpa credentials nyata) dan set credentials via environment variable di hosting.

### GitHub Actions — CI/CD ke Azure (Opsional)

Buat file `.github/workflows/deploy.yml`:

```yaml
name: Deploy TransformHQ ERP ke Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v2

    - name: Setup NuGet
      uses: NuGet/setup-nuget@v2

    - name: Restore NuGet packages
      run: nuget restore ERP_JasaKonsultasi.sln

    - name: Build Solution
      run: msbuild ERP_JasaKonsultasi.sln /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=AzureAppService

    - name: Deploy ke Azure Web App
      uses: azure/webapps-deploy@v3
      with:
        app-name: 'transformhq-erp'        # Ganti dengan nama App Service kamu
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: './ERP_JasaKonsultasi/bin/Release/Publish'
```

---

## 🤝 Kontribusi

Kontribusi sangat disambut! Berikut cara berkontribusi:

1. **Fork** repository ini
2. Buat branch baru: `git checkout -b fitur/nama-fitur-baru`
3. Commit perubahan: `git commit -m "feat: tambah fitur X"`
4. Push ke branch: `git push origin fitur/nama-fitur-baru`
5. Buat **Pull Request** ke branch `main`

### Konvensi Commit

```
feat:     fitur baru
fix:      perbaikan bug
docs:     perubahan dokumentasi
style:    perubahan tampilan/CSS
refactor: refactoring kode (tidak ada fitur/bug)
chore:    maintenance (update dependency, dll)
```

### Area yang Bisa Dikembangkan

- [ ] Closing entries / tutup buku akhir tahun (akuntansi)
- [ ] Notifikasi email (SMTP integration)
- [ ] Upload lampiran file (bukti transfer, kontrak)
- [ ] Multi-warehouse pada Sales Order
- [ ] Export ke format Excel asli (.xlsx via ClosedXML)
- [ ] Audit log lengkap (siapa mengubah apa, kapan)
- [ ] Modul Purchasing (Purchase Order, supplier management)
- [ ] Dashboard mobile-responsive yang lebih optimal

---

## 📄 Lisensi

Project ini dilisensikan di bawah **MIT License** — bebas digunakan, dimodifikasi, dan didistribusikan untuk keperluan apapun dengan mencantumkan attribution.

```
MIT License

Copyright (c) 2026 TransformHQ

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

---

## 🙏 Acknowledgements

- [ASP.NET Web Forms](https://docs.microsoft.com/en-us/aspnet/web-forms/) — Backend framework
- [Tailwind CSS](https://tailwindcss.com/) — UI styling
- [Chart.js](https://www.chartjs.org/) — Visualisasi data dashboard
- [jQuery DataTables](https://datatables.net/) — Tabel interaktif
- [Inter Font](https://rsms.me/inter/) — Tipografi modern
- [Heroicons](https://heroicons.com/) — SVG icon di sidebar

---

<div align="center">

**⭐ Kalau project ini membantu, jangan lupa beri Star di GitHub!**

[🐛 Report Bug](https://github.com/USERNAME/transformhq-erp/issues) · [✨ Request Feature](https://github.com/USERNAME/transformhq-erp/issues) · [📖 Documentation](https://github.com/USERNAME/transformhq-erp/wiki)

</div>
