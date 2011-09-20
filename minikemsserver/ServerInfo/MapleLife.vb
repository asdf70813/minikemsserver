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

Public Class MapleLife
    Public id As Integer
    Public oid As Integer
    Public type As String
    Public pos As Point
    Public fh As Integer
    Public startfh As Integer
    Public cy As Integer
    Public rx0 As Integer
    Public rx1 As Integer
    Public f As Integer = 0
    Private Controller As MapleCharacter = Nothing
    Public stance As Integer = 5

    Public Sub setControl(ByVal player As MapleCharacter)
        Controller = player
        If type.Equals("m") Then
            player.client.SendPacket(MaplePacketHandler.controlMob(Me, True, False))
        End If
    End Sub

    Public Sub removeControl()
        Controller = Nothing
    End Sub

    Public Function getController() As MapleCharacter
        Return Controller
    End Function

End Class
