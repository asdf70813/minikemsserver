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

Public Class MapleMap
    Public id As Integer = 0
    Private players As New List(Of MapleCharacter)
    Public life As New List(Of MapleLife)
    Public oid As Integer = 0

    Sub New(ByVal mapID As Integer)
        id = mapID
        Me.life = MapleInformationProvider.Map.getLifeOfMap(id)
        For Each lifes In life
            lifes.oid = setAndGetOID()
        Next
    End Sub

    Public Function setAndGetOID() As Integer
plus:   oid += 1
        If oid > &H7FFFFFF0 Then
            oid = 1
        End If
        For Each lifes In life
            If oid - 1 = lifes.oid Then
                GoTo plus
            End If
        Next
        Return oid - 1
    End Function

    Public Sub BroadCastMessage(ByVal c As MapleClient, ByVal packet As Byte())
        For Each chr As MapleCharacter In players
            If Not chr.Equals(c.Player) Then
                chr.client.SendPacket(packet)
            End If
        Next
    End Sub

    Public Sub AddPlayer(ByVal Player As MapleCharacter)
        For Each character In players
            Dim packet As Byte() = MaplePacketHandler.SpawnPlayerOnMap(character.client)
            Player.client.SendPacket(packet)
        Next
        players.Add(Player)
        For Each lifes As MapleLife In Me.life
            If lifes.type.Equals("n") Then
                Player.client.SendPacket(MaplePacketHandler.spawnNpc(lifes))
            End If
            If lifes.type.Equals("m") Then
                Player.client.SendPacket(MaplePacketHandler.spawnMob(lifes, True))
            End If
        Next
    End Sub

    Public Sub RemovePlayer(ByVal player As MapleCharacter)
        For Each character In players
            Dim packet As Byte() = MaplePacketHandler.RemovePlayerFromMap(player.id)
            character.client.SendPacket(packet)
        Next
        players.Remove(player)
    End Sub
End Class
