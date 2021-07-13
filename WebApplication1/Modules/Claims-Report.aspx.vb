Imports System.ComponentModel
Imports System.Data.Odbc
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Web.Services
Imports ClosedXML.Excel
Imports CTPWEB.DTO

Public Class Claims_Report
    Inherits System.Web.UI.Page

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'ClientScript.GetPostBackEventReference(Me, "")

        If Not IsPostBack Then

            Dim yearUse = DateTime.Now().AddYears(-3).Year
            Dim firstDate = New DateTime(yearUse, 1, 1).Date()
            Dim strDateFirst As String = firstDate.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            'Dim strDateReduc As String = firstDate.ToString("yyMM", System.Globalization.CultureInfo.InvariantCulture)
            Dim curDate = DateTime.Now.Date().ToString("MM/dd/yyyy")

            Dim strDate1 = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("strDates")), ConfigurationManager.AppSettings("strDates"), strDateFirst + "," + curDate)
            Dim strDates As String() = Nothing
            strDates = strDate1.Split(",")
            'strDates(0) = strDate1.Split(",")(0).ToString()
            'strDates(1) = strDate1.Split(",")(1).ToString()
            Session("STRDATE") = strDates

            GetClaimsReport("", 1, Nothing, strDates)
            LoadDropDownLists(ddlSearchDiagnose)
            LoadDropDownLists(ddlSearchExtStatus)
            LoadDropDownLists(ddlSearchReason)
            LoadDropDownLists(ddlSearchUser)
            Session("SelectedRadio") = Nothing

            ' getClaimNumbers("130644", Today.AddYears(-2))
        Else
            loadSessionClaims()
            LoadDropDownLists(ddlSearchDiagnose)
            LoadDropDownLists(ddlSearchExtStatus)
            LoadDropDownLists(ddlSearchReason)
            LoadDropDownLists(ddlSearchUser)

            Dim controlName As String = Page.Request.Params("__EVENTTARGET")
            If Not String.IsNullOrEmpty(controlName) Then
                If ((LCase(controlName).Contains("ddl"))) Then
                    Dim ddlTrig As DropDownList = DirectCast(Me.Form.FindControl(controlName), DropDownList)
                    executesDropDownList(ddlTrig)
                End If
            End If
        End If
    End Sub

#Region "Not in Use"

    'Public Shared Function GetRadioValue(controls As ControlCollection, Optional groupName As String = Nothing) As RadioButton
    '    Dim exMessage As String = " "
    '    Dim myRadio As RadioButton = Nothing
    '    Try
    '        For Each item As Control In controls
    '            If item.Controls.Count > 0 Then
    '                For Each item1 As Control In item.Controls
    '                    If item1.UniqueID.Equals("aspnetForm") Then
    '                        For Each item2 As Control In item1.Controls
    '                            Dim pepe = item2.GetType().ToString()
    '                            If item2.GetType().ToString() = "System.Web.UI.WebControls.ContentPlaceHolder" Then
    '                                For Each item3 As Control In item2.Controls
    '                                    If item3.GetType().ToString() = "System.Web.UI.WebControls.RadioButton" Then
    '                                        myRadio = DirectCast(item3, RadioButton)
    '                                        If myRadio.Checked = True Then
    '                                            Return myRadio
    '                                        End If
    '                                    End If
    '                                Next
    '                            End If
    '                        Next
    '                    End If
    '                Next
    '            End If
    '        Next

    '        Return myRadio
    '        'Dim selectedRadioButton = controls.OfType(Of RadioButton)().FirstOrDefault(Function(rb) rb.Checked)
    '        ''rb.GroupName = groupName And
    '        'Dim rdControl = If(selectedRadioButton Is Nothing, Nothing, selectedRadioButton)
    '        'Return rdControl
    '        'Dim empty = myTableLayout.Controls.OfType(Of Windows.Forms.TextBox)().Where(Function(txt) txt.Text.Length = 0 And arrayCheckOk.Contains(txt.Name))
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return Nothing
    '    End Try
    'End Function

    'Private Sub checkPostBack(rdSelected As RadioButton, Optional ByVal opt As String = Nothing)
    '    Dim exMessage As String = " "
    '    Try
    ' If Session("SelectedRadio") IsNot Nothing Then
    'Dim myControl = rdSelected
    'Dim myControlAction = myControl.ID + "_CheckedChanged"
    'Dim mi As MethodInfo = Me.GetType().GetMethod(myControlAction)
    'If opt = "opt1" Then
    '    mi.Invoke(Me, Nothing)
    'Else
    'If rdSelected.ID.ToLower.Contains("reason") Then
    '    AddHandler myControl.CheckedChanged, AddressOf Me.rdReason_CheckedChanged
    '    ClientScript.GetPostBackEventReference(myControl, "")
    'ElseIf rdSelected.ID.ToLower.Contains("diagnose") Then
    '    AddHandler myControl.CheckedChanged, AddressOf Me.rdDiagnose_CheckedChanged
    '    ClientScript.GetPostBackEventReference(myControl, "")
    'End If
    'End If
    'AddHandler myControl.CheckedChanged, AddressOf m
    'End If
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

#End Region

    Protected Sub GetClaimsReport(strWhere As String, flag As Integer, Optional ByVal dsSessionResult As DataSet = Nothing, Optional ByVal strDates As String() = Nothing)
        Dim dsResult = New DataSet()
        Dim dsResult1 = New DataSet()
        Dim exMessage As String = " "
        Dim lstGeneral = New List(Of ClaimsGeneral)()
        Dim extObj = New DetailsHeader()
        Dim lstDetails = New List(Of ClaimsDetails)()
        Dim lastDate As String = Nothing
        Dim firstDate As String = Nothing
        Dim result As Integer = 0
        Dim resultFull As Integer = 0

        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                If dsSessionResult IsNot Nothing Then
                    If dsSessionResult.Tables(0).Rows.Count > 0 Then
                        grvClaimReport.DataSource = dsSessionResult.Tables(0)
                        grvClaimReport.DataBind()
                    End If
                Else

                    If strDates IsNot Nothing Then
                        result = objBL.GetClaimsReportSingle(dsResult, strDates)
                        resultFull = objBL.GetClaimsReportFull(dsResult1, strDates)
                    Else
                        result = objBL.GetClaimsReportSingle(dsResult)
                        resultFull = objBL.GetClaimsReportFull(dsResult1)
                    End If

                    Session("ClaimsBckData") = dsResult1
                    Session("ClaimsSingleData") = dsResult

#Region "Revisar brutal"

                    'Dim prevClaimNo = New List(Of String)()

                    'For Each dw As DataRow In dsResult.Tables(0).Rows

                    '    Dim generalObj = New ClaimsGeneral()

                    '    If Not prevClaimNo.Contains(dw.ItemArray(0).ToString()) Then

                    '        Dim countClaimNo = dsResult.Tables(0).AsEnumerable().Count(Function(aa) UCase(aa.ItemArray(0).ToString()) = UCase(dw.ItemArray(0).ToString()))
                    '        Dim claimBase = dw.ItemArray(0).ToString()

                    '        If countClaimNo > 1 Then

                    '            Dim selectedClaims = dsResult.Tables(0).AsEnumerable().Where(Function(bb) UCase(bb.ItemArray(0).ToString()) = UCase(claimBase))


                    '            generalObj.Header = New DetailsHeader()
                    '            generalObj.Header.lstClaims = lstDetails

                    '            For Each dww As DataRow In selectedClaims
                    '                Dim detailObj = New ClaimsDetails()
                    '                detailObj.ClaimNo = dww.Item("CLAIM#").ToString()
                    '                detailObj.SubmittedBy = dww.Item("SUBMITTEDBY").ToString().Trim()
                    '                detailObj.InvoiceNo = dww.Item("INVOICENO").ToString()
                    '                detailObj.InitStatus = dww.Item("CLAIM#").ToString()
                    '                detailObj.Details = dww.Item("DESCRIPTION").ToString()
                    '                detailObj.Comment1 = dww.Item("COMMENT1").ToString()
                    '                detailObj.Comment2 = dww.Item("COMMENT2").ToString()
                    '                detailObj.Comment3 = dww.Item("COMMENT3").ToString()
                    '                detailObj.DateCM = dww.Item("DATECM").ToString() 'format to only date part
                    '                detailObj.App = dww.Item("APP").ToString()
                    '                detailObj.InitStatus = dww.Item("INTSTATUS").ToString()
                    '                generalObj.Header.lstClaims.Add(detailObj)
                    '            Next

                    '            generalObj.ClaimNo = dw.Item("CLAIM#").ToString()
                    '            generalObj.Type = dw.Item("TYPE").ToString()
                    '            generalObj.Reason = dw.Item("REASON").ToString()
                    '            generalObj.Diagnose = dw.Item("DIAGNOSE").ToString()
                    '            generalObj.Customer = dw.Item("CUSTOMER").ToString()
                    '            generalObj.Initdate = dw.Item("INITDATE").ToString()
                    '            generalObj.Extstatus = dw.Item("EXTSTATUS").ToString()
                    '            generalObj.PartNo = dw.Item("PART#").ToString()
                    '            generalObj.Qty = dw.Item("QTY").ToString()
                    '            generalObj.Unitpr = dw.Item("UNITPR").ToString()
                    '            generalObj.InclNo = dw.Item("INCLNO").ToString()
                    '            generalObj.User = dw.Item("USER").ToString()
                    '            lstGeneral.Add(generalObj)

                    '        Else
                    '            generalObj.ClaimNo = dw.Item("CLAIM#").ToString()
                    '            generalObj.Type = dw.Item("TYPE").ToString()
                    '            generalObj.Reason = dw.Item("REASON").ToString()
                    '            generalObj.Diagnose = dw.Item("DIAGNOSE").ToString()
                    '            generalObj.Customer = dw.Item("CUSTOMER").ToString()
                    '            generalObj.Initdate = dw.Item("INITDATE").ToString()
                    '            generalObj.Extstatus = dw.Item("EXTSTATUS").ToString()
                    '            generalObj.PartNo = dw.Item("PART#").ToString()
                    '            generalObj.Qty = dw.Item("QTY").ToString()
                    '            generalObj.Unitpr = dw.Item("UNITPR").ToString()
                    '            generalObj.Initdate = dw.Item("INCLNO").ToString()
                    '            generalObj.User = dw.Item("USER").ToString()
                    '            lstGeneral.Add(generalObj)
                    '        End If

                    '        prevClaimNo.Add(dw.ItemArray(0).ToString())

                    '    End If

                    'Next

#End Region

                    If (result > 0 And dsResult IsNot Nothing And dsResult.Tables(0).Rows.Count > 0) Then
                        Session("DataSource") = dsResult
                        Session("PageAmounts") = 10
                        Session("currentPage") = 1
                        Session("ItemCounts") = dsResult.Tables(0).Rows.Count
                        lblTotalClaims.Text = dsResult.Tables(0).Rows.Count

                        grvClaimReport.DataSource = dsResult.Tables(0)
                        grvClaimReport.DataBind()
                    End If
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Private Sub loadSessionClaims()
        Dim exMessage As String = " "
        Try
            If Session("DataSource") IsNot Nothing Then
                Dim allSessionClaims = DirectCast(Session("DataSource"), DataSet)
                lblTotalClaims.Text = allSessionClaims.Tables(0).Rows.Count
                grvClaimReport.DataSource = allSessionClaims.Tables(0)
                grvClaimReport.DataBind()
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "Events"

#Region "GridView"

    Protected Sub grvClaimReport_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim exMessage As String = " "
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then

                'set currency format for price
                Dim price = System.Convert.ToDecimal(e.Row.Cells(10).Text)
                e.Row.Cells(10).Text = String.Format("{0:C2}", price)

                'set the color by date difference in days
                Dim myDate As Literal = DirectCast(e.Row.Cells(6).FindControl("Literal1"), Literal)
                If Not String.IsNullOrEmpty(myDate.Text) Then
                    Dim value = setDateFlag(myDate.Text)
                    If value = 1 Then
                        e.Row.Cells(6).ForeColor = System.Drawing.Color.Green
                    ElseIf value = 2 Then
                        e.Row.Cells(6).ForeColor = System.Drawing.Color.Orange
                    ElseIf value = 3 Then
                        e.Row.Cells(6).ForeColor = System.Drawing.Color.Red
                    End If
                End If

            ElseIf (e.Row.RowType = DataControlRowType.Pager) Then
                Dim strTotal = DirectCast(Session("ItemCounts"), Integer).ToString()
                Dim strNumberOfPages = DirectCast(Session("PageAmounts"), Integer).ToString()
                Dim strCurrentPage = ((DirectCast(Session("currentPage"), Integer))).ToString()

                Dim strGrouping = String.Format("Showing {0} to {1} of {2} entries ", strCurrentPage, strNumberOfPages, strTotal)
                lblGrvGroup.Text = strGrouping

                Dim sortCell As New HtmlTableCell()
                sortCell.Controls.Add(lblGrvGroup)

                Dim row1 As HtmlTableRow = New HtmlTableRow
                row1.Cells.Add(sortCell)
                ndtt.Rows.Add(row1)

                e.Row.Cells(0).Controls.AddAt(0, ndtt)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvClaimReport_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Dim exMessage As String = " "
        Try
            grvClaimReport.PageIndex = e.NewPageIndex
            Dim rs = TryCast(Session("DataFilter"), DataSet)
            If rs IsNot Nothing Then
                GetClaimsReport("", 1, rs)
            Else
                GetClaimsReport("", 1)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvClaimReport_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grvClaimReport.RowCommand
        Dim exMessage As String = Nothing
        Dim ds As New DataSet
        Try
            If e.CommandName = "Detail" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)

                ds = DirectCast(Session("DataSource"), DataSet)

                Dim claimNo = row.Cells(0).Text

                Dim myitem = ds.Tables(0).AsEnumerable().Where(Function(item) item.Item("CLAIM#").ToString().Equals(claimNo, StringComparison.InvariantCultureIgnoreCase))

                'Dim myLabel As Label = DirectCast(dataFrom.FindControl("textlbl1"), Label)
                'txtPartNumber2.Text = Trim(myLabel.Text)
                'txtPartNumber2.Enabled = False

                'Dim assigned As String = Trim(row.Cells(8).Text)
                'ddlAssignedTo.SelectedIndex = ddlAssignedTo.Items.IndexOf(ddlAssignedTo.Items.FindByText(assigned))

                'Dim status As String = row.Cells(7).Text
                'ddlStatus2.SelectedIndex = ddlStatus2.Items.IndexOf(ddlStatus2.Items.FindByText(status))

                'txtComments2.Text = GetCommentById(Trim(row.Cells(2).Text))
            ElseIf e.CommandName = "show" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)
                Dim claimNo = row.Cells(1).Text

                Dim ds1 = DirectCast(Session("ClaimsBckData"), DataSet)

                Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) item.Item("CLAIM#").ToString().Equals(claimNo, StringComparison.InvariantCultureIgnoreCase))
                If myitem.Count > 1 Then
                    Dim dtt = New DataTable()
                    dtt = myitem(0).Table.Clone()
                    For Each item As DataRow In myitem
                        dtt.ImportRow(item)
                    Next

                    Dim grv = DirectCast(sender, GridView)
                    Dim grv1 = DirectCast(row.FindControl("grvDetails"), GridView)
                    If grv1 IsNot Nothing Then
                        grv1.DataSource = dtt
                        grv1.DataBind()
                    End If

                End If
            ElseIf e.CommandName = "show1" Then
                mdClaimDetailsExp.Show()
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "Radios"

    Public Sub rdDate_CheckedChanged(sender As Object, e As EventArgs)
        Session("SelectedRadio") = sender
    End Sub

    Public Sub rdPartNo_CheckedChanged(sender As Object, e As EventArgs)
        Session("SelectedRadio") = sender
    End Sub

    Public Sub rdCustomer_CheckedChanged(sender As Object, e As EventArgs)
        Session("SelectedRadio") = sender
    End Sub

    Public Sub rdClaimNo_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim dtOut As DataTable = New DataTable()
        Session("SelectedText") = sender
        Try
            'Dim dsData = DirectCast(Session("DataSource"), DataSet)

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub rdReason_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim dtOut As DataTable = New DataTable()
        Session("SelectedRadio") = sender
        Try
            'Dim dsData = DirectCast(Session("DataSource"), DataSet)

            'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            '    result = objBL.getSearchByReasonData(dsData, dtOut)
            'End Using

            'LoadingDropDownList(ddlSearchReason, dtOut.Columns("Reason").ColumnName,
            '                            dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Reason")

            'loadSessionClaims()
            'ddlSearchReason_SelectedIndexChanged(sender, e)
            'ddlSearchReason.DataSource = dtOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub rdDiagnose_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim dtOut As DataTable = New DataTable()
        Session("SelectedRadio") = sender
        Try
            'Dim dsData = DirectCast(Session("DataSource"), DataSet)

            'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            '    result = objBL.getSearchByDiagnoseData(dsData, dtOut)
            'End Using

            'LoadingDropDownList(ddlSearchDiagnose, dtOut.Columns("Diagnose").ColumnName,
            '                            dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Diagnose")

            'loadSessionClaims()
            'ddlSearchReason.DataSource = dtOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub rdStatusOut_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim dtOut As DataTable = New DataTable()
        Session("SelectedRadio") = sender
        Try
            'Dim dsData = DirectCast(Session("DataSource"), DataSet)

            'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            '    result = objBL.getSearchByStatusOutData(dsData, dtOut)
            'End Using

            'LoadingDropDownList(ddlSearchExtStatus, dtOut.Columns("extstatus").ColumnName,
            '                            dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Status")

            'loadSessionClaims()
            'ddlSearchReason.DataSource = dtOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub rdUser_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim dtOut As DataTable = New DataTable()
        Session("SelectedRadio") = sender
        Try
            'Dim dsData = DirectCast(Session("DataSource"), DataSet)

            'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            '    result = objBL.getSearchByUserData(dsData, dtOut)
            'End Using

            'LoadingDropDownList(ddlSearchUser, dtOut.Columns("User").ColumnName,
            '                            dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select User")

            'loadSessionClaims()
            'ddlSearchReason.DataSource = dtOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub rdStatusIn_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim dtOut As DataTable = New DataTable()
        Session("SelectedRadio") = sender
        Try
            Dim dsData = DirectCast(Session("DataSource"), DataSet)

            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                result = objBL.getSearchByStatusInData(dsData, dtOut)
            End Using

            LoadingDropDownList(ddlSearchIntStatus, dtOut.Columns("intstatus").ColumnName,
                                        dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Status")

            loadSessionClaims()
            'ddlSearchReason.DataSource = dtOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "DropDownList"

    Protected Sub ddlSearchReason_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSearchReason.SelectedIndexChanged
        Dim exMessage As String = " "
        Dim sentence As String = Nothing
        Try
            If ddlSearchReason.SelectedIndex = 0 Then
                loadSessionClaims()
            ElseIf ddlSearchReason.SelectedIndex > 0 Then
                Dim dsData = DirectCast(Session("DataSource"), DataSet)
                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlSearchReason.SelectedItem.Text.ToString()
                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.ItemArray(2).ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                        'sentence = " and reason = " + valueToCompare + " "
                        'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                    End If
                Next
                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                        GetClaimsReport("", 1, dsFilter)
                    Else
                        lblTotalClaims.Text = 0
                        grvClaimReport.DataSource = Nothing
                        grvClaimReport.DataBind()
                    End If
                End If
            Else
                'error message
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub ddlSearchUser_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim sentence As String = Nothing
        Try
            If ddlSearchUser.SelectedIndex = 0 Then
                loadSessionClaims()
            ElseIf ddlSearchUser.SelectedIndex > 0 Then
                Dim dsData = DirectCast(Session("DataSource"), DataSet)
                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlSearchUser.SelectedItem.Text.ToString()
                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.ItemArray(13).ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                        'sentence = " and reason = " + valueToCompare + " "
                        'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                    End If
                Next
                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                        GetClaimsReport("", 1, dsFilter)
                    Else
                        lblTotalClaims.Text = 0
                        grvClaimReport.DataSource = Nothing
                        grvClaimReport.DataBind()
                    End If
                End If
            Else
                'error
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlSearchIntStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim sentence As String = Nothing
        Try
            If ddlSearchIntStatus.SelectedIndex = 0 Then
                loadSessionClaims()
            ElseIf ddlSearchIntStatus.SelectedIndex > 0 Then
                Dim dsData = DirectCast(Session("DataSource"), DataSet)
                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlSearchIntStatus.SelectedItem.Text.ToString()
                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.ItemArray(11).ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                        'sentence = " and reason = " + valueToCompare + " "
                        'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                    End If
                Next
                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                        GetClaimsReport("", 1, dsFilter)
                    Else
                        lblTotalClaims.Text = 0
                        grvClaimReport.DataSource = Nothing
                        grvClaimReport.DataBind()
                    End If
                End If
            Else
                'error
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlSearchExtStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim sentence As String = Nothing
        Try
            If ddlSearchExtStatus.SelectedIndex = 0 Then
                loadSessionClaims()
            ElseIf ddlSearchExtStatus.SelectedIndex > 0 Then
                Dim dsData = DirectCast(Session("DataSource"), DataSet)
                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlSearchExtStatus.SelectedItem.Text.ToString()
                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.ItemArray(6).ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                        'sentence = " and reason = " + valueToCompare + " "
                        'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                    End If
                Next
                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                        GetClaimsReport("", 1, dsFilter)
                    Else
                        lblTotalClaims.Text = 0
                        grvClaimReport.DataSource = Nothing
                        grvClaimReport.DataBind()
                    End If
                End If
            Else
                'error
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlSearchDiagnose_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim sentence As String = Nothing
        Try
            If ddlSearchDiagnose.SelectedIndex = 0 Then
                loadSessionClaims()
            ElseIf ddlSearchDiagnose.SelectedIndex > 0 Then
                Dim dsData = DirectCast(Session("DataSource"), DataSet)
                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlSearchDiagnose.SelectedItem.Text.ToString()
                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.ItemArray(3).ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                        'sentence = " and reason = " + valueToCompare + " "
                        'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                    End If
                Next
                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                        GetClaimsReport("", 1, dsFilter)
                    Else
                        lblTotalClaims.Text = 0
                        grvClaimReport.DataSource = Nothing
                        grvClaimReport.DataBind()
                    End If
                End If
            Else
                'error
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

#End Region

#Region "TextBox"

    Public Sub tqId_TextChanged(sender As Object, e As EventArgs)

    End Sub

#End Region

#Region "Buttons"

    Protected Sub btnGetTemplate_Click(sender As Object, e As EventArgs) Handles btnGetTemplate.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Dim methodMessage As String = ""
        Try
            Dim sourcePath As String = ConfigurationManager.AppSettings("urlClaimsTemplate")
            Dim myFileExternal As FileInfo = New FileInfo(sourcePath)
            Dim extFile = myFileExternal.Name

            Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Dim folderPath As String = userPath & "\Claims_Data\"

            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            Else
                Dim files = Directory.GetFiles(folderPath)
                If files.Length = 1 Then
                    Dim fi = Nothing
                    For Each item In files
                        fi = item
                        Dim isOpened = IsFileinUse(New FileInfo(fi))
                        If Not isOpened Then
                            File.Delete(item)
                        Else
                            methodMessage = "Please close the file " & fi & " in order to proceed!"
                            SendMessage(methodMessage, messageType.info)
                            'Dim rsError As DialogResult = MessageBox.Show("Please close the file " & fi & " in order to proceed!", "CTP System", MessageBoxButtons.OK)
                            'btnSelect.Enabled = False
                            Exit Sub
                        End If
                    Next
                Else
                    For Each item1 In files
                        Dim fi1 = Nothing
                        If LCase(item1).Contains(LCase(extFile)) Then
                            fi1 = item1
                            Dim isOpened = IsFileinUse(New FileInfo(fi1))
                            If Not isOpened Then
                                File.Delete(item1)
                            Else
                                methodMessage = "Please close the file " & fi1 & " in order to proceed!"
                                SendMessage(methodMessage, messageType.info)
                                'Dim rsError As DialogResult = MessageBox.Show("Please close the file " & fi & " in order to proceed!", "CTP System", MessageBoxButtons.OK)
                                'btnSelect.Enabled = False
                                Exit Sub
                            End If
                        End If
                    Next
                End If
            End If

            Dim myFile As FileInfo = New FileInfo(sourcePath)
            fileName = myFile.Name
            Dim endFolderpath = folderPath & fileName
            File.Copy(sourcePath, endFolderpath)

            Dim updatedFolderPath = folderPath & fileName

            Dim newFile As FileInfo = New FileInfo(updatedFolderPath)

            If newFile.Exists Then
                methodMessage = "The template document will be downloaded to your documents folder"
                SendMessage(methodMessage, messageType.info)

                'fuOPenEx.GetRouteUrl()

                Session("DocumentToLoad") = updatedFolderPath
                btnImportExcel.Visible = True

                System.Diagnostics.Process.Start(updatedFolderPath)
            End If

            'If File.Exists(folderPath) Then
            '    Try
            '        Process.Start("explorer.exe", folderPath)
            '    Catch Win32Exception As Win32Exception
            '        Shell("explorer " & folderPath, AppWinStyle.NormalFocus)
            '    End Try
            'End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Try

            Dim dsResult = DirectCast(Session("ClaimsBckData"), DataSet)
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then

                    Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    Dim folderPath As String = userPath & "\Claims_Data\"

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            Exit Sub
                        End If

                        Dim title As String
                        title = "Claims-File-Generated_by "
                        fileName = objBL.adjustDatetimeFormat(title, fileExtension)

                    End Using

                    Dim fullPath = folderPath + fileName

                    Using wb As New XLWorkbook()
                        wb.Worksheets.Add(dsResult.Tables(0), "ClaimsData")
                        wb.SaveAs(fullPath)
                    End Using

                    If File.Exists(fullPath) Then

                        Dim methodMessage = "The Claims Data Document will be downloaded to your documents folder."
                        SendMessage(methodMessage, messageType.info)
                        'Dim rsConfirm As DialogResult = MessageBox.Show("The file was created successfully in this path " & folderPath & " .Do you want to open the created document location?", "CTP System", MessageBoxButtons.YesNo)
                        'If rsConfirm = DialogResult.Yes Then
                        '    Try
                        '        Process.Start("explorer.exe", folderPath)
                        '    Catch Win32Exception As Win32Exception
                        '        Shell("explorer " & folderPath, AppWinStyle.NormalFocus)
                        '    End Try
                        'End If
                    End If

                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    'load excel file
    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim exMessage As String = Nothing
        Dim strResult As String = Nothing
        Try
            'Dim dtExcel = GetDataTableFromExcel(fuOPenEx)
            'If dtExcel IsNot Nothing Then
            '    If dtExcel.Rows.Count > 0 Then
            '        strResult = processExcelData(dtExcel)
            '        If strResult Is Nothing Then
            '            'ok
            '        Else
            '            'errores
            '        End If
            '    End If
            'End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub btnReset_Click(sender As Object, e As EventArgs) Handles convert.Click
        Dim exMessage As String = " "
        Try
            Dim ds = DirectCast(Session("ClaimsBckData"), DataSet)
            Session("DataSource") = ds
            Session("DataFilter") = ds
            hiddenId2.Value = 1
            hiddenId1.Value = 0
            loadSessionClaims()

            'ScriptManager.RegisterStartupScript(Me, Page.GetType, "CleanControl", "cleanDDLRows()", True)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Dim exMessage As String = " "
        Try
            mdClaimDetailsExp.Hide()
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub



#End Region

#End Region

#Region "Utils"

    Public Sub executesDropDownList(ddl As DropDownList)
        Dim exMessage As String = " "
        Try
            If ddl.ID = "ddlSearchReason" Then
                ddlSearchReason.SelectedIndex = If(Not String.IsNullOrEmpty(hdReason.Value), CInt(hdReason.Value) + 1, 0)
                ddlSearchReason_SelectedIndexChanged(ddl, Nothing)
            ElseIf ddl.ID = "ddlSearchDiagnose" Then
                ddlSearchDiagnose.SelectedIndex = If(Not String.IsNullOrEmpty(hdDiagnose.Value), CInt(hdDiagnose.Value) + 1, 0)
                ddlSearchDiagnose_SelectedIndexChanged(ddl, Nothing)
            ElseIf ddl.ID = "ddlSearchExtStatus" Then
                ddlSearchExtStatus.SelectedIndex = If(Not String.IsNullOrEmpty(hdStatusOut.Value), CInt(hdStatusOut.Value) + 1, 0)
                ddlSearchExtStatus_SelectedIndexChanged(ddl, Nothing)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub LoadDropDownLists(ddl As DropDownList)
        Dim dtOut As DataTable = New DataTable()
        Dim result As Integer = -1
        Dim dsData = DirectCast(Session("DataSource"), DataSet)
        Dim exMessage As String = " "
        Try
            If ddl.ID = "ddlSearchReason" Then
                If ddl.Items.Count = 0 Then
                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        result = objBL.getSearchByReasonData(dsData, dtOut)
                    End Using

                    LoadingDropDownList(ddlSearchReason, dtOut.Columns("Reason").ColumnName,
                                                dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Reason")
                End If
            ElseIf ddl.ID = "ddlSearchDiagnose" Then
                If ddl.Items.Count = 0 Then
                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        result = objBL.getSearchByDiagnoseData(dsData, dtOut)
                    End Using

                    LoadingDropDownList(ddlSearchDiagnose, dtOut.Columns("Diagnose").ColumnName,
                                            dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Diagnose")
                End If
            ElseIf ddl.ID = "ddlSearchExtStatus" Then
                If ddl.Items.Count = 0 Then
                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        result = objBL.getSearchByStatusOutData(dsData, dtOut)
                    End Using

                    LoadingDropDownList(ddlSearchExtStatus, dtOut.Columns("extstatus").ColumnName,
                                                dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select Status")
                End If
            ElseIf ddl.ID = "ddlSearchUser" Then
                If ddl.Items.Count = 0 Then
                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        result = objBL.getSearchByUserData(dsData, dtOut)
                    End Using

                    LoadingDropDownList(ddlSearchUser, dtOut.Columns("User").ColumnName,
                                                dtOut.Columns("ID").ColumnName, dtOut, True, "NA - Select User")
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Function setDateFlag(itemDate As String) As Integer
        Dim exMessage As String = " "
        Dim days As Integer = 0
        Dim result As Integer = -1
        Try
            Dim datetimeValue = New DateTime()
            Dim curDate = DateTime.Now().Date()
            Dim flag = Date.TryParseExact(itemDate.Trim(), "MM/dd/yyyy", CultureInfo.CurrentCulture,
                      DateTimeStyles.None, datetimeValue)
            If flag Then
                days = (curDate - datetimeValue.Date()).Days
                If days <= 30 Then
                    'green (1)
                    result = 1
                ElseIf days > 30 And days <= 60 Then
                    'yellow (2)
                    result = 2
                Else
                    'red (3)
                    result = 3
                End If
            End If
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function IsFileinUse(file As FileInfo) As Boolean
        Dim exMessage As String = Nothing
        Dim opened As Boolean = False
        Dim myStream As FileStream = Nothing
        Try
            myStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'Log.Error(exMessage)
            opened = True
        Finally
            If myStream IsNot Nothing Then
                myStream.Close()
            End If
        End Try
        Return opened
    End Function

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

    Public Sub SendMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & "')", True)
    End Sub

    Public Function MyNewRow(claimId As Object) As String
        Return String.Format("</td></tr><tr id = 'tr{0}' class = 'collapsed-row'><td></td><td colspan = '100' style = 'padding:0px;margin: 0px;'>", claimId)
    End Function

    Private Function fillObj1(dt As DataTable) As List(Of ClaimsGeneral)
        Dim exMessage As String = Nothing
        Dim objLosSales = Nothing
        Dim lstGeneral = New List(Of ClaimsGeneral)()
        Try

            'Dim blah = exampleItems.Select (Function(x) New With { .Key = x.Key, .Value = x.Value }).ToList

            For Each dw As DataRow In dt.Rows

            Next

            Dim items As IList(Of ClaimsGeneral) = dt.AsEnumerable() _
                .Select(Function(row) New WishList() With {
                .WHLCODE = row.Item("WHLCODE").ToString(),
                .IMPTN = row.Item("IMPTN").ToString(),
                .WHLDATE = row.Item("WHLDATE").ToString(),
                .WHLUSER = row.Item("WHLUSER").ToString(),
                .IMDSC = row.Item("IMDSC").ToString(),
                .WHLSTATUS = row.Item("WHLSTATUS").ToString(),
                .WHLSTATUSU = row.Item("WHLSTATUSU").ToString(),
                .VENDOR = If(row.Item("VENDOR").ToString() = "000000" Or row.Item("VENDOR").ToString() = " ", "", row.Item("VENDOR").ToString()),
                .PA = row.Item("PA").ToString(),
                .PS = row.Item("PS").ToString(),
                .qtysold = row.Item("qtysold").ToString(),
                .QTYQTE = row.Item("QTYQTE").ToString(),
                .TIMESQ = row.Item("TIMESQ").ToString(),
                .IMPRC = row.Item("IMPRC").ToString(),
                .LOC20 = row.Item("LOC20").ToString(),
                .IMMOD = row.Item("IMMOD").ToString(),
                .IMCATA1 = row.Item("IMCATA1").ToString(),
                .SUBCAT = row.Item("SUBCAT").ToString(),
                .IMPC1 = row.Item("IMPC1").ToString(),
                .IMPC2 = row.Item("IMPC2").ToString(),
                .WHLFROM = row.Item("WHLFROM").ToString(),
                .WHLCOMMENT = row.Item("WHLCOMMENT").ToString(),
                .VENDORNAME = row.Item("vendorname").ToString()
                }).ToList()

            Return items
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return objLosSales
        End Try
    End Function

    Protected Sub hdClaimNoSelected_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            Dim dsData = DirectCast(Session("DataSource"), DataSet)
            Dim dsFilter As DataSet = New DataSet()
            Dim dtFilter As DataTable = dsData.Tables(0).Clone()
            Dim valueToCompare As String = hdClaimNoSelected.Value
            txtClaimNo.Text = valueToCompare

            'Dim controlName As String = Page.Request.Params("__EVENTTARGET")

            'If Not LCase(controlName).Contains("ddl") Then
            For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.ItemArray(0).ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                        'sentence = " and reason = " + valueToCompare + " "
                        'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                    End If
                Next
                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                        GetClaimsReport("", 1, dsFilter)
                    Else
                        lblTotalClaims.Text = 0
                        grvClaimReport.DataSource = Nothing
                        grvClaimReport.DataBind()
                    End If
                End If
            'End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub hdPartNoSelected_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            Dim dsData = DirectCast(Session("DataSource"), DataSet)
            Dim dsFilter As DataSet = New DataSet()
            Dim dtFilter As DataTable = dsData.Tables(0).Clone()
            Dim valueToCompare As String = hdPartNoSelected.Value
            txtPartNo.Text = valueToCompare


            For Each dr As DataRow In dsData.Tables(0).Rows
                If dr.ItemArray(7).ToString() = valueToCompare Then
                    Dim dtr As DataRow = dtFilter.NewRow()
                    dtr.ItemArray = dr.ItemArray
                    dtFilter.Rows.Add(dtr)
                    'sentence = " and reason = " + valueToCompare + " "
                    'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                End If
            Next
            If dtFilter IsNot Nothing Then
                If dtFilter.Rows.Count > 0 Then
                    dsFilter.Tables.Add(dtFilter)
                    Session("DataFilter") = dsFilter
                    lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                    GetClaimsReport("", 1, dsFilter)
                Else
                    lblTotalClaims.Text = 0
                    grvClaimReport.DataSource = Nothing
                    grvClaimReport.DataBind()
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub hdCustomerNoSelected_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            Dim dsData = DirectCast(Session("DataSource"), DataSet)
            Dim dsFilter As DataSet = New DataSet()
            Dim dtFilter As DataTable = dsData.Tables(0).Clone()
            Dim valueToCompare As String = hdCustomerNoSelected.Value
            txtCustomer.Text = valueToCompare


            For Each dr As DataRow In dsData.Tables(0).Rows
                If dr.ItemArray(4).ToString() = valueToCompare Then
                    Dim dtr As DataRow = dtFilter.NewRow()
                    dtr.ItemArray = dr.ItemArray
                    dtFilter.Rows.Add(dtr)
                    'sentence = " and reason = " + valueToCompare + " "
                    'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                End If
            Next
            If dtFilter IsNot Nothing Then
                If dtFilter.Rows.Count > 0 Then
                    dsFilter.Tables.Add(dtFilter)
                    Session("DataFilter") = dsFilter
                    lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                    GetClaimsReport("", 1, dsFilter)
                Else
                    lblTotalClaims.Text = 0
                    grvClaimReport.DataSource = Nothing
                    grvClaimReport.DataBind()
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub hdDateInitSelected_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            Dim dsData = DirectCast(Session("DataSource"), DataSet)
            Dim dsFilter As DataSet = New DataSet()
            Dim dtFilter As DataTable = dsData.Tables(0).Clone()
            Dim valueToCompare As String = hdDateInitSelected.Value
            txtDateInit.Text = valueToCompare


            For Each dr As DataRow In dsData.Tables(0).Rows
                If dr.ItemArray(5).ToString() = valueToCompare Then
                    Dim dtr As DataRow = dtFilter.NewRow()
                    dtr.ItemArray = dr.ItemArray
                    dtFilter.Rows.Add(dtr)
                    'sentence = " and reason = " + valueToCompare + " "
                    'datatable2.Rows(i).ItemArray = datatable1(i).ItemArray
                End If
            Next
            If dtFilter IsNot Nothing Then
                If dtFilter.Rows.Count > 0 Then
                    dsFilter.Tables.Add(dtFilter)
                    Session("DataFilter") = dsFilter
                    lblTotalClaims.Text = dsFilter.Tables(0).Rows.Count
                    GetClaimsReport("", 1, dsFilter)
                Else
                    lblTotalClaims.Text = 0
                    grvClaimReport.DataSource = Nothing
                    grvClaimReport.DataBind()
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub getClaimNumbers(claimSelected As String, dateValue As DateTime)
        Dim exMessage As String = " "
        Dim dsResult = New DataSet()
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim result As Integer = objBL.getClaimNumbers(claimSelected, dateValue, dsResult)
                If (result > 0 And dsResult IsNot Nothing And dsResult.Tables(0).Rows.Count > 0) Then

                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub LoadingDropDownList(dwlControl As DropDownList, displayMember As String, valueMember As String, data As DataTable, genrateSelect As Boolean, strTextSelect As String)

        Dim dtTemp As DataTable = data.Copy()
        dwlControl.Items.Clear()
        If (genrateSelect) Then
            Dim row As DataRow = dtTemp.NewRow()
            row(displayMember) = strTextSelect
            row(valueMember) = -1
            dtTemp.Rows.InsertAt(row, 0)
        End If

        dwlControl.DataSource = dtTemp
        dwlControl.DataTextField = displayMember
        dwlControl.DataValueField = valueMember
        dwlControl.DataBind()

    End Sub

    Private Sub cleanSearchTextboxes(txtCurrent As TextBox, flag As Boolean)
        txtCurrent.Text = Not flag
        txtClaimNo.Text = If(txtCurrent.ID = txtClaimNo.ID, Not flag, flag)

    End Sub

#End Region

#Region "AutoComplete"

    <WebMethod()>
    Public Shared Function GetAutoCompleteDataClaimNo(prefixText As String) As List(Of String)
        Dim exMessage As String = " "
        'Dim resultInt As Integer
        Try
            Dim result = New List(Of String)
            'ConString = "DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;"
            'Using con As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")

            Dim con As OdbcConnection = New OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")
            con.Open()

            Dim DbCommand As OdbcCommand = con.CreateCommand()
            Dim Sql = " SELECT DISTINCT (CSMREH.MHMRNR) ClAIM# FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CSMREH.MHMRNR) LIKE '%{2}%' ORDER BY CSMREH.MHMRNR DESC 
                    FETCH FIRST 10 ROWS ONLY"

            Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)
            DbCommand.CommandText = sqlResult
            Dim DbReader As OdbcDataReader = DbCommand.ExecuteReader()

            Using sdr As OdbcDataReader = DbReader
                While sdr.Read()
                    result.Add(sdr("Claim#").ToString())
                End While
            End Using

            DbReader.Close()
            DbCommand.Dispose()
            con.Close()

            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    <WebMethod()>
    Public Shared Function GetAutoCompleteDataPartNo(prefixText As String) As List(Of String)
        Dim exMessage As String = " "
        'Dim resultInt As Integer
        Try
            Dim result = New List(Of String)
            'ConString = "DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;"
            'Using con As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")

            Dim con As OdbcConnection = New OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")
            con.Open()

            Dim DbCommand As OdbcCommand = con.CreateCommand()
            Dim Sql = " SELECT DISTINCT (CLMWRN.CWPTNO) PART# FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CLMWRN.CWPTNO) LIKE '%{2}%' ORDER BY CLMWRN.CWPTNO DESC 
                    FETCH FIRST 10 ROWS ONLY"

            Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)
            DbCommand.CommandText = sqlResult
            Dim DbReader As OdbcDataReader = DbCommand.ExecuteReader()

            Using sdr As OdbcDataReader = DbReader
                While sdr.Read()
                    result.Add(sdr("Part#").ToString())
                End While
            End Using

            DbReader.Close()
            DbCommand.Dispose()
            con.Close()

            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    <WebMethod()>
    Public Shared Function GetAutoCompleteDataCustomerNo(prefixText As String) As List(Of String)
        Dim exMessage As String = " "
        'Dim resultInt As Integer
        Try
            Dim result = New List(Of String)
            'ConString = "DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;"
            'Using con As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")

            Dim con As OdbcConnection = New OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")
            con.Open()

            Dim DbCommand As OdbcCommand = con.CreateCommand()
            Dim Sql = " SELECT DISTINCT (CSMREH.MHCUNR) CUSTOMER FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CSMREH.MHCUNR) LIKE '%{2}%' ORDER BY CSMREH.MHCUNR DESC 
                    FETCH FIRST 10 ROWS ONLY"

            Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)
            DbCommand.CommandText = sqlResult
            Dim DbReader As OdbcDataReader = DbCommand.ExecuteReader()

            Using sdr As OdbcDataReader = DbReader
                While sdr.Read()
                    result.Add(sdr("Customer").ToString())
                End While
            End Using

            DbReader.Close()
            DbCommand.Dispose()
            con.Close()

            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    <WebMethod()>
    Public Shared Function GetAutoCompleteDataInitDate(prefixText As String) As List(Of String)
        Dim exMessage As String = " "
        'Dim resultInt As Integer
        Try
            Dim result = New List(Of String)
            'ConString = "DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;"
            'Using con As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")

            Dim con As OdbcConnection = New OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")
            con.Open()

            Dim DbCommand As OdbcCommand = con.CreateCommand()
            Dim Sql = " SELECT DISTINCT (CSMREH.MHCUNR) CUSTOMER FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CSMREH.MHMRNR) LIKE '%{2}%' ORDER BY CSMREH.MHMRNR DESC 
                    FETCH FIRST 5 ROWS ONLY"

            Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)
            DbCommand.CommandText = sqlResult
            Dim DbReader As OdbcDataReader = DbCommand.ExecuteReader()

            Using sdr As OdbcDataReader = DbReader
                While sdr.Read()
                    result.Add(sdr("Customer").ToString())
                End While
            End Using

            DbReader.Close()
            DbCommand.Dispose()
            con.Close()

            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

#End Region

End Class

#Region "Not in Use"
'If Session("SelectedRadio") IsNot Nothing Then
'    Dim rdSelectedRadio = GetRadioValue(Me.Controls)
'    Dim sessionRadio = DirectCast(Session("SelectedRadio"), RadioButton)
'    If rdSelectedRadio IsNot Nothing Then
'        If rdSelectedRadio.ID = sessionRadio.ID Then
'            checkPostBack(rdSelectedRadio)
'        End If
'    End If
'End If

'<WebService(Namespace:="http://tempuri.org/")>
'<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
'<System.Web.Script.Services.ScriptService()>
'Public Class AutoCompleteCustomersVB
'    Inherits System.Web.Services.WebService

'<WebMethod(), System.Web.Script.Services.ScriptMethod()>
'Public Shared Function GetCompletionList(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As List(Of String)
'    Dim exMessage As String = " "
'    Dim claimNumbers As List(Of String) = New List(Of String)
'    Try

'        'getClaimNumbersSW("130644", Today.AddYears(-2))

'        Using con As SqlConnection = New SqlConnection()
'            con.ConnectionString = ConfigurationManager.ConnectionStrings("").ConnectionString
'            Using com As SqlCommand = New SqlCommand()

'                Dim Sql = " SELECT (CSMREH.MHMRNR) ClAIM# FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
'                and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
'                and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
'                and VARCHAR_FORMAT(CSMREH.MHMRNR) LIKE '%{2}%' ORDER BY CSMREH.MHMRNR DESC"

'                Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)

'                sqlResult = "select * from CSMREH order by CSMREH.MHMRNR DESC fetch first 10 row only"

'                'com.CommandText = "select CountryName from Countries where " + "CountryName like @Search + '%'"
'                com.CommandText = sqlResult
'                'com.Parameters.AddWithValue("@Search", prefixText)
'                com.Connection = con
'                con.Open()

'                Dim slqBaseReader = com.ExecuteReader()
'                Using sdr As SqlDataReader = slqBaseReader
'                    While sdr.Read()
'                        'listaUser.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(dataRow["cod_usuario"].ToString(), "1"));
'                        claimNumbers.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(sdr("Claim#").ToString(), 1))
'                        'claimNumbers.Add(sdr("Claim#").ToString())
'                    End While
'                End Using
'                con.Close()
'                'Return claimNumbers.ToArray()
'                Return claimNumbers
'            End Using
'        End Using
'        'Dim dtResult As DataTable = New DataTable()
'        'Dim query As String = "select nvName from Friend where nvName like '" + prefixText + "%'"
'        'da = New SqlDataAdapter(query, con)
'        'dt = New DataTable()
'        'da.Fill(dt)
'    Catch ex As Exception
'        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
'        Return claimNumbers
'    End Try

'End Function


'End Class

#End Region

