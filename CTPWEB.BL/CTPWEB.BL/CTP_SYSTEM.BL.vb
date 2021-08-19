Imports System.Configuration
Imports System.Globalization
Imports System.Reflection
Imports CTPWEB.DTO
Imports Microsoft.Win32

Public Class CTP_SYSTEM : Implements IDisposable
    Private disposedValue As Boolean

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    'Shared ReadOnly objLog = New Logs()


#Region "Lost Sales"

    Public Function GetLostSalesData(strwhere As String, flag As Integer, ByRef dsResult As DataSet, Optional optVendors As String = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim pageindex As String = Nothing
        Dim pagesize As String = Nothing
        Try

#Region "Old Query"

            '            Dim Sql As String = "with z as (SELECT                                                   
            'WRKPTN,qt,t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 TQ from (SELECT
            ' WRKPTN,(WRK001+ WRK002+ WRK003+ WRK004+ WRK005+                    
            'WRK006+WRK007+WRK008+ WRK009+ WRK010+ WRK011+ WRK012+ WRK013)       
            'QT,CASE WRK001 WHEN 0 THEN 0 ELSE 1 END T1,CASE WRK002 WHEN 0 THEN 0
            ' ELSE 1 END T2,CASE WRK003 WHEN 0 THEN 0 ELSE 1 END T3,CASE WRK004  
            'WHEN 0 THEN 0 ELSE 1 END T4,CASE WRK005 WHEN 0 THEN 0 ELSE 1 END T5,
            ' CASE WRK006 WHEN 0 THEN 0 ELSE 1 END T6,CASE WRK007 WHEN 0 THEN 0  
            ' ELSE 1 END T7,CASE WRK008 WHEN 0 THEN 0 ELSE 1 END T8,CASE WRK009  
            'WHEN 0 THEN 0 ELSE 1 END T9,CASE WRK010 WHEN 0 THEN 0 ELSE 1 END    
            'T10,CASE WRK011 WHEN 0 THEN 0 ELSE 1 END T11,CASE WRK012 WHEN 0 THEN
            ' 0 ELSE 1 END T12,CASE WRK013 WHEN 0 THEN 0 ELSE 1 END T13 FROM     
            'ddtwrk) a where t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 > 5)     
            'select imptn, imdsc, imds2, imds3, (select zoned(sum(odrsq),10,0)   
            'from horddt where odrcd = 1 and                                     
            ' SUBSTR(DIGITS(ODDATE),5 ,2) || SUBSTR(DIGITS(ODDATE),1 ,2) ||      
            'SUBSTR(DIGITS(ODDATE),3 ,2) >= '191206' and odptn = imptn and odcu# 
            'not in (4384,4385) and odlcn in ('01', '04', '05', '07', '02')      
            'group by odptn) qtysold, x.onhand, x.onorder,                       
            'coalesce(x.vendor, '') vendor, impc2,                                
            '(INMSTA.IMQTE+INMSTA.IMQT01+INMSTA.IMQT02+INMSTA.IMQT03+            
            'INMSTA.IMQT04+                                                      
            'INMSTA.IMQT05+INMSTA.IMQT06+INMSTA.IMQT07+INMSTA.IMQT08+            
            'inmsta.imqT09+INMSTA.IMQT10+INMSTA.IMQT11+INMSTA.IMQT12) TQUOTE,    
            'imprc,invptyf.iptqte Timesq, coalesce((select 'X' from dvinva       
            'where dvlocn='20' and dvpart=imptn and dvonh#>0),' ') F20,          
            '(case when x.vendor = '261339' or                                   
            ' x.vendor='060106' or x.vendor='262369' or                          
            'x.vendor = '262673' or x.vendor='261903' or x.vendor='150064' then  
            ''X' else '' end) Foem,zoned(coalesce((select count(distinct qdcuno) 
            'from qtedtld where qdptno=imptn and qdyear||qdmth||qdday >=         
            ''191206'),0),5,0) Ncus, impc1, imcata, (select mindes from mincodes 
            'where mincod = inmsta.impc2) mindsc, '' vendorname, '' pagent, '' wlist,
            ''' project, '' projstatus
            'from inmsta left join (select   
            'dvpart, sum(dvonh#) onhand, sum(dvono#) onorder, max(dvprmg) vendor 
            'from dvinva where dvlocn in ('01', '05', '07') and                  
            ' trim(dvprmg) <> '' and dvonh# <= 0 and                             
            '         dvono# <= 0 group by dvpart) x on inmsta.imptn = x.dvpart  
            'inner join invptyf on inmsta.imptn = invptyf.ippart where           
            'substr(ucase(trim(imdsc)),1,3) <> 'USE' and impc1 = '01' and        
            '(INMSTA.IMQTE+INMSTA.IMQT01+INMSTA.IMQT02+INMSTA.IMQT03+            
            'INMSTA.IMQT04+                                                      
            '  INMSTA.IMQT05+INMSTA.IMQT06+INMSTA.IMQT07+INMSTA.IMQT08+          
            'INMSTA.IMQt09+                                                      
            'INMSTA.IMQT10+INMSTA.IMQT11+INMSTA.IMQT12) > 0 
            'union select z.wrkptn imptn, coalesce(catdsc,coalesce(kodesc,'N/A'))
            'imdsc, coalesce(imds2, 'N/A') imds2, coalesce(imds3, 'N/A') imds3,  
            ' 0 qtysold, 0 onhand, 0 onorder, '' vendor,                         
            'impc2, qt tquote, coalesce(catprc,coalesce(kopric,0)) imprc, z.TQ   
            'Timesq, '' F20, '' Foem, 0 Ncus, impc1, imcata, (select mindes from 
            'mincodes where mincod = inmsta.impc2) mindsc, '' vendorname, '' pagent, '' wlist,
            ''' project, '' projstatus from z left join cater 
            'on z.wrkptn = catptn left join inmsta on z.wrkptn = inmsta.imptn    
            'left join komat on z.wrkptn = koptno where z.wrkptn not in (select  
            'dvpart from dvinva where dvlocn in ('01', '05', '07')) 
            ' FETCH FIRST 200 ROWS ONLY
            '"
            'FETCH FIRST 5000 ROWS ONLY

#End Region
            If Not String.IsNullOrEmpty(strwhere) Then
                pageindex = strwhere.Split(",")(0)
                pagesize = strwhere.Split(",")(1)
            End If

            Dim objLS = New DTO.LostSales
            Dim dtLS = ObjectToDataTableLS(objLS)
            Dim dtOk = dtLS.Clone()

            Dim objDal = New DAL.CTP_SYSTEM()
            'result = If(String.IsNullOrEmpty(strwhere), objDal.GetLostSalesData(dsResult), objDal.GetLostSalesData(dsResult, strwhere))
            result = objDal.GetLostSalesData(pageindex, pagesize, dsResult, optVendors, dtOk)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetLostSalesCountData(ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetLostSalesCountData(dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function InsertWishListReference(userId As String, partNo As String, status As String, from As String, table As String, field As String, Optional ByRef dsReturn As DataSet = Nothing) As Integer
        Dim dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Dim maxItem As Integer
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            maxItem = getmax(table, field)
            If maxItem < 0 Then
                Return result
            Else
                result = objDal.InsertWishListReference(maxItem + 1, userId, partNo, status, from)

                If result = 1 Then
                    Dim firstResult = objDal.GetCustomDataForWishListObj(partNo, dsResult)
                    If dsResult IsNot Nothing Then
                        If dsResult.Tables(0).Rows.Count >= 1 Then
                            dsReturn = dsResult
                        End If
                    End If
                End If

                Return result
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function UpdateWishListTwoReferences(maxItem As String, partNo As String, status As String, user As String) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Dim sql As String = " "
        'Dim maxItem As Integer
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.UpdateWishListTwoReferences(maxItem, partNo, status, user)
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function UpdateWishListGenericReferenceByConcatValues(item As String, user As String, status As String, str As String) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Dim sql As String = " "
        Dim maxItem As Integer
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.UpdateWishListGenericReferenceByConcatValues(item, user, status, str)
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function UpdateWishListGenericReference(user As String, status As String, item As String, partNo As String) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Dim sql As String = " "
        Dim maxItem As Integer
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.UpdateWishListGenericReference(item, user, status, partNo)
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function UpdateWishListSingleReference(user As String, status As String, item As String, comment As String) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Dim sql As String = " "
        Dim maxItem As Integer
        Dim maxLength As Integer = 250
        Try
            Dim objDal = New DAL.CTP_SYSTEM()

            Dim commentNew = If(String.IsNullOrEmpty(comment), comment, If(comment.Length < maxLength, comment, comment.Substring(0, Math.Min(comment.Length, maxLength))))

            result = objDal.UpdateWishListSingleReference(item, user, status, commentNew)
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetTotalClients(partNo As String, factor As Integer) As String
        Dim exMessage As String = Nothing
        Dim ds = New DataSet()
        Dim result As String = Nothing
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            Dim old_strSql = "select count(distinct A1.qdcuno) totalclients from qs36f.qtedtld A1 where (A1.qdptno='{0}') and ((mod(A1.qdqtdt,100)*100)+(int(A1.qdqtdt/10000))) > '{1}'"
            Dim strSql = "SELECT count(distinct sccuno) FROM qs36f.slsbyccm WHERE SCCUNO not in (4384,4385,4381) and SCPTNO ='{0}' and (SCYEAR*100)+ SCMNTH  >= '{1}' "
            Dim yearc = Now.AddYears(CInt(factor)).Year
            Dim customDate = (yearc Mod 100).ToString() + (Now.Month.ToString("d2"))
            Dim sql = String.Format(strSql, partNo, customDate)
            'result = objDal.GetTotalClients(partNo, factor, sql)
            result = If(objDal.GetTotalClients(sql) IsNot Nothing, objDal.GetTotalClients(sql), "0")
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetTotalCountries(partNo As String, factor As Integer) As String
        Dim exMessage As String = Nothing
        Dim ds = New DataSet()
        Dim result As String = Nothing
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            Dim old_strSql = "select count(distinct A2.cuctry) totalcountries from qs36f.cscumpno A2 where A2.cunum in ( select distinct A3.qdcuno from qs36f.qtedtld A3 where A3.qdptno='{0}' and ((mod(  A3.qdqtdt,100)*100)+(int(A3.qdqtdt/10000)) > '{1}'))"
            Dim strSql = "SELECT count(distinct scctry) FROM qs36f.slsbyccm WHERE SCCUNO not in (4384,4385,4381) and SCPTNO ='{0}' and ((SCYEAR*100)+ SCMNTH) >= '{1}' "
            Dim yearc = Now.AddYears(CInt(factor)).Year
            Dim customDate = (yearc Mod 100).ToString() + (Now.Month.ToString("d2"))
            Dim sql = String.Format(strSql, partNo, customDate)
            'result = objDal.GetTotalClients(partNo, factor, sql)
            result = If(objDal.GetTotalCountries(sql) IsNot Nothing, objDal.GetTotalCountries(sql), "0")
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetOEMPart(partNo As String, vendorNo As String) As String
        Dim exMessage As String = Nothing
        Dim ds = New DataSet()
        Dim result As String = Nothing
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            Dim strSql = "select min('X') oempart from qs36f.poqota where pqptn='{0}' and digits('{1}')  not in (select vndnum from qs36f.oemvend)"
            'Dim yearc = Now.AddYears(CInt(factor)).Year
            'Dim customDate = (yearc Mod 100).ToString() + (Now.Month.ToString("d2"))
            Dim sql = String.Format(strSql, partNo, vendorNo)
            'result = objDal.GetTotalClients(partNo, factor, sql)
            result = If(objDal.GetOEMPart(sql) IsNot Nothing, objDal.GetOEMPart(sql), "0")
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetLSBackData(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim objLogs = New Logs()
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetLSBackData(partNo, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function SaveLSItemInProcess(objLS As LostSales) As Integer
        Dim dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Dim objLogs = New Logs()
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.SaveLSItemInProcess(objLS)
            If result = 1 Then

                objLogs.WriteLog(Logs.ErrorTypeEnum.Information, "Info", strLogCadenaCabecera, "NA", "Insertion Succesfully.", "")
            End If

            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLogs.WriteLog(Logs.ErrorTypeEnum.Exception, "Exception", ex.Message, strLogCadenaCabecera, "NA", ex.ToString())
            Return result
        End Try
    End Function



#End Region

#Region "Wish List"

    Public Function GetWishListDataByUser(userSql As String, ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        Dim dsResult1 = New DataSet()
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetWishListDataByUser(userSql, dsResult1, messageOut)
            dsResult = fixLoc20(dsResult1)
            Return result
        Catch ex As Exception
            Throw ex
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetWishListData(ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        Dim dsResult1 = New DataSet()
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetWishListData(dsResult1, messageOut)
            dsResult = fixLoc20(dsResult1)
            Return result
        Catch ex As Exception
            Throw ex
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function getAllPurcUsers(ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.getAllPurcUsers(dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetPartInWishList(partNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetPartInWishList(partNo, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetAllWLStatuses(ByRef Optional messageOut As String = Nothing) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetAllWLStatus(messageOut)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetAllWLFrom() As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetAllWLFrom()
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetReferencesInProject(projectCode As Integer) As Integer
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetReferencesInProject(projectCode)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function DeletePDHeader(projectNo As String, ByRef affectedRows As Integer) As Integer
        affectedRows = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            affectedRows = objDal.DeletePDHeader(projectNo, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return affectedRows
        End Try
    End Function

#Region "Product Development"

#Region "Inserts"

    Public Function InsertNewProject(projectno As String, userid As String, dtValue As Date, strInfo As String, strName As String, ddlStatus As String, strUser As String) As Integer
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.InsertNewProject(projectno, userid, dtValue, strInfo, strName, ddlStatus, strUser)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function InsertProductDetail(projectno As String, partNo As String, dtValue As Date, userid As String, dtValue1 As Date, userid1 As String, dtValue2 As Date, ctpNo As String, qty As String,
                                        mfr As String, mfrNo As String, unitCost As String, unitCostNew As String, poNo As String, dtValue3 As Date, ddlStatus As String, benefits As String,
                                        comments As String, ddlUser As String, dtValue4 As Date, sampleCost As String, miscCost As String, vendorNo As String,
                                        partsToShow As String, ddlMinorCode As String, toolingCost As String, dtValue5 As Date, dtValue6 As Date, sampleQty As String) As Integer
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.InsertProductDetail(projectno, partNo, dtValue, userid, dtValue1, userid1, dtValue2, ctpNo, qty, mfr, mfrNo, unitCost, unitCostNew, poNo, dtValue3, ddlStatus, benefits,
                                                comments, ddlUser, dtValue4, sampleCost, miscCost, vendorNo, partsToShow, ddlMinorCode, toolingCost, dtValue5, dtValue6, sampleQty)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function InsertNewPOQota(partNo As String, vendorNo As String, maxValue As String, strYear As String, strMonth As String, mpnPo As String, strDay As String,
                                    strStsQuote As String, strSpace As String, strUnitCostNew As String, strMinQty As String) As Integer
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.InsertNewPOQota(partNo, vendorNo, maxValue, strYear, strMonth, mpnPo, strDay, strStsQuote, strSpace, strUnitCostNew, strMinQty)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

#End Region

#Region "Updates"

    Public Function UpdatePoQoraRow(mpnopo As String, minQty As String, unitCostNew As String, statusquote As String, insertYear As String, insertMonth As String, insertDay As String,
                                    vendorNo As String, partNo As String) As Integer
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.UpdatePoQoraRow(mpnopo, minQty, unitCostNew, statusquote, insertYear, insertMonth, insertDay, vendorNo, partNo)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

#End Region

#Region "Generics"

    Public Function GetVendorsInProject(projectNo As String) As Data.DataSet
        Dim strResult As String = " "
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            Dim ds = objDal.GetVendorsInProject(projectNo)
            Return ds
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetProjectData(projectNo As String) As Data.DataSet
        Dim strResult As String = " "
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            Dim ds = objDal.GetProjectData(projectNo)
            Return ds
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Private Function ObjectToDataTableLS(objLS As DTO.LostSales) As DataTable
        Dim exMessage As String = Nothing
        Try
            Dim dt As New DataTable
            Dim properties As List(Of PropertyInfo) = objLS.GetType.GetProperties.ToList()

            For Each prop As PropertyInfo In properties
                dt.Columns.Add(prop.Name, prop.PropertyType)
            Next

            dt.TableName = objLS.GetType.Name
            Return dt
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function ObjectToDataTableIn(ByVal o As Object) As DataTable
        Dim exMessage As String = Nothing
        Try
            Dim dt As New DataTable
            Dim properties As List(Of PropertyInfo) = o.GetType.GetProperties.ToList()

            For Each prop As PropertyInfo In properties
                dt.Columns.Add(prop.Name, prop.PropertyType)
            Next

            dt.TableName = o.GetType.Name
            Return dt
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Function GetCtpNumber(partNo As String, ctpNo As String) As DTO.ObjCtp
        Dim exMessage As String = " "
        Dim objctp = New DTO.ObjCtp()

        Try
            If Trim(partNo) <> "" Then
                If Trim(ctpNo) <> "" Then
                    objctp.CtpNumber = partNo
                    objctp.MfrNo = ctpNo
                Else
                    Dim strCTPExist = GetCTPPartRef(partNo)
                    If Not String.IsNullOrEmpty(strCTPExist) Then
                        objctp.CtpNumber = strCTPExist
                        objctp.MfrNo = strCTPExist
                    Else
                        'Dim PartNo = Trim(UCase(txtpartno.Text)).Substring(0, 19) & "                   "
                        Dim myPart = Trim(UCase(partNo)) & "                   "
                        myPart = myPart.Substring(0, Math.Min(myPart.Length, 19))
                        Dim ctppartno = "                   "
                        Dim flagctp = "9"
                        Dim dsctpValue = CallForCtpNumber(partNo, ctppartno, flagctp)
                        If Not dsctpValue Is Nothing Then
                            If dsctpValue.Tables(0).Rows.Count > 0 Then
                                objctp.CtpNumber = Trim(UCase(dsctpValue.Tables(0).Rows(0).ItemArray(1).ToString()))
                                objctp.MfrNo = Trim(UCase(dsctpValue.Tables(0).Rows(0).ItemArray(1).ToString()))
                            Else
                                objctp.CtpNumber = ""
                                objctp.MfrNo = ""
                            End If
                        End If
                    End If
                End If
                Return objctp
            Else
                'part is a must
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function CallForCtpNumber(PartNo As String, ctppartno As String, flagctp As String) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.CallForCtpNumber(PartNo, ctppartno, flagctp)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function isVendorAccepted(vendorNo As String) As Boolean
        Dim exMessage As String = " "
        Try
            'Dim vendorType = getVendorTypeByVendorNum(vendorNo)
            Dim ds As DataSet = getVendorTypeByVendorNum(vendorNo)
            If ds IsNot Nothing Then
                Dim vendorType = ds.Tables(0).Rows(0).ItemArray(0).ToString()
                Dim vendorName = ds.Tables(0).Rows(0).ItemArray(1).ToString()
                Dim listDeniedCodes = ConfigurationManager.AppSettings("vendorCodesDenied")
                'VendorCodesDenied.Split(",")
                Dim containsDenied = listDeniedCodes.AsEnumerable().Any(Function(x As String) x = "'" & vendorType & "'")
                If Not containsDenied Then
                    Dim OEMContain = getOEMVendorCodes(ConfigurationManager.AppSettings("vendorOEMCodeDenied"))
                    Dim containsOEM = OEMContain.Tables(0).AsEnumerable().Any(Function(x) Trim(x.ItemArray(0).ToString()) = Trim(vendorNo))
                    If Not containsOEM Then
                        'frmLoadExcel.lblVendorDesc.Text = vendorName
                        'MessageBox.Show("The vendor " & RTrim(vendorName) & " is an accepted vendor for the operation.", "CTP System", MessageBoxButtons.OK)
                        Return True
                    Else
                        'MessageBox.Show("The vendor " & RTrim(vendorName) & " is not an accepted vendor for the operation.", "CTP System", MessageBoxButtons.OK)
                        Return False
                    End If
                Else
                    'MessageBox.Show("The vendor " & RTrim(vendorName) & " is not an accepted vendor for the operation.", "CTP System", MessageBoxButtons.OK)
                    Return False
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'Log.Error(exMessage)
            Return False
        End Try
    End Function

    Public Function getOEMVendorCodes(cntrCode As String) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.getOEMVendorCodes(cntrCode)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function getVendorTypeByVendorNum(vendorNo As String, Optional ByVal flag As Integer = 0) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim TcpPartNo As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.getVendorTypeByVendorNum(vendorNo, flag)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetCTPPartRef(partNo As String) As String
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim TcpPartNo As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            TcpPartNo = objDal.GetCTPPartRef(partNo)
            Return TcpPartNo
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetCodeAndNameByPartNo(partNo As String) As DataSet
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetCodeAndNameByPartNo(partNo)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetCodeAndNameByPartNoAndVendorNo(partNo As String, vendorNo As String) As DataSet
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetCodeAndNameByPartNoAndVendorNo(partNo, vendorNo)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function CheckIfreferenceExistsinProj(code As String, partNo As String, vendorNo As String) As DataSet
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.CheckIfreferenceExistsinProj(code, partNo, vendorNo)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetProjectStatusDescription(code As String) As String
        Dim strResult As String = " "
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            strResult = objDal.GetProjectStatusDescription(code)
            Return strResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetPOQotaData(vendorNo As String, partNo As String) As DataSet
        Dim dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetPOQotaData(vendorNo, partNo)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

#End Region

#End Region

#End Region

#Region "Claims"

    Public Function GetClaimsReportSingle(ByRef dsResult As DataSet, Optional ByVal strDates As String() = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetClaimsDataSingle(dsResult, strDates)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetClaimsReportFull(ByRef dsResult As DataSet, Optional ByVal strDates As String() = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetClaimsDataFull(dsResult, strDates)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function getClaimNumbers(claimSelected As String, dateValue As DateTime, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        dsResult = New DataSet()
        Dim result As Integer = -1
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.getClaimNumbers(claimSelected, dateValue, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function getSearchByReasonData(dsData As DataSet, ByRef dt As DataTable) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim strReason As String = Nothing

        dt = New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("Reason")
        dt.Columns(0).AutoIncrement = True

        Dim strDataArray As String()
        Try
            strReason = dsData.Tables(0).Rows(0).ItemArray(2).ToString().ToUpper()
            For Each dr As DataRow In dsData.Tables(0).Rows
                If Not strReason.Contains(dr.ItemArray(2).ToString().ToUpper()) Then
                    strReason += "," + dr.ItemArray(2).ToString()
                End If
            Next

            If strReason IsNot Nothing Then
                strDataArray = strReason.Split(",")
                For Each item As String In strDataArray
                    If Not String.IsNullOrEmpty(item) Then
                        Dim R As DataRow = dt.NewRow
                        R("Reason") = item
                        dt.Rows.Add(R)
                    End If
                Next
            End If

            If dt.Rows.Count > 0 Then
                result = 0
            Else
                result = -1
            End If
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try

    End Function

    Public Function getSearchByDiagnoseData(dsData As DataSet, ByRef dt As DataTable) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim strDiagnose As String = Nothing

        dt = New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("Diagnose")
        dt.Columns(0).AutoIncrement = True

        Dim strDataArray As String()
        Try
            strDiagnose = dsData.Tables(0).Rows(0).ItemArray(3).ToString().ToUpper()
            For Each dr As DataRow In dsData.Tables(0).Rows
                If Not strDiagnose.Contains(dr.ItemArray(3).ToString().ToUpper()) Then
                    strDiagnose += "," + dr.ItemArray(3).ToString()
                End If
            Next

            If strDiagnose IsNot Nothing Then
                strDataArray = strDiagnose.Split(",")
                For Each item As String In strDataArray
                    If Not String.IsNullOrEmpty(item) Then
                        Dim R As DataRow = dt.NewRow
                        R("Diagnose") = item
                        dt.Rows.Add(R)
                    End If
                Next
            End If

            If dt.Rows.Count > 0 Then
                result = 0
            Else
                result = -1
            End If
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try

    End Function

    Public Function getSearchByStatusOutData(dsData As DataSet, ByRef dt As DataTable) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim strStatusOut As String = Nothing

        dt = New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("ExtStatus")
        dt.Columns(0).AutoIncrement = True

        Dim strDataArray As String()
        Try
            strStatusOut = dsData.Tables(0).Rows(0).ItemArray(6).ToString().ToUpper()
            For Each dr As DataRow In dsData.Tables(0).Rows
                If (Not strStatusOut.Contains(dr.ItemArray(6).ToString().ToUpper())) And (Not String.IsNullOrEmpty(dr.ItemArray(6).ToString().Trim())) Then
                    strStatusOut += "," + dr.ItemArray(6).ToString()
                End If
            Next

            If strStatusOut IsNot Nothing Then
                strDataArray = strStatusOut.Split(",")
                For Each item As String In strDataArray
                    If Not String.IsNullOrEmpty(item.Trim()) Then
                        Dim R As DataRow = dt.NewRow
                        R("ExtStatus") = item
                        dt.Rows.Add(R)
                    End If
                Next
            End If

            If dt.Rows.Count > 0 Then
                result = 0
            Else
                result = -1
            End If
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try

    End Function

    Public Function getSearchByUserData(dsData As DataSet, ByRef dt As DataTable) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim strUser As String = Nothing

        dt = New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("User")
        dt.Columns(0).AutoIncrement = True

        Dim strDataArray As String()
        Try
            strUser = dsData.Tables(0).Rows(0).ItemArray(11).ToString().ToUpper()
            For Each dr As DataRow In dsData.Tables(0).Rows
                If Not strUser.Contains(dr.ItemArray(11).ToString().ToUpper()) Then
                    strUser += "," + dr.ItemArray(11).ToString()
                End If
            Next

            If strUser IsNot Nothing Then
                strDataArray = strUser.Split(",")
                For Each item As String In strDataArray
                    If Not String.IsNullOrEmpty(item) Then
                        Dim R As DataRow = dt.NewRow
                        R("User") = item
                        dt.Rows.Add(R)
                    End If
                Next
            End If

            If dt.Rows.Count > 0 Then
                result = 0
            Else
                result = -1
            End If
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try

    End Function

    Public Function getSearchByStatusInData(dsData As DataSet, ByRef dt As DataTable) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim strUser As String = Nothing

        dt = New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("intstatus")
        dt.Columns(0).AutoIncrement = True

        Dim strDataArray As String()
        Try
            strUser = dsData.Tables(0).Rows(0).ItemArray(11).ToString().ToUpper()
            For Each dr As DataRow In dsData.Tables(0).Rows
                If Not strUser.Contains(dr.ItemArray(11).ToString().ToUpper()) Then
                    strUser += "," + dr.ItemArray(11).ToString()
                End If
            Next

            If strUser IsNot Nothing Then
                strDataArray = strUser.Split(",")
                For Each item As String In strDataArray
                    If Not String.IsNullOrEmpty(item) Then
                        Dim R As DataRow = dt.NewRow
                        R("intstatus") = item
                        dt.Rows.Add(R)
                    End If
                Next
            End If

            If dt.Rows.Count > 0 Then
                result = 0
            Else
                result = -1
            End If
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try

    End Function

    Public Function GetPDLogsFromSql(ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetPDLogsFromSql(dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

#End Region

    Public Function getmax(table As String, field As String, Optional strWhereAdd As String = Nothing) As Integer
        Dim dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = If((strWhereAdd IsNot Nothing Or Not String.IsNullOrEmpty(strWhereAdd)), objDal.getmax(table, field, strWhereAdd), objDal.getmax(table, field))

            'result = objDal.getmax(table, field)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetGridParameterDin() As List(Of String)
        Dim exMessage As String = Nothing
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            Dim lstResult = objDal.GetGridParameterDin()
            Return lstResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function FillDDlPrPech(ByRef dtTemp As DataTable) As Integer
        Dim dsResult As DataSet = New DataSet()
        dtTemp = New DataTable()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim Sql As String = "SELECT USUSER,USNAME FROM QS36F.CSUSER WHERE USPTY8 = 'X' AND USPTY9 <> 'R' ORDER BY USNAME "
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.FillDDL(Sql, dsResult)
            If result > 0 Then
                dtTemp = dsResult.Tables(0).Copy()

                dtTemp.Columns.Add("MixUser", GetType(String))

                For Each item As DataRow In dtTemp.Rows
                    item("MixUser") = item("USUSER") + " - " + item("USNAME")
                Next

                'Dim row As DataRow = dtTemp.NewRow()
                'row("MixUser") = "NA - No User Selected"
                'row("USUSER") = "NA"
                'dtTemp.Rows.InsertAt(row, 0)
                Return result
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    'Public Function SelectData(sqlQuery As String) As DataTable
    '    Dim exMessage As String = " "
    '    Dim dsResult = New DataSet()
    '    Dim result As Integer = -1
    '    Try

    '        Dim objDal = New DAL.CTP_SYSTEM()
    '        result = objDal.FillGrid(sqlQuery, dsResult)
    '        If result <> -1 Then
    '            Return dsResult.Tables(0)
    '        Else
    '            Return Nothing
    '        End If
    '        '           String connectionString =
    '        '   System.Web.Configuration.WebConfigurationManager.ConnectionStrings
    '        '                           ["SQLServerConnectionString"].ConnectionString;
    '        'Using (SqlDataAdapter sqlDataAdapter = New SqlDataAdapter(sqlQuery, connectionString))
    '        '{
    '        '	DataTable dt = New DataTable("Customers");
    '        '       SqlDataAdapter.Fill(dt);
    '        '	Return dt;
    '        '}
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return Nothing
    '    End Try
    'End Function

    Public Function FillDDlPrPech1(ByRef dtTemp As DataTable) As Integer
        Dim dsResult As DataSet = New DataSet()
        dtTemp = New DataTable()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim Sql As String = "SELECT USUSER,USNAME FROM QS36F.CSUSER WHERE USPTY8 = 'X' AND USPTY9 <> 'R' ORDER BY USNAME "

            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.FillDDL(Sql, dsResult)
            If result > 0 Then
                dtTemp = dsResult.Tables(0).Copy()

                dtTemp.Columns.Add("MixUser", GetType(String))

                For Each item As DataRow In dtTemp.Rows
                    item("MixUser") = item("USUSER") + " - " + item("USNAME")
                Next

                Dim row As DataRow = dtTemp.NewRow()
                row("MixUser") = "NA - No User Selected"
                row("USUSER") = "NA"
                dtTemp.Rows.InsertAt(row, 0)

                Return result
            Else
                Return result
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function FillDDlStatus(ByRef dtTemp As DataTable) As Integer
        Dim dsResult As DataSet = New DataSet()
        dtTemp = New DataTable()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim Sql As String = "SELECT CNT03,CNTDE1 FROM QS36F.cntrll where cnt01 = 'DSI' order by cnt02"
            Dim objDal = New DAL.CTP_SYSTEM()

            result = objDal.FillDDL(Sql, dsResult)
            If result > 0 Then
                dtTemp = dsResult.Tables(0).Copy()

                dtTemp.Columns.Add("MixStatus", GetType(String))

                For Each item As DataRow In dtTemp.Rows
                    item("MixStatus") = item("CNT03") + " - " + item("CNTDE1")
                Next
                Return result
            Else
                Return result
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function fixLoc20(ds As DataSet) As DataSet
        Dim exMessage As String = " "
        Try
            For Each dw As DataRow In ds.Tables(0).Rows
                If Not String.IsNullOrEmpty(dw.Item("loc20flag").ToString()) Then
                    dw.Item("LOC20") = "1"
                Else
                    dw.Item("LOC20") = "0"
                End If
            Next
            Return ds
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    'Public Function FillGridProjects(strwhere As String, flag As Integer, ByRef dsResult As DataSet) As Integer
    '    dsResult = New DataSet()
    '    Dim result As Integer = -1
    '    Dim exMessage As String = " "
    '    Try
    '        Dim Sql As String = "SELECT * FROM QS36F.PRDVLH " + strwhere + " ORDER BY PRDATE DESC"
    '        Dim objDal = New DAL.CTP_SYSTEM()
    '        result = objDal.FillGrid(Sql, dsResult)
    '        Return result
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return result
    '    End Try
    'End Function

    'Public Function FillGridProjectsDetails(strwhere As String, flag As Integer, ByRef dsResult As DataSet) As Integer
    '    dsResult = New DataSet()
    '    Dim result As Integer = -1
    '    Dim exMessage As String = " "
    '    Try
    '        Dim Sql As String = "SELECT distinct QS36F.prdvld.prhcod,prname,prdate,prpech,prstat FROM QS36F.PRDVLH INNER JOIN QS36F.PRDVLD ON QS36F.PRDVLH.PRHCOD = QS36F.PRDVLD.PRHCOD " + strwhere + " ORDER BY PRDATE DESC"
    '        Dim objDal = New DAL.CTP_SYSTEM()
    '        result = objDal.FillGrid(Sql, dsResult)
    '        Return result
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return result
    '    End Try
    'End Function

    'Public Function FillGridProjectsParts(code As String, flag As Integer, ByRef dsResult As DataSet) As Integer
    '    dsResult = New DataSet()
    '    Dim result As Integer = -1
    '    Dim exMessage As String = " "
    '    Try
    '        Dim Sql As String = "SELECT PRDDAT,PRDPTN,PRDCTP,PRDMFR#,QS36F.PRDVLD.VMVNUM,VMNAME,PRDSTS,PRDJIRA FROM QS36F.PRDVLD INNER JOIN QS36F.VNMAS ON QS36F.PRDVLD.VMVNUM = QS36F.VNMAS.VMVNUM WHERE PRHCOD = " + code
    '        Dim objDal = New DAL.CTP_SYSTEM()
    '        result = objDal.FillGrid(Sql, dsResult)
    '        Return result
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return result
    '    End Try
    'End Function

    Public Function GetDataByPRHCOD(code As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim Sql As String = "SELECT * FROM QS36F.PRDVLH WHERE PRHCOD = " + (code.Trim())
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataByPRHCOD(Sql, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetDataByCodeAndPartNo(code As String, partNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim Sql As String = "SELECT * FROM QS36F.PRDVLD INNER JOIN QS36F.VNMAS ON QS36F.PRDVLD.VMVNUM = QS36F.VNMAS.VMVNUM WHERE PRHCOD = " + code.Trim() + " AND trim(ucase(PRDPTN)) = '" + UCase(partNo).Trim() + "'"
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataByCodeAndPartNo(Sql, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetDataByPartNo(partNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim columnToChange As String = "IMDSC"
        Dim strResult = " "
        Try
            Dim Sql As String = "SELECT * FROM QS36F.INMSTA INNER JOIN QS36F.DVINVA ON QS36F.INMSTA.IMPTN = QS36F.DVINVA.DVPART WHERE UCASE(IMPTN) = '" + partNo.ToUpper().Trim() + "'"
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataByPartNo(Sql, columnToChange, strResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetDataByPartNo2(partNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            sql = "SELECT * FROM QS36F.DVINVA INNER JOIN QS36F.VNMAS ON QS36F.DVINVA.DVPRMG = digits(QS36F.VNMAS.VMVNUM) WHERE DVPART = '" + partNo.ToUpper().Trim() + "' and dvlocn = '01'"
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataByCodeAndPartNo(sql, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetVendorByVendorNo(vendorNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataByCodeAndPartNo(vendorNo, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function getUserDataByPurc(purcNumber As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.getUserDataByPurc(purcNumber, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetDataFromDev(partNo As String, vendorNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataFromDev(partNo, vendorNo, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetDataFromCatDesc(categoryCode As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetDataFromCatDesc(categoryCode, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetAutoCompleteDataPartNo(prefixText As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetAutoCompleteDataPartNo(prefixText, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetAutocompleteSelectedVendorName(prefixVendorName As String, VendorCodesDenied As String, VendorOEMCodeDenied As String, ItemCategories As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetAutocompleteSelectedVendorName(prefixVendorName, VendorCodesDenied, VendorOEMCodeDenied, ItemCategories, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetVendorByNumber(vendorNo As String, VendorCodesDenied As String, VendorOEMCodeDenied As String, ItemCategories As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetVendorByNumber(vendorNo, VendorCodesDenied, VendorOEMCodeDenied, ItemCategories, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetNewPartData(partNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim sql As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.GetNewPartData(partNo, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetAllPaAndPsUsers(ByRef Optional messageOut As String = Nothing) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetAllPaAndPsUsers(messageOut)
            Return dsResult
        Catch ex As Exception
            Throw ex
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function GetAllMinors(ByRef Optional messageOut As String = Nothing) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetAllMinors(messageOut)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function buildStatusString(status As String) As String
        Dim exMessage As String = ""
        Dim newValue As String = ""
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetAllStatuses()

            'dsStatuses.Tables(0).Columns.Add("FullValue", GetType(String))

            'For i As Integer = 0 To dsStatuses.Tables(0).Rows.Count - 1
            '    If dsStatuses.Tables(0).Rows(i).Table.Columns("FullValue").ToString = "FullValue" Then
            '        Dim fllValueName = dsStatuses.Tables(0).Rows(i).Item(2).ToString() + " -- " + dsStatuses.Tables(0).Rows(i).Item(3).ToString()
            '        dsStatuses.Tables(0).Rows(i).Item(5) = fllValueName
            '    End If
            'Next

            Dim dwResult = dsResult.Tables(0).AsEnumerable() _
                          .Where(Function(x) Trim(UCase(x.Field(Of String)("CNT03"))) = Trim(UCase(status)))
            Dim rowLenght = dwResult.LongCount
            If rowLenght > 0 Then
                newValue = Trim(dwResult(0).ItemArray(1).ToString())
                Return newValue
            Else
                Exit Function
            End If
        Catch ex As Exception
            exMessage = ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function adjustDatetimeFormat(documentName As String, documentExt As String) As String

        Dim exMessage As String = Nothing
        Try
            Dim name As String = Nothing
            Dim culture As CultureInfo = CultureInfo.CreateSpecificCulture("en-US")
            Dim dtfi As DateTimeFormatInfo = culture.DateTimeFormat
            dtfi.DateSeparator = "."

            Dim now As DateTime = DateTime.Now
            Dim halfName = now.ToString("G", dtfi)
            halfName = halfName.Replace(" ", ".")
            halfName = halfName.Replace(":", "")
            Dim fileName = documentName & "." & halfName & "." & documentExt
            Return fileName
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Function Determine_OfficeVersion() As String
        Dim exMessage As String = " "
        Dim strExt As String = Nothing
        Try
            Dim strEVersionSubKey As String = "\Excel.Application\CurVer" '/HKEY_CLASSES_ROOT/Excel.Application/Curver

            Dim strValue As String 'Value Present In Above Key
            Dim strVersion As String 'Determines Excel Version
            Dim strExtension() As String = {"xls", "xlsx"}

            Dim rkVersion As RegistryKey = Nothing 'Registry Key To Determine Excel Version
            rkVersion = Registry.ClassesRoot.OpenSubKey(name:=strEVersionSubKey, writable:=False) 'Open Registry Key

            If Not rkVersion Is Nothing Then 'If Key Exists
                strValue = rkVersion.GetValue(String.Empty) 'get Value
                strValue = strValue.Substring(strValue.LastIndexOf(".") + 1) 'Store Value

                Select Case strValue 'Determine Version
                    Case "7"
                        strVersion = "95"
                        strExt = strExtension(0)
                    Case "8"
                        strVersion = "97"
                        strExt = strExtension(0)
                    Case "9"
                        strVersion = "2000"
                        strExt = strExtension(0)
                    Case "10"
                        strVersion = "2002"
                        strExt = strExtension(0)
                    Case "11"
                        strVersion = "2003"
                        strExt = strExtension(0)
                    Case "12"
                        strVersion = "2007"
                        strExt = strExtension(1)
                    Case "14"
                        strVersion = "2010"
                        strExt = strExtension(1)
                    Case "15"
                        strVersion = "2013"
                        strExt = strExtension(1)
                    Case "16"
                        strVersion = "2016"
                        strExt = strExtension(1)
                    Case Else
                        strExt = strExtension(1)
                End Select

                Return strExt
            Else
                Return strExt
            End If
        Catch ex As Exception
            exMessage = ex.Message + ". " + ex.ToString
            Return strExt
        End Try
    End Function

    Public Function GetLSBackData400(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            dsResult = objDal.GetLSBackData400(partNo, dsResult)
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then
                    result = dsResult.Tables(0).Rows.Count
                End If
            End If
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function InsertLSBackData400(objLS As LostSales, Optional externalStatus As String = Nothing) As Integer
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.InsertLSBackData400(objLS, externalStatus)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function UpdateLSBackData400(partNo As String, externalStatus As String, Optional user As String = Nothing) As Integer
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CTP_SYSTEM()
            result = objDal.UpdateLSBackData400(partNo, externalStatus, user)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

#Region "DISPOSABLE"

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
