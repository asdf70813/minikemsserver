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
Public Class ViewAllCharsHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region
    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim chrList As New List(Of MapleCharacter)
        Dim worldPackets As New List(Of Byte())
        For Each World As MapleWorld In Server.Worlds
            Dim chrsInWorld As List(Of MapleCharacter) = MapleCharacter.LoadAllFromDB(c, World.id)
            chrList.AddRange(chrsInWorld)
            Dim wpacket As Byte()
            wpacket = MaplePacketHandler.showAllCharacterList(World, chrsInWorld)
            worldPackets.Add(wpacket)
        Next
        Dim unk As Integer = chrList.Count + 3 - chrList.Count Mod 3
        Dim packet As Byte()
        packet = MaplePacketHandler.showAllCharacter(chrList.Count, unk)
        c.SendPacket(packet)
        Dim i As Integer = 0
        For Each World As MapleWorld In Server.Worlds
            c.SendPacket(worldPackets.Item(i))
            i += 1
        Next
        Me.Dispose()
    End Sub
End Class
