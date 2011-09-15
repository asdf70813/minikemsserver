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
        writer.WriteByte(chr.gender)
        writer.WriteByte(chr.skincolor)
        writer.WriteInt(chr.face)
        writer.WriteInt(chr.hair)
        For i As Integer = 1 To 3 'TODO : pets
            writer.WriteLong(0)
        Next
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
            For Each item As MapleInventory.Items In chr.Inventory.ItemList
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
            For Each item As MapleInventory.Items In chr.Inventory.ItemList
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

    Shared Function getAfterLoginError(ByVal reason As Short)
        Dim writer As New PacketWriter
        writer.WriteShort(AFTER_LOGIN_ERROR)
        writer.WriteShort(reason)
        Return writer.ToArray
    End Function

    Shared Function getCharInfo(ByVal chr As MapleCharacter)
        Dim writer As New PacketWriter
        writer.WriteShort(WARP_TO_MAP)
        writer.WriteInt(chr.client.channel.id - 1)
        writer.WriteBytes(New Byte() {1, 1, 0, 0})
        For i = 1 To 3
            writer.WriteInt(Random())
        Next
        addCharacterInfo(writer, chr)
        writer.WriteLong(GetTickCount())
        Return writer.ToArray
    End Function

    Private Shared Sub addCharacterInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        writer.WriteLong(-1)
        writer.WriteByte(0)
        addCharStats(writer, chr)
        writer.WriteByte(0) 'TODO: buddylist
        writer.WriteByte(0) 'TODO: Linked characters
        writer.WriteInt(0) 'TODO: Mesos
        addInventoryInfo(writer, chr)
        addSkillInfo(writer, chr)
        writer.WriteShort(0)
        addRingInfo(writer, chr)
        addTeleportInfo(writer, chr)
        addMonsterBookInfo(writer, chr)
        writer.WriteShort(0)
        writer.WriteInt(0)
    End Sub

    Private Shared Sub addInventoryInfo(ByVal writer As PacketWriter, ByVal chr As MapleCharacter)
        For i = 1 To 5
            writer.WriteByte(1) 'TODO: Inventory size left
        Next
    End Sub

End Class