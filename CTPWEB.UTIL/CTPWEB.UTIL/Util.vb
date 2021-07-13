Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Class Util : Implements IDisposable
    Private disposedValue As Boolean


    Public Sub ClearTextBoxes(p1 As Control)
        For Each ctrl As Control In p1.Controls
            Dim ctrlType = ctrl.GetType().ToString()
            If ctrlType.Equals("System.Windows.Forms.TextBox") Then
            ElseIf ctrlType.Equals("System.Windows.Forms.DropDownList") Then
            ElseIf ctrlType.Equals("System.Windows.Forms.DropDownList") Then
            Else

            End If
        Next
    End Sub

#Region "DISPOSABLE"

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region
End Class
