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
Imports MinikeMSServer.SendHeaders
Imports MinikeMSServer.Functions

Public Class MaplePacketHandler
    Public Shared Function LoginSucces(ByVal c As MapleClient) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(LOGIN_STATUS)
        writer.WriteInt(0)
        writer.WriteShort(0)
        writer.WriteInt(0) 'TODO: AccountID
        writer.WriteByte(0) 'TODO: Gender
        writer.WriteBool(False) 'TODO: Set admin thingy
        writer.WriteByte(0)
        writer.WriteByte(0)
        writer.WriteMapleString(c.AccountName)
        writer.WriteByte(0)
        writer.WriteByte(0) 'isquietbanned
        writer.WriteLong(0)
        writer.WriteLong(0)
        writer.WriteInt(0)
        writer.WriteShort(2) 'pin
        Return writer.ToArray
    End Function

    Public Shared Function LoginFailed(ByVal reason As Integer) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(LOGIN_STATUS)
        writer.WriteInt(reason)
        writer.WriteShort(0)
        Return writer.ToArray
    End Function

    Public Shared Function GetServerList(ByVal world As MapleWorld) As Byte() 'TODO, use variables for server name etc
        Dim writer As New PacketWriter
        writer.WriteShort(SERVERLIST)
        writer.WriteByte(world.id)
        writer.WriteMapleString(world.Name)
        writer.WriteByte(world.Flag)
        writer.WriteMapleString(world.eventMessage)
        writer.WriteByte(&H64)
        writer.WriteByte(0)
        writer.WriteByte(&H64)
        writer.WriteByte(0)
        writer.WriteByte(0)
        Dim lastchannel As Byte = world.Channels.Count
        writer.WriteByte(lastchannel)
        For Each channel As MapleChannel In world.Channels
            writer.WriteMapleString(world.Name & "-" & (channel.id + 1).ToString)
            writer.WriteInt(channel.load) 'Load
            writer.WriteByte(1)
            writer.WriteShort(channel.id + 1)
        Next
        writer.WriteShort(0)
        Return writer.ToArray
    End Function

    Public Shared Function EndOfGetServerList() As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(SERVERLIST)
        writer.WriteByte(&HFF)
        Return writer.ToArray
    End Function

    Public Shared Function SelectWorld() As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(SELECT_WORLD)
        writer.WriteInt(0) 'world id
        Return writer.ToArray
    End Function

    Public Shared Function sendRecommended(ByVal world As MapleWorld) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(SEND_RECOMMENDED)
        writer.WriteByte(1)
        writer.WriteInt(world.id) 'world id
        writer.WriteMapleString(world.Name) 'no clue
        Return writer.ToArray
    End Function

    Shared Function getServerStatus(ByVal status As Short) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(SERVERSTATUS)
        writer.WriteShort(status)
        Return writer.ToArray
    End Function

    Shared Function sendCharList(ByVal c As MapleClient) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(CHARLIST)
        writer.WriteByte(0)
        Dim chars As List(Of MapleCharacter) = c.loadCharacters()
        If IsNothing(chars) Then
            writer.WriteByte(0)
        Else
            writer.WriteByte(chars.Count)
            For Each character As MapleCharacter In chars
                character.Inventory.SplitItems()
                addCharEntry(writer, character, False)
            Next
        End If
        If Settings.pic Then
            writer.WriteBool(c.hasPic)
        Else
            writer.WriteByte(2)

        End If
        writer.WriteInt(Settings.CharacterSlots) 'charslots
        Return writer.ToArray()
    End Function

    Private Shared Sub addCharEntry(ByVal writer As PacketWriter, ByVal chr As MapleCharacter, ByVal viewall As Boolean)
        addCharStats(writer, chr)
        addCharLook(writer, chr, False)
        If Not viewall Then
            writer.WriteByte(0)
        End If
        If chr.IsGM Then
            writer.WriteByte(0)
            Return
        End If
        writer.WriteByte(1)
        writer.WriteInt(1)
        writer.WriteInt(1)
        writer.WriteInt(1)
        writer.WriteInt(1)
    End Sub

    Private Shared Sub addCharStats(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        writer.WriteInt(chr.id)
        Dim name As String = chr.Name
        While name.Length < 13
            name = name & ControlChars.NullChar
        End While
        writer.WriteString(name)
        writer.WriteByte(chr.Gender)
        writer.WriteByte(chr.skincolor)
        writer.WriteInt(chr.face)
        writer.WriteInt(chr.hair)

        writer.WriteLong(0) 'TODO: pets
        writer.WriteLong(0)
        writer.WriteLong(0)

        writer.WriteByte(chr.level)
        writer.WriteShort(chr.job)
        writer.WriteShort(chr.str)
        writer.WriteShort(chr.dex)
        writer.WriteShort(chr.int)
        writer.WriteShort(chr.luk)
        writer.WriteShort(chr.curHp)
        writer.WriteShort(chr.maxHp)
        writer.WriteShort(chr.curMp)
        writer.WriteShort(chr.maxMp)
        writer.WriteShort(chr.ap)
        writer.WriteShort(chr.sp)
        writer.WriteInt(chr.exp)
        writer.WriteShort(chr.fame)
        writer.WriteInt(chr.gachaExp)
        writer.WriteInt(chr.mapId)
        writer.WriteByte(chr.spawnpoint)
        writer.WriteInt(0)
    End Sub

    Private Shared Sub addCharLook(ByVal writer As PacketWriter, ByVal chr As MapleCharacter, ByVal mega As Boolean)
        writer.WriteByte(chr.Gender)
        writer.WriteByte(chr.skincolor)
        writer.WriteInt(chr.face)
        writer.WriteBool(mega)
        writer.WriteInt(chr.hair)
        addCharEquips(writer, chr)
    End Sub

    Private Shared Sub addCharEquips(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        'TODO: MaskedEquip = Cash????
        If Not IsNothing(chr.Inventory) Then
            For Each item As MapleInventory.Items In chr.Inventory.Equiped
                'If item.position <> -11 Then
                writer.WriteByte(item.position * -1)
                writer.WriteInt(item.id)
                'End If
            Next
        End If
        writer.WriteByte(&HFF)

        'maskedEquips here?

        writer.WriteByte(&HFF)
        If Not IsNothing(chr.Inventory) Then
            For Each item As MapleInventory.Items In chr.Inventory.Equiped
                If item.position = -11 Then
                    writer.WriteInt(item.id)
                End If
            Next
        End If

        'petequips
        writer.WriteInt(0)
        writer.WriteInt(0)
        writer.WriteInt(0)
    End Sub

    Shared Function CheckCharNameResponse(ByVal name As String, ByVal cantCreate As Boolean) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(CHAR_NAME_RESPONSE)
        writer.WriteMapleString(name)
        writer.WriteBool(cantCreate)
        Return writer.ToArray
    End Function

    Shared Function addNewCharEntry(ByVal newCharacter As MapleCharacter) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(ADD_NEW_CHAR_ENTRY)
        writer.WriteByte(0)
        newCharacter.Inventory.SplitItems()
        addCharEntry(writer, newCharacter, False)
        Return writer.ToArray
    End Function

    Shared Function showAllCharacter(ByVal chars As Integer, ByVal unk As Integer) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(ALL_CHARLIST)
        writer.WriteBool(True)
        writer.WriteInt(chars)
        writer.WriteInt(unk)
        Return writer.ToArray
    End Function

    Shared Function showAllCharacterList(ByVal World As MapleWorld, ByVal chrsInWorld As List(Of MapleCharacter)) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(ALL_CHARLIST)
        writer.WriteBool(False)
        writer.WriteByte(World.id)
        writer.WriteByte(chrsInWorld.Count)
        For Each chr As MapleCharacter In chrsInWorld
            chr.Inventory.SplitItems()
            addCharEntry(writer, chr, True)
        Next
        Return writer.ToArray
    End Function

    Shared Function getServerIP(ByVal Ip As Byte(), ByVal port As Short, ByVal charId As Integer) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(SERVER_IP)
        writer.WriteShort(0)
        writer.WriteBytes(Ip)
        writer.WriteShort(port)
        writer.WriteInt(charId) 'todo, make a less exploitable way to handle this
        writer.WriteBytes(New Byte() {0, 0, 0, 0, 0})
        Return writer.ToArray
    End Function

    Shared Function WrongPic() As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(WRONG_PIC)
        writer.WriteByte(0)
        Return writer.ToArray
    End Function

    Shared Function getAfterLoginError(ByVal reason As Short) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(AFTER_LOGIN_ERROR)
        writer.WriteShort(reason)
        Return writer.ToArray
    End Function

    Shared Function getCharInfo(ByVal chr As MapleCharacter) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(WARP_TO_MAP)
        writer.WriteInt(chr.client.channel.id)
        writer.WriteBytes(New Byte() {1, 1, 0, 0})
        writer.WriteInt(Random())
        writer.WriteInt(Random())
        writer.WriteInt(Random())
        addCharacterInfo(writer, chr)
        writer.WriteLong(DateTime.Now.ToFileTimeUtc())
        Return writer.ToArray
    End Function

    Private Shared Sub addCharacterInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        chr.Inventory.SplitItems()
        writer.WriteLong(-1)
        writer.WriteByte(0)
        addCharStats(writer, chr)
        writer.WriteByte(&H64) 'TODO: buddylist
        writer.WriteBool(False) 'TODO: Linked characters
        addInventoryInfo(writer, chr)
        addSkillInfo(writer, chr)
        addQuestInfo(writer, chr)
        addRingInfo(writer, chr)
        writer.WriteBytes(New Byte() {&H0, &H0, &H0, &H0})
        addTeleportInfo(writer, chr)
        addMonsterBookInfo(writer, chr)
        writer.WriteShort(0)
        writer.WriteInt(0)
    End Sub

    Private Shared Sub addInventoryInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        writer.WriteInt(0) 'TODO: Mesos
        For Each slot In chr.slots
            writer.WriteByte(slot)
        Next
        writer.WriteBytes(New Byte() {&H0, &H40, &HE0, &HFD, &H3B, &H37, &H4F, &H1})
        

        'iteminfo packet for equipped sword : &HB, &H0, &H1, &HF0, &HDD, &H13, &H0, &H0, &H0, &H80, &H5, &H35, &HA7, &H43, &HBF, &H2, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H11, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H1, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H40, &HE0, &HFD, &H3B, &H37, &H4F, &H1, &HFF, &HFF, &HFF, &HFF
        For Each item As MapleInventory.Items In chr.Inventory.Equiped
            addItemInfo(writer, item)
        Next

        writer.WriteShort(0)

        'TODO: Cash equips

        writer.WriteShort(0)

        'TODO: add stats for weapons
        For Each item As MapleInventory.Items In chr.Inventory.Equips
            addItemInfo(writer, item)
        Next
        writer.WriteInt(0)

        For Each item As MapleInventory.Items In chr.Inventory.Use
            addItemInfo(writer, item)
        Next
        writer.WriteByte(0)
        For Each item As MapleInventory.Items In chr.Inventory.Setup
            addItemInfo(writer, item)
        Next
        writer.WriteByte(0)

        For Each item As MapleInventory.Items In chr.Inventory.Etc
            addItemInfo(writer, item)
        Next

        writer.WriteByte(0)

        'For Each item As MapleInventory.Items In chr.Inventory.Cash
        '    addItemInfo(writer, item, True, False)
        'Next
    End Sub

    Private Shared Sub addItemInfo(ByVal writer As PacketWriter, ByVal item As MapleInventory.Items)
        addItemInfo(writer, item, False, False)
    End Sub

    Private Shared Sub addItemInfo(ByVal writer As PacketWriter, ByVal item As MapleInventory.Items, ByVal cash As Boolean, ByVal zeroPosition As Boolean)
        If Not zeroPosition Then
            If item.type = MapleInventory.Types.Equipped Then
                writer.WriteShort(item.position * -1)
            Else
                writer.WriteByte(item.position)
            End If
        End If
        '1 = equiped, 2 = item , 3 = pet
        If item.type.Equals(MapleInventory.Types.Equipped) Then
            writer.WriteByte(1)
        Else
            writer.WriteByte(2)
        End If
        'writer.WriteByte(item.type)
        writer.WriteInt(item.id)
        
        writer.WriteBool(cash)
        'TODO: add cash/pet/ring handling
        addExpirationTime(writer, item.expiration)
        'TODO: add pet handling 
        
        If item.type <> MapleInventory.Types.Equipped Then
            writer.WriteShort(item.quantity)
            writer.WriteMapleString(item.owner)
            writer.WriteShort(item.flag)
            'TODO: rechargeable
            Return
        End If

        'TODO: Equips and stats
        With item.Stats
            writer.WriteByte(.slots)
            writer.WriteByte(0) 'level?!?!
            writer.WriteShort(.str)
            writer.WriteShort(.dex)
            writer.WriteShort(.int)
            writer.WriteShort(.luk)
            writer.WriteShort(.hp)
            writer.WriteShort(.mp)
            writer.WriteShort(.watk)
            writer.WriteShort(.matk)
            writer.WriteShort(.wdef)
            writer.WriteShort(.mdef)
            writer.WriteShort(.acc)
            writer.WriteShort(.avo)
            writer.WriteShort(.hands)
            writer.WriteShort(.speed)
            writer.WriteShort(.jump)
            writer.WriteMapleString(item.owner)
            writer.WriteShort(.flag)
            If cash Then
                For i = 0 To 9
                    writer.WriteByte(&H40)
                Next
            Else
                writer.WriteByte(0)
                writer.WriteByte(.itemlevel)
                writer.WriteShort(0)
                writer.WriteShort(.itemexp)
                writer.WriteInt(.vicious)
                writer.WriteLong(0)
            End If
        End With
        writer.WriteBytes(New Byte() {0, &H40, &HE0, &HFD, &H3B, &H37, &H4F, 1})
        writer.WriteInt(-1)
    End Sub

    Private Shared Sub addExpirationTime(ByVal writer As PacketWriter, ByVal expiration As Long)
        addExpirationTime(writer, expiration, True)
    End Sub

    Private Shared Sub addExpirationTime(ByVal writer As PacketWriter, ByVal expiration As Long, ByVal addzero As Boolean)
        writer.WriteBytes(New Byte() {&H0, &H80, &H5, &H35, &HA7, &H43, &HBF, &H2})
        'If addzero Then
        '    writer.WriteByte(0)
        'End If
        'If expiration < 1 Then
        '    writer.WriteInt(400967355)
        '    writer.WriteByte(2)
        'Else
        '    'TODO: add expiration
        'End If
    End Sub

    Private Shared Sub addSkillInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        writer.WriteByte(0)
        'Some skill : &H1, &H0, &HC, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H80, &H5, &HBB, &H46, &HE6, &H17, &H2

        'TODO: add skills
        writer.WriteShort(0)
        'TODO: add cooldown
        writer.WriteShort(0)

    End Sub

    Private Shared Sub addRingInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        writer.WriteShort(0)
        writer.WriteShort(0)
        writer.WriteShort(0)
    End Sub

    Private Shared Sub addTeleportInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        'TODO: add tele rocks
        For i = 1 To 5
            writer.WriteInt(999999999)
        Next
        For i = 1 To 10
            writer.WriteInt(999999999)
        Next
    End Sub

    Private Shared Sub addMonsterBookInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        'TODO add MonsterBookInfo
        writer.WriteInt(0)
        writer.WriteByte(0)
        writer.WriteShort(0)
    End Sub

    Private Shared Sub addQuestInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        'TODO: add quests
        writer.WriteShort(0)
        writer.WriteShort(0)

    End Sub

    Shared Function SpawnPlayerOnMap(ByVal c As MapleClient) As Byte()
        Dim writer As New PacketWriter
        With writer
            .WriteShort(SPAWN_PLAYER)
            .WriteInt(c.Player.id)
            .WriteByte(c.Player.level)
            .WriteMapleString(c.Player.Name)

            'TODO: guilds
            .WriteMapleString("")
            For i = 1 To 6
                .WriteByte(0)
            Next
            .WriteInt(0)
            .WriteShort(0)
            .WriteByte(&HFC)
            .WriteBool(True)

            'TODO: Morph/buffs
            .WriteInt(0)
            .WriteInt(CInt((0 >> 32) And &HFFFFFFFFL))
            .WriteInt(CInt(0 And &HFFFFFFFFL))

            Dim CHAR_MAGIC_SPAWN = Random()
            For i = 1 To 6
                .WriteByte(0)
            Next
            .WriteInt(CHAR_MAGIC_SPAWN)
            For i = 1 To 11
                .WriteByte(0)
            Next
            .WriteInt(CHAR_MAGIC_SPAWN)
            For i = 1 To 11
                .WriteByte(0)
            Next
            .WriteInt(CHAR_MAGIC_SPAWN)
            .WriteShort(0)
            .WriteByte(0)

            'TODO: add monster riding
            .WriteLong(0)

            .WriteInt(CHAR_MAGIC_SPAWN)
            For i = 1 To 9
                .WriteByte(0)
            Next
            .WriteInt(CHAR_MAGIC_SPAWN)
            .WriteShort(0)
            .WriteInt(0)
            For i = 1 To 10
                .WriteByte(0)
            Next
            .WriteInt(CHAR_MAGIC_SPAWN)
            For i = 1 To 13
                .WriteByte(0)
            Next
            .WriteInt(CHAR_MAGIC_SPAWN)
            .WriteShort(0)
            .WriteByte(0)
            .WriteShort(c.Player.job)
            addCharLook(writer, c.Player, False)
            .WriteInt(0) 'hell idk, look to odinms shizzl
            .WriteInt(0) 'itemeffect
            .WriteInt(0) 'chair

            .WriteShort(c.Player.Position.x)
            .WriteShort(c.Player.Position.y)
            .WriteByte(c.Player.stance)
            .WriteShort(0) 'fh

            .WriteByte(0)

            'TODO: add pets
            .WriteByte(0)
            .WriteInt(1)
            .WriteLong(0)

            'TODO: minigames / playershops
            .WriteByte(0)

            'TODO: chalkboard
            .WriteByte(0)

            'TODO: rings
            addRingLook(writer, c.Player, True)
            addRingLook(writer, c.Player, False)
            addMarriageRingLook(writer, c.Player)

            .WriteByte(0)
            .WriteByte(0)
            .WriteByte(0)
            .WriteByte(0) 'TODO: player teams
        End With
        Return writer.ToArray
    End Function

    Private Shared Sub addRingLook(ByVal writer As PacketWriter, ByVal Player As MapleCharacter, ByVal crush As Boolean)
        writer.WriteByte(0)
    End Sub

    Private Shared Sub addMarriageRingLook(ByVal writer As PacketWriter, ByVal Player As MapleCharacter)
        writer.WriteByte(0)
    End Sub

    Shared Function movePlayer(ByVal cid As Integer, ByVal moves As List(Of LifeMovement)) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(MOVE_PLAYER)
        writer.WriteInt(cid)
        writer.WriteInt(0)
        serializeMovementList(writer, moves)
        Return writer.ToArray
    End Function

    Private Shared Sub serializeMovementList(ByVal writer As PacketWriter, ByVal moves As List(Of LifeMovement))
        writer.WriteByte(moves.Count)
        For Each move As LifeMovement In moves
            move.serialize(writer)
        Next
    End Sub

    Shared Function RemovePlayerFromMap(ByVal cid As Integer) As Byte()
        Dim writer As New PacketWriter
        writer.WriteShort(REMOVE_PLAYER_FROM_MAP)
        writer.WriteInt(cid)
        Return writer.ToArray
    End Function

End Class