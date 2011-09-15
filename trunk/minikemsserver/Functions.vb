'    This file is part of MinikeMSServer.

'    MinikeMSServer is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.

'    MinikeMSServer is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.

'    You should have received a copy of the GNU General Public License
'    along with MinikeMSServer.  If not, see <http://www.gnu.org/licenses/>.
Imports System.Runtime.InteropServices

Public Class Functions

    Private Shared r As New Random()



    Public Shared Function RandomByte() As Byte
        Randomize()
        Return Math.Floor(r.Next() / &H1010101)
    End Function

    Public Shared Function Random() As Integer
        Randomize()
        Return Math.Floor(r.Next())
    End Function

    Public Shared Function Random(ByVal min As Integer, ByVal max As Integer) As Integer
        Randomize()
        Return Math.Floor(r.Next(min, max))
    End Function

    Public Shared Function StrToByteArray(ByVal strInput As String) As Byte()
        Dim i As Integer = 0
        Dim x As Integer = 0
        Dim bytes As Byte() = New Byte((strInput.Length) \ 2 - 1) {}
        While strInput.Length > i + 1
            Dim lngDecimal As Long = Convert.ToInt32(strInput.Substring(i, 2), 16)
            bytes(x) = Convert.ToByte(lngDecimal)
            i = i + 2
            x += 1
        End While
        Return bytes
    End Function

    Public Shared Function ByteArrayToStr(ByVal bytes As Byte()) As String
        Dim str As String = ""
        Dim sbstr As String
        For Each b As Byte In bytes
            sbstr = Hex(b)
            While sbstr.Length <= 1
                sbstr = "0" & sbstr
            End While
            str += sbstr
        Next
        Return str
    End Function

    Public Shared Function hStrToByteArray(ByVal str As String) As Byte()
        Dim encoding As New System.Text.UTF8Encoding()
        Return encoding.GetBytes(str)
    End Function
End Class
