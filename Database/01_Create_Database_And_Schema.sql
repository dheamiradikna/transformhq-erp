/* =====================================================================
   ERP Jasa & Konsultasi - Database Schema
   Target: Microsoft SQL Server 2022
   Cara pakai: Buka file ini di SSMS (SQL Server Management Studio)
                yang sudah terkoneksi ke SQL Server 2022, lalu Execute (F5).
   ===================================================================== */

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ERP_JasaKonsultasi')
BEGIN
    CREATE DATABASE ERP_JasaKonsultasi;
END
GO

USE ERP_JasaKonsultasi;
GO

/* ---------------------------------------------------------------------
   1. USERS  (Login aplikasi)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO
CREATE TABLE dbo.Users (
    UserID          INT IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(128) NOT NULL,   -- SHA256 hex (lowercase)
    FullName        NVARCHAR(100) NOT NULL,
    Role            NVARCHAR(30)  NOT NULL DEFAULT 'Staff', -- Admin / Manager / Staff
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   2. CRM - CUSTOMERS  (sekaligus menampung Lead -> Customer pipeline)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
GO
CREATE TABLE dbo.Customers (
    CustomerID      INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName     NVARCHAR(150) NOT NULL,
    ContactName     NVARCHAR(100) NULL,
    Email           NVARCHAR(100) NULL,
    Phone           NVARCHAR(30)  NULL,
    Address         NVARCHAR(300) NULL,
    Industry        NVARCHAR(100) NULL,
    Status          NVARCHAR(20)  NOT NULL DEFAULT 'Lead', -- Lead / Active / Inactive
    Notes           NVARCHAR(MAX) NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   3. INVENTORY - WAREHOUSES
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Warehouses', 'U') IS NOT NULL DROP TABLE dbo.Warehouses;
GO
CREATE TABLE dbo.Warehouses (
    WarehouseID     INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseName   NVARCHAR(100) NOT NULL,
    Location        NVARCHAR(200) NULL,
    IsActive        BIT NOT NULL DEFAULT 1
);
GO

/* ---------------------------------------------------------------------
   4. INVENTORY - ITEMS (barang & paket jasa)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Items', 'U') IS NOT NULL DROP TABLE dbo.Items;
GO
CREATE TABLE dbo.Items (
    ItemID          INT IDENTITY(1,1) PRIMARY KEY,
    ItemCode        NVARCHAR(30)  NOT NULL UNIQUE,
    ItemName        NVARCHAR(150) NOT NULL,
    ItemType        NVARCHAR(20)  NOT NULL DEFAULT 'Goods', -- Goods / Service
    Category        NVARCHAR(100) NULL,
    Unit            NVARCHAR(20)  NOT NULL DEFAULT 'Pcs',
    UnitPrice       DECIMAL(18,2) NOT NULL DEFAULT 0,
    ReorderLevel    INT NOT NULL DEFAULT 0,
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   5. INVENTORY - STOCK BALANCE (saldo stok per item per gudang)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.StockBalances', 'U') IS NOT NULL DROP TABLE dbo.StockBalances;
GO
CREATE TABLE dbo.StockBalances (
    StockID         INT IDENTITY(1,1) PRIMARY KEY,
    ItemID          INT NOT NULL REFERENCES dbo.Items(ItemID),
    WarehouseID     INT NOT NULL REFERENCES dbo.Warehouses(WarehouseID),
    QtyOnHand       DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT UQ_Stock_Item_Warehouse UNIQUE (ItemID, WarehouseID)
);
GO

/* ---------------------------------------------------------------------
   6. INVENTORY - STOCK MOVEMENTS (mutasi stok masuk/keluar)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.StockMovements', 'U') IS NOT NULL DROP TABLE dbo.StockMovements;
GO
CREATE TABLE dbo.StockMovements (
    MovementID      INT IDENTITY(1,1) PRIMARY KEY,
    ItemID          INT NOT NULL REFERENCES dbo.Items(ItemID),
    WarehouseID     INT NOT NULL REFERENCES dbo.Warehouses(WarehouseID),
    MovementType    NVARCHAR(10) NOT NULL,  -- IN / OUT
    Quantity        DECIMAL(18,2) NOT NULL,
    MovementDate    DATETIME NOT NULL DEFAULT GETDATE(),
    Reference       NVARCHAR(100) NULL,     -- contoh: nomor SO, atau manual
    Notes           NVARCHAR(300) NULL,
    CreatedBy       NVARCHAR(50) NULL
);
GO

/* ---------------------------------------------------------------------
   7. SALES - SALES ORDERS (header)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.SalesOrders', 'U') IS NOT NULL DROP TABLE dbo.SalesOrders;
GO
CREATE TABLE dbo.SalesOrders (
    SalesOrderID    INT IDENTITY(1,1) PRIMARY KEY,
    OrderNo         NVARCHAR(30) NOT NULL UNIQUE,
    CustomerID      INT NOT NULL REFERENCES dbo.Customers(CustomerID),
    OrderDate       DATETIME NOT NULL DEFAULT GETDATE(),
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Draft', -- Draft/Confirmed/Completed/Cancelled
    TotalAmount     DECIMAL(18,2) NOT NULL DEFAULT 0,
    Notes           NVARCHAR(300) NULL,
    CreatedBy       NVARCHAR(50) NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   8. SALES - SALES ORDER ITEMS (detail/line items)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.SalesOrderItems', 'U') IS NOT NULL DROP TABLE dbo.SalesOrderItems;
GO
CREATE TABLE dbo.SalesOrderItems (
    SalesOrderItemID INT IDENTITY(1,1) PRIMARY KEY,
    SalesOrderID     INT NOT NULL REFERENCES dbo.SalesOrders(SalesOrderID) ON DELETE CASCADE,
    ItemID           INT NOT NULL REFERENCES dbo.Items(ItemID),
    Quantity         DECIMAL(18,2) NOT NULL,
    UnitPrice        DECIMAL(18,2) NOT NULL,
    LineTotal        DECIMAL(18,2) NOT NULL
);
GO

/* ---------------------------------------------------------------------
   9. FINANCE - INVOICES (Tagihan ke customer)
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
   10. FINANCE - PAYMENTS (Riwayat pembayaran per invoice)
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

/* ---------------------------------------------------------------------
   11. PROJECTS - PROJECT DELIVERY (pengganti modul "Produksi" untuk bisnis jasa)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Projects', 'U') IS NOT NULL DROP TABLE dbo.Projects;
GO
CREATE TABLE dbo.Projects (
    ProjectID       INT IDENTITY(1,1) PRIMARY KEY,
    ProjectCode     NVARCHAR(30) NOT NULL UNIQUE,
    ProjectName     NVARCHAR(150) NOT NULL,
    CustomerID      INT NULL REFERENCES dbo.Customers(CustomerID),
    SalesOrderID    INT NULL REFERENCES dbo.SalesOrders(SalesOrderID),
    StartDate       DATE NULL,
    EndDate         DATE NULL,
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Planning', -- Planning/InProgress/OnHold/Completed
    ProjectManager  NVARCHAR(100) NULL,
    Budget          DECIMAL(18,2) NULL DEFAULT 0,
    Description     NVARCHAR(MAX) NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   12. PROJECTS - PROJECT TASKS
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.ProjectTasks', 'U') IS NOT NULL DROP TABLE dbo.ProjectTasks;
GO
CREATE TABLE dbo.ProjectTasks (
    TaskID          INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID       INT NOT NULL REFERENCES dbo.Projects(ProjectID) ON DELETE CASCADE,
    TaskName        NVARCHAR(200) NOT NULL,
    AssignedTo      NVARCHAR(100) NULL,
    StartDate       DATE NULL,
    DueDate         DATE NULL,
    Status          NVARCHAR(20) NOT NULL DEFAULT 'ToDo', -- ToDo/InProgress/Done
    Priority        NVARCHAR(10) NOT NULL DEFAULT 'Medium', -- Low/Medium/High
    Notes           NVARCHAR(300) NULL
);
GO

/* ---------------------------------------------------------------------
   13. HR - EMPLOYEES (Master Karyawan)
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
    UserID          INT NULL REFERENCES dbo.Users(UserID),
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* ---------------------------------------------------------------------
   14. HR - PAYROLL PERIODS
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.PayrollPeriods', 'U') IS NOT NULL DROP TABLE dbo.PayrollPeriods;
GO
CREATE TABLE dbo.PayrollPeriods (
    PayrollPeriodID INT IDENTITY(1,1) PRIMARY KEY,
    PeriodName      NVARCHAR(50) NOT NULL,
    PeriodMonth     INT NOT NULL,
    PeriodYear      INT NOT NULL,
    PayDate         DATE NULL,
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Draft',
    CreatedBy       NVARCHAR(50) NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Payroll_Period UNIQUE (PeriodMonth, PeriodYear)
);
GO

/* ---------------------------------------------------------------------
   15. HR - PAYROLL DETAILS
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.PayrollDetails', 'U') IS NOT NULL DROP TABLE dbo.PayrollDetails;
GO
CREATE TABLE dbo.PayrollDetails (
    PayrollDetailID INT IDENTITY(1,1) PRIMARY KEY,
    PayrollPeriodID INT NOT NULL REFERENCES dbo.PayrollPeriods(PayrollPeriodID) ON DELETE CASCADE,
    EmployeeID      INT NOT NULL REFERENCES dbo.Employees(EmployeeID),
    BaseSalary      DECIMAL(18,2) NOT NULL DEFAULT 0,
    Allowance       DECIMAL(18,2) NOT NULL DEFAULT 0,
    Deduction       DECIMAL(18,2) NOT NULL DEFAULT 0,
    NetSalary       AS (BaseSalary + Allowance - Deduction) PERSISTED,
    Notes           NVARCHAR(300) NULL
);
GO

/* =====================================================================
   SEED DATA (data contoh agar aplikasi langsung bisa dicoba)
   ===================================================================== */

-- Default login: username = admin / password = admin123 (Role: Admin)
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role)
VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Administrator', 'Admin');

-- Contoh akun Manager: username = manager / password = manager123
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role)
VALUES ('manager', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'Budi Manager', 'Manager');

-- Contoh akun Staff: username = staff / password = staff123
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Role)
VALUES ('staff', '10176e7b7b24d317acfcf8d2064cfd2f24e154f7b5a96603077d5ef813d6a6b6', 'Citra Staff', 'Staff');

INSERT INTO dbo.Customers (CompanyName, ContactName, Email, Phone, Address, Industry, Status)
VALUES
 (N'PT Maju Bersama', N'Andi Wijaya', N'andi@majubersama.co.id', N'021-5551234', N'Jakarta Selatan', N'Manufaktur', N'Active'),
 (N'CV Sumber Rejeki', N'Siti Nurhaliza', N'siti@sumberrejeki.co.id', N'022-7771234', N'Bandung', N'Retail', N'Lead'),
 (N'PT Teknologi Cerdas', N'Budi Santoso', N'budi@teknologicerdas.com', N'031-9991234', N'Surabaya', N'IT', N'Active');

INSERT INTO dbo.Warehouses (WarehouseName, Location)
VALUES (N'Gudang Pusat', N'Jakarta'), (N'Gudang Cabang Bandung', N'Bandung');

INSERT INTO dbo.Items (ItemCode, ItemName, ItemType, Category, Unit, UnitPrice, ReorderLevel)
VALUES
 (N'SVC-001', N'Jasa Konsultasi Strategi (per hari)', N'Service', N'Konsultasi', N'Hari', 2500000, 0),
 (N'SVC-002', N'Jasa Audit Sistem', N'Service', N'Konsultasi', N'Paket', 15000000, 0),
 (N'GDS-001', N'Laptop Standar Konsultan', N'Goods', N'Peralatan', N'Unit', 12000000, 2),
 (N'GDS-002', N'ATK Paket Proyek', N'Goods', N'Peralatan', N'Paket', 150000, 5);

INSERT INTO dbo.StockBalances (ItemID, WarehouseID, QtyOnHand)
VALUES (3, 1, 5), (4, 1, 20), (3, 2, 1), (4, 2, 8);

INSERT INTO dbo.SalesOrders (OrderNo, CustomerID, Status, TotalAmount, Notes, CreatedBy)
VALUES (N'SO-2026-0001', 1, N'Confirmed', 17500000, N'Proyek audit awal', N'admin');

INSERT INTO dbo.SalesOrderItems (SalesOrderID, ItemID, Quantity, UnitPrice, LineTotal)
VALUES (1, 2, 1, 15000000, 15000000), (1, 1, 1, 2500000, 2500000);

INSERT INTO dbo.Invoices (InvoiceNo, SalesOrderID, CustomerID, InvoiceDate, DueDate, TotalAmount, Status, Notes, CreatedBy)
VALUES (N'INV-2026-0001', 1, 1, GETDATE(), DATEADD(DAY, 14, GETDATE()), 17500000, N'PartiallyPaid', N'Invoice untuk proyek audit awal', N'admin');

INSERT INTO dbo.Payments (InvoiceID, Amount, PaymentMethod, Reference, Notes, CreatedBy)
VALUES (1, 7500000, N'Transfer', N'TRF-001', N'Pembayaran tahap 1 (DP)', N'admin');

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

INSERT INTO dbo.Projects (ProjectCode, ProjectName, CustomerID, SalesOrderID, StartDate, EndDate, Status, ProjectManager, Budget, Description)
VALUES (N'PRJ-2026-001', N'Audit Sistem Keuangan - PT Maju Bersama', 1, 1, '2026-07-01', '2026-08-15', N'Planning', N'Rina Kusuma', 17500000, N'Audit sistem keuangan dan rekomendasi perbaikan proses.');

INSERT INTO dbo.ProjectTasks (ProjectID, TaskName, AssignedTo, StartDate, DueDate, Status, Priority)
VALUES
 (1, N'Kick-off meeting dengan klien', N'Rina Kusuma', '2026-07-01', '2026-07-02', N'ToDo', N'High'),
 (1, N'Pengumpulan dokumen keuangan', N'Dedi Pratama', '2026-07-03', '2026-07-10', N'ToDo', N'Medium');

PRINT 'Database ERP_JasaKonsultasi berhasil dibuat dan diisi data contoh.';
GO
