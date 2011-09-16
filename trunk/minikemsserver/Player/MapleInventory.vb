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
                    query = "UPDATE tbl_inventory SET itemid='" & item.id &
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
                ElseIf count > 1 And Not sqlID = 0 Then
                    Console.WriteLine("[WARNING] Double item count id={0}", sqlID)
                Else
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
            Next
            itemAddCon.Dispose()
        Catch ex As Exception
            Console.WriteLine("Error ocured while writing invertoryDB, Error: {0}", ex.ToString)
        End Try
    End Sub

    Public Sub load(ByVal charID As Integer)
        Dim loadItemsCon As New MySQLCon(Settings.ConnectionString)
        Dim reader As MySqlDataReader = loadItemsCon.ReadQuery("SELECT * FROM tbl_inventory WHERE charid ='" & charID & "'")
        While reader.Read
            Dim item As New Items(reader.GetInt32("type"), reader.GetInt32("itemid"), reader.GetInt32("position"), reader.GetInt32("quantity"), reader.GetInt32("charid"))
            item.owner = reader.GetString("owner")
            item.flag = reader.GetInt32("flag")
            item.expiration = CLng(reader.GetString("expiration"))
            item.giftFrom = reader.GetString("giftfrom")
            item.flag = reader.GetInt32("equipid")
            item.sqlID = reader.GetInt32("id")
            ItemList.Add(item)
        End While
        reader.Dispose()
        loadItemsCon.Dispose()
    End Sub

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

        Sub New(ByVal _type As Short, ByVal _id As Integer, ByVal _posistion As Short, ByVal _quantity As Short, ByVal _charid As Integer)
            type = _type
            id = _id
            charID = _charid
            position = _posistion
            quantity = _quantity
            Select Case type
                Case Types.Equipped, Types.Equip, Types.Cash
                    'Add stat handling here
            End Select
        End Sub
    End Class

    Public Class EquipStats

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