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

Public Enum SendHeaders As UShort
    HandShake = &HE

    LOGIN_STATUS = &H0
    SERVERSTATUS = &H3
    ALL_CHARLIST = &H8
    AFTER_LOGIN_ERROR = &H9
    SERVERLIST = &HA
    CHARLIST = &HB
    SERVER_IP = &HC
    CHAR_NAME_RESPONSE = &HD
    ADD_NEW_CHAR_ENTRY = &HE
    CHANGE_CHANNEL = &H10
    PING = &H11
    SELECT_WORLD = &H1A
    SEND_RECOMMENDED = &H1B
    WRONG_PIC = &H1C
    WARP_TO_MAP = &H7D
    SPAWN_PLAYER = &HA0
    REMOVE_PLAYER_FROM_MAP = &HA1
    CHATTEXT = &HA2
    MOVE_PLAYER = &HB9
    FACIAL_EXPRESSION = &HC1
    SPAWN_MONSTER = &HEC
    KILL_MONSTER = &HED
    SPAWN_MONSTER_CONTROL = &HEE
    MOVE_MONSTER = &HEF
    MOVE_MONSTER_RESPONSE = &HF0
    SPAWN_NPC = &H101
End Enum