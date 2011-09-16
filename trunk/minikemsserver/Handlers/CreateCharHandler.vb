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

Class CreateCharHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        Dim name As String = packetReader.ReadMapleString
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
        If cantCreate Then
            c.Disconnect()
            Me.Dispose()
            Return
        End If
        Dim newCharacter As New MapleCharacter(c, name)
        Dim job As Integer = packetReader.ReadInt()
        Dim face As Integer = packetReader.ReadInt()
        Dim hair As Integer = packetReader.ReadInt() + packetReader.ReadInt()
        Dim skincolor As Integer = packetReader.ReadInt()
        If skincolor > 3 Then
            Console.WriteLine(skincolor)
            c.Disconnect()
            Me.Dispose()
            Return
        End If
        newCharacter.hair = hair
        newCharacter.face = face
        newCharacter.skincolor = skincolor

        'TODO: check if not PE exploitable
        Dim top As Integer = packetReader.ReadInt()
        Dim bottom As Integer = packetReader.ReadInt()
        Dim shoes As Integer = packetReader.ReadInt()
        Dim weapon As Integer = packetReader.ReadInt()
        Dim inventory As New MapleInventory()
        newCharacter.Gender = packetReader.ReadByte
        Select Case job
            Case 0
                newCharacter.job = MapleCharacter.Jobs.Noblesse
                inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Etc, 4161047, 0, 1, 0))
            Case 2
                newCharacter.job = MapleCharacter.Jobs.Legend
                inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Etc, 4161048, 0, 1, 0))
            Case Else 'default, could ban for > 2 though
                newCharacter.job = MapleCharacter.Jobs.beginner
                inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Etc, 4161001, 0, 1, 0))
        End Select
        inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Equipped, top, -5, 1, 0))
        inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Equipped, bottom, -6, 1, 0))
        inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Equipped, shoes, -7, 1, 0))
        inventory.ItemList.Add(New MapleInventory.Items(MapleInventory.Types.Equipped, weapon, -11, 1, 0))
        newCharacter = newCharacter.SaveToDB(True, c)
        For Each item In inventory.ItemList
            item.charID = newCharacter.id
        Next
        newCharacter.Inventory = inventory
        newCharacter.Inventory.save()
        Dim packet As Byte() = MaplePacketHandler.addNewCharEntry(newCharacter)
        c.SendPacket(packet)
        Me.Dispose()
    End Sub
End Class