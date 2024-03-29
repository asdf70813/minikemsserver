﻿'    This file is part of MinikeMSServer.

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

Public Class MapleChannel
    Public load As Integer = 0
    Public id As Short = 0
    Public players As New List(Of MapleCharacter)
    Public Maps As New List(Of MapleMap)

    Public Function getPlayerByname(ByVal name As String) As MapleCharacter
        For Each player In players
            If player.Name.ToLower.Equals(name) Then
                Return player
            End If
        Next
        Return Nothing
    End Function
End Class
