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
Public Class RegisterPICHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        packetReader.ReadByte()
        Dim charId As Integer = packetReader.ReadInt()
        Dim macs As String = packetReader.ReadMapleString
        packetReader.ReadMapleString()
        Dim pic As String = packetReader.ReadMapleString
        If c.hasPic Then
            c.Disconnect()
        Else
            Dim picCon As New MySQLCon(Settings.ConnectionString)
            picCon.ExecuteQuery("UPDATE tbl_accounts SET pic='" & pic & "' WHERE id='" & c.AccountID & "'")
            picCon.Dispose()
            c.hasPic = True
            c.pic = pic
            Dim pendingClient As New MapleClient(Nothing, Nothing, Nothing, True)
            pendingClient.AccountID = c.AccountID
            pendingClient.AccountName = c.AccountName
            pendingClient.world = c.world
            pendingClient.Player = MapleCharacter.LoadFromDB(pendingClient, pendingClient.world.id, charId)(0)
            pendingClient.channel = c.channel
            pendingClient.specialID = (charId * 7) Mod 5
            c.world.PendingClients.Add(pendingClient)
            Dim packet As Byte()
            packet = MaplePacketHandler.getServerIP(c.world.ipToByteArray(), c.world.port, pendingClient.specialID)
            c.SendPacket(packet)
        End If
        Me.Dispose()
    End Sub
End Class
