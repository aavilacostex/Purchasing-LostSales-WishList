Public Class ClaimsGeneral

    Sub MySub()
        _objDetails = New DetailsHeader()
    End Sub

    Private _claimNo As String
    Public Property ClaimNo() As String
        Get
            Return _claimNo
        End Get
        Set(ByVal value As String)
            _claimNo = value
        End Set
    End Property

    Private _type As String
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Private _reason As String
    Public Property Reason() As String
        Get
            Return _reason
        End Get
        Set(ByVal value As String)
            _reason = value
        End Set
    End Property

    Private _diagnose As String
    Public Property Diagnose() As String
        Get
            Return _diagnose
        End Get
        Set(ByVal value As String)
            _diagnose = value
        End Set
    End Property

    Private _customer As String
    Public Property Customer() As String
        Get
            Return _customer
        End Get
        Set(ByVal value As String)
            _customer = value
        End Set
    End Property

    Private _initdate As String
    Public Property Initdate() As String
        Get
            Return _initdate
        End Get
        Set(ByVal value As String)
            _initdate = value
        End Set
    End Property

    Private _extstatus As String
    Public Property Extstatus() As String
        Get
            Return _extstatus
        End Get
        Set(ByVal value As String)
            _extstatus = value
        End Set
    End Property

    Private _partNo As String
    Public Property PartNo() As String
        Get
            Return _partNo
        End Get
        Set(ByVal value As String)
            _partNo = value
        End Set
    End Property

    Private _qty As String
    Public Property Qty() As String
        Get
            Return _qty
        End Get
        Set(ByVal value As String)
            _qty = value
        End Set
    End Property

    Private _unitpr As String
    Public Property Unitpr() As String
        Get
            Return _unitpr
        End Get
        Set(ByVal value As String)
            _unitpr = value
        End Set
    End Property

    Private _inclno As String
    Public Property InclNo() As String
        Get
            Return _inclno
        End Get
        Set(ByVal value As String)
            _inclno = value
        End Set
    End Property

    Private _user As String
    Public Property User() As String
        Get
            Return _user
        End Get
        Set(ByVal value As String)
            _user = value
        End Set
    End Property

    Private _objDetails As DetailsHeader
    Public Property Header() As DetailsHeader
        Get
            Return _objDetails
        End Get
        Set(ByVal value As DetailsHeader)
            _objDetails = value
        End Set
    End Property

End Class
