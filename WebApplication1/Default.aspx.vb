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

#Region "Logs"

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "CustomerPaymentsApp" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region
End Class