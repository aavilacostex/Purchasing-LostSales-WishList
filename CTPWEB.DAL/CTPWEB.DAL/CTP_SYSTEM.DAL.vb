Imports System.Configuration
Imports System.Globalization
Imports System.Reflection
Imports CTPWEB.DTO
Imports CTPWEB.UTIL

Public Class CTP_SYSTEM : Implements IDisposable
    Private disposedValue As Boolean

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    Shared ReadOnly objLog = New Logs()

    Public Shared Function ObjectToDataTableDeep(ByVal o As Object) As DataTable
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
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try

    End Function

#Region "Lost Sales"

    Public Function GetLostSalesData(query As String, ByRef dsResult As DataSet, Optional dt As DataTable = Nothing) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            dt = New DataTable()
            'result = objDatos.GetOdBcDataFromDatabase(query, dsOut, dt)
            result = objDatos.GetDataFromDatabase(query, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetLostSalesCountData(ByRef dsResult As DataSet)
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "

        Dim initialQuery As String = "
                                 with z as (SELECT                                                   
                                WRKPTN,qt,t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 TQ from (SELECT
                                 WRKPTN,(WRK001+ WRK002+ WRK003+ WRK004+ WRK005+                    
                                WRK006+WRK007+WRK008+ WRK009+ WRK010+ WRK011+ WRK012+ WRK013)       
                                QT,CASE WRK001 WHEN 0 THEN 0 ELSE 1 END T1,CASE WRK002 WHEN 0 THEN 0
                                 ELSE 1 END T2,CASE WRK003 WHEN 0 THEN 0 ELSE 1 END T3,CASE WRK004  
                                WHEN 0 THEN 0 ELSE 1 END T4,CASE WRK005 WHEN 0 THEN 0 ELSE 1 END T5,
                                 CASE WRK006 WHEN 0 THEN 0 ELSE 1 END T6,CASE WRK007 WHEN 0 THEN 0  
                                 ELSE 1 END T7,CASE WRK008 WHEN 0 THEN 0 ELSE 1 END T8,CASE WRK009  
                                WHEN 0 THEN 0 ELSE 1 END T9,CASE WRK010 WHEN 0 THEN 0 ELSE 1 END    
                                T10,CASE WRK011 WHEN 0 THEN 0 ELSE 1 END T11,CASE WRK012 WHEN 0 THEN
                                0 ELSE 1 END T12,CASE WRK013 WHEN 0 THEN 0 ELSE 1 END T13 FROM     
                                qs36f.ddtwrk) a where t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 > 5) 

                                select count(imptn)  imptn 
                                from qs36f.inmsta Q left join 
                                (select dvpart, sum(dvonh#) onhand, sum(dvono#) onorder, max(dvprmg) vendor 
                                from qs36f.dvinva where dvlocn in ('01', '05', '07','26') and trim(dvprmg) <> '' and dvonh# <= 0 and dvono# <= 0 group by dvpart) x on Q.imptn = x.dvpart  
                                inner join qs36f.invptyf on Q.imptn = qs36f.invptyf.ippart
                                where substr(ucase(trim(imdsc)),1,3) <> 'USE' and impc1 in ('01','03') and        
                                (Q.IMQTE+Q.IMQT01+Q.IMQT02+Q.IMQT03+Q.IMQT04+ Q.IMQT05+Q.IMQT06+Q.IMQT07+Q.IMQT08+Q.IMQt09+ Q.IMQT10+Q.IMQT11+Q.IMQT12) > 0 
                                and (REGEXP_LIKE (x.vendor,'^[0-9]{2}$') /*Or (x.vendor = '')*/)
                                /*and imptn not in (select whlpartn from qs36f.prdwl where whlstatus <> '')*/
                                and imptn not in (select puoptn from qs36f.ptnuse where puinfo = 'N' and putype = 'C')	 
                                and imptn not in (select imptn from qs36f.inmstpat) 
                                union
                                select count(z.wrkptn)  imptn 
                                from z left join qs36f.cater on z.wrkptn = catptn 
                                left join qs36f.inmsta W on z.wrkptn = W.imptn    
                                left join qs36f.komat on z.wrkptn = koptno 
                                where z.wrkptn not in (select dvpart from qs36f.dvinva where dvlocn in ('01', '05', '07','26'))  "

        Dim yearUse = DateTime.Now().AddYears(-2).Year
        Dim firstDate = New DateTime(yearUse, 1, 1)
        Dim strDate As String = firstDate.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
        Dim regexCriteria As String = "{1,6}"
        'Dim d2 As DateTime = DateTime.ParseExact(strDate, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture)

        Dim resultQuery = String.Format(initialQuery, strDate, strDate, regexCriteria)

        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            'result = objDatos.GetOdBcDataFromDatabase(resultQuery, dsOut)
            result = objDatos.GetDataFromDatabase(resultQuery, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try

    End Function

    Public Function GetLostSalesData(pageIndex As String, pageSize As String, ByRef dsResult As DataSet, Optional optVendors As String = Nothing, Optional dt As DataTable = Nothing) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim devPagIndex As String = Nothing
        Dim devPagSize As String = Nothing
        Dim limit As String = Nothing
        Dim limitSql As String = Nothing

        If Not String.IsNullOrEmpty(pageSize) And Not String.IsNullOrEmpty(pageIndex) Then
            devPagIndex = ((CInt(pageIndex) * CInt(pageSize)) + 1).ToString()
            devPagSize = ((CInt(pageIndex) + 1) * CInt(pageSize)).ToString()

            limit = " order by 12 Desc limit {0} , {1}"
            limitSql = String.Format(limit, devPagIndex, devPagSize)
            'limitSql = ""
        Else
            limitSql = "order by 12 Desc"
        End If

        Dim initialQuery As String = "
                                 with z as (SELECT                                                   
                                WRKPTN,qt,t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 TQ from (SELECT
                                 WRKPTN,(WRK001+ WRK002+ WRK003+ WRK004+ WRK005+                    
                                WRK006+WRK007+WRK008+ WRK009+ WRK010+ WRK011+ WRK012+ WRK013)       
                                QT,CASE WRK001 WHEN 0 THEN 0 ELSE 1 END T1,CASE WRK002 WHEN 0 THEN 0
                                 ELSE 1 END T2,CASE WRK003 WHEN 0 THEN 0 ELSE 1 END T3,CASE WRK004  
                                WHEN 0 THEN 0 ELSE 1 END T4,CASE WRK005 WHEN 0 THEN 0 ELSE 1 END T5,
                                 CASE WRK006 WHEN 0 THEN 0 ELSE 1 END T6,CASE WRK007 WHEN 0 THEN 0  
                                 ELSE 1 END T7,CASE WRK008 WHEN 0 THEN 0 ELSE 1 END T8,CASE WRK009  
                                WHEN 0 THEN 0 ELSE 1 END T9,CASE WRK010 WHEN 0 THEN 0 ELSE 1 END    
                                T10,CASE WRK011 WHEN 0 THEN 0 ELSE 1 END T11,CASE WRK012 WHEN 0 THEN
                                0 ELSE 1 END T12,CASE WRK013 WHEN 0 THEN 0 ELSE 1 END T13 FROM     
                                qs36f.ddtwrk) a where t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 > 5) 

                                (select imptn, imdsc, imds2, imds3, 
                                (select zoned(sum(odrsq),10,0) from qs36f.horddt where odrcd = 1 and                                     
                                SUBSTR(DIGITS(ODDATE),5 ,2) || SUBSTR(DIGITS(ODDATE),1 ,2) ||      
                                SUBSTR(DIGITS(ODDATE),3 ,2) >= '{0}' and odptn = imptn and odcu# 
                                not in ({7}) and odlcn in ({8})      
                                group by odptn) qtysold,
                                x.onhand, x.onorder, coalesce(x.vendor, '') vendor, impc2,z.QT TQUOTE, 
                                imprc, qs36f.invptyf.iptqte Timesq,
                                coalesce((select 'X' from qs36f.dvinva where dvlocn='20' and dvpart=imptn and dvonh#>0),' ') F20, 
                                coalesce ((select 'X' from qs36f.cntrll A where A.cnt01 = '416' and cntde1 = x.vendor),'') Foem,                                
                                zoned(coalesce((select count(distinct qdcuno) from qs36f.qtedtld where qdptno=imptn and qdyear||qdmth||qdday >= '{1}'),0),5,0) Ncus, 
                                impc1, imcata, (select mindes from qs36f.mincodes where mincod = Q.impc2) mindsc,
                                CASE WHEN (coalesce (x.vendor,'') = '') Or (LENGTH(RTRIM(TRANSLATE(x.vendor, '*', ' 0123456789'))) = 0 ) THEN '' ELSE (select vmname from qs36f.vnmas where vmvnum = x.vendor) END  vendorname, 
                                coalesce(CASE WHEN coalesce(x.vendor, '') <> ''
                                THEN (SELECT MIN(USNAME) FROM qs36f.CSUSER WHERE USPTY9 = '' AND USPURC = case when coalesce(x.vendor, '') <> '' then (SELECT VM#POY FROM qs36f.VNMAS WHERE VMVNUM = x.vendor) else 5000000 end )  
                                ELSE ' '  END , '') pagent,                                
                                coalesce((SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = IMCATA), '') catdesc,
                                coalesce((SELECT INDESS FROM qs36f.INMCAS WHERE INSBCA = IMSBCA), '') subcatdesc,   
                                (SELECT count(distinct sccuno) FROM qs36f.slsbyccm WHERE SCCUNO not in  ({7}) and SCPTNO = Q.imptn and (SCYEAR*100)+ SCMNTH  between '{0}' and '{5}') totalclients,  
                                (SELECT count(distinct scctry) FROM qs36f.slsbyccm WHERE SCCUNO not in ({7}) and SCPTNO = Q.imptn and (SCYEAR*100)+ SCMNTH between '{0}' and '{5}' ) totalcountries,  
                                (select min('X')  from qs36f.poqota where pqptn=imptn and digits(pqvnd)  not in (select vndnum from qs36f.oemvend)) oempart, 
                                coalesce((select perpech from qs36f.LOSTSALBCK LS where LS.imptn = Q.imptn and LS.EXTERNALSTS = 'NEW'), '') prpech
                                from qs36f.inmsta Q inner join z on Q.imptn = z.wrkptn left join 
                                (select dvpart, sum(dvonh#) onhand, sum(dvono#) onorder, max(dvprmg) vendor 
                                from qs36f.dvinva where dvlocn in ({6}) and ((trim(dvprmg) = '' or trim(dvprmg) = '000000') and dvonh# <= 0 and dvono# <= 0) group by dvpart) x on Q.imptn = x.dvpart  
                                inner join qs36f.invptyf on Q.imptn = qs36f.invptyf.ippart
                                where substr(ucase(trim(imdsc)),1,3) <> 'USE' and impc1 in ('01','03')  
                                and imptn not in (select puoptn from qs36f.ptnuse where puinfo = 'N' and putype = 'C')	 
                                and imptn not in (select imptn from qs36f.inmstpat)                                  
                                and (not REGEXP_LIKE (coalesce(x.vendor, ''),'^[0-9]{2}$') or  x.vendor in ({4}))
                                and imptn not in (select dvpart from qs36f.dvinva where dvlocn in ({6}) and ((trim(dvprmg) <> '' and trim(dvprmg) <> '000000') or dvonh# > 0 or dvono# > 0 ))
                                and imptn not in (select imptn from qs36f.LOSTSALBCK LS where LS.imptn = Q.imptn and LS.EXTERNALSTS = 'WSH') 
                                and imptn not in (select  whlpartn from qs36f.prdwl)
                                and trim(imdsc) <> ''
                                union
                                select z.wrkptn imptn, coalesce(catdsc,coalesce(kodesc,'N/A'))
                                imdsc, coalesce(imds2, 'N/A') imds2, coalesce(imds3, 'N/A') imds3,  
                                 0 qtysold, 0 onhand, 0 onorder, '' vendor,                         
                                impc2, qt tquote, coalesce(catprc,coalesce(kopric,0)) imprc, z.TQ   
                                Timesq, '' F20, '' Foem, 0 Ncus, impc1, imcata, (select mindes from 
                                qs36f.mincodes where mincod = W.impc2) mindsc, '' vendorname, '' pagent, 
                                '' catdesc, '' subcatdesc, 0 totalclients, 0 totalcountries, '' oempart , '' prpech 
                                from z left join qs36f.cater on z.wrkptn = catptn 
                                left join qs36f.inmsta W on z.wrkptn = W.imptn    
                                left join qs36f.komat on z.wrkptn = koptno 
                                where z.wrkptn not in (select dvpart from qs36f.dvinva where dvlocn in ({6}))
                                and z.wrkptn not in (select puoptn from qs36f.ptnuse where puinfo = 'N' and putype = 'C')
                                and substr(ucase(trim(imdsc)),1,3) <> 'USE'
                                and z.wrkptn not in (select  whlpartn from qs36f.prdwl)
                                 ) {3} "

        'and trim(catdsc) <> '' and trim(kodesc) <> ''
        'FETCH FIRST 1000 ROWS ONLY
        'revisando aqui error en la query

        Dim yearUse = DateTime.Now().AddYears(-1).Year
        Dim monthUse = DateTime.Now().Month()
        Dim yearUseCurrent = DateTime.Now().Year
        Dim firstDate = New DateTime(yearUse, monthUse, 1)
        Dim firstDateCurrent = New DateTime(yearUseCurrent, monthUse, 1)
        Dim strDate As String = firstDate.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
        'Dim strDateCurrent As String = firstDateCurrent.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
        Dim strDateReduc As String = firstDate.ToString("yyMM", System.Globalization.CultureInfo.InvariantCulture)
        Dim strDateCurrentReduc As String = firstDateCurrent.ToString("yyMM", System.Globalization.CultureInfo.InvariantCulture)
        Dim regexCriteria As String = "{6}"
        Dim LostSalesLoc = ConfigurationManager.AppSettings("LostSalesLocations")
        Dim CustExceptions = ConfigurationManager.AppSettings("CustomerExceptions")
        Dim QtySoldLoc = ConfigurationManager.AppSettings("QtySoldLocations")
        'Dim limit = "limit 21,30"
        'Dim d2 As DateTime = DateTime.ParseExact(strDate, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture)

        Dim resultQuery = String.Format(initialQuery, strDateReduc, strDate, regexCriteria, limitSql, optVendors, strDateCurrentReduc, LostSalesLoc, CustExceptions, QtySoldLoc)

#Region "old query last"

        'Dim initialQuery As String = "
        '                         with z as (SELECT                                                   
        '                        WRKPTN,qt,t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 TQ from (SELECT
        '                         WRKPTN,(WRK001+ WRK002+ WRK003+ WRK004+ WRK005+                    
        '                        WRK006+WRK007+WRK008+ WRK009+ WRK010+ WRK011+ WRK012+ WRK013)       
        '                        QT,CASE WRK001 WHEN 0 THEN 0 ELSE 1 END T1,CASE WRK002 WHEN 0 THEN 0
        '                         ELSE 1 END T2,CASE WRK003 WHEN 0 THEN 0 ELSE 1 END T3,CASE WRK004  
        '                        WHEN 0 THEN 0 ELSE 1 END T4,CASE WRK005 WHEN 0 THEN 0 ELSE 1 END T5,
        '                         CASE WRK006 WHEN 0 THEN 0 ELSE 1 END T6,CASE WRK007 WHEN 0 THEN 0  
        '                         ELSE 1 END T7,CASE WRK008 WHEN 0 THEN 0 ELSE 1 END T8,CASE WRK009  
        '                        WHEN 0 THEN 0 ELSE 1 END T9,CASE WRK010 WHEN 0 THEN 0 ELSE 1 END    
        '                        T10,CASE WRK011 WHEN 0 THEN 0 ELSE 1 END T11,CASE WRK012 WHEN 0 THEN
        '                        0 ELSE 1 END T12,CASE WRK013 WHEN 0 THEN 0 ELSE 1 END T13 FROM     
        '                        qs36f.ddtwrk) a where t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 > 5) 

        '                        (select imptn, imdsc, imds2, imds3, 
        '                        /*(select zoned(sum(odrsq),10,0) from qs36f.horddt where odrcd = 1 and                                     
        '                        SUBSTR(DIGITS(ODDATE),5 ,2) || SUBSTR(DIGITS(ODDATE),1 ,2) ||      
        '                        SUBSTR(DIGITS(ODDATE),3 ,2) >= '{0}' and odptn = imptn and odcu# 
        '                        not in (4384,4385,4381) and odlcn in ('01', '04', '05', '07', '02','09','26')      
        '                        group by odptn) qtysold,*/ 
        '                        (SELECT sum(SCTQTY) FROM qs36f.slsbyccm WHERE SCCUNO not in (4384,4385,4381) and SCPTNO = imptn and (SCYEAR*100)+ SCMNTH  >= '{0}') qtysold,    
        '                        x.onhand, x.onorder, coalesce(x.vendor, '') vendor, impc2,                                
        '                        (Q.IMQTE+Q.IMQT01+Q.IMQT02+Q.IMQT03+Q.IMQT04+Q.IMQT05+Q.IMQT06+
        '                        Q.IMQT07+Q.IMQT08+Q.imqT09+Q.IMQT10+Q.IMQT11+Q.IMQT12) TQUOTE,    
        '                        imprc,qs36f.invptyf.iptqte Timesq, 
        '                        coalesce((select 'X' from qs36f.dvinva where dvlocn='20' and dvpart=imptn and dvonh#>0),' ') F20, 							 
        '                        /*(case when x.vendor = '261339' or x.vendor='060106' or x.vendor='262369' or x.vendor = '262673' or x.vendor='261903' or x.vendor='150064' then 'X' else '' end) Foem, */                               
        '                         coalesce ((select 'X' from qs36f.cntrll A where A.cnt01 = '416' and cntde1 = x.vendor),'') Foem,                                
        '                        zoned(coalesce((select count(distinct qdcuno) from qs36f.qtedtld where qdptno=imptn and qdyear||qdmth||qdday >= '{1}'),0),5,0) Ncus, 
        '                        impc1, imcata, (select mindes from qs36f.mincodes where mincod = Q.impc2) mindsc, 
        '                        coalesce((select vmname from qs36f.vnmas where vmvnum = x.vendor), '')  vendorname, 
        '                        coalesce((SELECT CASE WHEN VM#POY <> 0
        '                        THEN (SELECT MIN(USNAME) FROM qs36f.CSUSER WHERE USPTY9 = '' AND USPURC = (SELECT VM#POY FROM qs36f.VNMAS WHERE VMVNUM = x.vendor))
        '                        ELSE ' ' END AS PAGENT FROM qs36f.VNMAS WHERE VMVNUM = x.vendor), '') pagent,                                
        '                        (select count(whlpartn) from qs36f.prdwl where whlpartn = imptn ) wlist,
        '                        coalesce((select prhcod 
        '                        from qs36f.prdvld where vmvnum = x.vendor and prdptn = imptn FETCH FIRST 1 ROWS ONLY), 0) project,  
        '                        coalesce((select prdsts from qs36f.prdvld where vmvnum = x.vendor and prdptn = imptn FETCH FIRST 1 ROWS ONLY),'') projstatus,
        '                        coalesce((SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = IMCATA), '') catdesc,
        '                        coalesce((SELECT INDESS FROM qs36f.INMCAS WHERE INSBCA = IMSBCA), '') subcatdesc   
        '                        , 0 totalclients, 0 totalcountry, '' oemvendor                                
        '                        /*,(select count(distinct A1.qdcuno) from qs36f.qtedtld A1 where (A1.qdptno=imptn) and ((mod(A1.qdqtdt,100)*100)+(int(A1.qdqtdt/10000))) > '2002'   ) totalclients,
        '                        (select count(distinct A2.cuctry) from qs36f.cscumpno A2 where A2.cunum in ( select distinct A3.qdcuno from qs36f.qtedtld A3 where A3.qdptno=imptn and ((mod(  A3.qdqtdt,100)*100)+(int(A3.qdqtdt/10000)) > '2002')) )  totalcountry,
        '                        (select min('X')  from qs36f.poqota where pqptn=imptn and digits(pqvnd)  not in (select vndnum from qs36f.oemvend)) oemvendor */  
        '                        from qs36f.inmsta Q left join 
        '                        (select dvpart, sum(dvonh#) onhand, sum(dvono#) onorder, max(dvprmg) vendor 
        '                        from qs36f.dvinva where dvlocn in ('01', '05', '07','26') and trim(dvprmg) <> '' and dvonh# <= 0 and dvono# <= 0 group by dvpart) x on Q.imptn = x.dvpart  
        '                        inner join qs36f.invptyf on Q.imptn = qs36f.invptyf.ippart
        '                        where substr(ucase(trim(imdsc)),1,3) <> 'USE' and impc1 in ('01','03') and        
        '                        (Q.IMQTE+Q.IMQT01+Q.IMQT02+Q.IMQT03+Q.IMQT04+ Q.IMQT05+Q.IMQT06+Q.IMQT07+Q.IMQT08+Q.IMQt09+ Q.IMQT10+Q.IMQT11+Q.IMQT12) > 0 
        '                        and (REGEXP_LIKE (x.vendor,'^[0-9]{2}$') /*Or (x.vendor = '')*/)
        '                        /*and imptn not in (select whlpartn from qs36f.prdwl where whlstatus <> '')*/
        '                        and imptn not in (select puoptn from qs36f.ptnuse where puinfo = 'N' and putype = 'C')	 
        '                        and imptn not in (select imptn from qs36f.inmstpat) 
        '                        union
        '                        select z.wrkptn imptn, coalesce(catdsc,coalesce(kodesc,'N/A'))
        '                        imdsc, coalesce(imds2, 'N/A') imds2, coalesce(imds3, 'N/A') imds3,  
        '                         0 qtysold, 0 onhand, 0 onorder, '' vendor,                         
        '                        impc2, qt tquote, coalesce(catprc,coalesce(kopric,0)) imprc, z.TQ   
        '                        Timesq, '' F20, '' Foem, 0 Ncus, impc1, imcata, (select mindes from 
        '                        qs36f.mincodes where mincod = W.impc2) mindsc, '' vendorname, '' pagent, 0 wlist,
        '                        0 project, '' projstatus, '' catdesc, '' subcatdesc                        
        '                        , 0 totalclients, 0 totalcountry, '' oemvendor 
        '                        from z left join qs36f.cater on z.wrkptn = catptn 
        '                        left join qs36f.inmsta W on z.wrkptn = W.imptn    
        '                        left join qs36f.komat on z.wrkptn = koptno 
        '                        where z.wrkptn not in (select dvpart from qs36f.dvinva where dvlocn in ('01', '05', '07','26'))) {3} "

#End Region

#Region "old query first"

        'Dim query As String = "with z as (SELECT                                                   
        '                        WRKPTN,qt,t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 TQ from (SELECT
        '                         WRKPTN,(WRK001+ WRK002+ WRK003+ WRK004+ WRK005+                    
        '                        WRK006+WRK007+WRK008+ WRK009+ WRK010+ WRK011+ WRK012+ WRK013)       
        '                        QT,CASE WRK001 WHEN 0 THEN 0 ELSE 1 END T1,CASE WRK002 WHEN 0 THEN 0
        '                         ELSE 1 END T2,CASE WRK003 WHEN 0 THEN 0 ELSE 1 END T3,CASE WRK004  
        '                        WHEN 0 THEN 0 ELSE 1 END T4,CASE WRK005 WHEN 0 THEN 0 ELSE 1 END T5,
        '                         CASE WRK006 WHEN 0 THEN 0 ELSE 1 END T6,CASE WRK007 WHEN 0 THEN 0  
        '                         ELSE 1 END T7,CASE WRK008 WHEN 0 THEN 0 ELSE 1 END T8,CASE WRK009  
        '                        WHEN 0 THEN 0 ELSE 1 END T9,CASE WRK010 WHEN 0 THEN 0 ELSE 1 END    
        '                        T10,CASE WRK011 WHEN 0 THEN 0 ELSE 1 END T11,CASE WRK012 WHEN 0 THEN
        '                         0 ELSE 1 END T12,CASE WRK013 WHEN 0 THEN 0 ELSE 1 END T13 FROM     
        '                        qs36f.ddtwrk) a where t1+t2+t3+t4+t5+t6+t7+t8+t9+t10+t11+t12+t13 > 5) 
        '                        select imptn, imdsc, imds2, imds3, 
        '                        (select zoned(sum(odrsq),10,0) from qs36f.horddt where odrcd = 1 and                                     
        '                        SUBSTR(DIGITS(ODDATE),5 ,2) || SUBSTR(DIGITS(ODDATE),1 ,2) ||      
        '                        SUBSTR(DIGITS(ODDATE),3 ,2) >= '191206' and odptn = imptn and odcu# 
        '                        not in (4384,4385,4381) and odlcn in ('01', '04', '05', '07', '02','09','26')      
        '                        group by odptn) qtysold, 
        '                        x.onhand, x.onorder, coalesce(x.vendor, '') vendor, impc2,                                
        '                        (Q.IMQTE+Q.IMQT01+Q.IMQT02+Q.IMQT03+Q.IMQT04+Q.IMQT05+Q.IMQT06+
        '                        Q.IMQT07+Q.IMQT08+Q.imqT09+Q.IMQT10+Q.IMQT11+Q.IMQT12) TQUOTE,    
        '                        imprc,qs36f.invptyf.iptqte Timesq, 
        '                        coalesce((select 'X' from qs36f.dvinva where dvlocn='20' and dvpart=imptn and dvonh#>0),' ') F20,          
        '                        (case when x.vendor = '261339' or x.vendor='060106' or x.vendor='262369' or x.vendor = '262673' or x.vendor='261903' or x.vendor='150064' then 'X' else '' end) Foem,
        '                        zoned(coalesce((select count(distinct qdcuno) from qs36f.qtedtld where qdptno=imptn and qdyear||qdmth||qdday >= '191206'),0),5,0) Ncus, 
        '                        impc1, imcata, (select mindes from qs36f.mincodes where mincod = Q.impc2) mindsc, 
        '                        coalesce((select vmname from qs36f.vnmas where vmvnum = x.vendor), '')  vendorname, 
        '                        (SELECT CASE WHEN VMABB# <> 0
        '                        THEN (SELECT USNAME FROM qs36f.CSUSER WHERE USPURC = (SELECT VMABB# FROM qs36f.VNMAS WHERE VMVNUM = x.vendor))
        '                        ELSE ' ' END AS PAGENT FROM qs36f.VNMAS WHERE VMVNUM = x.vendor) pagent, 
        '                        (select count(whlpartn) from qs36f.prdwl where whlpartn = imptn ) wlist,
        '                        coalesce((select prhcod from qs36f.prdvld where vmvnum = x.vendor and prdptn = imptn FETCH FIRST 1 ROWS ONLY), 0) project,  
        '                        coalesce((select prdsts from qs36f.prdvld where vmvnum = x.vendor and prdptn = imptn FETCH FIRST 1 ROWS ONLY),'') projstatus,
        '                        coalesce((SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = IMCATA), '') catdesc
        '                        from qs36f.inmsta Q left join 
        '                        (select dvpart, sum(dvonh#) onhand, sum(dvono#) onorder, max(dvprmg) vendor 
        '                        from qs36f.dvinva where dvlocn in ('01', '05', '07','26') and trim(dvprmg) <> '' and dvonh# <= 0 and dvono# <= 0 group by dvpart) x on Q.imptn = x.dvpart  
        '                        inner join qs36f.invptyf on Q.imptn = qs36f.invptyf.ippart 
        '                        where substr(ucase(trim(imdsc)),1,3) <> 'USE' and impc1 in ('01','03') and        
        '                        (Q.IMQTE+Q.IMQT01+Q.IMQT02+Q.IMQT03+Q.IMQT04+ Q.IMQT05+Q.IMQT06+Q.IMQT07+Q.IMQT08+Q.IMQt09+ Q.IMQT10+Q.IMQT11+Q.IMQT12) > 0 
        '                        and (REGEXP_LIKE (x.vendor,'^[0-9]{1,6}$') Or (x.vendor = ''))
        '                        and imptn not in (select puoptn from ptnuse where puinfo = 'N' and putype = 'C')
        '                        union select z.wrkptn imptn, coalesce(catdsc,coalesce(kodesc,'N/A'))
        '                        imdsc, coalesce(imds2, 'N/A') imds2, coalesce(imds3, 'N/A') imds3,  
        '                         0 qtysold, 0 onhand, 0 onorder, '' vendor,                         
        '                        impc2, qt tquote, coalesce(catprc,coalesce(kopric,0)) imprc, z.TQ   
        '                        Timesq, '' F20, '' Foem, 0 Ncus, impc1, imcata, (select mindes from 
        '                        qs36f.mincodes where mincod = W.impc2) mindsc, '' vendorname, '' pagent, 0 wlist,
        '                        0 project, '' projstatus, '' catdesc 
        '                        from z left join qs36f.cater on z.wrkptn = catptn 
        '                        left join qs36f.inmsta W on z.wrkptn = W.imptn    
        '                        left join qs36f.komat on z.wrkptn = koptno 
        '                        where z.wrkptn not in (select dvpart from qs36f.dvinva where dvlocn in ('01', '05', '07','26')) "

#End Region

        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            dt = New DataTable()
            'result = objDatos.GetOdBcDataFromDatabase(resultQuery, dsOut, dt)
            result = objDatos.GetDataFromDatabase(resultQuery, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetTotalClients(sql As String) As String
        Dim exMessage As String = Nothing
        Dim result As String = Nothing
        Try
            Dim objDatos = New ClsRPGClientHelper()
            result = objDatos.GetSingleDataScalar(sql)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetTotalCountries(sql As String) As String
        Dim exMessage As String = Nothing
        Dim result As String = Nothing
        Try
            Dim objDatos = New ClsRPGClientHelper()
            result = objDatos.GetSingleDataScalar(sql)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetOEMPart(sql As String) As String
        Dim exMessage As String = Nothing
        Dim result As String = Nothing
        Try
            Dim objDatos = New ClsRPGClientHelper()
            result = objDatos.GetSingleDataScalar(sql)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetValidUsers(dpto As String, ByRef dsOut As DataSet) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Try
            Dim sql = "SELECT * FROM QS36F.CSUSER WHERE TRIM(DECODE) = '" + dpto + "' AND TRIM(USPTY9) = ''"
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(sql, dsOut, dt)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

#End Region

#Region "Wish List"

    Public Function getAllPurcUsers(ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()

            Dim Sql = "select ususer from qs36f.csuser  where decode = 2 and uspty9 <> 'R' and uspty8='X'"
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            result = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetWishListDataByUser(userSql As String, ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        Dim result As Integer = -1
        'messageOut = Nothing
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "SELECT distinct A3.WHLCODE, DATE(A3.WHLDATE) WHLDATE, A3.WHLUSER, A3.WHLSTATUSU,ucase(A3.WHLCOMMENT) a3comment,A7.IPVNUM VENDOR,
                    y.IMPTN, y.IMDSC,y.IMPC1, y.IMPC2,y.IMCATA,y.IMSBCA,y.IMPRC, case when y.IMMOD <> ' ' then y.IMMOD else 'N/A' end IMMOD,                    
                    CASE A3.WHLSTATUS WHEN '1' THEN 'OPEN' WHEN '2' THEN 'DOCUMENTATION' WHEN '3' THEN 'TO DEVELOP' WHEN '4' THEN 'RE-OPEN' WHEN '5' THEN 'MOVED TO DEV' WHEN '6' THEN 'REJECTED' END  WHLSTATUS, 
                    coalesce((SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = Y.IMCATA), '') IMCATA1, A7.IPYSLS qtysold,A7.IPQQTE QTYQTE,A7.IPTQTE TIMESQ,
                    (select DVLOCN from qs36f.dvinva where dvpart = A3.WHLPARTN and DVLOCN = '20') loc20flag, (select imsbca from qs36f.inmsta where imptn = y.imptn) SUBCAT,'' as LOC20,
                    case A3.WHLFROM when '1' THEN 'LS' WHEN '2' THEN 'VNDL' WHEN '3' THEN 'MAN' WHEN '4' THEN 'EXC' WHEN ' ' THEN 'N/A' END WHLFROM,
                    '' as WHLFROM, 
                    (select usname from qs36f.csuser where uspurc = A8.vm#poy fetch first 1 row only) PA, 
                    (select usname from qs36f.csuser where uspurc = A8.vmabb# fetch first 1 row only) PS,
                    coalesce((select vmname from qs36f.vnmas where vmvnum = A8.vmvnum ), '')  vendorname,
                    (select mindes from qs36f.mincodes where mincod = y.IMPC2) minordesc,
                    (select indess from qs36f.inmcas where insbca = y.IMSBCA) subcatdesc
                    FROM 
                    ( SELECT  IMPTN, IMDSC, IMPC1,IMPC2,IMCATA,IMSBCA,IMMOD, case when IMPRC <> 0 then cast(round(IMPRC,2) as decimal(10,2)) else 0 end IMPRC    
                    FROM qs36f.WHLINMSTAJ A1 UNION SELECT  WHLPARTN, WHLADDDESC, WHLADDMAJO, WHLADDMINO, WHLADDCATE, WHLADDSUBC, WHLADDMODE, 
                    case when WHLADDPRIC <> 0 then cast(round(WHLADDPRIC,2) as decimal(10,2)) else 0 end IMPRC                       
                    FROM qs36f.WHLADDINMJ A2 ) y left JOIN qs36f.PRDWL A3 on y.IMPTN = A3.WHLPARTN LEFT JOIN qs36f.invptyf A4 on y.IMPTN = A4.IPPART 
                    left join qs36f.dvinva A5 on y.IMPTN = A5.DVPART left join qs36f.inmsta A6 on y.IMPTN = A6.IMPTN left join qs36f.invptyf A7 on y.IMPTN = A7.IPPART
                    left join qs36f.vnmas A8 on right('000000'||trim(A7.ipvnum),6) = A8.vmvnum where A3.WHLSTATUS <> '5' 
                    and A3.whlpartn not in (select prdptn from qs36f.prdvld)  {0}
                    ORDER BY A3.WHLCODE ASC  "

        Dim resultQuery = String.Format(query, userSql)

        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(resultQuery, dsOut, dt, messageOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            Throw ex
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetWishListData(ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        Dim result As Integer = -1
        'messageOut = Nothing
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "SELECT distinct A3.WHLCODE, DATE(A3.WHLDATE) WHLDATE, A3.WHLUSER, A3.WHLSTATUSU,ucase(A3.WHLCOMMENT) a3comment,A7.IPVNUM VENDOR,
                    y.IMPTN, y.IMDSC,y.IMPC1, y.IMPC2,y.IMCATA,y.IMSBCA,y.IMPRC, case when y.IMMOD <> ' ' then y.IMMOD else 'N/A' end IMMOD,                    
                    CASE A3.WHLSTATUS WHEN '1' THEN 'OPEN' WHEN '2' THEN 'DOCUMENTATION' WHEN '3' THEN 'TO DEVELOP' WHEN '4' THEN 'RE-OPEN' WHEN '5' THEN 'MOVED TO DEV' WHEN '6' THEN 'REJECTED' END  WHLSTATUS, 
                    coalesce((SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = Y.IMCATA), '') IMCATA1, A7.IPYSLS qtysold,A7.IPQQTE QTYQTE,A7.IPTQTE TIMESQ,
                    (select DVLOCN from qs36f.dvinva where dvpart = A3.WHLPARTN and DVLOCN = '20') loc20flag, (select imsbca from qs36f.inmsta where imptn = y.imptn) SUBCAT,'' as LOC20,
                    case A3.WHLFROM when '1' THEN 'LS' WHEN '2' THEN 'VNDL' WHEN '3' THEN 'MAN' WHEN '4' THEN 'EXC' WHEN ' ' THEN 'N/A' END WHLFROM,
                    '' as WHLFROM, 
                    (select usname from qs36f.csuser where uspurc = A8.vm#poy fetch first 1 row only) PA, 
                    (select usname from qs36f.csuser where uspurc = A8.vmabb# fetch first 1 row only) PS,
                    coalesce((select vmname from qs36f.vnmas where vmvnum = A8.vmvnum ), '')  vendorname,
                    (select mindes from qs36f.mincodes where mincod = y.IMPC2) minordesc,
                    (select indess from qs36f.inmcas where insbca = y.IMSBCA) subcatdesc
                    FROM 
                    ( SELECT  IMPTN, IMDSC, IMPC1,IMPC2,IMCATA,IMSBCA,IMMOD, case when IMPRC <> 0 then cast(round(IMPRC,2) as decimal(10,2)) else 0 end IMPRC    
                    FROM qs36f.WHLINMSTAJ A1 UNION SELECT  WHLPARTN, WHLADDDESC, WHLADDMAJO, WHLADDMINO, WHLADDCATE, WHLADDSUBC, WHLADDMODE, 
                    case when WHLADDPRIC <> 0 then cast(round(WHLADDPRIC,2) as decimal(10,2)) else 0 end IMPRC                       
                    FROM qs36f.WHLADDINMJ A2 ) y left JOIN qs36f.PRDWL A3 on y.IMPTN = A3.WHLPARTN LEFT JOIN qs36f.invptyf A4 on y.IMPTN = A4.IPPART 
                    left join qs36f.dvinva A5 on y.IMPTN = A5.DVPART left join qs36f.inmsta A6 on y.IMPTN = A6.IMPTN left join qs36f.invptyf A7 on y.IMPTN = A7.IPPART
                    left join qs36f.vnmas A8 on right('000000'||trim(A7.ipvnum),6) = A8.vmvnum where A3.WHLSTATUS <> '5'
                    and A3.whlpartn not in (select prdptn from qs36f.prdvld)
                    ORDER BY A3.WHLCODE ASC  "
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(query, dsOut, dt, messageOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            Throw ex
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetCustomDataForWishListObj(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Sql = "SELECT distinct A3.WHLCODE, DATE(A3.WHLDATE) WHLDATE, A3.WHLUSER, A3.WHLSTATUSU, ucase(A3.WHLCOMMENT) a3comment,
            CASE A3.WHLSTATUS WHEN '1' THEN 'OPEN' WHEN '2' THEN 'DOCUMENTATION' WHEN '3' THEN 'TO DEVELOP' WHEN '4' THEN 'RE-OPEN' WHEN '5' THEN 'MOVED TO DEV' WHEN '6' THEN 'REJECTED' END  WHLSTATUS, 
            case A3.WHLFROM when '1' THEN 'LS' WHEN '2' THEN 'VNDL' WHEN '3' THEN 'MAN' WHEN '4' THEN 'EXC' WHEN ' ' THEN 'N/A' END WHLFROM,
            y.IMPTN, y.IMDSC,y.IMPC1, y.IMPC2,y.IMCATA,y.IMSBCA,y.IMPRC, 
            case when y.IMMOD <> ' ' then y.IMMOD else 'N/A' end IMMOD,
            coalesce((SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = Y.IMCATA), '') IMCATA1,  
            (select imsbca from qs36f.inmsta where imptn = y.imptn) SUBCAT,
            (select mindes from qs36f.mincodes where mincod = y.IMPC2) minordesc,
            (select indess from qs36f.inmcas where insbca = y.IMSBCA) subcatdesc
            FROM 
            ( SELECT  IMPTN, IMDSC, IMPC1,IMPC2,IMCATA,IMSBCA,IMMOD, case when IMPRC <> 0 then cast(round(IMPRC,2) as decimal(10,2)) else 0 end IMPRC    
            FROM qs36f.WHLINMSTAJ A1 UNION SELECT  WHLPARTN, WHLADDDESC, WHLADDMAJO, WHLADDMINO, WHLADDCATE, WHLADDSUBC, WHLADDMODE, 
            case when WHLADDPRIC <> 0 then cast(round(WHLADDPRIC,2) as decimal(10,2)) else 0 end IMPRC   
            FROM qs36f.WHLADDINMJ A2 ) y left JOIN qs36f.PRDWL A3 on y.IMPTN = A3.WHLPARTN 
            where A3.whlpartn = '" & Trim(UCase(partNo)) & "' ORDER BY A3.WHLCODE ASC"

            'Sql = "select * from prdwl where TRIM(UCASE(WHLPARTN)) = '" & Trim(UCase(partNo)) & "'"
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            result = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetAllDataPartInWishList(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dsOut = New DataSet()
            Dim dt As DataTable = New DataTable()

            Sql = "select * from qs36f.prdwl where TRIM(UCASE(WHLPARTN)) = '" & Trim(UCase(partNo)) & "'"
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            result = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetPartInWishList(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "select WHLPARTN from qs36f.prdwl where TRIM(UCASE(WHLPARTN)) = '" & Trim(UCase(partNo)) & "'"
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            result = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetReferencesInProject(projectCode As Integer) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim amount As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "Select COUNT(PRHCOD) FROM qs36f.PRDVLD WHERE PRHCOD = " & projectCode
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            result = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'ds = GetDataFromDatabase(Sql)
            dsResult = dsOut
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then
                    amount = CInt(dsResult.Tables(0).Rows(0).ItemArray(0).ToString())
                End If
            End If
            Return amount
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return amount
        End Try
    End Function

    Public Function GetAllWLStatus(ByRef Optional messageOut As String = Nothing) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Sql = "select distinct whlstatus from qs36f.prdwl where whlstatus <> ' '"
            result = objDatos.GetDataFromDatabase(Sql, dsResult, dt, messageOut)

            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetAllWLFrom() As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Sql = "select distinct whlfrom from qs36f.prdwl where whlfrom <> ' '"
            result = objDatos.GetDataFromDatabase(Sql, dsResult, dt)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function InsertWishListReference(maxItem As String, userId As String, partNo As String, status As String, from As String) As Integer
        Dim exMessage As String = " "
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Sql = "INSERT INTO qs36f.PRDWL(WHLCODE,WHLUSER,WHLDATE,WHLPARTN,WHLREASONT,WHLCOMMENT,WHLSTATUS,WHLSTATUSD,WHLSTATUSU,WHLFROM,WHLMOUSER,WHLMODATE)
				VALUES(" & maxItem & ",'" & userId & "','" & Format(Now, "yyyy-MM-dd") & "','" & Trim(UCase(partNo)) & "','1','N/A', '" & status & "','" & Format(Now, "yyyy-MM-dd") & "', 
                        '" & userId & "', '" & from & "', '" & userId & "', '" & Format(Now, "yyyy-MM-dd") & "')"
            objDatos.InsertDataInDatabase(Sql, affectedRows)
            'Dim affectedRows = objDatos.InsertOdBcDataToDatabase(Sql)

            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

    'pending
    Public Function UpdateWishListTwoReferences(maxItem As String, partNo As String, status As String, user As String) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()

            Sql = "UPDATE qs36f.PRDWL SET WHLSTATUS = '" & status & "',WHLSTATUSU = '" & user & "' WHERE WHLCODE  = " & maxItem & " AND WHLPARTN = " & partNo & "  "

            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            'Dim affectedRows = objDatos.UpdateOdBcDataToDatabase(Sql)
            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

    Public Function UpdateWishListGenericReference(maxItem As String, user As String, status As String, partNo As String) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()

            Sql = "UPDATE qs36f.PRDWL SET WHLSTATUS = '" & status & "',WHLSTATUSU = '" & user & "' WHERE WHLCODE  = " & maxItem & " and WHLPARTN = '" & partNo & "' "

            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            ' Dim affectedRows = objDatos.UpdateOdBcDataToDatabase(Sql)
            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

    Public Function UpdateWishListGenericReferenceByConcatValues(maxItem As String, user As String, status As String, str As String) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()

            If Not String.IsNullOrEmpty(str) Then
                Sql = "UPDATE qs36f.PRDWL SET WHLSTATUS = '" & status & "',WHLSTATUSU = '" & user & "' WHERE WHLCODE  = " & maxItem & " and " & str & " "
            Else
                Sql = "UPDATE qs36f.PRDWL SET WHLSTATUS = '" & status & "',WHLSTATUSU = '" & user & "' WHERE WHLCODE  = " & maxItem & " "
            End If

            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            ' Dim affectedRows = objDatos.UpdateOdBcDataToDatabase(Sql)
            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

    Public Function UpdateWishListSingleReference(maxItem As String, user As String, status As String, comment As String) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()

            Sql = "UPDATE qs36f.PRDWL SET WHLSTATUS = '" & status & "',WHLSTATUSU = '" & user & "',WHLCOMMENT= '" & comment & "' WHERE WHLCODE  = " & maxItem & " "

            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            'Dim affectedRows = objDatos.UpdateOdBcDataToDatabase(Sql)
            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

    Public Function DeletePDHeader(projectNo As String, ByRef affectedRows As Integer) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        affectedRows = -1

        Try
            Dim objDatos = New ClsRPGClientHelper()

            Sql = "DELETE FROM qs36f.PRDVLH WHERE PRHCOD = '" & projectNo & "' "
            'Sql = "UPDATE PRDWL SET WHLSTATUS = '" & status & "',WHLSTATUSU = '" & user & "',WHLCOMMENT= '" & comment & "' WHERE WHLCODE  = " & maxItem & " "

            objDatos.DeleteRecordFromDatabase(Sql, affectedRows)
            Return affectedRows

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return affectedRows
        End Try
    End Function

    Public Function UpdateWSHUserComment(partNo As String, comment As String) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()

            Sql = "UPDATE qs36f.LOSTSALBCK SET commentrs = '" & comment & "' WHERE trim(IMPTN) = '" & partNo & "' "

            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            'Dim affectedRows = objDatos.UpdateOdBcDataToDatabase(Sql)
            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

    Public Function GetWSHUserComment(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT commentrs FROM qs36f.LOSTSALBCK where imptn = '" & Trim(UCase(partNo)) & "'"
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            result = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

#Region "Product Development"

#Region "Inserts"

    Public Function InsertNewProject(projectno As String, userid As String, dtValue As Date, strInfo As String, strName As String, ddlStatus As String, strUser As String) As Integer
        Dim exMessage As String = " "
        Dim Sql As String
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Sql = "INSERT INTO qs36f.PRDVLH(PRHCOD,CRUSER,CRDATE,PRDATE,PRINFO,PRNAME,PRSTAT,MOUSER,MODATE,PRPECH) VALUES 
            (" & projectno & ",'" & userid & "','" & Format(Now, "yyyy-MM-dd") & "','" & Format(dtValue, "yyyy-MM-dd") & "',
            '" & Trim(strInfo) & "', '" & Trim(strName) & "','" & Left(ddlStatus, 1) & "','" & userid & "',
            '" & Format(Now, "yyyy-MM-dd") & "','" & Left(Trim(strUser), 10) & "')"
            objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try
    End Function

    Public Function InsertProductDetail(projectno As String, partNo As String, dtValue As Date, userid As String, dtValue1 As Date, userid1 As String, dtValue2 As Date, ctpNo As String, qty As String,
                                        mfr As String, mfrNo As String, unitCost As String, unitCostNew As String, poNo As String, dtValue3 As Date, ddlStatus As String, benefits As String,
                                        comments As String, ddlUser As String, dtValue4 As Date, sampleCost As String, miscCost As String, vendorNo As String,
                                        partsToShow As String, ddlMinorCode As String, toolingCost As String, dtValue5 As Date, dtValue6 As Date, sampleQty As String) As Integer
        Dim exMessage As String = " "
        Dim Sql As String
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim chkSelection As Integer = If(getValueCheckTab3(vendorNo, partNo) = -1, 0, 1)

            Sql = "INSERT INTO qs36f.PRDVLD(PRHCOD,PRDPTN,PRDDAT,CRUSER,CRDATE,MOUSER,MODATE,PRDCTP,PRDQTY,PRDMFR,PRDMFR#,PRDCOS,PRDCON,PRDPO#,PODATE,PRDSTS,PRDBEN,PRDINF,PRDUSR,PRDNEW,
                                        PRDEDD,PRDSCO,PRDTTC,VMVNUM,PRDPTS,PRDMPC,PRDTCO,PRDERD,PRDPDA,PRDSQTY) 
                   VALUES (" & projectno & ",'" & Trim(UCase(partNo)) & "','" & Format(dtValue, "yyyy-MM-dd") & "','" & userid & "','" & Format(dtValue1, "yyyy-MM-dd") & "',
                    '" & userid & "','" & Format(dtValue2, "yyyy-MM-dd") & "','" & Trim(ctpNo) & "'," & qty & ",'" & Trim(mfr) & "','" & Trim(mfrNo) & "'," & (unitCost) & ",
                    " & (unitCostNew) & ",'" & Trim(poNo) & "','" & Format(dtValue3, "yyyy-MM-dd") & "','" & Trim(ddlStatus) & "','" & Trim(benefits) & "','" & Trim(comments) & "',
                    '" & Trim(ddlUser) & "'," & chkSelection & ",'" & Format(dtValue4, "yyyy-MM-dd") & "'," & sampleCost & "," & miscCost & "," & Trim(vendorNo) & ",'" & partsToShow & "',
                    '" & (ddlMinorCode) & "'," & toolingCost & ",'" & Format(dtValue5, "yyyy-MM-dd") & "', '" & Format(dtValue6, "yyyy-MM-dd") & "' ," & sampleQty & ")"

            objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try
    End Function

    Public Function InsertNewPOQota(partNo As String, vendorNo As String, maxValue As String, strYear As String, strMonth As String, mpnPo As String, strDay As String,
                                    strStsQuote As String, strSpace As String, strUnitCostNew As String, strMinQty As String) As Integer
        Dim exMessage As String = " "
        Dim Sql As String
        Dim affectedRows As Integer = -1
        Dim maxLength As Integer = 20
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim statusquoteNew = If(String.IsNullOrEmpty(strStsQuote), strStsQuote, If(strStsQuote.Length < maxLength, strStsQuote, strStsQuote.Substring(0, Math.Min(strStsQuote.Length, maxLength))))

            Sql = "INSERT INTO qs36f.POQOTA (PQPTN,PQVND,PQSEQ,PQQDTY,PQQDTM,PQMPTN,PQQDTD,PQCOMM,SPACE,PQPRC,PQMIN) VALUES 
            ('" & Trim(UCase(partNo)) & "'," & Trim(vendorNo) & "," & maxValue & "," & strYear.Substring(strYear.Length - 2) & ",
            " & strMonth & ",'" & mpnPo & "'," & strDay & ",'" & statusquoteNew & "','" & strSpace & "'," & strUnitCostNew & "," & strMinQty & ")"
            objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try
    End Function

#End Region

#Region "Updates"

    Public Function UpdatePoQoraRow(mpnopo As String, minQty As String, unitCostNew As String, statusquote As String, insertYear As String, insertMonth As String, insertDay As String,
                                    vendorNo As String, partNo As String) As Integer
        Dim exMessage As String = " "
        Dim Sql As String
        Dim affectedRows As Integer = -1
        Dim maxLength As Integer = 20
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim statusquoteNew = If(String.IsNullOrEmpty(statusquote), statusquote, If(statusquote.Length < maxLength, statusquote, statusquote.Substring(0, Math.Min(statusquote.Length, maxLength))))

            Sql = "UPDATE qs36f.POQOTA SET PQMPTN = '" & mpnopo & "',PQMIN  = " & minQty & ",PQPRC  = " & unitCostNew & ",PQCOMM = '" & statusquoteNew & "',
                PQQDTY =  " & insertYear.Substring(insertYear.Length - 2) & " ,PQQDTM = " & insertMonth & " ,PQQDTD = " & insertDay & " 
                WHERE PQVND  = " & Trim(vendorNo) & " AND PQPTN  = '" & Trim(UCase(partNo)) & "' AND SUBSTR(UCASE(SPACE),32,3) = 'DEV' " &
                " AND PQCOMM LIKE 'D%'"
            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try
    End Function

#End Region

#Region "Generics"

    Public Function GetVendorsInProject(projectNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "select distinct(vmvnum) from qs36f.prdvld where prhcod= '" & Trim(UCase(projectNo)) & "' order by 1 desc  "
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetProjectData(projectNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "select * from qs36f.prdvlh where prhcod = '" & Trim(UCase(projectNo)) & "' order by 1 desc  "
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function getValueCheckTab3(vendorNo As String, partno As String)
        Dim exMessage As String = " "
        Dim VendorWhiteFlagMethod = ConfigurationManager.AppSettings("itemCategories")
        Try
            Dim listItemCat = VendorWhiteFlagMethod.Split(",")

            Dim dsResult1 = getItemCategoryByVendorAndPart(vendorNo, partno)
            If dsResult1 IsNot Nothing Then
                If dsResult1.Tables(0).Rows.Count > 0 Then
                    For Each item As String In listItemCat
                        If Trim(item).Equals(Trim(vendorNo)) Then
                            Return 1
                        End If
                    Next
                    Return -1
                Else
                    Return 1
                End If
            Else
                Return 1
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return -1
        End Try
    End Function

    Public Function getItemCategoryByVendorAndPart(vendorNo As String, partNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1

        Dim objDatos = New ClsRPGClientHelper()
        Dim dt As DataTable = New DataTable()
        Dim dsOut = New DataSet()
        Dim Sql = "SELECT PRHCOD FROM qs36f.PRDVLD WHERE VMVNUM = " & Trim(vendorNo) & " And trim(ucase(PRDPTN)) = '" & Trim(UCase(partNo)) & "'"
        Try
            'affectedRows = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            If dsOut IsNot Nothing Then
                Return dsOut
            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetProjectStatusDescription(code As String) As String
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ProjectDescStatus As String = " "
        Dim columnToChange = "CNTDE1"
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim CodeOk As String = Trim(UCase(code))
            Sql = "SELECT CNTDE1 FROM qs36f.cntrll where cnt01 = 'DSI' and cnt03 = '" & CodeOk & "'"
            ProjectDescStatus = objDatos.GetSingleDataScalar(Sql)
            'ProjectDescStatus = ds.Tables(0).Rows(0).ItemArray(0).ToString()
            Return Trim(ProjectDescStatus)
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetCodeAndNameByPartNo(partNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT A1.PRHCOD,A1.PRNAME,A2.VMVNUM FROM qs36f.PRDVLH A1 INNER JOIN qs36f.PRDVLD A2 ON A1.PRHCOD = A2.PRHCOD WHERE TRIM(A2.PRDPTN) = '" & Trim(UCase(partNo)) & "' ORDER BY A2.CRDATE DESC"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function CheckIfreferenceExistsinProj(code As String, partNo As String, vendorNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT A1.PRHCOD,A1.PRNAME,A2.VMVNUM FROM qs36f.PRDVLH A1 INNER JOIN qs36f.PRDVLD A2 ON A1.PRHCOD = A2.PRHCOD where A1.PRHCOD = '" & Trim(code) & "' and TRIM(A2.PRDPTN) = '" & Trim(UCase(partNo)) & "' and A2.vmvnum = " & Trim(vendorNo) & " ORDER BY A2.CRDATE DESC"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetCodeAndNameByPartNoAndVendorNo(partNo As String, vendorNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT A1.PRHCOD,A1.PRNAME,A2.VMVNUM FROM qs36f.PRDVLH A1 INNER JOIN qs36f.PRDVLD A2 ON A1.PRHCOD = A2.PRHCOD where TRIM(A2.PRDPTN) = '" & Trim(UCase(partNo)) & "' and A2.vmvnum = " & Trim(vendorNo) & " ORDER BY A2.CRDATE DESC"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetPOQotaData(vendorNo As String, partNo As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT PQMPTN,PQPRC,PQSEQ,PQPTN,PQVND,PQMIN,PQCOMM FROM qs36f.POQOTA WHERE PQVND = " & Trim(vendorNo) & " AND PQPTN = '" & Trim(UCase(partNo)) & "' AND SUBSTR(UCASE(SPACE),32,3) = 'DEV' 
                    AND PQCOMM LIKE 'D%' ORDER BY PQQDTY DESC, PQQDTM DESC, PQQDTD DESC"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetCTPPartRef(partNo As String) As String
        Dim exMessage As String = " "
        Dim Sql As String
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Sql = "SELECT CRCTPR FROM qs36f.CTPREFS WHERE TRIM(UCASE(CRPTNO)) = '" & Trim(UCase(partNo)) & "'"
            Dim str = objDatos.GetSingleDataScalar(Sql)
            Return str
        Catch ex As Exception
            exMessage = ex.ToString() + ". " + ex.Message + ". " + ex.ToString()
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function getVendorTypeByVendorNum(vendorNo As String, Optional ByVal flag As Integer = 0) As Data.DataSet
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim ds = New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1

        Dim objDatos = New ClsRPGClientHelper()
        Dim dt As DataTable = New DataTable()
        Dim dsOut = New DataSet()
        Dim Sql = "select vmvtyp, vmname from qs36f.vnmas where vmvnum = " & vendorNo & " "
        Try
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            If dsOut IsNot Nothing Then
                Return dsOut
            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function getOEMVendorCodes(cntrCode As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1

        Dim objDatos = New ClsRPGClientHelper()
        Dim dt As DataTable = New DataTable()
        Dim dsOut = New DataSet()
        Dim Sql = "select CNTDE1 from qs36f.cntrll where cnt01 = " & cntrCode & " "
        Try
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            If dsOut IsNot Nothing Then
                Return dsOut
            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

#End Region

#End Region

#End Region

#Region "Claims"

    Public Function GetClaimsDataSingle(ByRef dsResult As DataSet, Optional ByVal strDates As String() = Nothing) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "SELECT distinct VARCHAR_FORMAT(A1.MHMRNR)ClAIM#, (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '185' and cnt02 = '' and cnt03 = A1.MHRTTY) TYPE,
                    (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '188' and cnt02 = '' and cnt03 = A1.MHREASN) REASON,
                    (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '189' and cnt02 = '' and cnt03 = A1.MHDIAG) DIAGNOSE, 
                    A1.MHCUNR CUSTOMER, CTPINV.CVTDCDTF(A1.MHMRDT,'MDY') INITDATE,
                    (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '186' and cnt02 = '' and trim(cnt03) = trim(A1.MHSTAT)) EXTSTATUS, 
                    A2.CWPTNO PART#, A2.CWCQTY QTY, A2.CWUNCS UNITPR, A3.INCLNO, A1.MHUSER usr 
                    FROM qs36f.CSMREH A1, qs36f.CLMWRN A2, qs36f.CLMINTSTS A3 WHERE A1.MHRTTY <> 'B' and A1.MHMRNR = A2.CWDOCN and A2.CWWRNO = A3.INCLNO and 
                    CTPINV.CVTDCDTF(A1.MHMRDT, 'MDY') >= {0} AND CTPINV.CVTDCDTF(A1.MHMRDT,'MDY') <= {1} ORDER BY 1 DESC "
        Try
            'Dim yearUse = DateTime.Now().AddYears(-3).Year
            'Dim firstDate = New DateTime(yearUse, 1, 1).Date()
            'Dim strDateFirst As String = firstDate.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            ''Dim strDateReduc As String = firstDate.ToString("yyMM", System.Globalization.CultureInfo.InvariantCulture)
            'Dim curDate = DateTime.Now.Date().ToString("MM/dd/yyyy")

            Dim newQuery = String.Format(query, strDates(0), strDates(1))

            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            result = objDatos.GetOdBcDataFromDatabase(newQuery, dsOut)
            dsResult = dsOut
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetClaimsDataFull(ByRef dsResult As DataSet, Optional ByVal strDates As String() = Nothing) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query As String = "SELECT VARCHAR_FORMAT(A1.MHMRNR)ClAIM#, (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '185' and cnt02 = '' and cnt03 = A1.MHRTTY) TYPE,
                                (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '188' and cnt02 = '' and cnt03 = A1.MHREASN) REASON,
                                (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '189' and cnt02 = '' and cnt03 = A1.MHDIAG) DIAGNOSE, 
                                A1.MHCUNR CUSTOMER, CTPINV.CVTDCDTF(A1.MHMRDT,'MDY') INITDATE,
                                (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '186' and cnt02 = '' and trim(cnt03) = trim(A1.MHSTAT)) EXTSTATUS, 
                                (select substr(cntde1,1,25) from qs36f.cntrll where cnt01 = '193' and cnt02 = '' and cnt03=A3.INSTAT) INTSTATUS,
                                A2.CWPTNO PART#, A2.CWCQTY QTY, A2.CWUNCS UNITPR, A3.INCLNO,                                  
                                (CASE WHEN A3.INTAPPRV = 'L' THEN 'APPROVED' WHEN A3.INTAPPRV = 'M' THEN 'DENIED' ELSE ' ' END) AS APP, 
                                A3.INUSER USER, A3.INCLDT DATECM, A3.INDESC DESCRIPTION, A1.MHSCO1 COMMENT1,A1.MHSCO2 COMMENT2,A1.MHSCO3 COMMENT3,A1.MHSUBBY SUBMITTEDBY,A2.CWINVC INVOICENO
                                FROM qs36f.CSMREH A1, qs36f.CLMWRN A2, qs36f.CLMINTSTS A3 WHERE A1.MHRTTY <> 'B' and A1.MHMRNR = A2.CWDOCN and A2.CWWRNO = A3.INCLNO and 
                                CTPINV.CVTDCDTF(A1.MHMRDT, 'MDY') >= {0} AND CTPINV.CVTDCDTF(A1.MHMRDT,'MDY') <= {1} ORDER BY A1.MHMRNR DESC"
        Try
            'Dim yearUse = DateTime.Now().AddYears(-3).Year
            'Dim firstDate = New DateTime(yearUse, 1, 1).Date()
            'Dim strDateFirst As String = firstDate.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            ''Dim strDateReduc As String = firstDate.ToString("yyMM", System.Globalization.CultureInfo.InvariantCulture)
            'Dim curDate = DateTime.Now.Date().ToString("MM/dd/yyyy")

            Dim newQuery = String.Format(query, strDates(0), strDates(1))

            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            result = objDatos.GetOdBcDataFromDatabase(newQuery, dsOut)
            'result = objDatos.GetDataFromDatabase(query, dsOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetAutoCompleteDataPartNo(prefixText As String, ByRef dsResult As DataSet) As Integer
        Dim lstResult = New List(Of String)
        Dim exMessage As String = Nothing
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1

        Try
            Dim objDatos = New ClsRPGClientHelper()
            'Dim Sql = " SELECT DISTINCT (CLMWRN.CWPTNO) PART# FROM CSMREH, CLMWRN, CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
            '        and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
            '        and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
            '        and VARCHAR_FORMAT(CLMWRN.CWPTNO) LIKE '%{2}%' ORDER BY CLMWRN.CWPTNO DESC 
            '        FETCH FIRST 10 ROWS ONLY"
            Dim Sql = "select CATPTN, CATDSC from qs36f.cater A1 where A1.catptn = '{0}' union select KOPTNO,KODESC from qs36f.komat where KOPTNO = '{1}'"
            Dim sqlResult = String.Format(Sql, prefixText, prefixText)

            result = objDatos.GetOdBcDataFromDatabase(sqlResult, dsResult)

            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try

    End Function

    Public Function GetAutocompleteSelectedVendorName(prefixVendorName As String, VendorCodesDenied As String, VendorOEMCodeDenied As String, ItemCategories As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Sql = "SELECT VMVNUM, VMNAME, VMVTYP FROM qs36f.VNMAS WHERE VMVTYP NOT IN (" & VendorCodesDenied & ") 
                   AND VMVNUM NOT IN (SELECT CNTDE1 FROM qs36f.CNTRLL WHERE CNT01 IN (" & VendorOEMCodeDenied & "))
                   AND VMVNUM NOT IN (" & ItemCategories & ")
                   AND VMNAME LIKE '%" & Replace(Trim(UCase(prefixVendorName)), "'", "") & "%'
                   ORDER BY VMNAME FETCH FIRST 10 ROWS ONLY"

            result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function getClaimNumbers(claimSelected As String, dateValue As DateTime, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Sql = " SELECT (CSMREH.MHMRNR) ClAIM# FROM qs36f.CSMREH, qs36f.CLMWRN, qs36f.CLMINTSTS WHERE CSMREH.MHRTTY <> 'B' 
                    and CSMREH.MHMRNR = CLMWRN.CWDOCN and CLMWRN.CWWRNO = CLMINTSTS.INCLNO 
                    and CVTDCDTF(CSMREH.MHMRDT, 'MDY') >= '{0}' AND CVTDCDTF(CSMREH.MHMRDT,'MDY') <= '{1}' 
                    and VARCHAR_FORMAT(CSMREH.MHMRNR) LIKE '%{2}%' ORDER BY CSMREH.MHMRNR DESC"

            Dim sqlResult = String.Format(Sql, dateValue.ToString("MM/dd/yyyy"), Today().ToString("MM/dd/yyyy"), claimSelected)
            result = objDatos.GetOdBcDataFromDatabase(sqlResult, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
        End Try
    End Function

#End Region

    Public Function getRightUser(user As String) As Boolean
        Dim exMessage As String = " "
        Try
            Dim objDatos = New ClsRPGClientHelper()

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Function

    Public Function CallForCtpNumber(partno As String, ctppartno As String, flagctp As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "CALL CTPINV.CATCTPR ('" & partno & "','" & ctppartno & "','" & flagctp & "')"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString() + ". " + ex.Message + ". " + ex.ToString()
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function getmax(table As String, field As String, Optional strWhereAdd As String = Nothing) As Integer
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Dim Sql As String = " "
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1

        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Dim strResult = Nothing

            Dim str1 = "Select " & field & " FROM " & table & " ORDER BY " & field & " DESC FETCH FIRST 1 ROW ONLY"
            Dim str2 = "Select " & field & " FROM " & table & " " & strWhereAdd & " ORDER BY " & field & " DESC FETCH FIRST 1 ROW ONLY"
            Sql = If((strWhereAdd IsNot Nothing Or Not String.IsNullOrEmpty(strWhereAdd)), str2, str1)
            'Sql = "Select " & field & " FROM " & table & " ORDER BY " & field & " DESC FETCH FIRST 1 ROW ONLY"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)

            Dim intResult = If(affectedRows <= 0, 0, CInt(dsOut.Tables(0).Rows(0).ItemArray(0).ToString()))
            Return intResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetGridParameterDin() As List(Of String)
        Dim exMessage As String = Nothing
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim totalRow = objDatos.GetRowCount
            Dim pageSize = objDatos.GetPageSize
            Dim lstGridParameter = New List(Of String)()
            lstGridParameter.Add(totalRow)
            lstGridParameter.Add(pageSize)
            Return lstGridParameter
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function FillDDL(query As String, ByRef dsResult As DataSet) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim affectedRows As Integer = -1
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()

            affectedRows = objDatos.GetDataFromDatabase(query, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(query, dsOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function FillDDL1(query As String, ByRef dsResult As DataSet) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim affectedRows As Integer = -1
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()

            affectedRows = objDatos.GetDataFromDatabase(query, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(query, dsOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function


    'Private Function GetData(query As String, ByRef dsResult As DataSet) As Integer
    '    Dim result As Integer = -1
    '    dsResult = New DataSet()
    '    Dim exMessage As String = " "
    '    Try
    '        Dim dsOut = New DataSet()
    '        Dim objDatos = New ClsRPGClientHelper()
    '        result = objDatos.GetOdBcDataFromDatabase(query, dsOut)
    '        dsResult = dsOut
    '        Return result
    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        Return result
    '    End Try
    'End Function

    Public Function GetDataByPRHCOD(query As String, ByRef dsResult As DataSet) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim affectedRows As Integer = -1
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            affectedRows = objDatos.GetDataFromDatabase(query, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(query, dsOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetDataByPartNo(query As String, partNo As String, ByRef strResult As String) As Integer
        Dim result As Integer = -1
        strResult = " "
        Dim exMessage As String = " "
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            result = objDatos.GetSingleDataFromDatabase(query, partNo, strResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetDataByCodeAndPartNo(vendorNo As String, ByRef dsResult As DataSet) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim affectedRows As Integer = -1
        Try
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim query = "SELECT VMNAMEE,VMABB# FROM qs36f.VNMAS WHERE VMVNUM = " & Trim(UCase(vendorNo))
            affectedRows = objDatos.GetDataFromDatabase(query, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(query, dsOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetVendorByVendorNo(vendorNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT * FROM qs36f.VNMAS WHERE VMVNUM = " & Trim(UCase(vendorNo))
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function getUserDataByPurc(purcNumber As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT USNAME FROM qs36f.CSUSER WHERE USPURC = '" & Trim(UCase(purcNumber)) & "'"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetDataFromDev(partNo As String, vendorNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "select A2.prhcod, A2.prdsts from qs36f.prdvlh A1 inner join qs36f.prdvld A2 on A1.prhcod = A2.prhcod where 
                    A2.prdptn = '" & Trim(UCase(partNo)) & "' and A2.vmvnum = " & vendorNo
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetDataFromCatDesc(categoryCode As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT INDESC FROM qs36f.INMCAT WHERE INCATA = '" & Trim(UCase(categoryCode)) & "'"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetAllStatuses() As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT CNT03, CNTDE1 FROM qs36f.cntrll where cnt01 = 'DSI' order by cnt02"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetAllUsers() As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT USUSER, USNAME FROM qs36f.CSUSER fetch first 20 rows only"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            Return dsOut
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetAllPaAndPsUsers(ByRef Optional messageOut As String = Nothing) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Sql = "SELECT A1.CNT03 PA, TRIM(A2.USNAME) FULLNAME, TRIM(A2.USUSER) USER  FROM qs36f.CNTRLL A1 INNER JOIN qs36f.CSUSER A2 ON CNT03 = DIGITS(USPURC)  WHERE CNT01 = '216' AND USPTY9 <> 'R' AND USPURC <> 0"
            result = objDatos.GetDataFromDatabase(Sql, dsResult, dt, messageOut)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetAllMinors(ByRef Optional messageOut As String = Nothing) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        Dim dsResult As New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Sql = "select MINCOD, MINDES from qs36f.mincodes order by 1"
            result = objDatos.GetDataFromDatabase(Sql, dsResult, dt, messageOut)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function GetVendorByNumber(vendorNo As String, VendorCodesDenied As String, VendorOEMCodeDenied As String, ItemCategories As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()
            Sql = "SELECT VMVNUM, VMNAME, VMVTYP FROM qs36f.VNMAS WHERE VMVTYP NOT IN (" & VendorCodesDenied & ") 
                   AND VMVNUM NOT IN (SELECT CNTDE1 FROM qs36f.CNTRLL WHERE CNT01 IN (" & VendorOEMCodeDenied & "))
                   AND VMVNUM NOT IN (" & ItemCategories & ")
                   AND VMVNUM = " & vendorNo & "  ORDER BY VMVNUM FETCH FIRST 10 ROWS ONLY"
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            dsResult = dsOut
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try

    End Function

    Public Function GetNewPartData(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Dim result As Integer = -1
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Dim dsOut = New DataSet()

            Sql = "select CATPTN, CATDSC, CATPRC, 'Cater' CatType from qs36f.cater A1 where A1.catptn = '{0}' union select KOPTNO,KODESC,KOPRIC, 'Komat' KoType from qs36f.komat where KOPTNO = '{1}'"
            Dim sqlResult = String.Format(Sql, partNo, partNo)

            affectedRows = objDatos.GetDataFromDatabase(sqlResult, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(sqlResult, dsResult)
            dsResult = dsOut
            Return affectedRows

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try

    End Function

#Region "Lost Sale Backed Data"

    Public Function GetLSBackData400(partNo As String, ByRef dsResult As DataSet) As Data.DataSet
        Dim exMessage As String = " "
        Dim Sql As String
        Dim result As Integer = -1
        dsResult = New DataSet()
        dsResult.Locale = CultureInfo.InvariantCulture
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            Sql = "select * from qs36f.LOSTSALBCK WHERE IMPTN = '" & Trim(UCase(partNo)) & "' "
            result = objDatos.GetDataFromDatabase(Sql, dsResult, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, dsResult)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function InsertLSBackData400(objLS As LostSales, Optional externalStatus As String = Nothing) As Integer
        Dim exMessage As String = " "
        Dim Sql As String
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim extSts = If(String.IsNullOrEmpty(externalStatus), "NEW", "WSH")
            Sql = "INSERT INTO qs36f.LOSTSALBCK(IMPTN,IMDSC,IMDSC2,IMDSC3,TQUOTE,TIMESQ,NCUS,QTYSOLD,VENDOR,VENDORNAME,PAGENT,IMPRC,WLIST,PROJECT,PROJSTATUS,
		    F20,FOEM,IMPC1,CATDESC,IMPC2,MINDSC,TCOUNTRIES,OEMPART,SUBCATDESC,PERPECH,EXTERNALSTS) VALUES 
            ('" & objLS.IMPTN & "', '" & objLS.IMDSC & "', '" & objLS.IMDS2 & "', '" & objLS.IMDS3 & "', '" & objLS.TQUOTE & "', '" & objLS.TIMESQ & "', '" & objLS.NCUS & "', '" & objLS.QTYSOLD & "'
                    , '" & objLS.VENDOR & "', '" & objLS.VENDORNAME & "', '" & objLS.PAGENT & "', '" & objLS.IMPRC & "', '" & objLS.WLIST & "', '" & objLS.PROJECT & "', '" & objLS.PROJSTATUS & "'
                    , '" & objLS.F20 & "', '" & objLS.FOEM & "', '" & objLS.IMPC1 & "', '" & objLS.CATDESC & "', '" & objLS.IMPC2 & "', '" & objLS.MINDSC & "', '" & objLS.TotalCountries & "', '" & objLS.OEMPart & "'
                    , '" & objLS.SubCatDesc & "', '" & objLS.PrPech & "', '" & extSts & "')"
            objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try
    End Function

    Public Function UpdateLSBackData400(partNo As String, externalStatus As String, Optional user As String = Nothing) As Integer
        Dim exMessage As String = Nothing
        Dim Sql As String
        Dim QueryResult As Integer = -1
        Dim ds = New DataSet()
        Dim affectedRows As Integer = -1
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim optSql = If(String.IsNullOrEmpty(user), "", " ,perpech = '" & user & "'")
            Sql = "UPDATE qs36f.LOSTSALBCK SET EXTERNALSTS = '" & externalStatus & "'" & optSql & " WHERE imptn  = '" & partNo & "' "

            objDatos.UpdateDataInDatabase(Sql, affectedRows)
            ' Dim affectedRows = objDatos.UpdateOdBcDataToDatabase(Sql)
            'objDatos.InsertDataInDatabase(Sql, affectedRows)
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return QueryResult
        End Try
    End Function

#End Region


#Region "PDLogs-SQL"

    Public Function GetPDLogsFromSql(ByRef dsResult As DataSet) As Integer
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim dt As DataTable = New DataTable()
        Dim exMessage As String = " "
        Dim query = "SELECT * FROM dbCTPSystem.dbo.CtpSystemLog"
        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            dt = objDatos.ExecuteQueryStoredProcedure(query, Nothing)

            If dt.Rows.Count > 0 Then
                dsResult.Tables.Add(dt)
                result = dt.Rows.Count
            End If

            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

#End Region

#Region "Sql Backup for lost sale items in process"

    Public Function GetLSBackData(partNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        dsResult = New DataSet()
        Dim dt As DataTable = New DataTable()
        Try
            Dim query = "SELECT * FROM dbo.LOSTSALBCK WHERE IMPTN = '" & Trim(UCase(partNo)) & "' "
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            dt = objDatos.ExecuteQueryStoredProcedure(query, Nothing)

            If dt.Rows.Count > 0 Then
                dsResult.Tables.Add(dt)
                result = dt.Rows.Count
            End If

            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function SaveLSItemInProcess(objLS As LostSales, Optional externalStatus As String = Nothing) As Integer
        Dim exMessage As String = Nothing
        Dim result As Integer = -1
        Dim dsResult = New DataSet()
        Dim dt As DataTable = New DataTable()
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim query = "INSERT INTO dbo.LOSTSALBCK (IMPTN,IMDSC,IMDSC2,IMDSC3,TQUOTE,TIMESQ,NCUS,QTYSOLD,VENDOR,VENDORNAME,PAGENT,IMPRC,WLIST,PROJECT,PROJSTATUS,
		   F20,FOEM,IMPC1,CATDESC,IMPC2,MINDSC,TCOUNTRIES,OEMPART,SUBCATDESC,PERPECH,EXTERNALSTS)
            VALUES('" & objLS.IMPTN & "', '" & objLS.IMDSC & "', '" & objLS.IMDS2 & "', '" & objLS.IMDS3 & "', '" & objLS.TQUOTE & "', '" & objLS.TIMESQ & "', '" & objLS.NCUS & "', '" & objLS.QTYSOLD & "'
                    , '" & objLS.VENDOR & "', '" & objLS.VENDORNAME & "', '" & objLS.PAGENT & "', '" & objLS.IMPRC & "', '" & objLS.WLIST & "', '" & objLS.PROJECT & "', '" & objLS.PROJSTATUS & "'
                    , '" & objLS.F20 & "', '" & objLS.FOEM & "', '" & objLS.IMPC1 & "', '" & objLS.CATDESC & "', '" & objLS.IMPC2 & "', '" & objLS.MINDSC & "', '" & objLS.TotalCountries & "', '" & objLS.OEMPart & "'
                    , '" & objLS.SubCatDesc & "', '" & objLS.PrPech & "', 'NEW' )"

            objDatos.ExecuteNotQueryCommand1(query, result)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

#End Region


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
