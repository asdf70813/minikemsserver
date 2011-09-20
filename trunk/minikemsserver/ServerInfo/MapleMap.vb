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
            lifes.oid = life.IndexOf(lifes)
        Next
    End Sub

    Public Sub BroadCastMessage(ByVal c As MapleClient, ByVal packet As Byte())
        For Each chr As MapleCharacter In players
            If Not chr.Equals(c.Player) Then
                chr.client.SendPacket(packet)
            End If
        Next
    End Sub

    Public Sub BroadCastGMMessage(ByVal c As MapleClient, ByVal packet As Byte())
        For Each chr As MapleCharacter In players
            If Not chr.Equals(c.Player) And chr.IsGM Then
                chr.client.SendPacket(packet)
            End If
        Next
    End Sub

    Public Sub AddPlayer(ByVal Player As MapleCharacter)
        Dim packet As Byte()
        packet = MaplePacketHandler.SpawnPlayerOnMap(Player.client)
        If Player.hidden Then
            BroadCastGMMessage(Player.client, packet)
        Else
            BroadCastMessage(Player.client, packet)
        End If
        For Each character In players
            If character.id <> Player.id Then
                packet = MaplePacketHandler.SpawnPlayerOnMap(character.client)
                Player.client.SendPacket(packet)
            End If
        Next
        SyncLock players
            players.Add(Player)
        End SyncLock
        For Each lifes As MapleLife In Me.life
            If lifes.type.Equals("n") Then
                Player.client.SendPacket(MaplePacketHandler.spawnNpc(lifes))
            End If
            If lifes.type.Equals("m") Then
                Player.client.SendPacket(MaplePacketHandler.spawnMob(lifes, True))
            End If
            If IsNothing(lifes.getController) Then
                lifes.setControl(Player)
            End If
        Next
    End Sub

    Public Sub RemovePlayer(ByVal player As MapleCharacter)
        Dim packet As Byte()
        For Each character In players
            If character.id Then
                packet = MaplePacketHandler.RemovePlayerFromMap(character.id)
                player.client.SendPacket(packet)
            End If
        Next
        SyncLock players
            players.Remove(player)
        End SyncLock
        packet = MaplePacketHandler.RemovePlayerFromMap(player.id)
        If player.hidden Then
            BroadCastGMMessage(player.client, packet)
        Else
            BroadCastMessage(player.client, packet)
        End If
        player.client.SendPacket(MaplePacketHandler.RemovePlayerFromMap(player.id))
        For Each lifes As MapleLife In Me.life
            If lifes.getController.id = player.id Then
                lifes.removeControl()
            End If
        Next
        If players.Count > 0 Then
            For Each lifes As MapleLife In Me.life
                If IsNothing(lifes.getController) Then
                    lifes.setControl(players(0))
                End If
            Next
        End If
    End Sub

End Class