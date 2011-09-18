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
Imports MinikeMSServer.Functions

Class ChangeChannelHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region
    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim newChannelID As Integer = packetReader.ReadByte
        Dim packet As Byte() = MaplePacketHandler.getChangeChannel(c.world.ipToByteArray(), c.world.port)
        c.channel.players.Remove(c.Player)
        c.Player.SaveToDB(False, c)
        c.Player.Map.RemovePlayer(c.Player)
        Dim pendingClient As New MapleClient(Nothing, True)
        pendingClient.AccountID = c.AccountID
        pendingClient.AccountName = c.AccountName
        pendingClient.world = c.world
        pendingClient.Player = c.Player
        pendingClient.channel = c.world.Channels(newChannelID)
        pendingClient.specialID = c.specialID
        c.world.PendingClients.Add(pendingClient)
        c.cloned = True
        c.SendPacket(packet)
        Me.Dispose()
    End Sub

End Class
