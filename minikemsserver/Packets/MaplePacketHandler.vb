Imports MapleLib.PacketLib
Imports MinikeMSServer.SendHeaders

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

End Class