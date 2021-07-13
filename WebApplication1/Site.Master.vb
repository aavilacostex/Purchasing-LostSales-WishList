Imports System.DirectoryServices.AccountManagement
Imports System.Reflection
Imports System.Security.Principal
Imports CTPWEB.DTO

Public Class SiteMaster
    Inherits MasterPage

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing
    Private Shared eventLog1 As EventLog = New EventLog("Purchasing.Log", GetComputerName(), "Purchasing.App")
    Private Shared ReadOnly Log As log4net.ILog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)

    Dim objLog = New Logs()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

#Region "not usable now"

        'Using objBL As CTPWEB.BL.CTP_SYSTEM = New CTPWEB.BL.CTP_SYSTEM()
        'Using resource As CImpersonation = New CImpersonation()
        '    Dim user As UserPrincipal = Nothing
        '    Dim token As IntPtr = WindowsIdentity.GetCurrent().Token

        '    resource.Impersonate(token)
        '    Dim loggedUser = WindowsIdentity.GetCurrent().Name
        '    Dim domainName As String = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName.ToString()
        '    Dim strUser = UserPrincipal.FindByIdentity(New PrincipalContext(ContextType.Domain, domainName),
        '                                     IdentityType.SamAccountName, loggedUser)
        '    If strUser IsNot Nothing Then
        '        Session("userid") = strUser.Name
        '        lblUsername.Text = "User Logged: " + strUser.Name
        '    Else
        '        lblUsername.Text = "User Logged: " + loggedUser
        '    End If


        'End Using

        'Dim windowsUser = WindowsIdentity.GetCurrent().Name

#End Region
        Dim methodMessage As String = Nothing
        Try
            If Not IsPostBack() Then
                Log.Info("Starting Process")
                Dim windowsUser = System.Web.HttpContext.Current.User.Identity.Name
                If windowsUser IsNot Nothing Then
                    If String.IsNullOrEmpty(windowsUser.Trim()) Then
                        windowsUser = WindowsIdentity.GetCurrent().Name
                    End If
                End If

                Log.Info("Getting Domain Data")
                Dim domainName As String = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName.ToString()

                Log.Info("Getting User Principal Data")
                Dim strUser = UserPrincipal.FindByIdentity(New PrincipalContext(ContextType.Domain, domainName),
                                                     IdentityType.SamAccountName, windowsUser)

                Session("UserPrincipal") = strUser

                Log.Info("Preparing Only User")
                Dim onlyUser As String = Nothing
                If strUser IsNot Nothing Then
                    Log.Info("User Is Not Nothing")
                    If windowsUser.Contains("\") Then
                        onlyUser = windowsUser.Split("\")(1)
                    Else
                        onlyUser = windowsUser
                    End If

                    Session("username") = strUser.Name
                    Session("userid") = strUser.SamAccountName
                    lblUsername.Text = "User: " + onlyUser + " - " + strUser.Name

                Else
                    Log.Info("User Is Nothing")
                    onlyUser = windowsUser
                    Session("userid") = onlyUser
                    lblUsername.Text = "User Logged: " + onlyUser

                End If

                writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In PurchasingApp: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
            End If

        Catch ex As Exception
            Log.Error(strLogCadenaCabecera + ".." + ex.Message)
            writeComputerEventLog(ex.Message)
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message + ". " + ex.ToString(), "At time: " + DateTime.Now.ToString())
        End Try

        'If Request.RawUrl.ToLower().Contains("wish-list") Then
        '    Attributes.Add("onload", "RefreshContent()")
        'End If
    End Sub

    Public Shared Function GetComputerName() As String
        Dim exMessage As String = Nothing
        Try
            Dim ComputerName As String
            ComputerName = Environment.MachineName
            Return ComputerName
        Catch ex As Exception
            Log.Error(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

#Region "Logs"

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

        Catch ex As Exception
            Log.Error("Error trying to put info un console log: " + ex.Message + ".")
        End Try
    End Sub

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "PurchasingApp" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region

End Class