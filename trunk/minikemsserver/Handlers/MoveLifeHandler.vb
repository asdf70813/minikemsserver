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

Class MoveLifeHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim oid As Integer = packetReader.ReadInt
        Dim moveid As Short = packetReader.ReadShort
        Dim currentLife As MapleLife
        Try
            currentLife = c.Player.Map.life(oid)
        Catch ex As Exception
            Me.Dispose()
            Return
        End Try
        If Not currentLife.type.Equals("m") Then
            Me.Dispose()
            Return
        End If
        Dim mob As MapleMob = currentLife
        Dim skillByte As Byte = packetReader.ReadByte
        Dim skill_1 As Byte = packetReader.ReadByte
        Dim skill_2 As Byte = packetReader.ReadByte
        Dim skill_3 As Byte = packetReader.ReadByte
        Dim skill_4 As Byte = packetReader.ReadByte
        Dim skill_5 As Byte = packetReader.ReadByte
        packetReader.ReadLong()
        packetReader.ReadByte()
        packetReader.ReadInt()
        Dim startx As Short = packetReader.ReadShort
        Dim starty As Short = packetReader.ReadShort
        Dim startpos As New Point(startx, starty)
        'TODO: Mobskills and mob mp
        Dim res As List(Of LifeMovement) = AbstractLifeMovement.parseMovement(packetReader, c)
        c.SendPacket(MaplePacketHandler.moveMonsterResponse(oid, moveid, 0, mob.aggro))
        If res.Count > 0 Then
            c.Player.Map.BroadCastMessage(c, MaplePacketHandler.moveMonster(skillByte, skill_1, skill_2, skill_3, skill_4, skill_5, oid, startpos, res))
        End If
        UpdatePosition(res, mob, -1)
        Me.Dispose()
    End Sub

    Private Sub UpdatePosition(ByVal res As List(Of LifeMovement), ByVal mob As MapleMob, ByVal yoffset As Integer)
        For Each move In res
            If TypeOf move Is AbsoluteLifeMovement Then
                Dim newPos As Point = move.getPosition
                newPos.y += yoffset
                mob.pos = newPos
            End If
            mob.stance = move.getNewstate
        Next
    End Sub

End Class
