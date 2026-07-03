/* =====================================================================
   TransformHQ - Modul HR & Payroll
   Script TAMBAHAN - jalankan ini SETELAH script 01 (dan 02 jika ada) sudah
   pernah dieksekusi sebelumnya. Script ini hanya menambah tabel baru
   (Employees, PayrollPeriods, PayrollDetails) tanpa mengubah data lain.
   ===================================================================== */

USE ERP_JasaKonsultasi;
GO

/* ---------------------------------------------------------------------
   1. HR - EMPLOYEES (Master Karyawan)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;
GO
CREATE TABLE dbo.Employees (
    EmployeeID      INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode    NVARCHAR(30)  NOT NULL UNIQUE,
    FullName        NVARCHAR(100) NOT NULL,
    Position        NVARCHAR(100) NULL,
    Department      NVARCHAR(100) NULL,
    Email           NVARCHAR(100) NULL,
    Phone           NVARCHAR(30)  NULL,
    Address         NVARCHAR(300) NULL,
    HireDate        DATE NULL,
    Status          NVARCHAR(20)  NOT NULL DEFAULT 'Active', -- Active / Resigned
    BaseSalary      DECIMAL(18,2) NOT NULL DEFAULT 0,
    BankName        NVARCHAR(50)  NULL,
    BankAccountNo   NVARCHAR(50)  NULL,
    UserID          INT NULL REFERENCES dbo.Users(UserID), -- opsional: hubungkan ke akun login
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   2. HR - PAYROLL PERIODS (Header per periode gajian, contoh: "Juli 2026")
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.PayrollPeriods', 'U') IS NOT NULL DROP TABLE dbo.PayrollPeriods;
GO
CREATE TABLE dbo.PayrollPeriods (
    PayrollPeriodID INT IDENTITY(1,1) PRIMARY KEY,
    PeriodName      NVARCHAR(50) NOT NULL,   -- contoh: "Juli 2026"
    PeriodMonth     INT NOT NULL,
    PeriodYear      INT NOT NULL,
    PayDate         DATE NULL,
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Draft', -- Draft / Finalized
    CreatedBy       NVARCHAR(50) NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Payroll_Period UNIQUE (PeriodMonth, PeriodYear)
);
GO

/* ---------------------------------------------------------------------
   3. HR - PAYROLL DETAILS (rincian gaji per karyawan per periode)
   NetSalary dihitung otomatis oleh SQL Server (computed column),
   jadi tidak perlu dihitung manual di kode - selalu konsisten.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.PayrollDetails', 'U') IS NOT NULL DROP TABLE dbo.PayrollDetails;
GO
CREATE TABLE dbo.PayrollDetails (
    PayrollDetailID INT IDENTITY(1,1) PRIMARY KEY,
    PayrollPeriodID INT NOT NULL REFERENCES dbo.PayrollPeriods(PayrollPeriodID) ON DELETE CASCADE,
    EmployeeID      INT NOT NULL REFERENCES dbo.Employees(EmployeeID),
    BaseSalary      DECIMAL(18,2) NOT NULL DEFAULT 0,
    Allowance       DECIMAL(18,2) NOT NULL DEFAULT 0,  -- Tunjangan
    Deduction       DECIMAL(18,2) NOT NULL DEFAULT 0,  -- Potongan (BPJS, PPh21, dll)
    NetSalary       AS (BaseSalary + Allowance - Deduction) PERSISTED,
    Notes           NVARCHAR(300) NULL
);
GO

/* =====================================================================
   SEED DATA contoh
   ===================================================================== */

INSERT INTO dbo.Employees (EmployeeCode, FullName, Position, Department, Email, Phone, HireDate, Status, BaseSalary, BankName, BankAccountNo)
VALUES
 (N'EMP-001', N'Rina Kusuma', N'Project Manager', N'Operasional', N'rina@transformhq.co.id', N'0812-1111-2222', '2024-03-01', N'Active', 12000000, N'BCA', N'1234567890'),
 (N'EMP-002', N'Dedi Pratama', N'Konsultan Senior', N'Konsultasi', N'dedi@transformhq.co.id', N'0812-3333-4444', '2024-06-15', N'Active', 9500000, N'Mandiri', N'0987654321'),
 (N'EMP-003', N'Sari Wulandari', N'Staff Administrasi', N'Administrasi', N'sari@transformhq.co.id', N'0812-5555-6666', '2025-01-10', N'Active', 5500000, N'BNI', N'1122334455');

INSERT INTO dbo.PayrollPeriods (PeriodName, PeriodMonth, PeriodYear, PayDate, Status, CreatedBy)
VALUES (N'Juni 2026', 6, 2026, '2026-06-28', N'Draft', N'admin');

INSERT INTO dbo.PayrollDetails (PayrollPeriodID, EmployeeID, BaseSalary, Allowance, Deduction)
VALUES
 (1, 1, 12000000, 1000000, 600000),
 (1, 2, 9500000, 750000, 475000),
 (1, 3, 5500000, 500000, 275000);

/* Contoh akun Manager & Staff untuk uji coba Role-Based Access Control.
   Lewati INSERT ini kalau username sudah ada (jalankan satu per satu / cek dulu). */
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'manager')
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role)
VALUES ('manager', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'Budi Manager', 'Manager');

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'staff')
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role)
VALUES ('staff', '10176e7b7b24d317acfcf8d2064cfd2f24e154f7b5a96603077d5ef813d6a6b6', 'Citra Staff', 'Staff');

PRINT 'Modul HR & Payroll (Employees, PayrollPeriods, PayrollDetails) berhasil ditambahkan.';
GO
