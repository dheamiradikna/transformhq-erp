Imports System.Data.SqlClient

''' <summary>
''' JournalHelper - kumpulan fungsi untuk membuat (posting) jurnal akuntansi otomatis
''' dari transaksi lain (Invoice, Payment, Payroll), dan untuk jurnal manual.
''' Semua posting dilakukan di dalam SqlTransaction yang DIBERIKAN oleh pemanggil,
''' supaya jurnal otomatis selalu konsisten dengan transaksi sumbernya
''' (kalau transaksi sumber gagal/rollback, jurnalnya ikut batal).
''' </summary>
Public Module JournalHelper

    ''' <summary>Generate nomor jurnal otomatis: JE-{tahun}-{urutan}.</summary>
    Public Function GenerateEntryNo(conn As SqlConnection, tran As SqlTransaction) As String
        Dim currentYear As Integer = DateTime.Now.Year
        Dim countThisYear As Integer
        Using cmd As New SqlCommand("SELECT COUNT(*) FROM dbo.JournalEntries WHERE YEAR(EntryDate) = @Year", conn, tran)
            cmd.Parameters.AddWithValue("@Year", currentYear)
            countThisYear = Convert.ToInt32(cmd.ExecuteScalar())
        End Using
        Return "JE-" & currentYear & "-" & (countThisYear + 1).ToString("D4")
    End Function

    ''' <summary>Cari AccountID berdasarkan kode akun (contoh: "1-1200" untuk Piutang Usaha).</summary>
    Public Function GetAccountIdByCode(conn As SqlConnection, tran As SqlTransaction, accountCode As String) As Integer
        Using cmd As New SqlCommand("SELECT AccountID FROM dbo.ChartOfAccounts WHERE AccountCode = @Code", conn, tran)
            cmd.Parameters.AddWithValue("@Code", accountCode)
            Dim result = cmd.ExecuteScalar()
            If result Is Nothing Then
                Throw New Exception("Akun dengan kode '" & accountCode & "' tidak ditemukan di Chart of Accounts.")
            End If
            Return Convert.ToInt32(result)
        End Using
    End Function

    ''' <summary>
    ''' Posting jurnal 2 baris sederhana (1 Debit, 1 Credit dengan jumlah sama) - kasus paling umum
    ''' seperti "Piutang Usaha (D) vs Pendapatan Jasa (K)" saat invoice dibuat.
    ''' Dipanggil di DALAM transaction yang sama dengan transaksi sumber (conn & tran diberikan oleh pemanggil).
    ''' </summary>
    Public Sub PostSimpleEntry(conn As SqlConnection, tran As SqlTransaction,
                                 entryDate As DateTime, description As String,
                                 sourceType As String, sourceId As Integer,
                                 debitAccountCode As String, creditAccountCode As String,
                                 amount As Decimal, createdBy As String)

        If amount <= 0 Then Return ' Tidak perlu jurnal untuk jumlah nol/negatif

        Dim entryNo As String = GenerateEntryNo(conn, tran)
        Dim debitAccountId As Integer = GetAccountIdByCode(conn, tran, debitAccountCode)
        Dim creditAccountId As Integer = GetAccountIdByCode(conn, tran, creditAccountCode)

        Dim journalEntryId As Integer
        Using cmdHeader As New SqlCommand(
            "INSERT INTO dbo.JournalEntries (EntryNo, EntryDate, Description, SourceType, SourceID, CreatedBy) " &
            "VALUES (@EntryNo, @EntryDate, @Description, @SourceType, @SourceID, @CreatedBy); SELECT SCOPE_IDENTITY();", conn, tran)
            cmdHeader.Parameters.AddWithValue("@EntryNo", entryNo)
            cmdHeader.Parameters.AddWithValue("@EntryDate", entryDate)
            cmdHeader.Parameters.AddWithValue("@Description", description)
            cmdHeader.Parameters.AddWithValue("@SourceType", sourceType)
            cmdHeader.Parameters.AddWithValue("@SourceID", sourceId)
            cmdHeader.Parameters.AddWithValue("@CreatedBy", If(String.IsNullOrEmpty(createdBy), CType(DBNull.Value, Object), createdBy))
            journalEntryId = Convert.ToInt32(cmdHeader.ExecuteScalar())
        End Using

        Using cmdDebit As New SqlCommand(
            "INSERT INTO dbo.JournalEntryLines (JournalEntryID, AccountID, Debit, Credit) VALUES (@JEID, @AccID, @Amount, 0)", conn, tran)
            cmdDebit.Parameters.AddWithValue("@JEID", journalEntryId)
            cmdDebit.Parameters.AddWithValue("@AccID", debitAccountId)
            cmdDebit.Parameters.AddWithValue("@Amount", amount)
            cmdDebit.ExecuteNonQuery()
        End Using

        Using cmdCredit As New SqlCommand(
            "INSERT INTO dbo.JournalEntryLines (JournalEntryID, AccountID, Debit, Credit) VALUES (@JEID, @AccID, 0, @Amount)", conn, tran)
            cmdCredit.Parameters.AddWithValue("@JEID", journalEntryId)
            cmdCredit.Parameters.AddWithValue("@AccID", creditAccountId)
            cmdCredit.Parameters.AddWithValue("@Amount", amount)
            cmdCredit.ExecuteNonQuery()
        End Using
    End Sub

End Module
