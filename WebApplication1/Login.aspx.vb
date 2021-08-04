Imports System.DirectoryServices
Imports System.DirectoryServices.AccountManagement

Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("userid") IsNot Nothing Then
            Response.Redirect("Default.aspx", False)
        End If
    End Sub

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Dim methodMessage As String = Nothing
        Dim sr As SearchResult = Nothing
        Dim dct As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim strLdap As String = Nothing
        Try
            Dim user = UserName.Text.Trim()
            Dim pass = Password.Text.Trim()

            Dim blFlag = getLDAPConnectionString("", strLdap, False, True)
            If blFlag Then

                Using pc As PrincipalContext = New PrincipalContext(ContextType.Domain, strLdap, user, ContextOptions.SimpleBind.ToString())
                    Dim validCred = pc.ValidateCredentials(user, pass)
                    If validCred Then
                        Dim strUser = UserPrincipal.FindByIdentity(New PrincipalContext(ContextType.Domain, strLdap), IdentityType.SamAccountName, user)
                        'BuildUserSearcherFromPrincipal(strUser)

                        If strUser IsNot Nothing Then
                            Session("UserPrincipal") = strUser
                            Session("username") = strUser.Name
                            Session("userid") = strUser.SamAccountName.ToUpper()

                            FormsAuthentication.RedirectFromLoginPage(UserName.Text, False)
                        Else
                            lblOptionalMessage.Text = "There is an error getting data for the user: " + user
                        End If
                    Else
                        lblOptionalMessage.Text = "The user name or password is incorrect."
                    End If
                End Using

            Else
                lblOptionalMessage.Text = "There is an error getting data for Company Domain: Costex.com"
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Function getLDAPConnectionString(defSearch As String, ByRef strLdap As String, Optional flag As Boolean = False, Optional method As Boolean = False) As Boolean
        Dim blResult As Boolean = False
        strLdap = Nothing
        'method = false(Directory Services), true(User Principal)
        Try
            Dim de As DirectoryEntry = New DirectoryEntry("LDAP://RootDSE")
            If de IsNot Nothing Then

                If Not method Then
                    If flag Then
                        strLdap = "LDAP://" + de.Properties("defaultNamingContext")(0).ToString()
                    Else
                        strLdap = de.Properties("defaultNamingContext")(0).ToString()
                    End If
                Else
                    strLdap = de.Properties("ldapServiceName")(0).ToString().Split("@")(1).ToString().Trim()
                End If

                'strLdap = If(flag, strLdap = "LDAP://" + de.Properties("defaultNamingContext")(0).ToString(), strLdap = de.Properties("defaultNamingContext")(0).ToString())
                blResult = If(String.IsNullOrEmpty(strLdap), False, True)
            End If

            Return blResult
        Catch ex As Exception
            Dim msg = ex.Message
            Return blResult
        End Try
    End Function

End Class