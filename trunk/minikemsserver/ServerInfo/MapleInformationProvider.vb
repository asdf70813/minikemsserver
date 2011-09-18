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
Imports MinikeMSServer.Functions

Public Class MapleInformationProvider
    Public Class Item
        Public Shared Function getEquipStatsByID(ByVal itemId As Integer, ByVal randomize As Boolean) As MapleInventory.EquipStats
            Dim stats As New MapleInventory.EquipStats
            Dim file = FileSearch(System.AppDomain.CurrentDomain.BaseDirectory & "\wz\character.wz", itemId & ".img.xml")
            If file = "Nothing" Then
                Console.WriteLine("[ERROR] xml not found file={0}", file)
                Return Nothing
            End If
            Dim xml As New XmlDocument
            Dim nodelist As XmlNodeList
            Dim node As XmlNode
            Dim child As XmlNode
            xml.Load(file)
            nodelist = xml.SelectNodes("/imgdir/imgdir")
            For Each node In nodelist
                If node.Attributes.GetNamedItem("name").Value.ToString.ToLower.Equals("info") Then
                    For Each child In node.ChildNodes
                        Select Case child.Attributes.GetNamedItem("name").Value.ToString.ToLower
                            Case "incpad"
                                stats.watk = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.watk = RandomizeStat(stats.watk, 5)
                                End If
                            Case "incmad"
                                stats.matk = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.matk = RandomizeStat(stats.matk, 5)
                                End If
                            Case "incpdd"
                                stats.wdef = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.wdef = RandomizeStat(stats.wdef, 10)
                                End If
                            Case "incmdd"
                                stats.mdef = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.mdef = RandomizeStat(stats.mdef, 10)
                                End If
                            Case "incmhp"
                                stats.hp = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.hp = RandomizeStat(stats.hp, 10)
                                End If
                            Case "incmmp"
                                stats.mp = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.mp = RandomizeStat(stats.mp, 10)
                                End If
                            Case "tuc"
                                stats.slots = CByte(child.Attributes.GetNamedItem("value").Value.ToString)
                            Case "incstr"
                                stats.str = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.str = RandomizeStat(stats.str, 5)
                                End If
                            Case "incdex"
                                stats.dex = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.dex = RandomizeStat(stats.dex, 5)
                                End If
                            Case "incluk"
                                stats.luk = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.luk = RandomizeStat(stats.luk, 5)
                                End If
                            Case "incint"
                                stats.int = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.int = RandomizeStat(stats.int, 5)
                                End If
                            Case "incacc"
                                stats.acc = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.acc = RandomizeStat(stats.acc, 5)
                                End If
                            Case "inceva"
                                stats.avo = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.avo = RandomizeStat(stats.avo, 5)
                                End If
                            Case "incspeed"
                                stats.speed = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.speed = RandomizeStat(stats.speed, 5)
                                End If
                            Case "incjump"
                                stats.jump = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                                If randomize Then
                                    stats.jump = RandomizeStat(stats.jump, 5)
                                End If
                            Case "reqjob"
                                stats.reqjob = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                            Case "reqlevel"
                                stats.reqlevel = CByte(child.Attributes.GetNamedItem("value").Value.ToString)
                            Case "reqstr"
                                stats.reqstr = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                            Case "reqdex"
                                stats.reqdex = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                            Case "reqint"
                                stats.reqint = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                            Case "reqluk"
                                stats.reqluk = CShort(child.Attributes.GetNamedItem("value").Value.ToString)
                        End Select
                    Next
                End If
            Next
            Return stats
        End Function
    End Class

    Public Class Portal
        Public Shared Function getSpawnPointForPortal(ByVal Map As MapleMap, ByVal MapIdTo As Integer, ByVal portalName As String) As Integer
            Dim retVal As Integer = 0
            Dim mapid As String = Map.id
            While mapid.Length < 9
                mapid = "0" & mapid
            End While
            Dim startNum As Char = mapid.ToCharArray()(0)
            Dim file = FileSearch(System.AppDomain.CurrentDomain.BaseDirectory & "wz\Map.wz\Map\Map" & startNum, mapid & ".img.xml")
            If file = "Nothing" Then
                Console.WriteLine("[ERROR] xml not found file={0}", System.AppDomain.CurrentDomain.BaseDirectory & "wz\Map.wz\Map\Map" & startNum)
                Return Nothing
            End If
            Dim xml As New XmlDocument
            Dim nodelist As XmlNodeList
            Dim node As XmlNode
            Dim child As XmlNode
            Dim subChild As XmlNode
            Dim pn As String
            xml.Load(file)
            nodelist = xml.SelectNodes("/imgdir/imgdir")
            For Each node In nodelist
                If node.Attributes.GetNamedItem("name").Value.ToString.ToLower.Equals("info") Then
                    For Each child In node.ChildNodes
                        For Each subChild In child.ChildNodes
                            If subChild.Attributes.GetNamedItem("name").Value.ToString.ToLower.Equals("pn") Then
                                pn = subChild.Attributes.GetNamedItem("value").Value.ToString
                            End If
                        Next
                    Next
                End If
            Next
            Return retVal
        End Function

    End Class

    Public Shared Function RandomizeStat(ByVal in_val As Integer, ByVal max_range As Integer)
        Dim stat = in_val
        If stat = 0 Then
            Return 0
        End If
        Dim lmax_range = CInt(Math.Min(Math.Ceiling(stat * 0.1), max_range))
        stat = ((stat - max_range) + Math.Floor(RandomDouble() * (max_range * 2 + 1)))
        If stat < 0 Then
            Return 0
        End If
        Return CShort(stat)
    End Function

    Public Shared Function FileSearch(ByVal directory_path As String, ByVal fileextension As String)
        Dim found_file As String = "Nothing"
        Dim files As String() = IO.Directory.GetFiles(directory_path)
        For Each file As String In files
            If file.EndsWith(fileextension) Then
                Return file
            End If
        Next
        Dim directories As String() = IO.Directory.GetDirectories(directory_path)
        For Each directory As String In directories
            found_file = FileSearch(directory, fileextension)
            If found_file <> "Nothing" Then
                Exit For
            End If
        Next
        Return found_file
    End Function
End Class