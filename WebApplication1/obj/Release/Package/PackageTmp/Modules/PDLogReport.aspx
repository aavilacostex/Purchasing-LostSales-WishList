<%@ Page Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="PDLogReport.aspx.vb" Inherits="WebApplication1.PDLogReport" ViewStateMode="Disabled" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="updatepnl" runat="server"> 
        <ContentTemplate>                      

            <div class="container-fluid">
                <div class="breadcrumb-area breadcrumb-bg">
                    <div class="row custom-padding" >
                        <div class="col-md-offset-4 col-md-8 center">
                            <div class="breadcrumb-inner">
                                <div class="row">
                                    <div class="col-md-11">
                                        <div class="bread-crumb-inner">
                                            <div class="breadcrumb-area page-list">
                                                <div class="row">
                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-7 link">
                                                        <i class="fa fa-map-marker"></i>
                                                        <a href="/Default">Home</a>
                                                        " - "
                                                    <span>PRODUCT DEVELOPMENT LOG DATA</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="row">
                                <div class="col-md-3 hideProp">
                                    <asp:LinkButton ID="btnNewItem" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-plus fa-1x"" aria-hidden="true"> </i> NEW ITEM
                                                        </asp:LinkButton>
                                </div>
                                <div class="col-md-3 hideProp">
                                    <asp:LinkButton ID="btnImportExcel" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-file-excel-o fa-1x" aria-hidden="true"> </i> IMPORT
                                                        </asp:LinkButton>
                                </div>
                                <div class="col-md-3 hideProp">
                                    <asp:LinkButton ID="btnImportFromLs" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-list-alt fa-1x" aria-hidden="true"> </i> FROM LS
                                                        </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="showActionsSection" class="container" runat="server">          
                <div id="rowFilters" class="row">                    
                    <%--<div id="col1-custom" class="col-md-1"></div>--%>
                    <div class="col-md-6">                        
                        <div class="accordion-wrapper">
                            <div id="accordion_3">
                                <div class="card">
                                    <div class="card-header" id="headingOne_3">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_3" aria-expanded="false" aria-controls="collapseOne_3">
                                                <span class="">FILTERS  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>
                                    <div id="collapseOne_3" class="collapse show" aria-labelledby="headingOne_3" data-parent="#accordion_3" style="">
                                        <div class="card-body">
                                            <div id="rowRadios1" class="form-group col-md-12">
                                                <div class="row">
                                                    <div class="form-group col-md-6 radio-toolbar">
                                                        <label class="form-check">
                                                            <p>User</p>
                                                            <asp:RadioButton ID="rdUser" OnCheckedChanged="rdUser_CheckedChanged" onclick="yesnoCheck('rowUser1');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                            <span class="checkmark"></span>
                                                        </label>
                                                    </div>

                                                    <div class="form-group col-md-6 radio-toolbar">
                                                        <label class="form-check">
                                                            <p>Type</p>
                                                            <asp:RadioButton ID="rdType" OnCheckedChanged="rdType_CheckedChanged" onclick="javascript:yesnoCheck('rowType');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                            <span class="checkmark"></span>
                                                        </label>
                                                    </div>
                                                </div> 
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="accordion-wrapper">
                            <div id="accordion_4">
                                <div class="card">
                                    <div class="card-header" id="headingOne_4">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_4" aria-expanded="false" aria-controls="collapseOne_4">
                                                <span class="">FILTER CRITERIA  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>
                                    <div id="collapseOne_4" class="collapse show" aria-labelledby="headingOne_4" data-parent="#accordion_4" style="">
                                        <div class="card-body">
                                            <!--search by User-->
                                            <div id="rowUser1" class="rowCategory" style="display: none;">
                                                <div class="col-md-2"></div>
                                                <div class="col-md-10">
                                                    <label for="sel-vndassigned">User</label>
                                                    <br>
                                                    <asp:DropDownList ID="ddlUser" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlUser_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by User." runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <!--search by Type Error-->
                                            <div id="rowType" class="rowVndName" style="display: none;">
                                                <div class="col-md-2"></div>
                                                <div class="col-md-10">
                                                    <label for="sel-vndassigned">Type</label>
                                                    <br>
                                                    <asp:DropDownList ID="ddlType" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Error Type." runat="server"></asp:DropDownList>
                                                </div>
                                            </div>                                     
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>  
                </div>
            </div>

            <div class="row" style="display: none">
                <asp:HiddenField ID="hiddenId4" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId5" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenName" Value="" runat="server" />

                <asp:HiddenField ID="hdLinkExpand" value="0" runat="server" />
                <asp:HiddenField ID="hdTriggeredControl" value="" runat="server" />
                <asp:HiddenField ID="hdLaunchControl" value="" runat="server" />
                <asp:HiddenField ID="hdSelectedClass" value="" runat="server" />

                <table id="ndtt" runat="server"></table>
                <asp:Label ID="lblGrvGroup" Text="test" runat="server"></asp:Label>
            </div>

            <div id="gridSection" class="container" runat="server">
                <div class="panel panel-default">
                    <div class="panel-body">                
                        <div class="form-horizontal"> 
                            <div id="rowGridView1">
                                <asp:GridView ID="grvLogs" runat="server" AutoGenerateColumns="false"
                                    PageSize="10" CssClass="table table-striped table-bordered" AllowPaging="True" AllowSorting="true"
                                    GridLines="None" OnRowCommand="grvLogs_RowCommand" OnPageIndexChanging="grvLogs_PageIndexChanging"
                                    OnRowDataBound="grvLogs_RowDataBound" OnSorting="grvLogs_Sorting" ShowHeader="true"  
                                    OnRowUpdating="grvLogs_RowUpdating" >
                                    <Columns>                                                                                                              
                                        <asp:BoundField DataField="LOGID" HeaderText="ID" ItemStyle-Width="3%" SortExpression="LOGID" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol" />
                                        <%--<asp:BoundField DataField="WHLDATE" HeaderText="DATE" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="3%" />--%>
                                        <asp:TemplateField HeaderText="DATE" SortExpression="LOGDATE" ItemStyle-Width="15%"  >
                                            <ItemTemplate>
                                                <asp:Literal ID="Literal1" runat="server"
                                                    Text='<%#String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(Eval("LOGDATE"))) %>'>        
                                                </asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="LOGAPP" HeaderText="APPLICATION" ItemStyle-Width="15%" SortExpression="LOGAPP" />
                                        <asp:BoundField DataField="LOGLEVEL" HeaderText="ERROR LEVEL" ItemStyle-Width="15%" SortExpression="LOGLEVEL" />
                                        <asp:BoundField DataField="LOGTYPE" HeaderText="STATUS" ItemStyle-Width="15%" SortExpression="LOGTYPE" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol" />
                                        <asp:BoundField DataField="LOGUSER" HeaderText="USER" ItemStyle-Width="15%" SortExpression="LOGUSER" />
                                        <asp:BoundField DataField="LOGMESSAGE" HeaderText="ERROR TYPE" ItemStyle-Width="30%" SortExpression="LOGMESSAGE" />                                         
                                        <asp:BoundField DataField="LOGORIGEN" HeaderText="ERROR ORIGEN" ItemStyle-Width="15%" SortExpression="LOGORIGEN" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="LOGEXCEPTION" HeaderText="EXCEPTION" ItemStyle-Width="15%" SortExpression="LOGEXCEPTION" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:TemplateField HeaderText="LOG DETAILS">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkExpander" runat="server" TabIndex="1" ToolTip="Get Log Detail" CssClass="click-in" CommandName="show"
                                                    OnClientClick='<%# String.Format("return divexpandcollapse(this, {0});", Eval("LOGID")) %>'>
                                                    <span id="Span11" aria-hidden="true" runat="server">
                                                        <i class="fa fa-folder"></i>
                                                    </span>
                                                </asp:LinkButton>

                                                </td>
                                                    <tr>
                                                        <td colspan="17" class="padding0">
                                                            <div id="div<%# Eval("LOGID") %>" class="divCustomClass">
                                                                <asp:GridView ID="grvDetails" runat="server" AutoGenerateColumns="false" GridLines="None" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="LOGORIGEN" HeaderText="ORIGEN" ItemStyle-Width="15%" SortExpression="LOGORIGEN" />
                                                                        <asp:BoundField DataField="LOGEXCEPTION" HeaderText="EXCEPTION" ItemStyle-Width="10%" SortExpression="LOGEXCEPTION" />                                                                        
                                                                    </Columns>
                                                                    <HeaderStyle BackColor="#95B4CA" ForeColor="White" />
                                                                </asp:GridView>
                                                            </div>
                                                        </td>
                                                    </tr>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>                                    
                                    <PagerSettings  Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" pagebuttoncount="10"  />
                                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />                                                                
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>    
    <script type="text/javascript" src="../Scripts/bootstrap4-input-clearer.js"></script>

    <script type="text/javascript">

        function divexpandcollapse(controlid, divname) {
            debugger

            if (divname == null) {

            } else {
                var iAccess = $("#div" + divname);
                var iContainer = $("#" + controlid.id)

                //var iAccess = $("#" + divname).attr('id').replace("div", "");                
                var temp2

                if (iContainer.find("i").length) {
                    temp2 = iContainer.attr('class');

                    for (var i = 0; i < iContainer.children.length; i++) {
                        if (iContainer.children(i).prop('tagName') == 'SPAN') {
                            var myControl = iContainer.children(i);
                            var iValue = myControl.children(0);
                            var iClass = iValue.attr('class');

                            if (iClass == "fa fa-folder" ) {

                                //iAccess.toggleClass("divCustomClass divCustomClassOk");
                                //iAccess.removeClass('divCustomClass');
                                //iAccess.addClass('divCustomClassOk');

                                //iValue.addClass('fa').removeClass('fa');
                                //iValue.toggleClass('fa-plus fa-minus');//.removeClass('fa-plus');                                

                                //iAccess.closest('td').removeClass('padding0');

                                $('#<%=hiddenId5.ClientID %>').val("1");

                            } else if (iClass == "fa fa-folder-open") {                                

                                //iAccess.toggleClass("divCustomClassOk divCustomClass");
                                //iAccess.removeClass('divCustomClassOk');
                                //iAccess.addClass('divCustomClass');

                                //iValue.addClass('fa').removeClass('fa');
                                //iValue.toggleClass('fa-minus fa-plus');//.removeClass('fa-minus');

                                //iAccess.closest('td').addClass('padding0');

                                $('#<%=hiddenId5.ClientID %>').val("1");
                                
                            } 
                        }
                    } 

                    $('#<%=hdTriggeredControl.ClientID %>').val(divname);
                    $('#<%=hdLaunchControl.ClientID %>').val(controlid.id);
                    $('#<%=hdSelectedClass.ClientID %>').val(iClass);
                }
            }
        }

        $('body').on('click', '#accordion_4 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse4 = document.getElementById('collapseOne_4');
            isActivePanel(collapse4, 5);
        });

        $('body').on('click', '#accordion_3 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse3 = document.getElementById('collapseOne_3');
            isActivePanel(collapse3, 4);
        });

        function isActivePanel(activePanel, valorActive) {
            debugger

            var hd1 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId5.ClientID%>').value;
                      
            
            if (valorActive == 4) {
                if ($('#<%=hiddenId4.ClientID %>').val() == "0") {
                    $('#<%=hiddenId4.ClientID %>').val("1");
                    <%--$('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId4.ClientID %>').val("0");
                   <%-- $('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            if (valorActive == 5) {
                if ($('#<%=hiddenId5.ClientID %>').val() == "0") {
                    $('#<%=hiddenId5.ClientID %>').val("1");
                    <%--$('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId5.ClientID %>').val("0");
                    <%--$('#<%=hiddenId3.ClientID %>').val("0");--%>
                    hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            JSFunction();
        }

        function JSFunction() {
            __doPostBack('<%= updatepnl.ClientID  %>', '');
        }        

        $(function () {

            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd5, collapse4);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd4, collapse3);

        })

        function pageLoad(event, args) {

            if (args.get_isPartialLoad()) {
            }

            // nested gridview

            var hd = document.getElementById('<%=hdLinkExpand.ClientID%>').value;
            var hd1 = document.getElementById('<%=hdTriggeredControl.ClientID%>').value;
            var hd2 = document.getElementById('<%=hdLaunchControl.ClientID%>').value;

                var iAccess = $("#div" + hd1);
                var iContainer = $("#" + hd2);                

                var iValue = iContainer.children(0).children(0)
                var iClass = document.getElementById('<%=hdSelectedClass.ClientID%>').value;
                var iCurrentClass = iValue.attr('class');                                
                if (iClass == "fa fa-folder") { 
                    if (iAccess.attr('class') != "divCustomClassOk") {
                        iAccess.toggleClass('divCustomClass divCustomClassOk');
                    }
                    if (iClass != "fa fa-folder-open" && iCurrentClass == iClass) {
                        iValue.toggleClass('fa-folder fa-folder-open');
                    }                    
                    iAccess.closest('td').removeClass('padding0');
                }
                else {
                    if (iAccess.attr('class') != "divCustomClass") {
                        iAccess.toggleClass('divCustomClassOk divCustomClass');
                    }
                    if (iClass != "fa fa-folder" && iCurrentClass == iClass) {
                        iValue.toggleClass('fa-folder-open fa-folder');
                    }                    
                    iAccess.closest('td').addClass('padding0');
                }
            $('#<%=hdLinkExpand.ClientID %>').val("0");   

            var hdName = document.getElementById('<%=hiddenName.ClientID%>').value;
            yesnoCheckCustom(hdName)

            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd5, collapse4);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd4, collapse3);
        }

        function afterDdlCheck(hdFieldId, divId) {           

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        function afterRadioCheck(hdFieldId, divId) {   

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        function yesnoCheck(id) {
            debugger

            if (id !== null && id !== "" && id !== undefined) {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowUser1", "rowType"];

                var i;
                for (i = 0; i < divs.length; i++) {                   
                    if (divs[i] != id) {                       
                        x = document.getElementById(divs[i]);
                        xstyle = x.style;
                        xstyle.display = "none";
                    } else {                        
                        x = document.getElementById(divs[i]);
                        xstyle = x.style;
                        xstyle.display = "block";
                        $('#<%=hiddenName.ClientID %>').val(id);
                        //x.display = "block";
                    }
                }                
                var collapse3 = document.getElementById('collapseOne_3');
                var collapse4 = document.getElementById('collapseOne_4');
                
                var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;                
           
            if (hd4 == 1) {              
                $('#<%=hiddenId5.ClientID %>').val("1");                
                hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
            }           
            else { hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value; }
               
                afterRadioCheck(hd4, collapse3)
                afterRadioCheck(hd5, collapse4)           
            }            
        }

        function yesnoCheckCustom(id) {
            debugger

            if (id !== null && id !== "" && id !== undefined) {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowUser1", "rowType"];

                var i;
                for (i = 0; i < divs.length; i++) {
                    //text += divs[i] + "<br>";
                    if (divs[i] != id) {
                        //x = document.getElementById(divs[i]).style;
                        x = document.getElementById(divs[i]);
                        xstyle = x.style;
                        xstyle.display = "none";
                    } else {
                        //x = document.getElementById(divs[i]).style;
                        x = document.getElementById(divs[i]);
                        xstyle = x.style;
                        xstyle.display = "block";
                        $('#<%=hiddenName.ClientID %>').val(id);
                        //x.display = "block";
                    }
                }
                //isActivePanel(collapse1, 1);
                //isActivePanel(collapse2, 2);
            }

        }

    </script>

</asp:Content>

    
    
