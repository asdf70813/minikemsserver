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

Class SelectCharHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim pic As String = packetReader.ReadMapleString
        Dim charId As Integer = packetReader.ReadInt()
        Dim macs As String = packetReader.ReadMapleString
        If c.hasPic Then
            Dim packet As Byte()
            If pic.Equals(c.pic) Then
                Dim pendingClient As New MapleClient(Nothing, True)
                pendingClient.AccountID = c.AccountID
                pendingClient.AccountName = c.AccountName
                pendingClient.world = c.world
                pendingClient.Player = MapleCharacter.LoadFromDB(pendingClient, pendingClient.world.id, charId)(0)
                pendingClient.channel = c.channel
                pendingClient.specialID = CInt(&H7FFFFF - (((charId * 7 ^ 2) Mod 5) * 123.5)) - charId
                c.world.PendingClients.Add(pendingClient)
                packet = MaplePacketHandler.getServerIP(c.world.ipToByteArray(), c.world.port, pendingClient.specialID)
            Else
                packet = MaplePacketHandler.WrongPic()
            End If
            c.SendPacket(packet)
        Else
            c.Disconnect()
        End If
        Me.Dispose()
    End Sub

End Class
