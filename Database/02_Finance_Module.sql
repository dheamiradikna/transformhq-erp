/* =====================================================================
   TransformHQ - Modul Akuntansi/Keuangan (Invoice & Pembayaran)
   Script TAMBAHAN - jalankan ini SETELAH script 01_Create_Database_And_Schema.sql
   sudah pernah dieksekusi sebelumnya. Script ini hanya menambah 2 tabel baru
   (Invoices & Payments) tanpa mengubah/menghapus data yang sudah ada.
   ===================================================================== */

USE ERP_JasaKonsultasi;
GO

/* ---------------------------------------------------------------------
   1. FINANCE - INVOICES (Tagihan ke customer)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Invoices', 'U') IS NOT NULL DROP TABLE dbo.Invoices;
GO
CREATE TABLE dbo.Invoices (
    InvoiceID       INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNo       NVARCHAR(30)  NOT NULL UNIQUE,
    SalesOrderID    INT NULL REFERENCES dbo.SalesOrders(SalesOrderID),
    CustomerID      INT NOT NULL REFERENCES dbo.Customers(CustomerID),
    InvoiceDate     DATETIME NOT NULL DEFAULT GETDATE(),
    DueDate         DATE NULL,
    TotalAmount     DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status          NVARCHAR(20)  NOT NULL DEFAULT 'Unpaid', -- Unpaid/PartiallyPaid/Paid/Cancelled
    Notes           NVARCHAR(300) NULL,
    CreatedBy       NVARCHAR(50)  NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   2. FINANCE - PAYMENTS (Riwayat pembayaran per invoice)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL DROP TABLE dbo.Payments;
GO
CREATE TABLE dbo.Payments (
    PaymentID       INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceID       INT NOT NULL REFERENCES dbo.Invoices(InvoiceID) ON DELETE CASCADE,
    PaymentDate     DATETIME NOT NULL DEFAULT GETDATE(),
    Amount          DECIMAL(18,2) NOT NULL,
    PaymentMethod   NVARCHAR(30) NOT NULL DEFAULT 'Transfer', -- Cash/Transfer/Credit Card/dll
    Reference       NVARCHAR(100) NULL,
    Notes           NVARCHAR(300) NULL,
    CreatedBy       NVARCHAR(50) NULL
);
GO

/* =====================================================================
   SEED DATA contoh: buat 1 invoice dari Sales Order yang sudah ada (SO-2026-0001)
   beserta 1 pembayaran parsial, supaya modul langsung bisa dicoba.
   ===================================================================== */

INSERT INTO dbo.Invoices (InvoiceNo, SalesOrderID, CustomerID, InvoiceDate, DueDate, TotalAmount, Status, Notes, CreatedBy)
VALUES (N'INV-2026-0001', 1, 1, GETDATE(), DATEADD(DAY, 14, GETDATE()), 17500000, N'PartiallyPaid', N'Invoice untuk proyek audit awal', N'admin');

INSERT INTO dbo.Payments (InvoiceID, Amount, PaymentMethod, Reference, Notes, CreatedBy)
VALUES (1, 7500000, N'Transfer', N'TRF-001', N'Pembayaran tahap 1 (DP)', N'admin');

PRINT 'Modul Keuangan (Invoices & Payments) berhasil ditambahkan ke database ERP_JasaKonsultasi.';
GO
