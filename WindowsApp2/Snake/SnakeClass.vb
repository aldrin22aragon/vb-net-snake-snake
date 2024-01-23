Public Class SnakeClass
    Public Head As New BodyPartClass
    Public Tail As New BodyPartClass
    Public prevTail As New BodyPartClass
    Public bodyParts As List(Of BodyPartClass)
    Public tatagos As Boolean = False
    Public moveTimerInterval As Integer = 200
    Public tmp_currentMovement As MOVEMENT = MOVEMENT.NONE
    Public tmp_snakeMode As SnakeModeEnum = SnakeModeEnum.none
    Public prevMovemnt As MOVEMENT = currentMovement
    Public Event GameOver()
    Dim random As New Random
    Friend WithEvents moveTimer As New Timer
    Public Enum SnakeModeEnum As Integer
        none = 0
        Running = 1
        Stoped = 2
        pause = 3
    End Enum
    Public Enum MoveValidation As Integer
        valid = 1
        collided = 2
        invalidMove = 3
        kain = 4
    End Enum
    Public Enum MOVEMENT As Integer
        NONE = 0
        UP = 1
        DOWN = 2
        RIGHT = 3
        LEFT = 4
    End Enum
    Sub restartSnake()
        Form1.dgv.ClearSelection()
        moveTimer.Interval = moveTimerInterval
        currentMovement = MOVEMENT.RIGHT
        Head = New BodyPartClass() With {.col = 2, .row = 0, .bodyPart = BodyPartClass.EnumBodyPart.HEAD}
        Tail = New BodyPartClass() With {.col = 0, .row = 0, .bodyPart = BodyPartClass.EnumBodyPart.TAIL}
        bodyParts = New List(Of BodyPartClass)
        bodyParts.Add(Head)
        bodyParts.Add(New BodyPartClass() With {.col = 1, .row = 0, .bodyPart = BodyPartClass.EnumBodyPart.BODY})
        bodyParts.Add(Tail)
        paintSnakeToForm(True)
    End Sub
    Property currentMovement As MOVEMENT
        Get
            Return tmp_currentMovement
        End Get
        Set(value As MOVEMENT)
            prevMovemnt = currentMovement
            tmp_currentMovement = value
        End Set
    End Property
    Property snakeMode As SnakeModeEnum
        Get
            Return tmp_snakeMode
        End Get
        Set(value As SnakeModeEnum)
            Select Case value
                Case SnakeModeEnum.Running
                    restartSnake()
                    moveTimer.Start()
                Case SnakeModeEnum.Stoped
                    restartSnake()
                    moveTimer.Stop()
                Case SnakeModeEnum.pause
                    moveTimer.Stop()
            End Select
            tmp_snakeMode = value
        End Set
    End Property

    Public Function go(move As MOVEMENT) As MoveValidation
        prevTail = Tail.ShallowCopy
        Dim followBodyPart As New BodyPartClass
        For i As Integer = 0 To bodyParts.Count - 1
            Dim i_body As BodyPartClass = bodyParts(i).ShallowCopy
            If i_body.bodyPart = BodyPartClass.EnumBodyPart.HEAD Then
                Dim tmp As New BodyPartClass
                Select Case move
                    Case MOVEMENT.DOWN
                        tmp.col = i_body.col
                        tmp.row = (i_body.row + 1)
                    Case MOVEMENT.UP
                        tmp.col = i_body.col
                        tmp.row = (i_body.row - 1)
                    Case MOVEMENT.RIGHT
                        tmp.col = (i_body.col + 1)
                        tmp.row = i_body.row
                    Case MOVEMENT.LEFT
                        tmp.col = (i_body.col - 1)
                        tmp.row = i_body.row
                End Select
                tmp.bodyPart = i_body.bodyPart
                Try
                    Dim item As DataGridViewCell = Form1.dgv.Item(tmp.col, tmp.row)
                    Dim itemValue As String = item.Value.ToString
                    If item.Selected Then
                        Dim snakeNeck As BodyPartClass = bodyParts(1).ShallowCopy
                        If item.ColumnIndex = snakeNeck.col And item.RowIndex = snakeNeck.row Then
                            Return MoveValidation.invalidMove
                        Else
                            Return MoveValidation.collided
                        End If
                    ElseIf itemValue <> "" Then
                        item.Value = ""
                        Randomize()
                        Dim col As Integer = random.Next(0, box_width - 1)
                        Dim row As Integer = random.Next(0, box_height - 1)
                        Dim cel As DataGridViewCell = Form1.dgv.Item(col, row)
                        While cel.Value.ToString <> "" Or cel.Selected
                            col = random.Next(0, box_width - 1)
                            row = random.Next(0, box_height - 1)
                            cel = Form1.dgv.Item(col, row)
                            Application.DoEvents()
                        End While
                        cel.Value = "O"
                        bodyParts(i) = tmp
                        bodyParts.Add(Tail.ShallowCopy)
                        Form1.lblScore.Text = "Score: " & bodyParts.Count
                    End If
                Catch ex As Exception
                    Return MoveValidation.collided
                End Try
                bodyParts(i) = tmp
            Else
                Dim tmp As New BodyPartClass
                tmp.col = followBodyPart.col
                tmp.row = followBodyPart.row
                tmp.bodyPart = i_body.bodyPart
                bodyParts(i) = tmp
            End If
            followBodyPart = i_body.ShallowCopy
            Application.DoEvents()
        Next
        Return paintSnakeToForm()
    End Function
    Sub run()
        snakeMode = SnakeModeEnum.Running
    End Sub
    Public Function paintSnakeToForm(Optional skipSelected As Boolean = False) As MoveValidation
        Form1.dgv.Item(prevTail.col, prevTail.row).Selected = False
        For Each i As BodyPartClass In bodyParts
            With Form1
                Try
                    If .dgv.Item(i.col, i.row).Selected And skipSelected Then
                        Return MoveValidation.collided
                    Else
                        .dgv.Item(i.col, i.row).Selected = True
                    End If
                Catch ex As Exception
                    Return MoveValidation.collided
                End Try
            End With
            Application.DoEvents()
        Next
        Head = bodyParts(0)
        Tail = bodyParts(bodyParts.Count - 1)
        Return MoveValidation.valid
    End Function
    Dim moving As Boolean = False
    Private Sub moveTimer_Tick(sender As Object, e As EventArgs) Handles moveTimer.Tick
        If Not moving Then
            moving = True
            Select Case go(currentMovement)
                Case MoveValidation.collided
                    moveTimer.Stop()
                    MsgBox("Game Over")
                    RaiseEvent GameOver()
                    snakeMode = SnakeModeEnum.Stoped
                Case MoveValidation.invalidMove
                    currentMovement = prevMovemnt
                Case MoveValidation.valid, MoveValidation.kain
            End Select
            moving = False
            Application.DoEvents()
        End If
    End Sub
End Class
