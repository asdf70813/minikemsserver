Imports System.Net.Sockets
Imports System.Net

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

Public Class MapleWorld
    Public Flag As Byte = 0
    Public Name As String = ""
    Public eventMessage As String = ""
    Public ChannelCount As Byte = 0
    Public id As Byte = 0
    Public port As UShort
    Public WorldServer As Socket
    Public listen As Integer
    Public Clients As New List(Of MapleClient)
    Public Channels As New List(Of MapleChannel)

    Sub New(ByVal _ChannelCount As Byte, ByVal _id As Byte, ByVal _port As UShort)
        port = _port
        id = _id
        ChannelCount = _ChannelCount
        Dim i As Integer = 1
        While i <= ChannelCount
            Dim channel As New MapleChannel
            channel.id = i
            Channels.Add(channel)
            i += 1
        End While
        WorldServer = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        WorldServer.Bind(New IPEndPoint(IPAddress.Any, port))
        Console.WriteLine("WorldServer[{1}] binded to port {0}", port, id)
        WorldServer.Listen(listen)
        BeginLoginListenerAccept(Nothing)
    End Sub

    Private Sub BeginLoginListenerAccept(ByVal pArgs As SocketAsyncEventArgs)
        If pArgs Is Nothing Then
            pArgs = New SocketAsyncEventArgs()
            AddHandler pArgs.Completed, AddressOf EndLoginListenerAccept
        End If
        pArgs.AcceptSocket = Nothing
        If Not WorldServer.AcceptAsync(pArgs) Then
            EndLoginListenerAccept(Nothing, pArgs)
        End If
    End Sub

    Private Sub EndLoginListenerAccept(ByVal sender As Object, ByVal pArguments As SocketAsyncEventArgs)
        Try
            If pArguments.SocketError = SocketError.Success Then
                Dim ip As IPEndPoint = DirectCast(pArguments.RemoteEndPoint, IPEndPoint)
                Dim client As MapleClient = Server.getClientByIp(ip.Address)
                Clients.Add(client)
                Server.ClientDisconnected(client)
                BeginLoginListenerAccept(pArguments)
            ElseIf pArguments.SocketError <> SocketError.OperationAborted Then
                Console.WriteLine("[Server] LoginListener Error: {0}", pArguments.SocketError)
            End If
        Catch generatedExceptionName As ObjectDisposedException
        Catch exc As Exception
            Console.WriteLine("[Server] LoginListener Exception: {0}", exc)
        End Try
    End Sub

    Friend Sub ClientDisconnected(ByVal pClient As MapleClient)
        SyncLock Clients
            Clients.Remove(pClient)
        End SyncLock
    End Sub
End Class