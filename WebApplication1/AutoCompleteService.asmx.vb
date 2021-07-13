Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.Configuration

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class AutoCompleteService
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function GetCompletionList(ByVal prefixText As String,
        ByVal count As Integer) As List(Of String)

        Dim exMessage As String = " "
        Dim claimNumbers As List(Of String) = New List(Of String)
        Try

            'getClaimNumbersSW("130644", Today.AddYears(-2))

            Using con As SqlConnection = New SqlConnection()

                '[con.ConnectionString = WebConfigurationManager.AppSettings.
                con.ConnectionString = WebConfigurationManager.ConnectionStrings("ConnectionStringOdbc").ConnectionString
                'con.ConnectionString = ConfigurationManager.ConnectionStrings("ConnectionStringOdbc").ConnectionString
                Using com As SqlCommand = New SqlCommand()

                    Dim Sql = " SELECT (CSMREH.MHMRNR) ClAIM# FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CSMREH.MHMRNR) LIKE '%{2}%' ORDER BY CSMREH.MHMRNR DESC"

                    Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)

                    'com.CommandText = "select CountryName from Countries where " + "CountryName like @Search + '%'"
                    com.CommandText = sqlResult
                    'com.Parameters.AddWithValue("@Search", prefixText)
                    com.Connection = con
                    con.Open()

                    Dim slqBaseReader = com.ExecuteReader()
                    Using sdr As SqlDataReader = slqBaseReader
                        While sdr.Read()


                            'listaUser.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(dataRow["cod_usuario"].ToString(), "1"));
                            'claimNumbers.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(sdr("Claim#").ToString(), 1))
                            claimNumbers.Add(sdr("Claim#").ToString())
                        End While
                    End Using
                    con.Close()
                    'Return claimNumbers.ToArray()
                    Return claimNumbers
                End Using
            End Using
            'Dim dtResult As DataTable = New DataTable()
            'Dim query As String = "select nvName from Friend where nvName like '" + prefixText + "%'"
            'da = New SqlDataAdapter(query, con)
            'dt = New DataTable()
            'da.Fill(dt)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return claimNumbers
        End Try

    End Function

End Class