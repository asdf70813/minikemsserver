Public Class GMCommands
    Public Shared Sub handleCommand(ByVal text As String, ByVal c As MapleClient)
        handleCommand(text.Split(" "), c)
    End Sub

    Public Shared Sub handleCommand(ByVal text As String(), ByVal c As MapleClient)
        text(0) = text(0).Replace("#", "")
        Select Case text(0).ToLower
            Case "warp"
                If text.Length = 2 Then
                    If IsNumeric(text(1)) Then
                        Dim mapId As Integer = CInt(text(1))
                        c.Player.warp(c.Player, mapId, 0)
                    End If
                    Return
                End If
            Case "disconnect", "dc"
                If text.Length = 2 Then
                    Dim target As MapleCharacter = c.world.getPlayerByname(text(1))
                    target.disconnect(True)
                    Return
                End If
            Case "makegm"
                If text.Length = 2 Then
                    Dim target As MapleCharacter = c.world.getPlayerByname(text(1))
                    target.IsGM = True
                    Return
                End If
            Case "hide"
                If text.Length = 1 Then
                    c.Player.Map.RemovePlayer(c.Player)
                    c.Player.hidden = Not c.Player.hidden
                    c.Player.Map.AddPlayer(c.Player)
                    Return
                ElseIf text.Length = 2 Then
                    Dim target As MapleCharacter = c.world.getPlayerByname(text(1))
                    target.Map.RemovePlayer(target)
                    target.hidden = Not target.hidden
                    target.Map.AddPlayer(target)
                    Return
                End If
        End Select
    End Sub
End Class