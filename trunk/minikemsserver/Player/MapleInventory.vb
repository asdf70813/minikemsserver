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

Imports System.Xml
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class MapleInventory
    Public ItemList As New List(Of Items)
    Public Splitted As Boolean = False
    Public Equiped As New List(Of Items)
    Public Equips As New List(Of Items)
    Public Use As New List(Of Items)
    Public Setup As New List(Of Items)
    Public Etc As New List(Of Items)
    Public Cash As New List(Of Items)
    Public slotLimit = 0

    Public Sub save() 'TODO: Equip stats
        If Splitted Then
            UnSplitItems()
            Splitted = False
        End If
        Dim newitem As Boolean = False
        Try
            Dim itemAddCon As New MySQLCon(Settings.ConnectionString)
            For Each item As Items In ItemList
                Dim reader As MySqlDataReader = itemAddCon.ReadQuery("SELECT t.id FROM tbl_inventory t WHERE charid='" & item.charID & "' AND type='" & item.type & "' AND position='" & item.position & "' AND itemid='" & item.id & "'")
                Dim count As Integer = 0
                Dim sqlID As Integer = 0
                Dim query As String = ""
                While reader.Read
                    sqlID = reader.GetInt32("id")
                    count += 1
                End While
                reader.Dispose()
                If count = 1 And Not sqlID = 0 Then
                    query = "UPDATE tbl_inventory SET unk='" & item.unk &
                        "' AND itemid='" & item.id &
                        "' AND position='" & item.position &
                        "' AND type='" & item.type &
                        "' AND quantity='" & item.quantity &
                        "' AND owner='" & item.owner &
                        "' AND flag='" & item.flag &
                        "' AND expiration ='" & item.expiration &
                        "' AND giftfrom='" & item.giftFrom &
                        "' AND charid='" & item.charID &
                        "' AND equipid='" & item.EquipID &
                        "' WHERE id='" & item.sqlID & "'"
                    Console.WriteLine(item.id)
                ElseIf count > 1 And Not sqlID = 0 Then
                    Console.WriteLine("[WARNING] Double item count id={0}", sqlID)
                Else
                    newitem = True
                    query = "INSERT INTO tbl_inventory (itemid,position,type,quantity,owner,flag,expiration,giftfrom,charid,equipid) VALUES('" &
                        item.id & "','" &
                        item.position & "','" &
                        item.type & "','" &
                        item.quantity & "','" &
                        item.owner & "','" &
                        item.flag & "','" &
                        item.expiration & "','" &
                        item.giftFrom & "','" &
                        item.charID & "','" &
                        item.EquipID & "')"
                End If
                If Not query.Equals("") Then
                    itemAddCon.ExecuteQuery(query)
                End If
                reader = itemAddCon.ReadQuery("SELECT t.id FROM tbl_inventory t WHERE charid='" & item.charID & "' AND type='" & item.type & "' AND position='" & item.position & "' AND itemid='" & item.id & "'")
                count = 0
                While reader.Read
                    sqlID = reader.GetInt32("id")
                    count += 1
                End While
                reader.Dispose()
                Select Case item.type
                    Case Types.Equipped, Types.Equip, Types.Cash
                        Try
                            query = ""
                            Dim stats = item.Stats
                            If newitem Then

                                query = "INSERT INTO tbl_equips (slots,str,dex,_int,luk,hp,mp,watk,matk,wdef,mdef,acc,avo,hands,speed,jump,flag,itemlevel,itemexp,vicious,inventoryid) VALUES " &
                                    "('" & stats.slots & "'," &
                                    "'" & stats.str & "'," &
                                    "'" & stats.dex & "'," &
                                    "'" & stats.int & "'," &
                                    "'" & stats.luk & "'," &
                                    "'" & stats.hp & "'," &
                                    "'" & stats.mp & "'," &
                                    "'" & stats.watk & "'," &
                                    "'" & stats.matk & "'," &
                                    "'" & stats.wdef & "'," &
                                    "'" & stats.mdef & "'," &
                                    "'" & stats.acc & "'," &
                                    "'" & stats.avo & "'," &
                                    "'" & stats.hands & "'," &
                                    "'" & stats.speed & "'," &
                                    "'" & stats.jump & "'," &
                                    "'" & stats.flag & "'," &
                                    "'" & stats.itemlevel & "'," &
                                    "'" & stats.itemexp & "'," &
                                    "'" & stats.vicious & "'," &
                                    "'" & sqlID & "')"
                            Else
                                query = "UPDATE tbl_equips SET " &
                                    "unk='" & stats.unk & "' AND " &
                                    "slots='" & stats.slots & "' AND " &
                                    "str='" & stats.str & "' AND " &
                                    "dex='" & stats.dex & "' AND " &
                                    "_int='" & stats.int & "' AND " &
                                    "luk='" & stats.luk & "' AND " &
                                    "hp='" & stats.hp & "' AND " &
                                    "mp='" & stats.mp & "' AND " &
                                    "watk='" & stats.watk & "' AND " &
                                    "matk='" & stats.matk & "' AND " &
                                    "wdef='" & stats.wdef & "' AND " &
                                    "mdef='" & stats.mdef & "' AND " &
                                    "acc='" & stats.acc & "' AND " &
                                    "avo='" & stats.avo & "' AND " &
                                    "hands='" & stats.hands & "' AND " &
                                    "speed='" & stats.speed & "' AND " &
                                    "jump='" & stats.jump & "' AND " &
                                    "flag='" & stats.flag & "' AND " &
                                    "itemlevel='" & stats.itemlevel & "' AND " &
                                    "itemexp='" & stats.itemexp & "' AND " &
                                    "vicious='" & stats.vicious & "' AND " &
                                    "inventoryid='" & sqlID & "'" &
                                    " WHERE inventoryid='" & sqlID & "'"
                            End If
                            itemAddCon.ExecuteQuery(query)
                        Catch ex As Exception
                            Console.WriteLine("Error ocured while writing equipDB, Error: {0}", ex.ToString)
                            Return
                        End Try
                End Select
            Next
            itemAddCon.Dispose()
        Catch ex As Exception
            Console.WriteLine("Error ocured while writing invertoryDB, Error: {0}", ex.ToString)
            Return
        End Try
    End Sub

    Public Sub load(ByVal charID As Integer)
        Dim loadItemsCon As New MySQLCon(Settings.ConnectionString)
        Dim reader As MySqlDataReader = loadItemsCon.ReadQuery("SELECT * FROM tbl_inventory WHERE charid ='" & charID & "'")
        While reader.Read
            Dim _type = reader.GetInt32("type")
            Dim item As New Items(_type, reader.GetInt32("itemid"), reader.GetInt32("position"), reader.GetInt32("quantity"), reader.GetInt32("charid"))
            item.owner = reader.GetString("owner")
            item.flag = reader.GetInt32("flag")
            item.expiration = CLng(reader.GetString("expiration"))
            item.giftFrom = reader.GetString("giftfrom")
            item.EquipID = reader.GetInt32("equipid")
            item.sqlID = reader.GetInt32("id")
            Select Case _type
                Case Types.Equipped, Types.Equip, Types.Cash
                    item.Stats = loadStats(item.sqlID)
            End Select
            ItemList.Add(item)
        End While
        reader.Dispose()
        loadItemsCon.Dispose()
    End Sub

    Public Function loadStats(ByVal sqlid As Integer) As EquipStats
        Dim stats As New EquipStats
        Dim loadStatsCon As New MySQLCon(Settings.ConnectionString)
        Dim reader As MySqlDataReader = loadStatsCon.ReadQuery("SELECT * FROM tbl_equips WHERE inventoryid='" & sqlid & "'")
        While reader.Read
            With stats
                .slots = reader.GetInt32("slots")
                .str = reader.GetInt32("str")
                .dex = reader.GetInt32("dex")
                .int = reader.GetInt32("_int")
                .luk = reader.GetInt32("luk")
                .hp = reader.GetInt32("hp")
                .mp = reader.GetInt32("mp")
                .watk = reader.GetInt32("watk")
                .matk = reader.GetInt32("matk")
                .wdef = reader.GetInt32("wdef")
                .mdef = reader.GetInt32("mdef")
                .acc = reader.GetInt32("acc")
                .avo = reader.GetInt32("avo")
                .hands = reader.GetInt32("hands")
                .speed = reader.GetInt32("speed")
                .jump = reader.GetInt32("jump")
                .itemlevel = reader.GetInt32("itemlevel")
                .vicious = reader.GetInt32("vicious")
                .itemexp = reader.GetInt32("itemexp")
            End With
        End While
        reader.Dispose()
        loadStatsCon.Dispose()
        Return stats
    End Function

    Public Sub SplitItems()
        For Each item As Items In ItemList
            Select Case item.type
                Case Types.Equipped
                    Equiped.Add(item)
                Case Types.Equip
                    Equips.Add(item)
                Case Types.Use
                    Use.Add(item)
                Case Types.Setup
                    Setup.Add(item)
                Case Types.Etc
                    Etc.Add(item)
                Case Types.Cash
                    Cash.Add(item)
            End Select
        Next
        Splitted = True
        ItemList.Clear()
    End Sub

    Public Sub UnSplitItems()
        For Each item As Items In Equiped
            ItemList.Add(item)
        Next
        Equiped.Clear()
        For Each item As Items In Equips
            ItemList.Add(item)
        Next
        Equips.Clear()
        For Each item As Items In Use
            ItemList.Add(item)
        Next
        Use.Clear()
        For Each item As Items In Setup
            ItemList.Add(item)
        Next
        Setup.Clear()
        For Each item As Items In Etc
            ItemList.Add(item)
        Next
        Etc.Clear()
        For Each item As Items In Cash
            ItemList.Add(item)
        Next
        Cash.Clear()
    End Sub

    Public Class Items
        Public sqlID As Integer = 0
        Public charID As Integer = 0
        Public id, cashid, sn As Integer
        Public position As Short = 0
        Public type As Short = 0
        Public quantity As Short = 0
        Public owner As String = ""
        Public flag As Byte = 0
        Public expiration As Long = -1
        Public giftFrom As String = ""
        Public EquipID As Integer = 0
        Public Stats As EquipStats = Nothing
        Public unk As Integer = 0 'some wierd bug, long story

        Sub New(ByVal _type As Short, ByVal _id As Integer, ByVal _posistion As Short, ByVal _quantity As Short, ByVal _charid As Integer)
            type = _type
            id = _id
            charID = _charid
            position = _posistion
            quantity = _quantity
            Select Case type
                Case Types.Equipped, Types.Equip, Types.Cash
                    Stats = MapleInformationProvider.Item.getEquipStatsByID(id, True)
            End Select
        End Sub
    End Class

    Public Class EquipStats
        Public unk As Integer = 0 'some wierd bug, long story
        Public slots As Byte = 0
        Public reqlevel As Byte = 0
        Public reqjob As Short = 0
        Public reqstr As Short = 0
        Public reqdex As Short = 0
        Public reqint As Short = 0
        Public reqluk As Short = 0
        Public str As Short = 0
        Public dex As Short = 0
        Public int As Short = 0
        Public luk As Short = 0
        Public hp As Short = 0
        Public mp As Short = 0
        Public watk As Short = 0
        Public matk As Short = 0
        Public wdef As Short = 0
        Public mdef As Short = 0
        Public acc As Short = 0
        Public avo As Short = 0
        Public hands As Short = 0
        Public speed As Short = 0
        Public jump As Short = 0
        Public flag As Short = 0
        Public itemlevel As Byte = 0
        Public itemexp As Short = 0
        Public vicious As Integer = 0
    End Class

    Public Enum Types As Short
        Equipped = -1
        Undefinded = 0
        Equip = 1
        Use = 2
        Setup = 3
        Etc = 4
        Cash = 5
    End Enum
End Class