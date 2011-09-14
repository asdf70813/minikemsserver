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

Imports MapleLib.PacketLib
Imports MinikeMSServer.RecvHeaders

Public Class RecvPacketHandler

    Sub HandlePacket(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim pHeader As Short = packetReader.ReadShort
        Dim handler = Nothing
        Select Case pHeader
            Case LOGIN_PASSWORD
                handler = New LoginHandler(packetReader, c)
            Case SERVERLIST_REQUEST
                handler = New ServerlistRequestHandler(packetReader, c)
            Case SERVERSTATUS_REQUEST
                handler = New ServerStatusRequestHandler(packetReader, c)
        End Select
    End Sub

End Class