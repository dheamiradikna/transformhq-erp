# Panduan Kontribusi — TransformHQ ERP

Terima kasih sudah tertarik berkontribusi! Berikut panduan lengkapnya.

## 🚀 Quick Start untuk Contributor

```bash
# 1. Fork repo ini di GitHub (klik tombol Fork di kanan atas)

# 2. Clone fork kamu ke lokal
git clone https://github.com/USERNAME-KAMU/transformhq-erp.git
cd transformhq-erp

# 3. Tambahkan remote upstream (repo asli)
git remote add upstream https://github.com/OWNER/transformhq-erp.git

# 4. Buat branch baru dari main yang sudah up-to-date
git checkout main
git pull upstream main
git checkout -b fitur/nama-fitur-kamu
```

## 📋 Sebelum Mulai Coding

1. **Cek Issues** — lihat apakah fitur/bug yang ingin kamu kerjakan sudah ada di tab Issues
2. **Buka Issue baru** kalau belum ada — describe apa yang ingin kamu buat/perbaiki
3. **Assign dirimu** ke Issue tersebut supaya tidak ada yang mengerjakan hal yang sama

## 🏗️ Konvensi Kode

### VB.NET

```vb
' ✅ BAIK - deklarasi kontrol ada di .designer.vb, bukan di file utama
Partial Public Class Finance_Invoices
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' AuthHelper selalu dipanggil pertama untuk halaman yang dibatasi
        AuthHelper.RequireRole(Me, "Admin", "Manager")
        ...
    End Sub
End Class

' ✅ BAIK - selalu pakai parameterized query (JANGAN string concatenation untuk SQL)
Dim dt = DBHelper.ExecuteQuery(
    "SELECT * FROM dbo.Customers WHERE CustomerID = @id",
    New SqlParameter("@id", customerId))

' ❌ JANGAN - SQL injection risk
Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Customers WHERE CustomerID = " & customerId)
```

### ASP.NET Web Forms

```aspx
<%-- ✅ Semua kontrol server harus ada CodeBehind (bukan CodeFile) --%>
<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" 
         CodeBehind="MyPage.aspx.vb" Inherits="MyPage" %>

<%-- ✅ GUNAKAN If() bukan ternary C# ?:  --%>
<%# If(CBool(Eval("IsActive")), "Active", "Inactive") %>

<%-- ❌ JANGAN - ini C# syntax, tidak valid di VB.NET aspx --%>
<%# CBool(Eval("IsActive")) ? "Active" : "Inactive" %>
```

### CSS / Tailwind

- Utamakan **Tailwind utility classes** untuk elemen baru
- Tambah ke `site.css` hanya untuk komponen yang tidak bisa dibuat dengan utility classes
- Jaga konsistensi: gunakan palet `slate`, `blue`, `green`, `red`, `amber` dari Tailwind

## 📁 Menambah Halaman Baru

Saat menambah halaman baru, ikuti checklist ini:

```
[ ] Buat file .aspx dengan CodeBehind= (bukan CodeFile=)
[ ] Buat file .aspx.vb dengan Partial Public Class
[ ] Buat file .aspx.designer.vb dengan Protected WithEvents declarations
[ ] Tambahkan ketiga file ke ERP_JasaKonsultasi.vbproj
[ ] Tambahkan AuthHelper.RequireRole() di Page_Load kalau halaman terbatas
[ ] Tambahkan nav link di Site.master (markup + code-behind active state)
[ ] Tambahkan link nav di Site.master.designer.vb
[ ] Test semua role (Admin, Manager, Staff) untuk memastikan akses benar
[ ] Tidak ada emoji di markup/code (gunakan SVG atau teks biasa)
[ ] Tidak ada inline style yang bisa digantikan dengan Tailwind/CSS class
```

## 🧪 Checklist Sebelum Pull Request

```
[ ] Rebuild Solution berhasil tanpa error (Build → Rebuild Solution)
[ ] Tidak ada CodeFile= tersisa (harus CodeBehind=)
[ ] Tidak ada duplikat di .vbproj (Compile Include yang muncul 2x)
[ ] Tidak ada emoji/karakter non-ASCII di file .aspx dan .vb
[ ] Web.config tidak ikut di commit (ada di .gitignore)
[ ] Semua form sudah ditest di browser (Chrome, minimal)
[ ] Role-based access berfungsi dengan benar
[ ] Tidak ada SQL injection vulnerability (semua query pakai parameter)
```

## 📝 Pesan Commit

Format: `type(scope): deskripsi singkat`

| Type | Kapan dipakai |
|------|--------------|
| `feat` | Fitur baru |
| `fix` | Perbaikan bug |
| `docs` | Perubahan dokumentasi saja |
| `style` | Perubahan CSS/tampilan (tidak ada logic change) |
| `refactor` | Refactoring kode (tidak ada fitur/bug baru) |
| `perf` | Peningkatan performa |
| `chore` | Update dependency, maintenance, dll |

**Contoh:**
```
feat(finance): tambah laporan cashflow bulanan
fix(payroll): PeriodId harus Protected supaya bisa diakses dari markup
docs(readme): tambah panduan deploy ke Azure
style(dashboard): update KPI card dengan Tailwind gradient
```

## 🐛 Melaporkan Bug

Buka Issue baru dengan template berikut:

```markdown
## Deskripsi Bug
[Jelaskan bug secara singkat dan jelas]

## Langkah Reproduksi
1. Login sebagai [admin/manager/staff]
2. Buka halaman [...]
3. Klik [...]
4. Error muncul: [...]

## Yang Diharapkan
[Apa yang seharusnya terjadi]

## Screenshot
[Lampirkan screenshot kalau ada]

## Environment
- VS: 2022 version [...]
- SQL Server: 2022 / 2019 / Express
- Browser: Chrome [version] / Edge [version]
```

## 💡 Mengusulkan Fitur Baru

Buka Issue dengan label `enhancement` dan jelaskan:
- **Problem yang diselesaikan** — kenapa fitur ini dibutuhkan?
- **Solusi yang diusulkan** — bagaimana cara kerjanya?
- **Alternatif yang sudah dipertimbangkan** — ada cara lain?
- **Modul yang terpengaruh** — Finance? HR? Sales?

---

Pertanyaan? Buka Discussion di GitHub atau kontak maintainer lewat Issues.
