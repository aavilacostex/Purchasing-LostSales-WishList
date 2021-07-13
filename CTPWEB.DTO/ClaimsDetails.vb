Public Class ClaimsDetails

    Sub MySub()
        _selfObject = New ClaimsDetails()
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

    Private _initstatus As String
    Public Property InitStatus() As String
        Get
            Return _initstatus
        End Get
        Set(ByVal value As String)
            _initstatus = value
        End Set
    End Property

    Private _app As String
    Public Property App() As String
        Get
            Return _app
        End Get
        Set(ByVal value As String)
            _app = value
        End Set
    End Property

    Private _dateCM As String
    Public Property DateCM() As String
        Get
            Return _dateCM
        End Get
        Set(ByVal value As String)
            _dateCM = value
        End Set
    End Property

    Private _details As String
    Public Property Details() As String
        Get
            Return _details
        End Get
        Set(ByVal value As String)
            _details = value
        End Set
    End Property

    Private _submittedBy As String
    Public Property SubmittedBy() As String
        Get
            Return _submittedBy
        End Get
        Set(ByVal value As String)
            _submittedBy = value
        End Set
    End Property

    Private _invoiceNo As String
    Public Property InvoiceNo() As String
        Get
            Return _invoiceNo
        End Get
        Set(ByVal value As String)
            _invoiceNo = value
        End Set
    End Property

    Private _comment1 As String
    Public Property Comment1() As String
        Get
            Return _comment1
        End Get
        Set(ByVal value As String)
            _comment1 = value
        End Set
    End Property

    Private _comment2 As String
    Public Property Comment2() As String
        Get
            Return _comment2
        End Get
        Set(ByVal value As String)
            _comment2 = value
        End Set
    End Property

    Private _comment3 As String
    Public Property Comment3() As String
        Get
            Return _comment3
        End Get
        Set(ByVal value As String)
            _comment3 = value
        End Set
    End Property

    Private _selfObject As ClaimsDetails
    Public Property SelfObj() As ClaimsDetails
        Get
            Return _selfObject
        End Get
        Set(ByVal value As ClaimsDetails)
            _selfObject = value
        End Set
    End Property

End Class
