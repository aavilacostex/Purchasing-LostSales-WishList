<%@ Page Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Claims-Report.aspx.vb" Inherits="WebApplication1.Claims_Report" ViewStateMode="Disabled" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="Atk" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">

        function messageFormSubmitted(mensaje, show) {
            //debugger
            messages.alert(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
        }

        function afterDdlCheck(hdFieldId, divId) {
            //debugger

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        $('body').on('click', '#accordion_2 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse1 = document.getElementById('collapseOne_2');
            isActivePanel(collapse1, 2);
        });

        $('body').on('click', '#accordion h5 a', function () {
            //debugger
            //alert("pepi");
            var collapse2 = document.getElementById('collapseOne');
            isActivePanel(collapse2, 1);
        }); 

        function isActivePanel(activePanel, valorActive) {
            //debugger

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;

            if (valorActive == 1) {
                if ($('#<%=hiddenId1.ClientID %>').val() == "0") {
                    $('#<%=hiddenId1.ClientID %>').val("1");
                    hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
                    //afterDdlCheck(hd1, activePanel)
                } else {
                    $('#<%=hiddenId1.ClientID %>').val("0");
                    hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
                    //afterDdlCheck(hd1, activePanel)
                }
            }
            if (valorActive == 2) {
                if ($('#<%=hiddenId2.ClientID %>').val() == "0") {
                    $('#<%=hiddenId2.ClientID %>').val("1");
                    hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId2.ClientID %>').val("0");
                    hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            JSFunction();
        } 

        function JSFunction() {
            //debugger

            __doPostBack('<%= updatepnl.ClientID  %>', '');
        }

        function disableInputs() {

            //debugger

            //$("#upPanel :input").attr("disabled", true);
            //$("#upPanel :input").prop("disabled", true);

            $("#pnClaimDetails :input").attr("disabled", true);
            $("#pnClaimDetails :input").prop("disabled", true);
        }

        function ToggleGridPanel(btn, row) {
           //debugger

            alert(value);
        }

        $('body').on('click', '.click-in', function (e) {

            var hd1 = document.getElementById('<%=hdLinkExpand.ClientID%>').value;
            //$('#MainContent_addNewPartManual2').closest('.container').removeClass('hideProp')
            var idd = e.currentTarget.id
            //document.getElementById('<%=hiddenId2.ClientID%>').value;
            //$('#' + idd).

            var val = $(this).attr('CLAIM#');
            var val1 = $(this).closest('.hideProp').removeClass('hideProp');
            //var id = $('MainContent_grvClaimReport_lnkExpander').data('CLAIM#');
            console.log(val1);
            //console.log(id);            
            console.log(e);

        });

        function divexpandcollapse(divname) {
            //debugger

            var div = document.getElementById(divname);
            var img = document.getElementById('img' + divname);
            if (div.style.display == "none") {
                div.style.display = "inline";
                img.src = "../Images/minus.gif";
            } else {
                div.style.display = "none";
                img.src = "../Images/plus.gif";
            }
        }

        function divexpandcollapseChild(divname) {
            var div1 = document.getElementById(divname);
            var img = document.getElementById('img' + divname);
            if (div1.style.display == "none") {
                div1.style.display = "inline";
                img.src = "../Images/minus.gif";
            } else {
                div1.style.display = "none";
                img.src = "../Images/plus.gif";;
            }
        }

        function divexpandcollapse(controlid, divname) {
            //debugger
            
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

                            var val1 = $('#<%=hdLinkExpand.ClientID %>').val();
                            
                            if (iClass == "fa fa-plus" && $('#<%=hdLinkExpand.ClientID %>').val() == "0") {  
                                
                                //iAccess.toggleClass("divCustomClass divCustomClassOk");
                                //iAccess.removeClass('divCustomClass');
                                //iAccess.addClass('divCustomClassOk');

                                //iValue.addClass('fa').removeClass('fa');
                                //iValue.toggleClass('fa-plus fa-minus');//.removeClass('fa-plus');                                

                                //iAccess.closest('td').removeClass('padding0');

                                $('#<%=hdLinkExpand.ClientID %>').val("1");                                                                

                            } else if (iClass == "fa fa-minus" && $('#<%=hdLinkExpand.ClientID %>').val() == "0") {                                

                                //iAccess.toggleClass("divCustomClassOk divCustomClass");
                                //iAccess.removeClass('divCustomClassOk');
                                //iAccess.addClass('divCustomClass');

                                //iValue.addClass('fa').removeClass('fa');
                                //iValue.toggleClass('fa-minus fa-plus');//.removeClass('fa-minus');

                                //iAccess.closest('td').addClass('padding0');

                                $('#<%=hdLinkExpand.ClientID %>').val("1");
                                
                            } 
                        }
                    } 

                    $('#<%=hdTriggeredControl.ClientID %>').val(divname);
                    $('#<%=hdLaunchControl.ClientID %>').val(controlid.id);
                    $('#<%=hdSelectedClass.ClientID %>').val(iClass);
                }                  
            }
        }

    </script>

    <asp:UpdatePanel ID="updatepnl" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="hdPartNoSelected" />   
            <asp:AsyncPostBackTrigger ControlID="hdClaimNoSelected" />   
            <asp:AsyncPostBackTrigger ControlID ="ddlSearchReason" />
            <asp:AsyncPostBackTrigger ControlID ="ddlSearchDiagnose" />
            <asp:AsyncPostBackTrigger ControlID ="ddlSearchExtStatus" />            
        </Triggers>

        <ContentTemplate>

            <div class="container-fluid">
                <div class="breadcrumb-area breadcrumb-bg">
                    <div class="row">
                        <div class="col-md-offset-4 col-md-9 center">
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
                                                    <span>CLAIMS</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="row">
                                <div class="col-md-4">
                                    <asp:LinkButton ID="btnGetTemplate" class="boxed-btn-layout btn-rounded" runat="server">
                                                            <i class="fa fa-plus fa-1x"" aria-hidden="true"> </i> Get Template
                                    </asp:LinkButton>
                                </div>
                                <div class="col-md-4">
                                    <asp:LinkButton ID="btnImportExcel" class="boxed-btn-layout btn-rounded hideProp" Visible="false" runat="server">
                                                            <i class="fa fa-file-excel-o fa-1x" aria-hidden="true"> </i> IMPORT
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="loadFileSection" class="container hideProp" runat="server">
                <div class="row">
                    <div class="col-md-3"></div>
                    <div class="col-md-6">
                        <div id="pnLoadFile" class="shadow-to-box">
                            <div class="row" style="padding: 30px 0;">
                                <div class="col-md-3"></div>
                                <div class="col-md-6"><span id="spnLoadExcel">file to import data</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="row">
                                <div class="col-md-3"></div>
                                <div class="col-md-6 center-row">
                                    <asp:FileUpload ID="fuOPenEx" CssClass="form-control" runat="server" />
                                </div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="row" style="padding: 5px 0;">
                                <div class="col-md-3"></div>
                                <div class="col-md-6"><span id="spnTypeFormat">(CSV and XLS formats are allowed)</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="row" style="padding: 20px 0;">
                                <div class="col-md-3"></div>
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-6" style="float: right; text-align: right !important;">
                                            <asp:Button ID="btnSave" Text="Upload" class="btn btn-primary btn-lg btnFullSize" OnClick="btnSave_Click" runat="server" />
                                        </div>
                                        <div class="col-md-6" style="float: left;">
                                            <asp:Button ID="btnBack" Text="   Back   " class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3"></div>
                            </div>                    
                        </div>
                    </div>
                    <div class="col-md-3"></div>
                </div>        
            </div>

            <div id="integratedRow" class="container-fluid">
                <div class="row">
                    <div class="col-md-3">
                        <div id="rowPageSize" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed">
                                <asp:Label ID="lblText1" Text="Show " runat="server"></asp:Label>
                            </div>
                            <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed">
                                <asp:DropDownList name="ddlPageSize" ID="ddlPageSize" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" class="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed">
                                <asp:Label ID="lblText2" Text=" entries." runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div id="rowBtnOpt" class="row">
                            <div class="col-xs-12 col-sm-3"></div>
                            <div class="col-xs-12 col-sm-2 flex-item-1 padd-fixed">
                                <asp:Button ID="btnExcel" class="btn btn-primary btn-lg float-right btnFullSize" runat="server" Text="Excel" />
                            </div>
                            <div class="col-xs-12 col-sm-2 flex-item-2 padd-fixed">
                                <asp:Button ID="btnPdf" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Pdf" />
                            </div>
                            <div class="col-xs-12 col-sm-2 flex-item-3 padd-fixed">
                                <asp:Button ID="btnCopy" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Copy" />
                            </div>
                            <div class="col-xs-12 col-sm-3"></div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div id="rowBtnSearch" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" style="float: right;">
                                <asp:Label ID="lblSearch" Text="Search: " runat="server" Height="27px"></asp:Label>
                            </div>
                            <div class="col-xs-12 col-sm-5 flex-item-2 padd-fixed">
                                <asp:TextBox name="txtSearch" ID="txtSearch" class="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-xs-12 col-sm-3 flex-item-2 padd-fixed">
                                <asp:Button name="btnSearch" ID="btnSearch" class="btn btn-primary btn-sm btnFullSize1" Text="Search" runat="server"></asp:Button>
                            </div>
                            <%--<div class="spinner-grow text-warning"></div>--%>
                        </div>
                        <div id="notVisibleKeyPress" style="display: none" runat="server">
                            <asp:Button ID="Button1" runat="server" Text="Button" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="container">
                <div id="rowFilters" class="row">
                    <div class="col-md-1"></div>
                    <div class="col-md-5">
                        <div class="accordion-wrapper">
                            <div id="accordion_2">
                                <div class="card">
                                    <div class="card-header" id="headingOne_2">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_2" aria-expanded="false" aria-controls="collapseOne_2">
                                                <span class="">FILTER DATA  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>

                                    <!--FORM TO GET DATA FOR FILTERING DATA-->
                                    <!--ACCORDION CONTENT-->
                                    <div id="collapseOne_2" class="collapse" aria-labelledby="headingOne_2" data-parent="#accordion_2" style="">
                                        <div class="card-body">
                                            <div class="row">

                                                <div id="claimsbuilder" class="form-group col-md-12">
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>Reason</p>
                                                                <asp:RadioButton ID="rdReason" OnCheckedChanged="rdReason_CheckedChanged" onclick="yesnoCheck('rowReason');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>PartNo</p>
                                                                <asp:RadioButton ID="rdPartNo" OnCheckedChanged="rdPartNo_CheckedChanged" onclick="javascript:yesnoCheck('rowPartNo');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>                                                        
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>Diagnose</p>
                                                                <asp:RadioButton ID="rdDiagnose" OnCheckedChanged="rdDiagnose_CheckedChanged" onclick="yesnoCheck('rowDiagnose');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>ClaimNo</p>
                                                                <asp:RadioButton ID="rdClaimNo" OnCheckedChanged="rdClaimNo_CheckedChanged" onclick="yesnoCheck('rowClaimNo');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>                                                        
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>Customer</p>
                                                                <asp:RadioButton ID="rdCustomer" OnCheckedChanged="rdCustomer_CheckedChanged" onclick="yesnoCheck('rowCustomer');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>User</p>
                                                                <asp:RadioButton ID="rdUser" OnCheckedChanged="rdUser_CheckedChanged" onclick="yesnoCheck('rowUser');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>                                                        
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>StatusOut</p>
                                                                <asp:RadioButton ID="rdStatusOut" OnCheckedChanged="rdStatusOut_CheckedChanged" onclick="yesnoCheck('rowExtStatus');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>Date</p>
                                                                <asp:RadioButton ID="rdDate" OnCheckedChanged="rdDate_CheckedChanged" onclick="javascript:yesnoCheck('rowDateInit');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>                                                        
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <p>StatusIn</p>
                                                                <asp:RadioButton ID="rdStatusIn" OnCheckedChanged="rdStatusIn_CheckedChanged" onclick="yesnoCheck('rowIntStatus');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>
                                                            </label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label class="form-check">
                                                                <%--<p>Date</p>
                                                                <asp:RadioButton ID="RadioButton2" Text="Date" OnCheckedChanged="rdDate_CheckedChanged" onclick="javascript:yesnoCheck('rowDateInit');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark-claims"></span>--%>
                                                            </label>
                                                        </div>                                                        
                                                    </div>

                                                </div>

                                                <!-- cross site forgery's attack avoiding -->
                                                <div class="col-md-1">
                                                    <input type="hidden" name="csrf" value="b3f24ac9359094f7b4629613138570a6-106b16695033660d3701da01a206aeba">
                                                </div>
                                                
                                            </div>
                                            <div class="row">
                                                <!-- SUBMIT BUTTON AND CONVERT TO EXCEL THE ACTUAL PAGE -->
                                                <%--<div id="rowBtnFilters" class="row make-it-flex">--%>

                                                    <div class="col-sm-6" style="float: right;">
                                                        <asp:Button ID="submit" class="btn btn-primary btn-lg float-right btnFullSize" runat="server" Text="Submit" />
                                                    </div>
                                                    <div class="col-sm-6">
                                                        <asp:Button ID="convert" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Reset" />
                                                    </div>

                                                <%--</div>--%>
                                            </div>
                                        </div>
                                        <!-- COLLAPSE CONTENT END -->
                                    </div>
                                    <!-- CARD END -->
                                </div>
                                <!-- ACCORDION END -->
                            </div>
                        </div>
                    </div>
                    <div class="col-md-1"></div>
                    <div class="col-md-5">
                        <div class="accordion-wrapper">
                            <div id="accordion">
                                <div class="card">
                                    <!--ACCORDION DEFAULT VALUES HEADER-->
                                    <div class="card-header" id="headingOne">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                                <span class="">GENERAL CLAIMS DATA <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>

                                    <!--ACCORDION: CONTENT-->
                                    <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordion" style="">
                                        <div id="claimsCardBody" class="card-body">
                                            <ul class="checklist">
                                                <li><i class="fa fa-check"></i>COUNT ITEMS:
                                                    <asp:Label ID="lblTotalClaims" Text="" runat="server" />
                                                    
                                                </li>
                                            </ul>
                                        </div>
                                        <!--search by Claim Number-->
                                        <div id="rowClaimNo" class="rowClaimNo" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Claim Number</label>
                                                <br>
                                                <asp:TextBox ID="txtClaimNo" name="txt-claimNo" class="form-control autosuggestclaim" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!--search by Customer-->
                                        <div id="rowCustomer" class="rowCustomer" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Customer Number</label>
                                                <br>
                                                <asp:TextBox ID="txtCustomer" name="txt-Customer" class="form-control autosuggestcustomer" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!--search by Part Number-->
                                        <div id="rowPartNo" class="rowPartNo" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Part Number</label>
                                                <br>
                                                <asp:TextBox ID="txtPartNo" name="txt-partNo" class="form-control autosuggestpart" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!--search by Initial Date-->
                                        <div id="rowDateInit" class="rowDateInit" style="display: none;">
                                            <div class="col-md-1"></div>
                                            <div class="col-md-5 padding0">
                                                <label for="sel-vndassigned">From</label>
                                                <div class="input-group-append">    
                                                    <asp:TextBox ID="txtDateInit" name="txt-dateinit" placeholder="MM/DD/AAAA" class="form-control autosuggestdateinit" runat="server"></asp:TextBox>
                                                    <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                                </div>                                                
                                            </div>
                                            <div class="col-md-1"></div>
                                            <div class="col-md-5 padding0">
                                                <label for="sel-vndassigned">To</label>
                                                <div class="input-group-append">
                                                    <asp:TextBox ID="txtDateTo" name="txt-dateto" placeholder="MM/DD/AAAA" class="form-control autosuggestdateto" runat="server"></asp:TextBox>
                                                    <span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                                </div>                                                
                                            </div>
                                        </div>
                                        <!--search by reason-->
                                        <div id="rowReason" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Claim Reason</label>
                                                <br>
                                                <asp:DropDownList ID="ddlSearchReason" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlSearchReason_SelectedIndexChanged" AutoPostBack="true" title="Search by Claim Reason." EnableViewState="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <!--search by Diagnose-->
                                        <div id="rowDiagnose" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Claim Diagnose</label>
                                                <br>
                                                <asp:DropDownList ID="ddlSearchDiagnose" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlSearchDiagnose_SelectedIndexChanged" AutoPostBack="true" title="Search by Claim Reason." EnableViewState="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <!--search by ExtStatus-->
                                        <div id="rowExtStatus" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Claim External Status</label>
                                                <br>
                                                <asp:DropDownList ID="ddlSearchExtStatus" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlSearchExtStatus_SelectedIndexChanged" AutoPostBack="true" title="Search by Claim Reason." EnableViewState="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <!--search by IntStatus-->
                                        <div id="rowIntStatus" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Claim Internal Status</label>
                                                <br>
                                                <asp:DropDownList ID="ddlSearchIntStatus" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlSearchIntStatus_SelectedIndexChanged" AutoPostBack="true" title="Search by Claim Reason." EnableViewState="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <!--search by User-->
                                        <div id="rowUser" style="display: none;">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-10">
                                                <label for="sel-vndassigned">Claim User</label>
                                                <br>
                                                <asp:DropDownList ID="ddlSearchUser" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlSearchUser_SelectedIndexChanged" AutoPostBack="true" title="Search by Claim Reason." EnableViewState="true" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- COLLAPSE CONTENT END -->
                                </div>
                                <!-- CARD END -->
                            </div>
                            <!-- ACCORDION END -->
                        </div>
                    </div>
                    <%--<div class="col-md-1"></div>--%>
                </div>

                <div id="hiddenFields" class="row">
                    <asp:HiddenField ID="hiddenId1" Value="0" runat="server" />
                    <asp:HiddenField ID="hiddenId2" Value="0" runat="server" />
                    <asp:HiddenField ID="hiddenName" Value="" runat="server" />
                    <asp:HiddenField ID="hdClaimNoSelected" OnValueChanged="hdClaimNoSelected_ValueChanged" Value="" runat="server" />
                    <asp:HiddenField ID="hdPartNoSelected" OnValueChanged="hdPartNoSelected_ValueChanged" Value="" runat="server" />
                    <asp:HiddenField ID="hdCustomerNoSelected" OnValueChanged="hdCustomerNoSelected_ValueChanged" Value="" runat="server" />
                    <asp:HiddenField ID="hdDateInitSelected" OnValueChanged="hdDateInitSelected_ValueChanged" Value="" runat="server" />
                    <asp:HiddenField ID="hdLinkExpand" value="0" runat="server" />
                    <asp:HiddenField ID="hdTriggeredControl" value="" runat="server" />
                    <asp:HiddenField ID="hdLaunchControl" value="" runat="server" />
                    <asp:HiddenField ID="hdSelectedClass" value="" runat="server" />

                    <asp:HiddenField ID="hdFileImportFlag" Value="0" runat="server" />
                    <asp:HiddenField ID="hdShowTemplate" Value="0" runat="server" />

                    <asp:HiddenField ID="hdReason" Value="" runat="server" />
                    <asp:HiddenField ID="hdDiagnose" Value="" runat="server" />
                    <asp:HiddenField ID="hdStatusOut" Value="" runat="server" />

                    <asp:HiddenField ID="selectedFilter" Value=""  runat="server" />

                    <table id="ndtt" runat="server"></table>
                    <asp:Label ID="lblGrvGroup" Text="test" runat="server"></asp:Label>

                </div>
            </div>

            <div id="addProdDev" class="container hideProp" runat="server">
                <div class="row" style="max-width:70% !important;margin: 0 auto !important;">
                    <div class="col-md-12">
                        <div id="pnClaimDetails" class="shadow-to-box">
                            <div class="row" style="padding: 55px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnClaimDetails">Claim Data</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="form-row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <asp:Label ID="lblWhlCode" CssClass="label-style" Text="Claim No." runat="server"></asp:Label>
                                        <asp:TextBox ID="txtWhlCode" CssClass="form-control fullTextBox" runat="server" />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <asp:Label ID="lblCreationDate" CssClass="label-style" Text="Submitted By" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCreationDate" CssClass="form-control fullTextBox" runat="server" />
                                    </div>
                                </div>                                
                            </div>
                            <div class="form-row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <asp:Label ID="lblCurrentVendor" CssClass="label-style" Text="Invoice No." runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCurrentVendor" CssClass="form-control fullTextBox" runat="server" />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <asp:Label ID="lblDescriptionPD" CssClass="label-style" Text="Internal Status" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtDescriptionPD" CssClass="form-control fullTextBox" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <asp:Label ID="Label1" CssClass="label-style" Text="Description" runat="server"></asp:Label>
                                        <asp:TextBox ID="TextBox1" TextMode="MultiLine" CssClass="form-control fullTextBox" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <asp:Label ID="Label2" CssClass="label-style" Text="Comments" runat="server"></asp:Label>
                                        <asp:TextBox ID="TextBox2" TextMode="MultiLine" CssClass="form-control fullTextBox" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group col-md-3"></div>
                                <div class="form-group col-md-6" style="padding-top: 20px;">
                                    <div class="row">
                                        <div class="col-md-6" style="float: right; text-align: right !important;">
                                            <asp:Button ID="btnCreateProjectPD" Text="create project" class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                        <div class="col-md-6" style="float: left;">
                                            <asp:Button ID="btnPDBack" Text="   Back   " class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                    </div>                                    
                                </div>
                                <div class="form-group col-md-3"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="container-fluid">
                <div class="container-fluid">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="form-horizontal">

                                <div id="rowGridView">
                                    <asp:GridView ID="grvClaimReport" runat="server" AutoGenerateColumns="false" ShowFooter="false" PageSize="10" 
                                        CssClass="table table-striped table-bordered" AllowPaging="True" DataKeyNames="CLAIM#" GridLines="None" 
                                        OnPageIndexChanging="grvClaimReport_PageIndexChanging" OnRowDataBound="grvClaimReport_RowDataBound" OnRowCommand="grvClaimReport_RowCommand">
                                        <Columns>                                           
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkExpander" runat="server" TabIndex="1" ToolTip="Get Claim Detail" CssClass="click-in" CommandName="show" 
                                                        OnClientClick='<%# String.Format("return divexpandcollapse(this, {0});", Eval("CLAIM#")) %>' >
                                                        <span id="Span1" aria-hidden="true" runat="server">                                                           
                                                            <i class="fa fa-plus"></i>                                                            
                                                        </span>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField ItemStyle-Width="20px">
                                                <ItemTemplate>
                                                    <a href="JavaScript:divexpandcollapse('div<%# Eval("CLAIM#") %>');">
                                                        <img id="imgdiv<%# Eval("CLAIM#") %>" width="9" src="../Images/plus.gif"
                                                            alt="" /></a>
                                                </ItemTemplate>
                                                <ItemStyle Width="20px" VerticalAlign="Middle"></ItemStyle>
                                            </asp:TemplateField>--%>
                                            <asp:BoundField DataField="CLAIM#" HeaderText="CLAIM NUMBER" ItemStyle-Width="7%" />
                                            <asp:BoundField DataField="TYPE" HeaderText="TYPE" ItemStyle-Width="11%" />
                                            <asp:BoundField DataField="REASON" HeaderText="REASON" ItemStyle-Width="15%" />
                                            <asp:BoundField DataField="DIAGNOSE" HeaderText="DIAGNOSE" ItemStyle-Width="15%" />
                                            <asp:BoundField DataField="CUSTOMER" HeaderText="CUSTOMER" ItemStyle-Width="7%" />
                                            <asp:TemplateField HeaderText="INITDATE" ItemStyle-Width="7%" SortExpression="INITDATE">
                                                <ItemTemplate>
                                                    <asp:Literal ID="Literal1" runat="server"
                                                        Text='<%#String.Format("{0:MM/dd/yyyy}", System.Convert.ToDateTime(Eval("INITDATE"))) %>'>        
                                                    </asp:Literal>
                                                </ItemTemplate>
                                            </asp:TemplateField>                                            
                                            <asp:BoundField DataField="EXTSTATUS" HeaderText="EXTSTATUS" ItemStyle-Width="10%" />
                                            <asp:BoundField DataField="PART#" HeaderText="PART#" ItemStyle-Width="6%" />
                                            <asp:BoundField DataField="QTY" HeaderText="QTY" ItemStyle-Width="5%" /> 
                                            <asp:BoundField DataField="UNITPR" HeaderText="UNITPR" ItemStyle-Width="6%" />
                                            <%--<asp:BoundField DataField="INCLNO" HeaderText="INCLNO" ItemStyle-Width="5%" />  --%>      
                                            <asp:TemplateField  HeaderText="INCLNO" >
                                                <ItemTemplate>
                                                    <asp:Label ID="Label12" runat="server" Text='<%# Eval("INCLNO").ToString() %>'></asp:Label>
                                                    </td>
                                                    <tr>
                                                        <td colspan="100%" class="padding0">
                                                            <div id="div<%# Eval("CLAIM#") %>" class="divCustomClass">
                                                                <asp:GridView ID="grvDetails" runat="server" AutoGenerateColumns="false" GridLines="None">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="CLAIM#" HeaderText="CLAIM NUMBER" ItemStyle-Width="5%" />
                                                                        <asp:BoundField DataField="SUBMITTEDBY" HeaderText="SUBMITTED BY" ItemStyle-Width="13%" />
                                                                        <asp:BoundField DataField="INVOICENO" HeaderText="INVOICE NUMBER" ItemStyle-Width="6%" />
                                                                        <asp:BoundField DataField="INTSTATUS" HeaderText="INTSTATUS" ItemStyle-Width="12%" />
                                                                        <asp:BoundField DataField="DESCRIPTION" HeaderText="DESCRIPTION" ItemStyle-Width="19%" />
                                                                        <asp:BoundField DataField="COMMENT1" HeaderText="COMMENT1" ItemStyle-Width="15%" />
                                                                        <asp:BoundField DataField="COMMENT2" HeaderText="COMMENT2" ItemStyle-Width="15%" />
                                                                        <asp:BoundField DataField="COMMENT3" HeaderText="COMMENT3" ItemStyle-Width="15%" />
                                                                    </Columns>
                                                                    <HeaderStyle BackColor="#95B4CA" ForeColor="White" />
                                                                </asp:GridView>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <%--<%# MyNewRow(Eval("CLAIM#")) %>--%>                                                   
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td colspan="13" class="hideProp">
                                                            <div id='<%# Eval("CLAIM#") %>' class="hideProp"></div>
                                                            <asp:GridView id="grdDetails" AutoGenerateColumns="false" runat="server">
                                                                <Columns>
                                                                    <asp:BoundField DataField="INTSTATUS" HeaderText="INTSTATUS" ItemStyle-Width="8%" />
                                                                </Columns>
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <%--<asp:TemplateField HeaderText="Details" ItemStyle-Width="8%">
                                                <HeaderStyle CssClass="GridHeaderStyle" />
                                                <ItemStyle CssClass="GridHeaderStyle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton
                                                        ID="lbClaimDetail"
                                                        runat="server"
                                                        TabIndex="1" CommandName="Detail"
                                                        ToolTip="Get Claim Detail">
                                                        <span id="Span1" aria-hidden="true" runat="server">
                                                            <i class="fa fa-server"></i></span>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                        </Columns>
                                        <HeaderStyle BackColor="#0063A6" ForeColor="White" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" PageButtonCount="10" />
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <Atk:ModalPopupExtender ID="mdClaimDetailsExp" runat="server" PopupControlID="navsSection" Enabled ="True" TargetControlID="hdModalExtender" CancelControlID="btnClose" ></Atk:ModalPopupExtender>
             <asp:HiddenField ID="hdModalExtender" Value="" runat="server" />

            <div id="navsSection" class="container" runat="server">
                <div class="row">
                    <!-- Tab links -->
                    <ul id="ntab" class="nav nav-tabs" role="tablist" style="visibility: visible;">
                        <li class="nav-item">
                            <a class="nav-link active" data-toggle="tab" href="#issue-description" role="tab" aria-selected="false">issue description</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" data-toggle="tab" href="#claims-dept" role="tab" aria-selected="false">departments</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" data-toggle="tab" href="#claims-status" role="tab" aria-selected="false">status</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" data-toggle="tab" href="#claim-description" role="tab" aria-selected="false">claim description</a>
                        </li>
                    </ul>

                    <!-- Tab panes -->
                    <div id="tabc" class="tab-content">
                        <div class="tab-pane active" id="issue-description">
                            <div class="col-md-12">
                                <h3>ISSUE DESCRIPTION</h3>
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Panel ID="pnISDet" GroupingText="Claims Data" runat="server">
                                            <%--<div class="form-row">                                            
                                                <div class="col-md-6">
                                                    <div class="form-group">
                                                        <div class="row">
                                                            <div class="col-sm-2">
                                                                <asp:Label ID="lblClaimNoData" Text="Claim No." CssClass="control-label" runat="server"></asp:Label>
                                                            </div>
                                                            <div class="com-sm-10">
                                                                <asp:TextBox ID="txtClaimNoData" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>                                                        
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="form-group">
                                                        <div class="row">
                                                            <div class="col-sm-2">
                                                                <asp:Label ID="lblClaimType" Text="Claim Type" CssClass="control-label" runat="server"></asp:Label>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <asp:TextBox ID="txtClaimType" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>                                                        
                                                    </div>
                                                </div>                                                                                       
                                            </div>--%>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblClaimNoData" Text="Claim No." CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtClaimNoData" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblClaimType" Text="Claim Type" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtClaimType" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblPartNoData" Text="Part No." CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtPartNoData" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblPartDescription" Text="Part Desc." CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtPartDescription" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblEnteredBy" Text="Entered By" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtEnteredBy" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblEnteredFrom" Text="Entered From" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtEnteredFrom" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblQty" Text="QTY" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtQty" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblUnitCost" Text="Unit Cost" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtUnitCost" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblInvoiceNo" Text="Invoice No." CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtInvoiceNo" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblLocation" Text="Location" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtLocation" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblInstDate" Text="Installation Date" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtInstDate" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblHWorked" Text="Hours Worked" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtHWorked" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row last">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblDateEntered" Text="Date Entered" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtDateEntered" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    
                                                </div>
                                            </div>

                                        </asp:Panel> 
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Panel ID="pnContact" GroupingText="Customer Info" runat="server">
                                            
                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblCustomerData" Text="Customer No." CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtCustomerData" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblCustomerName" Text="Customer Name" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtCustomerName" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblContactName" Text="Contact Name" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtContactName" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblContactPhone" Text="Contact Phone" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtContactPhone" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row last">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblContactEmail" Text="Contact Email" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtContactEmail" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>                                                
                                            </div>

                                        </asp:Panel>
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Panel ID="pnInformation" GroupingText="Information" runat="server">                                                                                      
                                                
                                             <h5>Equipment</h5>
                                            <div class="form-row">                                               
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblModel" Text="Model" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtModel" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>  
                                            
                                            <div class="form-row">                                               
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblSerial" Text="Serial" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtSerial" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div> 

                                            <div class="form-row last">                                               
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblArrangement" Text="Arrangement" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtArrangement" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div> 

                                            <h5>Engine</h5>
                                            <div class="form-row">                                               
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblModel1" Text="Model" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtModel1" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>  
                                            
                                            <div class="form-row">                                               
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblSerial1" Text="Serial" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtSerial1" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div> 

                                            <div class="form-row last">                                               
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblArrangement1" Text="Arrangement" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtArrangement1" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div> 
                                            
                                        </asp:Panel>
                                    </div>                                    
                                </div>                                
                            </div>                            
                        </div>
                        <div class="tab-pane" id="claims-dept">
                            <div class="col-md-12">
                                <h3>DEPARTMENTS</h3>
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Panel ID="pnClaimsDept" GroupingText="Claims Department" runat="server">

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblQuarantine" Text="Quarantine Required?" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkQuarantine" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtQuarantine" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtQuarantineDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblDiagnose" Text="Diagnose" CssClass="control-label" runat="server"></asp:Label>
                                                    <div class="form-row">                                                        
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtDiagnoseData" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:DropDownList ID="ddlDiagnoseData" CssClass="form-control" runat="server" />
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblClaimReason" Text="Claim Reason" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtClaimReason" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblProductionCode" Text="Production Code" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtProductionCode" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblVendorNo" Text="Vendor No." CssClass="control-label" runat="server"></asp:Label>
                                                    <div class="form-row">
                                                        <div class="col-md-8">
                                                            <asp:TextBox ID="txtVendorNo" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-4 pleft0">
                                                            <asp:Button ID="btnChangeVendor" CssClass="btn btn-primary btnpadding"  Text="Change" runat="server" />
                                                        </div>
                                                    </div>                                                    
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblVendorName" Text="Vendor Name" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtVendorName" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="form-row paddingtop8">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblSubClaim" Text="Subtotal Claimed" CssClass="control-label undermark" runat="server"></asp:Label>
                                                    <div class="form-row paddingtop8">
                                                        <div class="col-md-2">
                                                            <asp:Label ID="lblParts" CssClass="control-label" Text="Parts" runat="server" /> 
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtParts" CssClass="form-control" runat="server" />
                                                        </div>
                                                        <div class="col-md-2">
                                                            <asp:Label ID="lblFreight" CssClass="control-label" Text="Freight" runat="server" /> 
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtFreight" CssClass="form-control" runat="server" />
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row last paddingtop8">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblClaim" Text="Claim" CssClass="control-label undermark" runat="server"></asp:Label>
                                                    <div class="form-row paddingtop30">
                                                        <div class="col-md-4">
                                                            <asp:Label ID="lblApproved" CssClass="control-label" Text="Approved" runat="server" /> 
                                                        </div>
                                                        <div class="col-md-2">
                                                            <asp:CheckBox ID="chkApproved" runat="server" />
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:Label ID="lblDeclined" CssClass="control-label" Text="Declined" runat="server" /> 
                                                        </div>
                                                        <div class="col-md-2">
                                                            <asp:CheckBox ID="chkDeclined" runat="server" />
                                                        </div>
                                                    </div>                                                    
                                                </div> 
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblConsDamage" Text="Consequental damage cost suggested, if any." CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtConsDamage" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>                                            

                                        </asp:Panel>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Panel ID="pnSalesDept" GroupingText="Sales Department" runat="server">

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblClaimAuth" Text="Claim Authorization if over $500" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkClaimAuth" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtClaimAuth" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtClaimAuthDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row last">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblAmountApproved" Text="Claim amount approved by sales on top of Claims Approval" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtAmountApproved" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                        </asp:Panel>

                                        <br />

                                        <asp:Panel ID="pnRestock" GroupingText="Re-Stock Actions" runat="server">

                                            <div class="form-row last">
                                                <div class="col-md-6">
                                                    <asp:Button ID="btnRestock" Text="Re-Stock Part" CssClass="btn btn-primary" runat="server" />
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Button ID="btnUndoRestock" Text="Undo Re-Stock" CssClass="btn btn-primary" runat="server" />
                                                </div>
                                            </div>

                                        </asp:Panel>
                                    </div>
                                </div>                                
                            </div>                            
                        </div>
                        <div class="tab-pane" id="claims-status">
                            <div class="col-md-12">
                                <h3>STATUS</h3>
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Panel ID="pnInternalStatus" GroupingText="Internal Status" runat="server">

                                            <div class="form-row">                                                
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblActualStatus" Text="Actual Status" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtActualStatus" CssClass="form-control" placeholder="Info requested to cust." runat="server"></asp:TextBox>
                                                </div>
                                            </div>  

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblInitialReview" Text="Initial Review" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkInitialReview" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtInitialReview" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtInitialReviewDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>   
                                            
                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblAcknowledgeEmail" Text="Acknowledge Email" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkAcknowledgeEmail" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtAcknowledgeEmail" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtAcknowledgeEmailDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblInfoCust" Text="Info Requested to Customer" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkInfoCust" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtInfoCust" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtInfoCustDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblPartRequested" Text="Part Requested" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkPartRequested" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtPartRequested" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtPartRequestedDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblPartReceived" Text="Part Received" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkPartReceived" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtPartReceived" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtPartReceivedDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblTechReview" Text="Technical Review" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkTechReview" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtTechReview" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtTechReviewDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblWaitSupReview" Text="Waiting Supplier Review" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkWaitSupReview" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtWaitSupReview" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtWaitSupReviewDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                            <div class="form-row last">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblClaimCompleted" Text="Claim Completed" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:CheckBox ID="chkClaimCompleted" runat="server" />
                                                    <div class="form-row">                                                       
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtClaimCompleted" CssClass="form-control" runat="server" />                                                            
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:TextBox ID="txtClaimCompletedDate" CssClass="form-control" runat="server" />  
                                                        </div>
                                                    </div>                                                    
                                                </div>                                                
                                            </div>

                                        </asp:Panel>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Panel ID="pnExternalStatus" GroupingText="External Status" runat="server">

                                            <div class="form-row"> 
                                                <div class="col-md-3"></div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblClaimStatus" Text="Claim Status" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtClaimStatus" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-3"></div>
                                            </div>

                                            <div class="form-row last"> 
                                                <div class="col-md-3"></div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblTotalAmount" Text="Total Amount Approved" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtTotalAmount" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-md-3"></div>
                                            </div>

                                        </asp:Panel>

                                        <br />

                                        <asp:Panel ID="pnActions" GroupingText="Actions" CssClass="last" runat="server">

                                            <asp:Panel ID="pnSubActionComment" GroupingText="Comments" runat="server">
                                                <div class="form-row last">
                                                    <div class="col-md-6">
                                                        <asp:Button ID="btnAddComment" Text="Add Comments" CssClass="btn btn-primary" runat="server" />
                                                    </div>
                                                    <div class="col-md-6">
                                                        <asp:Button ID="btnSeeComments" Text="See Comments" CssClass="btn btn-primary" runat="server" />
                                                    </div>
                                                </div>
                                            </asp:Panel>  
                                            
                                            <br />

                                            <asp:Panel ID="pnSubActionFiles" GroupingText="Files" runat="server">
                                                <div class="form-row last">
                                                    <div class="col-md-6">
                                                        <asp:Button ID="btnAddFiles" Text="Add Files" CssClass="btn btn-primary" runat="server" />
                                                    </div>
                                                    <div class="col-md-6">
                                                        <asp:Button ID="btnSeeFiles" Text="See Files" CssClass="btn btn-primary" runat="server" />
                                                    </div>
                                                </div>
                                            </asp:Panel>   
                                            
                                            <br />

                                            <asp:Panel ID="pnSubActionFinal" GroupingText="Final Actions" runat="server">
                                                <div class="form-row last">
                                                    <div class="col-md-6">
                                                        <asp:Button ID="bntPurchasing" Text="Send to Purchasing" CssClass="btn btn-primary" runat="server" />
                                                    </div>
                                                    <div class="col-md-6">
                                                        <asp:Button ID="btnCloseClaim" Text="Close Claim" CssClass="btn btn-primary" runat="server" />
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                        </asp:Panel>
                                    </div>
                                </div>                                
                            </div>                            
                        </div>
                        <div class="tab-pane" id="claim-description">
                            <div class="col-md-12">
                                <h3>DESCRIPTION</h3>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Panel ID="pnDescription" GroupingText="Claim Description" runat="server">
                                            <div class="form-row last">
                                                <div class="col-md-12">
                                                    <asp:Label ID="lblClaimDesc" Text="Description" CssClass="control-label" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtClaimDesc" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox>                                                    
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </div>                                
                            </div>                            
                        </div>
                    </div>
                </div>     
                <div class="row">
                    <asp:Button ID="btnClose" CssClass="btn btn-primary" Text="Close" OnClick="btnClose_Click" runat="server" />
                </div>
            </div>
            
            <br />

        </ContentTemplate>

    </asp:UpdatePanel>    
    
    <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>

    <script type="text/javascript">

        // show import excel panel
        $('body').on('click', '#MainContent_btnGetTemplate', function (e) {
            //debugger

            $('#<%=hdShowTemplate.ClientID %>').val("1")
            $('#MainContent_btnImportExcel').removeClass('hideProp')
        });

        // show import excel panel
        $('body').on('click', '#MainContent_btnImportExcel', function (e) {
            //debugger

            var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdFile == "0")
                $('#<%=hdFileImportFlag.ClientID %>').val("1")
        });

        $('body').on('click', '#MainContent_btnSave', function (e) {
            //debugger

            $('#<%=hdShowTemplate.ClientID %>').val("0")
            $('#MainContent_btnImportExcel').addClass('hideProp')
        });

        //btnBack
        //btnSave

        //region para retener el valor de los combobox

        
        //var gettext = value.options[value.selectedIndex].selectedIndex;  

        $('body').on('change', "#<%=ddlSearchExtStatus.ClientID %>", function () {
            var value = document.getElementById("<%=ddlSearchExtStatus.ClientID %>");            
            var gettext = value.options[value.selectedIndex].text; 
            var getindex = value.options[value.selectedIndex].value;  
            $('#<%=hdStatusOut.ClientID %>').val(getindex); 

            var value1 = document.getElementById("<%=ddlSearchExtStatus.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1);

        });

        $('body').on('change', "#<%=ddlSearchDiagnose.ClientID %>", function () {
            var value = document.getElementById("<%=ddlSearchDiagnose.ClientID %>");            
            var gettext = value.options[value.selectedIndex].text; 
            var getindex = value.options[value.selectedIndex].value;  
            $('#<%=hdDiagnose.ClientID %>').val(getindex);

            var value1 = document.getElementById("<%=ddlSearchDiagnose.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1); 
        });

        $('body').on('change', "#<%=ddlSearchReason.ClientID %>", function () {
            debugger

            var value = document.getElementById("<%=ddlSearchReason.ClientID %>");            
            var gettext = value.options[value.selectedIndex].text;
            var getindex = value.options[value.selectedIndex].value;  
            $('#<%=hdReason.ClientID %>').val(getindex);  

            var value1 = document.getElementById("<%=ddlSearchReason.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1);             
            
         });

        //close import excel panel
        $('body').on('click', "#MainContent_btnBack", function (e) {
            //debugger

            var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdFile == "1")                
                $('#<%=hdFileImportFlag.ClientID %>').val("0")
        }); 

        function checkHdImportExcel(control) {
            var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdFile == "0")
                $('#<%=hdFileImportFlag.ClientID %>').val("1")
            else
                $('#<%=hdFileImportFlag.ClientID %>').val("0")
        }

        var divname;
        //function SearchText() {
        //    $("#txtClaimNo").autocomplete({
        //        source: function (request, response) {
        //            $.ajax({
        //                type: "POST",
        //                contentType: "application/json; charset=utf-8",
        //                url: "Default.aspx/GetEmployeeName",
        //                data: "{'empName':'" + document.getElementById('txtClaimNo').value + "'}",
        //                dataType: "json",
        //                success: function (data) {
        //                    response(data.d);
        //                },
        //                error: function (result) {
        //                    alert("No Match");
        //                }
        //            });
        //        }
        //    });
        //}       
        <%--function OnContactSelected(source, eventArgs) {
            debugger
            var hdnValueID = "<%= hdnValue.ClientID %>";
            

            document.getElementById(hdnValueID).value = eventArgs.get_text();
            //document.getElementById(hdnIdModelo).value = eventArgs.get_value();

            //if (MainContent_idValueModelo.value != 0) {
                __doPostBack(hdnValueID, "");
            //}
            //else {
                //__doPostBack(hdnValueID, "");
                //ErrorModelo();
            //}
        }--%>

        function afterRadioCheck(hdFieldId, divId) { 
            //debugger

            <%--if (divId.className == "collapse show") {
                $('#<%=hiddenId1.ClientID %>').val("1");
            } else {
                $('#<%=hiddenId1.ClientID %>').val("0");
            }--%>

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        <%--function isActivePanel(activePanel, valorActive) {
            debugger

            if (valorActive == 1) {
                if (activePanel.className == "collapse") {
                    $('#<%=hiddenId1.ClientID %>').val("1");
                } else {
                    $('#<%=hiddenId1.ClientID %>').val("0");
                }
            }
            if (valorActive == 2) {
                if (activePanel.className == "collapse") {
                    $('#<%=hiddenId2.ClientID %>').val("1");
                } else {
                    $('#<%=hiddenId2.ClientID %>').val("0");
                }
            }            
        }--%>        

        $(function () {

            debugger

            var watermarkSearch = 'Search...';

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);

            var collapse1 = document.getElementById('collapseOne');
            afterDdlCheck(hd1, collapse1);  

            //$('#accordion_2 h5 a').click(function () {
            //    var collapse1 = document.getElementById('collapseOne_2');
            //    isActivePanel(collapse1, 2);
            //});

            //$('#accordion h5 a').click(function () {
            //    var collapse1 = document.getElementById('collapseOne');
            //    isActivePanel(collapse1, 1);
            //});            


            claimNoAutoComplete()
            PartNoAutoComplete()
            CustomerNoAutoComplete() 
            
            divexpandcollapse(divname)

            
            //$('#MainContent_txtDateTo').datepicker(
            //    {
            //        dateFormat: 'MM/dd/yyyy',
            //        changeMonth: true,
            //        changeYear: true,
            //        yearRange: '1950:2100'
            //    }); 

            //$('#MainContent_txtDateInit').datepicker(
            //    {
            //        dateFormat: 'MM/dd/yyyy',
            //        changeMonth: true,
            //        changeYear: true,
            //        yearRange: '1950:2100'
            //    }); 

            $.datepicker.setDefaults($.datepicker.regional['en']);

            <%--$("#<%= txtDateTo.ClientID %>").datepicker({
                dateFormat: 'mm/dd/yy',
                autoClose: true
                //,               
                //onSelect: function () {
                //    selectedDate = $.datepicker.formatDate("yyyy/mm/dd", $(this).datepicker());
                //}
            });

            $("#<%= txtDateInit.ClientID %>").datepicker({
                dateFormat: 'mm/dd/yy',
                autoClose: true
                //,
                //onSelect: function () {
                //    selectedDate = $.datepicker.formatDate("yyyy/mm/dd", $(this).datepicker());
                //}
            });--%>

            var dateFormat = "mm/dd/yy",
                from = $("#<%= txtDateInit.ClientID %>")
                    .datepicker({
                        defaultDate: "+1w",
                        changeMonth: true,
                        dateFormat: 'mm/dd/yy',
                        autoClose: true,
                        numberOfMonths: 3
                    })
                    .on("change", function () {
                        to.datepicker("option", "minDate", getDate(this));
                    }),
                to = $("#<%= txtDateTo.ClientID %>").datepicker({
                    defaultDate: "+1w",
                    changeMonth: true,
                    dateFormat: 'mm/dd/yy',
                    autoClose: true,
                    numberOfMonths: 3
                })
                    .on("change", function () {
                        from.datepicker("option", "maxDate", getDate(this));
                    });

            function getDate(element) {
                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }

                return date;
            }

            //alert("function");

            //processDDLSelection()

        });    
            
            //var dim = $(div).find(".fa-plus").length
            //$(div).each(function () {
            //    if ($(this).attr('class') == ".fa-plus") {
            //        alert("pepe");
            //    }
            //});
            //if (dim == 1) {
            //    div.style.display = "inline"; 
            //}
            //else {
            //    div.style.display = "none"; 
            //}
            ////var spanClass = div.attr('class');
            ////var child = $(div)[0];   
            //var form1 = $(div).siblings('.fa-plus').eq(1);
            //var ee = $(this).closest(div).find('.fa-plus').first();
            //var child = $(div)[0].find('.fa-plus').first();
            ////var child = $(divname).attr('class');  
            //var pepe2 = $(divname + ":first-child");
            //var pepe1 = $(divname + ":first-child").closest(divname).first();
            //var tony = pepe;
            ////var root = document.getElementsByClassName(img);   
            ////console.log(root);
            ////var firstChild = root.children[0];    
            //            //var x = document.getElementById(img);
            ////console.log(x);
            ////x.getElementsByClassName("child")[1].style.backgroundColor = "red";


            //if ($("#" + img).attr("src") == "images/plus.png") {
            //    $("#" + img).closest("tr").after("<tr><td></td><td colspan = '100%' > " + $("#" + divname).html() + "</td></tr>")
            //    $("#" + img).attr("src", "images/minus.png");
            //} else {
            //    $("#" + img).closest("tr").next().remove();
            //    $("#" + img).attr("src", "images/plus.png");
            //}
        //}

        function claimNoAutoComplete() {
            $(".autosuggestclaim").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "Claims-Report.aspx/GetAutoCompleteDataClaimNo",
                        data: "{'prefixText':'" + document.getElementById("<%=txtClaimNo.ClientID %>").value + "'}",
                        dataType: "json",
                        autoFocus: true,
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });
                },
                select: function (event, ui) {      
                    var autocomplete_value = ui.item;                       
                    $("#<%=hdClaimNoSelected.ClientID %>").val(autocomplete_value.value);
                    __doPostBack("#<%=hdClaimNoSelected.ClientID %>", "");
                }
            });
        }

        function PartNoAutoComplete() {
            $(".autosuggestpart").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "Claims-Report.aspx/GetAutoCompleteDataPartNo",
                        data: "{'prefixText':'" + document.getElementById("<%=txtPartNo.ClientID %>").value + "'}",
                        dataType: "json",
                        autoFocus: true,
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });
                },
                select: function (event, ui) {
                    var autocomplete_value = ui.item;
                    //alert(autocomplete_value.value);
                    $("#<%=hdPartNoSelected.ClientID %>").val(autocomplete_value.value);
                    __doPostBack("#<%=hdPartNoSelected.ClientID %>", "");
                }
            });
        }

        var prmInstance = Sys.WebForms.PageRequestManager.getInstance();
        prmInstance.add_endRequest(function () {
            //you need to re-bind your jquery events here
            debugger

            claimNoAutoComplete();
            PartNoAutoComplete();
            CustomerNoAutoComplete();
        });

        function CustomerNoAutoComplete() {
            $(".autosuggestcustomer").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "Claims-Report.aspx/GetAutoCompleteDataCustomerNo",
                        data: "{'prefixText':'" + document.getElementById("<%=txtCustomer.ClientID %>").value + "'}",
                        dataType: "json",
                        autoFocus: true,
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });
                },
                select: function (event, ui) {
                    var autocomplete_value = ui.item;
                    $("#<%=hdCustomerNoSelected.ClientID %>").val(autocomplete_value.value);
                    __doPostBack("#<%=hdCustomerNoSelected.ClientID %>", "");
                }
            });
        }

        function pageLoad(event, args) {
            debugger

            //alert("PageLoad");

            if (args.get_isPartialLoad()) {  

                //case fileExcel  
                var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
                if (hdFile == "1") {
                    $('#MainContent_loadFileSection').closest('.container').removeClass('hideProp')
                } 

                var hdShowBtn = document.getElementById('<%=hdShowTemplate.ClientID%>').value;
                if (hdShowBtn = "1") {
                    $('#MainContent_btnImportExcel').removeClass('hideProp')
                }
                else {
                    $('#MainContent_btnImportExcel').addClass('hideProp')
                }

                var hd = document.getElementById('<%=hdLinkExpand.ClientID%>').value;
                var hd1 = document.getElementById('<%=hdTriggeredControl.ClientID%>').value;
                var hd2 = document.getElementById('<%=hdLaunchControl.ClientID%>').value;

                var iAccess = $("#div" + hd1);
                var iContainer = $("#" + hd2);                

                var iValue = iContainer.children(0).children(0)     
                //var iClass = iValue.attr('class');
                var iClass = document.getElementById('<%=hdSelectedClass.ClientID%>').value;
                var iCurrentClass = iValue.attr('class');
                                
                if (iClass == "fa fa-plus") {

                    //var ok1 = iAccess.attr('class');
                    //var iClass1 = iValue.attr('class');

                    if (iAccess.attr('class') != "divCustomClassOk") {
                        iAccess.toggleClass('divCustomClass divCustomClassOk');
                    }

                    if (iClass != "fa fa-minus" && iCurrentClass == iClass) {
                        iValue.toggleClass('fa-plus fa-minus');
                    }
                    
                    iAccess.closest('td').removeClass('padding0');

                    //iAccess.removeClass('divCustomClass');
                    //iAccess.addClass('divCustomClassOk');

                    //iValue.addClass('fa').removeClass('fa');
                    //iValue.addClass('fa-minus').removeClass('fa-plus');
                    //iValue.toggleClass('fa-plus fa-minus');//.removeClass('fa fa-plus');

                    //$('#MainContent_loadFileSection').closest('.container').removeClass('hideProp')
                }
                else {

                    //var ok2 = iAccess.attr('class');
                    //var iClass = iValue.attr('class');

                    if (iAccess.attr('class') != "divCustomClass") {
                        iAccess.toggleClass('divCustomClassOk divCustomClass');
                    }

                    if (iClass != "fa fa-plus" && iCurrentClass == iClass) {
                        iValue.toggleClass('fa-minus fa-plus');
                    }
                    
                    iAccess.closest('td').addClass('padding0');

                    //iAccess.removeClass('divCustomClassOk');
                    //iAccess.addClass('divCustomClass');

                    //iValue.addClass('fa').removeClass('fa');
                    //iValue.addClass('fa-plus').removeClass('fa-minus');
                    //iValue.toggleClass('fa-minus fa-plus');//.removeClass('fa fa-minus');
                }


                $('#<%=hdLinkExpand.ClientID %>').val("0");               
            }

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;

            var hdName = document.getElementById('<%=hiddenName.ClientID%>').value;
            //$('#accordion_2 h5 a').click(function () {

            yesnoCheckCustom(hdName)           

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);

            var collapse1 = document.getElementById('collapseOne');
            afterDdlCheck(hd1, collapse1);  

            claimNoAutoComplete()
            PartNoAutoComplete()
            CustomerNoAutoComplete()
            
            var hdFil = document.getElementById('<%=selectedFilter.ClientID%>').value;
            processDDLSelection(hdFil)

            //$('#MainContent_txtDateTo').datepicker(
            //    {
            //        dateFormat: 'MM/dd/yyyy',
            //        changeMonth: true,
            //        changeYear: true,
            //        yearRange: '1950:2100'
            //    });

            //$('#MainContent_txtDateInit').datepicker(
            //    {
            //        dateFormat: 'MM/dd/yyyy',
            //        changeMonth: true,
            //        changeYear: true,
            //        yearRange: '1950:2100'
            //    });

            $.datepicker.setDefaults($.datepicker.regional['en']);

            <%--$("#<%= txtDateTo.ClientID %>").datepicker({
                dateFormat: 'mm/dd/yy',
                autoClose: true
                //,
                //onSelect: function () {
                //    selectedDate = $.datepicker.formatDate("yyyy/mm/dd", $(this).datepicker());
                //}
            });

            $("#<%= txtDateInit.ClientID %>").datepicker({
                dateFormat: 'mm/dd/yy',
                autoclose: true
                //,
                //onSelect: function () {
                //    selectedDate = $.datepicker.formatDate("yyyy/mm/dd", $(this).datepicker());
                //}
            });--%>

            var dateFormat = "mm/dd/yy",
                from = $("#<%= txtDateInit.ClientID %>")
                    .datepicker({
                        defaultDate: "+1w",
                        changeMonth: true,
                        dateFormat: 'mm/dd/yy',
                        autoClose: true,
                        numberOfMonths: 1
                    })
                    .on("change", function () {
                        to.datepicker("option", "minDate", getDate(this));
                    }),
                to = $("#<%= txtDateTo.ClientID %>").datepicker({
                    defaultDate: "+1w",
                    changeMonth: true,
                    dateFormat: 'mm/dd/yy',
                    autoClose: true,
                    numberOfMonths: 1
                })
                    .on("change", function () {
                        from.datepicker("option", "maxDate", getDate(this));
                    });

            function getDate(element) {
                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }

                return date;
            }

        } 

        //function cleanDDLRows() {
        //    debugger

        //    var divs = ["rowUser", "rowIntStatus", "rowExtStatus", "rowDiagnose", "rowReason", "rowClaimNo", "rowDateInit", "rowCustomer", "rowPartNo"];
        //    for (i = 0; i < divs.length; i++) {
        //        var id = divs[i]
        //        var x = document.getElementById(id);
        //        var xstyle = x.style;
        //        var test = xstyle.display;
        //        if (xstyle.display == "block") {
        //            x.style.display = "none";
        //        }
        //    }

        //}

        function processDDLSelection(filter) {
            debugger

            if (filter == "MainContent_ddlSearchReason") {
                var hdRe = document.getElementById('<%=hdReason.ClientID%>').value;
                $("#<%=ddlSearchReason.ClientID%>").prop('selectedIndex', parseInt(hdRe) + 1);

            }
            else if (filter == "MainContent_ddlSearchDiagnose") {
                var hdDia = document.getElementById('<%=hdDiagnose.ClientID%>').value;
                $("#<%=ddlSearchDiagnose.ClientID%>").prop('selectedIndex', parseInt(hdDia) + 1);

            }
            else if (filter == "MainContent_ddlSearchExtStatus") {
                var hdSt = document.getElementById('<%=hdStatusOut.ClientID%>').value; 
                $("#<%=ddlSearchExtStatus.ClientID%>").prop('selectedIndex', parseInt(hdSt) + 1);
            }

            <%--var hdRe = document.getElementById('<%=hdReason.ClientID%>').value;
            var hdDia = document.getElementById('<%=hdDiagnose.ClientID%>').value;
            var hdSt = document.getElementById('<%=hdStatusOut.ClientID%>').value; 

            $("#<%=ddlSearchReason.ClientID%>").prop('selectedIndex', parseInt(hdRe) + 1);
            $("#<%=ddlSearchDiagnose.ClientID%>").prop('selectedIndex', parseInt(hdDia) + 1);
            $("#<%=ddlSearchExtStatus.ClientID%>").prop('selectedIndex', parseInt(hdSt) + 1); --%>           
        }

        function yesnoCheckCustom(id) {
            //debugger

            if (id != "") {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowUser", "rowIntStatus", "rowExtStatus", "rowDiagnose", "rowReason", "rowClaimNo", "rowDateInit", "rowCustomer", "rowPartNo"];

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
                        xstyle.display = "flex";
                        $('#<%=hiddenName.ClientID %>').val(id);
                        //x.display = "block";
                    }
                }
            //isActivePanel(collapse1, 1);
            //isActivePanel(collapse2, 2);
            }
            
        }

        function yesnoCheck(id) {
            //debugger

            x = document.getElementById(id);
            xstyle = document.getElementById(id).style;

            var divs = ["rowUser", "rowIntStatus", "rowExtStatus", "rowDiagnose", "rowReason", "rowClaimNo", "rowDateInit", "rowCustomer", "rowPartNo"];

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
                    xstyle.display = "flex";
                    $('#<%=hiddenName.ClientID %>').val(id);
                    //x.display = "block";
                }
            }
            var collapse1 = document.getElementById('collapseOne');
            var collapse2 = document.getElementById('collapseOne_2');

            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd1

            if (hd2 == 1)
            {
                $('#<%=hiddenId1.ClientID %>').val("1");
                hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            }
            else { hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value; }
            
            afterRadioCheck(hd1, collapse1)
            afterRadioCheck(hd2, collapse2)
            //isActivePanel(collapse1, 1);
            //isActivePanel(collapse2, 2);
        }

    </script>

</asp:content>

