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


'http://dev.mysql.com/downloads/connector/net/

Imports MySql.Data.MySqlClient
Public Class MySQLCon
#Region "IDisposable"
    Implements IDisposable
    Private disposedValue As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If Not IsNothing(connection) Then
                    connection.Close()
                End If
            End If
        End If
        Me.disposedValue = True
    End Sub
#End Region
    Private connection As MySqlConnection = Nothing

    Sub New(ByVal connStr As String)
        Try
            connection = New MySqlConnection(connStr)
            connection.Open()
        Catch ex As Exception
            Console.WriteLine("Theres something wrong with mysql: {0}", ex.ToString)
        End Try
    End Sub

    Public Sub ExecuteQuery(ByVal querystring As String)
        Try
            If IsNothing(connection) Then
                Console.WriteLine("Trying to execute a query without connection")
                Me.Dispose()
                Return
            End If
            Dim query As New MySqlCommand(querystring, connection)
            query.ExecuteNonQuery()
        Catch ex As Exception
            Console.WriteLine(querystring)
            Console.WriteLine("Something went wrong while executing a query {0}", ex.ToString)
        End Try
    End Sub

    Public Function ReadQuery(ByVal querystring As String) As MySqlDataReader
        Try
            If IsNothing(connection) Then
                Console.WriteLine("Trying to execute a readquery without connection")
                Me.Dispose()
                Return Nothing
            End If
            Dim query As New MySqlCommand(querystring, connection)
            Return query.ExecuteReader
        Catch ex As Exception
            Console.WriteLine(querystring)
            Console.WriteLine("Something went wrong while executing a readquery {0}", ex.ToString)
        End Try
        Return Nothing
    End Function
End Class
