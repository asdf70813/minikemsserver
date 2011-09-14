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

Friend NotInheritable Class ByteArraySegment
    Private mBuffer As Byte() = Nothing
    Private mStart As Integer = 0
    Private mLength As Integer = 0

    Public Sub New(ByVal pBuffer As Byte())
        mBuffer = pBuffer
        mLength = mBuffer.Length
    End Sub
    Public Sub New(ByVal pBuffer As Byte(), ByVal pStart As Integer, ByVal pLength As Integer)
        mBuffer = pBuffer
        mStart = pStart
        mLength = pLength
    End Sub

    Public ReadOnly Property Buffer() As Byte()
        Get
            Return mBuffer
        End Get
    End Property
    Public ReadOnly Property Start() As Integer
        Get
            Return mStart
        End Get
    End Property
    Public ReadOnly Property Length() As Integer
        Get
            Return mLength
        End Get
    End Property
    Public Function Advance(ByVal pLength As Integer) As Boolean
        mStart += pLength
        mLength -= pLength
        If mLength <= 0 Then
            Return True
        End If
        Return False
    End Function
End Class