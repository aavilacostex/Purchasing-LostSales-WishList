Public Class PDLogReport
    Inherits System.Web.UI.Page

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            GetLogsReport()

            fill_Users(ddlUser)
            fill_Types(ddlType)
        Else
            Dim ds = DirectCast(Session("LogsData"), DataSet)

            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    grvLogs.DataSource = ds
                    grvLogs.DataBind()
                End If
            End If
        End If

    End Sub

#End Region

#Region "DropDownList"

#Region "DropDownList-Load"

    Protected Sub fill_Users(dwlControl As DropDownList)
        Dim ds As DataSet = New DataSet()
        Dim exMessage As String = Nothing
        Try

            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                ds = objBL.GetAllPaAndPsUsers()
            End Using

            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    LoadingDropDownList(dwlControl, ds.Tables(0).Columns("USER").ColumnName,
                                        ds.Tables(0).Columns("PA").ColumnName, ds.Tables(0), True, " ")
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub fill_Types(dwlControl As DropDownList)

        Dim ListItem As ListItem = New ListItem()
        dwlControl.Items.Add(New WebControls.ListItem("Start", "1"))
        dwlControl.Items.Add(New WebControls.ListItem("Stop", "2"))
        dwlControl.Items.Add(New WebControls.ListItem("Information", "3"))
        dwlControl.Items.Add(New WebControls.ListItem("Error", "4"))
        dwlControl.Items.Add(New WebControls.ListItem("Trace", "5"))
        dwlControl.Items.Add(New WebControls.ListItem("Warning", "6"))
        dwlControl.Items.Add(New WebControls.ListItem("Exception", "7"))

        '[Information]
        '[Error]()
        '[Trace]
        '[Warning]
        '[Exception]

    End Sub

#End Region

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim dsNew As DataSet = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim ds = DirectCast(Session("LogsDataBck"), DataSet)
            If ddlType.SelectedIndex = 0 Then
                grvLogs.DataSource = ds
                grvLogs.DataBind()
            ElseIf ddlType.SelectedIndex > 0 Then
                Dim ddlValue = LCase(ddlType.SelectedItem.Text)
                Dim myitem = ds.Tables(0).AsEnumerable().Where(Function(item) LCase(item.Item("LOGLEVEL").ToString()) = ddlValue).AsEnumerable().ToList()
                dsNew = ListToDataTableDr(myitem)
                Session("LogsData") = dsNew
                grvLogs.DataSource = dsNew
                grvLogs.DataBind()
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub ddlUser_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim dsNew As DataSet = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim ds = DirectCast(Session("LogsDataBck"), DataSet)
            If ddlUser.SelectedIndex = 0 Then
                grvLogs.DataSource = ds
                grvLogs.DataBind()
            ElseIf ddlUser.SelectedIndex > 0 Then
                Dim ddlValue = LCase(ddlUser.SelectedItem.Text)
                Dim myitem = ds.Tables(0).AsEnumerable().Where(Function(item) LCase(item.Item("LOGUSER").ToString()) = ddlValue).AsEnumerable().ToList()
                dsNew = ListToDataTableDr(myitem)
                Session("LogsData") = dsNew
                grvLogs.DataSource = dsNew
                grvLogs.DataBind()
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

#End Region

#Region "Utils"

    Protected Sub GetLogsReport(Optional ByVal dsSessionResult As DataSet = Nothing)
        Dim dsResult = New DataSet()
        Dim dsResult1 = New DataSet()
        Dim exMessage As String = " "

        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                If dsSessionResult IsNot Nothing Then
                    If dsSessionResult.Tables(0).Rows.Count > 0 Then
                        grvLogs.DataSource = dsSessionResult.Tables(0)
                        grvLogs.DataBind()
                    End If
                Else
                    Dim result As Integer = objBL.GetPDLogsFromSql(dsResult)
                    Session("LogsDataBck") = dsResult
                    Session("LogsData") = dsResult

                    If (result > 0 And dsResult IsNot Nothing) Then

                        If dsResult.Tables(0).Rows.Count > 0 Then

                            Session("PageAmounts") = 10
                            Session("currentPage") = 1
                            Session("ItemCounts") = dsResult.Tables(0).Rows.Count

                            grvLogs.DataSource = dsResult.Tables(0)
                            grvLogs.DataBind()
                        End If

                    End If
                End If
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

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
            Return Nothing
        End Try

    End Function

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

#End Region

#Region "Radios, Textboxes"

    Public Sub rdType_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Public Sub rdUser_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

#End Region

#Region "GridView"

    Protected Sub grvLogs_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grvLogs.Sorting
        Dim exMessage As String = Nothing
        Try

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvLogs_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Dim exMessage As String = Nothing

        Try

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try

    End Sub

    Protected Sub grvLogs_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grvLogs.PageIndexChanging
        Dim exMessage As String = " "
        Try
            grvLogs.PageIndex = e.NewPageIndex

            Session("currentPage") = (CInt(e.NewPageIndex + 1) * 10) - 9
            Session("PageAmounts") = If((CInt(e.NewPageIndex + 1) * 10) > DirectCast(Session("ItemCounts"), Integer), DirectCast(Session("ItemCounts"), Integer), (CInt(e.NewPageIndex + 1) * 10))

            Dim ds = DirectCast(Session("LogsData"), DataSet)

            If ds IsNot Nothing Then
                grvLogs.DataSource = ds.Tables(0)
            Else
                Dim grid = DirectCast(sender, GridView)
                Dim dtGrid = TryCast(grid.DataSource, DataTable)
                grvLogs.DataSource = dtGrid
            End If
            grvLogs.DataBind()
            'GetLostSalesData("", 1)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Protected Sub grvLogs_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grvLogs.RowCommand
        Dim exMessage As String = Nothing
        Try
            If e.CommandName = "show" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)
                Dim id = row.Cells(0).Text

                Dim ds1 = DirectCast(Session("LogsData"), DataSet)

                Dim myitem = ds1.Tables(0).AsEnumerable().Where(Function(item) item.Item("LOGID").ToString().Equals(id, StringComparison.InvariantCultureIgnoreCase))
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
        End Try
    End Sub

    Protected Sub grvLogs_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grvLogs.RowDataBound
        Try
            If (e.Row.RowType = DataControlRowType.Pager) Then
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

        End Try
    End Sub

#End Region

End Class