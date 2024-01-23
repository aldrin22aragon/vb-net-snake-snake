Public Class BodyPartClass
    Enum EnumBodyPart As Integer
        NONE = 0
        HEAD = 1
        BODY = 2
        TAIL = 3
    End Enum
    Public col As Integer = 0
    Public row As Integer = 0
    Public bodyPart As EnumBodyPart = EnumBodyPart.NONE
    Public Function ShallowCopy() As BodyPartClass
        Return DirectCast(Me.MemberwiseClone, BodyPartClass)
    End Function
End Class