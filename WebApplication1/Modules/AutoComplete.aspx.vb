Imports System
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.Odbc
Imports System.Data.Linq.Mapping


Partial Public Class AutoComplete
    Inherits System.Web.UI.Page
    Protected Sub Page_Load()

    End Sub


    'Using ObjConn As Odbc.OdbcConnection = New Odbc.OdbcConnection(ConString)
    '       Dim dataAdapter As New Odbc.OdbcDataAdapter()
    '       Dim ds As New DataSet()
    '       ds.Locale = CultureInfo.InvariantCulture

    '       ObjConn.Open()
    '       Dim cmd As New Odbc.OdbcCommand(sqlStatement, ObjConn)
    '       dataAdapter = New Odbc.OdbcDataAdapter(cmd)
    '       result = dataAdapter.Fill(dsResult)
    '       Return result
    '   End Using


    <WebMethod()>
    Public Shared Function GetAutoCompleteData(prefixText As String) As List(Of String)
        Dim exMessage As String = " "
        'Dim resultInt As Integer
        Try
            Dim result = New List(Of String)
            'ConString = "DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;"
            'Using con As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")

            Dim con As OdbcConnection = New OdbcConnection("DSN=COSTEX400;UID=INTRANET;PWD=CTP6100;")
            con.Open()

            Dim DbCommand As OdbcCommand = con.CreateCommand()
            Dim Sql = " SELECT (CSMREH.MHMRNR) ClAIM# FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CSMREH.MHMRNR) LIKE '%{2}%' ORDER BY CSMREH.MHMRNR DESC"

            Dim sqlResult = String.Format(Sql, Today().AddYears(-2).ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), prefixText)
            DbCommand.CommandText = sqlResult
            Dim DbReader As OdbcDataReader = DbCommand.ExecuteReader()

            Using sdr As OdbcDataReader = DbReader
                While sdr.Read()
                    result.Add(sdr("Claim#").ToString())
                End While
            End Using

            DbReader.Close()
            DbCommand.Dispose()
            con.Close()

            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

End Class

