Public Class WishList
    Dim dsResult As New DataSet()

    Public Sub MySub()

    End Sub

#Region "Attribute declaration"

    Private _wlid As String
    Public Property WHLCODE() As String
        Get
            Return _wlid
        End Get
        Set(ByVal value As String)
            _wlid = value
        End Set
    End Property

    Private _partNo As String
    Public Property IMPTN() As String
        Get
            Return _partNo
        End Get
        Set(ByVal value As String)
            _partNo = value
        End Set
    End Property

    Private _date As String
    Public Property WHLDATE() As String
        Get
            Return _date
        End Get
        Set(ByVal value As String)
            _date = value
        End Set
    End Property

    Private _user As String
    Public Property WHLUSER() As String
        Get
            Return _user
        End Get
        Set(ByVal value As String)
            _user = value
        End Set
    End Property

    Private _description As String
    Public Property IMDSC() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Private _status As String
    Public Property WHLSTATUS() As String
        Get
            Return _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property

    Private _assigned As String
    Public Property WHLSTATUSU() As String
        Get
            Return _assigned
        End Get
        Set(ByVal value As String)
            _assigned = value
        End Set
    End Property

    Private _vendor As String
    Public Property VENDOR() As String
        Get
            Return _vendor
        End Get
        Set(ByVal value As String)
            _vendor = value
        End Set
    End Property

    Private _vendorName As String
    Public Property VENDORNAME() As String
        Get
            Return _vendorName
        End Get
        Set(ByVal value As String)
            _vendorName = value
        End Set
    End Property

    Private _pa As String
    Public Property PA() As String
        Get
            Return _pa
        End Get
        Set(ByVal value As String)
            _pa = value
        End Set
    End Property

    Private _ps As String
    Public Property PS() As String
        Get
            Return _ps
        End Get
        Set(ByVal value As String)
            _ps = value
        End Set
    End Property

    Private _yearSales As String
    Public Property qtysold() As String
        Get
            Return _yearSales
        End Get
        Set(ByVal value As String)
            _yearSales = value
        End Set
    End Property

    Private _qtyQte As String
    Public Property QTYQTE() As String
        Get
            Return _qtyQte
        End Get
        Set(ByVal value As String)
            _qtyQte = value
        End Set
    End Property

    Private _timesq As String
    Public Property TIMESQ() As String
        Get
            Return _timesq
        End Get
        Set(ByVal value As String)
            _timesq = value
        End Set
    End Property

    Private _oemPrice As String
    Public Property IMPRC() As String
        Get
            Return _oemPrice
        End Get
        Set(ByVal value As String)
            _oemPrice = value
        End Set
    End Property

    Private _loc20 As String
    Public Property LOC20() As String
        Get
            Return _loc20
        End Get
        Set(ByVal value As String)
            _loc20 = value
        End Set
    End Property

    Private _model As String
    Public Property IMMOD() As String
        Get
            Return _model
        End Get
        Set(ByVal value As String)
            _model = value
        End Set
    End Property

    Private _category As String
    Public Property IMCATA1() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property

    Private _subcat As String
    Public Property SUBCAT() As String
        Get
            Return _subcat
        End Get
        Set(ByVal value As String)
            _subcat = value
        End Set
    End Property

    Private _major As String
    Public Property IMPC1() As String
        Get
            Return _major
        End Get
        Set(ByVal value As String)
            _major = value
        End Set
    End Property

    Private _minor As String
    Public Property IMPC2() As String
        Get
            Return _minor
        End Get
        Set(ByVal value As String)
            _minor = value
        End Set
    End Property

    Private _from As String
    Public Property WHLFROM() As String
        Get
            Return _from
        End Get
        Set(ByVal value As String)
            _from = value
        End Set
    End Property

    Private _a3comment As String
    Public Property A3COMMENT() As String
        Get
            Return _a3comment
        End Get
        Set(ByVal value As String)
            _a3comment = value
        End Set
    End Property

    Private _comment As String
    Public Property WHLCOMMENT() As String
        Get
            Return _comment
        End Get
        Set(ByVal value As String)
            _comment = value
        End Set
    End Property

#End Region

End Class
