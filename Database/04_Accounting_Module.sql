/* =====================================================================
   TransformHQ - Modul Akuntansi (Chart of Accounts & Jurnal Umum)
   Script TAMBAHAN - jalankan ini SETELAH script 01, 02, 03 sudah pernah
   dieksekusi sebelumnya. Hanya menambah tabel baru, tidak mengubah data lain.
   ===================================================================== */

USE ERP_JasaKonsultasi;
GO

/* ---------------------------------------------------------------------
   1. CHART OF ACCOUNTS (Daftar Akun)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.ChartOfAccounts', 'U') IS NOT NULL DROP TABLE dbo.ChartOfAccounts;
GO
CREATE TABLE dbo.ChartOfAccounts (
    AccountID       INT IDENTITY(1,1) PRIMARY KEY,
    AccountCode     NVARCHAR(20)  NOT NULL UNIQUE,
    AccountName     NVARCHAR(150) NOT NULL,
    AccountType     NVARCHAR(20)  NOT NULL,  -- Asset / Liability / Equity / Revenue / Expense
    NormalBalance   NVARCHAR(10)  NOT NULL,  -- Debit / Credit
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   2. JOURNAL ENTRIES (Header Jurnal)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.JournalEntries', 'U') IS NOT NULL DROP TABLE dbo.JournalEntries;
GO
CREATE TABLE dbo.JournalEntries (
    JournalEntryID  INT IDENTITY(1,1) PRIMARY KEY,
    EntryNo         NVARCHAR(30)  NOT NULL UNIQUE,
    EntryDate       DATETIME NOT NULL DEFAULT GETDATE(),
    Description     NVARCHAR(300) NOT NULL,
    SourceType      NVARCHAR(20)  NOT NULL DEFAULT 'Manual', -- Manual / Invoice / Payment / Payroll
    SourceID        INT NULL,  -- ID referensi ke tabel sumber (InvoiceID/PaymentID/PayrollPeriodID), NULL kalau Manual
    CreatedBy       NVARCHAR(50) NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   3. JOURNAL ENTRY LINES (Detail Debit/Kredit)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.JournalEntryLines', 'U') IS NOT NULL DROP TABLE dbo.JournalEntryLines;
GO
CREATE TABLE dbo.JournalEntryLines (
    LineID          INT IDENTITY(1,1) PRIMARY KEY,
    JournalEntryID  INT NOT NULL REFERENCES dbo.JournalEntries(JournalEntryID) ON DELETE CASCADE,
    AccountID       INT NOT NULL REFERENCES dbo.ChartOfAccounts(AccountID),
    Debit           DECIMAL(18,2) NOT NULL DEFAULT 0,
    Credit          DECIMAL(18,2) NOT NULL DEFAULT 0,
    Notes           NVARCHAR(300) NULL
);
GO

/* =====================================================================
   SEED DATA: Chart of Accounts standar untuk bisnis jasa/konsultasi
   ===================================================================== */

INSERT INTO dbo.ChartOfAccounts (AccountCode, AccountName, AccountType, NormalBalance) VALUES
(N'1-1000', N'Kas',                          N'Asset',     N'Debit'),
(N'1-1100', N'Bank',                         N'Asset',     N'Debit'),
(N'1-1200', N'Piutang Usaha',                N'Asset',     N'Debit'),
(N'1-1300', N'Persediaan Barang',            N'Asset',     N'Debit'),
(N'1-2000', N'Peralatan',                    N'Asset',     N'Debit'),
(N'2-1000', N'Hutang Usaha',                 N'Liability', N'Credit'),
(N'2-1100', N'Hutang Gaji',                  N'Liability', N'Credit'),
(N'3-1000', N'Modal Pemilik',                N'Equity',    N'Credit'),
(N'3-2000', N'Laba Ditahan',                 N'Equity',    N'Credit'),
(N'4-1000', N'Pendapatan Jasa',              N'Revenue',   N'Credit'),
(N'4-1100', N'Pendapatan Penjualan Barang',  N'Revenue',   N'Credit'),
(N'5-1000', N'Beban Gaji',                   N'Expense',   N'Debit'),
(N'5-1100', N'Beban Operasional',            N'Expense',   N'Debit'),
(N'5-1200', N'Beban Lain-lain',              N'Expense',   N'Debit');

PRINT 'Modul Akuntansi (ChartOfAccounts, JournalEntries, JournalEntryLines) berhasil ditambahkan.';
GO
