Imports System.Web
Imports System.Web.Services

Public Class KeepAlive
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try

            Dim user = context.Session("userid")
            If user Is Nothing Then
                context.Response.ContentType = "text/plain"
                context.Response.Write("Your user session expired. In order to refresh your session please select Accept.")
            Else

                Dim defaultTimeOut = context.Session("TimeOutDefined")
                Dim beforeWarningTimeOut = CInt(context.Session("BeforeExpireUserSessionMS"))

                Dim initTime = context.Session("UserInitTime") '5/26/2021 1:28:57 PM
                Dim curTime = context.Request("p1") '5/26/2021, 1:29:16 PM
                Dim dtInitTime As DateTime = New DateTime()
                Dim dtCurTime As DateTime = New DateTime()

                Dim blInitTime = DateTime.TryParse(initTime, dtInitTime)
                Dim blCurTime = DateTime.TryParse(curTime, dtCurTime)
                If blInitTime And blCurTime Then
                    Dim elapsedTime = (CInt(dtCurTime.Subtract(dtInitTime).TotalSeconds)) * 1000 'elapsed time in the current session ms
                    'Dim flagValue = ((CInt(defaultTimeOut) * 60 * 1000) - CInt(beforeWarningTimeOut)) / 1000 'time to notify almost done the session

                    If CInt(beforeWarningTimeOut) <= elapsedTime Then

                        Dim leaveTime = CInt(((CInt(defaultTimeOut) - elapsedTime) / 1000) / 60).ToString()
                        Dim strValue = String.Format("Your session will expire in {0} minutes. If you want to refresh your session select Accept otherwise if you want to save your changes select Cancel.", leaveTime)

                        context.Response.ContentType = "text/plain"
                        context.Response.Write(strValue)
                    Else
                        context.Response.ContentType = "text/plain"
                        context.Response.Write("Your user session expired. In order to refresh your session please select Accept.")
                    End If
                Else
                    context.Response.ContentType = "text/plain"
                    context.Response.Write("An error ocurred. Please refresh the page.")
                End If

                'Dim msg = String.Format("<script languaje='javascript'>alert('this is a test')</script>")
                'context.Response.Write(msg)
            End If

            'context.Response.ContentType = "text/json"

            'Dim usr = System.Web.Security.Membership.GetUser()
            'If usr IsNot Nothing Then
            '    context.Response.Write("Hello World")
            'End If
        Catch ex As Exception
            Dim msg = ex.Message
            Dim pp = msg
        End Try

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class