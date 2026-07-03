Imports System.Data
Imports System.Text

''' <summary>
''' CsvHelper - kumpulan fungsi untuk export DataTable ke file CSV yang bisa
''' dibuka langsung di Excel. Dipilih CSV (bukan format .xlsx asli) supaya tidak
''' perlu menambah library/NuGet pihak ketiga - cukup ADO.NET bawaan .NET Framework.
''' </summary>
Public Module CsvHelper

    ''' <summary>Mengirim DataTable sebagai file CSV untuk didownload user (dipanggil dari code-behind halaman laporan).</summary>
    Public Sub ExportToCsv(response As System.Web.HttpResponse, dt As DataTable, fileName As String)
        response.Clear()
        response.ContentType = "text/csv; charset=utf-8"
        response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)

        ' BOM (Byte Order Mark) UTF-8 supaya Excel membaca karakter non-ASCII (Rp, dsb) dengan benar
        response.BinaryWrite(New Byte() {&HEF, &HBB, &HBF})

        Dim sb As New StringBuilder()

        ' Header kolom
        Dim headers As New List(Of String)
        For Each col As DataColumn In dt.Columns
            headers.Add(EscapeCsvField(col.ColumnName))
        Next
        sb.AppendLine(String.Join(",", headers))

        ' Baris data
        For Each row As DataRow In dt.Rows
            Dim fields As New List(Of String)
            For Each col As DataColumn In dt.Columns
                fields.Add(EscapeCsvField(row(col).ToString()))
            Next
            sb.AppendLine(String.Join(",", fields))
        Next

        response.Write(sb.ToString())
        response.End()
    End Sub

    Private Function EscapeCsvField(value As String) As String
        If value Is Nothing Then Return ""
        ' Bungkus dengan tanda kutip kalau field mengandung koma, kutip, atau baris baru
        If value.Contains(",") OrElse value.Contains("""") OrElse value.Contains(vbCr) OrElse value.Contains(vbLf) Then
            Return """" & value.Replace("""", """""") & """"
        End If
        Return value
    End Function

End Module
