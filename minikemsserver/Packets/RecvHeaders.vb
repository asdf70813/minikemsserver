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

Public Enum RecvHeaders As UShort
    LOGIN_PASSWORD = &H1
    SERVERLIST_REREQUEST = &H4
    CHARLIST_REQUEST = &H5
    SERVERSTATUS_REQUEST = &H6
    SERVERLIST_REQUEST = &HB
    VIEW_ALL_CHAR = &HD
    CHECK_CHAR_NAME = &H15
    CREATE_CHAR = &H16
End Enum