Imports System.Xml
Imports System.IO

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

    Public Sub save(ByVal FileName As String)
        If Splitted Then
            UnSplitItems()
            Splitted = False
        End If
        Try
            Dim writer As New XmlTextWriter(FileName, System.Text.Encoding.UTF8)
            writer.WriteStartElement("ItemList")

            For Each item As Items In ItemList
                writer.WriteStartElement("Item")
                writer.WriteStartElement("ID")
                writer.WriteAttributeString("Value", item.id)
                writer.WriteStartElement("posistion")
                writer.WriteAttributeString("Value", item.position)
                writer.WriteStartElement("type")
                writer.WriteAttributeString("Value", item.type)
                writer.WriteStartElement("quantity")
                writer.WriteAttributeString("Value", item.quantity)
                writer.WriteStartElement("owner")
                writer.WriteAttributeString("Value", item.owner)
                writer.WriteStartElement("flag")
                writer.WriteAttributeString("Value", item.flag)
                writer.WriteStartElement("expiration")
                writer.WriteAttributeString("Value", item.expiration)
                writer.WriteStartElement("giftFrom")
                writer.WriteAttributeString("Value", item.giftFrom)
                For i = 0 To 8
                    writer.WriteEndElement()
                Next
            Next

            writer.WriteEndElement()
            writer.Close()
        Catch ex As Exception
            Console.WriteLine("Error ocured while writing invertory XML FileName: {0} Error: {1}", FileName, ex.ToString)
        End Try
    End Sub

    Public Sub load(ByVal FileName As String)
        Dim reader As New XmlDocument()
        reader.Load(FileName)
        Dim nodelist As XmlNodeList = reader.SelectNodes("/ItemList/Item")
        For Each node As XmlNode In nodelist
            Dim subnode = node.ChildNodes.Item(0)
            Dim _id As Integer = CInt(subnode.Attributes.GetNamedItem("Value").Value)
            subnode = subnode.ChildNodes.Item(0)
            Dim _posistion As Short = CShort(subnode.Attributes.GetNamedItem("Value").Value)
            subnode = subnode.ChildNodes.Item(0)
            Dim _type As Short = CShort(subnode.Attributes.GetNamedItem("Value").Value)
            subnode = subnode.ChildNodes.Item(0)
            Dim _quantity As Short = CShort(subnode.Attributes.GetNamedItem("Value").Value)
            Dim item As Items = New Items(_type, _id, _posistion, _quantity)
            subnode = subnode.ChildNodes.Item(0)
            item.owner = CStr(subnode.Attributes.GetNamedItem("Value").Value)
            subnode = subnode.ChildNodes.Item(0)
            item.flag = CByte(subnode.Attributes.GetNamedItem("Value").Value)
            subnode = subnode.ChildNodes.Item(0)
            item.expiration = CLng(subnode.Attributes.GetNamedItem("Value").Value)
            subnode = subnode.ChildNodes.Item(0)
            item.giftFrom = CStr(subnode.Attributes.GetNamedItem("Value").Value)
            ItemList.Add(item)
        Next
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
        Public id, cashid, sn As Integer
        Public position As Short = 0
        Public type As Short = 0
        Public quantity As Short = 0
        Public owner As String = ""
        Public flag As Byte = 0
        Public expiration As Long = -1
        Public giftFrom As String = ""

        Sub New(ByVal _type As Short, ByVal _id As Integer, ByVal _posistion As Short, ByVal _quantity As Short)
            Type = _type
            id = _id
            position = _posistion
            quantity = _quantity
        End Sub
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