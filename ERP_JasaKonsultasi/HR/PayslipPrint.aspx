<%@ Page Language="VB" AutoEventWireup="true" CodeBehind="PayslipPrint.aspx.vb" Inherits="HR_PayslipPrint" %>

<!DOCTYPE html>
<html lang="id">
<head runat="server">
    <meta charset="utf-8" />
    <title>Slip Gaji</title>
    <link rel="stylesheet" href="../Content/site.css" />
    <style>
        body { background: #fff; padding: 40px; max-width: 700px; margin: 0 auto; }
        .print-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 30px; border-bottom: 3px solid #2563eb; padding-bottom: 16px; }
        .print-header h1 { margin: 0; font-size: 24px; }
        .print-header .doc-title { text-align: right; }
        .print-header .doc-title h2 { margin: 0; color: #2563eb; }
        .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-bottom: 24px; }
        .info-box .label { font-size: 12px; color: #64748b; text-transform: uppercase; }
        .info-box .value { font-size: 15px; font-weight: 600; }
        table.totals { width: 100%; margin-top: 16px; }
        table.totals td { padding: 8px 0; }
        table.totals tr.deduction td { color: #dc2626; }
        table.totals tr.grand-total td { font-size: 18px; font-weight: 700; border-top: 2px solid #1e293b; padding-top: 12px; }
        .no-print { margin-top: 30px; }
        .no-print button { padding: 10px 20px; font-size: 14px; font-weight: 600; border-radius: 6px; border: none; background: #2563eb; color: #fff; cursor: pointer; }
        @media print {
            .no-print { display: none; }
            body { padding: 0; }
        }
    </style>
</head>
<body>
    <!-- Catatan: halaman ini SENGAJA tidak memakai asp:Literal/asp:Button server control sama
         sekali (cukup <%= %> inline expression ke Public Property + tombol HTML biasa).
         Tujuannya supaya tidak ada field apapun yang perlu dideklarasikan manual,
         jadi tidak mungkin terjadi konflik "duplicate declaration" dengan auto-generate VS. -->
    <div class="print-header">
        <div>
            <h1>TransformHQ</h1>
            <p class="hint">Slip Gaji Karyawan</p>
        </div>
        <div class="doc-title">
            <h2>SLIP GAJI</h2>
            <p><%= PeriodNameText %></p>
        </div>
    </div>

    <div class="info-grid">
        <div class="info-box">
            <div class="label">Nama Karyawan</div>
            <div class="value"><%= EmployeeNameText %></div>
            <div class="label" style="margin-top:8px;">Jabatan</div>
            <div class="value"><%= PositionText %></div>
        </div>
        <div class="info-box">
            <div class="label">Kode Karyawan</div>
            <div class="value"><%= EmployeeCodeText %></div>
            <div class="label" style="margin-top:8px;">Tanggal Bayar</div>
            <div class="value"><%= PayDateText %></div>
        </div>
    </div>

    <div class="card">
        <table class="totals">
            <tr>
                <td>Gaji Pokok</td>
                <td style="text-align:right;"><%= BaseSalaryText %></td>
            </tr>
            <tr>
                <td>Tunjangan</td>
                <td style="text-align:right;"><%= AllowanceText %></td>
            </tr>
            <tr class="deduction">
                <td>Potongan</td>
                <td style="text-align:right;">- <%= DeductionText %></td>
            </tr>
            <tr class="grand-total">
                <td>Take Home Pay</td>
                <td style="text-align:right;"><%= NetSalaryText %></td>
            </tr>
        </table>
    </div>

    <p class="hint">Dokumen ini dibuat otomatis oleh sistem TransformHQ dan sah tanpa tanda tangan basah.</p>

    <div class="no-print">
        <button type="button" onclick="window.print();">Print / Simpan sebagai PDF</button>
    </div>
</body>
</html>
