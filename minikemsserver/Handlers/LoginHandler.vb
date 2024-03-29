﻿'    This file is part of MinikeMSServer.

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
Imports MySql.Data.MySqlClient
Imports MinikeMSServer.LoginNotices
Imports MinikeMSServer.Functions
Imports System.Security.Cryptography

Public Class LoginHandler
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose

    End Sub
#End Region

    Private _packetReader As PacketReader

    Sub New(ByVal packetReader As PacketReader, ByVal c As MapleClient)
        _packetReader = packetReader
        Dim accountName As String = packetReader.ReadMapleString
        Dim Password As String = packetReader.ReadMapleString
        c.loginattempt += 1
        Dim LoginOK As Integer = CheckLogin(accountName, Password, packetReader, c)
        Select Case LoginOK
            Case login_ok
                c.AccountName = accountName
                Dim packet As Byte() = MaplePacketHandler.LoginSucces(c)
                c.SendPacket(packet)
                Dim loggedinCon As New MySQLCon(Settings.ConnectionString)
                loggedinCon.ExecuteQuery("UPDATE tbl_accounts SET loggedin='1' WHERE account='" & accountName & "' AND id='" & c.AccountID & "'")
                loggedinCon.Dispose()
                c.LoggedIn = 1
            Case Else
                Dim packet As Byte() = MaplePacketHandler.LoginFailed(LoginOK)
                c.SendPacket(packet)
        End Select
        Me.Dispose()
    End Sub

    Private Function CheckLogin(ByVal accountName As String, ByVal Password As String, ByVal packetReader As PacketReader, ByVal c As MapleClient) As Integer

        Dim AccountCon As New MySQLCon(Settings.ConnectionString)
        Dim reader As MySqlDataReader = AccountCon.ReadQuery("SELECT * FROM tbl_accounts WHERE account='" & accountName & "'")
        Dim id As Integer = Nothing
        While reader.Read()
            id = 0
            If accountName.ToLower.Equals(reader.GetString("account").ToLower) Then
                id = reader.GetInt32("id")
                If reader.GetString("banned").ToLower.StartsWith("true") Then
                    Return blocked
                End If
                If reader.GetInt16("loggedin") > 0 Then
                    Return already_logged
                End If
                Dim hash As HashAlgorithm = New SHA512Managed()
                Dim hashedPW As String = ByteArrayToStr(hash.ComputeHash(hStrToByteArray(Password)))
                If reader.GetString("password").Equals(hashedPW) Then
                    c.AccountID = id
                    c.pic = reader.GetString("PIC").Replace(" ", "")
                    c.hasPic = Not c.pic.Equals("")
                    Return login_ok
                Else
                    Return wrong_pw
                End If
            End If
        End While
        AccountCon.Dispose()
        If IsNothing(id) Then
            Return not_registered
        End If
        If id = 0 Then
            Return not_registered
        End If
        Return system_error_1
    End Function
End Class

Public Enum LoginNotices As Integer
    login_ok = 0
    blocked = 3 'ID deleted or blocked
    wrong_pw = 4 'Incorrect password
    not_registered = 5 'Not a registered id
    system_error_1 = 6 'System error
    already_logged = 7 'Already logged in
    system_error_2 = 8 'System error
    system_error_3 = 9 'System error
    server_limit = 10 ' Cannot process so many connections
    only_older = 11 'Only users older than 20 can use this channel
    unable_to_log_gm = 13 'Unable to log on as master at this ip
    wrong_gateway_1 = 14 'Wrong gateway or personal info
    processing_request = 15 'Processing request
    email_verify_1 = 16 'Please verify your account through email...
    wrong_gateway_2 = 17 'Wrong gateway or personal info
    email_verify_2 = 21 'Please verify your account through email...
    license = 23 'License agreement
    eublock_notice = 25 'Maple Europe notice
    trial = 27 'Some weird full client notice, probably for trial versions
End Enum