
'Please install Newtonsoft.Json.13.0.1 
'in Manage NuGet Pacjages
Imports Newtonsoft.Json
Public Class FileSettingsCreator
    Dim path As String
    Dim crpt As Crypt.String
    Shared password As String = "key_csgi_gi_search_key"
    'Example
    Class SettingsInfo
        Public name As String = ""
    End Class
    Sub example()
        Dim FC As New FileSettingsCreator("C:\aldrin.settings")
        'save settings 
        FC.setSettings(New SettingsInfo() With {.name = "Aldrin osias aragon"})
        'get Settings 
        'setings_ variable must be a class not Structure 
        Dim setings_ As New SettingsInfo
        FC.getSettings(setings_)
    End Sub
    'end of example
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="settingFilePath">The settings info to be stored</param>
    Sub New(settingFilePath As String)
        path = settingFilePath
        crpt = New Crypt.String(password)
    End Sub
    Public Function isSettingExist() As Boolean
        Return IO.File.Exists(path)
    End Function
    Function setSettings(classObj As Object, Optional showError As Boolean = False) As Boolean
        Dim res As Boolean = False
        Dim tmpStr As String = JsonConvert.SerializeObject(classObj)
        Dim spl = SplitInParts(tmpStr, 10)
        Dim newSplt As New List(Of String)
        For Each i As String In spl
            newSplt.Add(crpt.Encrypt(i))
        Next
        Try
            Dim wr As New IO.StreamWriter(path, False)
            For Each i In newSplt
                wr.WriteLine(i)
            Next
            wr.Close()
            res = True
        Catch ex As Exception
            'Throw New Exception("FileSettingsCreator > setSettings Error: " & ex.Message)
            If showError Then
                MsgBox(String.Concat("FileSettingsCreator Error > ", ex.Message))
            End If
        End Try
        GC.Collect()
        GC.WaitForPendingFinalizers()
        Return res
    End Function
    Public Shared Function convertObject_To_StringEncrypted(classObj As Object) As String
        Dim tmpStr As String = JsonConvert.SerializeObject(classObj)
        Dim c As New Crypt.String(password)
        Dim res = c.Encrypt(tmpStr)
        Return res
    End Function
    Public Shared Sub convertStringEncryted_To_Object(str As String, ByRef obj As Object)
        Dim c As New Crypt.String(password)
        str = c.Decrypt(str)
        JsonConvert.PopulateObject(str, obj)
    End Sub

    'Function setSettings(classObj As Object) As Boolean
    '    Dim res As Boolean = False
    '    Dim tmpStr As String = JsonConvert.SerializeObject(classObj)
    '    Dim encrypt As String = crpt.Encrypt(tmpStr)
    '    Try
    '        Dim wr As New IO.StreamWriter(path, False)
    '        wr.Write(encrypt)
    '        wr.Close()
    '        res = True
    '    Catch ex As Exception
    '        'Throw New Exception("FileSettingsCreator > setSettings Error: " & ex.Message)
    '        MsgBox(String.Concat("FileSettingsCreator Error > ", ex.Message))
    '    End Try
    '    GC.Collect()
    '    GC.WaitForPendingFinalizers()
    '    Return res
    'End Function
    Sub getSettings(ByRef obj As Object, Optional showError As Boolean = False)
        Try
            If Not IO.File.Exists(path) Then
                Using a As New IO.StreamWriter(path)
                End Using
            End If
            Dim txtLines As String() = IO.File.ReadAllLines(path)
            Dim tx As String = ""
            For Each i As String In txtLines
                tx = String.Concat(tx, crpt.Decrypt(i))
            Next
            JsonConvert.PopulateObject(tx, obj)
        Catch ex As Exception
            'Throw New Exception("FileSettingsCreator > getSettings Error: " & ex.Message)
            If showError Then
                MsgBox(String.Concat("FileSettingsCreator Error > ", ex.Message))
            End If
        End Try
    End Sub
    'Sub getSettings(ByRef aa As Object)
    '    Try
    '        Dim txt As String = IO.File.ReadAllText(path)
    '        txt = crpt.Decrypt(txt)
    '        JsonConvert.PopulateObject(txt, aa)
    '    Catch ex As Exception
    '        'Throw New Exception("FileSettingsCreator > getSettings Error: " & ex.Message)
    '        MsgBox(String.Concat("FileSettingsCreator Error > ", ex.Message))
    '    End Try
    'End Sub

    Public Function SplitInParts(s As String, partLength As Integer) As IEnumerable(Of String)
        If String.IsNullOrEmpty(s) Then
            Throw New ArgumentNullException("String cannot be null or empty.")
        End If
        If partLength <= 0 Then
            Throw New ArgumentException("Split length has to be positive.")
        End If
        Return Enumerable.Range(0, Math.Ceiling(s.Length / partLength)).Select(Function(i) s.Substring(i * partLength, If(s.Length - (i * partLength) >= partLength, partLength, Math.Abs(s.Length - (i * partLength)))))
    End Function

#Region "Encryptor and decryptor"
    Public Class Crypt

        Public Class [String]
            Private TripleDes As New Security.Cryptography.TripleDESCryptoServiceProvider

            Public Sub New(ByVal key As String)
                key = key & "DotNetCS-" & key & "-Key"
                TripleDes.Key = TruncateHash(key, TripleDes.KeySize \ 8)
                TripleDes.IV = TruncateHash("", TripleDes.BlockSize \ 8)
            End Sub

            Private Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()
                Dim sha1 As New Security.Cryptography.SHA1CryptoServiceProvider
                Dim keyBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(key)
                Dim hash() As Byte = sha1.ComputeHash(keyBytes)
                ReDim Preserve hash(length - 1)
                Return hash
            End Function

            Public Function Encrypt(ByVal plaintext As String) As String
                If plaintext.Equals("") Then
                    Return plaintext
                Else
                    Dim plaintextBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(plaintext)
                    Dim ms As New System.IO.MemoryStream
                    Dim encStream As New Security.Cryptography.CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
                    encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
                    encStream.FlushFinalBlock()
                    Return Convert.ToBase64String(ms.ToArray)
                End If
            End Function

            Public Function Decrypt(ByVal encryptedtext As String) As String
                Dim retStr As String = ""
                If encryptedtext.Trim.Equals("") Then
                    retStr = encryptedtext
                Else
                    Try
                        Dim encryptedBytes() As Byte = Convert.FromBase64String(encryptedtext)
                        Dim ms As New System.IO.MemoryStream
                        Dim decStream As New Security.Cryptography.CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
                        decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
                        decStream.FlushFinalBlock()
                        retStr = System.Text.Encoding.Unicode.GetString(ms.ToArray)
                    Catch ex As Exception
                    End Try
                End If
                Return retStr
            End Function

            Public Shared Function GetMD5(ByVal value As String) As String
                Using md5 As Security.Cryptography.MD5 = Security.Cryptography.MD5.Create()
                    Dim bytes As Byte() = md5.ComputeHash(Text.Encoding.UTF8.GetBytes(value))
                    Dim builder As New Text.StringBuilder()
                    For n As Integer = 0 To bytes.Length - 1
                        builder.Append(bytes(n).ToString("X2"))
                    Next n
                    Return builder.ToString()
                End Using
            End Function
        End Class
    End Class
#End Region
End Class
