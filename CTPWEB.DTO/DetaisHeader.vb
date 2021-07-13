Public Class DetailsHeader

    Sub MySub()
        _objSingle = New ClaimsDetails
    End Sub

    Private _objSingle As ClaimsDetails
    Public Property ObjDetail() As ClaimsDetails
        Get
            Return _objSingle
        End Get
        Set(ByVal value As ClaimsDetails)
            _objSingle = value
        End Set
    End Property

    Private _lstClaimsDetails As List(Of ClaimsDetails)
    Public Property lstClaims() As List(Of ClaimsDetails)
        Get
            Return _lstClaimsDetails
        End Get
        Set(ByVal value As List(Of ClaimsDetails))
            _lstClaimsDetails = value
        End Set
    End Property

End Class
