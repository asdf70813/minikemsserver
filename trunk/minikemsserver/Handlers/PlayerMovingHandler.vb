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
Class PlayerMovingHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region
    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        packetReader.ReadBytes(9) 'skip
        Dim res = parseMovement(packetReader, c)
        c.Player.UpdatePosition(res, 0)
        Dim packet As Byte()
        packet = MaplePacketHandler.movePlayer(c.Player.id, res)
        c.Player.Map.BroadCastMessage(c, packet)
    End Sub

    Public Function parseMovement(ByVal packetReader As PacketReader, ByVal c As MapleClient) As List(Of LifeMovement)
        Dim res As New List(Of LifeMovement)
        With packetReader
            Dim totalCommands As Byte = .ReadByte()
            For i As Byte = 0 To totalCommands - 1
                Dim command As Byte = .ReadByte
                Select Case command
                    Case 0, 5, 17
                        Dim xpos As Short = .ReadShort
                        Dim ypos As Short = .ReadShort
                        Dim xwobble As Short = .ReadShort
                        Dim ywobble As Short = .ReadShort
                        Dim unk As Short = .ReadShort
                        Dim newstate As Byte = .ReadByte
                        Dim duration As Short = .ReadShort
                        Dim alm As New AbsoluteLifeMovement(New Point(xpos, ypos), duration, command, newstate)
                        alm.Unk = unk
                        alm.pixelsPerSecond = New Point(xwobble, ywobble)
                        res.Add(alm)
                        Exit Select
                    Case 1, 2, 6, 12, 13, 16
                        Dim xpos As Short = .ReadShort
                        Dim ypos As Short = .ReadShort
                        Dim newstate As Byte = .ReadByte
                        Dim duration As Short = .ReadShort
                        Dim rlm As New RelativeLifeMovement(New Point(xpos, ypos), duration, command, newstate)
                        res.Add(rlm)
                        Exit Select
                    Case 3, 4, 7, 8, 9, 14
                        .ReadBytes(9)
                        Exit Select
                    Case 10 'change equip, TODO: handle it
                    Case 11 'Chair
                        Dim xpos As Short = .ReadShort
                        Dim ypos As Short = .ReadShort
                        Dim unk As Short = .ReadShort
                        Dim newstate As Byte = .ReadByte
                        Dim duration As Short = .ReadShort
                        Dim cm As New ChairMovement(New Point(xpos, ypos), duration, command, newstate)
                        cm.Unk = unk
                        res.Add(cm)
                        Exit Select
                    Case 15
                        Dim xpos As Short = .ReadShort
                        Dim ypos As Short = .ReadShort
                        Dim xwobble As Short = .ReadShort
                        Dim ywobble As Short = .ReadShort
                        Dim unk As Short = .ReadShort
                        Dim foothold As Short = .ReadShort
                        Dim newstate As Byte = .ReadByte
                        Dim duration As Short = .ReadShort
                        Dim jdm As New JumpDownMovement(New Point(xpos, ypos), duration, command, newstate)
                        jdm.Unk = unk
                        jdm.Foothold = foothold
                        jdm.pixelsPerSecond = New Point(xwobble, ywobble)
                        res.Add(jdm)
                        Exit Select
                    Case 21
                        .ReadBytes(3)
                        Exit Select
                End Select
            Next
        End With
        Return res
    End Function

End Class
