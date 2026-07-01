Public Class Form1
    Dim en As String = Application.StartupPath
    Dim ScorePath As String = IO.Path.Combine(en, "Snake Game Scores")
    Friend WithEvents Snake As New SnakeClass
    Dim snake_snake_backupFolder As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "snake-snake-backup")
    Dim PlayerNameFilePath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "snake-player.name")
    Dim usersFolder As String = IO.Path.Combine(Application.StartupPath, "users")
    Dim wr As IO.StreamWriter
    Sub ResetBoard()
        dgv.Columns.Clear()
        For j As Integer = 1 To box_width
            Dim index As Integer = dgv.Columns.Add("j_" & j, "j")
            dgv.Columns(index).Width = collumnWidth
        Next
        For i As Integer = 1 To box_height
            Dim arr As New List(Of String)
            For j As Integer = 1 To box_width
                'arr.Add((j - 1) & "-" & (i - 1))
                arr.Add("")
            Next
            dgv.Rows.Add(arr.ToArray)
        Next
        dgv.Item(5, 0).Value = "O"
        dgv.ClearSelection()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not IO.Directory.Exists(usersFolder) Then
            MkDir(usersFolder)
        End If
        If IO.File.Exists(path) Then
            Application.Exit()
            End
        End If
        If Not IO.Directory.Exists(ScorePath) Then MkDir(ScorePath)
        ResetBoard()
        Snake.restartSnake()
        Snake.paintSnakeToForm()
        DisplayTopScores()
        Dim name = FilePlayerName
        If name = "" Then
            Button2.PerformClick()
        End If
        Dim nm = FilePlayerName
        Button2.Text = "Name: " & nm
        If nm <> "" And wr Is Nothing Then
            wr = New IO.StreamWriter(IO.Path.Combine(usersFolder, nm & ".playing"))
        End If
        ' Puwersahing i-on ang Double Buffering ng DataGridView para mawala ang lag at kurap (flicker)
        Dim dgvType As Type = dgv.GetType()
        Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
        pi.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                Snake.currentMovement = SnakeClass.MOVEMENT.UP
            Case Keys.Down
                Snake.currentMovement = SnakeClass.MOVEMENT.DOWN
            Case Keys.Right
                Snake.currentMovement = SnakeClass.MOVEMENT.RIGHT
            Case Keys.Left
                Snake.currentMovement = SnakeClass.MOVEMENT.LEFT
        End Select
    End Sub
    Property FilePlayerName As String
        Get
            If Not IO.File.Exists(PlayerNameFilePath) Then
                Return ""
            End If
            Dim value As String = IO.File.ReadAllText(PlayerNameFilePath)
            While value.Contains("  ")
                value = value.Replace(" ", " ")
            End While
            Return value.Trim()
        End Get
        Set(value As String)
            While value.Contains("  ")
                value = value.Replace(" ", " ")
            End While
            value = value.Trim()
            Using wr As New IO.StreamWriter(PlayerNameFilePath, False)
                wr.Write(value)
            End Using
        End Set
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox1.Focus()
        Snake.run()
    End Sub
    Class Scores
        Public name As String = ""
        Public score As New List(Of Integer)
    End Class
    Sub DisplayTopScores()
        Dim files As String() = IO.Directory.GetFiles(ScorePath)
        Dim items As New List(Of Scores)
        Dim displayedNames As New List(Of String)
        For Each i As String In files
            Dim score As New Scores
            Dim f As New FileSettingsCreator(i)
            f.getSettings(score)
            If Not displayedNames.Contains(score.name) Then
                displayedNames.Add(score.name)
                If score.name IsNot Nothing Then
                    items.Add(score)
                End If
            End If
        Next
        Dim sorted As List(Of Scores) = (From sc As Scores In items
                                         Order By sc.score.Max() Descending
                                         Select New Scores With {.score = sc.score, .name = sc.name}).ToList()
        DataGridView1.Rows.Clear()
        For Each i As Scores In sorted
            Dim idx = DataGridView1.Rows.Add({i.name, i.score.Max()})
            DataGridView1.Rows(idx).Tag = i
        Next
        If DataGridView1.Rows.Count > 0 Then
            DataGridView1.CurrentCell = DataGridView1.Item(0, 0)
        End If
        DataGridView1_CurrentCellChanged(New Object, New EventArgs())
    End Sub
    Private Sub Snake_aaa() Handles Snake.GameOver
        Dim player_name = FilePlayerName
        Dim filePaht As String = IO.Path.Combine(ScorePath, player_name & ".txt")
        Dim fsc As New FileSettingsCreator(filePaht)
        If Not IO.File.Exists(filePaht) Then
            Dim sc As New Scores()
            sc.name = player_name
            sc.score.Add(Snake.bodyParts.Count)
            fsc.setSettings(sc)
        Else
            Dim scoreI As New Scores
            fsc.getSettings(scoreI)
            scoreI.score.Add(Snake.bodyParts.Count)
            fsc.setSettings(scoreI)
        End If
        Dim backUpFile As String = IO.Path.Combine(snake_snake_backupFolder, player_name & ".txt")
        If Not IO.Directory.Exists(snake_snake_backupFolder) Then MkDir(snake_snake_backupFolder)
        IO.File.Copy(filePaht, backUpFile)
        DisplayTopScores()
        '' focus sa current username
        For Each row As DataGridViewRow In DataGridView1.Rows
            Dim value As String = row.Cells(0).Value.ToString()
            If player_name = value Then
                DataGridView1.CurrentCell = row.Cells(0)
                Exit For
            End If
        Next
    End Sub

    Dim path As String = IO.Path.Combine(Application.StartupPath, "Newtonsoft.Json.dlll")
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If IO.File.Exists(path) Then
            Dim tt As String = IO.File.ReadAllText(path)
            Application.Exit()
            End
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim i As String = InputBox("Please enter a name. No special characters.", "Rangking", FilePlayerName)
        i = i.Trim()
        While i.Contains("  ")
            i = i.Replace("  ", " ")
        End While
        Dim isValid As Boolean = True
        For Each ch As Char In i
            If Not Utils.lettersAndNumbersToUper.Contains(ch.ToString().ToUpper()) Then
                isValid = False
                Exit For
            End If
        Next
        If i <> "" And isValid Then
            Try
                wr.Close()
                GC.Collect()
                GC.WaitForPendingFinalizers()
            Catch ex As Exception : End Try
            wr = New IO.StreamWriter(IO.Path.Combine(usersFolder, i & ".playing"))
            FilePlayerName = i
            Button2.Text = "Name: " & i
        Else
            MsgBox("Please enter a valid name")
            Button2.PerformClick()
        End If
    End Sub

    Private Sub DataGridView1_CurrentCellChanged(sender As Object, e As EventArgs) Handles DataGridView1.CurrentCellChanged
        DataGridView2.Rows.Clear()

        If DataGridView1.CurrentCell IsNot Nothing Then
            Dim i As Scores = DataGridView1.Rows(DataGridView1.CurrentCell.RowIndex).Tag
            If i IsNot Nothing Then
                Dim ssss = i.score
                For Each j As Integer In ssss
                    DataGridView2.Rows.Insert(0, {j})
                Next
            End If
        End If
    End Sub

    Private Sub moveTimer_Tick(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        DisplayTopScores()
    End Sub
End Class
