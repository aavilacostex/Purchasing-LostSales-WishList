Public Class ObjCtp

    Sub MySub()

        CtpNumber = Nothing
        MfrNo = Nothing

    End Sub

    Private _ctpNumber As String
    Public Property CtpNumber() As String
        Get
            Return _ctpNumber
        End Get
        Set(ByVal value As String)
            _ctpNumber = value
        End Set
    End Property

    Private _mfrNo As String
    Public Property MfrNo() As String
        Get
            Return _mfrNo
        End Get
        Set(ByVal value As String)
            _mfrNo = value
        End Set
    End Property

End Class
