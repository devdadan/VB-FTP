Imports System.Net
Imports System.Diagnostics
Imports System.ComponentModel
Imports MySql.Data.MySqlClient
Imports System.IO


Public Class Form1
    Dim conn As MySqlConnection
    Dim user As String
    Dim server As String
    Dim pass As String
    Dim sql As String
    Dim cmd As MySqlCommand
    Dim read As MySqlDataReader
    Dim kodetoko As String
    Dim namatoko As String
    Dim pth As String
    Dim tulis As StreamWriter


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "FtpCabang v." & Application.ProductVersion
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        absen()
        list()
        'list2()
        pth = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\FTPCABANG\", "LOKAL_HR", Nothing)
        TextBox1.Text = pth
    End Sub

    Public Sub list()
        If Not conn Is Nothing Then conn.Close()
        Dim connstr As String = "server=192.168.85.101;user id=root; password=123456; database=FtpCabang; pooling=false"

        Try
            Dim bln As String
            Dim thn, th As String
            Dim hr As String
            bln = Microsoft.VisualBasic.Mid(Period.Text.Trim, 4, 2)
            thn = Microsoft.VisualBasic.Right(Period.Text.Trim, 4)
            th = Microsoft.VisualBasic.Right(Period.Text.Trim, 2)
            hr = Microsoft.VisualBasic.Left(Period.Text, 2)
            hr = Microsoft.VisualBasic.Left(Period.Text, 2)
            conn = New MySqlConnection(connstr)
            conn.Open()

            sql = "select KDTK FROM m_stb where kdtk in(select kdtk from absen_hr" + bln + thn + " where  tg" + hr + " is null or tg" + hr + "='' ) order by kdtk"
            cmd = New MySqlCommand(sql, conn)
            ListBox1.Items.Clear()
            cmd = New MySqlCommand(sql, conn)
            read = cmd.ExecuteReader

            While read.Read
                ListBox1.Items.Add(read.Item(0))

            End While
            ListBox1.SelectedIndex = 0
            read.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Label7.Text = ListBox1.Items.Count
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        BackgroundWorker1.RunWorkerAsync()
        'proses()

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Public server1, nftp As String
    Private Sub proses()
        Timer1.Start()
        Dim buffer(1023) As Byte
        Dim BytesIn As Integer
        Dim totalBytesIn As Integer
        Dim output As IO.Stream
        Dim flength As Integer
        Dim nilai As Integer
        Dim bln As String
        Dim thn, th As String
        Dim hr As String
        Dim nharian As String
        Dim nbulanan As String
        bln = Microsoft.VisualBasic.Mid(Period.Text.Trim, 4, 2)
        thn = Microsoft.VisualBasic.Right(Period.Text.Trim, 4)
        th = Microsoft.VisualBasic.Right(Period.Text.Trim, 2)
        hr = Microsoft.VisualBasic.Left(Period.Text, 2)
        ProgressBar2.Minimum = 0
        ProgressBar2.Maximum = Label7.Text
        For nilai = 0 To ListBox1.Items.Count


            kdtk.Text = ListBox1.Items(nilai)
            Dim tk As String = kdtk.Text.Substring(0, 4)
            kodetoko = tk
            If Not conn Is Nothing Then conn.Close()
            Dim connstr As String = "server=192.168.85.101;user id=root; password=123456; database=FtpCabang; pooling=false"
            Try
                conn = New MySqlConnection(connstr)
                conn.Open()
                sql = "select * FROM m_stb where kdtk='" + kodetoko.ToString + "'"
                Dim act As New MySqlCommand(sql, conn)
                Dim baca As MySqlDataReader
                baca = act.ExecuteReader
                ProgressBar2.Value = nilai
                If baca.Read Then
                    namatoko = baca.Item(1)
                    ip.Text = (baca.Item(2))
                    dpn.Text = baca.Item(4)
                    blkng.Text = baca.Item(5)
                    baca.Close()
                Else


                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            server1 = ip.Text
            Form2.ShowDialog()
            nharian = dpn.Text & th & bln & hr & "." & blkng.Text
            nbulanan = kodetoko & th & bln & ".IDT"
            If ComboBox2.Text = "HARIAN" Then
                nftp = "ftp://" & server1 & Label9.Text & nharian
            Else
                nftp = "ftp://" & server1 & Label9.Text & nbulanan
            End If

            'MsgBox(nftp)
            If Form2.nil2 > 3 Then
                Dim cserver As String = ip.Text
                Try
                    Dim FTPRequest As FtpWebRequest = DirectCast(WebRequest.Create(nftp.ToString), FtpWebRequest)
                    FTPRequest.Credentials = New NetworkCredential("posterm", "dAZAD9yq")
                    FTPRequest.Method = Net.WebRequestMethods.Ftp.GetFileSize
                    flength = CInt(FTPRequest.GetResponse.ContentLength)
                    Label3.Text = flength & "Bytes"

                Catch ex As Exception
                    'MsgBox(ex.Message)
                End Try
                Try
                    'MsgBox(nftp)
                    ProgressBar1.Minimum = 0
                    ProgressBar1.Maximum = flength
                    ProgressBar1.Value = 0
                    Label5.Text = ""
                    Dim FTPRequest As FtpWebRequest = DirectCast(WebRequest.Create(nftp.ToString), FtpWebRequest)
                    FTPRequest.Credentials = New NetworkCredential("posterm", "dAZAD9yq")
                    FTPRequest.Method = Net.WebRequestMethods.Ftp.DownloadFile
                    Dim stream As System.IO.Stream = FTPRequest.GetResponse.GetResponseStream
                    Dim Outputfilepath As String = TextBox1.Text & "\" & IO.Path.GetFileName(nftp.ToString)
                    output = System.IO.File.Create(Outputfilepath)
                    BytesIn = 1

                    Do Until BytesIn < 1
                        BytesIn = stream.Read(buffer, 0, 1024)
                        If BytesIn > 0 Then
                            output.Write(buffer, 0, BytesIn)
                            totalBytesIn += BytesIn
                            Label5.Text = totalBytesIn.ToString & " Bytes"
                            If flength > 0 Then
                                Dim perc As Integer = (totalBytesIn / flength) * 100
                                ProgressBar1.Value = perc
                            End If
                        End If
                    Loop

                    output.Close()
                    stream.Close()
                    sql = "UPDATE absen_hr" + bln + thn + " set tg" + hr + "='1' WHERE kdtk='" + kodetoko.ToString + "'"
                    cmd = New MySqlCommand(sql, conn)
                    cmd.ExecuteNonQuery()
                    list2()
                    list()
                Catch ex As Exception
                    If Not conn Is Nothing Then conn.Close()
                    connstr = "server=192.168.85.101;user id=root; password=123456; database=FtpCabang; pooling=false"
                    conn = New MySqlConnection(connstr)
                    conn.Open()
                    sql = "UPDATE absen_hr" + bln + thn + " set keter='TIDAK ADA DI FOLDER SCHEDULE' WHERE kdtk='" + kodetoko.ToString + "'"
                    cmd = New MySqlCommand(sql, conn)
                    cmd.ExecuteNonQuery()
                    If Not File.Exists(Application.StartupPath & "\LOG-FTP_" & Period.Text & ".txt") Then
                        tulis = File.CreateText(Application.StartupPath & "\LOG-FTP_" & Period.Text & ".txt")
                        tulis.WriteLine("===============LOG-FTP===========")
                        tulis.WriteLine(kodetoko.ToString & " == " & ex.Message)
                        tulis.Flush()
                        tulis.Close()
                    Else
                        tulis = File.AppendText(Application.StartupPath & "\LOG-FTP_" & Period.Text & ".txt")
                        tulis.WriteLine(kodetoko.ToString & " == " & ex.Message)
                        tulis.Flush()
                        tulis.Close()
                    End If
                End Try


            Else
                sql = "UPDATE absen_hr" + bln + thn + " set KETER='KONEKSI STB NOK' WHERE kdtk='" + kodetoko.ToString + "'"
                cmd = New MySqlCommand(sql, conn)
                cmd.ExecuteNonQuery()
            End If


        Next

    End Sub

    Private Sub absen()
        If Not conn Is Nothing Then conn.Close()
        Dim connstr As String = "server=192.168.85.101;user id=root; password=123456; database=FtpCabang; pooling=false"
        Try
            conn = New MySqlConnection(connstr)
            conn.Open()
            Dim bln As String = Microsoft.VisualBasic.Mid(Period.Text.Trim, 4, 2)
            Dim thn As String = Microsoft.VisualBasic.Right(Period.Text.Trim, 4)
            sql = "CREATE TABLE IF NOT EXISTS absen_hr" + bln + thn + " select * from absen "
            cmd = New MySqlCommand(sql, conn)
            cmd.ExecuteNonQuery()
        Catch ex As Exception

        End Try


    End Sub


    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.Text = "Scheduled" Then
            Label9.Text = ":21/scheduled/"
        Else
            Label9.Text = ":21/output/"
        End If
    End Sub
    Private Sub openf1()

        Dim fbd As New FolderBrowserDialog
        fbd.SelectedPath = ""
        Dim result As DialogResult = fbd.ShowDialog
        If result = Windows.Forms.DialogResult.Cancel Then
            fbd.SelectedPath = Nothing
            TextBox1.Text = ""
        Else
            TextBox1.Text = fbd.SelectedPath.Trim
            Dim folderInfo As New IO.DirectoryInfo(TextBox1.Text)
            TextBox1.Text = fbd.SelectedPath.Trim.ToUpper

            Dim ss As String
            ss = TextBox1.Text.ToString
            TextBox1.Text = ss
            If Microsoft.VisualBasic.Right(TextBox1.Text, 1) = "\" Then
                TextBox1.Text = ss

            Else
                TextBox1.Text = ss + "\"
            End If

        End If
    End Sub
    Private Sub list2()

        Dim path As String
        ListBox2.Items.Clear()
        path = Replace(TextBox1.Text, "/", "\")
        Dim folderInfo As New IO.DirectoryInfo(path)
        Dim arrFilesInFolder() As IO.FileInfo
        Dim fileInFolder As IO.FileInfo
        arrFilesInFolder = folderInfo.GetFiles("*.*")
        For Each fileInFolder In arrFilesInFolder
            ListBox2.Items.Add(fileInFolder.Name)
        Next
        Label11.Text = ListBox2.Items.Count
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        openf1()

    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        proses()

        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\SOFTWARE\FTPCABANG", "LOKAL_HR", TextBox1.Text)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Period.Text = DateAdd("d", -1, Date.Now)
    End Sub


End Class
