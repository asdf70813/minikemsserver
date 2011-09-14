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

Imports System.Net

Public Class Settings
    Public Shared ReadOnly Port As UShort = 8484
    Public Shared ReadOnly Ip As IPAddress = IPAddress.Parse("127.0.0.1")
    Public Shared ReadOnly mapleVersion As Short = 83

    '{port,channelcount,name,flag,eventmessage}
    Public Shared ReadOnly WorldSettings As Object() = New Object() {
        New Object() {7575, 19, "World1", 0, "World one"},
        New Object() {7576, 13, "World2", 0, "Testing world"}
    }

    Public Shared ReadOnly ConnectionString As String = "Database=VBMS;Data Source=127.0.0.1;User Id=root;Password=ZFzf17"
End Class
