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

Public Class FaceExpressionHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim emoticon As Integer = packetReader.ReadInt
        'TODO; add itemcheck
        Dim packet As Byte() = MaplePacketHandler.facialExpression(c.Player.id, emoticon)
        c.Player.Map.BroadCastMessage(c, packet)
        Me.Dispose()
    End Sub
End Class
