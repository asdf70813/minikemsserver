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

Imports System.Net.Sockets
Imports System.Net
Imports MinikeMSServer.Functions
Imports MapleLib.MapleCryptoLib
Imports System.IO
Module Server
    Private LoginServer As Socket
    Private Clients As New List(Of MapleClient)
    Public Worlds As New List(Of MapleWorld)
    Public test As Integer

    Sub Main()
        Try
            'Resseting the loggedin values for accounts
            Console.WriteLine("This file is part of MinikeMSServer." & Environment.NewLine & "MinikeMSServer is free software: you can redistribute it and/or modify" & Environment.NewLine & "it under the terms of the GNU General Public License as published by" & Environment.NewLine & "the Free Software Foundation, either version 3 of the License, or" & Environment.NewLine & "(at your option) any later version." & Environment.NewLine & Environment.NewLine & "MinikeMSServer is distributed in the hope that it will be useful," & Environment.NewLine & "but WITHOUT ANY WARRANTY; without even the implied warranty of" & Environment.NewLine & "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the" & Environment.NewLine & "GNU General Public License for more details." & Environment.NewLine & Environment.NewLine & "You should have received a copy of the GNU General Public License" & Environment.NewLine & "along with MinikeMSServer.  If not, see <http://www.gnu.org/licenses/>.")
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Please use 'Stop' to stop the server, this to prevent rollbacks")
            Console.ForegroundColor = Settings.textColor
            If Not Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory & "\wz") Then
                Console.WriteLine("[NUB WARNING!] Wz directory not found, niblet")
            End If
            Dim loggedinCon As New MySQLCon(Settings.ConnectionString)
            loggedinCon.ExecuteQuery("UPDATE tbl_accounts SET loggedin='0' WHERE loggedin='1'")
            loggedinCon.Dispose()
            'Starting the login server
            LoginServer = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            LoginServer.Bind(New IPEndPoint(IPAddress.Parse(Settings.IP), Settings.Port))
            LoginServer.Listen(test)
            BeginLoginListenerAccept(Nothing)
            Console.WriteLine("LoginSever binded to port {0}", Settings.Port)
            Dim i As Integer = 0
            For Each WorldInfo In Settings.WorldSettings
                Dim world As New MapleWorld(WorldInfo(1), i, WorldInfo(0), WorldInfo(5)) 'Info for channels must be in the sub new
                world.Name = WorldInfo(2)
                world.Flag = WorldInfo(3)
                world.eventMessage = WorldInfo(4)
                Worlds.Add(world)
                i += 1
            Next
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
        End Try
        Dim line As String
pause:  line = Console.ReadLine()
        line = line.ToLower
        If Not line.Contains("stop") And Not line.Contains("exit") Then
            GoTo pause
        End If
        For Each world In Worlds
            For Each c In world.Clients
                c.Player.SaveToDB(False, c)
            Next
        Next
        System.Threading.Thread.Sleep(1000)
        LoginServer.Dispose()
    End Sub

    Private Sub BeginLoginListenerAccept(ByVal pArgs As SocketAsyncEventArgs)
        If pArgs Is Nothing Then
            pArgs = New SocketAsyncEventArgs()
            AddHandler pArgs.Completed, AddressOf EndLoginListenerAccept
        End If
        pArgs.AcceptSocket = Nothing
        If Not LoginServer.AcceptAsync(pArgs) Then
            EndLoginListenerAccept(Nothing, pArgs)
        End If
    End Sub

    Private Sub EndLoginListenerAccept(ByVal sender As Object, ByVal pArguments As SocketAsyncEventArgs)
        Try
            If pArguments.SocketError = SocketError.Success Then

                Dim receiveIV As Byte() = New Byte(3) {}
                receiveIV(0) = RandomByte()
                receiveIV(1) = RandomByte()
                receiveIV(2) = RandomByte()
                receiveIV(3) = RandomByte()
                Dim sendIV As Byte() = New Byte(3) {}
                sendIV(0) = RandomByte()
                sendIV(1) = RandomByte()
                sendIV(2) = RandomByte()
                sendIV(3) = RandomByte()
                Dim _RIV As New MapleCrypto(receiveIV, Settings.mapleVersion)
                Dim _SIV As New MapleCrypto(sendIV, Settings.mapleVersion)
                Dim client As New MapleClient(pArguments.AcceptSocket)
                client._RIV = _RIV
                client._SIV = _SIV
                client.SendHandshake(Settings.mapleVersion, receiveIV, sendIV)
                Clients.Add(client)
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

End Module