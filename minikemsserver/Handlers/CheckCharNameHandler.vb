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
Imports MySql.Data.MySqlClient

Public Class CheckCharNameHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region
    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim name As String = packetReader.ReadMapleString()
        Dim cantCreate As Boolean = False
        Dim charnameCon As New MySQLCon(Settings.ConnectionString)
        Dim reader As MySqlDataReader = charnameCon.ReadQuery("SELECT * FROM tbl_characters WHERE name='" & name & "'")
        Dim buff As String = Nothing
        While reader.Read
            buff = reader.GetString("name")
            If buff <> "" Then
                cantCreate = True
            End If
        End While
        buff = Nothing
        reader = Nothing
        charnameCon.Dispose()
        Dim packet As Byte() = MaplePacketHandler.CheckCharNameResponse(name, cantCreate)
        c.SendPacket(packet)
        Me.Dispose()
    End Sub
End Class
