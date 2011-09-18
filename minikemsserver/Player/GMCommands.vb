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
                        c.Player.warp(mapId, 0)
                    End If
                    Return
                End If
        End Select
    End Sub
End Class