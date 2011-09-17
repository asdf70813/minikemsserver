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

Public Class AbstractLifeMovement
    Implements LifeMovement



    Private position As Point
    Private duration As Integer
    Private type, newstate As Byte

    Sub New(ByVal _position As Point, ByVal _duration As Integer, ByVal _type As Byte, ByVal _newstate As Byte)
        position = _position
        duration = _duration
        type = _type
        newstate = _newstate
    End Sub

    Public Function getDuration() As Integer Implements LifeMovement.getDuration
        Return duration
    End Function

    Public Function getNewstate() As Byte Implements LifeMovement.getNewstate
        Return newstate
    End Function

    Public Function getType1() As Byte Implements LifeMovement.getType
        Return type
    End Function

    Public Function getPosition() As Point Implements LifeMovement.getPosition
        Return position
    End Function

    Public Overridable Sub serialize(ByVal writer As MapleLib.PacketLib.PacketWriter) Implements LifeMovement.serialize

    End Sub
End Class
