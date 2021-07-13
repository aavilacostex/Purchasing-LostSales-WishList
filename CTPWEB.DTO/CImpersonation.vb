Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Principal

Enum LogonSessionType As UInteger
    Interactive = 2
    Network
    Batch
    Service
    NetworkCleartext = 8
    NewCredentials
End Enum
Enum LogonProvider As UInteger
    [Default] = 0
    ' default for platform (use this!)
    WinNT35
    ' sends smoke signals to authority
    WinNT40
    ' uses NTLM
    WinNT50
    ' negotiates Kerb or NTLM
End Enum

Public Class CImpersonation : Implements IDisposable


    <Runtime.InteropServices.DllImport("advapi32.dll", EntryPoint:="LogonUser", SetLastError:=True)>
    Private Shared Function LogonUser(ByVal principal As String, ByVal authority As String, ByVal password As String, ByVal logonType As LogonSessionType, ByVal logonProvider As LogonProvider, ByRef token As IntPtr) As Boolean
    End Function

    <Runtime.InteropServices.DllImport("advapi32.dll", EntryPoint:="DuplicateToken", SetLastError:=True)>
    Private Shared Function DuplicateToken(ByVal hToken As IntPtr, ByVal impersonationLevel As Integer, ByRef hNewToken As IntPtr) As Boolean
    End Function

    <Runtime.InteropServices.DllImport("advapi32.dll", EntryPoint:="RevertToSelf", SetLastError:=True)>
    Private Shared Function RevertToSelf() As Boolean
    End Function


    <Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint:="CloseHandle", SetLastError:=True)>
    Private Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

    Dim impersonationContext As WindowsImpersonationContext

    Public Sub Impersonate(token As IntPtr)
        Try
            Dim tokenDuplicate As IntPtr = IntPtr.Zero
            If RevertToSelf() Then
                If DuplicateToken(token, 2, tokenDuplicate) Then

                    Dim tempWindowsIdentity As WindowsIdentity
                    tempWindowsIdentity = New WindowsIdentity(tokenDuplicate)
                    impersonationContext = tempWindowsIdentity.Impersonate()

                    If impersonationContext IsNot Nothing Then
                        CloseHandle(token)
                        CloseHandle(tokenDuplicate)
                    End If
                Else
                    If token <> IntPtr.Zero Then
                        CloseHandle(token)
                    End If
                    If tokenDuplicate <> IntPtr.Zero Then
                        CloseHandle(tokenDuplicate)
                    End If
                End If
            Else

            End If
        Catch ex As Exception

        End Try
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable

    Protected Overridable Sub Dispose(disposing As Boolean)
        '    If Not Me.disposedValue Then
        '        If disposing Then
        '            Me.Dispose()
        '            ' TODO: dispose managed state (managed objects).
        '        End If

        '        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        '        ' TODO: set large fields to null.
        '    End If
        '    Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
