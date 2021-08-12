Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports CTPWEB.DTO

Public Class DownloadWLTemplate
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing
    Dim objLog = New Logs()
    Dim userid As String = Nothing

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim exMessage As String = Nothing
        Try

            Dim path = context.Session("filePathExcel")
            userid = context.Session("userid")
            Dim myFile As FileInfo = New FileInfo(path)
            Dim response As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
            response.ClearContent()
            response.Clear()
            response.ContentType = "application/octet-stream"
            response.AddHeader("Content-Disposition", "attachment; filename=" + myFile.Name + ";")
            response.AddHeader("Content-Length", myFile.Length.ToString())
            'response.TransmitFile(Server.MapPath("FileDownload.csv"))
            response.TransmitFile(path)
            'response.WriteFile(path)
            response.Flush()

            File.Delete(path)
            response.End()

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List Program: " + userid, "Login at time: " + DateTime.Now.ToString())
        End Try

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim user = If(userid IsNot Nothing, userid, "N/A")
        objLog.WriteLog(strLevel, "CTPSystem" & strLevel, strLogCadena, user, strMessage, strDetails)
    End Sub

End Class