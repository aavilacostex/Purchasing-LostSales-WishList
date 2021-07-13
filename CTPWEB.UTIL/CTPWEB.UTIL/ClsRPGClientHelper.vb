Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Globalization
Imports IBM.Data.DB2.iSeries

Public Class ClsRPGClientHelper

    Dim conn As IBM.Data.DB2.iSeries.iDB2Connection = Nothing
    Dim connSql As SqlConnection = Nothing

    Private connString As String
    Public Property ConString() As String
        Get
            Return connString
        End Get
        Set(ByVal value As String)
            connString = value
        End Set
    End Property

    Private TotalRowCount As String
    Public Property GetRowCount() As String
        Get
            Return TotalRowCount
        End Get
        Set(ByVal value As String)
            TotalRowCount = value
        End Set
    End Property

    Private PageSize As String
    Public Property GetPageSize() As String
        Get
            Return PageSize
        End Get
        Set(ByVal value As String)
            PageSize = value
        End Set
    End Property

    Private _extraConString As String
    Public Property ConStringDB2() As String
        Get
            Return _extraConString
        End Get
        Set(ByVal value As String)
            _extraConString = value
        End Set
    End Property

    Private _extraConStringTest As String
    Public Property ConStringDB2Test() As String
        Get
            Return _extraConStringTest
        End Get
        Set(ByVal value As String)
            _extraConStringTest = value
        End Set
    End Property

    Private _strconnSQL As String
    Public Property StrConnSQL() As String
        Get
            Return _strconnSQL
        End Get
        Set(ByVal value As String)
            _strconnSQL = value
        End Set
    End Property

    Public Sub New()
        ConString = ConfigurationManager.AppSettings("ConnectionStringOdbc").ToString()
        ConStringDB2 = ConfigurationManager.AppSettings("ConnectionStringDB2").ToString() 'production
        ConStringDB2Test = ConfigurationManager.AppSettings("ConnectionStringDB2Test").ToString() ' test
        GetRowCount = ConfigurationManager.AppSettings("totalRowCount").ToString()
        GetPageSize = ConfigurationManager.AppSettings("pageSize").ToString()
        StrConnSQL = ConfigurationManager.AppSettings("strconnSQL").ToString()

        connSql = New SqlConnection(StrConnSQL)
        conn = New iDB2Connection(ConStringDB2Test)
    End Sub

    ' <summary>
    '  Executes a stored procedure that returns a result set.
    ' </summary>
    ' <param name="connString">The conn string.</param>
    ' <param name="sqlStatement">The SQL statement.</param>
    ' <param name="parameters">The parameters.</param>
    Public Function GetDataFromDatabase(sqlStatement As String, ByRef dsResult As DataSet, Optional dt As DataTable = Nothing, ByRef Optional messageOut As String = Nothing) As Integer
        Dim exMessage As String = " "
        conn = New iDB2Connection(ConStringDB2Test)
        conn.Open()

        Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
            dsResult = New DataSet()
            Dim result As Integer = -1
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text
                command.CommandTimeout = 0

                'Dim dataAdapter As iDB2DataAdapter = New iDB2DataAdapter(command)
                'result = dataAdapter.Fill(dsResult)
                Dim dtt As DataTable = New DataTable()
                dtt = dt.Clone()
                If dtt IsNot Nothing Then
                    dtt.Namespace = "LostSales"
                Else
                    dtt = New DataTable()
                End If

                dsResult.Tables.Add(dtt)
                dsResult.EnforceConstraints = False
                dsResult.Tables(0).BeginLoadData()

                Dim reader As iDB2DataReader = command.ExecuteReader()
                dsResult.Tables(0).Load(reader)
                result = dsResult.Tables(0).Rows.Count()

                dsResult.Tables(0).EndLoadData()
                dsResult.EnforceConstraints = True

                Return result
            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
                messageOut = exMessage
                dsResult = Nothing
                Throw ex
                Return result
            Finally
                conn.Close()
                conn.Dispose()
            End Try

        End Using

    End Function

    Public Function GetOdBcDataFromDatabase(sqlStatement As String, ByRef dsResult As DataSet, Optional dt As DataTable = Nothing) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1

        Using ObjConn As Odbc.OdbcConnection = New Odbc.OdbcConnection(ConString)
            Dim dataAdapter As New Odbc.OdbcDataAdapter()
            Dim ds As New DataSet()
            ds.Locale = CultureInfo.InvariantCulture

            Try
                ObjConn.Open()
                Dim cmd As New Odbc.OdbcCommand(sqlStatement, ObjConn)
                dataAdapter = New Odbc.OdbcDataAdapter(cmd)

                'Dim reader As Odbc.OdbcDataReader = cmd.ExecuteReader()
                'dsResult.Tables(0).Load(reader)
                'result = dsResult.Tables(0).Rows.Count()

                dt = New DataTable()
                'dt.Namespace = "LostSales"
                'dsResult.Tables.Add(dt)

                'dsResult.EnforceConstraints = False
                'dsResult.Tables("LostSales").BeginLoadData()

                result = dataAdapter.Fill(dt)

                dt.Namespace = "LostSales"
                dsResult.Tables.Add(dt)

                'dsResult.Tables("LostSales").EndLoadData()
                'dsResult.EnforceConstraints = True

                Return result
            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
                dsResult = Nothing
                Return result
            Finally
                ObjConn.Close()
                ObjConn.Dispose()
            End Try

        End Using

    End Function

    Public Function InsertOdBcDataToDatabase(sqlStatement As String) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1

        Try
            Using ObjConn As Odbc.OdbcConnection = New Odbc.OdbcConnection(ConString)
                'Dim dataAdapter As New Odbc.OdbcDataAdapter()
                'Dim ds As New DataSet()
                'ds.Locale = CultureInfo.InvariantCulture

                ObjConn.Open()
                Dim cmd As New Odbc.OdbcCommand(sqlStatement, ObjConn)
                Dim affectedRows = cmd.ExecuteNonQuery()

                'dataAdapter = New Odbc.OdbcDataAdapter(cmd)
                'result = dataAdapter.Fill(dsResult)

                Return affectedRows
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function UpdateOdBcDataToDatabase(sqlStatement As String) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Try
            Using ObjConn As Odbc.OdbcConnection = New Odbc.OdbcConnection(ConString)
                ObjConn.Open()
                Dim cmd As New Odbc.OdbcCommand(sqlStatement, ObjConn)
                Dim affectedRows = cmd.ExecuteNonQuery()

                Return affectedRows
            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetSingleDataScalar(sqlStatement As String) As String
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim str As String = Nothing

        Try
            conn = New iDB2Connection(ConStringDB2Test)
            conn.Open()

            Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                str = command.ExecuteScalar()
                Return str
                'If ds IsNot Nothing Then
                '    Return ds
                'Else
                '    Return Nothing
                'End If
            End Using

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Function GetSingleDataFromDatabase1(sqlStatement As String, columnToChange As String, ByRef strResult As String) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture

        conn = New iDB2Connection(ConStringDB2Test)
        conn.Open()

        Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
            Dim dsDataValues = New DataSet()
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                Dim dataAdapter As iDB2DataAdapter = New iDB2DataAdapter(command)
                result = dataAdapter.Fill(dsDataValues)

                Dim index As Integer = dsDataValues.Tables(0).Columns(columnToChange).Ordinal
                If (dsDataValues.Tables(0).Rows.Count > 0) Then
                    For Each tt As DataRow In dsDataValues.Tables(0).Rows
                        strResult = dsDataValues.Tables(0).Rows(0).ItemArray(index).ToString()
                        Exit For
                    Next
                    Return result
                Else
                    strResult = ""
                    Return result
                End If

            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
                strResult = ""
                Return result
            End Try
        End Using
    End Function

    Public Function GetSingleDataFromDatabase(sqlStatement As String, columnToChange As String, ByRef strResult As String) As Integer
        Dim exMessage As String = " "
        conn = New iDB2Connection(ConString)
        conn.Open()

        Dim DescriptionCode As String = ""
        Dim result As Integer = -1
        strResult = " "

        Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
            Dim dsDataValues = New DataSet()
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                Dim dataAdapter As iDB2DataAdapter = New iDB2DataAdapter(command)
                result = dataAdapter.Fill(dsDataValues)

                Dim index As Integer = dsDataValues.Tables(0).Columns(columnToChange).Ordinal
                If (dsDataValues.Tables(0).Rows.Count > 0) Then
                    For Each tt As DataRow In dsDataValues.Tables(0).Rows
                        strResult = dsDataValues.Tables(0).Rows(0).ItemArray(index).ToString()
                        Exit For
                    Next
                    Return result
                Else
                    strResult = ""
                    Return result
                End If

            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
                strResult = ""
                Return result
            End Try
        End Using

    End Function

    Public Sub UpdateDataInDatabase(sqlStatement As String, ByRef affectedRows As Integer)
        Dim exMessage As String = " "
        affectedRows = -1
        conn = New iDB2Connection(ConStringDB2Test)
        conn.Open()

        Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                affectedRows = command.ExecuteNonQuery()
            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            End Try
        End Using
    End Sub

    Public Sub InsertDataInDatabase(sqlStatement As String, ByRef affectedRows As Integer)
        Dim exMessage As String = " "
        affectedRows = -1
        conn = New iDB2Connection(ConStringDB2Test)
        conn.Open()

        Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                affectedRows = command.ExecuteNonQuery()
            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            End Try
        End Using
    End Sub

    Public Sub InsertDataInDatabaseSQL(sqlStatement As String, ByRef affectedRows As Integer)
        Dim exMessage As String = " "
        affectedRows = -1
        Using cmd As iDB2Command = New iDB2Command(sqlStatement, connSql)
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                affectedRows = command.ExecuteNonQuery()
            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            End Try
        End Using
    End Sub

    Public Sub DeleteRecordFromDatabase(sqlStatement As String, ByRef affectedRows As Integer)
        Dim exMessage As String = " "
        affectedRows = -1
        Using cmd As iDB2Command = New iDB2Command(sqlStatement, conn)
            Try
                Dim command As iDB2Command = conn.CreateCommand()
                command.CommandText = sqlStatement
                command.CommandType = System.Data.CommandType.Text

                affectedRows = command.ExecuteNonQuery()
            Catch ex As Exception
                exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            End Try
        End Using
    End Sub


#Region "SQL Store Procedures"

    ' <summary>
    ' Ejecuta un Stored Procedure que retorna tuplas como resultado de su ejecución.
    ' </summary>
    ' <param name="strSpName">Nombre del Stored Procedure</param>
    ' <param name="parameters">Arreglo de Parámetros que recive el Stored Procedure, en el caso que no reciba parámetros el Stored Procedure se envía null.</param>
    ' <returns>Las tuplas de resultado de la ejecución del Stored Procedure.</returns>

    Public Sub ExecuteNotQueryCommand1(queryString As String, ByRef affectedRows As Integer)

        Dim exMessage As String = " "
        affectedRows = -1
        Try
            Using connection As New SqlConnection(StrConnSQL)
                Dim command As New SqlCommand(queryString, connection)
                command.CommandType = CommandType.Text

                command.Connection.Open()
                affectedRows = command.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString

        Finally

        End Try

    End Sub

    Public Function ExecuteNotQueryCommand(queryString As String, connectionString As String) As Integer

        Dim exMessage As String = " "
        Dim rsResult As Integer = -1
        Try
            Using connection As New SqlConnection(connectionString)
                Dim command As New SqlCommand(queryString, connection)
                command.CommandType = CommandType.Text

                command.Connection.Open()
                rsResult = command.ExecuteNonQuery()
            End Using
            Return rsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return rsResult
        Finally

        End Try

    End Function

    Public Function ExecuteQueryStoredProcedure(strSpName As String, parameters As SqlParameter()) As DataTable
        Dim exMessage As String = " "
        Try

            Dim dtQuery As DataTable = New DataTable()

            Using sqlConn = New SqlConnection(StrConnSQL)

                sqlConn.Open()
                Dim sqlCom As SqlCommand = sqlConn.CreateCommand()
                sqlCom.CommandText = strSpName
                sqlCom.CommandType = CommandType.Text
                If parameters IsNot Nothing Then
                    For Each item In parameters
                        sqlCom.Parameters.Add(item)
                    Next
                End If

                sqlCom.Prepare()
                sqlCom.CommandTimeout = 1000
                Dim data As SqlDataAdapter = New SqlDataAdapter(sqlCom)
                data.Fill(dtQuery)
                sqlCom.Parameters.Clear()
                sqlCom.Dispose()
                sqlConn.Close()
                Return dtQuery

            End Using
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            connSql.Close()
            Return Nothing
            'log.Error(ex.Message, ex);
            'Throw ex;
        End Try

    End Function

#End Region


End Class
