﻿'    This file is part of MinikeMSServer.

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

Imports MapleLib.PacketLib
Public Class ChairMovement
    Inherits AbstractLifeMovement

    Public Unk As Integer

    Sub New(ByVal _position As Point, ByVal _duration As Integer, ByVal _type As Byte, ByVal _newstate As Byte)
        MyBase.New(_position, _duration, _type, _newstate)
    End Sub

    Public Overrides Sub serialize(ByVal writer As PacketWriter)
        writer.WriteByte(getType1())
        writer.WriteShort(getPosition().x)
        writer.WriteShort(getPosition().y)
        writer.WriteShort(Unk)
        writer.WriteByte(getNewstate())
        writer.WriteShort(getDuration())
    End Sub
End Class
