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
'    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports MapleLib.PacketLib
Imports MapleLib.MapleCryptoLib
Imports MinikeMSServer.Functions

Public NotInheritable Class MapleClient

    Private Const MAX_RECEIVE_BUFFER As Integer = 16384

    Private mSocket As Socket = Nothing
    Private mHost As String = Nothing
    Private mDisconnected As Integer = 0

    Private mReceiveBuffer As Byte() = Nothing
    Private mReceiveStart As Integer = 0
    Private mReceiveLength As Integer = 0
    Private mReceiveLast As DateTime = DateTime.Now
    Private mSendSegments As New LockFreeQueue(Of ByteArraySegment)()
    Private mSending As Integer = 0

    Private _RIV As MapleCrypto = Nothing
    Private _SIV As MapleCrypto = Nothing
    Private _type = SessionType.SERVER_TO_CLIENT
    Private mReceivingPacketLength As UShort = 0
    Private rPacket_Handler As New RecvPacketHandler

    Public loginattempt As Integer = 0

    Public AccountName As String = ""
    Public AccountID As Integer = 0

    Public Sub New(ByVal pSocket As Socket, ByVal ReceiveMapleCrypto As MapleCrypto, ByVal SendMapleCrypto As MapleCrypto)
        mSocket = pSocket
        _RIV = ReceiveMapleCrypto
        _SIV = SendMapleCrypto
        mReceiveBuffer = New Byte(MAX_RECEIVE_BUFFER - 1) {}
        mHost = DirectCast(mSocket.RemoteEndPoint, IPEndPoint).Address.ToString()
        Console.WriteLine("[{0}] Connected", mHost)
        WaitForData()
    End Sub

    Public ReadOnly Property Host() As String
        Get
            Return mHost
        End Get
    End Property

    Public Sub WaitForData()
        If Not IsNothing(mSocket) Then
            Dim SI As New SocketInfo(mSocket, 4)
            WaitForData(SI)
        End If
    End Sub

    Public Sub WaitForData(ByVal SI As SocketInfo)
        Try
            mSocket.BeginReceive(SI.DataBuffer, SI.Index, SI.DataBuffer.Length - SI.Index, SocketFlags.None, New AsyncCallback(AddressOf OnDataReceived), SI)
        Catch ex As Exception
            Console.WriteLine("Error Client.WaitForData {0}", ex.ToString)
        End Try
    End Sub

    Public Sub OnDataReceived(ByVal iar As IAsyncResult)
        Dim socketInfo__1 As SocketInfo = DirectCast(iar.AsyncState, SocketInfo)
        Try
            Dim received As Integer = socketInfo__1.Socket.EndReceive(iar)
            If received = 0 Then
                Me.Disconnect()
                Return
            End If

            socketInfo__1.Index += received

            If socketInfo__1.Index = socketInfo__1.DataBuffer.Length Then
                Select Case socketInfo__1.State
                    Case SocketInfo.StateEnum.Header
                        If socketInfo__1.NoEncryption Then
                            WaitForData(socketInfo__1)
                        Else
                            Dim headerReader As New PacketReader(socketInfo__1.DataBuffer)
                            Dim packetHeaderB As Byte() = headerReader.ToArray()
                            Dim packetHeader As Integer = headerReader.ReadInt()
                            Dim packetLength As Short = CShort(MapleCrypto.getPacketLength(packetHeader))
                            If Not _RIV.checkPacketToServer(BitConverter.GetBytes(packetHeader)) Then
                                Console.WriteLine("[Error] Packet check failed. Disconnecting client.")
                                Me.Disconnect()
                            End If
                            socketInfo__1.State = SocketInfo.StateEnum.Content
                            socketInfo__1.DataBuffer = New Byte(packetLength - 1) {}
                            socketInfo__1.Index = 0
                            WaitForData(socketInfo__1)
                        End If
                        Exit Select
                    Case SocketInfo.StateEnum.Content
                        Dim data As Byte() = socketInfo__1.DataBuffer
                        If socketInfo__1.NoEncryption Then
                            WaitForData()
                        Else
                            _RIV.crypt(data)
                            MapleCustomEncryption.Decrypt(data)
                            rPacket_Handler.HandlePacket(New PacketReader(data), Me)
                            Console.WriteLine(ByteArrayToStr(data))
                            WaitForData()
                        End If
                        Exit Select
                End Select
            Else
                Console.WriteLine("[Warning] Not enough data")
                WaitForData(socketInfo__1)
            End If
        Catch generatedExceptionName As ObjectDisposedException
            Console.WriteLine("[Error] Session.OnDataReceived: Socket has been closed")
        Catch se As SocketException
            If se.ErrorCode <> 10054 Then
                Console.WriteLine("[Error] Session.OnDataReceived: " & se.ToString)
            Else
                Console.WriteLine("[Error] Session.OnDataReceived: Socket has been closed")
                Me.Disconnect()
            End If
        Catch e As Exception
            Console.WriteLine("[Error] Session.OnDataReceived: " & e.ToString)
        End Try

    End Sub

    Public Sub Disconnect()
        If Interlocked.CompareExchange(mDisconnected, 1, 0) = 0 Then
            mSocket.Shutdown(SocketShutdown.Both)
            mSocket.Close()
            Console.WriteLine("[{0}] Disconnected", Host)
            Server.ClientDisconnected(Me)
        End If
    End Sub

    Friend Sub SendHandshake(ByVal pBuild As UShort, ByVal pReceiveIV As Byte(), ByVal pSendIV As Byte())
        Dim writer As New PacketWriter
        writer.WriteShort(SendHeaders.HandShake)
        writer.WriteShort(pBuild)
        writer.WriteShort(1)
        writer.WriteByte(49)
        writer.WriteBytes(pReceiveIV)
        writer.WriteBytes(pSendIV)
        writer.WriteByte(8)
        Send(writer.ToArray)
    End Sub

    Private Sub Send(ByVal pBuffer As Byte())
        If mDisconnected <> 0 Then
            Return
        End If
        mSendSegments.Enqueue(New ByteArraySegment(pBuffer))
        If Interlocked.CompareExchange(mSending, 1, 0) = 0 Then
            BeginSend()
        End If
    End Sub


    Private Sub BeginSend()
        Dim args As New SocketAsyncEventArgs()
        AddHandler args.Completed, AddressOf EndSend
        Dim segment As ByteArraySegment = mSendSegments.[Next]
        args.SetBuffer(segment.Buffer, segment.Start, segment.Length)
        Try
            If Not mSocket.SendAsync(args) Then
                EndSend(Nothing, args)
            End If
        Catch generatedExceptionName As ObjectDisposedException
        End Try
    End Sub
    Private Sub EndSend(ByVal sender As Object, ByVal pArguments As SocketAsyncEventArgs)
        If mDisconnected <> 0 Then
            Return
        End If
        If pArguments.BytesTransferred <= 0 Then
            If pArguments.SocketError <> SocketError.Success Then
                Console.WriteLine("[{0}] Send Error: {1}", Host, pArguments.SocketError)
            End If
            Disconnect()
            Return
        End If
        If mSendSegments.[Next].Advance(pArguments.BytesTransferred) Then
            mSendSegments.Dequeue()
        End If
        If mSendSegments.[Next] IsNot Nothing Then
            BeginSend()
        Else
            mSending = 0
        End If
    End Sub

    ''' <summary>
    ''' Encrypts the packet then send it to the client.
    ''' </summary>
    ''' <param name="packet">The PacketWrtier object to be sent.</param>
    Public Sub SendPacket(ByVal packet As PacketWriter)
        SendPacket(packet.ToArray())
    End Sub

    ''' <summary>
    ''' Encrypts the packet then send it to the client.
    ''' </summary>
    ''' <param name="input">The byte array to be sent.</param>
    Public Sub SendPacket(ByVal input As Byte())
        Try
            Dim cryptData As Byte() = input
            Dim sendData As Byte() = New Byte(cryptData.Length + 3) {}
            Dim header As Byte() = _SIV.getHeaderToClient(cryptData.Length)

            MapleCustomEncryption.Encrypt(cryptData)
            _SIV.crypt(cryptData)

            System.Buffer.BlockCopy(header, 0, sendData, 0, 4)
            System.Buffer.BlockCopy(cryptData, 0, sendData, 4, cryptData.Length)
            SendRawPacket(sendData)

        Catch ex As Exception
            Console.WriteLine(ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Sends a raw buffer to the client.
    ''' </summary>
    ''' <param name="buffer">The buffer to be sent.</param>
    Public Sub SendRawPacket(ByVal buffer As Byte())
        Try
            mSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, Function(ar) mSocket.EndSend(ar), Nothing)
        Catch generatedExceptionName As SocketException
            Me.Disconnect()
        End Try
    End Sub
End Class