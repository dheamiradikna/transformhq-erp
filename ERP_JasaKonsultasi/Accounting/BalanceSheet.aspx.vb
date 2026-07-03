Imports System.Data.SqlClient

Partial Public Class Accounting_BalanceSheet
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Admin", "Manager")

        If Not IsPostBack Then
            txtAsOfDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            BindReport()
        End If
    End Sub

    Protected Sub txtAsOfDate_TextChanged(sender As Object, e As EventArgs)
        BindReport()
    End Sub

    ''' <summary>Saldo akun Asset/Liability/Equity per tanggal, dihitung sesuai saldo normalnya
    ''' (Asset & Liability: Debit - Credit untuk Asset, Credit - Debit untuk Liability/Equity).</summary>
    Private Function GetBalanceForType(accountType As String, asOfDate As DateTime) As DataTable
        Dim amountExpr As String = If(accountType = "Asset", "SUM(jel.Debit) - SUM(jel.Credit)", "SUM(jel.Credit) - SUM(jel.Debit)")

        Dim sql As String =
            "SELECT coa.AccountName, " & amountExpr & " AS Amount " &
            "FROM dbo.ChartOfAccounts coa " &
            "INNER JOIN dbo.JournalEntryLines jel ON jel.AccountID = coa.AccountID " &
            "INNER JOIN dbo.JournalEntries je ON jel.JournalEntryID = je.JournalEntryID " &
            "WHERE coa.AccountType = @AccountType AND je.EntryDate <= @AsOfDate " &
            "GROUP BY coa.AccountName " &
            "HAVING " & amountExpr & " <> 0 " &
            "ORDER BY coa.AccountName"

        Return DBHelper.ExecuteQuery(sql,
            New SqlParameter("@AccountType", accountType),
            New SqlParameter("@AsOfDate", asOfDate.AddDays(1).AddSeconds(-1)))
    End Function

    ''' <summary>Laba/Rugi kumulatif sejak awal (semua transaksi Revenue/Expense sampai asOfDate) -
    ''' ditambahkan ke bagian Equity sebagai "Laba Tahun Berjalan", supaya Neraca balance
    ''' (Aset = Kewajiban + Modal), mengikuti prinsip akuntansi dasar.</summary>
    Private Function GetCumulativeNetIncome(asOfDate As DateTime) As Decimal
        Dim sql As String =
            "SELECT " &
            "ISNULL(SUM(CASE WHEN coa.AccountType = 'Revenue' THEN jel.Credit - jel.Debit ELSE 0 END), 0) - " &
            "ISNULL(SUM(CASE WHEN coa.AccountType = 'Expense' THEN jel.Debit - jel.Credit ELSE 0 END), 0) AS NetIncome " &
            "FROM dbo.JournalEntryLines jel " &
            "INNER JOIN dbo.ChartOfAccounts coa ON jel.AccountID = coa.AccountID " &
            "INNER JOIN dbo.JournalEntries je ON jel.JournalEntryID = je.JournalEntryID " &
            "WHERE coa.AccountType IN ('Revenue','Expense') AND je.EntryDate <= @AsOfDate"

        Dim result = DBHelper.ExecuteScalar(sql, New SqlParameter("@AsOfDate", asOfDate.AddDays(1).AddSeconds(-1)))
        Return If(result IsNot Nothing AndAlso result IsNot DBNull.Value, Convert.ToDecimal(result), 0)
    End Function

    Private Sub BindReport()
        Dim asOfDate As DateTime = If(txtAsOfDate.Text = "", DateTime.Now, Convert.ToDateTime(txtAsOfDate.Text))

        Dim dtAsset = GetBalanceForType("Asset", asOfDate)
        Dim dtLiability = GetBalanceForType("Liability", asOfDate)
        Dim dtEquity = GetBalanceForType("Equity", asOfDate)
        Dim netIncome = GetCumulativeNetIncome(asOfDate)

        gvAsset.DataSource = dtAsset
        gvAsset.DataBind()
        gvLiability.DataSource = dtLiability
        gvLiability.DataBind()

        ' Tambahkan baris "Laba Tahun Berjalan" ke tabel Equity yang ditampilkan
        Dim dtEquityDisplay = dtEquity.Copy()
        If netIncome <> 0 Then
            Dim newRow = dtEquityDisplay.NewRow()
            newRow("AccountName") = "Laba Tahun Berjalan"
            newRow("Amount") = netIncome
            dtEquityDisplay.Rows.Add(newRow)
        End If
        gvEquity.DataSource = dtEquityDisplay
        gvEquity.DataBind()

        Dim totalAsset As Decimal = 0
        For Each row As DataRow In dtAsset.Rows
            totalAsset += Convert.ToDecimal(row("Amount"))
        Next

        Dim totalLiability As Decimal = 0
        For Each row As DataRow In dtLiability.Rows
            totalLiability += Convert.ToDecimal(row("Amount"))
        Next

        Dim totalEquity As Decimal = netIncome
        For Each row As DataRow In dtEquity.Rows
            totalEquity += Convert.ToDecimal(row("Amount"))
        Next

        litTotalAsset.Text = "Rp " & totalAsset.ToString("N0")
        litTotalLiability.Text = "Rp " & totalLiability.ToString("N0")
        litTotalEquity.Text = "Rp " & totalEquity.ToString("N0")
        litTotalLiabilityEquity.Text = "Rp " & (totalLiability + totalEquity).ToString("N0")

        If totalAsset = (totalLiability + totalEquity) Then
            litBalanceCheck.Text = "<span class='badge badge-active'>Neraca Balance &#10003; (Aset = Kewajiban + Modal)</span>"
        Else
            litBalanceCheck.Text = "<span class='badge badge-overdue'>Neraca TIDAK Balance - cek kembali jurnal yang sudah diposting</span>"
        End If
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        Dim asOfDate As DateTime = If(txtAsOfDate.Text = "", DateTime.Now, Convert.ToDateTime(txtAsOfDate.Text))

        Dim dtAsset = GetBalanceForType("Asset", asOfDate)
        Dim dtLiability = GetBalanceForType("Liability", asOfDate)
        Dim dtEquity = GetBalanceForType("Equity", asOfDate)
        Dim netIncome = GetCumulativeNetIncome(asOfDate)

        Dim exportDt As New DataTable()
        exportDt.Columns.Add("Kategori")
        exportDt.Columns.Add("Akun")
        exportDt.Columns.Add("Saldo")

        Dim totalAsset As Decimal = 0
        For Each row As DataRow In dtAsset.Rows
            exportDt.Rows.Add("Aset", row("AccountName"), Convert.ToDecimal(row("Amount")).ToString("N0"))
            totalAsset += Convert.ToDecimal(row("Amount"))
        Next
        exportDt.Rows.Add("", "TOTAL ASET", totalAsset.ToString("N0"))

        Dim totalLiability As Decimal = 0
        For Each row As DataRow In dtLiability.Rows
            exportDt.Rows.Add("Kewajiban", row("AccountName"), Convert.ToDecimal(row("Amount")).ToString("N0"))
            totalLiability += Convert.ToDecimal(row("Amount"))
        Next

        Dim totalEquity As Decimal = netIncome
        For Each row As DataRow In dtEquity.Rows
            exportDt.Rows.Add("Modal", row("AccountName"), Convert.ToDecimal(row("Amount")).ToString("N0"))
            totalEquity += Convert.ToDecimal(row("Amount"))
        Next
        If netIncome <> 0 Then
            exportDt.Rows.Add("Modal", "Laba Tahun Berjalan", netIncome.ToString("N0"))
        End If
        exportDt.Rows.Add("", "TOTAL KEWAJIBAN + MODAL", (totalLiability + totalEquity).ToString("N0"))

        CsvHelper.ExportToCsv(Response, exportDt, "Neraca_" & DateTime.Now.ToString("yyyyMMdd") & ".csv")
    End Sub

End Class
