Public Class LostSales
    Dim dsResult As New DataSet()

    Public Sub MySub()

    End Sub


#Region "Attribute declaration"

    Private _partNo As String
    Public Property IMPTN() As String
        Get
            Return _partNo
        End Get
        Set(ByVal value As String)
            _partNo = value
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

    Private _description2 As String
    Public Property IMDS2() As String
        Get
            Return _description2
        End Get
        Set(ByVal value As String)
            _description2 = value
        End Set
    End Property

    Private _description3 As String
    Public Property IMDS3() As String
        Get
            Return _description3
        End Get
        Set(ByVal value As String)
            _description3 = value
        End Set
    End Property

    Private _quoteQty As String
    Public Property TQUOTE() As String
        Get
            Return _quoteQty
        End Get
        Set(ByVal value As String)
            _quoteQty = value
        End Set
    End Property

    Private _timesQuote As String
    Public Property TIMESQ() As String
        Get
            Return _timesQuote
        End Get
        Set(ByVal value As String)
            _timesQuote = value
        End Set
    End Property

    Private _customersQuote As String
    Public Property NCUS() As String
        Get
            Return _customersQuote
        End Get
        Set(ByVal value As String)
            _customersQuote = value
        End Set
    End Property

    Private _salesLast12 As String
    Public Property QTYSOLD() As String
        Get
            Return _salesLast12
        End Get
        Set(ByVal value As String)
            _salesLast12 = value
        End Set
    End Property

    Private _vendorNo As String
    Public Property VENDOR() As String
        Get
            Return _vendorNo
        End Get
        Set(ByVal value As String)
            _vendorNo = value
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

    Private _purchasingAgent As String
    Public Property PAGENT() As String
        Get
            Return _purchasingAgent
        End Get
        Set(ByVal value As String)
            _purchasingAgent = value
        End Set
    End Property

    Private _listPrice As String
    Public Property IMPRC() As String
        Get
            Return _listPrice
        End Get
        Set(ByVal value As String)
            _listPrice = value
        End Set
    End Property

    Private _wishList As String
    Public Property WLIST() As String
        Get
            Return _wishList
        End Get
        Set(ByVal value As String)
            _wishList = value
        End Set
    End Property

    Private _devProj As String
    Public Property PROJECT() As String
        Get
            Return _devProj
        End Get
        Set(ByVal value As String)
            _devProj = value
        End Set
    End Property

    Private _devStatus As String
    Public Property PROJSTATUS() As String
        Get
            Return _devStatus
        End Get
        Set(ByVal value As String)
            _devStatus = value
        End Set
    End Property

    Private _loc20 As String
    Public Property F20() As String
        Get
            Return _loc20
        End Get
        Set(ByVal value As String)
            _loc20 = value
        End Set
    End Property

    Private _oemVendor As String
    Public Property FOEM() As String
        Get
            Return _oemVendor
        End Get
        Set(ByVal value As String)
            _oemVendor = value
        End Set
    End Property

    Private _majorCode As String
    Public Property IMPC1() As String
        Get
            Return _majorCode
        End Get
        Set(ByVal value As String)
            _majorCode = value
        End Set
    End Property

    Private _category As String
    Public Property IMCATA() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property

    Private _minorCode As String
    Public Property IMPC2() As String
        Get
            Return _minorCode
        End Get
        Set(ByVal value As String)
            _minorCode = value
        End Set
    End Property

    Private _descOther As String
    Public Property MINDSC() As String
        Get
            Return _descOther
        End Get
        Set(ByVal value As String)
            _descOther = value
        End Set
    End Property

    Private _catDescription As String
    Public Property CATDESC() As String
        Get
            Return _catDescription
        End Get
        Set(ByVal value As String)
            _catDescription = value
        End Set
    End Property

    Private _totalClients As String
    Public Property TotalClients() As String
        Get
            Return _totalClients
        End Get
        Set(ByVal value As String)
            _totalClients = value
        End Set
    End Property

    Private _totalCountries As String
    Public Property TotalCountries() As String
        Get
            Return _totalCountries
        End Get
        Set(ByVal value As String)
            _totalCountries = value
        End Set
    End Property

    Private _oemPart As String
    Public Property OEMPart() As String
        Get
            Return _oemPart
        End Get
        Set(ByVal value As String)
            _oemPart = value
        End Set
    End Property

    Private _subcatDesc As String
    Public Property SubCatDesc() As String
        Get
            Return _subcatDesc
        End Get
        Set(ByVal value As String)
            _subcatDesc = value
        End Set
    End Property

    Private _prPech As String
    Public Property PrPech() As String
        Get
            Return _prPech
        End Get
        Set(ByVal value As String)
            _prPech = value
        End Set
    End Property

#End Region

End Class
