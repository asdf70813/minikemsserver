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

Class PongHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Sub New(ByVal c As MapleClient)
        c.ReceivedPong = True
        c.timesWithoutPong = 0
        Dim pingedMS As Integer = Math.Floor((DateTime.Now.ToFileTimeUtc - c.lastPingSend) / 10000)
        If c.PingBuffer(0) = 0 Then
            c.PingBuffer(0) = pingedMS
        ElseIf c.PingBuffer(1) = 0 Then
            c.PingBuffer(1) = pingedMS
        ElseIf c.PingBuffer(2) = 0 Then
            c.PingBuffer(2) = pingedMS
        ElseIf c.PingBuffer(3) = 0 Then
            c.PingBuffer(3) = pingedMS
        Else
            Dim avgPing As Integer = Math.Floor((c.PingBuffer(0) + c.PingBuffer(1) + c.PingBuffer(2) + c.PingBuffer(3)) / 4)
            If avgPing > Settings.maxAvgPing Then
                Console.WriteLine("Disconnect client [{0}] (Reason: High Ping: limit {1}, current {2})", c.Host, Settings.maxAvgPing, avgPing)
                c.Disconnect()
            Else
                c.PingBuffer = New Integer() {0, 0, 0, 0}
            End If
        End If
        Me.Dispose()
    End Sub

End Class
