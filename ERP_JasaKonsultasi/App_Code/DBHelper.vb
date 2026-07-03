Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' DBHelper - kelas bantu untuk koneksi & eksekusi query ke SQL Server.
''' Semua halaman (code-behind) memakai class ini supaya kode lebih rapi
''' dan tidak perlu menulis ulang logika koneksi di setiap halaman.
''' </summary>
Public Module DBHelper

    Public Function GetConnectionString() As String
        Return ConfigurationManager.ConnectionStrings("ERPConnectionString").ConnectionString
    End Function

    ''' <summary>Menjalankan query SELECT dan mengembalikan DataTable.</summary>
    Public Function ExecuteQuery(sql As String, ParamArray parameters As SqlParameter()) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ''' <summary>Menjalankan INSERT/UPDATE/DELETE. Mengembalikan jumlah baris terpengaruh.</summary>
    Public Function ExecuteNonQuery(sql As String, ParamArray parameters As SqlParameter()) As Integer
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If
                conn.Open()
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ''' <summary>Menjalankan query yang mengembalikan satu nilai tunggal (contoh: COUNT, SUM, atau SCOPE_IDENTITY()).</summary>
    Public Function ExecuteScalar(sql As String, ParamArray parameters As SqlParameter()) As Object
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(sql, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If
                conn.Open()
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    ''' <summary>Hash password dengan SHA256 (hex lowercase) - dipakai untuk Login.</summary>
    Public Function HashPassword(plainText As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText))
            Dim sb As New StringBuilder()
            For Each b As Byte In bytes
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function

End Module
