// =====================================================================
// ERP Jasa & Konsultasi - site.js
// Fungsi JS umum yang dipakai di berbagai halaman
// =====================================================================

// Konfirmasi sebelum hapus data (dipanggil dari OnClientClick tombol Delete)
function confirmDelete(itemName) {
    return confirm("Yakin ingin menghapus '" + itemName + "'? Tindakan ini tidak dapat dibatalkan.");
}

// =====================================================================
// DataTables: otomatis aktifkan sorting, search, & pagination di SEMUA
// tabel dengan class "grid" (yaitu semua GridView di seluruh aplikasi)
// tanpa perlu konfigurasi tambahan per halaman.
// =====================================================================
var DATATABLES_ID_BAHASA = {
    "search": "Cari:",
    "lengthMenu": "Tampilkan _MENU_ baris",
    "info": "Menampilkan _START_ - _END_ dari _TOTAL_ data",
    "infoEmpty": "Tidak ada data",
    "infoFiltered": "(disaring dari _MAX_ total data)",
    "zeroRecords": "Data tidak ditemukan",
    "paginate": { "first": "Awal", "last": "Akhir", "next": "Selanjutnya", "previous": "Sebelumnya" }
};

$(document).ready(function () {
    $("table.grid").each(function () {
        // Lewati tabel yang sedang kosong (GridView EmptyDataText tidak punya <thead>,
        // jadi DataTables tidak bisa diterapkan di situ)
        if ($(this).find("thead tr").length === 0) return;

        $(this).DataTable({
            language: DATATABLES_ID_BAHASA,
            pageLength: 10,
            lengthChange: false,
            columnDefs: [
                { orderable: false, targets: -1 } // kolom terakhir (Aksi) tidak perlu sorting
            ],
            dom: '<"flex items-center justify-between mb-3"<"text-xs text-slate-400"i>f>t<"flex items-center justify-between mt-3"p>',
        });
    });
});

// Format angka jadi format Rupiah sederhana (untuk tampilan live di form)
function formatRupiah(value) {
    var num = parseFloat(value);
    if (isNaN(num)) return "Rp 0";
    return "Rp " + num.toLocaleString("id-ID", { maximumFractionDigits: 2 });
}

// Hitung ulang total baris (Qty x Harga) secara live di form Sales Order.
// qtyId & priceId = ID textbox qty/harga, totalSpanId = ID elemen span untuk menampilkan hasil
function recalcLineTotal(qtyId, priceId, totalSpanId) {
    var qty = parseFloat(document.getElementById(qtyId).value) || 0;
    var price = parseFloat(document.getElementById(priceId).value) || 0;
    var total = qty * price;
    var el = document.getElementById(totalSpanId);
    if (el) {
        el.textContent = formatRupiah(total);
    }
    recalcGrandTotal();
}

// Menjumlahkan semua elemen dengan class "line-total-value" menjadi grand total
function recalcGrandTotal() {
    var totalEls = document.querySelectorAll(".line-total-raw");
    var sum = 0;
    totalEls.forEach(function (el) {
        var qty = parseFloat(el.getAttribute("data-qty")) || 0;
        var price = parseFloat(el.getAttribute("data-price")) || 0;
        sum += qty * price;
    });
    var grandEl = document.getElementById("lblGrandTotalClient");
    if (grandEl) {
        grandEl.textContent = formatRupiah(sum);
    }
}

document.addEventListener("DOMContentLoaded", function () {
    recalcGrandTotal();
});
