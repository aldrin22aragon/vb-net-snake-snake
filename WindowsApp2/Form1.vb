Public Class Form1
   Dim en As String = Application.StartupPath
   Dim ScorePath As String = IO.Path.Combine(en, "Snake Game Scores")
   Friend WithEvents Snake As New SnakeClass
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
      If Not IO.Directory.Exists(ScorePath) Then MkDir(ScorePath)
      ResetBoard()
      Snake.restartSnake()
      Snake.paintSnakeToForm()
      DisplayTopScores()
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
   Dim player_name As String = ""
   Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
      player_name = InputBox("Please Enter Name")
      player_name = player_name.Trim.ToUpper
      If player_name <> "" Then
         lblPlayer.Text = "Player : " & player_name
         TextBox1.Focus()
         Snake.run()
      Else
         MsgBox("Please enter a valid player name")
      End If
   End Sub
   Class Scores
      Public name As String
      Public score As Integer
   End Class
   Sub DisplayTopScores()
      Dim files As String() = IO.Directory.GetFiles(ScorePath)
      Dim items As New List(Of Scores)
      For Each i As String In files
         Dim score As New Scores
         Dim f As New FileSettingsCreator(i)
         f.getSettings(score)
         If score.name IsNot Nothing Then
            items.Add(score)
         End If
      Next
      rt.Text = "Top Players..." & vbNewLine
      Dim b As List(Of Scores) = (From sc As Scores In items
                                  Order By sc.score Descending
                                  Select New Scores With {.score = sc.score, .name = sc.name}).ToList
      For Each i As Scores In b
         rt.Text = String.Concat(rt.Text, vbNewLine, i.name, " : ", i.score)
      Next



   End Sub
   Private Sub Snake_aaa() Handles Snake.GameOver
      Dim filePaht As String = IO.Path.Combine(ScorePath, player_name & ".txt")
      Dim fsc As New FileSettingsCreator(filePaht)
      If Not IO.File.Exists(filePaht) Then
         fsc.setSettings(New Scores() With {.name = player_name, .score = Snake.bodyParts.Count})
      Else
         Dim scoreI As New Scores
         fsc.getSettings(scoreI)
         If Snake.bodyParts.Count > scoreI.score Then
            fsc.setSettings(New Scores() With {.name = player_name, .score = Snake.bodyParts.Count})
         End If
      End If
      fsc = Nothing
      DisplayTopScores()
   End Sub
End Class
