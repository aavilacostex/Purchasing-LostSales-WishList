Imports System.IO
Imports System.Reflection
Imports ClosedXML.Excel
Imports CTPWEB.DTO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports Newtonsoft

Public Class Lost_Sales1
    Inherits System.Web.UI.Page

    Dim total As Integer = 0

    Dim totaRowsCount As Integer = 0
    Dim pageSizeCustom As Integer = 0
    'Dim _sortDirection As String = "ASC"

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim exMessage As String = Nothing
        'Session("sortDirection") = "0"
        Try
            If Not IsPostBack Then
                load_Combos()
                'set to default when beggining
                ddlVendAssign.SelectedIndex = 1
                tqId.Text = Nothing
                unselectRadios()
                createDdlDictionary()

                Dim dsResult1 = GetLostSalesCountData()
                Dim dc = createCustomPagDictionary(dsResult1)

                'importante
                grvLostSales.VirtualItemCount = CInt(dc.Values(2).ToString())
                Session("VirtualItemCount") = CInt(dc.Values(2).ToString())

                'load by default
                Session("PageSize") = 500
                Session("PageAmounts") = 10
                Session("currentPage") = 0

                Dim strPagValues = getLimit()
                'Dim strPagValues = ""

                'by default looking form references with vendor assigned
                Dim dsResult As DataSet = New DataSet()
                GetLostSalesData(strPagValues, 0, dsResult)

                'grvLostSales.DataSource = Nothing
                'grvLostSales.DataBind()

                'executing 
                ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                Session("EventRaised") = False
            Else
                Session("EventRaised") = True
                getDataSource()
                Dim flagLoad = If(DirectCast(Session("LostSaleBck"), DataSet) IsNot Nothing, True, False)
                If flagLoad Then
                    Load_Combos_inGrid()
                End If
                checkInnerDropDownCreated()
                'GetLostSalesData("", 1, Nothing, DirectCast(Session("LostSaleData"), DataSet))
                'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
        'lblMyLabel.Attributes.Add("onclick","javascript:alert('ALERT ALERT!!!')")
    End Sub

#End Region

#Region "Generics"

    Public Function getLimit() As String
        Dim exMessage As String = Nothing
        Try
            Dim mergePagValues As List(Of String) = New List(Of String)()
            mergePagValues.Add(DirectCast(Session("currentPage"), Integer).ToString())
            mergePagValues.Add(DirectCast(Session("PageSize"), Integer).ToString())
            Dim strPagValues = mergePagValues(0) + "," + mergePagValues(1)
            Session("mergePagiValues") = strPagValues
            Return strPagValues
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Function checkInnerDropDownCreated() As Boolean
        Dim exMessage As String = Nothing
        Dim outDs As New DataSet
        Try
            Dim ph As ContentPlaceHolder = DirectCast(Me.Master.FindControl("MainContent"), ContentPlaceHolder)
            Dim grv As GridView = DirectCast(ph.FindControl("grvLostSales"), GridView)
            If grv.DataSource Is Nothing Then
                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                GetLostSalesData(Nothing, 1, Nothing, ds)
            End If
            'Dim ddl As DropDownList = DirectCast(grv.FindControl("ddlStatusFoot"), DropDownList)
            'Dim ddl1 As DropDownList = DirectCast(grv.FindControl("ddlAssignFoot"), DropDownList)
            'Dim ddl2 As DropDownList = DirectCast(grv.FindControl("ddlFromFoot"), DropDownList)
            'If ddl IsNot Nothing Then
            '    AddHandler ddl.SelectedIndexChanged, AddressOf ddlStatusFoot_SelectedIndexChanged
            'ElseIf ddl1 IsNot Nothing Then
            '    AddHandler ddl1.SelectedIndexChanged, AddressOf ddlAssignFoot_SelectedIndexChanged
            'ElseIf ddl2 IsNot Nothing Then
            '    AddHandler ddl2.SelectedIndexChanged, AddressOf ddlFromFoot_SelectedIndexChanged
            'End If
            Return True
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return False
        End Try
    End Function

    Private Function fitSelection() As IEnumerable(Of LostSales)
        Dim exMessage As String = Nothing
        Try
            Dim dsAllData As DataSet = DirectCast(Session("LostSaleBck"), DataSet)
            Dim tquote = DirectCast(Session("TimesQuote"), String)
            Dim vndSel = DirectCast(Session("flagVnd"), String)

            Dim newData = New List(Of LostSales)()

            If dsAllData IsNot Nothing Then

                Dim lstAllData = fillObj(dsAllData.Tables(0))
                Dim iteration1 = lstAllData.AsEnumerable().Where(Function(val1) If(vndSel = "3", val1.VENDOR, If(vndSel = "1", Not String.IsNullOrEmpty(val1.VENDOR), String.IsNullOrEmpty(val1.VENDOR))) _
                                                                     And val1.TIMESQ >= CInt(tquote))

                If iteration1 IsNot Nothing Then
                    If ddlCategoryFoot.SelectedIndex <> 0 Then
                        Dim iteration2 = iteration1.AsEnumerable().Where(Function(val) val.CATDESC = ddlCategoryFoot.SelectedItem.ToString())
                        If iteration2 IsNot Nothing Then
                            iteration1 = iteration2
                        End If
                    End If
                Else
                    Return Nothing
                End If

                If iteration1 IsNot Nothing Then
                    If ddlVndNameFoot.SelectedIndex <> 0 Then
                        Dim iteration3 = iteration1.AsEnumerable().Where(Function(val) val.VENDORNAME = ddlVndNameFoot.SelectedItem.ToString())
                        If iteration3 IsNot Nothing Then
                            iteration1 = iteration3
                        End If
                    End If
                Else
                    Return Nothing
                End If

                If iteration1 IsNot Nothing Then
                    If ddlMajorFoot.SelectedIndex <> 0 Then
                        Dim iteration4 = iteration1.AsEnumerable().Where(Function(val) val.IMPC1 = ddlMajorFoot.SelectedItem.ToString())
                        If iteration4 IsNot Nothing Then
                            iteration1 = iteration4
                        End If
                    End If
                Else
                    Return Nothing
                End If

                If iteration1 IsNot Nothing Then
                    If ddlWLFoot.SelectedIndex <> 0 Then
                        Dim iteration5 = iteration1.AsEnumerable().Where(Function(val) val.WLIST = ddlWLFoot.SelectedItem.ToString())
                        If iteration5 IsNot Nothing Then
                            iteration1 = iteration5
                        End If
                    End If
                Else
                    Return Nothing
                End If

                If iteration1 IsNot Nothing Then
                    If ddlSaleLast12Foot.SelectedIndex <> 0 Then
                        Dim iteration6 = iteration1.AsEnumerable().Where(Function(val) val.QTYSOLD = ddlSaleLast12Foot.SelectedItem.ToString())
                        If iteration6 IsNot Nothing Then
                            iteration1 = iteration6
                        End If
                    End If
                Else
                    Return Nothing
                End If

                Return iteration1

            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function createCustomPagDictionary(ds As DataSet) As Dictionary(Of String, String)
        If ds IsNot Nothing Then
            If ds.Tables(0).Rows.Count > 0 Then
                Dim dc = New Dictionary(Of String, String)()
                dc.Add("yes", ds.Tables(0).Rows(0).ItemArray(0).ToString())
                dc.Add("no", ds.Tables(0).Rows(1).ItemArray(0).ToString())
                dc.Add("both", CInt(ds.Tables(0).Rows(0).ItemArray(0).ToString()) + CInt(ds.Tables(0).Rows(1).ItemArray(0).ToString()))
                Session("dcCustomPag") = dc
                Return dc
            End If
        End If
    End Function

    Public Sub createDdlDictionary()

        Dim dc = New Dictionary(Of String, String)()
        dc.Add("ddlVndNameFoot", "11")
        dc.Add("ddlMajorFoot", "19")
        dc.Add("ddlWLFoot", "14")
        dc.Add("ddlCategoryFoot", "20")
        dc.Add("ddlSaleLast12Foot", "9")

        Session("gridDdlDictionary") = dc

    End Sub

    Private Function getFilteredValueInGrid(value As String) As String
        Dim exMessage As String = Nothing
        Try
            Dim dcc = DirectCast(Session("gridDdlDictionary"), Dictionary(Of String, String))
            Dim dicValue As String = Nothing
            Dim strValue As String = Nothing
            If dcc IsNot Nothing Then

                For Each item In dcc
                    If UCase(Trim(item.Key)) = UCase(Trim(value)) Then
                        dicValue = item.Value
                        Exit For
                    End If
                Next
            End If

            If dicValue IsNot Nothing Then
                If Not String.IsNullOrEmpty(dicValue) Then
                    For Each gwr As GridViewRow In grvLostSales.Rows
                        strValue = gwr.Cells(CInt(dicValue)).Text
                        Exit For
                    Next
                End If
            End If

            Return strValue
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Protected Function SetSortDirection(sortDirection As String) As String
        Dim _sortDirection As String = Nothing
        If sortDirection = "0" Then
            _sortDirection = "DESC"
        Else
            _sortDirection = "ASC"
        End If
        Session("sortDirection") = If(_sortDirection = "DESC", "1", "0")
        Return _sortDirection
    End Function

    Public Sub SendMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & "')", True)
    End Sub

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

    Public Function getDataSource(Optional preventFilters As Boolean = False) As DataSet
        Dim exMessage As String = Nothing
        Try
            If preventFilters Then
                Session("flagVnd") = "4"
                Session("TempLostSaleData") = Session("LostSaleData")
                ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                'Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                'GetLostSalesData(Nothing, 1, Nothing, dsDataGrid)
            Else
                Dim dsDataGrid = DirectCast(Session("LostSaleData"), DataSet)
                'Dim dsSetDataSource = New DataSet()
                If grvLostSales.DataSource Is Nothing Then
                    'grvLostSales.DataSource = dsDataGrid.Tables(0)
                    'grvLostSales.DataBind()
                    Return dsDataGrid
                Else
                    If getDataSourceDif(grvLostSales, dsDataGrid) Then
                        'GetLostSalesData("", 1, Nothing, dsDataGrid)
                        Return dsDataGrid
                    Else
                        'GetLostSalesData("", 1, dsSetDataSource)
                        Return Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Protected Function GetLostSalesCountData() As DataSet
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                result = objBL.GetLostSalesCountData(dsResult)
                If (result > 0 And dsResult IsNot Nothing And dsResult.Tables(0).Rows.Count > 0) Then
                    Return dsResult
                Else
                    Return Nothing
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Protected Function GetLostSalesData(strWhere As String, flag As Integer, Optional ByRef dsResult As DataSet = Nothing, Optional dsLoad As DataSet = Nothing) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        dsResult = New DataSet()
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                If dsLoad IsNot Nothing Then
                    'custom
                    If dsLoad.Tables(0).Rows.Count > 0 Then
                        If flag = 0 Then
                            Session("LostSaleBck") = dsLoad
                        End If

                        lblItemsCount.Text = dsLoad.Tables(0).Rows.Count.ToString()
                        Session("ItemCounts") = dsLoad.Tables(0).Rows.Count
                        'Session("PageAmounts") = If(((dsLoad.Tables(0).Rows.Count) / 10) Mod 2 = 0, CInt((dsLoad.Tables(0).Rows.Count) / 10), CInt((dsLoad.Tables(0).Rows.Count) / 10) + 1)
                        lblTimesQuote.Text = If((getIfCheckedQuote() = False And String.IsNullOrEmpty(tqId.Text)), DirectCast(Session("TimesQuote"), String), getStrTQouteCriteria())
                        'Session("TimesQuote") = lblTimesQuote.Text
                        Session("LostSaleData") = dsLoad

                        Dim ddl1 = ddlCategoryFoot.SelectedIndex
                        Dim ddl2 = ddlVndNameFoot.SelectedIndex

                        grvLostSales.DataSource = dsLoad.Tables(0)
                        grvLostSales.DataBind()

                        'aqui agregar else clause
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        'Dim methodMessage = "There is not results with the selected criteria. "
                        'SendMessage(methodMessage, messageType.warning)
                    End If
                Else
                    'default
                    result = objBL.GetLostSalesData(strWhere, flag, dsResult)
                    If (result > 0 And dsResult IsNot Nothing And dsResult.Tables(0).Rows.Count > 0) Then

                        If flag = 0 Then
                            Session("LostSaleBck") = dsResult
                            Session("flagVnd") = "2"
                            Session("ItemCounts") = dsResult.Tables(0).Rows.Count
                            'Session("PageAmounts") = If(((dsResult.Tables(0).Rows.Count) / 10) Mod 2 = 0, CInt((dsResult.Tables(0).Rows.Count) / 10), CInt((dsResult.Tables(0).Rows.Count) / 10) + 1)
                            'Session("PageAmounts") = 10
                            'Session("currentPage") = 1
                            Session("firstLoad") = "1"
                        End If

                        Dim flagLoad = If(DirectCast(Session("LostSaleBck"), DataSet) IsNot Nothing, True, False)
                        If flagLoad Then
                            Load_Combos_inGrid()
                        End If

                        'DoExcel(dsResult.Tables(0))

                        'grvLostSales.DataSource = dsResult.Tables(0)
                        'grvLostSales.DataBind()
                        lblItemsCount.Text = dsResult.Tables(0).Rows.Count.ToString()
                        Session("ItemCounts") = dsResult.Tables(0).Rows.Count
                        lblTimesQuote.Text = If(getIfCheckedQuote() = False, "100+", getStrTQouteCriteria())
                        Session("TimesQuote") = lblTimesQuote.Text
                        Session("LostSaleData") = dsResult
                    End If
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Private Function fillObj(dt As DataTable) As List(Of LostSales)
        Dim exMessage As String = Nothing
        Dim objLosSales = Nothing
        Try

            'Dim blah = exampleItems.Select (Function(x) New With { .Key = x.Key, .Value = x.Value }).ToList

            Dim items As IList(Of LostSales) = dt.AsEnumerable() _
                .Select(Function(row) New LostSales() With {
                .IMPTN = row.Item("IMPTN").ToString(),
                .IMDSC = row.Item("IMDSC").ToString(),
                .IMDS2 = row.Item("IMDS2").ToString(),
                .IMDS3 = row.Item("IMDS3").ToString(),
                .TQUOTE = row.Item("TQUOTE").ToString(),
                .TIMESQ = row.Item("TIMESQ").ToString(),
                .NCUS = row.Item("NCUS").ToString(),
                .VENDOR = If(row.Item("VENDOR").ToString() = "000000" Or row.Item("VENDOR").ToString() = " ", "", row.Item("VENDOR").ToString()),
                .VENDORNAME = row.Item("VENDORNAME").ToString(),
                .IMPRC = row.Item("IMPRC").ToString(),
                .F20 = row.Item("F20").ToString(),
                .FOEM = row.Item("FOEM").ToString(),
                .IMPC1 = row.Item("IMPC1").ToString(),
                .IMCATA = "",
                .IMPC2 = row.Item("IMPC2").ToString(),
                .MINDSC = row.Item("MINDSC").ToString(),
                .CATDESC = row.Item("CATDESC").ToString(),
                .WLIST = row.Item("WLIST").ToString(),
                .PROJECT = row.Item("PROJECT").ToString(),
                .PROJSTATUS = row.Item("PROJSTATUS").ToString(),
                .PAGENT = row.Item("PAGENT").ToString()
                }).ToList()

            '.QytQte = row.Item("TQUOTE").ToString(), QTYSOLD? --> saleslast12?
            '.PROJECT = GetDataFromDev(row.Item("IMPTN").ToString(), row.Item("VENDOR").ToString(), 0),
            '.PROJSTATUS = GetDataFromDev(row.Item("IMPTN").ToString(), row.Item("VENDOR").ToString(), 1),
            '.CATDESC = GetDataFromCatDesc(row.Item("IMCATA").ToString()),
            '.PAGENT = GetVendorByVendorNo(row.Item("VENDOR").ToString(), 1)
            '.WLIST = GetPartInWishList(row.Item("IMPTN").ToString())
            '.VENDORNAME = GetVendorByVendorNo(row.Item("VENDOR").ToString(), 0),
            '.SalesLast12 = row.ItemArray(7).ToString(),
            '.VndName = row.ItemArray(9).ToString(), --- ok
            '.PurcAgent = row.ItemArray(10).ToString(), --- ok?
            '.WL = row.ItemArray(12).ToString(),
            '.DevProj = row.ItemArray(13).ToString(),
            '.DevStatus = row.ItemArray(14).ToString(),

            Return items
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return objLosSales
        End Try
    End Function

    Public Shared Function ListToDataTable(ByVal _List As Object) As DataTable

        Dim dt As New DataTable

        Dim obj As Object = _List(0)
        dt = ObjectToDataTable(obj)
        Dim dr As DataRow = dt.NewRow

        For Each obj In _List

            dr = dt.NewRow

            For Each p As PropertyInfo In obj.GetType.GetProperties

                'If p.Name = "WLIST" Then
                '    If p.GetValue(obj, p.GetIndexParameters) > 0 Then
                '        Dim pepe = p.GetValue(obj, p.GetIndexParameters)
                '    End If
                'End If
                dr.Item(p.Name) = p.GetValue(obj, p.GetIndexParameters)


            Next

            dt.Rows.Add(dr)

        Next

        Return dt

    End Function

    Public Shared Function ObjectToDataTable(ByVal o As Object) As DataTable
        Dim exMessage As String = Nothing
        Try
            Dim dt As New DataTable
            Dim properties As List(Of PropertyInfo) = o.GetType.GetProperties.ToList()

            For Each prop As PropertyInfo In properties
                dt.Columns.Add(prop.Name, prop.PropertyType)
            Next

            dt.TableName = o.GetType.Name
            Return dt
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Function GetVendorByVendorNo(vnd As String, opt As Integer) As String
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.GetVendorByVendorNo(vnd, dsResult)
                If opt = 0 Then
                    Return If(rsData > 0, dsResult.Tables(0).Rows(0).ItemArray(opt).ToString(), Nothing)
                Else
                    Dim purcNumber = dsResult.Tables(0).Rows(0).ItemArray(opt).ToString()
                    Dim Purc = If(Not String.IsNullOrEmpty(purcNumber) And purcNumber <> "0", True, False)
                    If Purc Then
                        Dim dsResult1 As New DataSet
                        Dim rePurc = objBL.getUserDataByPurc(purcNumber, dsResult1)
                        Return If(rePurc > 0, dsResult1.Tables(0).Rows(0).ItemArray(0).ToString(), "")
                    End If
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetDataFromCatDesc(cat As String) As String
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.GetDataFromCatDesc(cat, dsResult)
                Return If(rsData > 0, dsResult.Tables(0).Rows(0).ItemArray(0).ToString(), Nothing)
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetPartInWishList(partNo As String) As String
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.GetPartInWishList(partNo, dsResult)
                Return If(rsData > 0, dsResult.Tables(0).Rows(0).ItemArray(0).ToString(), Nothing)
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetDataFromDev(partNo As String, vendorNo As String, opt As Integer) As String
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.GetDataFromDev(partNo, vendorNo, dsResult)
                Return If(rsData > 0, dsResult.Tables(0).Rows(0).ItemArray(opt).ToString(), Nothing)
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Sub DoExcel(dtResult As DataTable)
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Try
            If dtResult IsNot Nothing Then
                If dtResult.Rows.Count > 0 Then

                    Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    Dim folderPath As String = userPath & "\Lost_Sale_Data\"

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            Exit Sub
                        End If

                        Dim title As String
                        title = "Lost_Sale_Generated_by "
                        fileName = objBL.adjustDatetimeFormat(title, fileExtension)

                    End Using

                    Dim fullPath = folderPath + fileName

                    Using wb As New XLWorkbook()
                        wb.Worksheets.Add(dtResult, "LostSale")
                        wb.SaveAs(fullPath)

                        'Response.Clear()
                        'Response.Buffer = True
                        'Response.Charset = ""
                        'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        'Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx")
                        'Using MyMemoryStream As New MemoryStream()
                        '    wb.SaveAs(MyMemoryStream)
                        '    MyMemoryStream.WriteTo(Response.OutputStream)
                        '    Response.Flush()
                        '    Response.End()
                        'End Using
                    End Using

                    If File.Exists(fullPath) Then
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

        End Try
    End Sub

    Public Function utilGridViewRowToDatarow(dt As DataTable, dgv As GridView, row As GridViewRow) As DataTable
        Dim exMessage As String = Nothing
        Try
            Dim dtTemp = DirectCast(dgv.DataSource, DataTable)
            dt = dtTemp.Clone()
            Dim dtw As DataRow = dt.NewRow()
            dt.Rows.Add(dtw)
            Dim numOfColumns = dgv.Columns.Count
            Dim i = 0

            For i = 0 To numOfColumns - 1
                dtw.Item(i) = row.Cells(i).Text
                'dt.Rows(0).ItemArray(i) = row.Cells(i).ToString()
            Next
            dt.Rows.Add(dtw)
            Return dt
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function getIfCheckedQuote(Optional ByRef strChecked As String = Nothing) As Boolean
        If tqr10.Checked Or tqr30.Checked Or tqr50.Checked Or tqr100.Checked Then
            If tqr10.Checked Then
                strChecked = tqr10.ID
            ElseIf tqr30.Checked Then
                strChecked = tqr30.ID
            ElseIf tqr50.Checked Then
                strChecked = tqr50.ID
            Else
                strChecked = tqr100.ID
            End If
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub unselectRadios()
        If getIfCheckedQuote() Then
            tqr10.Checked = False
            tqr30.Checked = False
            tqr50.Checked = False
            tqr100.Checked = False
        End If
    End Sub

    Public Function getStrTQouteCriteria() As String
        Dim strCriteria As String = Nothing
        Dim refStringValue As String = Nothing
        Dim exMessage As String = Nothing
        Try
            If Not String.IsNullOrEmpty(tqId.Text) Then
                strCriteria = tqId.Text
            Else
                Dim result = getIfCheckedQuote(refStringValue)
                If result Then
                    Select Case refStringValue
                        Case "tqr10"
                            strCriteria = refStringValue.Substring(refStringValue.Length - 2)
                        'mystring.Substring(mystring.Length - 4)
                        Case "tqr30"
                            strCriteria = refStringValue.Substring(refStringValue.Length - 2)
                        Case "tqr50"
                            strCriteria = refStringValue.Substring(refStringValue.Length - 2)
                        Case Else
                            strCriteria = "100"
                    End Select
                End If
            End If
            Return strCriteria
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

#End Region

#Region "DropDownList"

#Region "Load DropDownList"

    Protected Sub FillDDlPrPech(dwlControl As DropDownList)
        Dim dtResult = New DataTable()
        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            Dim rsValue = objBL.FillDDlPrPech(dtResult)
            If rsValue > 0 Then
                LoadingDropDownList(dwlControl, dtResult.Columns("MixUser").ColumnName,
                                    dtResult.Columns("USUSER").ColumnName, dtResult, True, "NA - Select an User")
            End If
        End Using
    End Sub

    'dropdownlist in gridview start

    Protected Sub fill_Category(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim lstObjData = New List(Of LostSales)()
        Try

            If dwlControl.Items.Count = 0 Then

                Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                Dim lstAllValues = fillObj(dsDataGrid.Tables(0))
                Dim lstHead As List(Of String) = New List(Of String)

                'lstHead.Add(lstAllValues(0).QTYSOLD)
                For Each dw In lstAllValues
                    If dw.CATDESC IsNot Nothing Then
                        If Not String.IsNullOrEmpty(dw.CATDESC) Then
                            If Not lstHead.Contains(dw.CATDESC) Then
                                lstHead.Add(dw.CATDESC)
                                lstObjData.Add(dw)
                            End If
                        End If
                    End If
                Next

                Dim sortedList = New List(Of LostSales)()
                sortedList = lstObjData.OrderBy(Function(o) o.CATDESC).ToList()

                If lstObjData.Count > 0 Then
                    Dim dtData = ListToDataTable(sortedList)
                    If dtData IsNot Nothing Then
                        If dtData.Rows.Count > 0 Then
                            LoadingDropDownList(ddlCategoryFoot, dtData.Columns("CATDESC").ColumnName, dtData.Columns("IMPTN").ColumnName, dtData, True, "--")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlCategoryFoot.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub fill_Major(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim lstObjData = New List(Of LostSales)()
        Try

            If dwlControl.Items.Count = 0 Then

                Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                Dim lstAllValues = fillObj(dsDataGrid.Tables(0))
                Dim lstHead As List(Of String) = New List(Of String)

                'lstHead.Add(lstAllValues(0).QTYSOLD)
                For Each dw In lstAllValues
                    If dw.IMPC1 IsNot Nothing Then
                        If Not String.IsNullOrEmpty(dw.IMPC1) Then
                            If Not lstHead.Contains(dw.IMPC1) Then
                                lstHead.Add(dw.IMPC1)
                                lstObjData.Add(dw)
                            End If
                        End If
                    End If
                Next

                Dim sortedList = New List(Of LostSales)()
                sortedList = lstObjData.OrderBy(Function(o) o.IMPC1).ToList()

                If lstObjData.Count > 0 Then
                    Dim dtData = ListToDataTable(sortedList)
                    If dtData IsNot Nothing Then
                        If dtData.Rows.Count > 0 Then
                            LoadingDropDownList(ddlMajorFoot, dtData.Columns("IMPC1").ColumnName,
                                                dtData.Columns("IMPTN").ColumnName, dtData, True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlMajorFoot.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub fill_WL(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim lstObjData = New List(Of LostSales)()
        Try

            If dwlControl.Items.Count = 0 Then

                Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                Dim lstAllValues = fillObj(dsDataGrid.Tables(0))
                Dim lstHead As List(Of String) = New List(Of String)

                'lstHead.Add(lstAllValues(0).QTYSOLD)
                For Each dw In lstAllValues
                    If dw.WLIST IsNot Nothing Then
                        If Not String.IsNullOrEmpty(dw.WLIST) Then
                            If Not lstHead.Contains(dw.WLIST) Then
                                lstHead.Add(dw.WLIST)
                                lstObjData.Add(dw)
                            End If
                        End If
                    End If
                Next

                If lstObjData.Count > 0 Then
                    Dim dtData = ListToDataTable(lstObjData)
                    If dtData IsNot Nothing Then
                        If dtData.Rows.Count > 0 Then
                            LoadingDropDownList(ddlWLFoot, dtData.Columns("WLIST").ColumnName,
                                                dtData.Columns("IMPTN").ColumnName, dtData, True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlWLFoot.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub fill_VndName(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim lstObjData = New List(Of LostSales)()
        Try

            If dwlControl.Items.Count = 0 Then

                Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                Dim lstAllValues = fillObj(dsDataGrid.Tables(0))
                Dim lstHead As List(Of String) = New List(Of String)

                'lstHead.Add(lstAllValues(0).QTYSOLD)
                For Each dw In lstAllValues
                    If dw.VENDORNAME IsNot Nothing Then
                        If Not String.IsNullOrEmpty(dw.VENDORNAME) Then
                            If Not lstHead.Contains(dw.VENDORNAME) Then
                                lstHead.Add(dw.VENDORNAME)
                                lstObjData.Add(dw)
                            End If
                        End If
                    End If
                Next

                Dim sortedList = New List(Of LostSales)()
                sortedList = lstObjData.OrderBy(Function(o) o.VENDORNAME).ToList()

                If lstObjData.Count > 0 Then
                    Dim dtData = ListToDataTable(sortedList)
                    If dtData IsNot Nothing Then
                        If dtData.Rows.Count > 0 Then
                            LoadingDropDownList(ddlVndNameFoot, dtData.Columns("VENDORNAME").ColumnName, dtData.Columns("IMPTN").ColumnName, dtData, True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlVndNameFoot.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub fill_SalesLast12(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim lstObjData = New List(Of LostSales)()
        Try

            If dwlControl.Items.Count = 0 Then

                Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                Dim lstAllValues = fillObj(dsDataGrid.Tables(0))
                Dim lstHead As List(Of String) = Nothing

                'lstHead.Add(lstAllValues(0).QTYSOLD)
                For Each dw In lstAllValues
                    If dw.QTYSOLD IsNot Nothing Then
                        If Not lstHead.Contains(dw.QTYSOLD) Then
                            lstHead.Add(dw.QTYSOLD)
                            lstObjData.Add(dw)
                        End If
                    End If
                Next

                Dim sortedList = New List(Of LostSales)()
                sortedList = lstObjData.OrderBy(Function(o) o.VENDORNAME).ToList()

                If lstObjData.Count > 0 Then
                    Dim dtData = ListToDataTable(sortedList)
                    If dtData IsNot Nothing Then
                        If dtData.Rows.Count > 0 Then
                            LoadingDropDownList(ddlSaleLast12Foot, dtData.Columns("QTYSOLD").ColumnName,
                                                dtData.Columns("IMPTN").ColumnName, ds.Tables(0), True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlSaleLast12Foot.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    'dropdownlist in gridview end

    Protected Sub fill_Vendors_Assigned(dwlControl As DropDownList)

        Dim ListItem As ListItem = New ListItem()
        dwlControl.Items.Add(New WebControls.ListItem("YES", "1"))
        dwlControl.Items.Add(New WebControls.ListItem("NO", "2"))
        dwlControl.Items.Add(New WebControls.ListItem("BOTH", "3"))

    End Sub

    Protected Sub fill_Page_Size(dwlControl As DropDownList)

        Dim ListItem As ListItem = New ListItem()
        'dwlControl.Items.Add(New ListItem("Select a Projet Status", "-1"))
        dwlControl.Items.Add(New WebControls.ListItem("10", "10"))
        dwlControl.Items.Add(New WebControls.ListItem("25", "25"))
        dwlControl.Items.Add(New WebControls.ListItem("50", "50"))
        dwlControl.Items.Add(New WebControls.ListItem("100", "100"))
        dwlControl.Items.Add(New WebControls.ListItem("All", "All"))

    End Sub

#End Region

    Private Sub Load_Combos_inGrid()

        If ddlVndNameFoot.Items.Count = 0 Then
            fill_VndName(ddlVndNameFoot)
        End If
        If ddlMajorFoot.Items.Count = 0 Then
            fill_Major(ddlMajorFoot)
        End If
        If ddlSaleLast12Foot.Items.Count = 0 Then
            fill_SalesLast12(ddlSaleLast12Foot)
        End If
        If ddlCategoryFoot.Items.Count = 0 Then
            fill_Category(ddlCategoryFoot)
        End If
        If ddlWLFoot.Items.Count = 0 Then
            fill_WL(ddlWLFoot)
        End If

    End Sub

    Private Sub load_Combos()

        fill_Vendors_Assigned(ddlVendAssign)
        fill_Page_Size(ddlPageSize)

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

    Protected Sub ddlWLFoot_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = Nothing
        Try
            If ddlWLFoot.SelectedIndex = 0 Then
                ddlWLFoot.ClearSelection()
            Else
                If (ddlWLFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                    ddlWLFoot.SelectedIndex = ddlWLFoot.Items.IndexOf(ddlWLFoot.Items.FindByText(DirectCast(Session("flagDdlWLFoot"), String)))
                End If

                Dim ddlSelection = ddlWLFoot.SelectedItem.Text
                Session("flagDdlWLFoot") = ddlWLFoot.SelectedItem.Text

                Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)

                Dim dtSelection As New DataTable
                Dim dsSelection As New DataSet
                Dim lstSelection = New List(Of LostSales)()
                Dim message As String = Nothing

                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                Dim lsTemp = fillObj(ds.Tables(0))
                For Each item In lsTemp
                    If UCase(Trim(item.WLIST)) = UCase(Trim(ddlSelection)) Then
                        'lsTemp.Remove(item)
                        lstSelection.Add(item)
                    End If
                Next

                If lstSelection.Count = 0 Then
                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
                        getDataSource(True)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.WLIST)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)
                                lstSelection.Add(item1)
                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If
                    Else
                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                Else
                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then
                        Dim check = DirectCast(Session("flagVnd"), String)
                        ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.WLIST)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)

                                'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
                                'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
                                Dim myitem = lstSelection.Find(Function(ite) ite.WLIST.Equals(item1.WLIST, StringComparison.InvariantCultureIgnoreCase))

                                If myitem Is Nothing Then
                                    lstSelection.Add(item1)
                                End If

                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If
                    Else
                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                End If

                If lstSelection.Count = 0 Then
                    grvLostSales.DataSource = Nothing
                    grvLostSales.DataBind()

                    'Session("WishListData") = Session("WishListBck")

                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
                Else
                    dtSelection = ListToDataTable(lstSelection)
                    dsSelection.Tables.Add(dtSelection)
                    GetLostSalesData("", 1, Nothing, dsSelection)
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlMajorFoot_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = Nothing
        Try
            If ddlMajorFoot.SelectedIndex = 0 Then
                ddlMajorFoot.ClearSelection()
            Else
                If (ddlMajorFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                    ddlMajorFoot.SelectedIndex = ddlMajorFoot.Items.IndexOf(ddlMajorFoot.Items.FindByText(DirectCast(Session("flagDdlMajorFoot"), String)))
                End If
                Dim ddlSelection = ddlMajorFoot.SelectedItem.Text
                Session("flagDdlMajorFoot") = ddlMajorFoot.SelectedItem.Text

                Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)


                Dim dtSelection As New DataTable
                Dim dsSelection As New DataSet
                Dim lstSelection = New List(Of LostSales)()
                Dim message As String = Nothing

                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                Dim lsTemp = fillObj(ds.Tables(0))
                For Each item In lsTemp
                    If UCase(Trim(item.IMPC1)) = UCase(Trim(ddlSelection)) Then
                        'lsTemp.Remove(item)
                        lstSelection.Add(item)
                    End If
                Next

                If lstSelection.Count = 0 Then
                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
                        getDataSource(True)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.IMPC1)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)
                                lstSelection.Add(item1)
                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If
                    Else
                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                Else
                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then
                        Dim check = DirectCast(Session("flagVnd"), String)
                        ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.IMPC1)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)

                                'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
                                'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
                                Dim myitem = lstSelection.Find(Function(ite) ite.IMPC1.Equals(item1.IMPC1, StringComparison.InvariantCultureIgnoreCase))

                                If myitem Is Nothing Then
                                    lstSelection.Add(item1)
                                End If

                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If
                    Else
                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                End If

                If lstSelection.Count = 0 Then
                    grvLostSales.DataSource = Nothing
                    grvLostSales.DataBind()

                    'Session("WishListData") = Session("WishListBck")

                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
                Else
                    dtSelection = ListToDataTable(lstSelection)
                    dsSelection.Tables.Add(dtSelection)
                    GetLostSalesData("", 1, Nothing, dsSelection)
                End If

            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlCategoryFoot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlCategoryFoot.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try

            If ddlCategoryFoot.SelectedIndex = 0 Then
                ddlCategoryFoot.ClearSelection()
            Else
                If (ddlCategoryFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                    ddlCategoryFoot.SelectedIndex = ddlCategoryFoot.Items.IndexOf(ddlCategoryFoot.Items.FindByText(DirectCast(Session("flagDdlCategoryFoot"), String)))
                End If
                Dim ddlSelection = ddlCategoryFoot.SelectedItem.Text
                Session("flagDdlCategoryFoot") = ddlCategoryFoot.SelectedItem.Text

                Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)

                Dim dtSelection As New DataTable
                Dim dsSelection As New DataSet
                Dim lstSelection = New List(Of LostSales)()
                Dim message As String = Nothing

                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                Dim lsTemp = fillObj(ds.Tables(0))
                For Each item In lsTemp
                    If UCase(Trim(item.CATDESC)) = UCase(Trim(ddlSelection)) Then
                        'lsTemp.Remove(item)
                        lstSelection.Add(item)
                    End If
                Next

                If lstSelection.Count = 0 Then
                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
                        'Dim check = DirectCast(Session("flagVnd"), String)
                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                        getDataSource(True)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.CATDESC)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)
                                lstSelection.Add(item1)
                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If

                    Else

                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If

                Else
                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then

                        'Session("LostSaleData") = Session("LostSaleBck")

                        Dim check = DirectCast(Session("flagVnd"), String)
                        ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.CATDESC)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)

                                'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
                                'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
                                Dim myitem = lstSelection.Find(Function(ite) ite.CATDESC.Equals(item1.CATDESC, StringComparison.InvariantCultureIgnoreCase))

                                If myitem Is Nothing Then
                                    lstSelection.Add(item1)
                                End If

                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If

                    Else

                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                End If

                If lstSelection.Count = 0 Then
                    grvLostSales.DataSource = Nothing
                    grvLostSales.DataBind()

                    'Session("WishListData") = Session("WishListBck")

                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
                Else
                    dtSelection = ListToDataTable(lstSelection)
                    dsSelection.Tables.Add(dtSelection)
                    GetLostSalesData("", 1, Nothing, dsSelection)
                End If
                'End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlVndNameFoot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVndNameFoot.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try

            If (ddlVndNameFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                ddlVndNameFoot.SelectedIndex = ddlVndNameFoot.Items.IndexOf(ddlVndNameFoot.Items.FindByText(DirectCast(Session("flagDdlVndNameFoot"), String)))
            End If
            Dim ddlSelection = ddlVndNameFoot.SelectedItem.Text
            Session("flagDdlVndNameFoot") = ddlVndNameFoot.SelectedItem.Text

            Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)

            Dim dtSelection As New DataTable
            Dim dsSelection As New DataSet
            Dim lstSelection = New List(Of LostSales)()
            Dim message As String = Nothing

            Dim ds = DirectCast(Session("LostSaleData"), DataSet)
            Dim lsTemp = fillObj(ds.Tables(0))
            For Each item In lsTemp
                If UCase(Trim(item.VENDORNAME)) = UCase(Trim(ddlSelection)) Then
                    'lsTemp.Remove(item)
                    lstSelection.Add(item)
                End If
            Next

            If lstSelection.Count = 0 Then
                If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 Then
                    'no hay resultados de busqueda y ningun otro combo esta seleccionado
                    getDataSource(True)
                    Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.VENDORNAME)) = UCase(Trim(ddlSelection)) Then
                            'lsTemp.Remove(item)
                            lstSelection.Add(item1)
                        End If
                    Next

                    If lstSelection.Count = 0 Then
                        Session("LostSaleData") = Session("TempLostSaleData")
                        message = "There is no result for this selection."
                    End If

                Else
                    'new function
                    Dim data = fitSelection()
                    lstSelection = data.AsEnumerable().ToList()

                    message = "There is not data with this multiple criteria."
                End If

            Else
                If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then

                    'Session("LostSaleData") = Session("LostSaleBck")

                    Dim check = DirectCast(Session("flagVnd"), String)
                    ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                    Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.VENDORNAME)) = UCase(Trim(ddlSelection)) Then
                            'lsTemp.Remove(item)

                            'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
                            'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
                            Dim myitem = lstSelection.Find(Function(ite) ite.VENDORNAME.Equals(item1.VENDORNAME, StringComparison.InvariantCultureIgnoreCase))

                            If myitem Is Nothing Then
                                lstSelection.Add(item1)
                            End If

                        End If
                    Next

                    If lstSelection.Count = 0 Then
                        message = "There is no result for this selection."
                    End If

                Else

                    Dim data = fitSelection()
                    lstSelection = data.AsEnumerable().ToList()

                    message = "There is not data with this multiple criteria."
                End If
            End If

            If lstSelection.Count = 0 Then
                grvLostSales.DataSource = Nothing
                grvLostSales.DataBind()

                'Session("WishListData") = Session("WishListBck")

                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
            Else
                dtSelection = ListToDataTable(lstSelection)
                dsSelection.Tables.Add(dtSelection)
                GetLostSalesData("", 1, Nothing, dsSelection)
            End If
            'End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlSaleLast12Foot_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = Nothing
        Try
            If ddlSaleLast12Foot.SelectedIndex = 0 Then
                ddlSaleLast12Foot.ClearSelection()
            Else
                If (ddlSaleLast12Foot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                    ddlSaleLast12Foot.SelectedIndex = ddlSaleLast12Foot.Items.IndexOf(ddlSaleLast12Foot.Items.FindByText(DirectCast(Session("flagDdlSaleLast12Foot"), String)))
                End If

                Dim ddlSelection = ddlSaleLast12Foot.SelectedItem.Text
                Session("flagDdlSaleLast12Foot") = ddlSaleLast12Foot.SelectedItem.Text

                Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)

                Dim dtSelection As New DataTable
                Dim dsSelection As New DataSet
                Dim lstSelection = New List(Of LostSales)()
                Dim message As String = Nothing

                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                Dim lsTemp = fillObj(ds.Tables(0))
                For Each item In lsTemp
                    If UCase(Trim(item.QTYSOLD)) = UCase(Trim(ddlSelection)) Then
                        'lsTemp.Remove(item)
                        lstSelection.Add(item)
                    End If
                Next

                If lstSelection.Count = 0 Then
                    If ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
                        getDataSource(True)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.QTYSOLD)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)
                                lstSelection.Add(item1)
                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If
                    Else
                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                Else
                    If (ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then
                        Dim check = DirectCast(Session("flagVnd"), String)
                        ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
                        Dim lsTemp1 = fillObj(ds1.Tables(0))
                        For Each item1 In lsTemp1
                            If UCase(Trim(item1.QTYSOLD)) = UCase(Trim(ddlSelection)) Then
                                'lsTemp.Remove(item)

                                'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
                                'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
                                Dim myitem = lstSelection.Find(Function(ite) ite.QTYSOLD.Equals(item1.QTYSOLD, StringComparison.InvariantCultureIgnoreCase))

                                If myitem Is Nothing Then
                                    lstSelection.Add(item1)
                                End If

                            End If
                        Next

                        If lstSelection.Count = 0 Then
                            message = "There is no result for this selection."
                        End If
                    Else
                        Dim data = fitSelection()
                        lstSelection = data.AsEnumerable().ToList()

                        message = "There is not data with this multiple criteria."
                    End If
                End If

                If lstSelection.Count = 0 Then
                    grvLostSales.DataSource = Nothing
                    grvLostSales.DataBind()

                    'Session("WishListData") = Session("WishListBck")

                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
                Else
                    dtSelection = ListToDataTable(lstSelection)
                    dsSelection.Tables.Add(dtSelection)
                    GetLostSalesData("", 1, Nothing, dsSelection)
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim intValue As Integer
        Dim dsSetDataSource = New DataSet()
        If Integer.TryParse(ddlPageSize.SelectedValue, intValue) Then
            grvLostSales.AllowPaging = True
            grvLostSales.PageSize = If(ddlPageSize.SelectedValue > 10, CInt(ddlPageSize.SelectedValue), 10)

            Dim dsLoad = DirectCast(Session("LostSaleData"), DataSet)
            If dsLoad IsNot Nothing Then
                If dsLoad.Tables(0).Rows.Count > 0 Then
                    GetLostSalesData("", 1, Nothing, dsLoad)
                Else
                    GetLostSalesData("", 1)
                End If
            Else
                GetLostSalesData("", 1)
            End If

            Session("PageSize") = CInt(ddlPageSize.SelectedValue)

            'If getDataSourceDif(grvLostSales, dsSetDataSource) Then
            '    GetLostSalesData("", 1, dsSetDataSource)
            'Else
            '    GetLostSalesData("", 1)
            'End If

            'grvLostSales.DataBind()
        Else
            'grvLostSales.AllowPaging = False
            'Dim dtGrid As New DataTable
            'dtGrid = (DirectCast(grvLostSales.DataSource, DataTable))
            'grvLostSales.PageSize = If(dtGrid IsNot Nothing, dtGrid.Rows.Count, 0)
            'If getDataSourceDif(grvLostSales, dsSetDataSource) Then
            '    GetLostSalesData("", 1, dsSetDataSource)
            'Else
            '    GetLostSalesData("", 1)
            'End If
            'GetLostSalesData("", 1)
            'grvLostSales.DataBind()
        End If
    End Sub

    Protected Sub ddlVendAssign_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVendAssign.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim timesQDefault As String = ConfigurationManager.AppSettings("timesQuoteDefault")
        Dim dsData As New DataSet()
        Dim dtData As New DataTable()
        Dim dtResult As New DataTable()
        Dim dtResult1 As New DataTable()
        Dim lstVendors As List(Of LostSales) = New List(Of LostSales)()
        Dim lstSelectedVData As List(Of LostSales) = New List(Of LostSales)()
        Dim loadFlag = DirectCast(Session("firstLoad"), String)
        Try

            'If hiddenId3.Value <> "1" Then
            Dim ddlOption As String = ddlVendAssign.SelectedValue
                Dim sessionOption As String = DirectCast(Session("flagVnd"), String)
                Dim dsRecover = New DataSet()

                If ddlOption <> sessionOption Then
                    dsRecover = DirectCast(Session("LostSaleBck"), DataSet)
                    'Dim rs = GetLostSalesData(Nothing, 1, dsRecover)
                    Session("LostSaleData") = dsRecover
                Else
                    dsRecover = DirectCast(Session("LostSaleData"), DataSet)
                    Dim grv = grvLostSales.DataSource
                End If

                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)
                Session("TimesQuote") = getSelectedQuote

                'If grvLostSales.DataSource Is Nothing Then
                If False Then
                    Exit Sub
                ElseIf True Then

                    Session("firstLoad") = "0"
                    If ddlOption = "3" Then
                        Session("flagVnd") = "3"
                        lstVendors = fillObj(dsRecover.Tables(0))
                        For Each obj As LostSales In lstVendors
                            If obj.TIMESQ >= CInt(getSelectedQuote) Then
                                lstSelectedVData.Add(obj)
                            End If
                            'If Not String.IsNullOrEmpty(getSelectedQuote) Then
                            '    If obj.TIMESQ >= CInt(getSelectedQuote) Then
                            '        lstSelectedVData.Add(obj)
                            '    End If
                            'Else
                            '    If obj.TIMESQ >= CInt(timesQDefault) Then
                            '        lstSelectedVData.Add(obj)
                            '    End If
                            'End If
                        Next

                        dtResult1 = ListToDataTable(lstSelectedVData)
                        Dim ds As DataSet = New DataSet()
                        ds.Tables.Add(dtResult1)
                        GetLostSalesData(Nothing, 1, Nothing, ds)
                    Else

                        lstVendors = fillObj(dsRecover.Tables(0))

                        For Each obj As LostSales In lstVendors
                            If ddlOption.Equals("1") Then
                                Session("flagVnd") = "1"
                            If Not String.IsNullOrEmpty(obj.VENDOR) Then
                                Dim tq = If(String.IsNullOrEmpty(obj.TIMESQ), 0, CInt(obj.TIMESQ))
                                If tq >= CInt(getSelectedQuote) Then
                                    lstSelectedVData.Add(obj)
                                End If
                                'If Not String.IsNullOrEmpty(getSelectedQuote) Then
                                '    Session("TimesQuote") = getSelectedQuote
                                '    If obj.TIMESQ >= CInt(getSelectedQuote) Then
                                '        lstSelectedVData.Add(obj)
                                '    End If
                                'Else
                                '    Session("TimesQuote") = timesQDefault
                                '    If obj.TIMESQ >= CInt(timesQDefault) Then
                                '        lstSelectedVData.Add(obj)
                                '    End If
                                'End If
                            End If
                        ElseIf ddlOption.Equals("2") Then
                                Session("flagVnd") = "2"
                            If String.IsNullOrEmpty(obj.VENDOR) Then
                                Dim tq = If(String.IsNullOrEmpty(obj.TIMESQ), 0, CInt(obj.TIMESQ))
                                If tq >= CInt(getSelectedQuote) Then
                                    lstSelectedVData.Add(obj)
                                End If
                                'If Not String.IsNullOrEmpty(getSelectedQuote) Then
                                '    Session("TimesQuote") = getSelectedQuote
                                '    If obj.TIMESQ >= CInt(getSelectedQuote) Then
                                '        lstSelectedVData.Add(obj)
                                '    End If
                                'Else
                                '    Session("TimesQuote") = timesQDefault
                                '    If obj.TIMESQ >= CInt(timesQDefault) Then
                                '        lstSelectedVData.Add(obj)
                                '    End If
                                'End If
                            End If
                        End If
                    Next

                    If lstSelectedVData.Count > 0 Then
                            dtResult1 = ListToDataTable(lstSelectedVData)
                            Dim ds As DataSet = New DataSet()
                            ds.Tables.Add(dtResult1)
                            GetLostSalesData(Nothing, 1, Nothing, ds)
                            'Else
                            '    Dim dss = New DataSet()
                            '    dss = DirectCast(Session("LostSaleData"), DataSet)
                            '    Dim dtt = New DataTable()
                            '    dtt = dss.Tables(0).Copy()
                            '    dtResult1 = dtt
                        Else
                            Dim ds1 As DataSet = New DataSet()
                            Dim dt1 As DataTable = New DataTable()
                            ds1.Tables.Add(dt1)
                            GetLostSalesData(Nothing, 1, Nothing, ds1)

                            'Dim methodMessage = "There is not results with the selected criteria. "
                            'SendMessage(methodMessage, messageType.warning)
                        End If
                    End If
                End If

                updatepnl2.Update()

                'Else
            'Dim ph As ContentPlaceHolder = DirectCast(Me.Master.FindControl("MainContent"), ContentPlaceHolder)
            'Dim grv As GridView = DirectCast(ph.FindControl("grvLostSales"), GridView)
            'Dim ds = DirectCast(Session("LostSaleData"), DataSet)
            'GetLostSalesData(Nothing, 1, Nothing, ds)
            'hiddenId3.Value = "0"
            'End If


        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "Gridview"

    'Protected Sub fillcell1(strWhere As String, flag As Integer)
    '    Dim dsResult = New DataSet()
    '    Try
    '        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
    '            Dim result As Integer = objBL.FillGridProjects(strWhere, flag, dsResult)
    '            If (result > 0 And dsResult IsNot Nothing And dsResult.Tables(0).Rows.Count > 0) Then
    '                grvLostSales.DataSource = dsResult.Tables(0)
    '                grvLostSales.DataBind()
    '            End If
    '        End Using
    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Protected Sub grvLostSales_PreRender(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Try
    '        Dim gv As GridView = CType(sender, GridView)
    '        Dim gvr As GridViewRow = CType(gv.BottomPagerRow, GridViewRow)
    '        If gvr IsNot Nothing Then
    '            gvr.Visible = True
    '        End If
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    Protected Sub grvLostSales_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Dim exMessage As String = " "
        Dim dsResult = New DataSet()
        Try
            grvLostSales.PageIndex = e.NewPageIndex
            Session("currentPage") = (CInt(e.NewPageIndex + 1) * 10) - 9
            Session("PageAmounts") = If((CInt(e.NewPageIndex + 1) * 10) > DirectCast(Session("ItemCounts"), Integer), (CInt(e.NewPageIndex + 1) * 10), DirectCast(Session("ItemCounts"), Integer))
            'Dim ds = getDataSource()
            'If ds IsNot Nothing Then
            '    grvLostSales.DataSource = ds.Tables(0)
            'Else
            '    Dim grid = DirectCast(sender, GridView)
            '    Dim dtGrid = TryCast(grid.DataSource, DataTable)
            '    grvLostSales.DataSource = dtGrid
            'End If
            'grvLostSales.DataBind()

            Dim strPagValues = getLimit()
            GetLostSalesData(strPagValues, 0, dsResult)

            updatepnl2.Update()

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvLostSales_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Dim exMessage As String = " "
        Dim methodMessage As String = Nothing
        Dim lstReferences As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim countReferences As Integer = 0
        Dim flagError As Integer = 0
        Try
            If e.CommandName = "AddAll" Then
                lstReferences = GetCheckboxesDisp()
                If lstReferences Is Nothing Then
                    methodMessage = "An exception occur in the method execution!"
                    SendMessage(methodMessage, messageType.Error)
                Else
                    If lstReferences.Count = 0 Then
                        methodMessage = "Please select the items that you want to place in Wish List and then click this button!"
                        SendMessage(methodMessage, messageType.warning)
                    Else
                        'process the data
                        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                            For Each item In lstReferences
                                Dim result = objBL.InsertWishListReference(item.Value, item.Key, "1", "1", "PRDWL", "WHLCODE")
                                'status when add to wish list
                                If result > 0 Then
                                    countReferences += 1
                                Else
                                    flagError += 1
                                End If
                            Next
                        End Using

                        If flagError > 0 Then
                            methodMessage = "There is an error in the insertion process."
                            SendMessage(methodMessage, messageType.Error)
                        Else
                            methodMessage = "Successful Insertion for " + countReferences.ToString() + " record."
                            SendMessage(methodMessage, messageType.success)
                        End If
                    End If
                End If

            ElseIf e.CommandName = "SingleAdd" Then
                Dim row As GridViewRow = DirectCast(e.CommandSource, LinkButton).Parent.Parent
                Dim partNo As String = row.Cells(2).Text
                Dim userid As String = If(Not String.IsNullOrEmpty(row.Cells(12).Text) And row.Cells(12).Text <> "&nbsp;", row.Cells(12).Text, "N/A")
                lstReferences.Add(partNo, userid)

                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    Dim result = objBL.InsertWishListReference(userid, partNo, "1", "1", "PRDWL", "WHLCODE")
                    'status when add to wish list
                    countReferences = result
                    If result > 0 Then
                        methodMessage = "Successful Insertion for " + result.ToString() + " record."
                        SendMessage(methodMessage, messageType.success)
                    Else
                        methodMessage = "There is an error in the insertion process."
                        SendMessage(methodMessage, messageType.Error)
                    End If

                End Using
            ElseIf e.CommandName = "show" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)
                Dim id = row.Cells(2).Text

                Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)

                Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) item.Item("IMPTN").ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase))
                If myitem.Count = 1 Then
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
            End If

            'actualizando gridview in live
            If countReferences > 0 Then

                For Each item In lstReferences
                    Dim value = Trim(item.Key)
                    For Each gwr As GridViewRow In grvLostSales.Rows
                        Dim cellvalue = Trim(gwr.Cells(2).Text.ToString())
                        If value = cellvalue Then
                            gwr.Cells(14).Text = "1"
                            grvLostSales.UpdateRow(gwr.RowIndex, False)
                            Exit For
                        End If
                    Next
                Next

                Dim dtt = New DataTable()
                dtt = DirectCast(grvLostSales.DataSource, DataTable)
                Dim dt = dtt.Copy()
                Dim dss = New DataSet()
                'dss.Tables.RemoveAt(0)
                dss.Tables.Add(dt)
                Session("LostSaleData") = dss
                updatepnl2.Update()
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvLostSales_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Dim exMessage As String = Nothing

        Try
            Dim dsss = New DataSet()
            dsss = DirectCast(Session("LostSaleData"), DataSet)

            Dim roww As GridViewRow = grvLostSales.Rows(e.RowIndex)
            dsss.Tables(0).Rows(roww.DataItemIndex)("WLIST") = roww.Cells(14).Text

            grvLostSales.DataBind()
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub grvLostSales_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim exMessage As String = " "
        Dim dsResult = New DataSet()
        Dim dsResult1 = New DataSet()
        Dim dsResultWL = New DataSet()
        Dim dsResultDev = New DataSet()
        Dim dsResultCataDesc = New DataSet()
        Dim lstValues = New List(Of String)()
        Dim result As Integer = -1
        Dim resultWL As Integer = -1
        Dim resultDev As Integer = -1
        Dim resultCataDesc As Integer = -1
        Dim flagPurc As Boolean = False
        Try

            'Dim headerRow As GridViewRow = grvLostSales.HeaderRow
            'createPagingSummaryOnPagerTemplate(sender, totaRowsCount, pageSizeCustom)

            If e.Row.RowType = DataControlRowType.DataRow Then

                Dim price = System.Convert.ToDecimal(e.Row.Cells(13).Text)
                e.Row.Cells(13).Text = String.Format("{0:C2}", price)

                'execute subqueries for every row
                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                    Dim partNo = e.Row.Cells(2).Text.Trim()
                    Dim factor = ConfigurationManager.AppSettings("yearFactor")
                    Dim vendorNo = If(String.IsNullOrEmpty(e.Row.Cells(10).Text) Or e.Row.Cells(10).Text = "&nbsp;", "000000", e.Row.Cells(10).Text.Trim())

                    'total client
                    Dim tclients = objBL.GetTotalClients(partNo, factor)
                    Dim lbl = DirectCast(e.Row.FindControl("lblTClients"), Label)
                    lbl.Text = tclients

                    'total country
                    Dim tcountries = objBL.GetTotalCountries(partNo, factor)
                    Dim lbl1 = DirectCast(e.Row.FindControl("lblTCountries"), Label)
                    lbl1.Text = tcountries

                    'oem vendor
                    Dim lbl2 = DirectCast(e.Row.FindControl("lblOEMPart"), Label)
                    Dim toempart = ""
                    If vendorNo = "000000" Then
                        lbl2.Text = toempart
                    Else
                        toempart = objBL.GetOEMPart(partNo, vendorNo)
                        lbl2.Text = toempart
                    End If

                    'Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                    'If ds IsNot Nothing Then
                    '    For Each dw As DataRow In ds.Tables(0).Rows
                    '        If Trim(UCase(dw.Item("IMPTN"))) = Trim(UCase(partNo)) Then
                    '            'dw.Item("totalclients") = tclients
                    '            'dw.Item("totalcountry") = tcountries
                    '            'dw.Item("oemvendor") = toempart
                    '            Exit For
                    '        End If
                    '    Next

                    '    Session("LostSaleData") = ds
                    'End If

                End Using

            ElseIf e.Row.RowType = DataControlRowType.Header Then
                For index = 0 To grvLostSales.Columns.Count - 1
                    Dim name = grvLostSales.Columns(index).HeaderText
                    'Dim style = grvLostSales.Columns(index).ItemStyle().CssClass
                    'If style <> "hidecol" Then
                    lstValues.Add(name)
                    'End If
                    Session("grvLostSalesHeaders") = lstValues
                Next
                Dim chk As CheckBox = DirectCast(e.Row.FindControl("chkAll"), CheckBox)
                AddHandler chk.CheckedChanged, AddressOf chkAll_CheckedChanged
            ElseIf (e.Row.RowType = DataControlRowType.Footer) Then

                Dim lstValuesFoot = DirectCast(Session("grvLostSalesHeaders"), List(Of String))
                If lstValuesFoot.Count > 0 Then
                    Dim x As Integer = 0
                    For Each item As String In lstValuesFoot
                        If Trim(item.ToUpper()) = "SALES LAST12" Then
                            fill_SalesLast12(ddlSaleLast12Foot)
                            AddHandler ddlSaleLast12Foot.SelectedIndexChanged, AddressOf ddlSaleLast12Foot_SelectedIndexChanged
                            e.Row.Cells(x).Controls.Add(ddlSaleLast12Foot)
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        ElseIf Trim(item.ToUpper()) = "VND NAME" Then
                            fill_VndName(ddlVndNameFoot)
                            AddHandler ddlVndNameFoot.SelectedIndexChanged, AddressOf ddlVndNameFoot_SelectedIndexChanged
                            e.Row.Cells(x).Controls.Add(ddlVndNameFoot)
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        ElseIf Trim(item.ToUpper()) = "WL" Then
                            fill_WL(ddlWLFoot)
                            AddHandler ddlWLFoot.SelectedIndexChanged, AddressOf ddlWLFoot_SelectedIndexChanged
                            e.Row.Cells(x).Controls.Add(ddlWLFoot)
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        ElseIf Trim(item.ToUpper()) = "MAJOR" Then
                            fill_Major(ddlMajorFoot)
                            AddHandler ddlMajorFoot.SelectedIndexChanged, AddressOf ddlMajorFoot_SelectedIndexChanged
                            e.Row.Cells(x).Controls.Add(ddlMajorFoot)
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        ElseIf Trim(item.ToUpper()) = "CATEGORY" Then
                            fill_Category(ddlCategoryFoot)
                            AddHandler ddlCategoryFoot.SelectedIndexChanged, AddressOf ddlCategoryFoot_SelectedIndexChanged
                            e.Row.Cells(x).Controls.Add(ddlCategoryFoot)
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        ElseIf item = "DESCRIPTION 2" Then
                            x += 1
                        ElseIf item = "DESCRIPTION 3" Then
                            x += 1
                        ElseIf item = "DESC" Then
                            x += 1
                        ElseIf item = "MINOR" Then
                            x += 1
                        ElseIf item = "" Then
                            Dim btn As Button = DirectCast(e.Row.FindControl("ButtonAdd"), Button)
                            AddHandler btn.Click, AddressOf ButtonAdd_Click
                            e.Row.Cells(x).Controls.Add(btn)
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        Else
                            e.Row.Cells(x).Text = item
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        End If
                    Next
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

                'Dim tbl As Table = DirectCast(e.Row.Cells(0).Controls.Item(0), Table)
                'tbl.Rows(0).Cells.AddAt(0, sortCell)
            End If

            'Dim grid As GridView = DirectCast(sender, GridView)
            'TryCast(sender, GridView)
            'If grid IsNot Nothing Then
            '    Dim row As New GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal)
            '    Dim header As GridViewRow = grvLostSales.HeaderRow
            '    For i As Integer = 0 To grvLostSales.Columns.Count - 1
            '        Dim TableCell As New TableHeaderCell()
            '        TableCell.Text = header.Cells(i).Text
            '        row.Cells.Add(TableCell)
            '    Next
            '    Dim t As Table = TryCast(grid.Controls(0), Table)
            '    If t IsNot Nothing Then
            '        t.Rows.AddAt(t.Rows.Count, row)
            '    End If
            'End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvLostSales_Sorting(sender As Object, e As GridViewSortEventArgs)
        Dim dtw As DataView = Nothing
        Dim newDt As DataTable = New DataTable()
        Dim exMessage As String = Nothing
        Try
            Dim dt As DataTable = DirectCast(grvLostSales.DataSource, DataTable)
            If dt IsNot Nothing Then
                dtw = New DataView(dt)
                dtw.Sort = e.SortExpression + " " + SetSortDirection(e.SortDirection)

                newDt = dtw.ToTable()
                Dim ds As DataSet = New DataSet()
                ds.Tables.Add(newDt)
                Session("LostSaleData") = ds

                ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
            Else
                Dim ds As DataSet = New DataSet()
                ds = getDataSource()
                dtw = New DataView(ds.Tables(0))
                Dim direction = DirectCast(Session("sortDirection"), String)
                dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                newDt = dtw.ToTable()
                ds.Tables.RemoveAt(0)
                ds.Tables.Add(newDt)
                Session("LostSaleData") = ds

                ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Public Sub setGridProperties()
        Dim exMessage As String = Nothing
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim lstResult = objBL.GetGridParameterDin()
                totaRowsCount = lstResult(0)
                pageSizeCustom = lstResult(1)
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Private Function getDataSourceDif(dgv As GridView, Optional ByRef dsDataSource As DataSet = Nothing) As Boolean
        Dim exMessage As String = Nothing
        Try
            Dim dtGrid = TryCast(dgv.DataSource, DataTable)
            Dim dsSessionGrid = DirectCast(Session("LostSaleData"), DataSet)
            If dtGrid.Rows.Count = dsSessionGrid.Tables(0).Rows.Count Then
                dsDataSource = dsSessionGrid
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return False
        End Try
    End Function

#Region "Summary code to check"

    'Protected Overrides Sub OnPreRender(e As EventArgs)
    '    Me.OnPreRender(e)
    '    SetFixedHeightForGridIfRowsAreLess(grvLostSales)
    'End Sub

    'Public Sub SetFixedHeightForGridIfRowsAreLess(gv As GridView)
    '    Dim exMessage As String = Nothing
    '    Try
    '        Dim headerFooterHeight As Double = gv.HeaderStyle.Height.Value + 35  'we set header height style=35px And there no footer  height so assume footer also same
    '        Dim rowHeight As Double = gv.RowStyle.Height.Value
    '        Dim gridRowCount As Integer = gv.Rows.Count
    '        If (gridRowCount <= gv.PageSize) Then
    '            Dim height As Double = (gridRowCount * rowHeight) + ((gv.PageSize - gridRowCount) * rowHeight) + headerFooterHeight
    '            'adjust footer height based on white space removal between footer And last row
    '            height += 40
    '            gv.Height = New Unit(height)
    '        End If
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    'Public Sub createPagingSummaryOnPagerTemplate(sender As Object, totalCount As Integer, pageSize As Integer)
    '    Dim exMessage As String = Nothing
    '    Try
    '        Dim gv As GridView = DirectCast(sender, GridView)
    '        If (gv IsNot Nothing) Then

    '            If gv.DataSource Is Nothing Then
    '                Dim dtDataSource = DirectCast(Session("LostSaleData"), DataSet)
    '                GetLostSalesData(Nothing, 0, Nothing, dtDataSource)

    '                Dim row As GridViewRow = gv.BottomPagerRow
    '                Dim rowTest As GridViewRow = gv.HeaderRow

    '                If row IsNot Nothing Then

    '                End If
    '            Else
    '                'Get Bottom Pager Row from a gridview
    '                Dim row As GridViewRow = gv.BottomPagerRow
    '                Dim rowTest As GridViewRow = gv.HeaderRow

    '                If row IsNot Nothing Then
    '                    'create New cell to add to page strip
    '                    Dim pagingSummaryCell As TableCell = New TableCell()
    '                    pagingSummaryCell.Text = DisplayCusotmPagingSummary(totalCount, gv.PageIndex, pageSize)
    '                    pagingSummaryCell.HorizontalAlign = HorizontalAlign.Right
    '                    pagingSummaryCell.VerticalAlign = VerticalAlign.Middle
    '                    pagingSummaryCell.Width = Unit.Percentage(100)
    '                    pagingSummaryCell.Height = Unit.Pixel(35)
    '                    'Getting table which shows PagingStrip

    '                    Dim cusDt As New DataTable()
    '                    Dim pepe = utilGridViewRowToDatarow(cusDt, grvLostSales, row)


    '                    Dim tbl As New Table()
    '                    Dim newRow As TableRow
    '                    newRow = DirectCast(row.DataItem, TableRow)




    '                    tbl.Rows.Add(newRow)

    '                    If (totalCount <= pageSize) Then
    '                        gv.BottomPagerRow.Visible = True
    '                        tbl.Rows(0).Cells.Clear()
    '                        tbl.Width = Unit.Percentage(100)
    '                    End If

    '                    'Find table And add paging summary text
    '                    tbl.Rows(0).Cells.Add(pagingSummaryCell)
    '                    'assign header row color to footer row
    '                    tbl.BackColor = System.Drawing.ColorTranslator.FromHtml("#1AD9F2")
    '                    tbl.Width = Unit.Percentage(100)
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    'Public Shared Function DisplayCusotmPagingSummary(numberOfRecords As Integer, currentPage As Integer, pageSize As Integer) As String
    '    Dim exMessage As String = Nothing
    '    Try
    '        Dim strDisplaySummary As StringBuilder = New StringBuilder()
    '        Dim numberOfPages As Integer

    '        If (numberOfRecords > pageSize) Then
    '            'Calculating the total number of pages
    '            Dim nor As Double = CInt(numberOfRecords)
    '            Dim ps As Double = CInt(pageSize)

    '            numberOfPages = CInt(Math.Ceiling(nor / ps))
    '            'numberOfPages = (Int())Math.Ceiling((Double)numberOfRecords / (Double)pageSize);
    '        Else
    '            numberOfPages = 1
    '        End If

    '        strDisplaySummary.Append("Showing ")
    '        Dim floor As Integer = (currentPage * pageSize) + 1
    '        strDisplaySummary.Append(floor.ToString())
    '        strDisplaySummary.Append("-")
    '        Dim ceil As Integer = ((currentPage * pageSize) + pageSize)

    '        If (ceil > numberOfRecords) Then
    '            strDisplaySummary.Append(numberOfRecords.ToString())
    '        Else
    '            strDisplaySummary.Append(ceil.ToString())
    '        End If

    '        strDisplaySummary.Append(" of ")
    '        strDisplaySummary.Append(numberOfRecords.ToString())
    '        strDisplaySummary.Append(" results ")
    '        Return strDisplaySummary.ToString()
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return Nothing
    '    End Try
    'End Function

#End Region

#End Region

#Region "RadioButtons"

    'Protected Sub tqr10__FixAutoPostBack_OnPreRender(sender As Object, e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Try
    '        Dim RadioButton As RadioButton = DirectCast(sender, RadioButton)
    '        Dim Label As System.Web.UI.WebControls.ContentPlaceHolder = DirectCast(RadioButton.Parent, ContentPlaceHolder)
    '        'Label.fin
    '        'Label.Attributes.Add("onclick", "javascript:setTimeout('__doPostBack(\\'" + RadioButton.UniqueID + "\\',\\'\\')', 0)")
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    Protected Sub tqr10_CheckedChanged(sender As Object, e As EventArgs)
        If (tqr10.Checked) Then
            tqId.Text = ""
        End If
    End Sub

    Protected Sub tqr30_CheckedChanged(sender As Object, e As EventArgs)
        If (tqr30.Checked) Then
            tqId.Text = ""
        End If
    End Sub

    Protected Sub tqr50_CheckedChanged(sender As Object, e As EventArgs)
        If (tqr50.Checked) Then
            tqId.Text = ""
        End If
    End Sub

    Protected Sub tqr100_CheckedChanged(sender As Object, e As EventArgs)
        If (tqr100.Checked) Then
            tqId.Text = ""
        End If
    End Sub

#End Region

#Region "Buttons"

    Protected Sub submit_Click(sender As Object, e As EventArgs) Handles submit.Click
        Dim exMessage As String = Nothing
        Dim lstData = New List(Of LostSales)()
        Dim filterData = New List(Of LostSales)()
        Dim dsWork As DataSet = New DataSet()
        Dim searchstring As String = Nothing
        Try
            If Not String.IsNullOrEmpty(tqId.Text) Or getIfCheckedQuote() Then
                searchstring = If(CInt(getStrTQouteCriteria()), getStrTQouteCriteria(), "0")

                Dim dsData = getDataSource(True)
                If dsData IsNot Nothing Then
                    If dsData.Tables(0).Rows.Count > 0 Then
                        lstData = fillObj(dsData.Tables(0))
                    End If
                Else
                    Dim dtData = DirectCast(grvLostSales.DataSource, DataTable)
                    lstData = fillObj(dtData)
                End If

                'all ocurrences without duplicate value string
                filterData = lstData.Where(Function(da) _
                                               If(Not String.IsNullOrEmpty(da.TIMESQ), CInt(da.TIMESQ) > CInt(searchstring), False)).ToList()


                'UCase(da.TIMESQ).Trim().Contains(UCase(searchstring))
                'CInt(da.TIMESQ) > CInt(searchstring)
                If filterData.Count > 0 Then
                    Dim dtResult = ListToDataTable(filterData)
                    If dtResult IsNot Nothing Then
                        If dtResult.Rows.Count > 0 Then
                            Dim ds = New DataSet()
                            ds.Tables.Add(dtResult)
                            GetLostSalesData(Nothing, 1, Nothing, ds)
                        End If
                    End If
                Else
                    'restore grid and message 
                    Dim dsLoad = DirectCast(Session("LostSaleBck"), DataSet)
                    If dsLoad IsNot Nothing Then
                        If dsLoad.Tables(0).Rows.Count > 0 Then
                            GetLostSalesData(Nothing, 1, Nothing, dsLoad)
                        Else
                            GetLostSalesData(Nothing, 1, dsWork)
                        End If
                    Else
                        GetLostSalesData(Nothing, 1, dsWork)
                    End If
                End If
            Else
                'no action message
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ButtonAdd_Click(sender As Object, e As EventArgs) Handles ButtonAdd1.Click

    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pepe = Nothing
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Try
            Dim dsResult = DirectCast(Session("LostSaleData"), DataSet)
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then

                    Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    Dim folderPath As String = userPath & "\Lost_Sale_Data\"

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            Exit Sub
                        End If

                        Dim title As String
                        title = "Lost_Sale_Generated_by "
                        fileName = objBL.adjustDatetimeFormat(title, fileExtension)

                    End Using

                    Dim fullPath = folderPath + fileName

                    Using wb As New XLWorkbook()
                        wb.Worksheets.Add(dsResult.Tables(0), "LostSale")
                        wb.SaveAs(fullPath)

                        'Response.Clear()
                        'Response.Buffer = True
                        'Response.Charset = ""
                        'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        'Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx")
                        'Using MyMemoryStream As New MemoryStream()
                        '    wb.SaveAs(MyMemoryStream)
                        '    MyMemoryStream.WriteTo(Response.OutputStream)
                        '    Response.Flush()
                        '    Response.End()
                        'End Using
                    End Using

                    If File.Exists(fullPath) Then

                        Dim methodMessage = "The template document will be downloaded to your documents folder"
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

    Protected Sub btnPdf_Click(sender As Object, e As EventArgs) Handles btnPdf.Click
        Dim exMessage As String = Nothing
        Try
            'Dim dsResult = DirectCast(Session("LostSaleData"), DataSet)

            Dim dtGrid = DirectCast(grvLostSales.DataSource, DataTable)

            If dtGrid IsNot Nothing Then
                If dtGrid.Rows.Count > 0 Then
                    exportpdf(dtGrid)
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim exMessage As String = Nothing
        Dim searchstring As String = Trim(txtSearch.Text)
        Dim filterData = New List(Of LostSales)()
        Dim lstData = New List(Of LostSales)()
        Dim dsWork As DataSet = New DataSet()
        Try
            If searchstring.Equals("Search...") Or String.IsNullOrEmpty(searchstring) Then
                'Dim dsDataRecover = If((DirectCast(Session("LostSaleBck"), DataSet)) IsNot Nothing, DirectCast(Session("LostSaleBck"), DataSet),
                '                            If(GetLostSalesData(Nothing, 0, dsWork) > 0, dsWork, Nothing))
                'Dim dsDataRecover = If((DirectCast(Session("LostSaleData"), DataSet)) IsNot Nothing, DirectCast(Session("LostSaleData"), DataSet),
                '                            If(GetLostSalesData(Nothing, 0, dsWork) > 0, dsWork, Nothing))
                'If dsWork Is Nothing Then
                '    'message
                '    Exit Sub
                'End If
                'GetLostSalesData(Nothing, 1, Nothing, dsDataRecover)
                Session("flagVnd") = "4"
                ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

            Else
                Dim dsData = getDataSource(True)
                dsData = If((DirectCast(Session("LostSaleData"), DataSet)) IsNot Nothing, DirectCast(Session("LostSaleData"), DataSet), Nothing)
                If dsData IsNot Nothing Then
                    If dsData.Tables(0).Rows.Count > 0 Then
                        lstData = fillObj(dsData.Tables(0))
                    End If
                Else
                    Dim dtData = DirectCast(grvLostSales.DataSource, DataTable)
                    lstData = fillObj(dtData)
                End If

                'all ocurrences without duplicate value string
                filterData = lstData.Where(Function(da) _
                                               If(Not String.IsNullOrEmpty(da.IMPTN), UCase(da.IMPTN).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMDSC), UCase(da.IMDSC).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMDS2), UCase(da.IMDS2).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMDS3), UCase(da.IMDS3).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.TQUOTE), UCase(da.TQUOTE).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.TIMESQ), UCase(da.TIMESQ).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.NCUS), UCase(da.NCUS).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.VENDOR), UCase(da.VENDOR).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMPRC), UCase(da.IMPRC).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.F20), UCase(da.F20).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.FOEM), UCase(da.FOEM).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMPC1), UCase(da.IMPC1).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.CATDESC), UCase(da.IMCATA).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMPC2), UCase(da.IMPC2).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.MINDSC), UCase(da.MINDSC).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.VENDORNAME), UCase(da.VENDORNAME).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.PAGENT), UCase(da.PAGENT).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.PROJECT), UCase(da.PROJECT).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.PROJSTATUS), UCase(da.PROJSTATUS).Trim().Contains(UCase(searchstring)), False)
                                               ).ToList()

                If filterData.Count > 0 Then
                    Dim dtResult = ListToDataTable(filterData)
                    If dtResult IsNot Nothing Then
                        If dtResult.Rows.Count > 0 Then
                            Dim ds = New DataSet()
                            ds.Tables.Add(dtResult)
                            GetLostSalesData(Nothing, 1, Nothing, ds)
                        End If
                    End If
                Else
                    'restore grid and message 
                    Dim dsLoad = DirectCast(Session("LostSaleBck"), DataSet)
                    If dsLoad IsNot Nothing Then
                        If dsLoad.Tables(0).Rows.Count > 0 Then
                            GetLostSalesData(Nothing, 1, Nothing, dsLoad)
                        Else
                            GetLostSalesData(Nothing, 1, dsWork)
                        End If
                    Else
                        GetLostSalesData(Nothing, 1, dsWork)
                    End If
                End If


#Region "UnUsed Code"

                ' Dim filtered = New List(Of LostSales)()
                'Dim duplicatesMix = New List(Of LostSales)()
                'Dim intersecValues As IEnumerable(Of LostSales)()

                'Dim id1 As Integer() = {44, 26, 92, 30, 71, 38}
                'Dim id2 As Integer() = {39, 59, 83, 47, 26, 4, 30}

                'Dim both As IEnumerable(Of Integer) = id1.Intersect(id2)


                'Dim duplicates = lstData.Where(Function(x) lstData.Where(Function(y) x.IMDS3.Trim() = y.IMDS3.Trim() And y.IMDS3.Trim().Contains(searchstring)).Count() > 1).Distinct()
                'Dim duplicatesMinDesc = lstData.Where(Function(x) lstData.Where(Function(y) x.MINDSC.Trim() = y.MINDSC.Trim() And y.MINDSC.Trim().Contains(searchstring)).Count() > 1).Distinct()
                'Dim duplicates2 = lstData.Where(Function(x) x.IMDS3.Trim().Contains(searchstring)).Count() > 1

                'all duplicate value string
                'Dim duplicatesMix = lstData.Where(Function(x) _
                '                                      lstData.Where(Function(y) x.IMPTN.Trim() = y.IMPTN.Trim() And y.IMPTN.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.IMDSC.Trim() = y.IMDSC.Trim() And y.IMDSC.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.IMDS2.Trim() = y.IMDS2.Trim() And y.IMDS2.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.IMDS3.Trim() = y.IMDS3.Trim() And y.IMDS3.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.TQUOTE.Trim() = y.TQUOTE.Trim() And y.TQUOTE.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.TIMESQ.Trim() = y.TIMESQ.Trim() And y.TIMESQ.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.NCUS.Trim() = y.NCUS.Trim() And y.NCUS.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.VENDOR.Trim() = y.VENDOR.Trim() And y.VENDOR.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.F20.Trim() = y.F20.Trim() And y.F20.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.FOEM.Trim() = y.FOEM.Trim() And y.FOEM.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.IMPC1.Trim() = y.IMPC1.Trim() And y.IMPC1.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.IMCATA.Trim() = y.IMCATA.Trim() And y.IMCATA.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.IMPC2.Trim() = y.IMCATA.Trim() And y.IMCATA.Trim().Contains(searchstring)).Count() > 1 _
                '                                      Or lstData.Where(Function(y) x.MINDSC.Trim() = y.MINDSC.Trim() And y.MINDSC.Trim().Contains(searchstring)).Count() > 1
                '                                  ).Distinct()

                ''get duplicate objects
                'Dim lstDupliMix = duplicatesMix.ToList()
                'Dim intersecValues = filterData.Intersect(duplicatesMix).ToList()

                ''remove duplicate objects
                'Dim lstIntersec = intersecValues.ToList()
                'lstDupliMix.RemoveAll(Function(item) lstIntersec.Contains(item))

                ''merge founded objects
                'Dim lstFilterData = filterData.ToList()
                'lstFilterData.AddRange(lstDupliMix)
                ''filterData.AddRange(lstDupliMix)

#End Region
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub lnkReloadBack_Click(sender As Object, e As EventArgs) Handles lnkReloadBack.Click
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("LostSaleBck"), DataSet)
            Session("LostSaleData") = dsData

            ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

            Session("ItemCounts") = (DirectCast(Session("LostSaleData"), DataSet)).Tables(0).Rows.Count
            'dsData.Tables(0).Rows.Count
            grvLostSales.DataSource = dsData
            grvLostSales.DataBind()
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub lnkReloadGrid_Click(sender As Object, e As EventArgs) Handles lnkReloadGrid.Click
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
            ddlVndNameFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlVndNameFoot"), String)), ddlVndNameFoot.Items.IndexOf(ddlVndNameFoot.Items.FindByText(DirectCast(Session("flagDdlVndNameFoot"), String))), 0)
            ddlCategoryFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlCategoryFoot"), String)), ddlCategoryFoot.Items.IndexOf(ddlCategoryFoot.Items.FindByText(DirectCast(Session("flagDdlCategoryFoot"), String))), 0)
            ddlMajorFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlMajorFoot"), String)), ddlMajorFoot.Items.IndexOf(ddlMajorFoot.Items.FindByText(DirectCast(Session("flagDdlMajorFoot"), String))), 0)
            ddlWLFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlWLFoot"), String)), ddlWLFoot.Items.IndexOf(ddlWLFoot.Items.FindByText(DirectCast(Session("flagDdlWLFoot"), String))), 0)
            ddlSaleLast12Foot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlSaleLast12Foot"), String)), ddlSaleLast12Foot.Items.IndexOf(ddlSaleLast12Foot.Items.FindByText(DirectCast(Session("flagDdlSaleLast12Foot"), String))), 0)

            grvLostSales.DataSource = dsData
            grvLostSales.DataBind()
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "Textbox"

    Protected Sub tqId_TextChanged(sender As Object, e As EventArgs) Handles tqId.TextChanged
        unselectRadios()
    End Sub

#End Region

#Region "Checkbox and radio gridview"

    Protected Function GetCheckboxesDisp() As Dictionary(Of String, String)

        Dim lstPartsToWL As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim exMessage As String = Nothing
        Try
            Dim checkAll As CheckBox = grvLostSales.HeaderRow.FindControl("chkAll")
            If checkAll.Checked Then
                For Each gvr As GridViewRow In grvLostSales.Rows
                    Dim userid As String = If(Not String.IsNullOrEmpty(gvr.Cells(12).Text) And gvr.Cells(12).Text <> "&nbsp;", gvr.Cells(12).Text, "N/A")
                    lstPartsToWL.Add(Trim(gvr.Cells(2).Text), userid)
                    'lstPartsToWL.Add(Trim(gvr.Cells(2).Text))
                Next
            Else
                For Each gvr As GridViewRow In grvLostSales.Rows
                    Dim Check As CheckBox = gvr.FindControl("chkSingleAdd")
                    If Check.Checked Then
                        Dim userid As String = If(Not String.IsNullOrEmpty(gvr.Cells(12).Text) And gvr.Cells(12).Text <> "&nbsp;", gvr.Cells(12).Text, "N/A")
                        lstPartsToWL.Add(Trim(gvr.Cells(2).Text), userid)
                    End If
                Next
            End If

            Return lstPartsToWL
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Sub chkAll_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            'fillcell1("", 1)

            'Dim Repo As CheckBox = (CheckBox)((Control)sender).FindControl("chkRepo");
            Dim chkAll As CheckBox = DirectCast(sender, CheckBox)

            For Each item As GridViewRow In grvLostSales.Rows
                Dim myControl As CheckBox = CType(item.FindControl(("chkSingleAdd")), CheckBox)
                Dim chk As CheckBox = CType(myControl, CheckBox)
                chk.Checked = If(chkAll.Checked = True, True, False)
            Next
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "iTextSharp"

    Private Sub exportpdf(dtResult As DataTable)
        Dim exMessage As String = Nothing

        Try
            'creating document object
            Dim MS As MemoryStream = New MemoryStream()
            Dim rec As iTextSharp.text.Rectangle = New iTextSharp.text.Rectangle(PageSize.A3)
            rec.Rotate()
            rec.HasBorder(0)
            rec.BackgroundColor = New BaseColor(System.Drawing.Color.Red)

            Dim doc As Document = New Document(rec, 0, 0, 0, 0)
            doc.SetPageSize(iTextSharp.text.PageSize.A3.Rotate())

            'doc.SetMargins(0F, 0F, 0F, 0F)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, MS)
            doc.Open()

            'Creating paragraph for header 
            Dim bfntHead As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
            Dim fntHead As iTextSharp.text.Font = New iTextSharp.text.Font(bfntHead, 10, 1, iTextSharp.text.BaseColor.WHITE)
            Dim prgHeading As Paragraph = New Paragraph()
            prgHeading.Alignment = Element.ALIGN_LEFT
            prgHeading.Add(New Chunk("Dynamic Report PDF".ToUpper(), fntHead))
            doc.Add(prgHeading)

            'Adding paragraph for report generated by 
            Dim prgGeneratedBY As Paragraph = New Paragraph()
            Dim btnAuthor As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
            Dim fntAuthor As iTextSharp.text.Font = New iTextSharp.text.Font(btnAuthor, 8, 2, iTextSharp.text.BaseColor.BLUE)
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT
            'prgGeneratedBY.Add(New Chunk("Report Generated by : ASPArticles", fntAuthor));  
            'prgGeneratedBY.Add(New Chunk("\nGenerated Date : " + DateTime.Now.ToShortDateString(), fntAuthor));  
            doc.Add(prgGeneratedBY)

            'Adding a line  
            Dim p As Paragraph = New Paragraph(New Chunk(New iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 1)))
            doc.Add(p)

            'Adding line break  
            doc.Add(New Chunk("\n", fntHead))

            'Adding  PdfPTable  
            Dim table As PdfPTable = New PdfPTable(dtResult.Columns.Count)

            For i = 0 To dtResult.Columns.Count - 1

                Dim cellText As String = Server.HtmlDecode(dtResult.Columns(i).ColumnName)
                Dim cell As PdfPCell = New PdfPCell()
                cell.Phrase = New Phrase(cellText, New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1, New BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"))))
                cell.BackgroundColor = New BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFCD45"))
                'cell.Phrase = New Phrase(cellText, New Font(Font.FontFamily.TIMES_ROMAN, 10, 1, New BaseColor(grdStudent.HeaderStyle.ForeColor)));  
                'cell.BackgroundColor = New BaseColor(grdStudent.HeaderStyle.BackColor);  

                'cell.HorizontalAlignment = Element.ALIGN_CENTER
                'cell.PaddingBottom = 5
                table.AddCell(cell)
            Next

            For i = 0 To dtResult.Rows.Count - 1
                For j = 0 To dtResult.Columns.Count - 1
                    table.AddCell(dtResult.Rows(i)(j).ToString())
                Next
            Next

            doc.Add(table)
            doc.Close()

            Dim name = "Lost_Sale_Data"
            Response.ContentType = "application/octet-stream"
            Response.AddHeader("Content-Disposition", "attachment; filename=" + name + "_" + DateTime.Now.ToString() + ".pdf")
            Response.Clear()
            Response.BinaryWrite(MS.ToArray())
            Response.End()

            'Dim result As Byte() = MS.ToArray()
            'Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

End Class
