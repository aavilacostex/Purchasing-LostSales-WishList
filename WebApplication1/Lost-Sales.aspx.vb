Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading
Imports ClosedXML.Excel
Imports CTPWEB.DTO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports Newtonsoft
Imports WebGrease

Public Class Lost_Sales
    Inherits System.Web.UI.Page

    Dim total As Integer = 0

    Dim totaRowsCount As Integer = 0
    Dim pageSizeCustom As Integer = 0

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing
    Delegate Sub executeFullQuery()

    Dim objLog = New Logs()

    'Dim _sortDirection As String = "ASC"

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim exMessage As String = Nothing
        Dim sel As Integer = -1
        Dim url As String = Nothing
        'Session("sortDirection") = "0"
        Try

            If Session("userid") Is Nothing Then
                url = String.Format("Login.aspx?data={0}", "Session Expired!")
                Response.Redirect(url, False)
            Else
                Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                hdWelcomeMess.Value = lblUserLogged.Text
            End If

            If Not IsPostBack Then

                Dim flag = GetAccessByUsers(sel)
                If Not flag Then
                    If sel = 0 Then
                        Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User: " + usr, " User is not authorized to access to LS. Time: " + DateTime.Now.ToString())
                        Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/Wish-List", True)
                    ElseIf sel = 1 Then
                        Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, Nothing, "There is not an user detected tryng to access to LS. Time: " + DateTime.Now.ToString())
                        Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/", True)
                    End If
                Else

                    load_Combos()
                    'set to default when beggining
                    ddlVendAssign.SelectedIndex = 1
                    tqId.Text = Nothing
                    unselectRadios()
                    createDdlDictionary()
                    'createDdlFootDictionary()

                    'load by default
                    Session("PageSize") = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PageSize")), ConfigurationManager.AppSettings("PageSize"), "1000")
                    Session("PageAmounts") = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PageAmounts")), ConfigurationManager.AppSettings("PageAmounts"), "10")
                    Session("VendorsAccepted") = ConfigurationManager.AppSettings("itemCategories")
                    Session("currentPage") = 0

                    Session("curCategory") = Nothing
                    Session("curVndName") = Nothing
                    Session("curMajor") = Nothing
                    Session("curWL") = Nothing
                    Session("curSaleLast12") = Nothing
                    Session("AllSelected") = Nothing

                    Dim strPagValues = getLimit()

                    'default values for filter data
                    Dim timesQDefault As String = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("timesQuoteDefault")), ConfigurationManager.AppSettings("timesQuoteDefault"), "50")
                    Session("TimesQuote") = timesQDefault
                    Dim vndSelDefault As String = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("vndSelectionDefault")), ConfigurationManager.AppSettings("vndSelectionDefault"), "2")
                    Session("flagVnd") = vndSelDefault

                    'search first 5000 records
                    Dim dsResult As DataSet = New DataSet()
                    GetLostSalesData(strPagValues, 0, dsResult)

                    getLSData(dsResult, CInt(timesQDefault), vndSelDefault) 'prepare the session variables with default values

                    'executing 
                    'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

                    Session("EventRaised") = False
                    'Session("flagDdlCategoryFoot") = Nothing
                    'Session("flagDdlVndNameFoot") = Nothing

                    Session("ddlCategoryIndex") = "-1"
                    Session("ddlVendorNameIndex") = "-1"
                    Session("ddlWlistIndex") = "-1"
                    Session("ddlMajorIndex") = "-1"
                    Session("ddlSaleLast12Index") = "-1"

                    Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                    loadData(dsData)

                    Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                    writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged in Lost Sales: " + usr, "Login at time: " + DateTime.Now.ToString())

#Region "Thread"

                    launchSecondaryThread()

#End Region

                    'Load_Combos_inGrid()

                    'objLog.writeLog(strLogCadenaCabecera, objLog.ToString, ex.Message, ex.ToString())

                End If

            Else
                Session("EventRaised") = True

                'Dim flagLoad = If(DirectCast(Session("LostSaleBck"), DataSet) IsNot Nothing, True, False)
                'If flagLoad Then
                '    Load_Combos_inGrid()
                'End If

                'getDataSource()
                'Dim flagLoad = If(DirectCast(Session("LostSaleBck"), DataSet) IsNot Nothing, True, False)
                'If flagLoad Then
                '    Load_Combos_inGrid()
                'End If
                'checkInnerDropDownCreated()
                'GetLostSalesData("", 1, Nothing, DirectCast(Session("LostSaleData"), DataSet))
                Dim controlName As String = Page.Request.Params("__EVENTTARGET")
                If (LCase(controlName).Contains("lnkdetails") Or LCase(controlName).Contains("lbsingleadd") Or LCase(controlName).Contains("buttonadd") Or
                    LCase(controlName).Contains("grvlostsales") Or LCase(controlName).Contains("ddluser2")) Then
                    ') _
                    'Or hiddenId3.Value = "2"
                    Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                    loadData(dsOut)
                    setDefaultValues(dsOut)
                ElseIf LCase(controlName).Contains("updatepnl") Or LCase(controlName).Contains("tqr") Or LCase(controlName).Contains("chk") Then
                    Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                    loadData(dsOut)
                    setDefaultValues(dsOut)
                ElseIf LCase(controlName).Contains("rd") Then
                    Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                    loadData(dsOut)
                    'Dim ddlName = GetDdlByRd(controlName)
                    'Dim ddlTrig As DropDownList = DirectCast(Me.Form.FindControl(ddlName), DropDownList)
                    'executesDropDownList(ddlTrig)
                    setDefaultValues(dsOut)
                ElseIf ((LCase(controlName).Contains("ddl"))) Then
                    Dim ddlTrig As DropDownList = DirectCast(Me.Form.FindControl(controlName), DropDownList)
                    executesDropDownList(ddlTrig)
                Else
                    Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                    loadData(dsOut)
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "An Exception occurs: " + ex.Message + " for the user: " + usr, " at time: " + DateTime.Now.ToString())
        End Try
        'lblMyLabel.Attributes.Add("onclick","javascript:alert('ALERT ALERT!!!')")
    End Sub

#End Region

#Region "Thread methods"

    Private Function worker_Thread()
        Dim result As Integer = -1
        Dim dsResult As DataSet = New DataSet()
        Try
            Threading.Thread.Sleep(8000)
            Dim vendorsOk = DirectCast(Session("VendorsAccepted"), String)
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                result = objBL.GetLostSalesData("", 0, dsResult, vendorsOk)
                Session("LostSaleBck") = dsResult
                Session("LostSaleBckCount") = dsResult.Tables(0).Rows.Count

                Dim vndSel = DirectCast(Session("flagVnd"), String)
                Dim timesQ = DirectCast(Session("TimesQuote"), String)

                getLSData(dsResult, CInt(timesQ), vndSel) 'prepare the session variables with default values

            End Using

        Catch ex As Exception
            Dim eee = ex.Message
        End Try
    End Function

    'Private Function worker_DoWork(worker As BackgroundWorker)
    '    Dim result As Integer = -1
    '    Dim dsResult As DataSet = New DataSet()
    '    Try
    '        Threading.Thread.Sleep(15000)
    '        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
    '            result = objBL.GetLostSalesData("", 0, dsResult)
    '            Session("LostSaleBck") = dsResult
    '            Session("LostSaleBckCount") = dsResult.Tables(0).Rows.Count
    '        End Using

    '    Catch ex As Exception
    '        Dim eee = ex.Message
    '    End Try
    'End Function

    'Private Function worker_WorkerCompleted(worker As BackgroundWorker)
    '    Try
    '        Dim dsCheck = DirectCast(Session("LostSaleBck"), DataSet)
    '        Dim count = dsCheck.Tables(0).Rows.Count()
    '        Dim pepe = "aa"
    '    Catch ex As Exception
    '        Dim eee = ex.Message
    '    End Try
    'End Function

    'Public Sub Appication_End(sender As Object, e As EventArgs)
    '    Try
    '        Dim uuuuu = "7878"
    '    Catch ex As Exception

    '    End Try
    'End Sub

#End Region

#Region "Generics"

    Public Function IsFileinUse(file As FileInfo) As Boolean
        Dim exMessage As String = Nothing
        Dim opened As Boolean = False
        Dim myStream As FileStream = Nothing
        Try
            myStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
        Catch ex As Exception

            If TypeOf ex Is IOException AndAlso IsFileLocked(ex) Then
                System.IO.File.Delete(file.Name)
                opened = False
            Else
                opened = True
            End If
        Finally
            If myStream IsNot Nothing Then
                myStream.Close()
            End If
        End Try
        Return opened
    End Function

    Private Shared Function IsFileLocked(exception As Exception) As Boolean
        Dim errorCode As Integer = Marshal.GetHRForException(exception) And ((1 << 16) - 1)
        Return errorCode = 32 OrElse errorCode = 33
    End Function

    Protected Sub lnkLogout_Click() Handles lnkLogout.Click
        Try
            FormsAuthentication.SignOut()
            Session.Abandon()
            coockieWork()
            Session("UserLoginData") = Nothing
            FormsAuthentication.RedirectToLoginPage()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub coockieWork()
        Try

            Dim cookie1 As HttpCookie = New HttpCookie(FormsAuthentication.FormsCookieName, "")
            cookie1.HttpOnly = True
            cookie1.Expires = DateTime.Now.AddYears(-1)
            Response.Cookies.Add(cookie1)

        Catch ex As Exception

        End Try
    End Sub

    Private Function GetDdlByRd(controlName As String) As String
        Dim lstDdls As List(Of String) = New List(Of String)()
        Try
            lstDdls.Add("ddlSaleLast12")
            lstDdls.Add("ddlWishList")
            lstDdls.Add("ddlMajor")
            lstDdls.Add("ddlVendorName")
            lstDdls.Add("MainContent_ddlCategory")

            ''For Each ddl As String In lstDdls
            If controlName.Contains("rdCategory") Then
                Return lstDdls(4).ToString()
            ElseIf controlName.Contains("rdVndName") Then
                Return lstDdls(3).ToString()
            ElseIf controlName.Contains("rdMajor") Then
                Return lstDdls(2).ToString()
            ElseIf controlName.Contains("rdWL") Then
                Return lstDdls(1).ToString()
            ElseIf controlName.Contains("rdLast12") Then
                Return lstDdls(0).ToString()
            End If
            'Next
        Catch ex As Exception

        End Try
    End Function

    Public Function GetAccessByUsers(ByRef sel As Integer) As Boolean
        Dim optionSelection As String = Nothing
        Dim user As String = Nothing
        Dim flag As Boolean = False
        Try
            Dim validUsers = ConfigurationManager.AppSettings("validUsersForWeb")

            'Dim args As String() = Environment.GetCommandLineArgs()
            'Dim argumentsJoined = String.Join(".", args)

            'Dim arrayArgs As String() = argumentsJoined.Split(".")
            'optionSelection = UCase(arrayArgs(3).ToString().Replace(",", ""))
            'user = UCase(arrayArgs(2).ToString().Replace(",", ""))

            user = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "NA")
            If Not user.Equals("NA") Then
                If LCase(validUsers.Trim()).Contains(LCase(user.Trim())) Then
                    'Session("userid") = user
                    flag = True
                    Return flag
                Else
                    'test
                    'Session("userid") = ConfigurationManager.AppSettings("authorizeTestUser")
                    'test
                    sel = 0
                    Return False
                    'Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/Wish-List", True)
                End If
            Else
                sel = 1
                Return False
                'Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/", True)
            End If

        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return flag
        End Try
    End Function

    Public Sub executesDropDownList(ddl As DropDownList)
        Dim exMessage As String = " "
        Try
            If ddl.ID = "ddlCategory" Then
                ddlCategory.SelectedIndex = If(Not String.IsNullOrEmpty(hdCategory.Value), CInt(hdCategory.Value) + 1, 0)
                ddlCategory_SelectedIndexChanged(ddl, Nothing)
            ElseIf ddl.ID = "ddlVendorName" Then
                ddlVendorName.SelectedIndex = If(Not String.IsNullOrEmpty(hdVendorName.Value), CInt(hdVendorName.Value) + 1, 0)
                ddlVendorName_SelectedIndexChanged(ddl, Nothing)
            ElseIf ddl.ID = "ddlMajor" Then
                ddlMajor.SelectedIndex = If(Not String.IsNullOrEmpty(hdMajor.Value), CInt(hdMajor.Value) + 1, 0)
                ddlMajor_SelectedIndexChanged(ddl, Nothing)
            ElseIf ddl.ID = "ddlWishList" Then
                ddlWishList.SelectedIndex = If(Not String.IsNullOrEmpty(hdWishList.Value), CInt(hdWishList.Value) + 1, 0)
                ddlWishList_SelectedIndexChanged(ddl, Nothing)
            ElseIf ddl.ID = "ddlSaleLast12" Then
                ddlSaleLast12.SelectedIndex = If(Not String.IsNullOrEmpty(hdSaleLast12.Value), CInt(hdSaleLast12.Value) + 1, 0)
                ddlSaleLast12_SelectedIndexChanged(ddl, Nothing)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Sub updatePagerSettings(grv As GridView)
        Try
            Dim strTotal = DirectCast(Session("ItemCounts"), String)
            Dim strNumberOfPages = DirectCast(Session("PageAmounts"), Integer).ToString()
            Dim strCurrentPage = ((DirectCast(Session("currentPage"), Integer))).ToString()

            Dim strGrouping = String.Format("Showing {0} to {1} of {2} entries ", strCurrentPage, strNumberOfPages, strTotal)
            lblGrvGroup.Text = strGrouping

            Dim sortCell As New HtmlTableCell()
            sortCell.Controls.Add(lblGrvGroup)

            Dim row1 As HtmlTableRow = New HtmlTableRow
            row1.Cells.Add(sortCell)
            ndtt.Rows.Add(row1)

            Dim pepe = grv.FooterRow
            Dim ppe = grv.PagerTemplate

            Dim BottomPagerRow = grv.BottomPagerRow
            BottomPagerRow.Cells(0).Controls.AddAt(0, ndtt)

            'For Each item As GridViewRow In grv.Rows
            '    If item.RowType = DataControlRowType.Pager Then
            '        item.Cells(0).Controls.AddAt(0, ndtt)
            '    End If
            'Next

            'e.Row.Cells(0).Controls.AddAt(0, ndtt)
        Catch ex As Exception
            Dim exMessage = ex.Message
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Function updateLostSaleGridView(partno As String, Optional flag As Boolean = False) As Integer
        Dim lstData = New List(Of LostSales)()
        Dim filterData = New List(Of LostSales)()
        Dim result As Integer = -1
        Try
            Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
            lstData = fillObj(dsData.Tables(0))

            ' tiene que ser com lsdata no con filter data para poder luego cargar todas referencias con ecepcipon de la recien eliminada.

            For Each ls As LostSales In lstData
                If LCase(ls.IMPTN.Trim()) = LCase(partno.Trim()) Then
                    filterData.Add(ls)
                End If
            Next

            Dim count1 = filterData.Count()

            'Dim lstSelected = lstData.AsEnumerable().Where(Function(da) LCase(da.IMPTN).Trim() = LCase(partno).Trim()).ToList()
            If filterData.Count = 1 Then
                filterData.Remove(filterData.Single(Function(da) LCase(da.IMPTN).Trim() = LCase(partno).Trim()))
            Else
                filterData.Remove(filterData.Single(Function(da) (LCase(da.IMPTN).Trim() = LCase(partno).Trim()) And (da.PrPech.Trim() <> "")))
            End If
            'Dim pp = lstData.Single(Function(da) LCase(da.IMPTN).Trim() = LCase(partno).Trim())
            Dim count2 = filterData.Count()

            If count1 > count2 Then
                Dim dtResult = ListToDataTable(filterData)
                If dtResult IsNot Nothing Then
                    If dtResult.Rows.Count > 0 Then
                        Dim ds = New DataSet()
                        ds.Tables.Add(dtResult)
                        Session("LostSaleData") = ds

                        If Not flag Then
                            loadData(ds)
                        End If
                        result = 0
                    Else
                        If Not flag Then
                            SendMessage("There is an error updatting the gridview!", messageType.Error)
                        End If
                    End If
                Else
                    If Not flag Then
                        SendMessage("There is an error updatting the gridview!", messageType.Error)
                    End If
                End If
            Else
                If Not flag Then
                    SendMessage("There is an error updatting the gridview!", messageType.Error)
                End If
            End If
            Return result

        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return result
        End Try
    End Function

    Public Sub launchSecondaryThread()
        Dim exMessage As String = ""
        Try
            Dim tt As Thread = New Thread(AddressOf worker_Thread)
            tt.Name = "Secondary tt"
            'tt.IsBackground = True
            tt.Start()
        Catch ex As Exception
            exMessage = ex.Message
        End Try
    End Sub

    Public Function getLimit() As String
        Dim exMessage As String = Nothing
        Try
            Dim mergePagValues As List(Of String) = New List(Of String)()
            mergePagValues.Add(DirectCast(Session("currentPage"), Integer).ToString())
            mergePagValues.Add(CInt(DirectCast(Session("PageSize"), String)))
            Dim strPagValues = mergePagValues(0) + "," + mergePagValues(1)
            Session("mergePagiValues") = strPagValues
            Return strPagValues
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return Nothing
        End Try

    End Function

    Public Function getActiveDdlAction() As String
        Dim exMessage As String = Nothing
        Try
            Dim dctValues = DirectCast(Session("gridDdlDictionary"), Dictionary(Of String, String))
            Dim catActive = DirectCast(Session("ddlCategoryIndex"), String)
            Dim vndActive = DirectCast(Session("ddlVendorNameIndex"), String)
            Dim wlActive = DirectCast(Session("ddlWlistIndex"), String)
            Dim majActive = DirectCast(Session("ddlMajorIndex"), String)
            Dim sal12Active = DirectCast(Session("ddlSaleLast12Index"), String)

            Dim selectedDdl As String = Nothing
            Dim selectedAction As String = Nothing

            If catActive <> "0" And catActive <> "-1" Then
                selectedDdl = ddlCategory.ID
            ElseIf vndActive <> "0" And vndActive <> "-1" Then
                selectedDdl = ddlVendorName.ID
            ElseIf wlActive <> "0" And wlActive <> "-1" Then
                selectedDdl = ddlWishList.ID
            ElseIf majActive <> "0" And majActive <> "-1" Then
                selectedDdl = ddlMajor.ID
            ElseIf sal12Active <> "0" And sal12Active <> "-1" Then
                selectedDdl = ddlSaleLast12.ID
            Else
                selectedDdl = Nothing
            End If

            If selectedDdl Is Nothing Then
                Return Nothing
            Else
                For Each item In dctValues
                    If LCase(selectedDdl) = LCase(item.Key) Then
                        selectedAction = item.Value
                        Exit For
                    End If
                Next
            End If

            If selectedAction Is Nothing Then
                Return Nothing
            Else
                Return selectedAction
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return "Error"
        End Try
    End Function

    Public Sub setDefaultValues(Optional ds As DataSet = Nothing)
        Dim exMessage As String = Nothing
        Try
            lblTimesQuote.Text = If((getIfCheckedQuote() = False And String.IsNullOrEmpty(tqId.Text)), DirectCast(Session("TimesQuote"), String), getStrTQouteCriteria())
            If ds IsNot Nothing Then
                lblItemsCount.Text = If(ds IsNot Nothing, ds.Tables(0).Rows.Count.ToString(), "50")
            Else
                lblItemsCount.Text = "0"
            End If
            Session("TimesQuote") = lblTimesQuote.Text
            Session("ItemCounts") = lblItemsCount.Text
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Function ddlVendAssignSimulation(ds1 As DataSet, ddlSelection As String, getSelectedQuote As String, propertyy As String) As List(Of LostSales)
        Dim exMessage As String = " "
        Dim lstSelection = New List(Of LostSales)()
        Try
            'Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
            Dim vndSel = DirectCast(Session("flagVnd"), String)

            Dim lsTemp1 = fillObj(ds1.Tables(0))
            For Each item1 In lsTemp1
                'item1.pepe = "a"
                'item1.GetType().GetProperties().Single(Function(pi) pi.Name = pepe)
                'item1.GetType().GetProperty(pepe).SetValue(item1, "Bob", Nothing)
                Dim test = item1.GetType().GetProperty(propertyy).GetValue(item1).ToString()

                If vndSel = "3" Then
                    If UCase(Trim(item1.GetType().GetProperty(propertyy).GetValue(item1).ToString())) = UCase(Trim(ddlSelection)) Then
                        lstSelection.Add(item1)
                    End If
                Else
                    If vndSel = "1" Then
                        If Not String.IsNullOrEmpty(item1.VENDOR) Then
                            Dim tq = If(String.IsNullOrEmpty(item1.TIMESQ), 0, CInt(item1.TIMESQ))
                            If tq >= CInt(getSelectedQuote) Then
                                If UCase(Trim(item1.GetType().GetProperty(propertyy).GetValue(item1).ToString())) = UCase(Trim(ddlSelection)) Then
                                    lstSelection.Add(item1)
                                End If
                            End If
                        End If
                    Else
                        If String.IsNullOrEmpty(item1.VENDOR) Then
                            Dim tq = If(String.IsNullOrEmpty(item1.TIMESQ), 0, CInt(item1.TIMESQ))
                            If tq >= CInt(getSelectedQuote) Then
                                If UCase(Trim(item1.GetType().GetProperty(propertyy).GetValue(item1).ToString())) = UCase(Trim(ddlSelection)) Then
                                    lstSelection.Add(item1)
                                End If
                            End If
                        End If
                    End If
                End If
            Next
            Return lstSelection
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return False
        End Try
    End Function

    Private Function fitSelection(Optional ddl As DropDownList = Nothing, Optional propertyy As String = Nothing) As IEnumerable(Of LostSales)
        Dim exMessage As String = Nothing
        Try
            'Dim dsAllData As DataSet = DirectCast(Session("LostSaleBck"), DataSet)
            Dim dsAllData As DataSet = DirectCast(Session("LostSaleData"), DataSet)
            Dim tquote = DirectCast(Session("TimesQuote"), String)
            Dim vndSel = DirectCast(Session("flagVnd"), String)
            Dim lstDefNames = DirectCast(Session("gridDdlFootDictionary"), Dictionary(Of String, String))
            Dim dictNames = New Dictionary(Of String, String)()

            Dim current = ddl.ID
            For Each item In lstDefNames
                If item.Key <> current Then
                    dictNames.Add(item.Key, item.Value)
                End If
            Next

            'Dim newData = New List(Of LostSales)()

            If dsAllData IsNot Nothing Then

                Dim lstAllData = fillObj(dsAllData.Tables(0))
                Dim iteration1 = lstAllData.AsEnumerable().Where(Function(val1) If(vndSel = "3", val1.VENDOR, If(vndSel = "1", Not String.IsNullOrEmpty(val1.VENDOR), String.IsNullOrEmpty(val1.VENDOR))) _
                                                                     And CInt(val1.TIMESQ) >= CInt(tquote))

                'val.GetType().GetProperty(propertyy).GetValue(val).ToString())

                If iteration1 IsNot Nothing Then
                    If ddl.SelectedIndex <> 0 Then
                        Dim iteration2 = iteration1.AsEnumerable().Where(Function(val) val.GetType().GetProperty(propertyy).GetValue(val).ToString() = ddl.SelectedItem.ToString())
                        If iteration2 IsNot Nothing Then
                            iteration1 = iteration2
                        End If
                    End If
                Else
                    Return Nothing
                End If

                Dim ph As ContentPlaceHolder = DirectCast(Me.Master.FindControl("MainContent"), ContentPlaceHolder)
                Dim grv As GridView = DirectCast(ph.FindControl("grvLostSales"), GridView)

                For Each item2 In dictNames

                    ' BUSCAR EL FOOTER GRIDVIEW ROW Y ALLI BUSCAR EL DROPDOWNLIST
                    'var Str() = SuburbGridView.Rows.Cast < GridViewRow > ().Where(r >= ((CheckBox)r.FindControl("SuburbSelector")).checked);
                    'Dim footerRow = grv.Rows.Cast(Of GridViewRow)().Where(Function(r) r.RowType = DataControlRowType.Footer)
                    Dim footerRow = grv.FooterRow
                    Dim ddlIn As DropDownList = DirectCast(footerRow.FindControl(item2.Key), DropDownList)
                    If ddlIn IsNot Nothing Then
                        If ddlIn.SelectedIndex <> 0 Then
                            Dim iteration3 = iteration1.AsEnumerable().Where(Function(val) val.GetType().GetProperty(item2.Value).GetValue(val).ToString() = ddlIn.SelectedItem.ToString())
                            If iteration3 IsNot Nothing Then
                                iteration1 = iteration3
                            End If
                        End If
                    End If

                Next
#Region "maybe"

                'If iteration1 IsNot Nothing Then
                '    If ddlVndNameFoot.SelectedIndex <> 0 Then
                '        Dim iteration3 = iteration1.AsEnumerable().Where(Function(val) val.VENDORNAME = ddlVndNameFoot.SelectedItem.ToString())
                '        If iteration3 IsNot Nothing Then
                '            iteration1 = iteration3
                '        End If
                '    End If
                'Else
                '    Return Nothing
                'End If

                'If iteration1 IsNot Nothing Then
                '    If ddlMajorFoot.SelectedIndex <> 0 Then
                '        Dim iteration4 = iteration1.AsEnumerable().Where(Function(val) val.IMPC1 = ddlMajorFoot.SelectedItem.ToString())
                '        If iteration4 IsNot Nothing Then
                '            iteration1 = iteration4
                '        End If
                '    End If
                'Else
                '    Return Nothing
                'End If

                'If iteration1 IsNot Nothing Then
                '    If ddlWLFoot.SelectedIndex <> 0 Then
                '        Dim iteration5 = iteration1.AsEnumerable().Where(Function(val) val.WLIST = ddlWLFoot.SelectedItem.ToString())
                '        If iteration5 IsNot Nothing Then
                '            iteration1 = iteration5
                '        End If
                '    End If
                'Else
                '    Return Nothing
                'End If

                'If iteration1 IsNot Nothing Then
                '    If ddlSaleLast12Foot.SelectedIndex <> 0 Then
                '        Dim iteration6 = iteration1.AsEnumerable().Where(Function(val) val.QTYSOLD = ddlSaleLast12Foot.SelectedItem.ToString())
                '        If iteration6 IsNot Nothing Then
                '            iteration1 = iteration6
                '        End If
                '    End If
                'Else
                '    Return Nothing
                'End If

#End Region
                Return iteration1

            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Sub createDdlDictionary()

        Dim dc = New Dictionary(Of String, String)()
        dc.Add("ddlCategory", "ddlCategory_SelectedIndexChanged")
        dc.Add("ddlVendorName", "ddlVendorName_SelectedIndexChanged")
        dc.Add("ddlWishList", "ddlWishList_SelectedIndexChanged")
        dc.Add("ddlMajor", "ddlMajor_SelectedIndexChanged")
        dc.Add("ddlSaleLast12", "ddlSaleLast12_SelectedIndexChanged")

        Session("gridDdlDictionary") = dc

    End Sub

    'Public Sub createDdlFootDictionary()

    '    Dim dc = New Dictionary(Of String, String)()
    '    dc.Add("ddlVndNameFoot", "VENDORNAME")
    '    dc.Add("ddlMajorFoot", "IMPC1")
    '    dc.Add("ddlWLFoot", "WLIST")
    '    dc.Add("ddlCategoryFoot", "CATDESC")
    '    dc.Add("ddlSaleLast12Foot", "QTYSOLD")

    '    Session("gridDdlFootDictionary") = dc

    'End Sub

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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
                    'If getDataSourceDif(grvLostSales, dsDataGrid) Then
                    '    'GetLostSalesData("", 1, Nothing, dsDataGrid)
                    '    Return dsDataGrid
                    'Else
                    '    'GetLostSalesData("", 1, dsSetDataSource)
                    '    Return Nothing
                    'End If
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
#Region "NO"

                        'lblItemsCount.Text = dsLoad.Tables(0).Rows.Count.ToString()
                        'Session("ItemCounts") = dsLoad.Tables(0).Rows.Count
                        ''Session("PageAmounts") = If(((dsLoad.Tables(0).Rows.Count) / 10) Mod 2 = 0, CInt((dsLoad.Tables(0).Rows.Count) / 10), CInt((dsLoad.Tables(0).Rows.Count) / 10) + 1)
                        'lblTimesQuote.Text = If((getIfCheckedQuote() = False And String.IsNullOrEmpty(tqId.Text)), DirectCast(Session("TimesQuote"), String), getStrTQouteCriteria())
                        ''Session("TimesQuote") = lblTimesQuote.Text
                        'Session("LostSaleData") = dsLoad

                        'Dim ddl1 = ddlCategoryFoot.SelectedIndex
                        'Dim ddl2 = ddlVndNameFoot.SelectedIndex

#End Region

                        If Not String.IsNullOrEmpty(strWhere) Then
                            Dim NoRef = CInt(strWhere.Split(",")(1))

                            Dim dtCustom = New DataTable()
                            dtCustom = dsLoad.Tables(0).AsEnumerable().Take(NoRef).CopyToDataTable()

                            loadData(Nothing, dtCustom)
                            'grvLostSales.DataSource = dtCustom
                            'grvLostSales.DataBind()
                            Return NoRef
                        End If

                        loadData(dsLoad)

                        'grvLostSales.DataSource = dsLoad.Tables(0)
                        'grvLostSales.DataBind()

                        'aqui agregar else clause
                    Else
                        loadData()
                        'grvLostSales.DataSource = Nothing
                        'grvLostSales.DataBind()

                        'Dim methodMessage = "There is not results with the selected criteria. "
                        'SendMessage(methodMessage, messageType.warning)
                    End If
                Else
                    'default

                    Dim vendorsOk = DirectCast(Session("VendorsAccepted"), String)

                    result = objBL.GetLostSalesData(strWhere, flag, dsResult, vendorsOk)
                    If (result > 0 And dsResult IsNot Nothing And dsResult.Tables(0).Rows.Count > 0) Then

                        If flag = 0 Then
                            'Session("LostSaleBck") = dsResult
                            'Session("LostSaleBckCount") = dsResult.Tables(0).Rows.Count
                            'Session("ItemCounts") = dsResult.Tables(0).Rows.Count
                            'Session("PageAmounts") = If(((dsResult.Tables(0).Rows.Count) / 10) Mod 2 = 0, CInt((dsResult.Tables(0).Rows.Count) / 10), CInt((dsResult.Tables(0).Rows.Count) / 10) + 1)
                            Session("PageAmounts") = 10
                            Session("currentPage") = 1
                            'Session("firstLoad") = "1"
                        End If

                        'Dim flagLoad = If(DirectCast(Session("LostSaleBck"), DataSet) IsNot Nothing, True, False)
                        'If flagLoad Then
                        '    Load_Combos_inGrid()
                        'End If

                        'DoExcel(dsResult.Tables(0))

                        'grvLostSales.DataSource = dsResult.Tables(0)
                        'grvLostSales.DataBind()
                        'lblItemsCount.Text = dsResult.Tables(0).Rows.Count.ToString()
                        'Session("ItemCounts") = dsResult.Tables(0).Rows.Count
                        'lblTimesQuote.Text = If(getIfCheckedQuote() = False, "100+", getStrTQouteCriteria())
                        'Session("TimesQuote") = lblTimesQuote.Text
                        Session("LostSaleData") = dsResult
                        Session("LostSaleBck") = dsResult
                    End If
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return result
        End Try
    End Function

    Private Function fillObj(dt As DataTable) As List(Of LostSales)
        Dim exMessage As String = Nothing
        Dim objLosSales = Nothing
        Try

            Dim items As IList(Of LostSales) = dt.AsEnumerable() _
                .Select(Function(row) New LostSales() With {
                .IMPTN = row.Item("IMPTN").ToString(),
                .IMDSC = row.Item("IMDSC").ToString(),
                .IMDS2 = row.Item("IMDS2").ToString(),
                .IMDS3 = row.Item("IMDS3").ToString(),
                .TQUOTE = row.Item("TQUOTE").ToString(),
                .TIMESQ = If(String.IsNullOrEmpty(row.Item("TIMESQ").ToString()), "0", row.Item("TIMESQ").ToString()),
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
                .WLIST = "",
                .PROJECT = "",
                .PROJSTATUS = "",
                .PAGENT = row.Item("PAGENT").ToString(),
                .PrPech = row.Item("PrPech").ToString(),
                .TotalClients = row.Item("totalclients").ToString(),
                .TotalCountries = row.Item("totalcountries").ToString(),
                .OEMPart = row.Item("oempart").ToString()
                }).ToList()

            '.WLIST = row.Item("WLIST").ToString(),
            '    .PROJECT = row.Item("PROJECT").ToString(),
            '    .PROJSTATUS = row.Item("PROJSTATUS").ToString(),

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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return objLosSales
        End Try
    End Function

    Public Function ListToDataTableDr(ByVal _List As List(Of DataRow)) As DataSet
        Dim dt = New DataTable()
        Dim ds = New DataSet()
        Dim exMessage As String = Nothing

        Try
            dt = _List(0).Table.Clone()
            For Each item As DataRow In _List
                dt.ImportRow(item)
            Next

            'ds.Tables.RemoveAt(0)
            ds.Tables.Add(dt)

            Return ds
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return Nothing
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return Nothing
        End Try

    End Function

    Public Function GetLSBackData(partNo As String, Optional ByRef dsResult As DataSet = Nothing) As Boolean
        Dim exMessage As String = Nothing
        dsResult = New DataSet
        Dim flag As Boolean = False
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                'Dim rsData = objBL.GetLSBackData(partNo, dsResult)  -- sql method
                Dim rsData = objBL.GetLSBackData400(partNo, dsResult)
                If dsResult IsNot Nothing Then
                    If dsResult.Tables(0).Rows.Count > 0 Then
                        flag = True
                    End If
                End If
            End Using
            Return flag
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return flag
        End Try

    End Function

    Public Function UpdateLSBackData400(partNo As String, externalStatus As String, Optional user As String = Nothing) As Integer
        Dim exMessage As String = Nothing
        'dsResult = New DataSet
        Dim flag As Boolean = False
        Dim rsData As Integer = -1
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                'Dim rsData = objBL.GetLSBackData(partNo, dsResult)  -- sql method
                rsData = objBL.UpdateLSBackData400(partNo, externalStatus, user)
                If rsData <= 0 Then
                    'handle the updation success
                End If
            End Using
            Return rsData
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return rsData
        End Try
    End Function

    Public Function SaveLSItemInProcess(Optional externalStatus As String = Nothing) As Integer
        Dim objLS1 As LostSales = New LostSales()
        Dim lstObjLS As List(Of LostSales) = New List(Of LostSales)()
        Dim dctLS As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Try
            Dim dsPrepare As DataSet = New DataSet()
            Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
            Dim dtPrepare As DataTable = dsData.Tables(0).Clone()


            dctLS = DirectCast(Session("dctSelectedParts"), Dictionary(Of String, String))

            For Each item In dctLS
                Dim part As String = item.Key

                For Each dw As DataRow In dsData.Tables(0).Rows
                    If LCase(dw.Item("IMPTN").ToString().Trim()) = LCase(part.Trim()) Then
                        dtPrepare.ImportRow(dw)
                        'dtPrepare.Rows.Add(dw)
                        Exit For
                    End If
                Next

            Next

            dsPrepare.Tables.Add(dtPrepare)
            lstObjLS = fillObj(dsPrepare.Tables(0))

            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                For Each item1 In lstObjLS
                    'Dim result = objBL.SaveLSItemInProcess(item1) -- save lostsale data sql server
                    Dim result = objBL.InsertLSBackData400(item1, externalStatus)
                    'status when add to wish list
                    If result <= 0 Then
                        'handle the insertion success
                    End If
                Next
            End Using
        Catch ex As Exception

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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
                    Session("TimesQuote") = strCriteria
                End If
            End If
            Return strCriteria
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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

                Dim dtData = New DataTable()
                dtData.Columns.Add("ID")
                dtData.Columns.Add("CATDESC")
                dtData.Columns(0).AutoIncrement = True

                For Each item As LostSales In sortedList
                    Dim R As DataRow = dtData.NewRow
                    R("CATDESC") = item.CATDESC
                    dtData.Rows.Add(R)
                Next

                'If lstObjData.Count > 0 Then
                '    Dim dtData = ListToDataTable(sortedList)
                If dtData IsNot Nothing Then
                    If dtData.Rows.Count > 0 Then
                        LoadingDropDownList(ddlCategory, dtData.Columns("CATDESC").ColumnName, dtData.Columns("ID").ColumnName, dtData, True, "--")
                    End If
                    'End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlCategory.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
                            LoadingDropDownList(ddlMajor, dtData.Columns("IMPC1").ColumnName,
                                                dtData.Columns("IMPTN").ColumnName, dtData, True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlMajor.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
                            LoadingDropDownList(ddlWishList, dtData.Columns("WLIST").ColumnName,
                                                dtData.Columns("IMPTN").ColumnName, dtData, True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlWishList.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub fill_VndName(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim lstObjData = New List(Of LostSales)()

        Dim dctCategories As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim result As Integer = 0
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

                Dim dtData = New DataTable()
                dtData.Columns.Add("ID")
                dtData.Columns.Add("VENDORNAME")
                dtData.Columns(0).AutoIncrement = True

                For Each item As LostSales In sortedList
                    Dim R As DataRow = dtData.NewRow
                    R("VENDORNAME") = item.VENDORNAME
                    dtData.Rows.Add(R)
                Next

                'If lstObjData.Count > 0 Then
                '    Dim dtData = ListToDataTable(sortedList)
                If dtData IsNot Nothing Then
                    If dtData.Rows.Count > 0 Then
                        LoadingDropDownList(ddlVendorName, dtData.Columns("VENDORNAME").ColumnName, dtData.Columns("ID").ColumnName, dtData, True, " ")
                    End If
                    'End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlVendorName.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
                            LoadingDropDownList(ddlSaleLast12, dtData.Columns("QTYSOLD").ColumnName,
                                                dtData.Columns("IMPTN").ColumnName, ds.Tables(0), True, " ")
                        End If
                    End If
                Else
                    Dim ListItem As ListItem = New ListItem()
                    ddlSaleLast12.Items.Add(New WebControls.ListItem(" ", " "))
                End If

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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

    Protected Sub fill_Users(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim messageOut As String = Nothing
        Try
            'If dwlControl.ID <> "ddlAssignFoot" Or (dwlControl.Items.Count = 0 And dwlControl.ID = "ddlAssignFoot") Then
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                ds = objBL.GetAllPaAndPsUsers(messageOut)
            End Using

            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    LoadingDropDownList(dwlControl, ds.Tables(0).Columns("USER").ColumnName,
                                        ds.Tables(0).Columns("PA").ColumnName, ds.Tables(0), True, " ")
                End If
            Else
                'Log.Info(strLogCadenaCabecera + ".. Result is: " + result.ToString())
                'log.Info(strLogCadenaCabecera + ".. Exception is: " + messageOut)
            End If
            'End If
        Catch ex As Exception
            'log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
        End Try
    End Sub


#End Region

    Private Sub Load_Combos_inGrid(Optional ddl As DropDownList = Nothing)

        If LCase(ddl.ID.Trim()) = "ddlvendorname" And ddlVendorName.Items.Count = 0 Then
            fill_VndName(ddlVendorName)
        ElseIf LCase(ddl.ID.Trim()) = "ddlmajor" And ddlMajor.Items.Count = 0 Then
            fill_Major(ddlMajor)
        ElseIf LCase(ddl.ID.Trim()) = "ddlsalelast12" And ddlSaleLast12.Items.Count = 0 Then
            fill_SalesLast12(ddlSaleLast12)
        ElseIf LCase(ddl.ID.Trim()) = "ddlcategory" And ddlCategory.Items.Count = 0 Then
            fill_Category(ddlCategory)
        ElseIf LCase(ddl.ID.Trim()) = "ddlwishlist" And ddlWishList.Items.Count = 0 Then
            fill_WL(ddlWishList)
        End If

    End Sub

    Private Sub load_Combos()

        fill_Vendors_Assigned(ddlVendAssign)
        fill_Page_Size(ddlPageSize)
        fill_Users(ddlUser2)



        Session("flagDdlWLFoot") = ""
        Session("flagDdlMajorFoot") = ""
        Session("flagDdlVndNameFoot") = ""
        Session("flagDdlSaleLast12Foot") = ""
        Session("flagDdlCategoryFoot") = ""

    End Sub

    Protected Sub LoadingDropDownList(dwlControl As DropDownList, displayMember As String, valueMember As String, data As DataTable, genrateSelect As Boolean, strTextSelect As String)

        Dim dtTemp As DataTable = data.Copy()
        dwlControl.Items.Clear()
        If (genrateSelect) Then
            Dim row As DataRow = dtTemp.NewRow()
            row(displayMember) = strTextSelect
            row(valueMember) = -1
            If dwlControl.ID.Contains("ddlUser") Then
                row("FULLNAME") = "  "
            End If
            dtTemp.Rows.InsertAt(row, 0)
        End If

        dwlControl.DataSource = dtTemp
        dwlControl.DataTextField = displayMember
        dwlControl.DataValueField = valueMember
        dwlControl.DataBind()

    End Sub

#Region "Old footer Dropdownlist"

    'Protected Sub ddlWLFoot_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Dim timesQDefault As String = ConfigurationManager.AppSettings("timesQuoteDefault")
    '    Try
    '        '            If ddlWLFoot.SelectedIndex = 0 Then
    '        '                ddlWLFoot.ClearSelection()
    '        '            Else
    '        '                If (ddlWLFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
    '        '                    ddlWLFoot.SelectedIndex = ddlWLFoot.Items.IndexOf(ddlWLFoot.Items.FindByText(DirectCast(Session("flagDdlWLFoot"), String)))
    '        '                End If

    '        '                Dim ddlSelection = ddlWLFoot.SelectedItem.Text

    '        '                Dim priorValueSelected = DirectCast(Session("flagDdlWLFoot"), String)
    '        '                'Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)

    '        '                If UCase(Trim(priorValueSelected)) = UCase(Trim(ddlWLFoot.SelectedItem.Text)) Then
    '        '                    Exit Sub
    '        '                End If

    '        '                Dim dtSelection As New DataTable
    '        '                Dim dsSelection As New DataSet
    '        '                Dim lstSelection = New List(Of LostSales)()
    '        '                Dim message As String = Nothing
    '        '                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)

    '        '                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
    '        '                Dim lsTemp = fillObj(ds.Tables(0))
    '        '                For Each item In lsTemp
    '        '                    If UCase(Trim(item.WLIST)) = UCase(Trim(ddlSelection)) Then
    '        '                        'lsTemp.Remove(item)
    '        '                        lstSelection.Add(item)
    '        '                    End If
    '        '                Next

    '        '                If lstSelection.Count = 0 Then
    '        '                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then

    '        '#Region "Maybe"

    '        '                        'getDataSource(True)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.WLIST)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)
    '        '                        '        lstSelection.Add(item1)
    '        '                        '    End If
    '        '                        'Next

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "WLIST"), New List(Of LostSales))


    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlWLFoot, "WLIST")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    Else
    '        '                        Dim data = fitSelection(ddlWLFoot, "WLIST")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    End If
    '        '                Else
    '        '                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then
    '        '#Region "Maybe"

    '        '                        'Dim check = DirectCast(Session("flagVnd"), String)
    '        '                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.WLIST)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)

    '        '                        '        'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
    '        '                        '        'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
    '        '                        '        Dim myitem = lstSelection.Find(Function(ite) ite.WLIST.Equals(item1.WLIST, StringComparison.InvariantCultureIgnoreCase))

    '        '                        '        If myitem Is Nothing Then
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If

    '        '                        '    End If
    '        '                        'Next

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "WLIST"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If
    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlWLFoot, "WLIST")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    Else

    '        '                        Dim data = fitSelection(ddlWLFoot, "WLIST")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    End If
    '        '                End If

    '        '                Session("flagDdlWLFoot") = ddlWLFoot.SelectedItem.Text

    '        '                If lstSelection.Count = 0 Then
    '        '                    grvLostSales.DataSource = Nothing
    '        '                    grvLostSales.DataBind()

    '        '                    'Session("WishListData") = Session("WishListBck")

    '        '                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
    '        '                Else
    '        '                    dtSelection = ListToDataTable(lstSelection)
    '        '                    dsSelection.Tables.Add(dtSelection)
    '        '                    GetLostSalesData("", 1, Nothing, dsSelection)
    '        '                End If

    '        '            End If

    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    'Protected Sub ddlMajorFoot_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Dim timesQDefault As String = ConfigurationManager.AppSettings("timesQuoteDefault")
    '    Try
    '        '            If ddlMajorFoot.SelectedIndex = 0 Then
    '        '                ddlMajorFoot.ClearSelection()
    '        '            Else
    '        '                If (ddlMajorFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
    '        '                    ddlMajorFoot.SelectedIndex = ddlMajorFoot.Items.IndexOf(ddlMajorFoot.Items.FindByText(DirectCast(Session("flagDdlMajorFoot"), String)))
    '        '                End If
    '        '                Dim ddlSelection = ddlMajorFoot.SelectedItem.Text

    '        '                'Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)
    '        '                Dim priorValueSelected = DirectCast(Session("flagDdlMajorFoot"), String)

    '        '                If UCase(Trim(priorValueSelected)) = UCase(Trim(ddlCategoryFoot.SelectedItem.Text)) Then
    '        '                    Exit Sub
    '        '                End If

    '        '                Dim dtSelection As New DataTable
    '        '                Dim dsSelection As New DataSet
    '        '                Dim lstSelection = New List(Of LostSales)()
    '        '                Dim message As String = Nothing
    '        '                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)

    '        '                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
    '        '                Dim lsTemp = fillObj(ds.Tables(0))
    '        '                For Each item In lsTemp
    '        '                    If UCase(Trim(item.IMPC1)) = UCase(Trim(ddlSelection)) Then
    '        '                        'lsTemp.Remove(item)
    '        '                        lstSelection.Add(item)
    '        '                    End If
    '        '                Next

    '        '                If lstSelection.Count = 0 Then
    '        '                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
    '        '#Region "Maybe"

    '        '                        'If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
    '        '                        '    getDataSource(True)
    '        '                        '    Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        '    Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        '    For Each item1 In lsTemp1
    '        '                        '        If UCase(Trim(item1.IMPC1)) = UCase(Trim(ddlSelection)) Then
    '        '                        '            'lsTemp.Remove(item)
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If
    '        '                        '    Next

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "IMPC1"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlWLFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then
    '        '#Region "maybe"

    '        '                        'Dim check = DirectCast(Session("flagVnd"), String)
    '        '                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.IMPC1)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)

    '        '                        '        'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
    '        '                        '        'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
    '        '                        '        Dim myitem = lstSelection.Find(Function(ite) ite.IMPC1.Equals(item1.IMPC1, StringComparison.InvariantCultureIgnoreCase))

    '        '                        '        If myitem Is Nothing Then
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If

    '        '                        '    End If
    '        '                        'Next

    '        '                        'If lstSelection.Count = 0 Then
    '        '                        '    message = "There is no result for this selection."
    '        '                        'End If

    '        '#End Region
    '        '                        Dim data = fitSelection(ddlMajorFoot, "IMPC1")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    Else
    '        '                        Dim data = fitSelection(ddlMajorFoot, "IMPC1")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    End If
    '        '                Else
    '        '                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then

    '        '                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "IMPC1"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlWLFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlMajorFoot, "IMPC1")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    Else

    '        '                        Dim data = fitSelection(ddlMajorFoot, "IMPC1")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    End If
    '        '                End If

    '        '                Session("flagDdlMajorFoot") = ddlMajorFoot.SelectedItem.Text

    '        '                If lstSelection.Count = 0 Then
    '        '                    grvLostSales.DataSource = Nothing
    '        '                    grvLostSales.DataBind()

    '        '                    'Session("WishListData") = Session("WishListBck")

    '        '                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
    '        '                Else
    '        '                    dtSelection = ListToDataTable(lstSelection)
    '        '                    dsSelection.Tables.Add(dtSelection)
    '        '                    GetLostSalesData("", 1, Nothing, dsSelection)
    '        '                End If

    '        '            End If
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    'Protected Sub ddlCategoryFoot_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Dim timesQDefault As String = ConfigurationManager.AppSettings("timesQuoteDefault")
    '    Try

    '        '            If ddlCategoryFoot.SelectedIndex = 0 Then
    '        '                ddlCategoryFoot.ClearSelection()
    '        '            Else
    '        '                If (ddlCategoryFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
    '        '                    ddlCategoryFoot.SelectedIndex = ddlCategoryFoot.Items.IndexOf(ddlCategoryFoot.Items.FindByText(DirectCast(Session("flagDdlCategoryFoot"), String)))
    '        '                End If
    '        '                Dim ddlSelection = ddlCategoryFoot.SelectedItem.Text

    '        '                'Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)
    '        '                Dim priorValueSelected = DirectCast(Session("flagDdlCategoryFoot"), String)

    '        '                If UCase(Trim(priorValueSelected)) = UCase(Trim(ddlCategoryFoot.SelectedItem.Text)) Then
    '        '                    Exit Sub
    '        '                End If

    '        '                Dim dtSelection As New DataTable
    '        '                Dim dsSelection As New DataSet
    '        '                Dim lstSelection = New List(Of LostSales)()
    '        '                Dim message As String = Nothing
    '        '                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)

    '        '                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
    '        '                Dim lsTemp = fillObj(ds.Tables(0))
    '        '                For Each item In lsTemp
    '        '                    If UCase(Trim(item.CATDESC)) = UCase(Trim(ddlSelection)) Then
    '        '                        'lsTemp.Remove(item)
    '        '                        lstSelection.Add(item)
    '        '                    End If
    '        '                Next

    '        '                If lstSelection.Count = 0 Then
    '        '                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then     'funcion para a la vez revisar si el resto estan sin seleccion

    '        '#Region "maybe"

    '        '                        'lnkReloadBack_Click(Nothing, Nothing)

    '        '                        'Dim check = DirectCast(Session("flagVnd"), String)
    '        '                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

    '        '                        'empty result no references for this criteria

    '        '                        'getDataSource(True)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
    '        '                        'Dim vndSel = DirectCast(Session("flagVnd"), String)

    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If vndSel = "3" Then
    '        '                        '        If UCase(Trim(item1.CATDESC)) = UCase(Trim(ddlSelection)) Then
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If
    '        '                        '    Else
    '        '                        '        If vndSel = "1" Then
    '        '                        '            If Not String.IsNullOrEmpty(item1.VENDOR) Then
    '        '                        '                Dim tq = If(String.IsNullOrEmpty(item1.TIMESQ), 0, CInt(item1.TIMESQ))
    '        '                        '                If tq >= CInt(getSelectedQuote) Then
    '        '                        '                    If UCase(Trim(item1.CATDESC)) = UCase(Trim(ddlSelection)) Then
    '        '                        '                        lstSelection.Add(item1)
    '        '                        '                    End If
    '        '                        '                End If
    '        '                        '            End If
    '        '                        '        Else
    '        '                        '            If String.IsNullOrEmpty(item1.VENDOR) Then
    '        '                        '                Dim tq = If(String.IsNullOrEmpty(item1.TIMESQ), 0, CInt(item1.TIMESQ))
    '        '                        '                If tq >= CInt(getSelectedQuote) Then
    '        '                        '                    If UCase(Trim(item1.CATDESC)) = UCase(Trim(ddlSelection)) Then
    '        '                        '                        lstSelection.Add(item1)
    '        '                        '                    End If
    '        '                        '                End If
    '        '                        '            End If
    '        '                        '        End If
    '        '                        '    End If
    '        '                        'Next

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "CATDESC"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If
    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlWLFoot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlCategoryFoot, "CATDESC")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        message = "There is not data with this multiple criteria."
    '        '                    Else

    '        '                        Dim data = fitSelection(ddlCategoryFoot, "CATDESC")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        message = "There is not data with this multiple criteria."
    '        '                    End If

    '        '                Else
    '        '                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then

    '        '                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "CATDESC"), New List(Of LostSales))
    '        '#Region "Maybe"

    '        '                        ''Session("LostSaleData") = Session("LostSaleBck")

    '        '                        'Dim check = DirectCast(Session("flagVnd"), String)
    '        '                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.CATDESC)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)

    '        '                        '        'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
    '        '                        '        'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
    '        '                        '        Dim myitem = lstSelection.Find(Function(ite) ite.CATDESC.Equals(item1.CATDESC, StringComparison.InvariantCultureIgnoreCase))

    '        '                        '        If myitem Is Nothing Then
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If

    '        '                        '    End If
    '        '                        'Next

    '        '                        'If lstSelection.Count = 0 Then
    '        '                        '    message = "There is no result for this selection."
    '        '                        'End If

    '        '#End Region

    '        '                    ElseIf (ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlWLFoot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0) Then

    '        '                        Dim data = fitSelection(ddlCategoryFoot, "CATDESC")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        message = "There is not data with this multiple criteria."

    '        '                    Else

    '        '                        Dim data = fitSelection(ddlCategoryFoot, "CATDESC")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        message = "There is not data with this multiple criteria."
    '        '                    End If
    '        '                End If

    '        '                Session("flagDdlCategoryFoot") = ddlCategoryFoot.SelectedItem.Text

    '        '                If lstSelection.Count = 0 Then
    '        '                    grvLostSales.DataSource = Nothing
    '        '                    grvLostSales.DataBind()

    '        '                    'Session("WishListData") = Session("WishListBck")

    '        '                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
    '        '                Else
    '        '                    dtSelection = ListToDataTable(lstSelection)
    '        '                    dsSelection.Tables.Add(dtSelection)
    '        '                    GetLostSalesData("", 1, Nothing, dsSelection)
    '        '                End If
    '        '                'End If
    '        '            End If

    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    'Protected Sub ddlVndNameFoot_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Dim timesQDefault As String = ConfigurationManager.AppSettings("timesQuoteDefault")
    '    Try
    '        '            If ddlVndNameFoot.SelectedIndex = 0 Then
    '        '                ddlVndNameFoot.ClearSelection()
    '        '            Else
    '        '                If (ddlVndNameFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
    '        '                    ddlVndNameFoot.SelectedIndex = ddlVndNameFoot.Items.IndexOf(ddlVndNameFoot.Items.FindByText(DirectCast(Session("flagDdlVndNameFoot"), String)))
    '        '                End If
    '        '                Dim ddlSelection = ddlVndNameFoot.SelectedItem.Text

    '        '                'Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)
    '        '                Dim priorValueSelected = DirectCast(Session("flagDdlVndNameFoot"), String)

    '        '                If UCase(Trim(priorValueSelected)) = UCase(Trim(ddlVndNameFoot.SelectedItem.Text)) Then
    '        '                    Exit Sub
    '        '                End If

    '        '                Dim dtSelection As New DataTable
    '        '                Dim dsSelection As New DataSet
    '        '                Dim lstSelection = New List(Of LostSales)()
    '        '                Dim message As String = Nothing
    '        '                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)

    '        '                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
    '        '                Dim lsTemp = fillObj(ds.Tables(0))
    '        '                For Each item In lsTemp
    '        '                    If UCase(Trim(item.VENDORNAME)) = UCase(Trim(ddlSelection)) Then
    '        '                        'lsTemp.Remove(item)
    '        '                        lstSelection.Add(item)
    '        '                    End If
    '        '                Next

    '        '                If lstSelection.Count = 0 Then
    '        '                    If ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 Then
    '        '#Region "maybe"

    '        '                        'no hay resultados de busqueda y ningun otro combo esta seleccionado
    '        '                        'getDataSource(True)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.VENDORNAME)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)
    '        '                        '        lstSelection.Add(item1)
    '        '                        '    End If
    '        '                        'Next

    '        '                        'If lstSelection.Count = 0 Then
    '        '                        '    Session("LostSaleData") = Session("TempLostSaleData")
    '        '                        '    message = "There is no result for this selection."
    '        '                        'End If

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "VENDORNAME"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlWLFoot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlVndNameFoot, "VENDORNAME")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    Else
    '        '                        'new function
    '        '                        Dim data = fitSelection(ddlVndNameFoot, "VENDORNAME")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If
    '        '                    End If

    '        '                Else
    '        '                    If (ddlSaleLast12Foot.SelectedIndex = 0 And ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then
    '        '#Region "Maybe"

    '        '                        ''Session("LostSaleData") = Session("LostSaleBck")

    '        '                        'Dim check = DirectCast(Session("flagVnd"), String)
    '        '                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.VENDORNAME)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)

    '        '                        '        'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
    '        '                        '        'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
    '        '                        '        Dim myitem = lstSelection.Find(Function(ite) ite.VENDORNAME.Equals(item1.VENDORNAME, StringComparison.InvariantCultureIgnoreCase))

    '        '                        '        If myitem Is Nothing Then
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If

    '        '                        '    End If
    '        '                        'Next

    '        '                        'If lstSelection.Count = 0 Then
    '        '                        '    message = "There is no result for this selection."
    '        '                        'End If

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "VENDORNAME"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    ElseIf ddlSaleLast12Foot.SelectedIndex <> 0 Or ddlWLFoot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlVndNameFoot, "VENDORNAME")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        message = "There is not data with this multiple criteria."

    '        '                    Else

    '        '                        Dim data = fitSelection(ddlVndNameFoot, "VENDORNAME")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        message = "There is not data with this multiple criteria."
    '        '                    End If
    '        '                End If

    '        '                Session("flagDdlVndNameFoot") = ddlVndNameFoot.SelectedItem.Text

    '        '                If lstSelection.Count = 0 Then
    '        '                    grvLostSales.DataSource = Nothing
    '        '                    grvLostSales.DataBind()

    '        '                    'Session("WishListData") = Session("WishListBck")

    '        '                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
    '        '                Else
    '        '                    dtSelection = ListToDataTable(lstSelection)
    '        '                    dsSelection.Tables.Add(dtSelection)
    '        '                    GetLostSalesData("", 1, Nothing, dsSelection)
    '        '                End If
    '        '            End If

    '        'End If
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    'Protected Sub ddlSaleLast12Foot_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Dim exMessage As String = Nothing
    '    Dim timesQDefault As String = ConfigurationManager.AppSettings("timesQuoteDefault")
    '    Try
    '        '            If ddlSaleLast12Foot.SelectedIndex = 0 Then
    '        '                ddlSaleLast12Foot.ClearSelection()
    '        '            Else
    '        '                If (ddlSaleLast12Foot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
    '        '                    ddlSaleLast12Foot.SelectedIndex = ddlSaleLast12Foot.Items.IndexOf(ddlSaleLast12Foot.Items.FindByText(DirectCast(Session("flagDdlSaleLast12Foot"), String)))
    '        '                End If

    '        '                Dim ddlSelection = ddlSaleLast12Foot.SelectedItem.Text

    '        '                'Dim priorValueSelected = getFilteredValueInGrid(DirectCast(sender, DropDownList).ID)
    '        '                Dim priorValueSelected = DirectCast(Session("flagDdlSaleLast12Foot"), String)

    '        '                If UCase(Trim(priorValueSelected)) = UCase(Trim(ddlSaleLast12Foot.SelectedItem.Text)) Then
    '        '                    Exit Sub
    '        '                End If

    '        '                Dim dtSelection As New DataTable
    '        '                Dim dsSelection As New DataSet
    '        '                Dim lstSelection = New List(Of LostSales)()
    '        '                Dim message As String = Nothing
    '        '                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)

    '        '                Dim ds = DirectCast(Session("LostSaleData"), DataSet)
    '        '                Dim lsTemp = fillObj(ds.Tables(0))
    '        '                For Each item In lsTemp
    '        '                    If UCase(Trim(item.QTYSOLD)) = UCase(Trim(ddlSelection)) Then
    '        '                        'lsTemp.Remove(item)
    '        '                        lstSelection.Add(item)
    '        '                    End If
    '        '                Next

    '        '                If lstSelection.Count = 0 Then
    '        '                    If ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0 Then
    '        '#Region "Maybe"

    '        '                        'getDataSource(True)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.QTYSOLD)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)
    '        '                        '        lstSelection.Add(item1)
    '        '                        '    End If
    '        '                        'Next

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleBck"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "QTYSOLD"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If

    '        '                    ElseIf ddlWLFoot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlSaleLast12Foot, "QTYSOLD")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    Else
    '        '                        Dim data = fitSelection(ddlSaleLast12Foot, "QTYSOLD")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If
    '        '                    End If
    '        '                Else
    '        '                    If (ddlWLFoot.SelectedIndex = 0 And ddlMajorFoot.SelectedIndex = 0 And ddlCategoryFoot.SelectedIndex = 0 And ddlVndNameFoot.SelectedIndex = 0) And ddlSelection <> priorValueSelected Then
    '        '#Region "Maybe"

    '        '                        'Dim check = DirectCast(Session("flagVnd"), String)
    '        '                        'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)
    '        '                        'Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        'Dim lsTemp1 = fillObj(ds1.Tables(0))
    '        '                        'For Each item1 In lsTemp1
    '        '                        '    If UCase(Trim(item1.QTYSOLD)) = UCase(Trim(ddlSelection)) Then
    '        '                        '        'lsTemp.Remove(item)

    '        '                        '        'Dim myitem = lstSelection.Find(item >= item.name.Equals("foo", StringComparison.InvariantCultureIgnoreCase);
    '        '                        '        'Dim rowDelete = dsResult.Tables(0).AsEnumerable().Where(Function(row) row.ItemArray(0).ToString() = partNo And row.ItemArray(6).ToString() = vendorNo).FirstOrDefault()
    '        '                        '        Dim myitem = lstSelection.Find(Function(ite) ite.QTYSOLD.Equals(item1.QTYSOLD, StringComparison.InvariantCultureIgnoreCase))

    '        '                        '        If myitem Is Nothing Then
    '        '                        '            lstSelection.Add(item1)
    '        '                        '        End If

    '        '                        '    End If
    '        '                        'Next

    '        '#End Region
    '        '                        Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)
    '        '                        lstSelection = If(ddlVendAssignSimulation(ds1, ddlSelection, getSelectedQuote, "QTYSOLD"), New List(Of LostSales))

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is no result for this selection."
    '        '                        End If
    '        '                    ElseIf ddlWLFoot.SelectedIndex <> 0 Or ddlMajorFoot.SelectedIndex <> 0 Or ddlCategoryFoot.SelectedIndex <> 0 Or ddlVndNameFoot.SelectedIndex <> 0 Then

    '        '                        Dim data = fitSelection(ddlSaleLast12Foot, "QTYSOLD")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If

    '        '                    Else
    '        '                        Dim data = fitSelection(ddlSaleLast12Foot, "QTYSOLD")
    '        '                        lstSelection = data.AsEnumerable().ToList()

    '        '                        If lstSelection.Count = 0 Then
    '        '                            message = "There is not data with this multiple criteria."
    '        '                        End If
    '        '                    End If
    '        '                End If

    '        '                Session("flagDdlSaleLast12Foot") = ddlSaleLast12Foot.SelectedItem.Text

    '        '                If lstSelection.Count = 0 Then
    '        '                    grvLostSales.DataSource = Nothing
    '        '                    grvLostSales.DataBind()

    '        '                    'Session("WishListData") = Session("WishListBck")

    '        '                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
    '        '                Else
    '        '                    dtSelection = ListToDataTable(lstSelection)
    '        '                    dsSelection.Tables.Add(dtSelection)
    '        '                    GetLostSalesData("", 1, Nothing, dsSelection)
    '        '                End If

    '        '            End If

    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

#End Region

    Protected Sub ddlUser2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlUser2.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try
            Dim pepe = ddlUser2.SelectedItem.Text
            Dim pepe1 = ddlUser2.SelectedItem.Value
            Session("PERPECHUSER") = ddlUser2.SelectedItem.Text
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim intValue As Integer
        Dim dsSetDataSource = New DataSet()
        Try
            If Integer.TryParse(ddlPageSize.SelectedValue, intValue) Then
                grvLostSales.AllowPaging = True
                grvLostSales.PageSize = If(ddlPageSize.SelectedValue > 10, CInt(ddlPageSize.SelectedValue), 10)

                Dim CurrentPage = (DirectCast(Session("currentPage"), Integer))
                Session("PageAmounts") = grvLostSales.PageSize * CurrentPage

                Dim dsLoad = DirectCast(Session("LostSaleData"), DataSet)
                If dsLoad IsNot Nothing Then
                    If dsLoad.Tables(0).Rows.Count > 0 Then
                        loadData(dsLoad)
                        'GetLostSalesData("", 1, Nothing, dsLoad)
                    Else
                        loadData(Nothing)
                    End If
                Else
                    loadData(Nothing)
                End If
            Else
                loadData(Nothing)
            End If
            updatePagerSettings(grvLostSales)
        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try

    End Sub

    Private Sub loadData(Optional ds As DataSet = Nothing, Optional dt As DataTable = Nothing)
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Try
            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    grvLostSales.DataSource = ds.Tables(0)
                    grvLostSales.DataBind()
                    Session("LostSaleData") = ds
                Else
                    grvLostSales.DataSource = Nothing
                    grvLostSales.DataBind()

                    methodMessage = "There is not results with the selected criteria."
                    SendMessage(methodMessage, messageType.warning)
                End If
                'updatepnl2.Update()

                Exit Sub
            Else
                If dt IsNot Nothing Then
                    If dt.Rows.Count > 0 Then
                        grvLostSales.DataSource = dt
                        grvLostSales.DataBind()

                        Dim dtt = New DataTable()
                        dtt = dt.Copy()
                        Dim dss = New DataSet()
                        dss.Tables.Add(dtt)
                        Session("LostSaleData") = dss
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)

                    End If
                End If
                'updatepnl2.Update()

                Exit Sub
            End If

            'updatepnl2.Update()

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Private Sub getLSData(Optional ds As DataSet = Nothing, Optional qty As Integer = 0, Optional selection As String = Nothing, Optional ddlExtraSel As DropDownList = Nothing)
        Dim exMessage As String = Nothing
        Dim lstVendors As List(Of LostSales) = New List(Of LostSales)()
        Dim lstSelectedCVData As List(Of LostSales) = New List(Of LostSales)()
        Dim lstSelectedNVData As List(Of LostSales) = New List(Of LostSales)()
        Try
            If ds IsNot Nothing Then
                lstVendors = fillObj(ds.Tables(0))

                For Each obj As LostSales In lstVendors
                    If Not String.IsNullOrEmpty(obj.VENDOR) Then
                        lstSelectedCVData.Add(obj)
                    Else
                        lstSelectedNVData.Add(obj)
                    End If
                Next

                If lstSelectedCVData.Count > 0 Then
                    Dim dtResultCV = ListToDataTable(lstSelectedCVData)
                    Dim dsResultCV As DataSet = New DataSet()
                    dsResultCV.Tables.Add(dtResultCV)
                    Session("CVendor") = dsResultCV
                End If

                If lstSelectedNVData.Count > 0 Then
                    Dim dtResultNV = ListToDataTable(lstSelectedNVData)
                    Dim dsResultNV As DataSet = New DataSet()
                    dsResultNV.Tables.Add(dtResultNV)
                    Session("NVendor") = dsResultNV
                End If
            End If

            Dim dsOut As DataSet = New DataSet()
            If qty > 0 Then
                Dim tq = 0
                If Not String.IsNullOrEmpty(selection) Then
                    If selection.Equals("1") Then 'with vendor    

                        Dim ds1 = DirectCast(Session("CVendor"), DataSet)
                        Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) CInt(item.Item("TIMESQ").ToString()) >= CInt(qty)).AsEnumerable().ToList()
                        If myitem.Count > 0 Then
                            dsOut = ListToDataTableDr(myitem)
                        End If
                        Session("LostSaleData") = dsOut
                        'setDefaultValues(dsOut)

                    ElseIf selection.Equals("2") Then ' without vendor     

                        Dim ds1 = DirectCast(Session("NVendor"), DataSet)
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Trace, "Count ds1", ds1.Tables(0).Rows.Count.ToString())
                        Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) CInt(item.Item("TIMESQ").ToString()) >= CInt(qty)).AsEnumerable().ToList()
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Trace, "Count myitem", myitem.Count.ToString())
                        If myitem.Count > 0 Then
                            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Trace, "Validacion", "Entro en la condicion de timesq")
                            dsOut = ListToDataTableDr(myitem)
                        End If
                        'If dsOut IsNot Nothing Then
                        Session("LostSaleData") = dsOut
                        'Else
                        'Session("LostSaleData") = New DataSet()
                        'End If

                        'setDefaultValues(dsOut)

                    Else
                        Session("LostSaleData") = Session("LostSaleBck")
                    End If

                    If ddlExtraSel IsNot Nothing Then
                        Dim ddlSelection = ddlExtraSel.SelectedValue
                        Dim dsData = If(Session("LostSaleData") IsNot Nothing, DirectCast(Session("LostSaleData"), DataSet), Nothing)
                        If dsData Is Nothing Then
                            Exit Sub
                        End If
                        Dim myitem = dsData.Tables(0).AsEnumerable().Where(Function(item) LCase(item.Item("CATDESC").ToString().Trim()) = LCase(ddlSelection.Trim())).AsEnumerable().ToList()
                        If myitem.Count > 0 Then
                            dsOut = ListToDataTableDr(myitem)
                        End If
                        Session("LostSaleData") = dsOut
                    End If

                Else
                    Session("LostSaleData") = Session("LostSaleBck")
                End If

            Else

                If Not String.IsNullOrEmpty(selection) Then
                    If selection.Equals("1") Then 'with vendor    

                        Dim ds1 = DirectCast(Session("CVendor"), DataSet)
                        Session("LostSaleData") = ds1
                        'setDefaultValues(dsOut)

                    ElseIf selection.Equals("2") Then ' without vendor     

                        Dim ds1 = DirectCast(Session("NVendor"), DataSet)
                        Session("LostSaleData") = ds1
                        'setDefaultValues(dsOut)

                    Else
                        Session("LostSaleData") = Session("LostSaleBck")
                    End If
                Else
                    Session("LostSaleData") = Session("LostSaleBck")
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub ddlVendAssign_SelectedIndexChanged(sender As Object, e As EventArgs, Optional ctrlName As String = Nothing) Handles ddlVendAssign.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim timesQDefault As String = Session("TimesQuote").ToString()
        Dim dsData As New DataSet()
        Dim dtData As New DataTable()
        Dim dtResult As New DataTable()
        Dim dtResult1 As New DataTable()
        Dim lstVendors As List(Of LostSales) = New List(Of LostSales)()
        Dim lstSelectedVData As List(Of LostSales) = New List(Of LostSales)()
        'Dim loadFlag = DirectCast(Session("firstLoad"), String)
        Dim ddlOption As String = Nothing
        Try

            'If Not UCase(ctrlName).Contains("FOOT") Then
            If hiddenId3.Value = "2" Or hdSubmit.Value = "1" Then
                ddlOption = ddlVendAssign.SelectedValue
                Dim sessionOption As String = DirectCast(Session("flagVnd"), String)
#Region "Old Method"

                'Dim dsRecover = New DataSet()

                'If ddlOption <> sessionOption Then
                '    dsRecover = DirectCast(Session("LostSaleBck"), DataSet)
                '    'Dim rs = GetLostSalesData(Nothing, 1, dsRecover)
                '    Session("LostSaleData") = dsRecover
                'Else
                '    dsRecover = DirectCast(Session("LostSaleData"), DataSet)
                '    Dim grv = grvLostSales.DataSource
                'End If

#End Region
                Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)
                Session("TimesQuote") = getSelectedQuote

                'If grvLostSales.DataSource Is Nothing Then
                If False Then
                    Exit Sub
                ElseIf True Then
                    Session("flagVnd") = ddlOption
                    'Session("firstLoad") = "0"
                    If ddlOption = "3" Then
                        'Session("flagVnd") = ddlOption
                        Dim ds = DirectCast(Session("LostSaleBck"), DataSet)
                        getLSData(ds, CInt(getSelectedQuote), ddlOption)
#Region "Old Method"

                        'lstVendors = fillObj(dsRecover.Tables(0))
                        'For Each obj As LostSales In lstVendors
                        '    If obj.TIMESQ >= CInt(getSelectedQuote) Then
                        '        lstSelectedVData.Add(obj)
                        '    End If
                        'Next

                        'dtResult1 = ListToDataTable(lstSelectedVData)
                        'Dim dsLoad As DataSet = New DataSet()
                        'dsLoad = DirectCast(Session("LostSaleBck"), DataSet)
                        'GetLostSalesData(Nothing, 1, Nothing, dsLoad)

#End Region
                    Else
#Region "Old Method"

                        'lstVendors = fillObj(dsRecover.Tables(0))
                        'Dim testCount As Integer = 0
                        'Dim testCount1 As Integer = 0

                        'For Each obj As LostSales In lstVendors
                        '    If ddlOption.Equals("1") Then
                        '        Session("flagVnd") = "1"
                        '        If Not String.IsNullOrEmpty(obj.VENDOR) Then
                        '            Dim tq = If(String.IsNullOrEmpty(obj.TIMESQ), 0, CInt(obj.TIMESQ))
                        '            'If obj.IMPTN.Contains("7K5449") Then
                        '            testCount += 1

                        '            'If tq >= CInt(getSelectedQuote) Then
                        '            'testCount1 += 1
                        '            lstSelectedVData.Add(obj)
                        '            'End If
                        '            'End If

                        '            'If Not String.IsNullOrEmpty(getSelectedQuote) Then
                        '            '    Session("TimesQuote") = getSelectedQuote
                        '            '    If obj.TIMESQ >= CInt(getSelectedQuote) Then
                        '            '        lstSelectedVData.Add(obj)
                        '            '    End If
                        '            'Else
                        '            '    Session("TimesQuote") = timesQDefault
                        '            '    If obj.TIMESQ >= CInt(timesQDefault) Then
                        '            '        lstSelectedVData.Add(obj)
                        '            '    End If
                        '            'End If
                        '        End If
                        '    ElseIf ddlOption.Equals("2") Then
                        '        Session("flagVnd") = "2"
                        '        If String.IsNullOrEmpty(obj.VENDOR) Then
                        '            Dim tq = If(String.IsNullOrEmpty(obj.TIMESQ), 0, CInt(obj.TIMESQ))
                        '            If tq >= CInt(getSelectedQuote) Then
                        '                lstSelectedVData.Add(obj)
                        '            Else
                        '                lstSelectedVData.Add(obj)
                        '            End If
                        '            'If Not String.IsNullOrEmpty(getSelectedQuote) Then
                        '            '    Session("TimesQuote") = getSelectedQuote
                        '            '    If obj.TIMESQ >= CInt(getSelectedQuote) Then
                        '            '        lstSelectedVData.Add(obj)
                        '            '    End If
                        '            'Else
                        '            '    Session("TimesQuote") = timesQDefault
                        '            '    If obj.TIMESQ >= CInt(timesQDefault) Then
                        '            '        lstSelectedVData.Add(obj)
                        '            '    End If
                        '            'End If
                        '        End If
                        '    End If
                        'Next

                        'Dim pepe = testCount
                        'Dim pepe1 = testCount1

                        'If lstSelectedVData.Count > 0 Then
                        '    'DoExcel(dsRecover.Tables(0))
                        '    dtResult1 = ListToDataTable(lstSelectedVData)
                        '    Dim ds As DataSet = New DataSet()
                        '    ds.Tables.Add(dtResult1)
                        '    GetLostSalesData(Nothing, 1, Nothing, ds)
                        '    'Else
                        '    '    Dim dss = New DataSet()
                        '    '    dss = DirectCast(Session("LostSaleData"), DataSet)
                        '    '    Dim dtt = New DataTable()
                        '    '    dtt = dss.Tables(0).Copy()
                        '    '    dtResult1 = dtt
                        'Else
                        '    Dim ds1 As DataSet = New DataSet()
                        '    Dim dt1 As DataTable = New DataTable()
                        '    ds1.Tables.Add(dt1)
                        '    GetLostSalesData(Nothing, 1, Nothing, ds1)

                        '    'Dim methodMessage = "There is not results with the selected criteria. "
                        '    'SendMessage(methodMessage, messageType.warning)
                        'End If

#End Region
                        getLSData(Nothing, CInt(Session("TimesQuote").ToString()), CInt(ddlOption))

                    End If
                End If

                hiddenId3.Value = "0"

            ElseIf hiddenId3.Value = "0" Then
                'Dim ph As ContentPlaceHolder = DirectCast(Me.Master.FindControl("MainContent"), ContentPlaceHolder)
                'Dim grv As GridView = DirectCast(ph.FindControl("grvLostSales"), GridView)
                'GetLostSalesData(Nothing, 1, Nothing, ds)
                'hiddenId3.Value = "0"
                If LCase(ctrlName).Contains("lnkdetails") Then
                    getLSData(Nothing, CInt(Session("TimesQuote").ToString()), CInt(ddlOption))
                    Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                    loadData(ds)
                    'updatepnl2.Update()

                End If

            End If

#Region "Old version"

            'Session("flagVnd") = ddlOption

            'ElseIf hdCloseAction.Value <> "0" Then

            'If hiddenId3.Value = "2" Then
            '    ddlOption = ddlVendAssign.SelectedValue
            '    Dim sessionOption As String = DirectCast(Session("flagVnd"), String)

            '    Dim getSelectedQuote As String = If(Not String.IsNullOrEmpty(getStrTQouteCriteria()), getStrTQouteCriteria(), timesQDefault)
            '    Session("TimesQuote") = getSelectedQuote

            '    'If grvLostSales.DataSource Is Nothing Then
            '    If False Then
            '        Exit Sub
            '    ElseIf True Then

            '        Session("firstLoad") = "0"
            '        If ddlOption = "3" Then
            '            Session("flagVnd") = "3"
            '            getLSData()
            '        Else
            '            getLSData(Nothing, Session("TimesQuote").ToString(), CInt(ddlOption))
            '        End If
            '    End If
            'ElseIf hiddenId3.Value = "0" Then
            '    getLSData()
            'End If

            'hiddenId3.Value = "0"
            'Session("flagVnd") = ddlOption

            'hdCloseAction.Value = "0"
            'Else

            'Dim ds = DirectCast(Session("LostSaleData"), DataSet)
            'Dim rs = GetLostSalesData("0,10", 1, Nothing, ds)

            'grvLostSales.DataSource = dt
            'grvLostSales.DataBind()
            'End If

#End Region

            If sender IsNot Nothing Then 'check if extra filter is active

                If getActiveDdlAction() <> "Error" And getActiveDdlAction() IsNot Nothing Then
                    Dim mi = getActiveDdlAction()
                    Dim selection(3) As Object
                    selection(0) = Nothing
                    selection(1) = Nothing
                    selection(2) = True
                    CallByName(Me, mi, CallType.Method, selection(0), selection(1), selection(2))
                    'Dim methodAction As MethodInfo = Me.GetType().GetMethod(mi, BindingFlags.InvokeMethod, Nothing, New Type() {GetType(Object), GetType(EventArgs), GetType(Boolean)}, Nothing)
                    'methodAction.Invoke(Me, New Object() {Nothing, Nothing, True})
                Else
                    Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                    setDefaultValues(ds)
                    loadData(ds)

                    'updatepnl2.Update()
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Session("flagVnd") = "-1"
        End Try
    End Sub

#Region "Extra Filters Dropdownlist"

    Public Sub ddlWishList_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False) Handles ddlWishList.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim ddlOption = ddlWishList.SelectedValue
        Try
            Dim dsData = New DataSet()
            'ddlMajor.SelectedIndex = 0
            'ddlSaleLast12.SelectedIndex = 0
            'ddlVendorName.SelectedIndex = 0
            'ddlCategory.SelectedIndex = 0
            Session("ddlCategoryIndex") = "0"
            Session("ddlVendorNameIndex") = "0"
            Session("ddlMajorIndex") = "0"
            Session("ddlSaleLast12Index") = "0"
            Dim ddlPrevious = If(Session("curWL") IsNot Nothing, Session("curWL").ToString(), "")

            If (CInt(DirectCast(Session("ddlWlistIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlWlistIndex"), String)) = -1) Or ddlOption <> ddlPrevious Or flag Then

                If ddlWishList.SelectedIndex = 0 Then
                    dsData = DirectCast(Session("LostSaleBck"), DataSet)

                    getLSData(dsData, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                    loadData(DirectCast(Session("LostSaleData"), DataSet))

                    setDefaultValues(DirectCast(Session("LostSaleData"), DataSet))

                ElseIf ddlWishList.SelectedIndex > 0 Then

                    If DirectCast(Session("ddlWlistIndex"), String) = "-1" Or flag = True Then
                        getLSData(Nothing, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                        dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Session("ddlCategoryIndex") = "0"
                        Session("ddlVendorNameIndex") = "0"
                        Session("ddlMajorIndex") = "0"
                        Session("ddlSaleLast12Index") = "0"
                    Else
                        If DirectCast(Session("ddlWlistIndex"), String) <> ddlWishList.SelectedIndex.ToString() Then
                            Dim dsFirst = DirectCast(Session("LostSaleBck"), DataSet)
                            getLSData(dsFirst, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Else
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        End If
                    End If

                    'Dim dsData = If(DirectCast(Session("ddlWlistIndex"), String) = "-1", DirectCast(Session("LostSaleData"), DataSet), DirectCast(Session("LostSaleBck"), DataSet))

                    'Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                    Dim dsFilter As DataSet = New DataSet()
                    Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                    Dim valueToCompare As String = ddlWishList.SelectedItem.Text.ToString()
                    For Each dr As DataRow In dsData.Tables(0).Rows
                        If LCase(dr.Item("WLIST").ToString().Trim()) = LCase(valueToCompare.Trim()) Then
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
                            Session("LostSaleData") = dsFilter
                            loadData(dsFilter)
                            'GetLostSalesData("", 1, Nothing, dsFilter)
                            'GetClaimsReport("", 1, dsFilter)
                        Else
                            grvLostSales.DataSource = Nothing
                            grvLostSales.DataBind()

                            methodMessage = "There is not results with the selected criteria."
                            SendMessage(methodMessage, messageType.warning)

                            'updatepnl2.Update()
                        End If
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)

                        'updatepnl2.Update()
                    End If

                    setDefaultValues(dsFilter)
                    'Else
                    'error message
                    Session("ddlWlistIndex") = ddlWishList.SelectedIndex.ToString()
                End If

                Session("curWL") = ddlOption
            Else
                If Session("curWL") Is Nothing Then
                    Session("curWL") = ddlOption
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Sub ddlMajor_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False) Handles ddlMajor.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim ddlOption = ddlMajor.SelectedValue
        Try
            Dim dsData = New DataSet()
            'ddlWishList.SelectedIndex = 0
            'ddlSaleLast12.SelectedIndex = 0
            'ddlVendorName.SelectedIndex = 0
            'ddlCategory.SelectedIndex = 0
            Session("ddlCategoryIndex") = "0"
            Session("ddlVendorNameIndex") = "0"
            Session("ddlWlistIndex") = "0"
            Session("ddlSaleLast12Index") = "0"
            Dim ddlPrevious = If(Session("curMajor") IsNot Nothing, Session("curMajor").ToString(), "")

            If (CInt(DirectCast(Session("ddlMajorIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlMajorIndex"), String)) = -1) Or ddlOption <> ddlPrevious Or flag Then

                If ddlMajor.SelectedIndex = 0 Then

                    dsData = DirectCast(Session("LostSaleBck"), DataSet)
                    getLSData(dsData, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                    loadData(DirectCast(Session("LostSaleData"), DataSet))

                    setDefaultValues(DirectCast(Session("LostSaleData"), DataSet))

                ElseIf ddlMajor.SelectedIndex > 0 Then

                    If DirectCast(Session("ddlMajorIndex"), String) = "-1" Or flag = True Then
                        getLSData(Nothing, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                        dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Session("ddlCategoryIndex") = "0"
                        Session("ddlVendorNameIndex") = "0"
                        Session("ddlWlistIndex") = "0"
                        Session("ddlSaleLast12Index") = "0"
                    Else
                        If DirectCast(Session("ddlMajorIndex"), String) <> ddlMajor.SelectedIndex.ToString() Then
                            Dim dsFirst = DirectCast(Session("LostSaleBck"), DataSet)
                            getLSData(dsFirst, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Else
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        End If
                    End If

                    'Dim dsData = If(DirectCast(Session("ddlMajorIndex"), String) = "-1", DirectCast(Session("LostSaleData"), DataSet), DirectCast(Session("LostSaleBck"), DataSet))

                    'Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                    Dim dsFilter As DataSet = New DataSet()
                    Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                    Dim valueToCompare As String = ddlMajor.SelectedItem.Text.ToString()
                    For Each dr As DataRow In dsData.Tables(0).Rows
                        If LCase(dr.Item("IMPC1").ToString().Trim()) = LCase(valueToCompare.Trim()) Then
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
                            Session("LostSaleData") = dsFilter
                            loadData(dsFilter)
                            'GetLostSalesData("", 1, Nothing, dsFilter)
                            'GetClaimsReport("", 1, dsFilter)
                        Else
                            grvLostSales.DataSource = Nothing
                            grvLostSales.DataBind()

                            methodMessage = "There is not results with the selected criteria."
                            SendMessage(methodMessage, messageType.warning)

                            'updatepnl2.Update()
                        End If
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)

                        'updatepnl2.Update()
                    End If

                    setDefaultValues(dsFilter)
                    'Else
                    'error message
                    Session("ddlMajorIndex") = ddlMajor.SelectedIndex.ToString()
                End If
                Session("curMajor") = ddlOption
            Else
                If Session("curMajor") Is Nothing Then
                    Session("curMajor") = ddlOption
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Sub ddlSaleLast12_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False) Handles ddlSaleLast12.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim ddlOption = ddlSaleLast12.SelectedValue
        Try
            Dim dsData = New DataSet()
            'ddlMajor.SelectedIndex = 0
            'ddlWishList.SelectedIndex = 0
            'ddlVendorName.SelectedIndex = 0
            'ddlCategory.SelectedIndex = 0
            Session("ddlCategoryIndex") = "0"
            Session("ddlVendorNameIndex") = "0"
            Session("ddlWlistIndex") = "0"
            Session("ddlMajorIndex") = "0"
            Dim ddlPrevious = If(Session("curCategory") IsNot Nothing, Session("curCategory").ToString(), "")

            If (CInt(DirectCast(Session("ddlCategoryIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlCategoryIndex"), String)) = -1) Or ddlOption <> ddlPrevious Or flag Then

                If ddlSaleLast12.SelectedIndex = 0 Then
                    dsData = DirectCast(Session("LostSaleBck"), DataSet)

                    getLSData(dsData, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                    loadData(DirectCast(Session("LostSaleData"), DataSet))

                    setDefaultValues(DirectCast(Session("LostSaleData"), DataSet))

                ElseIf ddlSaleLast12.SelectedIndex > 0 Then

                    If DirectCast(Session("ddlSaleLast12Index"), String) = "-1" Or flag = True Then
                        getLSData(Nothing, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                        dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Session("ddlCategoryIndex") = "0"
                        Session("ddlVendorNameIndex") = "0"
                        Session("ddlWlistIndex") = "0"
                        Session("ddlMajorIndex") = "0"
                    Else
                        If DirectCast(Session("ddlSaleLast12Index"), String) <> ddlSaleLast12.SelectedIndex.ToString() Then
                            Dim dsFirst = DirectCast(Session("LostSaleBck"), DataSet)
                            getLSData(dsFirst, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Else
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        End If
                    End If

                    'Dim dsData = If(DirectCast(Session("ddlSaleLast12Index"), String) = "-1", DirectCast(Session("LostSaleData"), DataSet), DirectCast(Session("LostSaleBck"), DataSet))

                    'Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                    Dim dsFilter As DataSet = New DataSet()
                    Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                    Dim valueToCompare As String = ddlSaleLast12.SelectedItem.Text.ToString()
                    For Each dr As DataRow In dsData.Tables(0).Rows
                        If LCase(dr.Item("QTYSOLD").ToString().Trim()) = LCase(valueToCompare.Trim()) Then
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
                            Session("LostSaleData") = dsFilter
                            loadData(dsFilter)
                            'GetLostSalesData("", 1, Nothing, dsFilter)
                            'GetClaimsReport("", 1, dsFilter)
                        Else
                            grvLostSales.DataSource = Nothing
                            grvLostSales.DataBind()

                            methodMessage = "There is not results with the selected criteria."
                            SendMessage(methodMessage, messageType.warning)

                            'updatepnl2.Update()
                        End If
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)

                        'updatepnl2.Update()
                    End If

                    setDefaultValues(dsFilter)
                    'Else
                    'error message
                    Session("ddlSaleLast12Index") = ddlSaleLast12.SelectedIndex.ToString()
                End If

                Session("curSaleLast12") = ddlOption
            Else
                If Session("curSaleLast12") Is Nothing Then
                    Session("curSaleLast12") = ddlOption
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Sub ddlVendorName_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False) Handles ddlVendorName.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim ddlOption = ddlVendorName.SelectedValue
        Try
            Dim dsData = New DataSet()
            'ddlMajor.SelectedIndex = 0
            'ddlWishList.SelectedIndex = 0
            'ddlSaleLast12.SelectedIndex = 0
            'ddlCategory.SelectedIndex = 0
            Session("ddlCategoryIndex") = "0"
            Session("ddlWlistIndex") = "0"
            Session("ddlMajorIndex") = "0"
            Session("ddlSaleLast12Index") = "0"
            Dim ddlPrevious = If(Session("curVndName") IsNot Nothing, Session("curVndName").ToString(), "")

            If (CInt(DirectCast(Session("ddlVendorNameIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlVendorNameIndex"), String)) = -1) Or ddlOption <> ddlPrevious Or flag Then

                If ddlVendorName.SelectedIndex = 0 Then
                    dsData = DirectCast(Session("LostSaleBck"), DataSet)

                    getLSData(dsData, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                    loadData(DirectCast(Session("LostSaleData"), DataSet))

                    setDefaultValues(DirectCast(Session("LostSaleData"), DataSet))

                ElseIf ddlVendorName.SelectedIndex > 0 Then

                    If DirectCast(Session("ddlVendorNameIndex"), String) = "-1" Or flag = True Then
                        getLSData(Nothing, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                        dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Session("ddlCategoryIndex") = "0"
                        Session("ddlWlistIndex") = "0"
                        Session("ddlMajorIndex") = "0"
                        Session("ddlSaleLast12Index") = "0"
                    Else
                        If DirectCast(Session("ddlVendorNameIndex"), String) <> ddlVendorName.SelectedIndex.ToString() Then
                            Dim dsFirst = DirectCast(Session("LostSaleBck"), DataSet)
                            getLSData(dsFirst, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Else
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        End If
                    End If

                    'Dim dsData = If(DirectCast(Session("ddlVendorNameIndex"), String) = "-1", DirectCast(Session("LostSaleData"), DataSet), DirectCast(Session("LostSaleBck"), DataSet))

                    'Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                    Dim dsFilter As DataSet = New DataSet()
                    Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                    Dim valueToCompare As String = ddlVendorName.SelectedItem.Text.ToString()
                    For Each dr As DataRow In dsData.Tables(0).Rows
                        If LCase(dr.Item("VENDORNAME").ToString().Trim()) = LCase(valueToCompare.Trim()) Then
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
                            Session("LostSaleData") = dsFilter
                            loadData(dsFilter)
                            'GetLostSalesData("", 1, Nothing, dsFilter)
                            'GetClaimsReport("", 1, dsFilter)
                        Else
                            grvLostSales.DataSource = Nothing
                            grvLostSales.DataBind()

                            methodMessage = "There is not results with the selected criteria."
                            SendMessage(methodMessage, messageType.warning)

                            'updatepnl2.Update()
                        End If
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)

                        'updatepnl2.Update()
                    End If

                    setDefaultValues(dsFilter)
                    'Else
                    'error message
                    Session("ddlVendorNameIndex") = ddlVendorName.SelectedIndex.ToString()
                End If
                Session("curVndName") = ddlOption
            Else
                If Session("curVndName") Is Nothing Then
                    Session("curVndName") = ddlOption
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Sub ddlCategory_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False) Handles ddlCategory.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim ddlOption = ddlCategory.SelectedValue
        Try
            Dim dsData = New DataSet()
            'ddlMajor.SelectedIndex = 0
            'ddlWishList.SelectedIndex = 0
            'ddlSaleLast12.SelectedIndex = 0
            'ddlVendorName.SelectedIndex = 0
            Session("ddlVendorNameIndex") = "0"
            Session("ddlWlistIndex") = "0"
            Session("ddlMajorIndex") = "0"
            Session("ddlSaleLast12Index") = "0"
            Dim ddlPrevious = If(Session("curCategory") IsNot Nothing, Session("curCategory").ToString(), "")


            If (CInt(DirectCast(Session("ddlCategoryIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlCategoryIndex"), String)) = -1) Or ddlOption <> ddlPrevious Or flag Then

                If ddlCategory.SelectedIndex = 0 Then
                    'Dim ddlOption = ddlVendAssign.SelectedValue
                    dsData = DirectCast(Session("LostSaleBck"), DataSet)

                    getLSData(dsData, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                    loadData(DirectCast(Session("LostSaleData"), DataSet))

                    setDefaultValues(DirectCast(Session("LostSaleData"), DataSet))

                ElseIf ddlCategory.SelectedIndex > 0 Then

                    If DirectCast(Session("ddlCategoryIndex"), String) = "-1" Or flag = True Then
                        getLSData(Nothing, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                        dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Session("ddlVendorNameIndex") = "0"
                        Session("ddlWlistIndex") = "0"
                        Session("ddlMajorIndex") = "0"
                        Session("ddlSaleLast12Index") = "0"
                    Else
                        If DirectCast(Session("ddlCategoryIndex"), String) <> ddlCategory.SelectedIndex.ToString() Then
                            Dim dsFirst = DirectCast(Session("LostSaleBck"), DataSet)
                            getLSData(dsFirst, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        Else
                            dsData = DirectCast(Session("LostSaleData"), DataSet)
                        End If
                    End If

                    'dsData = If(DirectCast(Session("ddlCategoryIndex"), String) = "-1", DirectCast(Session("LostSaleData"), DataSet), DirectCast(Session("LostSaleBck"), DataSet))

                    'Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                    Dim dsFilter As DataSet = New DataSet()
                    Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                    Dim valueToCompare As String = ddlCategory.SelectedItem.Text.ToString()
                    For Each dr As DataRow In dsData.Tables(0).Rows
                        If LCase(dr.Item("CATDESC").ToString().Trim()) = LCase(valueToCompare.Trim()) Then
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
                            Session("LostSaleData") = dsFilter
                            loadData(dsFilter)
                            'GetLostSalesData("", 1, Nothing, dsFilter)
                            'GetClaimsReport("", 1, dsFilter)
                        Else
                            grvLostSales.DataSource = Nothing
                            grvLostSales.DataBind()

                            methodMessage = "There is not results with the selected criteria."
                            SendMessage(methodMessage, messageType.warning)

                            'updatepnl2.Update()
                        End If
                    Else
                        grvLostSales.DataSource = Nothing
                        grvLostSales.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)

                        'updatepnl2.Update()
                    End If

                    setDefaultValues(dsFilter)
                    'Else
                    'error message
                    Session("ddlCategoryIndex") = ddlCategory.SelectedIndex.ToString()

                End If
                Session("curCategory") = ddlOption
            Else
                If Session("curCategory") Is Nothing Then
                    Session("curCategory") = ddlOption
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub rdWL_CheckedChanged(sender As Object, e As EventArgs)
        If Session("ddlWlistIndex") IsNot Nothing Then
            If CInt(DirectCast(Session("ddlWlistIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlWlistIndex"), String)) = -1 Then
                Load_Combos_inGrid(ddlWishList)
            End If
        End If
    End Sub

    Protected Sub rdVndName_CheckedChanged(sender As Object, e As EventArgs)
        If Session("ddlVendorNameIndex") IsNot Nothing Then
            If CInt(DirectCast(Session("ddlVendorNameIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlVendorNameIndex"), String)) = -1 Then
                Load_Combos_inGrid(ddlVendorName)
            End If
        End If
    End Sub

    Protected Sub rdLast12_CheckedChanged(sender As Object, e As EventArgs)
        If Session("ddlSaleLast12Index") IsNot Nothing Then
            If CInt(DirectCast(Session("ddlSaleLast12Index"), String)) = 0 Or CInt(DirectCast(Session("ddlSaleLast12Index"), String)) = -1 Then
                Load_Combos_inGrid(ddlSaleLast12)
            End If
        End If
    End Sub

    Protected Sub rdMajor_CheckedChanged(sender As Object, e As EventArgs)
        If Session("ddlMajorIndex") IsNot Nothing Then
            If CInt(DirectCast(Session("ddlMajorIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlMajorIndex"), String)) = -1 Then
                Load_Combos_inGrid(ddlMajor)
            End If
        End If
    End Sub

    Protected Sub rdCategory_CheckedChanged(sender As Object, e As EventArgs)
        Try
            If Session("ddlCategoryIndex") IsNot Nothing Then
                If CInt(DirectCast(Session("ddlCategoryIndex"), String)) = 0 Or CInt(DirectCast(Session("ddlCategoryIndex"), String)) = -1 Then
                    Load_Combos_inGrid(ddlCategory)
                End If
            End If
        Catch ex As Exception
            Dim exMessage = ex.Message
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub


#End Region

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
        Dim dsSetDataSource = New DataSet()
        Try
            grvLostSales.PageIndex = e.NewPageIndex


            Session("currentPage") = (CInt(e.NewPageIndex + 1) * 10) - 9
            Session("PageAmounts") = If((CInt(e.NewPageIndex + 1) * 10) > CInt(DirectCast(Session("ItemCounts"), String)), CInt(DirectCast(Session("ItemCounts"), String)), (CInt(e.NewPageIndex + 1) * 10))

            Dim ds = DirectCast(Session("LostSaleData"), DataSet)

            If ds IsNot Nothing Then
                grvLostSales.DataSource = ds.Tables(0)
            Else
                Dim grid = DirectCast(sender, GridView)
                Dim dtGrid = TryCast(grid.DataSource, DataTable)
                grvLostSales.DataSource = dtGrid
            End If
            grvLostSales.DataBind()

            setDefaultValues(ds)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub grvLostSales_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Dim exMessage As String = " "
        Dim methodMessage As String = Nothing
        Dim lstReferences As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim countReferences As Integer = 0
        Dim flagError As Integer = 0
        Dim updateError As Integer = 0
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
                        Session("dctSelectedParts") = lstReferences
                        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                            For Each item In lstReferences
                                Dim result = objBL.InsertWishListReference(item.Value, item.Key, "1", "1", "QS36F.PRDWL", "WHLCODE")
                                'status when add to wish list
                                If result > 0 Then

                                    'check if part is backed up before
                                    Dim flagExists = GetLSBackData(item.Key)
                                    If Not flagExists Then
                                        'if not, backup the part in process
                                        Dim rsInsert = SaveLSItemInProcess("WSH")
                                    Else
                                        Dim rsUpdate = UpdateLSBackData400(item.Key, "WSH", item.Value)
                                    End If

                                    countReferences += 1
                                    Dim resultMethod = updateLostSaleGridView(item.Key, True)
                                    If resultMethod <> 0 Then
                                        updateError += 1
                                    End If

                                Else
                                    flagError += 1
                                End If
                            Next
                        End Using

                        Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                        loadData(ds)
                        setDefaultValues(ds)

                        If flagError > 0 Then
                            methodMessage = "There is an error in the insertion process."
                            SendMessage(methodMessage, messageType.Error)
                        Else
                            Dim extraMessage = " Should be an error updating the gridview. Please refresh the data."
                            methodMessage = "Successful Insertion for " + countReferences.ToString() + " record."
                            If updateError > 0 Then
                                methodMessage += extraMessage
                            End If
                            SendMessage(methodMessage, messageType.success)
                        End If
                    End If
                End If

            ElseIf e.CommandName = "SingleAdd" Then
                Dim ds As DataSet = New DataSet()
                Dim row As GridViewRow = DirectCast(e.CommandSource, LinkButton).Parent.Parent

                Dim dataFrom = row.Cells(2)
                Dim myLabel As Label = DirectCast(dataFrom.FindControl("txtPartName"), Label)

                Dim partNo As String = myLabel.Text.Trim()
                Dim userid As String = If(Not String.IsNullOrEmpty(row.Cells(23).Text) And row.Cells(23).Text <> "&nbsp;", row.Cells(23).Text, "N/A")
                lstReferences.Add(partNo, userid)
                Session("dctSelectedParts") = lstReferences

                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                    Dim existsPart = objBL.GetPartInWishList(partNo, ds)
                    Dim result As Integer = 0
                    If existsPart = 0 Then
                        result = objBL.InsertWishListReference(userid, partNo, "1", "1", "QS36F.PRDWL", "WHLCODE")

                        'status when add to wish list
                        countReferences = result
                        If result > 0 Then

                            'check if part is backed up before
                            Dim flagExists = GetLSBackData(partNo)
                            If Not flagExists Then
                                'if not, backup the part in process
                                Dim rsInsert = SaveLSItemInProcess("WSH")
                            Else
                                Dim rsUpdate = UpdateLSBackData400(partNo, "WSH", userid)
                            End If

                            methodMessage = "Successful Insertion for " + result.ToString() + " record."

                            Dim resultMethod = updateLostSaleGridView(partNo)
                            If resultMethod = 0 Then
                                SendMessage(methodMessage, messageType.success)
                            End If
                        Else
                            methodMessage = "There is an error in the insertion process."
                            SendMessage(methodMessage, messageType.Error)
                        End If
                    Else
                        Dim resultMethod = updateLostSaleGridView(partNo)
                        If resultMethod = 0 Then
                            methodMessage = "There is already a reference of the part " + partNo.Trim() + " in Wishlist. This reference will be removed from this screen."
                            SendMessage(methodMessage, messageType.warning)
                        End If
                        Exit Sub
                    End If

                End Using
            ElseIf e.CommandName = "UpdatePart" Then
                'GridViewRow row = (GridViewRow)(e.CommandSource As LinkButton).Parent.Parent;
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)

                Dim tempDictionary = GetCheckboxesDisp()
                If tempDictionary.Count = 0 Then
                    Dim dataFrom = row.Cells(2)
                    Dim myLabel As Label = DirectCast(dataFrom.FindControl("txtPartName"), Label)
                    Dim userid As String = If(Not String.IsNullOrEmpty(row.Cells(23).Text) And row.Cells(23).Text <> "&nbsp;", row.Cells(23).Text, "N/A")
                    lblSelectedPart.Text = "The selected part is: " + myLabel.Text.Trim()

                    lstReferences.Add(myLabel.Text.Trim(), userid)
                    Session("dctSelectedParts") = lstReferences
                ElseIf tempDictionary.Count = 1 Then
                    lstReferences = tempDictionary
                    Session("dctSelectedParts") = lstReferences
                    Dim pp = lstReferences.Keys(0)
                    lblSelectedPart.Text = "The selected part is: " + pp.ToString().Trim()
                    hdShowUserAssignment.Value = "1"
                    ddlUser2.SelectedIndex = 0
                Else
                    methodMessage = "At this moment the application only assign users one by one. Please set the selection to one checkbox only."
                    hdShowUserAssignment.Value = "0"
                    ddlUser2.SelectedIndex = 0
                    SendMessage(methodMessage, messageType.info)
                End If


                'txtPartNumber2.Text = Trim(myLabel.Text)
                'txtPartNumber2.Enabled = False

                'hdWhlCode1.Value = row.Cells(2).Text

                'Dim assigned As String = Trim(row.Cells(8).Text)
                'ddlAssignedTo.SelectedIndex = ddlAssignedTo.Items.IndexOf(ddlAssignedTo.Items.FindByText(assigned))

                'Dim status As String = row.Cells(7).Text
                'ddlStatus2.SelectedIndex = ddlStatus2.Items.IndexOf(ddlStatus2.Items.FindByText(status))

                'txtComments2.Text = GetCommentById(Trim(row.Cells(2).Text))
            ElseIf e.CommandName = "show" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)
                'Dim row1 As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).NamingContainer, GridViewRow)
                Dim id = row.Cells(2).Text

                Dim dataFrom = row.Cells(2)
                Dim MyPartNo As Label = DirectCast(dataFrom.FindControl("txtPartName"), Label)
                'Dim id1 = row.Cells(2).Text

                Dim gvv = DirectCast(sender, GridView)
                Dim pepe = gvv.EditIndex

                Dim ds1 = DirectCast(Session("LostSaleData"), DataSet)

                Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) item.Item("IMPTN").ToString().Equals(MyPartNo.Text, StringComparison.InvariantCultureIgnoreCase))
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
                'updatepnl.Update()
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub grvLostSales_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Dim exMessage As String = Nothing

        'Try
        '    Dim dsss = New DataSet()
        '    dsss = DirectCast(Session("LostSaleData"), DataSet)

        '    Dim roww As GridViewRow = grvLostSales.Rows(e.RowIndex)
        '    dsss.Tables(0).Rows(roww.DataItemIndex)("WLIST") = roww.Cells(14).Text

        '    grvLostSales.DataBind()
        'Catch ex As Exception
        '    exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        'End Try

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


                'Dim lbl1 = DirectCast(e.Row.FindControl("lblTCountries"), Label)
                'lbl1.Text = "country"

                'Dim lbl2 = DirectCast(e.Row.FindControl("lblOEMPart"), Label)
                'lbl2.Text = "OEM"


                'execute subqueries for every row
                'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                '    Dim partNo = e.Row.Cells(2).Text.Trim()
                '    Dim factor = ConfigurationManager.AppSettings("yearFactor")
                '    Dim vendorNo = If(String.IsNullOrEmpty(e.Row.Cells(10).Text) Or e.Row.Cells(10).Text = "&nbsp;", "000000", e.Row.Cells(10).Text.Trim())

                '    'total client
                '    'Dim tclients = objBL.GetTotalClients(partNo, factor)
                '    'Dim lbl = DirectCast(e.Row.FindControl("lblTClients"), Label)
                '    'lbl.Text = tclients

                '    'total country
                '    Dim tcountries = objBL.GetTotalCountries(partNo, factor)
                '    Dim lbl1 = DirectCast(e.Row.FindControl("lblTCountries"), Label)
                '    lbl1.Text = tcountries

                '    'oem vendor
                '    Dim lbl2 = DirectCast(e.Row.FindControl("lblOEMPart"), Label)
                '    Dim toempart = ""
                '    If vendorNo = "000000" Then
                '        lbl2.Text = toempart
                '    Else
                '        toempart = objBL.GetOEMPart(partNo, vendorNo)
                '        lbl2.Text = toempart
                '    End If

                '    'Dim ds = DirectCast(Session("LostSaleData"), DataSet)
                '    'If ds IsNot Nothing Then
                '    '    For Each dw As DataRow In ds.Tables(0).Rows
                '    '        If Trim(UCase(dw.Item("IMPTN"))) = Trim(UCase(partNo)) Then
                '    '            'dw.Item("totalclients") = tclients
                '    '            'dw.Item("totalcountry") = tcountries
                '    '            'dw.Item("oemvendor") = toempart
                '    '            Exit For
                '    '        End If
                '    '    Next

                '    '    Session("LostSaleData") = ds
                '    'End If

                'End Using

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
                'chk.view
                AddHandler chk.CheckedChanged, AddressOf chkAll_CheckedChanged
                If chk.Checked = False Then
                    Session("chkUnselect") = chk
                End If
                'If Session("AllSelected") IsNot Nothing Then
                '    chkAll_CheckedChanged(Nothing, Nothing)
                'End If
            ElseIf (e.Row.RowType = DataControlRowType.Footer) Then

                Dim lstValuesFoot = DirectCast(Session("grvLostSalesHeaders"), List(Of String))
                If lstValuesFoot.Count > 0 Then
                    Dim x As Integer = 0
                    For Each item As String In lstValuesFoot
                        'If Trim(item.ToUpper()) = "SALES LAST12" Then
                        '    ''fill_SalesLast12(ddlSaleLast12Foot)
                        '    ''AddHandler ddlSaleLast12Foot.SelectedIndexChanged, AddressOf ddlSaleLast12Foot_SelectedIndexChanged
                        '    ''e.Row.Cells(x).Controls.Add(ddlSaleLast12Foot)

                        '    'e.Row.Cells(x).Text = item
                        '    'e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    'x += 1

                        '    'ddlSaleLast12Foot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlSaleLast12Foot"), String)), ddlSaleLast12Foot.Items.IndexOf(ddlSaleLast12Foot.Items.FindByText(DirectCast(Session("flagDdlSaleLast12Foot"), String))), "0")

                        'ElseIf Trim(item.ToUpper()) = "VND NAME" Then
                        '    ''fill_VndName(ddlVndNameFoot)
                        '    ''AddHandler ddlVndNameFoot.SelectedIndexChanged, AddressOf ddlVndNameFoot_SelectedIndexChanged
                        '    ''e.Row.Cells(x).Controls.Add(ddlVndNameFoot)

                        '    'e.Row.Cells(x).Text = item
                        '    'e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    'x += 1

                        '    'ddlVndNameFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlVndNameFoot"), String)), ddlVndNameFoot.Items.IndexOf(ddlVndNameFoot.Items.FindByText(DirectCast(Session("flagDdlVndNameFoot"), String))), "0")

                        'ElseIf Trim(item.ToUpper()) = "WL" Then
                        '    ''fill_WL(ddlWLFoot)
                        '    ''AddHandler ddlWLFoot.SelectedIndexChanged, AddressOf ddlWLFoot_SelectedIndexChanged
                        '    ''e.Row.Cells(x).Controls.Add(ddlWLFoot)

                        '    'e.Row.Cells(x).Text = item
                        '    'e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    'x += 1

                        '    'ddlWLFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlWLFoot"), String)), ddlWLFoot.Items.IndexOf(ddlWLFoot.Items.FindByText(DirectCast(Session("flagDdlWLFoot"), String))), "0")

                        'ElseIf Trim(item.ToUpper()) = "MAJOR" Then
                        '    ''fill_Major(ddlMajorFoot)
                        '    ''AddHandler ddlMajorFoot.SelectedIndexChanged, AddressOf ddlMajorFoot_SelectedIndexChanged
                        '    ''e.Row.Cells(x).Controls.Add(ddlMajorFoot)

                        '    'e.Row.Cells(x).Text = item
                        '    'e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    'x += 1

                        '    'ddlMajorFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlMajorFoot"), String)), ddlMajorFoot.Items.IndexOf(ddlMajorFoot.Items.FindByText(DirectCast(Session("flagDdlMajorFoot"), String))), "0")

                        'ElseIf Trim(item.ToUpper()) = "CATEGORY" Then
                        '    'e.Row.Cells(x).Text = item
                        '    'e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    'x += 1

                        'Else
                        If item = "DESCRIPTION 2" Then
                            x += 1
                        ElseIf item = "DESCRIPTION 3" Then
                            x += 1
                        ElseIf item = "DESC" Then
                            x += 1
                        ElseIf item = "MINOR" Then
                            x += 1
                        ElseIf item = "" Then
                            Dim btn As LinkButton = DirectCast(e.Row.FindControl("ButtonAdd"), LinkButton)
                            'AddHandler btn.Click, AddressOf ButtonAdd_Click
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

                Dim strTotal = DirectCast(Session("ItemCounts"), String)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub grvLostSales_Sorting(sender As Object, e As GridViewSortEventArgs)
        Dim dtw As DataView = Nothing
        Dim newDt As DataTable = New DataTable()
        Dim direction As String = Nothing
        Dim exMessage As String = Nothing

        Try
            direction = DirectCast(Session("sortDirection"), String)
            Dim dsFull = DirectCast(Session("LostSaleData"), DataSet)
            Dim dt As DataTable = DirectCast(grvLostSales.DataSource, DataTable)
            Dim field = e.SortExpression
            If dt IsNot Nothing Then

                Dim num = 0

                If SetSortDirection(direction) = "ASC" Then
                    'Dim dtQuery = dt.AsEnumerable().Where(Function(ee) Integer.TryParse(ee.Item(field).ToString(), num) = True).CopyToDataTable()
                    grvLostSales.DataSource = dt.AsEnumerable().OrderBy(Function(o) CInt(o.Item(field).ToString())).CopyToDataTable()
                Else
                    grvLostSales.DataSource = dt.AsEnumerable().OrderByDescending(Function(o) CInt(o.Item(field).ToString())).CopyToDataTable()
                End If

#Region "Old"

                ' dtw = New DataView(dt)
                '    direction = DirectCast(Session("sortDirection"), String)
                '    dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                '    newDt = dtw.ToTable()



                '    Dim ds As DataSet = New DataSet()
                '    ds.Tables.Add(newDt)
                '    Session("LostSaleData") = ds
                '    grvLostSales.DataSource = ds
                '    grvLostSales.DataBind()

                '    'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

#End Region

            Else

                If SetSortDirection(direction) = "ASC" Then
                    grvLostSales.DataSource = dsFull.Tables(0).AsEnumerable().OrderBy(Function(o) CInt(o.Item(field).ToString())).CopyToDataTable()
                Else
                    grvLostSales.DataSource = dsFull.Tables(0).AsEnumerable().OrderByDescending(Function(o) CInt(o.Item(field).ToString())).CopyToDataTable()
                End If

#Region "old"

                '    Dim ds As DataSet = New DataSet()
                '    ds = getDataSource()
                '    dtw = New DataView(ds.Tables(0))
                '    direction = DirectCast(Session("sortDirection"), String)
                '    dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                '    newDt = dtw.ToTable()
                '    ds.Tables.RemoveAt(0)
                '    ds.Tables.Add(newDt)
                '    Session("LostSaleData") = ds
                '    grvLostSales.DataSource = ds
                '    grvLostSales.DataBind()

                'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

#End Region

            End If

            grvLostSales.DataBind()

            Dim dtt = DirectCast(grvLostSales.DataSource, DataTable)
            Dim ds = New DataSet()
            ds.Tables.Add(dtt)
            Session("LostSaleData") = ds

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub grvLostSales_PreRender(sender As Object, e As EventArgs)
        Try
            Dim pepe = "test time elapssed after databound"
            If Session("chkUnselect") IsNot Nothing Then
                Session("AllSelected") = False
                chkAll_CheckedChanged(Nothing, Nothing)
            End If
            Dim cc = "aaaaa"
        Catch ex As Exception

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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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

    Protected Sub btnUpdate3_Click(sender As Object, e As EventArgs) Handles btnUpdate3.Click
        Dim exMessage As String = Nothing
        Dim updatedReferences As Integer = 0
        Dim methodMessage As String = String.Empty
        Dim lstReferences As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim partNo As String = Nothing
        Try
            'update the datasource
            'refresh the gridview
            Dim userSelected = DirectCast(Session("PERPECHUSER"), String)
            Dim ds As DataSet = New DataSet()
            Dim ds1 As DataSet = New DataSet()
            ds = DirectCast(Session("LostSaleData"), DataSet)
            lstReferences = DirectCast(Session("dctSelectedParts"), Dictionary(Of String, String))
            'Dim lstReferencesCopy = lstReferences

            If lstReferences.Count > 1 Then
                'Dim itt As Integer = 0
                'For Each dc1 In lstReferences
                '    Dim temp = lstReferencesCopy.Keys(itt)
                '    lstReferencesCopy(temp) = userSelected
                '    itt += 1
                'Next
            ElseIf lstReferences.Count > 0 And lstReferences.Count < 2 Then
                Dim pp = lstReferences.Keys(0)
                lstReferences(pp) = userSelected
            End If

            If lstReferences.Count > 0 And lstReferences.Count < 2 Then
                partNo = LCase(lblSelectedPart.Text.Split(":")(1).ToString().Trim())
                For Each dw As DataRow In ds.Tables(0).Rows
                    If LCase(dw.Item("IMPTN").ToString().Trim()) = partNo Then
                        dw.Item("PrPech") = userSelected
                        Exit For
                    End If
                Next
                ds.Tables(0).AcceptChanges()
                loadData(ds)
            Else
                Dim lengthdct = lstReferences.Count
                Dim iterator = 0
                For Each dc In lstReferences
                    Dim ptn = LCase(dc.Key.Trim())
                    partNo = ptn
                    Dim usr = dc.Value
                    For Each dww As DataRow In ds.Tables(0).Rows
                        If LCase(dww.Item("IMPTN").ToString().Trim()) = ptn Then
                            dww.Item("PrPech") = userSelected
                            iterator += 1
                            Exit For
                        End If
                    Next
                Next
                ds.Tables(0).AcceptChanges()
                loadData(ds)
            End If

            Dim flagExists = GetLSBackData(partNo, ds1)
            If Not flagExists Then
                'if not, backup the part in process
                Dim rsInsert = SaveLSItemInProcess()
            Else
                Dim rsUpdate = UpdateLSBackData400(partNo, "NEW", userSelected)
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub submit_Click(sender As Object, e As EventArgs) Handles submit.Click
        Dim exMessage As String = Nothing
        Dim lstData = New List(Of LostSales)()
        Dim filterData = New List(Of LostSales)()
        Dim dsWork As DataSet = New DataSet()
        Dim searchstring As String = Nothing
        Try
            If Not String.IsNullOrEmpty(tqId.Text) Or getIfCheckedQuote() Then
                searchstring = If(CInt(getStrTQouteCriteria()), getStrTQouteCriteria(), "0")

                'Dim dsData = getDataSource(True)
                'load lostsalebck
                Dim dsData = DirectCast(Session("LostSaleBck"), DataSet)
                If dsData IsNot Nothing Then

                    getLSData(Nothing, CInt(searchstring), Session("flagVnd").ToString())

                    'If dsData.Tables(0).Rows.Count > 0 Then
                    '    lstData = fillObj(dsData.Tables(0))
                    'End If
                Else
                    'message loading full data
                    SendMessage("The full data is not loaded yet. Please try in few seconds.", messageType.info)

                    'Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                    getLSData(Nothing, CInt(searchstring), Session("flagVnd").ToString())

                    'grvLostSales.DataSource = dsOut
                    'grvLostSales.DataBind()

                    'setDefaultValues(dsOut)
                    'updatepnl1.Update()
                    'Exit Sub

                    'Dim dtData = DirectCast(grvLostSales.DataSource, DataTable)
                    'lstData = fillObj(dtData)
                End If

                'all ocurrences without duplicate value string
                Dim dsCurData = DirectCast(Session("LostSaleData"), DataSet)
                If dsCurData Is Nothing Then
                    SendMessage("There is no data with the filtered options. Please select other filter criteria!", messageType.warning)
                    Exit Sub
                End If
                lstData = fillObj(dsCurData.Tables(0))
                filterData = lstData.Where(Function(da) _
                                               If(Not String.IsNullOrEmpty(da.TIMESQ), CInt(da.TIMESQ) >= CInt(searchstring), False)).ToList()


                'UCase(da.TIMESQ).Trim().Contains(UCase(searchstring))
                'CInt(da.TIMESQ) > CInt(searchstring)
                If filterData.Count > 0 Then
                    Dim dtResult = ListToDataTable(filterData)
                    If dtResult IsNot Nothing Then
                        If dtResult.Rows.Count > 0 Then
                            Dim ds = New DataSet()
                            ds.Tables.Add(dtResult)
                            loadData(ds)
                            'GetLostSalesData(Nothing, 1, Nothing, ds)
                        End If
                    End If

                    setDefaultValues(DirectCast(Session("LostSaleData"), DataSet))
                Else
                    'no filtered data

                    setDefaultValues(Nothing)
                    SendMessage("There is no data with the filtered options. Please select other filter criteria!", messageType.warning)

                    'restore grid and message 
                    'load lostsaledata
                    'Dim dsLoad = DirectCast(Session("LostSaleBck"), DataSet)
                    'If dsLoad IsNot Nothing Then
                    '    If dsLoad.Tables(0).Rows.Count > 0 Then
                    '        GetLostSalesData(Nothing, 1, Nothing, dsLoad)
                    '    Else
                    '        GetLostSalesData(Nothing, 1, dsWork)
                    '    End If
                    'Else
                    '    GetLostSalesData(Nothing, 1, dsWork)
                    'End If
                End If
            Else
                'no action message
                setDefaultValues(Nothing)
                SendMessage("Please select a times quote again!", messageType.warning)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub ButtonAdd_Click(sender As Object, e As EventArgs)
        Try
            For Each row As GridViewRow In grvLostSales.Rows
                Dim pepe = row
                Dim pp = "ee"
            Next
        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pepe = Nothing
    End Sub

    Protected Sub btnRestore_Click(sender As Object, e As EventArgs) Handles btnRestore.Click
        Dim exMessage As String = Nothing
        Dim dsResult As DataSet = New DataSet()
        Try

            Dim strPagValues = getLimit()
            GetLostSalesData(strPagValues, 0, dsResult)

            Dim timesQDefault As String = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("timesQuoteDefault")), ConfigurationManager.AppSettings("timesQuoteDefault"), "30")
            Dim vndSelDefault As String = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("vndSelectionDefault")), ConfigurationManager.AppSettings("vndSelectionDefault"), "2")
            Session("flagVnd") = vndSelDefault
            Session("TimesQuote") = timesQDefault

            getLSData(dsResult, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())

            Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
            loadData(dsData)

            setDefaultValues(dsData)

#Region "Thread"

            launchSecondaryThread()

#End Region

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Try
            Dim dsResult = DirectCast(Session("LostSaleData"), DataSet)
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then

                    Dim pathToProcess = ConfigurationManager.AppSettings("urlLSExcelOutput")
                    'Dim updUserPath = userPath + "\WishList-Template\"
                    Dim folderPath = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("urlLSExcelOutput")), ConfigurationManager.AppSettings("urlLSExcelOutput"), "")
                    Dim methodMessage = If(Not String.IsNullOrEmpty(folderPath), "The template document will be downloaded to your documents folder", "There is not a path defined for this document. Call an administrator!!")

                    'Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    'Dim folderPath As String = userPath & "\Lost_Sale_Data\"

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    Else
                        Dim files = Directory.GetFiles(pathToProcess)
                        Dim fi = Nothing
                        If files.Length = 1 Then
                            For Each item In files
                                fi = item
                                Dim isOpened = IsFileinUse(New FileInfo(fi))
                                If Not isOpened Then
                                    File.Delete(item)
                                Else
                                    SendMessage("Please close the file " & fi & " in order to proceed!", messageType.info)
                                    Exit Sub
                                End If
                            Next
                        Else
                            'SendMessage("Please close the file " & fi & " in order to proceed!", messageType.info)
                            'Exit Sub
                        End If
                    End If

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            Exit Sub
                        End If

                        Dim title As String
                        title = "Lost_Sale_Output_Generated_by "
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


                        Dim newLocalFile As FileInfo = New FileInfo(fullPath)
                        If newLocalFile.Exists Then
                            Try
                                Session("filePathLSExcelOutput") = fullPath
                                Response.Redirect("DownloadLSExcelOutput.ashx", True)
                                'Process.Start("explorer.exe", localFilePath)
                            Catch Win32Exception As Win32Exception
                                Shell("explorer " & fullPath, AppWinStyle.NormalFocus)
                            Catch ex As Exception
                                writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + ex.Message + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
                            End Try
                        End If

                        ''Dim methodMessage = "The template document will be downloaded to your documents folder"
                        ''SendMessage(methodMessage, messageType.info)
                        'Dim rsConfirm As DialogResult = MessageBox.Show("The file was created successfully in this path " & folderPath & " .Do you want to open the created document location?", "CTP System", MessageBoxButtons.YesNo)
                        'If rsConfirm = DialogResult.Yes Then
                        '    Try
                        '        Process.Start("explorer.exe", folderPath)
                        '    Catch Win32Exception As Win32Exception
                        '        Shell("explorer " & folderPath, AppWinStyle.NormalFocus)
                        '    End Try
                        'End If
                    End If

                    loadData(dsResult)

                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub btnPdf_Click(sender As Object, e As EventArgs) Handles btnPdf.Click
        Dim exMessage As String = Nothing
        Dim methodMessage As String = String.Empty
        Dim lstReferences As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Try

            'lstReferences = GetCheckboxesDisp()
            'If lstReferences Is Nothing Then
            '    methodMessage = "An exception occur in the method execution!"
            '    SendMessage(methodMessage, messageType.Error)
            'Else
            '    If lstReferences.Count = 0 Then
            '        methodMessage = "Please select the items that you want to update and then click this button!"
            '        SendMessage(methodMessage, messageType.warning)
            '    Else
            '        Session("dctSelectedParts") = lstReferences
            '        hdShowUserAssignment.Value = "1"
            '        ddlUser2.SelectedIndex = 0
            '    End If
            'End If


#Region "PDF WORK"

            'Dim dsResult = DirectCast(Session("LostSaleData"), DataSet)
            'If dsResult IsNot Nothing Then
            '    If dsResult.Tables(0).Rows.Count > 0 Then
            '        loadData(dsResult)
            '    End If
            'End If
            'Dim dtGrid = DirectCast(grvLostSales.DataSource, DataTable)
            'If dtGrid IsNot Nothing Then
            '    If dtGrid.Rows.Count > 0 Then
            '        exportpdf(dtGrid)
            '    End If
            'End If

#End Region            '

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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

                'Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                'If dsData IsNot Nothing Then
                '    loadData(dsData)
                '    setDefaultValues(dsData)
                'Else
                'dsData = DirectCast(Session("LostSaleBck"), DataSet)
                Dim timesQ = DirectCast(Session("TimesQuote"), String)
                Dim vndSel = DirectCast(Session("flagVnd"), String)
                getLSData(Nothing, CInt(timesQ), vndSel)

                Dim dss = DirectCast(Session("LostSaleData"), DataSet)
                loadData(dss)
                setDefaultValues(dss)
                updatePagerSettings(grvLostSales)
                'End If

                'Dim dsDataRecover = If((DirectCast(Session("LostSaleBck"), DataSet)) IsNot Nothing, DirectCast(Session("LostSaleBck"), DataSet),
                '                            If(GetLostSalesData(Nothing, 0, dsWork) > 0, dsWork, Nothing))
                'Dim dsDataRecover = If((DirectCast(Session("LostSaleData"), DataSet)) IsNot Nothing, DirectCast(Session("LostSaleData"), DataSet),
                '                            If(GetLostSalesData(Nothing, 0, dsWork) > 0, dsWork, Nothing))
                'If dsWork Is Nothing Then
                '    'message
                '    Exit Sub
                'End If
                'GetLostSalesData(Nothing, 1, Nothing, dsDataRecover)
                'Session("flagVnd") = "4"
                'ddlVendAssign_SelectedIndexChanged(Nothing, Nothing)

            Else
                'Dim dsData = getDataSource(True)
                Dim flag As Integer = 0
                Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
                'dsData = If((DirectCast(Session("LostSaleData"), DataSet)) IsNot Nothing, DirectCast(Session("LostSaleData"), DataSet), Nothing)
                If dsData IsNot Nothing Then
                    If dsData.Tables(0).Rows.Count > 0 Then
                        lstData = fillObj(dsData.Tables(0))
                    End If
                Else
                    dsData = DirectCast(Session("LostSaleBck"), DataSet)
                    lstData = fillObj(dsData.Tables(0))
                    flag = 1
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
                                               Or If(Not String.IsNullOrEmpty(da.CATDESC), UCase(da.CATDESC).Trim().Contains(UCase(searchstring)), False) _
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

                            If flag = 0 Then
                                loadData(ds)
                                setDefaultValues(ds)
                            Else
                                Dim timesQ = DirectCast(Session("TimesQuote"), String)
                                Dim vndSel = DirectCast(Session("flagVnd"), String)
                                getLSData(Nothing, CInt(timesQ), vndSel)

                                Dim dss = DirectCast(Session("LostSaleData"), DataSet)
                                loadData(dss)
                                setDefaultValues(dss)
                            End If

                        Else
                            loadData(Nothing)
                            SendMessage("There is no data with the selected criteria!", messageType.warning)
                        End If
                    End If

                    updatePagerSettings(grvLostSales)

                Else
                    loadData(Nothing)
                    SendMessage("There is no data with the selected criteria!", messageType.warning)

                    'restore grid and message 
                    'Dim dsLoad = DirectCast(Session("LostSaleBck"), DataSet)
                    'If dsLoad IsNot Nothing Then
                    '    If dsLoad.Tables(0).Rows.Count > 0 Then
                    '        GetLostSalesData(Nothing, 1, Nothing, dsLoad)
                    '    Else
                    '        GetLostSalesData(Nothing, 1, dsWork)
                    '    End If
                    'Else
                    '    GetLostSalesData(Nothing, 1, dsWork)
                    'End If
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub lnkReloadBack_Click(sender As Object, e As EventArgs) Handles lnkReloadBack.Click
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("LostSaleBck"), DataSet)

            If dsData IsNot Nothing Then
                getLSData(dsData, CInt(Session("TimesQuote").ToString()), Session("flagVnd").ToString())

                Session("ItemCounts") = (DirectCast(Session("LostSaleData"), DataSet)).Tables(0).Rows.Count.ToString()
                Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                grvLostSales.DataSource = dsOut
                grvLostSales.DataBind()

                setDefaultValues(dsOut)
                updatepnl1.Update()
            Else
                SendMessage("The full data is not loaded yet. Please try in few seconds.", messageType.info)

                Dim dsOut = DirectCast(Session("LostSaleData"), DataSet)
                grvLostSales.DataSource = dsOut
                grvLostSales.DataBind()

                setDefaultValues(dsOut)
                updatepnl1.Update()
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Protected Sub lnkReloadGrid_Click(sender As Object, e As EventArgs) Handles lnkReloadGrid.Click
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("LostSaleData"), DataSet)
            'ddlVndNameFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlVndNameFoot"), String)), ddlVndNameFoot.Items.IndexOf(ddlVndNameFoot.Items.FindByText(DirectCast(Session("flagDdlVndNameFoot"), String))), 0)
            'ddlCategoryFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlCategoryFoot"), String)), ddlCategoryFoot.Items.IndexOf(ddlCategoryFoot.Items.FindByText(DirectCast(Session("flagDdlCategoryFoot"), String))), 0)
            'ddlMajorFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlMajorFoot"), String)), ddlMajorFoot.Items.IndexOf(ddlMajorFoot.Items.FindByText(DirectCast(Session("flagDdlMajorFoot"), String))), 0)
            'ddlWLFoot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlWLFoot"), String)), ddlWLFoot.Items.IndexOf(ddlWLFoot.Items.FindByText(DirectCast(Session("flagDdlWLFoot"), String))), 0)
            'ddlSaleLast12Foot.SelectedIndex = If(Not String.IsNullOrEmpty(DirectCast(Session("flagDdlSaleLast12Foot"), String)), ddlSaleLast12Foot.Items.IndexOf(ddlSaleLast12Foot.Items.FindByText(DirectCast(Session("flagDdlSaleLast12Foot"), String))), 0)

            loadData(dsData)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
                    Dim userid As String = If(Not String.IsNullOrEmpty(gvr.Cells(23).Text) And gvr.Cells(23).Text <> "&nbsp;", gvr.Cells(23).Text, "N/A")
                    lstPartsToWL.Add(Trim(gvr.Cells(2).Text), userid)
                    'lstPartsToWL.Add(Trim(gvr.Cells(2).Text))
                Next
            Else
                For Each gvr As GridViewRow In grvLostSales.Rows
                    Dim Check As CheckBox = gvr.FindControl("chkSingleAdd")
                    If Check.Checked Then
                        Dim userid As String = If(Not String.IsNullOrEmpty(gvr.Cells(23).Text) And gvr.Cells(23).Text <> "&nbsp;", gvr.Cells(23).Text, "N/A")

                        Dim dataFrom = gvr.Cells(2)
                        Dim myLabel As Label = DirectCast(dataFrom.FindControl("txtPartName"), Label)

                        lstPartsToWL.Add(myLabel.Text.Trim(), userid)
                    End If
                Next
            End If

            Return lstPartsToWL
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Sub chkAll_CheckedChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            Dim chkAllSelection As String = Nothing
            Dim chkAll As CheckBox = Nothing
            'Dim Repo As CheckBox = (CheckBox)((Control)sender).FindControl("chkRepo");
            If sender Is Nothing Then
                chkAll = DirectCast(Session("chkUnselect"), CheckBox)
            Else
                chkAll = DirectCast(sender, CheckBox)
            End If

            'chkAllSelection = If(Session("AllSelected") IsNot Nothing, Session("AllSelected").ToString(), "")
            'If String.IsNullOrEmpty(chkAllSelection) Then
            '    chkAllSelection = chkAll.Checked.ToString()
            '    Session("AllSelected") = chkAll.Checked
            'Else
            '    chkAll.Checked = System.Convert.ToBoolean(chkAllSelection.Trim())
            'End If

            For Each item As GridViewRow In grvLostSales.Rows
                Dim myControl As CheckBox = CType(item.FindControl(("chkSingleAdd")), CheckBox)
                Dim chk As CheckBox = CType(myControl, CheckBox)
                chk.Checked = If(chkAll.Checked = True, True, False)
            Next
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
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
            'Response.ContentType = "application/octet-stream"
            'Response.AddHeader("Content-Disposition", "attachment; filename=" + name + "_" + DateTime.Now.ToString() + ".pdf")
            'Response.Clear()
            'Response.BinaryWrite(MS.ToArray())
            'Response.End()

            'Dim result As Byte() = MS.ToArray()
            'Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

#End Region

#Region "Logs"

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "CTPSystem" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region

End Class

'Class helperDelegateClass
'    Sub helperDelegateClass()

'    End Sub

'    Protected Sub DelegateCall()
'        Dim c1 As New helperDelegateClass
'        ' Create an instance of the delegate.
'        Dim msd As Lost_Sales.executeFullQuery = AddressOf c1.helperDelegateClass

'        Dim worker As BackgroundWorker = New BackgroundWorker()
'        AddHandler worker.DoWork, Function(sender, e) worker_DoWork(worker)
'        worker.WorkerSupportsCancellation = True
'        worker.WorkerReportsProgress = False
'        AddHandler worker.RunWorkerCompleted, Function(sender, e) worker_WorkerCompleted(worker)
'        worker.RunWorkerAsync()
'        Threading.Thread.Sleep(2000)

'        ' Call the method.
'        msd.Invoke()
'    End Sub


'End Class
