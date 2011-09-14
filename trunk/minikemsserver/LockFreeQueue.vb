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

Imports System.Threading

Public Class LockFreeQueue(Of T As Class)
    Private Class SingleLinkNode
        Public [Next] As SingleLinkNode
        Public Item As T
    End Class

    Private mHead As SingleLinkNode = Nothing
    Private mTail As SingleLinkNode = Nothing

    Public Sub New()
        mHead = New SingleLinkNode()
        mTail = mHead
    End Sub

    Private Shared Function CompareAndExchange(ByRef pLocation As SingleLinkNode, ByVal pComparand As SingleLinkNode, ByVal pNewValue As SingleLinkNode) As Boolean
        Return DirectCast(pComparand, Object) Is DirectCast(Interlocked.CompareExchange(Of SingleLinkNode)(pLocation, pNewValue, pComparand), Object)
    End Function

    Public ReadOnly Property [Next]() As T
        Get
            Return If(mHead.[Next] Is Nothing, Nothing, mHead.[Next].Item)
        End Get
    End Property
    Public Sub Enqueue(ByVal pItem As T)
        Dim oldTail As SingleLinkNode = Nothing
        Dim oldTailNext As SingleLinkNode

        Dim newNode As New SingleLinkNode()
        newNode.Item = pItem

        Dim newNodeWasAdded As Boolean = False
        While Not newNodeWasAdded
            oldTail = mTail
            oldTailNext = oldTail.[Next]

            If mTail Is oldTail Then
                If oldTailNext Is Nothing Then
                    newNodeWasAdded = CompareAndExchange(mTail.[Next], Nothing, newNode)
                Else
                    CompareAndExchange(mTail, oldTail, oldTailNext)
                End If
            End If
        End While

        CompareAndExchange(mTail, oldTail, newNode)
    End Sub

    Public Function Dequeue(ByRef pItem As T) As Boolean
        pItem = Nothing
        Dim oldHead As SingleLinkNode = Nothing

        Dim haveAdvancedHead As Boolean = False
        While Not haveAdvancedHead

            oldHead = mHead
            Dim oldTail As SingleLinkNode = mTail
            Dim oldHeadNext As SingleLinkNode = oldHead.[Next]

            If oldHead Is mHead Then
                If oldHead Is oldTail Then
                    If oldHeadNext Is Nothing Then
                        Return False
                    End If
                    CompareAndExchange(mTail, oldTail, oldHeadNext)
                Else

                    pItem = oldHeadNext.Item
                    haveAdvancedHead = CompareAndExchange(mHead, oldHead, oldHeadNext)
                End If
            End If
        End While
        Return True
    End Function

    Public Function Dequeue() As T
        Dim result As T
        Dequeue(result)
        Return result
    End Function
End Class