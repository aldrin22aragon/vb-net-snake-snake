Public Class Utils
    Public Const lettersAndNumbersToUper As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 "
    Shared Function GetAsciiArt2(input As String) As String
        ' 1. I-declare ang dictionary na maglalaman ng ASCII lines bawat karakter
        Dim asciiGrid As New Dictionary(Of Char, String())

        ' 2. I-define ang ASCII Art gamit ang puro Number Sign (#) at Space (5 lines ang taas)
        asciiGrid("A"c) = {"  #  ", " # # ", "#####", "#   #", "#   #"}
        asciiGrid("B"c) = {"#### ", "#   #", "#### ", "#   #", "#### "}
        asciiGrid("C"c) = {" ####", "#    ", "#    ", "#    ", " ####"}
        asciiGrid("D"c) = {"#### ", "#   #", "#   #", "#   #", "#### "}
        asciiGrid("E"c) = {"#####", "#    ", "###  ", "#    ", "#####"}
        asciiGrid("F"c) = {"#####", "#    ", "###  ", "#    ", "#    "}
        asciiGrid("G"c) = {" ####", "#    ", "#  ##", "#   #", " ####"}
        asciiGrid("H"c) = {"#   #", "#   #", "#####", "#   #", "#   #"}
        asciiGrid("I"c) = {"#####", "  #  ", "  #  ", "  #  ", "#####"}
        asciiGrid("J"c) = {"#####", "    #", "    #", "#   #", " ### "}
        asciiGrid("K"c) = {"#   #", "#  # ", "###  ", "#  # ", "#   #"}
        asciiGrid("L"c) = {"#    ", "#    ", "#    ", "#    ", "#####"}
        asciiGrid("M"c) = {"#   #", "## ##", "# # #", "#   #", "#   #"}
        asciiGrid("N"c) = {"#   #", "##  #", "# # #", "#  ##", "#   #"}
        asciiGrid("O"c) = {" ### ", "#   #", "#   #", "#   #", " ### "}
        asciiGrid("P"c) = {"#### ", "#   #", "#### ", "#    ", "#    "}
        asciiGrid("Q"c) = {" ### ", "#   #", "# # #", "#  # ", " ## #"}
        asciiGrid("R"c) = {"#### ", "#   #", "#### ", "#  # ", "#   #"}
        asciiGrid("S"c) = {" ####", "#    ", " ### ", "    #", "#### "}
        asciiGrid("T"c) = {"#####", "  #  ", "  #  ", "  #  ", "  #  "}
        asciiGrid("U"c) = {"#   #", "#   #", "#   #", "#   #", " ### "}
        asciiGrid("V"c) = {"#   #", "#   #", "#   #", " # # ", "  #  "}
        asciiGrid("W"c) = {"#   #", "#   #", "# # #", "## ##", "#   #"}
        asciiGrid("X"c) = {"#   #", " # # ", "  #  ", " # # ", "#   #"}
        asciiGrid("Y"c) = {"#   #", " # # ", "  #  ", "  #  ", "  #  "}
        asciiGrid("Z"c) = {"#####", "   # ", "  #  ", " #   ", "#####"}

        ' Mga Numero (0-9)
        asciiGrid("0"c) = {" ### ", "#   #", "#   #", "#   #", " ### "}
        asciiGrid("1"c) = {"  #  ", " ##  ", "  #  ", "  #  ", "#####"}
        asciiGrid("2"c) = {" ### ", "#   #", "   # ", "  #  ", "#####"}
        asciiGrid("3"c) = {"#### ", "    #", " ### ", "    #", "#### "}
        asciiGrid("4"c) = {"#  # ", "#  # ", "#####", "   # ", "   # "}
        asciiGrid("5"c) = {"#####", "#    ", "#### ", "    #", "#####"}
        asciiGrid("6"c) = {" ### ", "#    ", "#### ", "#   #", " ### "}
        asciiGrid("7"c) = {"#####", "   # ", "  #  ", " #   ", "#    "}
        asciiGrid("8"c) = {" ### ", "#   #", " ### ", "#   #", " ### "}
        asciiGrid("9"c) = {" ### ", "#   #", " ####", "    #", " ### "}
        asciiGrid(" "c) = {"     ", "     ", "     ", "     ", "     "} ' Space character

        ' 3. I-convert ang input sa UPPERCASE para mag-match sa dictionary
        Dim malakingText As String = input.ToUpper()

        ' Gagawa ng array ng strings para sa 5 lines ng output
        Dim outputLines(4) As String

        ' 4. I-loop ang bawat character sa iyong input text
        For Each c As Char In malakingText
            If asciiGrid.ContainsKey(c) Then
                Dim characterLines() As String = asciiGrid(c)
                ' Isama ang character line-by-line na may dagdag na dalawang space sa dulo bilang agwat
                For i As Integer = 0 To 4
                    outputLines(i) &= characterLines(i) & "  "
                Next
            End If
        Next

        ' 5. Pagsamahin ang 5 lines gamit ang NewLine para maging isang buong bloke ng text
        Return String.Join(Environment.NewLine, outputLines)
    End Function
End Class
