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

Imports MySql.Data.MySqlClient

Public Class MapleCharacter
    Public id As Integer = 0
    Public accountid As Integer = 0
    Public worldId As Integer = 0
    Public Name As String = ""
    Public IsGM As Boolean = False
    Public Gender As Byte = 0
    Public skincolor As Byte = 0
    Public hair As Integer = 0
    Public face As Integer = 0
    Public level As Byte = 0
    Public job As Short = 0
    Public str As Short = 0
    Public dex As Short = 0
    Public int As Short = 0
    Public luk As Short = 0
    Public curHp As Short = 0
    Public maxHp As Short = 0
    Public curMp As Short = 0
    Public maxMp As Short = 0
    Public ap As Short = 0
    Public sp As Short = 0
    Public exp As Integer = 0
    Public fame As Short = 0
    Public gachaExp As Integer = 0
    Public mapId As Integer = 0
    Public spawnpoint As Byte = 0
    Public slots As Byte() = New Byte(4) {&H60, &H60, &H60, &H60, &H60}
    Public client As MapleClient = Nothing
    Public Inventory As MapleInventory = New MapleInventory()
    Public Position As New Point(0, 0)
    Public stance As Byte = 0
    Public Map As MapleMap = Nothing

    Public Sub New(ByVal c As MapleClient, ByVal _name As String, Optional ByVal clean As Boolean = False)
        If clean Then
            Return
        End If
        accountid = c.AccountID
        Name = _name
        worldId = c.world.id
        Dim charnameCon As New MySQLCon(Settings.ConnectionString)
        Dim reader As MySqlDataReader = charnameCon.ReadQuery("SELECT * FROM tbl_characters WHERE name='" & Name & "' AND WorldID ='" & c.world.id & "'")
        While reader.Read
            Console.WriteLine("[WARNING] ({0}) Trying to register an already existing charname", c.Host)
            c.Disconnect()
            Return
        End While
        reader = Nothing
        charnameCon.Dispose()
        curHp = 50
        maxHp = 50
        curMp = 5
        maxMp = 5
        str = 12
        dex = 5
        int = 4
        luk = 4
        level = 1
        client = c
        'TODO: Add inventory,keymap, etc
    End Sub

    Public Sub disconnect()
        Me.SaveToDB(False, Me.client)
        Me.Map.RemovePlayer(Me)
        Me.client.channel.players.Remove(Me)
    End Sub

    Public Function SaveToDB(ByVal newchr As Boolean, ByVal c As MapleClient) As MapleCharacter
        Dim retChar As MapleCharacter = Nothing
        If newchr Then
            Dim saveCon As New MySQLCon(Settings.ConnectionString)
            Dim saveQuery As String = "INSERT INTO tbl_characters (accountid,Name,WorldID,GM,Gender,Skin,Hair,Face,Level,job,str,dex,_int,luk,curHp,maxHp,curMp,maxMp,ap,sp,exp,fame,gachaExp,mapId,spawnpoint) VALUES ('" &
                                    accountid & "','" &
                                    Name & "','" &
                                    worldId & "','" &
                                    IsGM.ToString & "','" &
                                    Gender & "','" &
                                    skincolor & "','" &
                                    hair & "','" &
                                    face & "','" &
                                    level & "','" &
                                    job & "','" &
                                    str & "','" &
                                    dex & "','" &
                                    int & "','" &
                                    luk & "','" &
                                    curHp & "','" &
                                    maxHp & "','" &
                                    curMp & "','" &
                                    maxMp & "','" &
                                    ap & "','" &
                                    sp & "','" &
                                    exp & "','" &
                                    fame & "','" &
                                    gachaExp & "','" &
                                    mapId & "','" &
                                    spawnpoint & "')"
            saveCon.ExecuteQuery(saveQuery)
            Dim reader As MySqlDataReader = saveCon.ReadQuery("SELECT t.id FROM tbl_characters t WHERE name='" & Name & "' AND WorldID='" & worldId & "' ORDER BY id DESC")
            While reader.Read()
                retChar = LoadFromDB(c, c.world.id, reader.GetInt32("id"))(0)
                Exit While
            End While
            reader.Dispose()
            saveCon.Dispose()
            saveQuery = Nothing
        Else
            Dim saveCon As New MySQLCon(Settings.ConnectionString)
            Dim saveQuery As String = "UPDATE tbl_characters SET " &
                                "accountid='" & accountid & "'," &
                                "worldId='" & worldId & "'," &
                                "name='" & Name & "'," &
                                "GM='" & IsGM.ToString & "'," &
                                "Gender='" & Gender & "'," &
                                "Skin='" & skincolor & "'," &
                                "Hair='" & hair & "'," &
                                "Face='" & face & "'," &
                                "Level='" & level & "'," &
                                "Job='" & job & "'," &
                                "str='" & str & "'," &
                                "dex='" & dex & "'," &
                                "_int='" & int & "'," &
                                "luk='" & luk & "'," &
                                "curHp='" & curHp & "'," &
                                "maxHp='" & maxHp & "'," &
                                "curMp='" & curMp & "'," &
                                "maxmp='" & maxMp & "'," &
                                "ap='" & ap & "'," &
                                "sp='" & sp & "'," &
                                "exp='" & exp & "'," &
                                "fame='" & fame & "'," &
                                "gachaExp='" & gachaExp & "'," &
                                "mapId='" & mapId & "'," &
                                "spawnpoint='" & spawnpoint & "'" &
                                "WHERE id='" & id & "'"
            saveCon.ExecuteQuery(saveQuery)
            saveCon.Dispose()
            saveQuery = Nothing
            Inventory.save()
        End If
        Return retChar
    End Function

    Public Shared Function LoadFromDB(ByVal c As MapleClient, ByVal WorldID As Integer, Optional ByVal CharID As Integer = -1) As List(Of MapleCharacter)
        Dim CharCon As New MySQLCon(Settings.ConnectionString)
        Dim query = "SELECT * FROM tbl_characters WHERE accountid='" & c.AccountID & "' AND WorldId='" & WorldID & "'"
        If Not CharID = -1 Then
            query &= " AND id='" & CharID & "'"
        End If
        Dim reader As MySqlDataReader = CharCon.ReadQuery(query)
        Dim charList As New List(Of MapleCharacter)
        While reader.Read
            Dim chr As New MapleCharacter(c, "", True)
            chr.id = reader.GetInt32("id")
            chr.accountid = reader.GetInt32("accountid")
            chr.worldId = reader.GetInt32("WorldId")
            chr.Name = reader.GetString("name")
            chr.IsGM = Boolean.Parse(reader.GetString("GM"))
            chr.Gender = reader.GetInt32("Gender")
            chr.skincolor = reader.GetInt32("Skin")
            chr.hair = reader.GetInt32("hair")
            chr.face = reader.GetInt32("face")
            chr.level = reader.GetInt32("level")
            chr.job = reader.GetInt32("job")
            chr.str = reader.GetInt32("str")
            chr.dex = reader.GetInt32("dex")
            chr.int = reader.GetInt32("_int")
            chr.luk = reader.GetInt32("luk")
            chr.curHp = reader.GetInt32("curHp")
            chr.maxHp = reader.GetInt32("maxHp")
            chr.curMp = reader.GetInt32("curMp")
            chr.maxMp = reader.GetInt32("maxMp")
            chr.ap = reader.GetInt32("ap")
            chr.sp = reader.GetInt32("sp")
            chr.fame = reader.GetInt32("fame")
            chr.gachaExp = reader.GetInt32("gachaExp")
            chr.mapId = reader.GetInt32("mapId")
            chr.spawnpoint = reader.GetInt32("spawnpoint")
            Dim inv As New MapleInventory
            inv.load(chr.id)
            chr.Inventory = inv
            charList.Add(chr)
        End While
        Return charList
    End Function

    Public Sub UpdatePosition(ByVal movement As List(Of LifeMovement), ByVal yoffset As Integer)
        For Each move As LifeMovement In movement
            If TypeOf move Is LifeMovement Then
                If TypeOf move Is AbsoluteLifeMovement Then
                    Dim pos As Point = move.getPosition
                    pos.y += yoffset
                    Position = pos
                End If
                stance = move.getNewstate
            End If
        Next
    End Sub

    Public Enum Jobs As Integer
        beginner = 0

        Warrior = 100
        Fighter = 110
        Crusader = 111
        Hero = 112
        Page = 120
        WhiteKnight = 121
        Paladin = 122
        Spearman = 130
        DragonKnight = 131
        DarkKnight = 132

        Magician = 200
        FP_Wizard = 210
        FP_Mage = 211
        FP_ArchMage = 212
        IL_Wizard = 220
        IL_Mage = 221
        IL_ArchMage = 222
        Cleric = 230
        Priest = 231
        Bishop = 232

        Bowman = 300
        Hunter = 310
        Ranger = 311
        BowMaster = 312
        Crossbowman = 320
        Sniper = 321
        Marksman = 322

        Thief = 400
        Assassin = 410
        Hermit = 411
        NightLord = 412
        Bandit = 420
        ChiefBanfit = 421
        Shadower = 422

        Pirate = 500
        Brawler = 510
        Marauder = 511
        Buccaneer = 512
        Gunslinger = 520
        Outlaw = 521
        Corsair = 522

        MapleLeaf_Brigadier = 800
        GM = 900
        SuperGM = 910

        Noblesse = 1000
        DawnWarrior1 = 1100
        DawnWarrior2 = 1110
        DawnWarrior3 = 1111
        DawnWarrior4 = 1112
        BlazeWizard1 = 1200
        BlazeWizard2 = 1210
        BlazeWizard3 = 1211
        BlazeWizard4 = 1212
        WindArcher1 = 1300
        WindArcher2 = 1310
        WindArcher3 = 1311
        WindArcher4 = 1312
        NightWalker1 = 1400
        NightWalker2 = 1410
        NightWalker3 = 1411
        NightWalker4 = 1412
        ThunderBreaker1 = 1500
        ThunderBreaker2 = 1510
        ThunderBreaker3 = 1511
        ThunderBreaker4 = 1512

        Legend = 2000
        Aran1 = 2100
        Aran2 = 2110
        Aran3 = 2111
        Aran4 = 2112
    End Enum
End Class