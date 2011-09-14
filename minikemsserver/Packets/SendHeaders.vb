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

Public Enum SendHeaders As UShort
    HandShake = &HE

    LOGIN_STATUS = &H0
    SERVERSTATUS = &H3
    ALL_CHARLIST = &H8
    SERVERLIST = &HA
    CHARLIST = &HB
    CHAR_NAME_RESPONSE = &HD
    ADD_NEW_CHAR_ENTRY = &HE
    SELECT_WORLD = &H1A
    SEND_RECOMMENDED = &H1B
End Enum
