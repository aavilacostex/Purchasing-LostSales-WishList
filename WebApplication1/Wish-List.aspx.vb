Imports System.ComponentModel
Imports System.Configuration
Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Web.Services
Imports ClosedXML.Excel
Imports CTPWEB.DTO

Public Class Wish_List
    Inherits System.Web.UI.Page

    Private Excel03ConString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1};IMEX={2}'"
    Private Excel07ConString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1};IMEX={2}'"
    Private CsvConString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Text;HDR={1};FMT=CSVDelimited'"

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    Private Shared eventLog1 As EventLog = New EventLog("Purchasing.Log", GetComputerName(), "Purchasing.App")
    Private Shared ReadOnly Log As log4net.ILog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)

    Dim objLog = New Logs()

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim exMessage As String = " "
        Dim sel As Integer = -1
        Dim fullData As Boolean = False
        Try
            If Not IsPostBack() Then

                Dim flag = GetAccessByUsers(sel, fullData)
                If Not flag Then
                    If sel = 0 Then
                        'not authorized user
                        Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User: " + usr, " User is not authorized to access to WL. Time: " + DateTime.Now.ToString())
                        Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/", False)
                    ElseIf sel = 1 Then
                        'not user detected (NA)
                        Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, Nothing, "There is not an user detected tryng to access to WL. Time: " + DateTime.Now.ToString())
                        Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/", False)
                    End If

                Else

                    Log.Info("Starting WishList")

                    'get the default session timeout 
                    Dim zeroBased = 0
                    Dim stepBased = (5 * 60 * 1000)
                    Dim firstValue = If(Session IsNot Nothing, Session.Timeout.ToString(), zeroBased.ToString())
                    Session("TimeOutDefined") = (CInt(firstValue) * 60 * 1000).ToString() 'stored in ms
                    hdSessionDefaultTimeOut.Value = Session("TimeOutDefined").ToString()
                    Session("BeforeExpireUserSessionMS") = ((CInt(firstValue) - 5) * 60 * 1000).ToString() '300000 ms
                    hdBeginNotification.Value = Session("BeforeExpireUserSessionMS").ToString()
                    hdSessionToCheckTimeOut.Value = stepBased.ToString() '180000 ms
                    Session("UserInitTime") = DateTime.Now.ToString()

                    'Dim objSession As HttpSessionState = HttpContext.Current.Session

                    'Dim user As String = Environment.UserName
                    'compare environment con parametro de session

                    'test assigmnet
                    'Session("userid") = User
                    'Session("userid") = UCase(user)

                    'Session("PageAmountsDdl") = grvWishList.PageSize
                    'Session("PageIndex") = 1

                    Session("NoPrivilegesUser") = Nothing
                    Session("PageSize") = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PageSize")), ConfigurationManager.AppSettings("PageSize"), "1000")
                    Session("PageAmounts") = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PageAmounts")), ConfigurationManager.AppSettings("PageAmounts"), "10")
                    Session("currentPage") = 1

                    Log.Info("loading combos")
                    LoadCombos()
                    Log.Info("Getting WL Data")

                    If Not fullData Then

                        hdUserVisibility.Value = "1"
                        Dim customUser = UCase(Session("userid").ToString().Trim())
                        Session("NoPrivilegesUser") = customUser
                        'customUser = "CMONTILVA" 'test remove
                        Dim strOptionalQuery As String = " and a3.whlstatusu = '{0}' "
                        Dim sqlResult As String = String.Format(strOptionalQuery, customUser)
                        ddlStatus2.Enabled = False
                        ddlAssignedTo.Enabled = False

                        GetWishListData(0, Nothing, Nothing, sqlResult)
                    Else
                        ddlStatus2.Enabled = True
                        ddlAssignedTo.Enabled = True
                        hdUserVisibility.Value = "0"
                        GetWishListData(0)

                    End If

                    Session("EventRaised") = False

                    Session("ddlAssignIndex") = "-1"
                    Session("ddlFromIndex") = "-1"
                    Session("ddlStatusIndex") = "-1"
                    Session("liSelected") = Nothing

                    Log.Info("Write in logs")
                    Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                    writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + usr, "Login at time: " + DateTime.Now.ToString())


                End If

            Else

                'SendConfirmationMessage("this is s test", "Info")
                Dim controlName As String = Page.Request.Params("__EVENTTARGET")
                If LCase(controlName).Contains("lnkreloadgrid") Then

                    Dim curUser = DirectCast(Session("NoPrivilegesUser"), String)
                    'customUser = "CMONTILVA" 'test remove
                    Dim strOptionalQuery As String = " and a3.whlstatusu = '{0}' "
                    Dim sqlResult As String = String.Format(strOptionalQuery, curUser)

                    GetWishListData(0, Nothing, Nothing, sqlResult)

                Else
                    Session("EventRaised") = True
                    checkInnerDropDownCreated()
                    fieldsVisibility(topFields, False)
                End If

                'LoadCombos()
                'txtWhlCode

            End If
        Catch ex As Exception
            Log.Info("Raise an exception")
            writeComputerEventLog(ex.Message)

            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    Public Sub writeComputerEventLog(Optional strMessage As String = Nothing)
        Dim exMessage As String = Nothing
        Try

            If Not EventLog.SourceExists("Purchasing.App") Then
                EventLog.CreateEventSource("Purchasing.App", "Purchasing.Log")
            End If
            'EventLog.CreateEventSource("CTPSystem-Net", "CTPSystem-Log")

            Dim lgSource = "Purchasing.App"
            Dim lgName = "Purchasing.Log"
            Dim msg = If(String.IsNullOrEmpty(strMessage), "Info: Session started for: " & Environment.UserName, strMessage)

            eventLog1 = New EventLog(lgName, Environment.MachineName, lgSource)
            eventLog1.WriteEntry(msg, EventLogEntryType.Information)

            Log.Info("Info Message: Adding info to console log.")

        Catch ex As Exception
            Log.Error("Error trying to put info un console log: " + ex.Message + ".")
        End Try
    End Sub

    Public Shared Function GetComputerName() As String
        Dim exMessage As String = Nothing
        Try
            Dim ComputerName As String
            ComputerName = Environment.MachineName
            Return ComputerName
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

#End Region

#Region "DropDownList"

#Region "Load DroDownList"

    Protected Sub fill_Types(dwlControl As DropDownList)

        Dim ListItem As ListItem = New ListItem()
        dwlControl.Items.Add(New WebControls.ListItem("New Vendor", "1"))
        dwlControl.Items.Add(New WebControls.ListItem("New Part", "2"))

    End Sub

    Protected Sub fill_From(dwlControl As DropDownList)
        Dim exMessage As String = Nothing
        Dim ds As DataSet = New DataSet()
        Try
            Dim dctValues = New Dictionary(Of String, String)()
            dctValues.Add("LS", "1")
            dctValues.Add("VNDL", "2")
            dctValues.Add("MAN", "3")
            dctValues.Add("EXC", "4")

            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                ds = objBL.GetAllWLFrom()
            End Using

            Dim tblStatuses = New DataTable()
            Dim column1 As DataColumn = New DataColumn("value")
            column1.DataType = System.Type.GetType("System.String")
            Dim column2 As DataColumn = New DataColumn("display")
            column2.DataType = System.Type.GetType("System.String")
            tblStatuses.Columns.Add(column1)
            tblStatuses.Columns.Add(column2)

            'For Each dw As DataRow In ds.Tables(0).Rows
            'Dim val = dw.ItemArray(0).ToString()
            For Each dct In dctValues
                'If dct.Value = Val() Then
                Dim newRow As DataRow
                newRow = tblStatuses.NewRow()
                newRow.Item("value") = dct.Value
                newRow.Item("display") = dct.Key
                tblStatuses.Rows.Add(newRow)
                'Exit For
                'End If
            Next
            'Next

            If tblStatuses IsNot Nothing Then
                If tblStatuses.Rows.Count > 0 Then
                    LoadingDropDownList(dwlControl, tblStatuses.Columns("display").ColumnName,
                                        tblStatuses.Columns("value").ColumnName, tblStatuses, True, " ")
                End If
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
        End Try

        'If dwlControl.Items.Count = 0 Then
        '    Dim ListItem As ListItem = New ListItem()
        '    dwlControl.Items.Add(New WebControls.ListItem(" ", "0"))
        '    dwlControl.Items.Add(New WebControls.ListItem("LS", "1"))
        '    dwlControl.Items.Add(New WebControls.ListItem("VNDL", "2"))
        '    dwlControl.Items.Add(New WebControls.ListItem("MAN", "3"))
        '    dwlControl.Items.Add(New WebControls.ListItem("EXC", "4"))
        'End If

    End Sub

    Protected Sub fill_Status(dwlControl As DropDownList)
        Dim exMessage As String = Nothing
        Dim ds As DataSet = New DataSet()
        Try
            Dim dctValues = New Dictionary(Of String, String)()
            dctValues.Add("OPEN", "1")
            dctValues.Add("DOCUMENTATION", "2")
            dctValues.Add("TO DEVELOP", "3")
            dctValues.Add("RE-OPEN", "4")
            dctValues.Add("MOVED TO DEV", "5")
            dctValues.Add("REJECTED", "6")

            'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            '    ds = objBL.GetAllWLStatuses()
            'End Using

            Dim tblStatuses = New DataTable()
            Dim column1 As DataColumn = New DataColumn("value")
            column1.DataType = System.Type.GetType("System.String")
            Dim column2 As DataColumn = New DataColumn("display")
            column2.DataType = System.Type.GetType("System.String")
            tblStatuses.Columns.Add(column1)
            tblStatuses.Columns.Add(column2)

            For Each dctt In dctValues
                Dim newRow As DataRow
                newRow = tblStatuses.NewRow()
                newRow.Item("display") = dctt.Key
                newRow.Item("value") = dctt.Value
                tblStatuses.Rows.Add(newRow)
            Next

            'For Each dw As DataRow In ds.Tables(0).Rows
            '    Dim val = dw.ItemArray(0).ToString()
            '    For Each dct In dctValues
            '        'If dct.Value = val Then
            '        Dim newRow As DataRow
            '            newRow = tblStatuses.NewRow()
            '            newRow.Item("value") = val
            '            newRow.Item("display") = dct.Key
            '            tblStatuses.Rows.Add(newRow)
            '            Exit For
            '        'End If
            '    Next
            'Next

            If tblStatuses IsNot Nothing Then
                If tblStatuses.Rows.Count > 0 Then
                    LoadingDropDownList(dwlControl, tblStatuses.Columns("display").ColumnName,
                                        tblStatuses.Columns("value").ColumnName, tblStatuses, True, " ")
                End If
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

        'If dwlControl.Items.Count = 0 Then
        '    Dim ListItem As ListItem = New ListItem()
        '    dwlControl.Items.Add(New WebControls.ListItem(" ", "0"))
        '    dwlControl.Items.Add(New WebControls.ListItem("OPEN", "1"))
        '    dwlControl.Items.Add(New WebControls.ListItem("DOCUMENTATION", "2"))
        '    dwlControl.Items.Add(New WebControls.ListItem("TO DEVELOP", "3"))
        '    dwlControl.Items.Add(New WebControls.ListItem("RE-OPEN", "4"))
        '    dwlControl.Items.Add(New WebControls.ListItem("REJECTED", "5"))
        'End If

    End Sub

    Protected Sub fill_Users(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim messageOut As String = Nothing
        Try
            If dwlControl.ID <> "ddlAssignFoot" Or (dwlControl.Items.Count = 0 And dwlControl.ID = "ddlAssignFoot") Then
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
                    Log.Info(strLogCadenaCabecera + ".. Exception is: " + messageOut)
                End If
            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub fill_Minor(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Dim messageOut As String = Nothing
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                ds = objBL.GetAllMinors(messageOut)
            End Using

            If ds IsNot Nothing Then

                If ds.Tables(0).Rows.Count > 0 Then
                    LoadingDropDownList(dlMinor, ds.Tables(0).Columns("mincod").ColumnName,
                                        ds.Tables(0).Columns("mindes").ColumnName, ds.Tables(0), False, "")
                End If
            Else
                'Log.Info(strLogCadenaCabecera + ".. Result is: " + result.ToString())
                Log.Info(strLogCadenaCabecera + ".. Exception is: " + messageOut)

            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
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

    Protected Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim intValue As Integer
        Dim dsSetDataSource = New DataSet()
        Dim exMessage As String = Nothing

        Try
            If Integer.TryParse(ddlPageSize.SelectedValue, intValue) Then
                grvWishList.AllowPaging = True
                grvWishList.PageSize = If(ddlPageSize.SelectedValue > 10, CInt(ddlPageSize.SelectedValue), 10)

                Dim CurrentPage = (DirectCast(Session("currentPage"), Integer))
                Session("PageAmounts") = (grvWishList.PageSize * CurrentPage).ToString()

                'Dim ItemConttt = (DirectCast(Session("ItemCounts"), Integer))

                'Session("PageAmountsDdl") = grvWishList.PageSize

                Dim dsLoad = DirectCast(Session("WishListData"), DataSet)
                If dsLoad IsNot Nothing Then
                    If dsLoad.Tables(0).Rows.Count > 0 Then
                        loadData(dsLoad)
                    End If
                Else
                    loadData(Nothing)
                End If

            Else
                loadData(Nothing)
            End If
            updatePagerSettings(grvWishList)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    Protected Sub ddlStatus3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStatus3.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try
            Dim pepe = ddlStatus3.SelectedItem.Text
            Dim pepe1 = ddlStatus3.SelectedItem.Value

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlUser2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlUser2.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try
            Dim pepe = ddlUser2.SelectedItem.Text
            Dim pepe1 = ddlUser2.SelectedItem.Value
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlStatusFoot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlStatusFoot.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try
            If (ddlStatusFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                ddlStatusFoot.SelectedIndex = ddlStatusFoot.Items.IndexOf(ddlStatusFoot.Items.FindByText(DirectCast(Session("flagDdlStatusFoot"), String)))
            End If

            Dim ddlSelection = ddlStatusFoot.SelectedItem.Text
            Session("flagDdlStatusFoot") = ddlStatusFoot.SelectedItem.Text
            Dim dtSelection As New DataTable
            Dim dsSelection As New DataSet
            Dim lstSelection = New List(Of WishList)()
            Dim message As String = Nothing

            Dim ds = DirectCast(Session("WishListData"), DataSet)
            Dim lsTemp = fillObj(ds.Tables(0))
            For Each item In lsTemp
                If UCase(Trim(item.WHLSTATUS)) = UCase(Trim(ddlSelection)) Then
                    'lsTemp.Remove(item)
                    lstSelection.Add(item)
                End If
            Next

            If lstSelection.Count = 0 Then
                If ddlAssignFoot.SelectedIndex = 0 And ddlFromFoot.SelectedIndex = 0 Then
                    Dim ds1 = DirectCast(Session("WishListBck"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.WHLSTATUS)) = UCase(Trim(ddlSelection)) Then
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
                If (ddlAssignFoot.SelectedIndex = 0 And ddlFromFoot.SelectedIndex = 0) And ddlSelection <> DirectCast(Session("flagDdlStatusFoot"), String) Then
                    Dim ds1 = DirectCast(Session("WishListBck"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.WHLSTATUS)) = UCase(Trim(ddlSelection)) Then
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
            End If

            If lstSelection.Count = 0 Then
                grvWishList.DataSource = Nothing
                grvWishList.DataBind()

                'Session("WishListData") = Session("WishListBck")

                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
            Else
                dtSelection = ListToDataTable1(lstSelection)
                dsSelection.Tables.Add(dtSelection)
                GetWishListData(1, Nothing, dsSelection)
            End If
            'End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlFromFoot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFromFoot.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try
            If (ddlFromFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                ddlFromFoot.SelectedIndex = ddlFromFoot.Items.IndexOf(ddlFromFoot.Items.FindByText(DirectCast(Session("flagDdlFromFoot"), String)))
            End If

            Dim ddlSelection = ddlFromFoot.SelectedItem.Text
            Session("flagDdlFromFoot") = ddlFromFoot.SelectedItem.Text
            Dim dtSelection As New DataTable
            Dim dsSelection As New DataSet
            Dim lstSelection = New List(Of WishList)()
            Dim message As String = Nothing

            Dim ds = DirectCast(Session("WishListData"), DataSet)
            Dim lsTemp = fillObj(ds.Tables(0))
            For Each item In lsTemp
                If UCase(Trim(item.WHLFROM)) = UCase(Trim(ddlSelection)) Then
                    'lsTemp.Remove(item)
                    lstSelection.Add(item)
                End If
            Next

            If lstSelection.Count = 0 Then
                If ddlAssignFoot.SelectedIndex = 0 And ddlStatusFoot.SelectedIndex = 0 Then
                    Dim ds1 = DirectCast(Session("WishListBck"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.WHLFROM)) = UCase(Trim(ddlSelection)) Then
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
                If (ddlAssignFoot.SelectedIndex = 0 And ddlStatusFoot.SelectedIndex = 0) And ddlSelection <> DirectCast(Session("flagDdlFromFoot"), String) Then
                    Dim ds1 = DirectCast(Session("WishListBck"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.WHLFROM)) = UCase(Trim(ddlSelection)) Then
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
            End If

            If lstSelection.Count = 0 Then
                grvWishList.DataSource = Nothing
                grvWishList.DataBind()

                'Session("WishListData") = Session("WishListBck")

                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)

                'Dim methodMessage = "There is not references with the selected status."
                'SendMessage(methodMessage, messageType.info)
            Else
                dtSelection = ListToDataTable1(lstSelection)
                dsSelection.Tables.Add(dtSelection)
                GetWishListData(1, Nothing, dsSelection)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlAssignFoot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlAssignFoot.SelectedIndexChanged
        Dim exMessage As String = Nothing
        Try
            If (ddlAssignFoot.SelectedIndex = 0 And DirectCast(Session("EventRaised"), Boolean)) Then
                ddlAssignFoot.SelectedIndex = ddlAssignFoot.Items.IndexOf(ddlAssignFoot.Items.FindByText(DirectCast(Session("flagDdlAssignFoot"), String)))
            End If

            Dim ddlSelection = ddlAssignFoot.SelectedItem.Text
            Session("flagDdlAssignFoot") = ddlAssignFoot.SelectedItem.Text
            Dim dtSelection As New DataTable
            Dim dsSelection As New DataSet
            Dim lstSelection = New List(Of WishList)()
            Dim message As String = Nothing

            Dim ds = DirectCast(Session("WishListData"), DataSet)
            Dim lsTemp = fillObj(ds.Tables(0))
            For Each item In lsTemp
                If UCase(Trim(item.WHLSTATUSU)) = UCase(Trim(ddlSelection)) Then
                    'lsTemp.Remove(item)
                    lstSelection.Add(item)
                End If
            Next

            If lstSelection.Count = 0 Then
                If ddlFromFoot.SelectedIndex = 0 And ddlStatusFoot.SelectedIndex = 0 Then
                    Dim ds1 = DirectCast(Session("WishListBck"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.WHLSTATUSU)) = UCase(Trim(ddlSelection)) Then
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
                If (ddlFromFoot.SelectedIndex = 0 And ddlStatusFoot.SelectedIndex = 0) And ddlSelection <> DirectCast(Session("flagDdlAssignFoot"), String) Then
                    Dim ds1 = DirectCast(Session("WishListBck"), DataSet)
                    Dim lsTemp1 = fillObj(ds1.Tables(0))
                    For Each item1 In lsTemp1
                        If UCase(Trim(item1.WHLSTATUSU)) = UCase(Trim(ddlSelection)) Then
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
            End If

            If lstSelection.Count = 0 Then
                grvWishList.DataSource = Nothing
                grvWishList.DataBind()
                'Session("WishListData") = Session("WishListBck")
                ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "removeHideReload('" & message & " ')", True)
            Else
                dtSelection = ListToDataTable1(lstSelection)
                dsSelection.Tables.Add(dtSelection)
                GetWishListData(1, Nothing, dsSelection)
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlAssign_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False)
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim valueToCompare As String = Nothing
        Try

            Dim dsData = New DataSet()
            ddlFrom.SelectedIndex = 0
            ddlStatus.SelectedIndex = 0
            Session("ddlStatusIndex") = "0"
            Session("ddlFromIndex") = "0"

            If ddlAssign.SelectedIndex = 0 Then
                'dsData = DirectCast(Session("WishListData"), DataSet)
                'GetWishListData(0, Nothing, dsData)
                valueToCompare = "N/A"
            End If

            If DirectCast(Session("ddlAssignIndex"), String) = "-1" Or flag = True Then
                dsData = DirectCast(Session("WishListData"), DataSet)
                Session("ddlStatusIndex") = "0"
                Session("ddlFromIndex") = "0"
            Else
                If DirectCast(Session("ddlAssignIndex"), String) <> ddlAssign.SelectedIndex.ToString() Then
                    Dim dsFirst = DirectCast(Session("WishListBck"), DataSet)
                    Session("WishListData") = dsFirst
                    dsData = DirectCast(Session("WishListData"), DataSet)
                Else
                    dsData = DirectCast(Session("WishListData"), DataSet)
                End If
            End If

            Dim dsFilter As DataSet = New DataSet()
            Dim dtFilter As DataTable = dsData.Tables(0).Clone()
            If String.IsNullOrEmpty(valueToCompare) Then
                valueToCompare = ddlAssign.SelectedItem.Text.ToString()
            End If

            For Each dr As DataRow In dsData.Tables(0).Rows
                If LCase(dr.Item("WHLSTATUSU").ToString().Trim()) = LCase(valueToCompare.Trim()) Then
                    Dim dtr As DataRow = dtFilter.NewRow()
                    dtr.ItemArray = dr.ItemArray
                    dtFilter.Rows.Add(dtr)
                End If
            Next

            If dtFilter IsNot Nothing Then
                If dtFilter.Rows.Count > 0 Then
                    dsFilter.Tables.Add(dtFilter)
                    Session("DataFilter") = dsFilter
                    GetWishListData(0, Nothing, dsFilter)
                Else
                    grvWishList.DataSource = Nothing
                    grvWishList.DataBind()

                    methodMessage = "There is not results with the selected criteria."
                    SendMessage(methodMessage, messageType.warning)
                End If
            Else
                grvWishList.DataSource = Nothing
                grvWishList.DataBind()

                methodMessage = "There is not results with the selected criteria."
                SendMessage(methodMessage, messageType.warning)
            End If

            Session("ddlAssignIndex") = ddlAssign.SelectedIndex.ToString()

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlFrom_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False)
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Try

            Dim dsData = New DataSet()
            ddlStatus.SelectedIndex = 0
            ddlAssign.SelectedIndex = 0
            Session("ddlStatusIndex") = "0"
            Session("ddlAssignIndex") = "0"

            If ddlFrom.SelectedIndex = 0 Then
                dsData = DirectCast(Session("WishListData"), DataSet)
                GetWishListData(0, Nothing, dsData)
            Else
                If DirectCast(Session("ddlFromIndex"), String) = "-1" Or flag = True Then
                    dsData = DirectCast(Session("WishListData"), DataSet)
                    Session("ddlStatusIndex") = "0"
                    Session("ddlAssignIndex") = "0"
                Else
                    If DirectCast(Session("ddlFromIndex"), String) <> ddlFrom.SelectedIndex.ToString() Then
                        Dim dsFirst = DirectCast(Session("WishListBck"), DataSet)
                        Session("WishListData") = dsFirst
                        dsData = DirectCast(Session("WishListData"), DataSet)
                    Else
                        dsData = DirectCast(Session("WishListData"), DataSet)
                    End If
                End If

                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlFrom.SelectedItem.Text.ToString()

                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.Item("WHLFROM").ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                    End If
                Next

                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        GetWishListData(0, Nothing, dsFilter)
                    Else
                        grvWishList.DataSource = Nothing
                        grvWishList.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)
                    End If
                Else
                    grvWishList.DataSource = Nothing
                    grvWishList.DataBind()

                    methodMessage = "There is not results with the selected criteria."
                    SendMessage(methodMessage, messageType.warning)
                End If

                Session("ddlFromIndex") = ddlFrom.SelectedIndex.ToString()

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs, Optional flag As Boolean = False)
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Try

            Dim dsData = New DataSet()
            ddlFrom.SelectedIndex = 0
            ddlAssign.SelectedIndex = 0
            Session("ddlAssignIndex") = "0"
            Session("ddlFromIndex") = "0"

            If ddlStatus.SelectedIndex = 0 Then
                dsData = DirectCast(Session("WishListData"), DataSet)
                GetWishListData(0, Nothing, dsData)
            Else
                If DirectCast(Session("ddlStatusIndex"), String) = "-1" Or flag = True Then
                    dsData = DirectCast(Session("WishListData"), DataSet)
                    Session("ddlAssignIndex") = "0"
                    Session("ddlFromIndex") = "0"
                Else
                    If DirectCast(Session("ddlStatusIndex"), String) <> ddlStatus.SelectedIndex.ToString() Then
                        Dim dsFirst = DirectCast(Session("WishListBck"), DataSet)
                        Session("WishListData") = dsFirst
                        dsData = DirectCast(Session("WishListData"), DataSet)
                    Else
                        dsData = DirectCast(Session("WishListData"), DataSet)
                    End If
                End If

                Dim dsFilter As DataSet = New DataSet()
                Dim dtFilter As DataTable = dsData.Tables(0).Clone()
                Dim valueToCompare As String = ddlStatus.SelectedItem.Text.ToString()

                For Each dr As DataRow In dsData.Tables(0).Rows
                    If dr.Item("WHLSTATUS").ToString() = valueToCompare Then
                        Dim dtr As DataRow = dtFilter.NewRow()
                        dtr.ItemArray = dr.ItemArray
                        dtFilter.Rows.Add(dtr)
                    End If
                Next

                If dtFilter IsNot Nothing Then
                    If dtFilter.Rows.Count > 0 Then
                        dsFilter.Tables.Add(dtFilter)
                        Session("DataFilter") = dsFilter
                        GetWishListData(0, Nothing, dsFilter)
                    Else
                        grvWishList.DataSource = Nothing
                        grvWishList.DataBind()

                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)
                    End If
                Else
                    grvWishList.DataSource = Nothing
                    grvWishList.DataBind()

                    methodMessage = "There is not results with the selected criteria."
                    SendMessage(methodMessage, messageType.warning)
                End If

                Session("ddlStatusIndex") = ddlStatus.SelectedIndex.ToString()

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub liProjVendors_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles liProjVendors.SelectedIndexChanged
        Dim exMessage As String = " "
        Try
            Session("liSelected") = liProjVendors.SelectedItem.Text
            Dim pp = "aa"
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub


#End Region

#Region "Buttons"

    'Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    '    Dim pepe = Nothing
    'End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Try
            Dim dsResult = DirectCast(Session("WishListData"), DataSet)
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then

                    Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    Dim folderPath As String = userPath & "\Wish_List_Data\"

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            Exit Sub
                        End If

                        Dim title As String
                        title = "Wish_List_Generated_by "
                        fileName = objBL.adjustDatetimeFormat(title, fileExtension)

                    End Using

                    Dim fullPath = folderPath + fileName

                    Using wb As New XLWorkbook()
                        wb.Worksheets.Add(dsResult.Tables(0), "WishList")
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim exMessage As String = Nothing
        Dim methodMessage As String = String.Empty
        Dim lstReferences As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Try

            lstReferences = GetCheckboxesDisp()
            If lstReferences Is Nothing Then
                methodMessage = "An exception occur in the method execution!"
                SendMessage(methodMessage, messageType.Error)
            Else
                If lstReferences.Count = 0 Then
                    methodMessage = "Please select the items that you want to update and then click this button!"
                    SendMessage(methodMessage, messageType.warning)
                Else
                    hdUpdateMedRefFlag.Value = "1"
                    ddlStatus3.SelectedIndex = 0
                    ddlUser2.SelectedIndex = 0
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnUpdate2_Click(sender As Object, e As EventArgs) Handles btnUpdate2.Click
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Try
            Dim userText As String = ddlAssignedTo.SelectedItem.Text
            Dim userValue As String = ddlAssignedTo.SelectedItem.Value
            Dim statusText As String = ddlStatus2.SelectedItem.Text
            Dim statusValue As String = ddlStatus2.SelectedItem.Value
            Dim code As String = selCheckbox.Value.ToString()
            Dim comments = txtComments2.Text
            Dim partNo = Trim(txtPartNumber2.Text)

            For Each grv As GridViewRow In grvWishList.Rows

                Dim lnk = DirectCast(grv.FindControl("lbPartNo"), LinkButton)
                Dim strValue = lnk.CommandArgument.ToString()

                If UCase(Trim(strValue)) = UCase(partNo) Then

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        Dim result = objBL.UpdateWishListSingleReference(userText, statusValue, grv.Cells(2).Text, comments)
                        If result > 0 Then

                            grv.Cells(7).Text = statusText
                            grv.Cells(8).Text = userText

                            grvWishList.UpdateRow(grv.RowIndex, False)
                        End If
                    End Using

                    hdUpdateFullRefFlag.Value = "0"
                    Exit For

                End If
            Next

            grvWishList.DataBind()

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnUpdate3_Click(sender As Object, e As EventArgs) Handles btnUpdate3.Click
        Dim exMessage As String = Nothing
        Dim updatedReferences As Integer = 0
        Dim methodMessage As String = String.Empty
        Dim lstReferences As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Try

            lstReferences = GetCheckboxesDisp()
            If lstReferences Is Nothing Then
                methodMessage = "An exception occur in the method execution!"
                SendMessage(methodMessage, messageType.Error)
            Else
                If lstReferences.Count = 0 Then
                    methodMessage = "Please select the items that you want to update and then click this button!"
                    SendMessage(methodMessage, messageType.warning)
                Else
                    Dim userText As String = ddlUser2.SelectedItem.Text
                    Dim userValue As String = ddlUser2.SelectedItem.Value
                    Dim statusText As String = ddlStatus3.SelectedItem.Text
                    Dim statusValue As String = ddlStatus3.SelectedItem.Value
                    Dim code As String = selCheckbox.Value.ToString()

                    For Each grv As GridViewRow In grvWishList.Rows
                        Dim chk = DirectCast(grv.FindControl("chkSingleAdd"), CheckBox)
                        If chk.Checked Then
                            grv.Cells(7).Text = statusText
                            grv.Cells(8).Text = userText

                            Dim lbl1 = DirectCast(grv.FindControl("lbPartNo"), LinkButton)
                            Dim partNo = If(lbl1 IsNot Nothing, Trim(lbl1.CommandArgument.ToString()), "")

                            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                                Dim result = objBL.UpdateWishListGenericReference(userText, statusValue, grv.Cells(2).Text, partNo)
                                If result > 0 Then
                                    grvWishList.UpdateRow(grv.RowIndex, False)
                                    updatedReferences += 1
                                End If
                            End Using

                        End If
                    Next

                    'ya funciono. Queda cerra cuando actualiza y que funcione el botonback
                    If updatedReferences > 0 Then
                        hdUpdateMedRefFlag.Value = "0"
                        grvWishList.DataBind()
                        methodMessage = "Successful update for " + updatedReferences.ToString() + " records."
                        SendMessage(methodMessage, messageType.success)
                    Else
                        methodMessage = "No references updated."
                        SendMessage(methodMessage, messageType.warning)
                    End If

                End If
            End If

            'Dim userText As String = ddlUser2.SelectedItem.Text
            'Dim userValue As String = ddlUser2.SelectedItem.Value
            'Dim statusText As String = ddlStatus3.SelectedItem.Text
            'Dim statusValue As String = ddlStatus3.SelectedItem.Value
            'Dim code As String = selCheckbox.Value.ToString()

            'For Each grv As GridViewRow In grvWishList.Rows
            '    Dim chk = DirectCast(grv.FindControl("chkSingleAdd"), CheckBox)
            '    If chk.Checked Then
            '        grv.Cells(7).Text = statusText
            '        grv.Cells(8).Text = userText

            '        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
            '            Dim result = objBL.UpdateWishListGenericReference(userText, statusValue, grv.Cells(2).Text)
            '            If result > 0 Then
            '                grvWishList.UpdateRow(grv.RowIndex, False)
            '                updatedReferences += 1
            '            End If
            '        End Using

            '    End If
            'Next

            'If updatedReferences > 0 Then
            '    grvWishList.DataBind()
            '    methodMessage = "Successful update for " + updatedReferences.ToString() + " records."
            '    SendMessage(methodMessage, messageType.success)
            'Else
            '    methodMessage = "No references updated."
            '    SendMessage(methodMessage, messageType.warning)
            'End If

            'call method to update from the code and setting the user and status

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnNewPD_Click(sender As Object, e As EventArgs) Handles btnNewPD.Click
        Dim exMessage As String = Nothing
        Try
            Dim partNo As String = Trim(txtPartNumber2.Text)
            fillPartInfoPD(partNo)

            txtPartNoPD.Enabled = False
            txtDescriptionPD1.Enabled = False
            txtCTPNoPD.Enabled = False

            'If String.IsNullOrEmpty(txtCurrentVendor.Text.Trim()) Then
            '    Dim methodMessage = "You must select a vendor for this reference."
            '    SendMessage(methodMessage, messageType.warning)
            'End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnCreateProjectPD_Click(sender As Object, e As EventArgs) Handles btnCreateProjectPD.Click
        Dim exMessage As String = Nothing
        Try
            SaveProdDevProject()
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnImportFromLs_Click(sender As Object, e As EventArgs) Handles btnImportFromLs.Click
        Response.Redirect("Lost-Sales.aspx")
    End Sub

    Protected Sub lnkSearchVendorNo_Click(sender As Object, e As EventArgs) Handles lnkSearchVendorNo.Click
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Dim vendorOEMCodeDenied As String = ConfigurationManager.AppSettings("vendorOEMCodeDenied")
        Dim itemCategories As String = ConfigurationManager.AppSettings("itemCategories")
        Dim vendorCodesDenied As String = ConfigurationManager.AppSettings("vendorCodesDenied")

        Try
            If Not String.IsNullOrEmpty(txtvendor.Text) Then
                Dim vendorNo = Trim(txtvendor.Text)

                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    Dim rsData = objBL.GetVendorByNumber(vendorNo, vendorCodesDenied, vendorOEMCodeDenied, itemCategories, dsResult)
                    If rsData > 0 Then
                        txtVndDesc.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(1).ColumnName).ToString()
                        txtVndDesc.Enabled = False
                        hdHideMessageVendor.Value = "0"
                    Else
                        txtVndDesc.Text = ""
                        txtvendor.Text = ""
                        hdNewRef2Flag.Value = "1"
                        hdNewRef1Flag.Value = "0"
                        hdHideMessageVendor.Value = "There is not a valid Vendor Number."
                    End If
                End Using
            Else
                txtVndDesc.Text = ""
                txtVndDesc.Enabled = True
                'warning message
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkSearchPartNo_Click(sender As Object, e As EventArgs) Handles lnkSearchPartNo.Click
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Dim methodMessage = "There is not a Part Number like the number that you entered."

        Try
            If Not String.IsNullOrEmpty(txtPartNo.Text) Then
                Dim partNo = UCase(Trim(txtPartNo.Text))

                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    Dim rsData = objBL.GetNewPartData(partNo, dsResult)
                    If rsData > 0 Then
                        txtDesc.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(1).ColumnName).ToString()
                        hdHideMessage.Value = "0"
                    Else
                        txtPartNo.Text = ""
                        txtDesc.Text = ""
                        hdHideMessage.Value = "This part number is wrong."
                    End If
                End Using
            Else
                txtDesc.Text = ""
                'warning message
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnWlTemplate_Click(sender As Object, e As EventArgs) Handles btnWlTemplate.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Dim folderPath As String = ""
        Dim resultMethod As Boolean = False
        Try

            Dim pathToProcess = ConfigurationManager.AppSettings("urlWlTemplateToProcess")
            'Dim updUserPath = userPath + "\WishList-Template\"
            folderPath = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("urlWlTemplate")), ConfigurationManager.AppSettings("urlWlTemplate"), "")
            Dim methodMessage = If(Not String.IsNullOrEmpty(folderPath), "The template document will be downloaded to your documents folder", "There is not a path defined for this document. Call an administrator!!")
            'SendMessage(methodMessage, messageType.info)

            If Not String.IsNullOrEmpty(folderPath) Then

                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    fileExtension = objBL.Determine_OfficeVersion()
                    If String.IsNullOrEmpty(fileExtension) Then
                        resultMethod = True
                        'Exit Sub
                    Else

                        'create the local path if not exists and if exists check if have docus and not are opened
                        If Not Directory.Exists(pathToProcess) Then
                            Directory.CreateDirectory(pathToProcess)
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

                        'copy the file in local

                        Dim myFileWL As FileInfo = New FileInfo(folderPath) 'server side file
                        Dim fileNameWL As String = myFileWL.Name ' server side file name

                        Dim localFilePath = pathToProcess + fileNameWL ' local end path creation
                        File.Copy(folderPath, localFilePath) 'copy file from server to local

                        Dim newLocalFile As FileInfo = New FileInfo(localFilePath)
                        If newLocalFile.Exists Then
                            Try
                                Process.Start("explorer.exe", localFilePath)
                            Catch Win32Exception As Win32Exception
                                Shell("explorer " & localFilePath, AppWinStyle.NormalFocus)
                            End Try
                        End If

                    End If
                End Using

            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    'Protected Sub btnImportExcel_Click(sender As Object, e As EventArgs) Handles btnImportExcel.Click
    '    Dim exMessage As String = Nothing
    '    Dim fileExtension As String = ""
    '    Try
    '        Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

    '        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
    '            fileExtension = objBL.Determine_OfficeVersion()
    '            If String.IsNullOrEmpty(fileExtension) Then
    '                Exit Sub
    '            End If
    '        End Using


    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    Protected Sub btnSubmitItem_Click(sender As Object, e As EventArgs) Handles btnSubmitItem.Click
        Dim exMessage As String = Nothing
        Dim strResult As String = Nothing
        Dim dsResult As New DataSet
        Try
            If Not String.IsNullOrEmpty(txtPartNumber.Text) Then
                Dim partNo = UCase(Trim(txtPartNumber.Text))

                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    Dim rsData = objBL.GetNewPartData(partNo, dsResult)
                    If rsData > 0 Then
                        'txtDesc.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(1).ColumnName).ToString()
                        'txtPartNo.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(0).ColumnName).ToString()
                        txDesc.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(1).ColumnName).ToString()
                        txPartNo.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(0).ColumnName).ToString()
                        txPrice.Text = dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(2).ColumnName).ToString()
                        txUser.Text = If(Not String.IsNullOrEmpty(Session("userid")), DirectCast(Session("userid"), String), "Computer logged user")
                        txDate.Text = Today.ToShortDateString()
                        txMajor.Text = If(UCase(Trim(dsResult.Tables(0).Rows(0).Item(dsResult.Tables(0).Columns(3).ColumnName).ToString())) = "CATER", "01", "03")

                        'Dim ddl = dlType

                        txDesc.Enabled = False
                        txPartNo.Enabled = False
                        txPrice.Enabled = False
                        txDate.Enabled = False
                        txMajor.Enabled = False
                        txUser.Enabled = False
                        hdHideMessage.Value = "0"
                    Else
                        txtDesc.Text = "0"
                        txtPartNo.Text = "0"
                        hdNewRef3Flag.Value = "0"
                        hdNewRef2Flag.Value = "0"
                        hdNewRef1Flag.Value = "1"
                        hdHideMessage.Value = "There is not a Part Number like the number that you entered."
                    End If
                End Using

            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim exMessage As String = Nothing
        Dim strResult As String = Nothing
        Try

            If fuOPenEx.HasFile() Then

                Dim dtExcel = GetDataTableFromExcel(fuOPenEx)
                If dtExcel IsNot Nothing Then
                    If dtExcel.Rows.Count > 0 Then
                        strResult = processExcelData(dtExcel)
                        If strResult Is Nothing Then
                            'ok
                        Else
                            'errores
                        End If
                    End If
                Else
                    'warning message
                End If
            Else
                Dim pepe = "1"
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkReloadBack_Click(sender As Object, e As EventArgs) Handles lnkReloadBack.Click
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("WishListBck"), DataSet)
            Session("WishListData") = dsData

            Session("ItemCounts") = (DirectCast(Session("WishListData"), DataSet)).Tables(0).Rows.Count
            'dsData.Tables(0).Rows.Count
            grvWishList.DataSource = dsData
            grvWishList.DataBind()

            ClearInputCustom(rowFilters)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkReloadGrid_Click(sender As Object, e As EventArgs) Handles lnkReloadGrid.Click
        Dim exMessage As String = Nothing
        Try
            If Session("NoPrivilegesUser") IsNot Nothing Then
                Dim curUser = DirectCast(Session("NoPrivilegesUser"), String)
                'customUser = "CMONTILVA" 'test remove
                Dim strOptionalQuery As String = " and a3.whlstatusu = '{0}' "
                Dim sqlResult As String = String.Format(strOptionalQuery, curUser)

                GetWishListData(0, Nothing, Nothing, sqlResult)
            Else
                GetWishListData(0)
            End If
            ClearInputCustom(rowFilters)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkSearchValue_Click(sender As Object, e As EventArgs) Handles lnkSearchValue.Click
        Dim exMessage As String = Nothing
        Dim vndName As String = Nothing
        Try
            'Dim dsData = DirectCast(Session("WishListData"), DataSet)

            If Not String.IsNullOrEmpty(txtSearchValue.Text) Then
                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    Dim dsData = objBL.GetProjectData(txtSearchValue.Text)
                    If dsData IsNot Nothing Then
                        If dsData.Tables(0).Rows.Count > 0 Then
                            lblProjName.Text = dsData.Tables(0).Rows(0).Item("PRNAME").ToString().Trim()
                            Dim dateTemp = dsData.Tables(0).Rows(0).Item("CRDATE").ToString().Trim()
                            lblProjCreationDate.Text = dateTemp.Split(" ")(0).ToString().Trim()
                            Dim statusTemp = dsData.Tables(0).Rows(0).Item("PRSTAT").ToString().Trim()
                            lblProjStatus.Text = If(statusTemp = "I", "IN PROCESS", "FINISHED")
                            lblProjUser.Text = dsData.Tables(0).Rows(0).Item("CRUSER").ToString().Trim()
                            lblProjInfo.Text = dsData.Tables(0).Rows(0).Item("PRINFO").ToString().Trim()

                            'search the project vendors
                            Dim dsVendors = objBL.GetVendorsInProject(txtSearchValue.Text)
                            If dsVendors IsNot Nothing Then
                                If dsVendors.Tables(0).Rows.Count > 0 Then
                                    Dim lstTemp As List(Of String) = New List(Of String)()
                                    For Each dww As DataRow In dsVendors.Tables(0).Rows
                                        lstTemp.Add(dww.Item("VMVNUM").ToString())
                                    Next

                                    liProjVendors.DataSource = lstTemp
                                    liProjVendors.DataBind()
                                End If
                            End If

                            lblProjectNumber.Text = dsData.Tables(0).Rows(0).Item("PRHCOD").ToString().Trim()
                            Dim flagChange As Boolean = False
                            If String.IsNullOrEmpty(txtCurrentVendor.Text.Trim()) And liProjVendors.Items.Count = 1 Then
                                hdDifVnd.Value = "0"
                                flagChange = False
                            Else
                                For Each dw As DataRow In dsVendors.Tables(0).Rows
                                    If LCase(dw.Item("VMVNUM").ToString().Trim()) = LCase(txtCurrentVendor.Text.Trim()) Then
                                        flagChange = True
                                        Exit For
                                    End If
                                Next
                            End If

                            If Not flagChange Then
                                hdDifVnd.Value = "0"
                            End If

                        End If
                    End If
                End Using
            Else
                'must be have a value
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkNewVendor_Click(sender As Object, e As EventArgs) Handles lnkNewVendor.Click
        Dim exMessage As String = Nothing
        Dim vndName As String = Nothing
        Try
            'Dim dsData = DirectCast(Session("WishListData"), DataSet)
            If Not String.IsNullOrEmpty(refreshTxtValue.Value) Then
                txtNewVendorNo.Text = refreshTxtValue.Value
            End If

            If Not String.IsNullOrEmpty(txtNewVendorNo.Text) Then
                Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                    Dim dsData = objBL.getVendorTypeByVendorNum(txtNewVendorNo.Text)
                    If dsData IsNot Nothing Then
                        If dsData.Tables(0).Rows.Count > 0 Then
                            Dim validVnd = objBL.isVendorAccepted(txtNewVendorNo.Text)
                            If validVnd Then
                                txtNewVendorPD.Text = dsData.Tables(0).Rows(0).ItemArray(1).ToString()
                            End If
                        End If
                    End If
                End Using
            Else
                'must be have a value
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim exMessage As String = Nothing
        Dim searchstring As String = Trim(txtSearch.Text)
        Dim filterData = New List(Of WishList)()
        Dim lstData = New List(Of WishList)()
        Dim dsWork As DataSet = New DataSet()
        Dim methodMessage As String = Nothing
        Try
            If searchstring.Equals("Search...") Or String.IsNullOrEmpty(searchstring) Then

                Dim ds = DirectCast(Session("WishListBck"), DataSet)
                loadData(ds)

                methodMessage = "When search without a search criteria the full data is loaded."
                SendMessage(methodMessage, messageType.warning)

            Else
                Dim dsData = New DataSet()
                dsData = If((DirectCast(Session("WishListData"), DataSet)) IsNot Nothing, DirectCast(Session("WishListData"), DataSet), Nothing)
                If dsData IsNot Nothing Then
                    If dsData.Tables(0).Rows.Count > 0 Then
                        lstData = fillObj(dsData.Tables(0))
                    End If
                Else
                    Dim dtData = DirectCast(grvWishList.DataSource, DataTable)
                    lstData = fillObj(dtData)
                End If

                'all ocurrences without duplicate value string
                filterData = lstData.Where(Function(da) _
                                               If(Not String.IsNullOrEmpty(da.IMPTN), UCase(da.IMPTN).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMDSC), UCase(da.IMDSC).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.WHLDATE), UCase(da.WHLDATE).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.WHLUSER), UCase(da.WHLUSER).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.WHLSTATUS), UCase(da.WHLSTATUS).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.WHLSTATUSU), UCase(da.WHLSTATUSU).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.VENDOR), UCase(da.VENDOR).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.VENDOR), UCase(da.VENDOR).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMPRC), UCase(da.IMPRC).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.PA), UCase(da.PA).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.PS), UCase(da.PS).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMPC1), UCase(da.IMPC1).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.qtysold), UCase(da.qtysold).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMPC2), UCase(da.IMPC2).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.QTYQTE), UCase(da.QTYQTE).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.VENDORNAME), UCase(da.VENDORNAME).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.TIMESQ), UCase(da.TIMESQ).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.LOC20), UCase(da.LOC20).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMMOD), UCase(da.IMMOD).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.IMCATA1), UCase(da.IMCATA1).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.SUBCAT), UCase(da.SUBCAT).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.WHLFROM), UCase(da.WHLFROM).Trim().Contains(UCase(searchstring)), False) _
                                               Or If(Not String.IsNullOrEmpty(da.WHLCOMMENT), UCase(da.WHLCOMMENT).Trim().Contains(UCase(searchstring)), False)
                                               ).ToList()

                If filterData.Count > 0 Then
                    Dim dtResult = ListToDataTable(filterData)
                    If dtResult IsNot Nothing Then
                        If dtResult.Rows.Count > 0 Then
                            Dim ds = New DataSet()
                            ds.Tables.Add(dtResult)
                            GetWishListData(0, Nothing, ds)
                        End If
                    End If
                Else
                    'restore grid and message 
                    Dim dsLoad = DirectCast(Session("LostSaleBck"), DataSet)
                    If dsLoad IsNot Nothing Then
                        If dsLoad.Tables(0).Rows.Count > 0 Then
                            GetWishListData(0, Nothing, dsLoad)
                        Else
                            GetWishListData(0, dsWork)
                        End If
                    Else
                        GetWishListData(0, dsWork)
                    End If
                End If

            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    'Protected Sub btnUpd_Click(sender As Object, e As EventArgs) Handles btnUpd.Click

    'End Sub

#End Region

#Region "TextBox"

    Public Sub txtNewVendorNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtNewVendorNo.TextChanged
        refreshTxtValue.Value = txtNewVendorNo.Text
    End Sub

#End Region

#Region "GridView"

    'Protected Sub grvDetails_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
    '    Dim exMessage As String = Nothing

    '    Try
    '        Dim dsss = New DataSet()
    '        dsss = DirectCast(Session("WishListData"), DataSet)
    '        Dim roww As GridViewRow = grvWishList.Rows(e.RowIndex)

    '        Dim grvDet = DirectCast(roww.FindControl("grvDetails"), GridView)

    '        Dim dt As DataTable = New DataTable()
    '        dt = dsss.Tables(0).Clone()

    '        Dim dr As DataRow = dt.NewRow()
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

    Protected Sub grvWishList_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Dim exMessage As String = Nothing

        Try
            Dim dsss = New DataSet()
            dsss = DirectCast(Session("WishListData"), DataSet)

            Dim roww As GridViewRow = grvWishList.Rows(e.RowIndex)
            dsss.Tables(0).Rows(roww.DataItemIndex)("WHLSTATUS") = roww.Cells(7).Text
            dsss.Tables(0).Rows(roww.DataItemIndex)("WHLSTATUSU") = roww.Cells(8).Text
            dsss.Tables(0).Rows(roww.DataItemIndex)("A3COMMENT") = txtComments2.Text

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    Protected Sub grvWishList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grvWishList.PageIndexChanging
        Dim exMessage As String = " "
        Dim dsSetDataSource = New DataSet()
        Try
            grvWishList.PageIndex = e.NewPageIndex

#Region "Need to works"

            'Session("PageIndex") = grvWishList.PageIndex + 1

            'Dim amount = DirectCast(Session("PageAmountsDdl"), Integer)
            'Session("currentPage") = (CInt(e.NewPageIndex + 1) * amount) - (amount - 1)
            'Session("PageAmounts") = If((CInt(e.NewPageIndex + 1) * amount) > DirectCast(Session("ItemCounts"), Integer), DirectCast(Session("ItemCounts"), Integer), (CInt(e.NewPageIndex + 1) * amount))

#End Region

            Session("currentPage") = (CInt(e.NewPageIndex + 1) * 10) - 9
            Dim vall = If((CInt(e.NewPageIndex + 1) * 10) > CInt(DirectCast(Session("ItemCounts"), String)), CInt(DirectCast(Session("ItemCounts"), String)), (CInt(e.NewPageIndex + 1) * 10))
            Session("PageAmounts") = vall.ToString()
            'Dim ds = getDataSource()
            'Dim dt = DirectCast(grvWishList.DataSource, DataTable)
            Dim ds = DirectCast(Session("WishListData"), DataSet)

            If ds IsNot Nothing Then
                loadData(ds)
            Else
                loadData(Nothing)
            End If

            'updatepnl.Update()
            'grvWishList.DataBind()
            'GetLostSalesData("", 1)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvWishList_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grvWishList.RowCommand
        Dim exMessage As String = Nothing
        Try
            If e.CommandName = "UpdatePart" Then
                'GridViewRow row = (GridViewRow)(e.CommandSource As LinkButton).Parent.Parent;
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)

                Dim dataFrom = row.Cells(5)
                Dim myLabel As Label = DirectCast(dataFrom.FindControl("txtPartName"), Label)
                txtPartNumber2.Text = Trim(myLabel.Text)
                txtPartNumber2.Enabled = False

                hdWhlCode1.Value = row.Cells(2).Text

                Dim assigned As String = Trim(row.Cells(8).Text)
                ddlAssignedTo.SelectedIndex = ddlAssignedTo.Items.IndexOf(ddlAssignedTo.Items.FindByText(assigned))

                Dim status As String = row.Cells(7).Text
                ddlStatus2.SelectedIndex = ddlStatus2.Items.IndexOf(ddlStatus2.Items.FindByText(status))

                txtComments2.Text = GetCommentById(Trim(row.Cells(2).Text))
            ElseIf e.CommandName = "show" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)
                Dim id = row.Cells(2).Text

                Dim ds1 = DirectCast(Session("WishListData"), DataSet)

                Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) item.Item("WHLCODE").ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase))
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
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvWishList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grvWishList.RowDataBound
        Dim exMessage As String = Nothing
        Dim lstValues = New List(Of String)()
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then

                'set currency format for price
                Dim price = Convert.ToDecimal(e.Row.Cells(15).Text)

                e.Row.Cells(15).Text = String.Format("{0:C2}", price)
                e.Row.Cells(15).ForeColor = System.Drawing.Color.Red

                ' paint the part no cell
                e.Row.Cells(5).ForeColor = System.Drawing.Color.Red

                'paint the status cell
                If LCase(e.Row.Cells(7).Text) = "open" Then
                    e.Row.Cells(7).ForeColor = System.Drawing.Color.Orange
                ElseIf LCase(e.Row.Cells(7).Text) = "documentation" Then
                    e.Row.Cells(7).ForeColor = System.Drawing.Color.Blue
                ElseIf LCase(e.Row.Cells(7).Text) = "rejected" Then
                    e.Row.Cells(7).ForeColor = System.Drawing.Color.Red
                Else
                    e.Row.Cells(7).ForeColor = System.Drawing.Color.Green
                End If

                'changing the style and font awesome icon
                Dim dataFrom = e.Row.Cells(1)
                Dim myButton As LinkButton = DirectCast(dataFrom.FindControl("lbSourceFrom"), LinkButton)
                Dim myLabel As Label = DirectCast(dataFrom.FindControl("textlbl"), Label)
                Dim htmlObj As HtmlGenericControl = New HtmlGenericControl("i")

                If myLabel.Text.Equals("EXC") Then
                    myLabel.Text = ""
                    htmlObj.Attributes("class") = "fa fa-file-excel-o fa-1x"
                    htmlObj.Style.Add("color", "#68b604 !important")
                    htmlObj.Style.Add("padding-right", "5px")
                    myLabel.Text = "EXC"
                    myLabel.Style.Add("color", "#68b604 !important")
                    'myLabel.Style.Add("font-size", "12px")
                    'htmlObj.Style.Add("font-size", "12px")
                ElseIf myLabel.Text.Equals("MAN") Then
                    myLabel.Text = ""
                    htmlObj.Attributes("class") = "fa fa-keyboard-o fa-1x"
                    htmlObj.Style.Add("color", "#0069d9 !important")
                    htmlObj.Style.Add("padding-right", "5px")
                    myLabel.Text = "MAN"
                    myLabel.Style.Add("color", "#0069d9 !important")
                    'myLabel.Style.Add("font-size", "12px")
                    'htmlObj.Style.Add("font-size", "12px")
                Else
                    myLabel.Text = ""
                    htmlObj.Attributes("class") = "fa fa-list fa-1x"
                    htmlObj.Style.Add("color", "#C70039 !important")
                    htmlObj.Style.Add("padding-right", "5px")
                    myLabel.Text = "LS"
                    myLabel.Style.Add("color", "#C70039 !important")
                    'myLabel.Style.Add("font-size", "12px")
                    'htmlObj.Style.Add("font-size", "12px")
                End If

                'If Page.Controls.IndexOf(myLabel) >= 0 Then
                '    Page.Controls.AddAt(Page.Controls.IndexOf(myLabel), htmlObj)
                'End If
                myButton.Controls.Add(htmlObj)

            ElseIf e.Row.RowType = DataControlRowType.Header Then

                For index = 0 To grvWishList.Columns.Count - 1
                    Dim name = grvWishList.Columns(index).HeaderText
                    Dim style = grvWishList.Columns(index).ItemStyle().CssClass
                    If style <> "hidecol" Then
                        lstValues.Add(name)
                    End If
                Next
                Session("grvWishListHeaders") = lstValues
            ElseIf e.Row.RowType = DataControlRowType.Footer Then

                Dim lstValuesFoot = DirectCast(Session("grvWishListHeaders"), List(Of String))
                If lstValuesFoot.Count > 0 Then
                    Dim x As Integer = 0
                    For Each item As String In lstValuesFoot
                        'If Trim(item.ToLower()) = "status" Then
                        '    fill_Status(ddlStatusFoot)
                        '    AddHandler ddlStatusFoot.SelectedIndexChanged, AddressOf ddlStatusFoot_SelectedIndexChanged
                        '    e.Row.Cells(x).Controls.Add(ddlStatusFoot)
                        '    e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    x += 1
                        'ElseIf Trim(item.ToLower()) = "assigned" Then
                        '    fill_Users(ddlAssignFoot)
                        '    AddHandler ddlAssignFoot.SelectedIndexChanged, AddressOf ddlAssignFoot_SelectedIndexChanged
                        '    e.Row.Cells(x).Controls.Add(ddlAssignFoot)
                        '    e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    x += 1
                        'ElseIf Trim(item.ToLower()) = "from" Then
                        '    fill_From(ddlFromFoot)
                        '    AddHandler ddlFromFoot.SelectedIndexChanged, AddressOf ddlFromFoot_SelectedIndexChanged
                        '    e.Row.Cells(x).Controls.Add(ddlFromFoot)
                        '    e.Row.Cells(x).Attributes.Add("class", "footermark")
                        '    x += 1
                        'else
                        If Trim(item.ToLower()) = "id" Then
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            'BtnventCss.Attributes.Add("class", "hom_but_a");
                        Else
                            e.Row.Cells(x).Text = item
                            e.Row.Cells(x).Attributes.Add("class", "footermark")
                            x += 1
                        End If
                    Next
                End If
            ElseIf (e.Row.RowType = DataControlRowType.Pager) Then
                Dim strTotal = DirectCast(Session("ItemCounts"), String).ToString()
                Dim strNumberOfPages = DirectCast(Session("PageAmounts"), String).ToString()
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvWishList_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles grvWishList.RowCreated
        Dim exMessage As String = Nothing
        Dim grid = DirectCast(sender, GridView)
        Dim lstValues = New List(Of String)()
        Try
            If e.Row.RowType = DataControlRowType.Header Then
                'For i = 0 To e.Row.Cells.Count - 1
                '    lstValues.Add(e.Row.Cells(i).Text)
                '    Session("grvWishListHeaders") = lstValues
                'Next
            ElseIf e.Row.RowType = DataControlRowType.Footer Then
                'Dim lstValuesFoot = DirectCast(Session("grvWishListHeaders"), List(Of String))
                'If lstValuesFoot.Count > 0 Then
                '    Dim x As Integer = 0
                '    For Each item As String In lstValuesFoot
                '        If Trim(item.ToLower()) = "status" Then
                '            fill_Status(ddlStatusFoot)
                '            AddHandler ddlStatusFoot.SelectedIndexChanged, AddressOf ddlStatusFoot_SelectedIndexChanged
                '            e.Row.Cells(x).Controls.Add(ddlStatusFoot)
                '            x += 1
                '        ElseIf Trim(item.ToLower()) = "assigned" Then
                '            fill_Users(ddlAssignFoot)
                '            AddHandler ddlAssignFoot.SelectedIndexChanged, AddressOf ddlAssignFoot_SelectedIndexChanged
                '            e.Row.Cells(x).Controls.Add(ddlAssignFoot)
                '            x += 1
                '        ElseIf Trim(item.ToLower()) = "from" Then
                '            fill_From(ddlFromFoot)
                '            AddHandler ddlFromFoot.SelectedIndexChanged, AddressOf ddlFromFoot_SelectedIndexChanged
                '            e.Row.Cells(x).Controls.Add(ddlFromFoot)
                '            x += 1
                '        Else
                '            e.Row.Cells(x).Text = item
                '            x += 1
                '        End If
                '    Next
                'End If
            ElseIf e.Row.RowType = DataControlRowType.DataRow Then
                'Dim dl As DropDownList = DirectCast(e.Row.FindControl("ddlStatusFoot"), DropDownList)
                'If dl IsNot Nothing Then
                '    AddHandler dl.SelectedIndexChanged, AddressOf ddlStatusFoot_SelectedIndexChanged
                'End If

                '(DropDownList)e.Row.FindControl("ddlPBXTypeNS")
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvWishList_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grvWishList.Sorting
        Dim dtw As DataView = Nothing
        Dim newDt As DataTable = New DataTable()
        Dim exMessage As String = Nothing
        Dim direction As String = Nothing
        Try
            Dim dt As DataTable = DirectCast(grvWishList.DataSource, DataTable)
            If dt IsNot Nothing Then
                dtw = New DataView(dt)
                direction = DirectCast(Session("sortDirection"), String)
                dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                newDt = dtw.ToTable()
                Dim ds As DataSet = New DataSet()
                ds.Tables.Add(newDt)
                Session("WishListData") = ds
                grvWishList.DataSource = ds
                grvWishList.DataBind()

            Else
                Dim ds As DataSet = New DataSet()
                ds = getDataSource()
                dtw = New DataView(ds.Tables(0))
                direction = DirectCast(Session("sortDirection"), String)
                dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                newDt = dtw.ToTable()
                ds.Tables.RemoveAt(0)
                ds.Tables.Add(newDt)
                Session("WishListData") = ds
                grvWishList.DataSource = ds
                grvWishList.DataBind()

            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Function updateWishListGridView(records As List(Of WishList), ByRef errorUpdate As Integer, Optional flag As Boolean = False) As Integer
        Dim lstData = New List(Of WishList)()
        Dim filterData = New List(Of WishList)()
        Dim result As Integer = -1
        Dim dataWarning As Integer = 0
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("WishListData"), DataSet)
            lstData = fillObj(dsData.Tables(0))
            Dim count1 = lstData.Count()
            'lstData.Remove(lstData.Single(Function(da) LCase(da.IMPTN).Trim() = LCase(partno).Trim()))
            lstData.AddRange(records)
            Dim count2 = lstData.Count()

            If count2 > count1 Then
                Dim dtResult = ListToDataTable(lstData)
                If dtResult IsNot Nothing Then
                    If dtResult.Rows.Count > 0 Then
                        Dim ds = New DataSet()
                        ds.Tables.Add(dtResult)
                        Session("WishListData") = ds

                        If Not flag Then

                            hdFileImportFlag.Value = "0"

                            loadData(ds, Nothing, True)
                        End If
                        result = 0
                    Else
                        'no data, problem in data

                        dataWarning += 1

                        'If Not flag Then
                        '    SendMessage("There is an error updatting the gridview!", messageType.Error)
                        'End If
                    End If
                Else
                    'no data, problem in data

                    dataWarning += 1

                    'If Not flag Then
                    '    SendMessage("There is an error updatting the gridview!", messageType.Error)
                    'End If
                End If
            Else
                'no record to add

                dataWarning += 1

                'If Not flag Then
                '    SendMessage("There is an error updatting the gridview!", messageType.Error)
                'End If
            End If
            errorUpdate = dataWarning
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return result
        End Try
    End Function

#End Region

#Region "Checkbox and radio gridview"

    Public Sub rdSearchByName_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub rdSearchByNo_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub chkAll_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub rdExistingProject_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub rdNewProject_CheckedChanged(sender As Object, e As EventArgs) Handles rdNewProject.CheckedChanged
        Dim exMessage As String = " "
        Try

            Dim controlName As String = Page.Request.Params("__EVENTTARGET")

            If Not String.IsNullOrEmpty(controlName) And LCase(controlName).Contains("rdnewproject") Then
                If String.IsNullOrEmpty(txtCurrentVendor.Text.Trim()) Then
                    Dim methodMessage = "You must select a vendor for this reference."
                    SendMessage(methodMessage, messageType.warning)

                    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "disableInputs()", True)
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Sub rdAssigment_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub rdFrom_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub rdStatus_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

#End Region

#Region "hidden fields"

    Protected Sub hdPartNoSelected_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Try
            Dim partNo As String = hdCustomerNoSelected.Value
            Dim pepe = "1"
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    Protected Sub hdCustomerNoSelected_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim methodMessage = "test message"
        Dim detailInfo = messageType.info
        Try

            Dim vendorName As String = hdCustomerNoSelected.Value
            vendorValidation(vendorName, 1, 2)
            'txtSearch.Text = If(String.IsNullOrEmpty(vendorName), "empty", vendorName)
            'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & " ')", True)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    Protected Sub hdCustomerNoSelected1_ValueChanged(sender As Object, e As EventArgs)
        Dim exMessage As String = " "
        Dim methodMessage = "test message"
        Dim detailInfo = messageType.info
        Try

            Dim vendorName As String = hdCustomerNoSelected1.Value
            vendorValidation(vendorName, 1, 1)
            'txtSearch.Text = If(String.IsNullOrEmpty(vendorName), "empty", vendorName)
            'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & " ')", True)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    'Protected Sub hdnValue_ValueChanged(sender As Object, e As EventArgs)
    '    Dim exMessage As String = " "
    '    Dim methodMessage = "test message"
    '    Dim detailInfo = messageType.info
    '    Try

    '        Dim pepe1 = Request.Form(hdnValue.UniqueID)
    '        Dim vendorName As String = hdnValue.Value
    '        txtSearch.Text = If(String.IsNullOrEmpty(vendorName), "empty", vendorName)
    '        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & " ')", True)
    '        Dim pepe = "1"
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '    End Try
    'End Sub

#End Region

#Region "Generics"

    Public Function getAllPurcUsers() As List(Of String)
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Dim lstPurcUsers As List(Of String) = New List(Of String)()
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.getAllPurcUsers(dsResult)
                If rsData > 0 Then
                    If dsResult IsNot Nothing Then
                        If dsResult.Tables(0).Rows.Count > 0 Then

                            For Each dw As DataRow In dsResult.Tables(0).Rows
                                lstPurcUsers.Add(UCase(dw.Item("USUSER").ToString().Trim()))
                            Next

                        End If
                    End If
                End If

                'Return If(rsData > 0, dsResult.Tables(0).Rows(0).ItemArray(0).ToString(), Nothing)
            End Using
            Return lstPurcUsers
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Function GetAccessByUsers(ByRef sel As Integer, ByRef fullData As Boolean) As Boolean
        Dim optionSelection As String = Nothing
        Dim user As String = Nothing
        Dim flag As Boolean = False
        Dim authUser As String = Nothing
        Dim lstPurcUsers As List(Of String) = New List(Of String)()

        Try
            Dim validUsers = ConfigurationManager.AppSettings("validUsersForWeb")

            'Dim args As String() = Environment.GetCommandLineArgs()
            'Dim argumentsJoined = String.Join(".", args)

            'Dim arrayArgs As String() = argumentsJoined.Split(".")
            'optionSelection = UCase(arrayArgs(3).ToString().Replace(",", ""))
            'user = UCase(arrayArgs(2).ToString().Replace(",", ""))

            user = If(Session("userid") IsNot Nothing, UCase(Session("userid").ToString().Trim()), "NA")
            If Not user.Equals("NA") Then

                lstPurcUsers = getAllPurcUsers()
                If lstPurcUsers.Count > 0 Then
                    lstPurcUsers.Add("AAVILA") 'test

                    authUser = lstPurcUsers.AsEnumerable().Where(Function(val) UCase(val).Trim().Contains(user)).First()

                    If Not String.IsNullOrEmpty(authUser) Then

                        fullData = If(LCase(validUsers.Trim()).Contains(LCase(authUser.Trim())), True, False)
                        'fullData = False 'test remove
                        'Session("userid") = user
                        'full query -- >
                        flag = True
                        Return flag
                    Else
                        'test
                        'Session("userid") = ConfigurationManager.AppSettings("authorizeTestUser")
                        'test

                        'not authorized user
                        sel = 0
                        Return False
                    End If

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

    Public Function getParamtersFromCTPSystem() As Boolean
        Dim optionSelection As String = Nothing
        Dim user As String = Nothing
        Dim flag As Boolean = False
        Session("userid") = Nothing
        Dim exMessage As String = Nothing
        Try
            Dim validUsers = ConfigurationManager.AppSettings("validUsersForWeb")

            Dim args As String() = Environment.GetCommandLineArgs()
            Dim argumentsJoined = String.Join(".", args)

            Dim arrayArgs As String() = argumentsJoined.Split(".")
            optionSelection = UCase(arrayArgs(3).ToString().Replace(",", ""))
            user = UCase(arrayArgs(2).ToString().Replace(",", ""))

            If LCase(validUsers.Trim()).Contains(LCase(user.Trim())) Then
                Session("userid") = user
                flag = True
                Return flag
            Else
                'test
                Session("userid") = ConfigurationManager.AppSettings("authorizeTestUser")
                'test

                'Response.Redirect("http://wwww.costex.com", True)
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return flag
        End Try
    End Function

    Public Sub updatePagerSettings(grv As GridView)
        Dim exMessage As String = Nothing
        Try

#Region "Need works"

            'Dim amount = DirectCast(Session("PageAmountsDdl"), Integer)
            'Dim strTotal = (DirectCast(Session("ItemCounts"), Integer)).ToString()
            'Dim pIndex = If(Session("PageIndex") IsNot Nothing, DirectCast(Session("PageIndex"), Integer), 1)
            ''Dim strNumberOfPages = DirectCast(Session("PageAmounts"), Integer).ToString()
            'Dim strNumberOfPages = If((DirectCast(Session("PageAmountsDdl"), Integer) * pIndex) > DirectCast(Session("ItemCounts"), Integer), DirectCast(Session("ItemCounts"), Integer).ToString(), (DirectCast(Session("PageAmountsDdl"), Integer) * pIndex).ToString())

            'Session("PageIndex") = If(DirectCast(Session("PageIndex"), Integer) * amount > CInt(strTotal), (CInt(strTotal) / amount), DirectCast(Session("PageIndex"), Integer))
            'Session("currentPage") = (DirectCast(Session("PageIndex"), Integer) * amount) - (amount - 1)

            'Dim strCurrentPage = ((DirectCast(Session("currentPage"), Integer))).ToString()

#End Region

            Dim strTotal = DirectCast(Session("ItemCounts"), String)
            Dim strNumberOfPages = DirectCast(Session("PageAmounts"), String).ToString()
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
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Function GetCheckboxesDisp() As Dictionary(Of String, String)

        Dim lstPartsToWL As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim exMessage As String = Nothing
        Try
            Dim checkAll As CheckBox = grvWishList.HeaderRow.FindControl("chkAll")
            If checkAll.Checked Then
                For Each gvr As GridViewRow In grvWishList.Rows
                    Dim userid As String = If(Not String.IsNullOrEmpty(gvr.Cells(8).Text) And gvr.Cells(8).Text <> "&nbsp;", gvr.Cells(8).Text, "N/A")
                    Dim lbl1 = DirectCast(gvr.FindControl("lbPartNo"), LinkButton)
                    Dim partNo = If(lbl1 IsNot Nothing, Trim(lbl1.CommandArgument.ToString()), "")
                    lstPartsToWL.Add(partNo, userid)
                    'lstPartsToWL.Add(Trim(gvr.Cells(2).Text))
                Next
            Else
                For Each gvr As GridViewRow In grvWishList.Rows
                    Dim Check As CheckBox = gvr.FindControl("chkSingleAdd")
                    If Check.Checked Then
                        Dim userid As String = If(Not String.IsNullOrEmpty(gvr.Cells(8).Text) And gvr.Cells(8).Text <> "&nbsp;", gvr.Cells(8).Text, "N/A")
                        Dim lbl1 = DirectCast(gvr.FindControl("lbPartNo"), LinkButton)
                        Dim partNo = If(lbl1 IsNot Nothing, Trim(lbl1.CommandArgument.ToString()), "")
                        lstPartsToWL.Add(partNo, userid)
                    End If
                Next
            End If

            Return lstPartsToWL
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try
    End Function

    Public Sub ClearInputCustom(parent As Control)
        Dim exMessage As String = Nothing
        Try
            For Each ctl As Control In parent.Controls

                If (ctl.Controls.Count > 0) Then
                    ClearInputCustom(ctl)
                Else
                    If TypeOf ctl Is TextBox Then
                        DirectCast(ctl, TextBox).Text = String.Empty
                    End If
                    If TypeOf ctl Is Label Then
                        DirectCast(ctl, Label).Text = String.Empty
                    End If
                    If TypeOf ctl Is DropDownList Then
                        If (DirectCast(ctl, DropDownList).Enabled Or Not (DirectCast(ctl, DropDownList)).Enabled) Then
                            DirectCast(ctl, DropDownList).ClearSelection()
                        End If
                    End If
                    If TypeOf ctl Is ListBox Then
                        If (DirectCast(ctl, ListBox).Enabled Or Not (DirectCast(ctl, ListBox)).Enabled) Then
                            DirectCast(ctl, ListBox).ClearSelection()
                            DirectCast(ctl, ListBox).Items.Clear()
                        End If
                    End If
                    If TypeOf ctl Is CheckBox Then
                        DirectCast(ctl, CheckBox).Checked = False
                    End If
                End If

            Next
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Sub fieldsVisibility(parent As Control, flag As Boolean)
        Dim exMessage As String = Nothing
        Try
            For Each ctl As Control In parent.Controls

                If (ctl.Controls.Count > 0) Then
                    ClearInputCustom(ctl)
                Else
                    If TypeOf ctl Is TextBox Then
                        DirectCast(ctl, TextBox).Enabled = flag
                    End If
                    If TypeOf ctl Is DropDownList Then
                        DirectCast(ctl, DropDownList).Enabled = flag
                    End If
                    If TypeOf ctl Is CheckBox Then
                        DirectCast(ctl, CheckBox).Enabled = flag
                    End If
                End If

            Next
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Function updateWishListGridView(partno As String, Optional flag As Boolean = False) As Integer
        Dim lstData = New List(Of WishList)()
        Dim filterData = New List(Of WishList)()
        Dim result As Integer = -1
        Dim exMessage As String = Nothing
        Try
            Dim userText As String = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")

            Dim dsData = DirectCast(Session("WishListData"), DataSet)
            Dim code = LCase(hdWhlCode1.Value.Trim())

            lstData = fillObj(dsData.Tables(0))
            Dim count1 = lstData.Count()
            lstData.Remove(lstData.Single(Function(da) (LCase(da.IMPTN).Trim() = LCase(partno).Trim()) And LCase(da.WHLCODE.Trim() = code)))
            Dim count2 = lstData.Count()
            If count1 > count2 Then
                Dim dtResult = ListToDataTable(lstData)
                If dtResult IsNot Nothing Then
                    If dtResult.Rows.Count > 0 Then
                        Dim ds = New DataSet()
                        ds.Tables.Add(dtResult)
                        Session("WishListData") = ds

                        'update status

                        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                            Dim resultUpd = objBL.UpdateWishListTwoReferences(code, partno, "5", userText)
                            If resultUpd <> 1 Then
                                SendMessage("There is an error updatting the gridview!", messageType.Error)
                            End If
                        End Using

                        If Not flag Then
                            loadData(ds)
                        End If
                        result = 0
                        hdProdDevFlag.Value = "0"
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
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return result
        End Try
    End Function

    Public Function updateWishListBckGridView(partno As String, Optional flag As Boolean = False) As Integer
        Dim lstData = New List(Of WishList)()
        Dim filterData = New List(Of WishList)()
        Dim result As Integer = -1
        Dim exMessage As String = Nothing
        Try
            Dim userText As String = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")

            Dim dsData = DirectCast(Session("WishListBck"), DataSet)
            Dim code = LCase(hdWhlCode1.Value.Trim())

            lstData = fillObj(dsData.Tables(0))
            Dim count1 = lstData.Count()
            lstData.Remove(lstData.Single(Function(da) (LCase(da.IMPTN).Trim() = LCase(partno).Trim()) And LCase(da.WHLCODE.Trim() = code)))
            Dim count2 = lstData.Count()
            If count1 > count2 Then
                Dim dtResult = ListToDataTable(lstData)
                If dtResult IsNot Nothing Then
                    If dtResult.Rows.Count > 0 Then
                        Dim ds = New DataSet()
                        ds.Tables.Add(dtResult)
                        Session("WishListBck") = ds

                        'update status

                        Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                            Dim resultUpd = objBL.UpdateWishListTwoReferences(code, partno, "5", userText)
                            If resultUpd <> 1 Then
                                SendMessage("There is an error updatting the gridview!", messageType.Error)
                            End If
                        End Using

                        If Not flag Then
                            loadData(ds)
                        End If
                        result = 0
                        hdProdDevFlag.Value = "0"
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
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return result
        End Try
    End Function

    Public Sub DoExcel(dtResult As DataTable)
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Dim exMessage As String = Nothing
        Try
            If dtResult IsNot Nothing Then
                If dtResult.Rows.Count > 0 Then

                    Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    Dim folderPath As String = userPath & "\wish-list-data\"

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            Exit Sub
                        End If

                        Dim title As String
                        title = "Wish-List_Generated_by "
                        fileName = objBL.adjustDatetimeFormat(title, fileExtension)

                    End Using

                    Dim fullPath = folderPath + fileName

                    Using wb As New XLWorkbook()
                        wb.Worksheets.Add(dtResult, "WishList")
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
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Private Function fitSelection() As IEnumerable(Of WishList)
        Dim exMessage As String = Nothing
        Try
            Dim dsAllData As DataSet = DirectCast(Session("WishListData"), DataSet)
            'Dim tquote = DirectCast(Session("TimesQuote"), String)
            Dim vndSel = DirectCast(Session("flagVnd"), String)

            Dim newData = New List(Of WishList)()

            If dsAllData IsNot Nothing Then

                Dim lstAllData = fillObj(dsAllData.Tables(0))
                Dim iteration1 = lstAllData.AsEnumerable()

                If iteration1 IsNot Nothing Then
                    If ddlStatusFoot.SelectedIndex <> 0 Then
                        Dim iteration2 = iteration1.AsEnumerable().Where(Function(val) UCase(Trim(val.WHLSTATUS)) = UCase(Trim(ddlStatusFoot.SelectedItem.ToString())))
                        If iteration2 IsNot Nothing Then
                            iteration1 = iteration2
                        End If
                    End If
                Else
                    Return Nothing
                End If

                If iteration1 IsNot Nothing Then
                    If ddlAssignFoot.SelectedIndex <> 0 Then
                        Dim iteration3 = iteration1.AsEnumerable().Where(Function(val) UCase(Trim(val.WHLSTATUSU)) = UCase(Trim(ddlAssignFoot.SelectedItem.ToString())))
                        If iteration3 IsNot Nothing Then
                            iteration1 = iteration3
                        End If
                    End If
                Else
                    Return Nothing
                End If

                If iteration1 IsNot Nothing Then
                    If ddlFromFoot.SelectedIndex <> 0 Then
                        Dim iteration4 = iteration1.AsEnumerable().Where(Function(val) UCase(Trim(val.WHLFROM)) = UCase(Trim(ddlFromFoot.SelectedItem.ToString())))
                        If iteration4 IsNot Nothing Then
                            iteration1 = iteration4
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
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

    Public Sub fillPartInfoPD(partNo As String)
        Dim exMessage As String = Nothing
        Try
            Dim dsData = DirectCast(Session("WishListData"), DataSet)
            For Each row As DataRow In dsData.Tables(0).Rows
                If UCase(Trim(row.Item("IMPTN").ToString())) = partNo Then
                    txtWhlCode.Text = row.Item("WHLCODE").ToString() 'code
                    hdWhlCode.Value = txtWhlCode.Text

                    txtPartNoPD.Text = row.Item("IMPTN").ToString()  'part

                    txtCreationDate.Text = row.Item("WHLDATE").ToString() 'creation date
                    hdCreationDate.Value = txtCreationDate.Text

                    txtUserCreated.Text = row.Item("WHLUSER").ToString() 'created user
                    hdUserCreated.Value = txtUserCreated.Text

                    txtDescriptionPD1.Text = row.Item("IMDSC").ToString() 'description

                    'txtAssignedToPD.Text = row.Item("WHLSTATUSU").ToString() 'assigned user
                    txtAssignedToPD.Text = ddlAssignedTo.SelectedItem.Text
                    hdAssignedToPD.Value = txtAssignedToPD.Text

                    txtCurrentVendor.Text = If(row.Item("VENDOR").ToString() = "000000" Or row.Item("VENDOR").ToString() = " ", "", row.Item("VENDOR").ToString()) 'vendor
                    hdCurrentVendor.Value = txtCurrentVendor.Text

                    txtQtySoldPD.Text = row.Item("qtysold").ToString()  'sold qty
                    hdQtySoldPD.Value = txtQtySoldPD.Text

                    txtTimesQuoteLY.Text = row.Item("TIMESQ").ToString() 'times quote
                    hdTimesQuoteLY.Value = txtTimesQuoteLY.Text

                    txtOEMPricePD.Text = row.Item("IMPRC").ToString() ' oem price
                    hdOEMPricePD.Value = txtOEMPricePD.Text

                    txtMinorCodePD.Text = row.Item("IMPC2").ToString() 'minor
                    hdMinorCodePD.Value = txtMinorCodePD.Text

                    txtCommentsPD.Text = GetCommentById(row.Item("WHLCODE").ToString()) 'comments
                    hdCommentsPD.Value = txtCommentsPD.Text

                    txtReasonTypePD.Text = ddlType.Items.FindByValue("1").ToString()
                    hdReasonTypePD.Value = txtReasonTypePD.Text

                    txtDescriptionPD.Text = row.Item("vendorname").ToString()
                    hdDescriptionPD.Value = txtDescriptionPD.Text
                    Exit For
                End If
            Next
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Function GetCommentById(id As String) As String
        Dim exMessage As String = Nothing
        Dim commentValue As String = Nothing
        Try
            Dim dsData = DirectCast(Session("WishListData"), DataSet)
            For Each item As DataRow In dsData.Tables(0).Rows
                If item.Item("WHLCODE").ToString() = id Then
                    commentValue = Trim(item.Item("A3COMMENT"))
                    Exit For
                End If
            Next
            Return commentValue
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return commentValue
        End Try
    End Function

    Public Function GetVendorNameById(id As String) As String
        Dim exMessage As String = Nothing
        Dim vendorName As String = Nothing
        Try
            Dim dsData = DirectCast(Session("WishListData"), DataSet)
            For Each item As DataRow In dsData.Tables(0).Rows
                If item.Item("VENDOR").ToString() = id Then
                    vendorName = Trim(item.Item("vendorname"))
                    Exit For
                End If
            Next
            Return vendorName
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return vendorName
        End Try
    End Function

    Public Function checkInnerDropDownCreated() As Boolean
        Dim exMessage As String = Nothing
        Dim outDs As New DataSet
        Try
            Dim ph As ContentPlaceHolder = DirectCast(Me.Master.FindControl("MainContent"), ContentPlaceHolder)
            Dim grv As GridView = DirectCast(ph.FindControl("grvWishList"), GridView)
            If grv.DataSource Is Nothing Then
                Dim ds = DirectCast(Session("WishListData"), DataSet)
                GetWishListData(0, Nothing, ds)
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return False
        End Try
    End Function

    Private Function getDataSourceDif(dgv As GridView, Optional ByRef dsDataSource As DataSet = Nothing) As Boolean
        Dim exMessage As String = Nothing
        Try
            Dim dtGrid = TryCast(dgv.DataSource, DataTable)
            Dim dsSessionGrid = DirectCast(Session("WishListData"), DataSet)
            If dtGrid.Rows.Count = dsSessionGrid.Tables(0).Rows.Count Then
                dsDataSource = dsSessionGrid
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return False
        End Try
    End Function

    Public Function getDataSource(Optional preventFilters As Boolean = False) As DataSet
        Dim exMessage As String = Nothing
        Try
            If preventFilters Then

                Session("flagVnd") = "4"
                'Dim dsDataGrid = DirectCast(Session("LostSaleBck"), DataSet)
                'GetLostSalesData(Nothing, 1, Nothing, dsDataGrid)
            Else
                Dim dsDataGrid = DirectCast(Session("WishListData"), DataSet)
                'Dim dsSetDataSource = New DataSet()
                If grvWishList.DataSource Is Nothing Then
                    'grvLostSales.DataSource = dsDataGrid.Tables(0)
                    'grvLostSales.DataBind()
                    Return dsDataGrid
                Else
                    If getDataSourceDif(grvWishList, dsDataGrid) Then
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
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try
    End Function

    Private Sub loadData(Optional ds As DataSet = Nothing, Optional dt As DataTable = Nothing, Optional flag As Boolean = False)
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Try
            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then

                    'grvWishList.PageSize = DirectCast(Session("PageAmountsDdl"), Integer)

                    grvWishList.DataSource = ds.Tables(0)
                    grvWishList.DataBind()
                    Session("WishListData") = ds
                Else
                    grvWishList.DataSource = Nothing
                    grvWishList.DataBind()

                    If Not flag Then
                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)
                    End If

                End If

                Exit Sub
            ElseIf dt IsNot Nothing Then
                If dt.Rows.Count > 0 Then

                    'grvWishList.PageSize = DirectCast(Session("PageAmountsDdl"), Integer)

                    grvWishList.DataSource = dt
                    grvWishList.DataBind()

                    Dim dtt = New DataTable()
                    dtt = dt.Copy()
                    Dim dss = New DataSet()
                    dss.Tables.Add(dtt)
                    Session("WishListData") = dss
                Else
                    grvWishList.DataSource = Nothing
                    grvWishList.DataBind()

                    If Not flag Then
                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)
                    End If

                End If
            Else
                Dim sel As Integer = 0
                Dim fullData As Boolean = False

                Dim flagUpdate = GetAccessByUsers(sel, fullData)
                If flagUpdate Then

                    If Not fullData Then

                        hdUserVisibility.Value = "1"
                        Dim customUser = UCase(Session("userid").ToString().Trim())
                        Session("NoPrivilegesUser") = customUser
                        'customUser = "CMONTILVA" 'test remove
                        Dim strOptionalQuery As String = " and a3.whlstatusu = '{0}' "
                        Dim sqlResult As String = String.Format(strOptionalQuery, customUser)
                        ddlStatus2.Enabled = False
                        ddlAssignedTo.Enabled = False

                        GetWishListData(0, Nothing, Nothing, sqlResult)
                    Else
                        ddlStatus2.Enabled = True
                        ddlAssignedTo.Enabled = True
                        hdUserVisibility.Value = "0"
                        GetWishListData(0)

                    End If

                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Function GetWishListData(flag As Integer, Optional ByRef dsResult As DataSet = Nothing, Optional dsLoad As DataSet = Nothing, Optional userSql As String = Nothing) As Integer
        Dim exMessage As String = Nothing
        dsResult = New DataSet()
        Dim result As Integer = -1

        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                If dsLoad IsNot Nothing Then
                    If dsLoad.Tables(0).Rows.Count > 0 Then

                        Session("ItemCounts") = dsLoad.Tables(0).Rows.Count.ToString()

                        loadData(dsLoad)

                        dsResult = dsLoad
                        Session("WishListData") = dsLoad
                        If flag = 0 Then
                            Session("flagBck") = dsLoad
                        End If
                    End If
                Else
                    Dim messageOut As String = Nothing

                    result = If(String.IsNullOrEmpty(userSql), objBL.GetWishListData(dsResult, messageOut), objBL.GetWishListDataByUser(userSql, dsResult, messageOut))

                    'result = objBL.GetWishListData(dsResult, messageOut)
                    Log.Info(strLogCadenaCabecera + ".. Result is: " + result.ToString())
                    Log.Info(strLogCadenaCabecera + ".. Exception is: " + messageOut)
                    If result > 0 Then
                        If dsResult IsNot Nothing Then
                            If dsResult.Tables(0).Rows.Count > 0 Then
                                'lblItemsCount.Text = dsResult.Tables(0).Rows.Count.ToString()
                                'Session("ItemCounts") = dsResult.Tables(0).Rows.Count.ToString()
                                Session("ItemCounts") = dsResult.Tables(0).Rows.Count.ToString()

                                'Dim ppe As Integer = DirectCast(Session("ItemCounts"), Integer)

                                Session("WishListData") = dsResult

                                loadData(dsResult)

                                If flag = 0 Then
                                    Session("WishListBck") = dsResult

                                    'DoExcel(dsResult.Tables(0))

                                    Session("flagBck") = "1"
                                End If
                            End If
                        End If
                    End If
                End If
            End Using

            Return result
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)

            'writeComputerEventLog(ex.Message)

            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return result
        End Try

    End Function

    Public Sub LoadCombos()
        fill_Types(ddlType)
        fill_Types(dlType)
        fill_Minor(dlMinor)
        fill_Status(ddlStatus2)
        fill_Users(ddlAssignedTo)
        fill_Status(ddlStatus3)
        fill_Users(ddlUser2)
        fill_Page_Size(ddlPageSize)

        fill_Users(ddlAssign)
        fill_Status(ddlStatus)
        fill_From(ddlFrom)

    End Sub

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

    Public Sub SendMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & " ')", True)
    End Sub

    Public Sub SendConfirmationMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "confirmFormSubmitted('" & methodMessage & " ', '" & detailInfo & " ')", True)
    End Sub

    Public Function processExcelData(dt As DataTable) As String
        Dim exMessage As String = Nothing
        Dim userid As String = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
        Dim ds As DataSet = New DataSet()
        Dim countReferences As Integer = 0
        Dim lstRecords = New List(Of WishList)()
        Dim dsRef As DataSet = New DataSet()
        Dim insertionErrors As Integer = 0
        Dim partExists As Integer = 0
        Try
            For Each dw As DataRow In dt.Rows
                Dim partNo = dw.Item("PARTNUMBER").ToString().Trim()
                If Not String.IsNullOrEmpty(partNo) Then
                    Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                        Dim existsPart = objBL.GetPartInWishList(partNo, ds)
                        Dim result As Integer = 0
                        If existsPart = 0 Then
                            result = objBL.InsertWishListReference(userid, partNo, "1", "4", "QS36F.PRDWL", "WHLCODE", dsRef)

                            'status when add to wish list
                            If result > 0 Then
                                countReferences = result

                                Dim dtFull = fixDatatableHeaderForObject(dsRef.Tables(0))
                                If dtFull IsNot Nothing Then
                                    Dim lstData = fillObj(dtFull)
                                    lstRecords.Add(lstData(0))
                                End If

                                'lstRecords.Add()
                                'methodMessage = "Successful Insertion for " + result.ToString() + " record."

                                'Dim resultMethod = updateLostSaleGridView(partNo)
                                'If resultMethod = 0 Then
                                '    SendMessage(methodMessage, messageType.success)
                                'End If
                            Else
                                insertionErrors += 1
                                'methodMessage = "There is an error in the insertion process."
                                'SendMessage(methodMessage, messageType.Error)
                            End If
                        Else
                            partExists += 1
                            'Dim resultMethod = updateLostSaleGridView(partNo)
                            'If resultMethod = 0 Then
                            '    methodMessage = "There is already a reference of the part " + partNo.Trim() + " in Wishlist. This reference will be removed from this screen."
                            '    SendMessage(methodMessage, messageType.warning)
                            'End If
                            'Exit Sub
                        End If

                    End Using
                End If
            Next

            If countReferences > 0 Then
                Dim errorUpdate As Integer = 0
                Dim resultMethod = updateWishListGridView(lstRecords, errorUpdate)

                Dim lstValues = New Dictionary(Of Integer, String)()

                'If insertionErrors > 0 Then
                '    lstValues.Add(0, "Records: '" + insertionErrors.ToString() + "' "" .Errors inserting data.")
                'ElseIf partExists > 0 Then
                '    lstValues.Add(1, "Records: '" + partExists.ToString() + "' "" .The part already exists in wishlist.")
                'ElseIf errorUpdate > 0 Then
                '    lstValues.Add(2, "Records: '" + errorUpdate.ToString() + "' "" .There is an issue with the data.")
                'End If

                Dim strResult = createOutputMessage(lstValues)
                If Not String.IsNullOrEmpty(strResult) Then
                    SendMessage(strResult, messageType.warning)
                Else
                    Dim methodMessage = "Successful Insertion for '" + countReferences.ToString() + "' record."
                    SendMessage(methodMessage, messageType.warning)
                End If
            Else
                Dim methodMessage1 = "Please you must add valid references in order to process the data."
                SendMessage(methodMessage1, messageType.warning)
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try
    End Function

    Public Function createOutputMessage(values As Dictionary(Of Integer, String)) As String
        Dim endMessage As String = String.Empty
        Dim strMessageFull As String = String.Empty
        Dim countOptions As Integer = 0
        Dim exMessage As String = Nothing
        Try
            If values.Count > 0 Then
                For Each dc In values
                    If dc.Key Then
                        strMessageFull += dc.Value + ","
                        countOptions += 1
                    End If
                Next

                If Not String.IsNullOrEmpty(strMessageFull) Then
                    endMessage = buildGeneralMessage(strMessageFull, countOptions)
                End If
            End If

            Return endMessage

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return endMessage
        End Try

    End Function

    Public Function buildGeneralMessage(messageFull As String, count As Integer) As String
        Dim strChainSingle = "There is the following issue: {0}. Please check your data input."
        Dim strChainDouble = "There are the following issues: {0} {1} . Please check your data input."
        Dim strChainTriple = "There are the following issues: {0} {1} {2}. Please check your data input."
        Dim generalMessage As String = String.Empty
        Dim exMessage As String = Nothing
        Try
            messageFull = messageFull.TrimEnd(",")
            Dim arrayMessage = Nothing
            'messageFull.Split(",")

            If count = 1 Then
                generalMessage = String.Format(strChainSingle, messageFull)
            ElseIf count = 2 Then
                arrayMessage = messageFull.Split(",")
                generalMessage = String.Format(strChainDouble, messageFull(0), messageFull(1))
            ElseIf count = 3 Then
                arrayMessage = messageFull.Split(",")
                generalMessage = String.Format(strChainTriple, messageFull(0), messageFull(1), messageFull(2))
            Else
                generalMessage = Nothing
            End If
        Catch ex As Exception
            generalMessage = "An exception occurs: " + ex.Message
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

        Return generalMessage

    End Function

    Public Function fixDatatableHeaderForObject(dt As DataTable) As DataTable
        Dim exMessage As String = Nothing
        Dim dtTemp = New DataTable()
        dtTemp = dt.Copy()
        Try
            Dim lstTest = New List(Of WishList)()
            Dim objTest = New WishList()
            lstTest.Add(objTest)
            Dim dtt = ListToDataTable(lstTest)
            Dim originalHeaders As String = ""

            For Each dc As DataColumn In dtt.Columns
                originalHeaders += dc.ColumnName + ","
            Next

            originalHeaders = originalHeaders.TrimEnd(",")
            Dim origenArray As String() = originalHeaders.Split(",")

            For Each str As String In origenArray
                If Not dt.Columns.Contains(str) Then
                    dtTemp.Columns.Add(str, GetType(String))
                End If
            Next

            Return dtTemp
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try

    End Function

    Protected Function isValidExtension(ext As String) As Boolean
        Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
        Dim extensions = ConfigurationManager.AppSettings("validExtensions")
        Dim exMessage As String = Nothing
        Try
            Dim validFileTypes As String() = extensions.Split(",")
            'String[] validFileTypes = { "png", "jpg", "jpeg", "rtf", "doc", "pdf", "docx", "zip", "rar", "msg", "xls", "xlsx" };
            Dim IsValid As Boolean = False
            For Each item As String In validFileTypes
                If LCase(ext.Trim()).Equals(LCase(item.Trim())) Then
                    IsValid = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

        Return IsValid
    End Function

    Public Function GetDataTableFromExcel(myFile As FileUpload) As DataTable
        Dim exMessage As String = Nothing
        Dim conStr As String = Nothing
        Dim sheetName As String = Nothing
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                Dim uploadedFiles As HttpFileCollection = Request.Files
                Dim userPostedFile As HttpPostedFile = uploadedFiles(0)

                Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                Dim updUserPath = userPath + "\WishList-Template\"

                If (userPostedFile.ContentLength > 0 And userPostedFile.ContentLength < Convert.ToInt32(ConfigurationManager.AppSettings("MaxFileSize"))) Then

                    Dim extension = Path.GetExtension(myFile.FileName)
                    If isValidExtension(Path.GetExtension(extension)) Then

                        Dim filePath As String = fuOPenEx.FileName
                        Dim myFileInfo As FileInfo = New FileInfo(Path.Combine(updUserPath, filePath))
                        Dim isOpened = IsFileinUse(myFileInfo)
                        If Not isOpened Then
                            Select Case extension
                                Case ".xls"
                                    'Excel 97-03
                                    conStr = String.Format(Excel03ConString, myFileInfo.FullName, "YES", 1)
                                    Exit Select
                                Case ".xlsx"
                                    'Excel 07
                                    conStr = String.Format(Excel07ConString, myFileInfo.FullName, "YES", 1)
                                    Exit Select
                                Case ".csv"
                                    conStr = String.Format(CsvConString, myFileInfo.FullName, "YES")
                                    Exit Select
                            End Select

                            Using con As New OleDbConnection(conStr)
                                Using cmd As New OleDbCommand()
                                    cmd.Connection = con
                                    con.Open()
                                    Dim dtExcelSchema As DataTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
                                    sheetName = dtExcelSchema.Rows(0)("TABLE_NAME").ToString()
                                    con.Close()
                                End Using
                            End Using

                            Using con As New OleDbConnection(conStr)
                                Using cmd As New OleDbCommand()
                                    Using oda As New OleDbDataAdapter()
                                        Dim dt As New DataTable()
                                        dt.Columns.Add("PART_NUMBER", GetType(String))
                                        dt.Columns.Add("MINOR", GetType(String))
                                        dt.AcceptChanges()
                                        cmd.CommandText = (Convert.ToString("SELECT * From [") & sheetName) + "]"
                                        cmd.Connection = con
                                        con.Open()
                                        oda.SelectCommand = cmd
                                        'oda.TableMappings.Add("Table", "Net-informations.com")
                                        oda.Fill(dt)

                                        Dim exColumnNames As String = ConfigurationManager.AppSettings("checkColumns")
                                        Dim cleanColumns = RemoveEmptyColumns(dt, exColumnNames)
                                        If cleanColumns Then
                                            'processExcelData(dt)
                                            Return dt
                                        End If
                                    End Using
                                End Using
                            End Using

                        Else
                            'show message
                            Return Nothing
                        End If
                    End If

                End If

            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try
    End Function

    Public Function IsFileinUse(file As FileInfo) As Boolean
        Dim exMessage As String = Nothing
        Dim opened As Boolean = False
        Dim myStream As FileStream = Nothing
        Try
            myStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
        Catch ex As Exception

            If TypeOf ex Is IOException AndAlso IsFileLocked(ex) Then
                IO.File.Delete(file.Name)
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

    Public Function RemoveEmptyColumns(Datatable As DataTable, exColumns As String) As Boolean

        Dim exMessage As String = Nothing
        Dim strColumns As String() = If(Not String.IsNullOrEmpty(exColumns), exColumns.Split(","), "")
        Dim goAhead As Boolean = False
        Try
            Dim mynetable As DataTable = Datatable.Copy
            Dim counter As Integer = mynetable.Rows.Count
            Dim col As DataColumn
            For Each col In mynetable.Columns
                If strColumns.Length > 0 Then
                    For Each item As String In strColumns
                        If Trim(item).Equals(col.ColumnName) Then
                            goAhead = True
                            Exit For
                        End If
                    Next
                End If
                If goAhead Then
                    goAhead = False
                    Continue For
                Else
                    Dim dr() As DataRow = mynetable.Select(col.ColumnName + " is   Null ")
                    If dr.Length = counter Then
                        Datatable.Columns.Remove(col.ColumnName)
                        Datatable.AcceptChanges()
                    End If
                End If
            Next
            Return True
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
            Return False
        End Try

    End Function

    Private Function fillObj(dt As DataTable) As List(Of WishList)
        Dim exMessage As String = Nothing
        Dim objLosSales = Nothing
        Try

            'Dim blah = exampleItems.Select (Function(x) New With { .Key = x.Key, .Value = x.Value }).ToList

            Dim items As IList(Of WishList) = dt.AsEnumerable() _
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
                .A3COMMENT = row.Item("A3COMMENT").ToString(),
                .VENDORNAME = row.Item("vendorname").ToString()
                }).ToList()

            Return items
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
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

    Public Function ListToDataTable1(ByVal _List As Object) As DataTable

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
            'writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try

    End Function

    Protected Sub LoadingDropDownList(dwlControl As DropDownList, displayMember As String, valueMember As String, data As DataTable, genrateSelect As Boolean, strTextSelect As String)

        Dim dtTemp As DataTable = data.Copy()
        dwlControl.Items.Clear()
        If (genrateSelect) Then
            Dim row As DataRow = dtTemp.NewRow()

            If dwlControl.ID = "ddlAssign" Or dwlControl.ID = "ddlAssignedTo" Or dwlControl.ID = "ddlUser2" Then
                row("FULLNAME") = "  "
            End If

            row(displayMember) = strTextSelect
            row(valueMember) = -1
            dtTemp.Rows.InsertAt(row, 0)
        End If

        dwlControl.DataSource = dtTemp
        dwlControl.DataTextField = displayMember
        dwlControl.DataValueField = valueMember
        dwlControl.DataBind()

    End Sub

    Public Shared Function convertFromDataTableToListString(data As DataTable, columnName As String) As List(Of String)
        Dim lsResult As New List(Of String)
        Dim exMessage As String = Nothing

        Try

            If data IsNot Nothing Then
                If data.Rows.Count > 0 Then
                    For Each dw As DataRow In data.Rows
                        lsResult.Add(dw.Item(columnName).ToString())
                    Next
                End If
            End If
            Return lsResult

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try

    End Function

    Public Sub vendorValidation(value As String, flag As Integer, Optional sel As Integer = 0)
        Dim exMessage As String = Nothing
        Dim rsData As Integer = -1
        Dim vendorOEMCodeDenied As String = ConfigurationManager.AppSettings("vendorOEMCodeDenied")
        Dim itemCategories As String = ConfigurationManager.AppSettings("itemCategories")
        Dim vendorCodesDenied As String = ConfigurationManager.AppSettings("vendorCodesDenied")
        Dim dsResult As New DataSet

        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                If flag = 0 Then ' vendorNo
                    rsData = objBL.GetVendorByNumber(value, vendorCodesDenied, vendorOEMCodeDenied, itemCategories, dsResult)
                Else ' vendorName
                    rsData = objBL.GetAutocompleteSelectedVendorName(value, vendorCodesDenied, vendorOEMCodeDenied, itemCategories, dsResult)
                End If

                If rsData > 0 Then

                    If sel = 1 Then
                        txtNewVendorPD.Text = dsResult.Tables(0).Rows(0).ItemArray(1).ToString()
                        txtNewVendorNo.Text = dsResult.Tables(0).Rows(0).ItemArray(0).ToString()
                    Else
                        txtvendor.Text = dsResult.Tables(0).Rows(0).ItemArray(0).ToString()
                        txtVndDesc.Text = dsResult.Tables(0).Rows(0).ItemArray(1).ToString()
                    End If

                    'lsResult = convertFromDataTableToListString(dsResult.Tables(0), dsResult.Tables(0).Columns(1).ColumnName.ToString()) 'name
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    Public Sub SaveNewProject(ByRef flagResult As Boolean)
        Dim exMessage As String = Nothing
        Dim status As String = Nothing
        flagResult = True
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                Dim projectId = objBL.getmax("QS36F.PRDVLH", "PRHCOD") + 1
                Dim user = DirectCast(Session("userid"), String)
                Dim partNo = txtPartNoPD.Text.Trim()
                Dim vendorNo = If(String.IsNullOrEmpty(txtNewVendorNo.Text), If(Not String.IsNullOrEmpty(txtCurrentVendor.Text), txtCurrentVendor.Text.Trim(), "000000"), If(objBL.isVendorAccepted(txtNewVendorNo.Text.Trim()), txtNewVendorNo.Text.Trim(), "000000"))

                Dim strQueryAdd As String = "WHERE PQVND = " & vendorNo & " AND PQPTN = '" & UCase(partNo) & "'"
                Dim spacepoqota1 As String = "                               DEV"
                Dim flag As Integer = 0
                If Not String.IsNullOrEmpty(txtPartNoPD.Text) And Not String.IsNullOrEmpty(vendorNo) Then
                    'error obteniendo datos con connection strin db2 -- aqui me quede, segui desde aqui
                    status = objBL.GetProjectStatusDescription("E")
                    Dim statusquote = "D-" & status

                    Dim ds = objBL.GetCodeAndNameByPartNo(txtPartNoPD.Text)

                    'test remove
                    ds = Nothing
                    'test remove

                    If ds IsNot Nothing Then
                        Dim mixProject = objBL.GetCodeAndNameByPartNoAndVendorNo(partNo, vendorNo)
                        flag = If(mixProject IsNot Nothing, 2, 1)
                        'mensaje en relacion a lo que tega flag, 1)o solo existe la parte o 2)existe la parte para el mismo vendor seleccionado
                    Else

                        If vendorNo <> "000000" Then

                            Dim personInCharge = UCase(ddlAssignedTo.SelectedItem.Text.Trim())

                            Dim rsHeaderInsertion = objBL.InsertNewProject(projectId, user, DateTime.Now, txtProjectDevDescription.Text, txtProjectNamePD.Text, "I", personInCharge)
                            If rsHeaderInsertion > 0 Then
                                'Dim ctpNo = If(Not String.IsNullOrEmpty(txtCTPNoPD.Text), txtCTPNoPD.Text.Trim(), objBL.GetCTPPartRef(txtPartNoPD.Text.Trim()))
                                Dim objCtp = objBL.GetCtpNumber(partNo, txtCTPNoPD.Text.Trim())
                                Dim rsDetailInsertion = objBL.InsertProductDetail(projectId, txtPartNoPD.Text.Trim(), DateTime.Now, user, DateTime.Now, user, DateTime.Now, objCtp.CtpNumber, txtQtySoldPD.Text.Trim(),
                                    "", objCtp.MfrNo, "0", "0", "", DateTime.Now, "E", "", txtCommentsPD.Text.Trim(), personInCharge, DateTime.Now, "0", "0", vendorNo, "", txtMinorCodePD.Text.Trim(), "0",
                                    DateTime.Now, DateTime.Now, "0")
                                'txtOEMPricePD.Text.Trim()
                                If rsDetailInsertion > 0 Then
                                    'insert en poqota

                                    Dim dsPoQota = objBL.GetPOQotaData(vendorNo, partNo)
                                    If dsPoQota IsNot Nothing Then
                                        If dsPoQota.Tables(0).Rows.Count > 0 Then
                                            Dim dsUpdatedData = objBL.UpdatePoQoraRow(objCtp.MfrNo, "0", "0", statusquote, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(),
                                                                                  DateTime.Now.Day.ToString(), vendorNo, partNo)
                                            'txtOEMPricePD.Text.Trim()
                                            If dsUpdatedData > 0 Then
                                                'updation ok
                                                Dim strMessage = "The insertion process was completed successfully."

                                                'ClearInputCustom(addProdDev)

                                                SendMessage(strMessage, messageType.success)
                                            Else
                                                'error updating poqota
                                                flagResult = False
                                                SendMessage("An error ocurred updating in poqota.", messageType.Error)
                                            End If
                                        Else
                                            Dim maxSeq = objBL.getmax("qs36f.POQOTA", "PQSEQ", strQueryAdd) + 1
                                            Dim poqotaInsertion = objBL.InsertNewPOQota(partNo, vendorNo, maxSeq, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), objCtp.CtpNumber,
                                                                                        DateTime.Now.Day.ToString(), statusquote, spacepoqota1, "0", "0")
                                            'txtOEMPricePD.Text.Trim()
                                            If poqotaInsertion > 0 Then
                                                Dim strMessage = "The insertion process was completed successfully."

                                                'ClearInputCustom(addProdDev)

                                                SendMessage(strMessage, messageType.success)
                                            Else
                                                'error inserting in poqota
                                                flagResult = False
                                                SendMessage("An error ocurred inserting in poqota.", messageType.Error)
                                            End If
                                        End If
                                    Else
                                        Dim maxSeq = objBL.getmax("qs36f.POQOTA", "PQSEQ", strQueryAdd) + 1
                                        Dim poqotaInsertion = objBL.InsertNewPOQota(partNo, vendorNo, maxSeq, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), objCtp.CtpNumber,
                                                                                    DateTime.Now.Day.ToString(), statusquote, spacepoqota1, "0", "0")
                                        'txtOEMPricePD.Text.Trim()
                                        If poqotaInsertion > 0 Then
                                            'insertion ok
                                            Dim strMessage = "The insertion process was completed successfully."

                                            'ClearInputCustom(addProdDev)

                                            SendMessage(strMessage, messageType.success)
                                        Else
                                            'error inserting in poqota
                                            flagResult = False
                                            SendMessage("An error ocurred inserting in poqota.", messageType.Error)
                                        End If
                                    End If
                                Else
                                    'rollback process
                                    Dim deletionAmount As Integer = 0
                                    Dim amountInHeader = objBL.GetReferencesInProject(projectId) ' check if the project have more than one reference
                                    If amountInHeader = 1 Then
                                        objBL.DeletePDHeader(projectId, deletionAmount) ' delete header if more than one reference
                                    End If

                                    'show error message no insertion en detail
                                    flagResult = False
                                    SendMessage("An error ocurred inserting data in Product Development Detail.", messageType.Error)
                                    'Exit Sub
                                End If

                            Else
                                'show error message no insertion en header
                                flagResult = False
                                SendMessage("An error ocurred inserting data in Product Development Header.", messageType.Error)
                                'Exit Sub
                            End If

                        Else
                            'vendor no aceptado
                            flagResult = False
                            SendMessage("Please select a right vendor for finish the insertion process.", messageType.Error)
                            'Exit Sub
                        End If

                    End If
                Else
                    'message part is a must
                    flagResult = False
                    SendMessage("Part and Vendor Numbers are required!", messageType.Error)
                    'Exit Sub
                End If

            End Using

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Sub SaveExistingProject(ByRef flagResult As Boolean)
        Dim exMessage As String = Nothing
        Dim status As String = Nothing
        flagResult = True
        Try

            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                Dim projectId = txtSearchValue.Text.Trim()
                Dim user = DirectCast(Session("userid"), String)
                Dim partNo = txtPartNoPD.Text.Trim()
                Dim vendorNo = If(Session("liSelected") IsNot Nothing, DirectCast(Session("liSelected"), String),
                                If(Not String.IsNullOrEmpty(txtNewVendorNo.Text.Trim()), txtNewVendorNo.Text.Trim(), txtCurrentVendor.Text.Trim()))

                Dim strQueryAdd As String = "WHERE PQVND = " & vendorNo & " AND PQPTN = '" & UCase(partNo) & "'"
                Dim spacepoqota1 As String = "                               DEV"
                Dim flag As Integer = 0

                If Not String.IsNullOrEmpty(txtPartNoPD.Text) And Not String.IsNullOrEmpty(vendorNo) Then
                    status = objBL.GetProjectStatusDescription("E")
                    Dim statusquote = "D-" & status

                    Dim ds = objBL.CheckIfreferenceExistsinProj(projectId, partNo, vendorNo)
                    'test remove
                    ds = Nothing
                    'test remove

                    If ds IsNot Nothing Then
                        'part exists in project for this vendor  ???
                    Else

                        If vendorNo <> "000000" Then

                            Dim personInCharge = UCase(ddlAssignedTo.SelectedItem.Text.Trim())

                            Dim objCtp = objBL.GetCtpNumber(partNo, txtCTPNoPD.Text.Trim())
                            Dim rsDetailInsertion = objBL.InsertProductDetail(projectId, partNo, DateTime.Now, user, DateTime.Now, user, DateTime.Now, objCtp.CtpNumber, txtQtySoldPD.Text.Trim(),
                                "", objCtp.MfrNo, "0", "0", "", DateTime.Now, "E", "", txtCommentsPD.Text.Trim(), personInCharge, DateTime.Now, "0", "0", vendorNo, "", txtMinorCodePD.Text.Trim(), "0",
                                DateTime.Now, DateTime.Now, "0")
                            'txtOEMPricePD.Text.Trim()
                            If rsDetailInsertion > 0 Then
                                Dim dsPoQota = objBL.GetPOQotaData(vendorNo, partNo)
                                If dsPoQota IsNot Nothing Then
                                    If dsPoQota.Tables(0).Rows.Count > 0 Then
                                        Dim dsUpdatedData = objBL.UpdatePoQoraRow(objCtp.MfrNo, "0", "0", statusquote, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(),
                                                                              DateTime.Now.Day.ToString(), vendorNo, partNo)
                                        'txtOEMPricePD.Text.Trim()
                                        If dsUpdatedData > 0 Then
                                            'updation ok
                                            Dim strMessage = "The insertion process was completed successfully."

                                            'ClearInputCustom(addProdDev)

                                            SendMessage(strMessage, messageType.success)
                                        Else
                                            'error updating poqota
                                            flagResult = False
                                            SendMessage("An error ocurred updating in poqota.", messageType.Error)
                                        End If
                                    Else
                                        Dim maxSeq = objBL.getmax("qs36f.POQOTA", "PQSEQ", strQueryAdd) + 1
                                        Dim poqotaInsertion = objBL.InsertNewPOQota(partNo, vendorNo, maxSeq, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), objCtp.CtpNumber,
                                                                                    DateTime.Now.Day.ToString(), statusquote, spacepoqota1, "0", "0")
                                        'txtOEMPricePD.Text.Trim()
                                        If poqotaInsertion > 0 Then
                                            Dim strMessage = "The insertion process was completed successfully."

                                            'ClearInputCustom(addProdDev)

                                            SendMessage(strMessage, messageType.success)
                                        Else
                                            'error inserting in poqota
                                            flagResult = False
                                            SendMessage("An error ocurred inserting in poqota.", messageType.Error)
                                        End If
                                    End If
                                Else
                                    Dim maxSeq = objBL.getmax("qs36f.POQOTA", "PQSEQ", strQueryAdd) + 1
                                    Dim poqotaInsertion = objBL.InsertNewPOQota(partNo, vendorNo, maxSeq, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), objCtp.CtpNumber,
                                                                                DateTime.Now.Day.ToString(), statusquote, spacepoqota1, "0", "0")
                                    'txtOEMPricePD.Text.Trim()
                                    If poqotaInsertion > 0 Then
                                        'insertion ok
                                        Dim strMessage = "The insertion process was completed successfully."

                                        'ClearInputCustom(addProdDev)

                                        SendMessage(strMessage, messageType.success)
                                    Else
                                        'error inserting in poqota
                                        flagResult = False
                                        SendMessage("An error ocurred inserting in poqota.", messageType.Error)
                                    End If
                                End If
                            Else
                                'rollback process
                                Dim deletionAmount As Integer = 0
                                Dim amountInHeader = objBL.GetReferencesInProject(projectId) ' check if the project have more than one reference
                                If amountInHeader = 1 Then
                                    objBL.DeletePDHeader(projectId, deletionAmount) ' delete header if more than one reference
                                End If

                                'show error message no insertion en detail
                                flagResult = False
                                SendMessage("An error ocurred inserting data in Product Development Detail.", messageType.Error)
                            End If

                        Else
                            'vendor no aceptado
                            flagResult = False
                            SendMessage("Please select a right vendor for finish the insertion process.", messageType.Error)
                        End If

                    End If
                Else
                    'message part is a must
                    flagResult = False
                    SendMessage("Part and Vendor Numbers are required!", messageType.Error)
                End If

            End Using

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Sub SaveProdDevProject()
        Dim exMessage As String = Nothing
        Dim status As String = Nothing
        Dim flagResult As Boolean = True

        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()

                'here begins to process in order of the selection insertion
                'here continue the save process
                If hdNewProj.Value = "1" Then
                    SaveNewProject(flagResult)
                Else
                    SaveExistingProject(flagResult)
                End If

                If flagResult = True Then

                    Dim rsUpdate = updateWishListGridView(txtPartNoPD.Text.Trim())
                    Dim rsUpdate1 = updateWishListBckGridView(txtPartNoPD.Text.Trim())

                    If rsUpdate >= 0 And rsUpdate1 >= 0 Then
                        hdNewProj.Value = "0"
                        hdExistProj.Value = "0"
                        ClearInputCustom(addProdDev)
                        'ClearInputCustom(dvExistingProject)
                        'ClearInputCustom(dvNewProject)
                    End If

                    loadData()

                Else
                    Exit Sub
                End If

                'if error in insertion rollback
                'update grid


            End Using

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, exMessage, "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub



#End Region

#Region "Autocomplete"

    <WebMethod()>
    Public Shared Function GetAutoCompleteDataPartNo(prefixText As String) As List(Of String)
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Dim lsResult As New List(Of String)
        'Dim resultInt As Integer
        Try
            Dim lstResult = New List(Of String)

            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.GetAutoCompleteDataPartNo(prefixText, dsResult)
                If rsData > 0 Then
                    lsResult = convertFromDataTableToListString(dsResult.Tables(0), dsResult.Tables(0).Columns(0).ColumnName.ToString())
                End If
            End Using

            Return lstResult

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try

    End Function

    <WebMethod()>
    Public Shared Function GetAutocompleteSelectedVendorName(prefixVendorName As String) As List(Of String)
        Dim exMessage As String = Nothing
        Dim dsResult As New DataSet
        Dim lsResult As New List(Of String)
        Dim dictionary As New Dictionary(Of String, String)

        Dim vendorOEMCodeDenied As String = ConfigurationManager.AppSettings("vendorOEMCodeDenied")
        Dim itemCategories As String = ConfigurationManager.AppSettings("itemCategories")
        Dim vendorCodesDenied As String = ConfigurationManager.AppSettings("vendorCodesDenied")
        'vendorCodesDenied As String, VendorOEMCodeDenied As String, ItemCategories As String
        'Dim resultInt As Integer
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                Dim rsData = objBL.GetAutocompleteSelectedVendorName(prefixVendorName, vendorCodesDenied, vendorOEMCodeDenied, itemCategories, dsResult)
                If rsData > 0 Then
                    lsResult = convertFromDataTableToListString(dsResult.Tables(0), dsResult.Tables(0).Columns(1).ColumnName.ToString()) 'name
                End If
            End Using

            Return lsResult

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try

    End Function

#End Region

#Region "Logs"

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "CTPSystem" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region

End Class