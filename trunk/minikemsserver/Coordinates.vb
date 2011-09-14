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

Public Class Coordinates
    Private mX As Short
    Private mY As Short

    Public Sub New(ByVal pX As Short, ByVal pY As Short)
        mX = pX
        mY = pY
    End Sub

    Public Property X() As Short
        Get
            Return mX
        End Get
        Set(ByVal value As Short)
            mX = value
        End Set
    End Property
    Public Property Y() As Short
        Get
            Return mY
        End Get
        Set(ByVal value As Short)
            mY = value
        End Set
    End Property

    Public Shared Operator -(ByVal pCoordinates1 As Coordinates, ByVal pCoordinates2 As Coordinates) As Integer
        Return CInt(Math.Sqrt(Math.Pow(pCoordinates1.X - pCoordinates2.X, 2) + Math.Pow(pCoordinates1.Y - pCoordinates2.Y, 2)))
    End Operator
End Class
