Imports CTPWEB.DTO

Public Class _Default
    Inherits Page

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    Dim objLog = New Logs()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim url As String = Nothing
        Try
            If Session("userid") Is Nothing Then
                url = String.Format("Login.aspx?data={0}", "Session Expired!")
                Response.Redirect(url, False)
            Else
                Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                hdWelcomeMess.Value = lblUserLogged.Text

                Dim validUsers = ConfigurationManager.AppSettings("validUsersForWeb")
                Dim user = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "NA")
                Dim fullData = If(LCase(validUsers.Trim()).Contains(LCase(user.Trim())), True, False)
                Dim dpto = ConfigurationManager.AppSettings("Department")

                If Not fullData Then
                    Dim ds As DataSet = New DataSet()
                    Dim userIn = GetValidUsers(dpto, ds)
                    If userIn Then
                        For Each dw As DataRow In ds.Tables(0).Rows
                            If dw.Item("USUSER").ToString().Trim().ToUpper() = user.Trim().ToUpper() Then
                                'Or user = "AAVILA"
                                Response.Redirect("Wish-List.aspx", False)
                            End If
                        Next
                    End If

                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

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

    Private Function GetValidUsers(dpto As String, ByRef ds As DataSet) As Boolean
        Dim flIn As Boolean = False
        Dim result As Integer = -1
        Try
            Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
                result = objBL.GetValidUsers(dpto, ds)
                If result > 0 Then
                    If ds IsNot Nothing Then
                        If ds.Tables(0).Rows.Count > 0 Then
                            flIn = True
                        End If
                    End If
                End If
            End Using
            Return flIn
        Catch ex As Exception
            Return flIn
        End Try
    End Function

#Region "Logs"

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "CustomerPaymentsApp" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region
End Class