<%@ Page Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" Async="true" CodeBehind="Lost-Sales.aspx.vb" Inherits="WebApplication1.Lost_Sales" EnableEventValidation="false" ViewStateMode="Disabled"  %>

<%--EnableViewState="true" ViewStateMode="Disabled"--%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="updatepnl1" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="submit" /> 
            <asp:PostBackTrigger ControlID="btnExcel" /> 
            <asp:PostBackTrigger ControlID="btnFullExcel" /> 
            <%--<asp:AsyncPostBackTrigger ControlID="ddlVendAssign" /> --%>           
        </Triggers>
        <ContentTemplate> 

            <div class="row">
                <div class="col-md-9"></div>
                <div class="col-md-2">
                    <asp:Label ID="lblUserLogged" Text="" runat="server"></asp:Label>
                </div>
                <div class="col-md-1">
                    <asp:LinkButton ID="lnkLogout" Text="Click to Logout." OnClick="lnkLogout_Click" runat="server"></asp:LinkButton>
                </div>
            </div>

            <div class="container-fluid">
                <div class="breadcrumb-area breadcrumb-bg">
                    <div class="row">
                        <div class="col-md-offset-4 col-md-8">
                            <div class="breadcrumb-inner">
                                <div class="row">
                                    <div class="col-md-11">
                                        <div id="lsBd" class="bread-crumb-inner">
                                            <div class="breadcrumb-area page-list">
                                                <div class="row">
                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-7 link">
                                                        <i class="fa fa-map-marker"></i>
                                                        <a href="/Default">Home</a>
                                                        " - "
                                                    <span>Lost Sales</span>
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
            </div>     

            <div class="container-fluid">          
                <div id="rowFilters" class="row" runat="server">                    
                    <div class="col-md-2">
                        <asp:Panel ID="pnDefValues" CssClass="pnFilterStyles" GroupingText="DEFAULT VALUES" runat="server">
                            <ul class="checklist">
                                <li><i class="fa fa-check"></i><span id="spnCountItems">COUNT ITEMS:</span><asp:Label ID="lblItemsCount" runat="server"></asp:Label></li>
                                <li><i class="fa fa-check"></i><span id="spnTimesQuotes">TIMES QUOTE: (Current value): </span>
                                    <asp:Label ID="lblTimesQuote" runat="server"></asp:Label>
                                </li>
                            </ul>
                        </asp:Panel>
                        <div class="accordion-wrapper hideProp">
                            <div id="accordion">
                                <div class="card">
                                    <!--ACCORDION DEFAULT VALUES HEADER-->
                                    <div class="card-header" id="headingOne">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                                <span class="">DEFAULT VALUES <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>

                                    <!--ACCORDION: CONTENT-->
                                    <div id="collapseOne" class="collapse show" aria-labelledby="headingOne" data-parent="#accordion" style="">
                                        <div id="card-body-custom" class="card-body">
                                            
                                        </div>
                                    </div>
                                    <!-- COLLAPSE CONTENT END -->
                                </div>
                                <!-- CARD END -->
                            </div>
                            <!-- ACCORDION END -->
                        </div>
                    </div>                    
                    <div class="col-md-3">
                        <asp:Panel ID="pnFilters" CssClass="pnFilterStyles" GroupingText="FILTER DATA" runat="server">
                            <div id="rowRadios" class="row">
                                <%--<div style="width:100%">--%>
                                <input type="hidden" name="oldtimesquoted" value="100">
                                <div class="form-group col-md-3 radio-toolbar">
                                    <label class="form-check">
                                        <p>10+</p>
                                        <asp:RadioButton ID="tqr10" GroupName="radio" OnCheckedChanged="tqr10_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                        <span class="checkmark"></span>
                                    </label>
                                </div>                                
                                <div class="form-group col-md-3 radio-toolbar">
                                    <label class="form-check">
                                        <p>50+</p>
                                        <asp:RadioButton ID="tqr50" GroupName="radio" OnCheckedChanged="tqr50_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                        <span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="form-group col-md-3 radio-toolbar">
                                    <label class="form-check">
                                        <p>100+</p>
                                        <asp:RadioButton ID="tqr100" GroupName="radio" OnCheckedChanged="tqr100_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                        <span class="checkmark"></span>
                                    </label>
                                </div>
                                <div class="form-group col-md-3 radio-toolbar">
                                    <label class="form-check">
                                        <p>200+</p>
                                        <asp:RadioButton ID="tqr200" GroupName="radio" OnCheckedChanged="tqr200_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                        <span class="checkmark"></span>
                                    </label>
                                </div>
                                <%--</div>--%>
                            </div>
                            <div class="row hideProp">
                                <!--SHORT WAY-->
                                <!--times quote : rendering-->
                                <div class="col-md-6">
                                    <asp:Label ID="lbltqId" Text="TIMES QUOTE:10-150" runat="server"></asp:Label>
                                    <asp:TextBox name="num-tq" ID="tqId" class="form-control" TextMode="Number" min="10" max="150" OnTextChanged="tqId_TextChanged" runat="server" title="TIMES QUOTE:10-150"></asp:TextBox>
                                    <%--<asp:RangeValidator ErrorMessage="The value must be from 10 to 150!" ControlToValidate="tqId" MinimumValue="10" MaximumValue="150" Type="Integer" EnableClientScript="false" runat="server" />--%>
                                </div>

                                <!--LONG WAY-->
                                <!--vendor assigned : rendering-->
                                <div class="col-md-6 hideProp">
                                    <asp:Label ID="lblVndAss" Text="Vendors Assigned" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlVendAssign" name="sel-vndassigned" AutoPostBack="true" class="form-control" title="Both: It shows Parts with vendors assigned and without vendors assigned at the same time." ViewStateMode="enabled" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <!-- cross site forgery's attack avoiding -->
                            <div class="col-md-1">
                                <input type="hidden" name="csrf" value="b3f24ac9359094f7b4629613138570a6-106b16695033660d3701da01a206aeba">
                            </div>

                            <!-- SUBMIT BUTTON AND CONVERT TO EXCEL THE ACTUAL PAGE -->
                            <div id="rowBtnFilters" class="row make-it-flex">

                                <div class="col-xs-12 col-sm-6 flex-item-1 padd-fixed" style="float: right;">
                                    <asp:Button ID="submit" class="btn btn-primary btn-lg float-right btnFullSize" runat="server" Text="Submit" />
                                </div>
                                <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed hideProp">
                                    <asp:Button ID="convert" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Convert to Excel" />
                                </div>

                            </div>
                        </asp:Panel>
                        <div class="accordion-wrapper hideProp">
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
                                        <div id="collapseOne_2" class="collapse show" aria-labelledby="headingOne_2" data-parent="#accordion_2" style="">
                                            <div class="card-body">
                                                
                                            </div>                                    
                                        </div>
                                        <!-- COLLAPSE CONTENT END -->                            
                                </div>
                                <!-- CARD END -->
                            </div>
                            <!-- ACCORDION END -->
                        </div>

                    </div>  
                    <div id="col1-custom" class="col-md-1"></div>
                    <div class="col-md-3">
                        <asp:Panel ID="pnExtraFilters" CssClass="pnFilterStyles" GroupingText="EXTRA FILTERS" runat="server">
                            <div id="rowRadios1" class="form-group col-md-12">
                                <div class="row">
                                    <div class="form-group col-md-6 radio-toolbar">
                                        <label class="form-check">
                                            <p>Category</p>
                                            <asp:RadioButton ID="rdCategory" OnCheckedChanged="rdCategory_CheckedChanged" onclick="yesnoCheck('rowCategory');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>

                                    <div class="form-group col-md-6 radio-toolbar">
                                        <label class="form-check">
                                            <p>Major</p>
                                            <asp:RadioButton ID="rdMajor" OnCheckedChanged="rdMajor_CheckedChanged" onclick="yesnoCheck('rowMajor');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>
                                    
                                </div>

                                <div class="row">  
                                    <div class="form-group col-md-6 radio-toolbar hideProp">
                                        <label class="form-check">
                                            <p>Vendor Name</p>
                                            <asp:RadioButton ID="rdVndName" OnCheckedChanged="rdVndName_CheckedChanged" onclick="javascript:yesnoCheck('rowVndName');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>

                                    <div class="form-group col-md-6 radio-toolbar hideProp">
                                        <label class="form-check">
                                            <p>Wish List</p>
                                            <asp:RadioButton ID="rdWL" OnCheckedChanged="rdWL_CheckedChanged" onclick="yesnoCheck('rowWL');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="form-group col-md-6 radio-toolbar hideProp">
                                        <label class="form-check">
                                            <p>Sale Last 12</p>
                                            <asp:RadioButton ID="rdLast12" OnCheckedChanged="rdLast12_CheckedChanged" onclick="yesnoCheck('rowLast12');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="accordion-wrapper hideProp">
                            <div id="accordion_3">
                                <div class="card">
                                    <div class="card-header" id="headingOne_3">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_3" aria-expanded="false" aria-controls="collapseOne_3">
                                                <span class="">EXTRA FILTERS  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>
                                    <div id="collapseOne_3" class="collapse show" aria-labelledby="headingOne_3" data-parent="#accordion_3" style="">
                                            <div class="card-body">
                                                
                                            </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Panel ID="pnFilterCriteria" CssClass="pnFilterStyles" GroupingText="FILTER CRITERIA" runat="server">
                            <!--search by Category-->
                            <div id="rowCategory" class="rowCategory" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Category</label>
                                    <br>
                                    <asp:DropDownList ID="ddlCategory" name="sel-vndassigned" class="form-control" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Category." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <!--search by VendorName-->
                            <div id="rowVndName" class="rowVndName" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Vendor Name</label>
                                    <br>
                                    <asp:DropDownList ID="ddlVendorName" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlVendorName_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Vendor Name." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <!--search by Major-->
                            <div id="rowMajor" class="rowMajor" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Major</label>
                                    <br>
                                    <asp:DropDownList ID="ddlMajor" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlMajor_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Major Code." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <!--search by WishList-->
                            <div id="rowWL" class="rowWL" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Wish List</label>
                                    <br>
                                    <asp:DropDownList ID="ddlWishList" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlWishList_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Wish List." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <!--search by SaleLast12-->
                            <div id="rowLast12" class="rowLast12" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Sale Last 12</label>
                                    <br>
                                    <asp:DropDownList ID="ddlSaleLast12" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlSaleLast12_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Sale Last 12." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="accordion-wrapper hideProp">
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
                                             
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>   
            
            <div id="addPerPech" class="container hideProp" runat="server">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-8">
                        <div id="pnUpdatePart2" class="shadow-to-box" style="width: 80% !important;">
                            <div class="row" style="padding: 25px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-10" style="text-align: center !important;"><span id="spnUpdatePart2">add person in charge</span></div>
                                <div class="col-md-1"></div>
                            </div>
                            <div class="form-row" style="padding: 20px 10px 2px 10px;"> 
                                <div class="form-group col-md-6" style="text-align: justify;font-size: 12px;padding: 15px;">
                                    <asp:Label ID="lblSelectedPart" CssClass="label-style" Text="" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:Label>                                    
                                    <br />
                                    <asp:Label ID="lblDescription" CssClass="label-style" Text="Please select the person in charge for the reference that you are moving to Wish List" runat="server"></asp:Label>
                                </div>
                                <div class="form-group col-md-6">
                                    <asp:Label ID="lblUser2" CssClass="label-style" Text="assigned to" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlUser2" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlUser2_SelectedIndexChanged" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                </div>
                                <%--<div class="form-group col-md-6 hideProp">
                                    <asp:Label ID="lblStatus3" CssClass="label-style" Text="status" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlStatus3" OnSelectedIndexChanged="ddlStatus3_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                </div>--%>
                            </div> 
                            <div class="form-row" style="padding: 5px 1px 20px 1px;">
                                <div class="col-md-2"></div>
                                <div class="col-md-8">
                                    <asp:CheckBox ID="chkToWS" runat="server" />
                                    <asp:Label ID="lblToWS" Text="Do you want to  add this reference /s to the Wish list? "  runat="server"></asp:Label>
                                </div>   
                                <div class="col-md-2"></div>
                            </div>
                            <div class="form-row">
                                <div class="form-group col-md-6" style="float: right; text-align: right !important;">
                                    <asp:Button ID="btnUpdate3" Text="assign" class="btn btn-primary btn-lg btnMidSize" OnClick="btnUpdate3_Click" runat="server" />
                                </div>
                                <div class="form-group col-md-6" style="float: left;">
                                    <asp:Button ID="btnBack3" Text="   Back   " class="btn btn-primary btn-lg btnMidSize" runat="server" />
                                </div>
                            </div>
                        </div>                
                    </div>
                    <div class="col-md-2 hideProp">
                        <div id="pnDisplayImage2" class="shadow-to-box hideProp">    
                            <div class="row" style="padding: 20px 0;">
                                <asp:Image ID="Image1" ImageUrl="~/Images/mapall.PNG" style="padding: 0 50px;" runat="server" />
                            </div>                    
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="container-fluid">
                <div class="row">
                    <div class="col-md-3">
                        <div id="rowPageSize" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed">
                                <asp:Label ID="lblText1" Text="Show " runat="server"></asp:Label></div>
                            <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed">
                                <asp:DropDownList name="ddlPageSize" ID="ddlPageSize" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" ViewStateMode="enabled" class="form-control" runat="server"></asp:DropDownList></div>
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed">
                                <asp:Label ID="lblText2" Text=" entries." runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div id="rowBtnOpt" class="row">
                            <div class="col-xs-12 col-sm-4"></div>
                            <div class="col-xs-12 col-sm-2 flex-item-1 padd-fixed">
                                <asp:Button ID="btnExcel" class="btn btn-primary btn-lg float-right btnFullSize" runat="server" Text="Current Excel" />
                                <%--<asp:LinkButton class="boxed-btn-layout-2 btnFullSize" runat="server">
                                    <i class="fa fa-file-excel fa-1x" aria-hidden="true"> </i> <span>EXCEL</span>
                                </asp:LinkButton>--%>
                                <%--<asp:LinkButton ID="LinkButton1" class="boxed-btn-layout btn-rounded btnFullSize" OnClick="lnkReloadGrid_Click" runat="server">
                                    <i class="fa fa-retweet fa-1x" aria-hidden="true"> </i> <span>RELOAD LAST SEARCH</span>
                                </asp:LinkButton>--%>
                            </div>
                            <div class="col-xs-12 col-sm-2 flex-item-2 padd-fixed">
                                <asp:Button ID="btnFullExcel" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Full Excel" />
                            </div>
                            <div class="col-xs-12 col-sm-2 flex-item-3 padd-fixed">
                                <asp:Button ID="btnRestore" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Restore Data" />
                            </div>
                            <div class="col-xs-12 col-sm-4"></div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div id="rowBtnSearch" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" style="float: right;">
                                <%--<asp:LinkButton ID="lnkRefreshSearch" runat="server">
                                    <i class="fa fa-retweet fa-1x" aria-hidden="true"> </i> <span>RESTORE DATA</span>
                                </asp:LinkButton> --%>
                            </div>
                            <div class="col-xs-12 col-sm-5 flex-item-2 padd-fixed">
                                <asp:TextBox name="txtSearch" ID="txtSearch" class="form-control" runat="server"></asp:TextBox></div>
                            <div class="col-xs-12 col-sm-3 flex-item-2 padd-fixed">
                                <asp:Button name="btnSearch" ID="btnSearch" class="btn btn-primary btn-sm btnFullSize1" Text="Search" runat="server"></asp:Button></div>
                            <%--<div class="spinner-grow text-warning"></div>--%>
                        </div>
                        <div id="notVisibleKeyPress" style="display: none" runat="server">
                            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
                        </div>
                    </div>
                </div>
            </div>              

            <div class="row" style="display: none !important;">
                <%--<asp:DropDownList ID="ddlSaleLast12Foot" OnSelectedIndexChanged="ddlSaleLast12Foot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="ddlVndNameFoot" OnSelectedIndexChanged="ddlVndNameFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="ddlWLFoot" OnSelectedIndexChanged="ddlWLFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="ddlMajorFoot" OnSelectedIndexChanged="ddlMajorFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="ddlCategoryFoot" OnSelectedIndexChanged="ddlCategoryFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:DropDownList>--%>
                <%--<asp:Button ID="ButtonAdd1" class="btn btn-inverse btn-primary btn-sm" CommandName="AddAll" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" runat="server" Text="Add Selected" />
                <asp:LinkButton ID="lbSingleAdd" runat="server" TabIndex="1" CommandName="SingleAdd" ToolTip="Add Single Reference">
                                                    <span id="Span1" aria-hidden="true" runat="server">
                                                        <i class="fa fa-plus"></i></span>
                                                </asp:LinkButton>--%>
                <%--<asp:Button ID="ButtonAdd" class="btn btn-inverse btn-primary btn-sm" CommandName="AddAll" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>"
                                                    runat="server" Text="Add Selected" OnClick="ButtonAdd_Click" />--%>

                <asp:Label ID="lblGrvGroup" Text="test" runat="server"></asp:Label>
                <table id="ndtt" runat="server"></table>

                <%--collapsible initialization--%>
                <asp:HiddenField ID="hiddenId1" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId2" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId4" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId5" Value="0" runat="server" />

                <%--vendor Assign Dropdownlist value--%>
                <asp:HiddenField ID="hiddenId3" Value="2" runat="server" />

                <asp:HiddenField ID="hiddenName" Value="" runat="server" />

                <asp:HiddenField ID="hdLinkExpand" Value="0" runat="server" />
                <asp:HiddenField ID="hdTriggeredControl" Value="" runat="server" />
                <asp:HiddenField ID="hdLaunchControl" Value="" runat="server" />
                <asp:HiddenField ID="hdSelectedClass" Value="" runat="server" />

                <asp:HiddenField ID="hdCloseAction" Value="0" runat="server" />

                <asp:HiddenField ID="hdVendorAssigned" Value="0" runat="server" />
                <asp:HiddenField ID="hdSubmit" Value="0" runat="server" />

                <asp:HiddenField ID="hdCategory" Value="" runat="server" />
                <asp:HiddenField ID="hdVendorName" Value="" runat="server" />
                <asp:HiddenField ID="hdWishList" Value="" runat="server" />
                <asp:HiddenField ID="hdMajor" Value="" runat="server" />
                <asp:HiddenField ID="hdSaleLast12" Value="" runat="server" />

                <asp:HiddenField ID="selectedFilter" Value=""  runat="server" />

                <asp:HiddenField ID="hdShowUserAssignment" Value ="0" runat="server" />
                <asp:HiddenField ID="hdUserAssigment" Value ="0" runat="server" />

                <asp:HiddenField id="hdWelcomeMess" Value="" runat="server" />

            </div>

            <div class="container-fluid">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="form-horizontal">
                            <%--OnRowUpdating="grvLostSales_RowUpdating"--%>

                            <div id="rowGridView">
                                <asp:GridView ID="grvLostSales" runat="server" AutoGenerateColumns="false"
                                    PageSize="10" CssClass="table table-striped table-bordered" AllowPaging="True" AllowSorting="true"
                                    GridLines="None" OnRowCommand="grvLostSales_RowCommand" OnPageIndexChanging="grvLostSales_PageIndexChanging"
                                    OnRowDataBound="grvLostSales_RowDataBound" OnSorting="grvLostSales_Sorting" OnPreRender="grvLostSales_PreRender"
                                    ShowHeader="true" ShowFooter="true" DataKeyNames="IMPTN"  >
                                    <Columns>
                                        <asp:TemplateField ItemStyle-Width="3%">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkAll" Text="  " Visible="true" runat="server" OnCheckedChanged="chkAll_CheckedChanged" AutoPostBack="true"
                                                    ToolTip="Select All" EnableViewState="true" ViewStateMode="Enabled"></asp:CheckBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <span style="padding: 10px;">
                                                    <asp:CheckBox ID="chkSingleAdd" runat="server" ToolTip="Select to Wish List" />
                                                </span>
                                            </ItemTemplate>
                                            <FooterStyle HorizontalAlign="Right" />
                                            <FooterTemplate>
                                                <asp:LinkButton ID="ButtonAdd" runat="server" class="boxed-btn-layout btn-rounded" Text="" TabIndex="1" CommandName="AddAll" ToolTip="Add Multiple References"><p>Add All</p></asp:LinkButton>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Add" ItemStyle-Width="3%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lbSingleAdd"
                                                    runat="server"
                                                    TabIndex="1" CommandName="SingleAdd"
                                                    CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>"
                                                    ToolTip="Add Single Reference">
                                                    <span id="Span1" aria-hidden="true" runat="server">
                                                        <i class="fa fa-plus"></i></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="PART NUMBER" ItemStyle-Width="7%" SortExpression="IMPTN" >
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <EditItemTemplate>  
                                                <asp:Label ID="lblPartName" runat="server" Text='<%# Bind("IMPTN") %>' />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lbPartNo"
                                                    runat="server"
                                                    TabIndex="1" CommandName="UpdatePart"
                                                    ToolTip="Assign Person" CssClass="clickme" CommandArgument='<%#Eval("IMPTN") %>'>
                                                    <span id="Span2" aria-hidden="true" runat="server">
                                                         <asp:Label ID="txtPartName" Text='<%# Bind("IMPTN") %>' runat="server"></asp:Label>
                                                    </span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField> 

                                        <%--<asp:BoundField DataField="IMPTN" HeaderText="PART NUMBER" ItemStyle-Width="5%" SortExpression="IMPTN" />--%>
                                        <%-- 2 --%>
                                        <asp:BoundField DataField="IMDSC" HeaderText="DESCRIPTION" ItemStyle-Width="10%" SortExpression="IMDSC" />
                                        <asp:BoundField DataField="IMDS2" HeaderText="DESCRIPTION 2" ItemStyle-Width="7%" SortExpression="IMDS2" ItemStyle-CssClass="hidecol" HeaderStyle-CssClass="hidecol" />
                                        <asp:BoundField DataField="IMDS3" HeaderText="DESCRIPTION 3" ItemStyle-Width="7%" SortExpression="IMDS3" ItemStyle-CssClass="hidecol" HeaderStyle-CssClass="hidecol" />
                                        <asp:BoundField DataField="TQUOTE" HeaderText="QTY QTE" ItemStyle-Width="3%" SortExpression="TQUOTE" />
                                        <asp:BoundField DataField="TIMESQ" HeaderText="TIMES QTE" ItemStyle-Width="4%" SortExpression="TIMESQ" DataFormatString="{0:D}" />
                                        <asp:BoundField DataField="NCUS" HeaderText="CUSTS.QUOTE" ItemStyle-Width="4%" SortExpression="NCUS" />
                                        <asp:BoundField DataField="QTYSOLD" HeaderText="LAST12" ItemStyle-Width="4%" SortExpression="QTYSOLD" />
                                        <asp:BoundField DataField="VENDOR" HeaderText="VND NO" ItemStyle-Width="5%" SortExpression="VENDOR" />
                                        <asp:BoundField DataField="VENDORNAME" HeaderText="VND NAME" ItemStyle-Width="6%" SortExpression="VENDORNAME" />
                                        <asp:BoundField DataField="PAGENT" HeaderText="P.AGENT" ItemStyle-Width="6%" SortExpression="PAGENT" />
                                        <%-- 12 --%>
                                        <asp:BoundField DataField="IMPRC" HeaderText="LIST PRICE" ItemStyle-Width="4%" SortExpression="IMPRC" />
                                        <%--<asp:BoundField DataField="WLIST" HeaderText="WL" ItemStyle-Width="2%" SortExpression="WLIST" />
                                        <asp:BoundField DataField="PROJECT" HeaderText="DEV.PROJ" ItemStyle-Width="3%" SortExpression="PROJECT" />
                                        <asp:BoundField DataField="PROJSTATUS" HeaderText="DEV.STATUS" ItemStyle-Width="5%" SortExpression="PROJSTATUS" />--%>
                                        <asp:BoundField DataField="F20" HeaderText="LOC.20" ItemStyle-Width="2%" SortExpression="F20" />
                                        <asp:BoundField DataField="FOEM" HeaderText="OEM VND" ItemStyle-Width="3%" SortExpression="FOEM" />
                                        <asp:BoundField DataField="IMPC1" HeaderText="MAJOR" ItemStyle-Width="2%" SortExpression="IMPC1" />
                                        <asp:BoundField DataField="CATDESC" HeaderText="CATEGORY" ItemStyle-Width="10%" SortExpression="CATDESC" />
                                        <asp:BoundField DataField="IMPC2" HeaderText="MINOR" ItemStyle-Width="3%" SortExpression="IMPC2" ItemStyle-CssClass="hidecol" HeaderStyle-CssClass="hidecol" />
                                        <asp:BoundField DataField="MINDSC" HeaderText="DESC" ItemStyle-Width="9%" SortExpression="MINDSC" ItemStyle-CssClass="hidecol" HeaderStyle-CssClass="hidecol" />
                                        <asp:BoundField DataField="totalclients" HeaderText="T.Clients" ItemStyle-Width="4%" SortExpression="totalclients" />
                                        <asp:BoundField DataField="totalcountries" HeaderText="T.Countries" ItemStyle-Width="4%" SortExpression="totalcountries" />
                                        <asp:BoundField DataField="oempart" HeaderText="OEM Part" ItemStyle-Width="4%" SortExpression="oempart" />
                                        <asp:BoundField DataField="PRPECH" HeaderText="Person in Charge" ItemStyle-Width="6%" SortExpression="PRPECH" />
                                        <%--<asp:TemplateField HeaderText="Total Clients" ItemStyle-Width="3%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <HeaderTemplate>Total Clients</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTClients" Text="" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> --%>
                                        <%--<asp:TemplateField HeaderText="T.Countries" ItemStyle-Width="4%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <HeaderTemplate>T.Countries</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCountries" Text="" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <%--<asp:TemplateField HeaderText="OEM Part" ItemStyle-Width="3%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <HeaderTemplate>OEM Part</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblOEMPart" Text="" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="DETAILS">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDetails" runat="server" TabIndex="1" ToolTip="Get Reference Detail" CssClass="click-in" CommandName="show" 
                                                   OnClientClick= <%# String.Format("return divexpandcollapse(this,'{0}');return false", Eval("IMPTN").ToString())  %> >
                                                    <span id="Span16" aria-hidden="true" runat="server">
                                                        <i class="fa fa-folder"></i>
                                                    </span>
                                                </asp:LinkButton>
                                                </td>
			                                        <tr>
                                                        <td colspan="23" class="padding0">
                                                            <div id="div<%# Trim(Eval("IMPTN")) %>" class="divCustomClass">
                                                                <asp:GridView ID="grvDetails" runat="server" AutoGenerateColumns="false" GridLines="None">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="IMDS2" HeaderText="DESCRIPTION 2" ItemStyle-Width="15%" SortExpression="IMDS2" />
                                                                        <asp:BoundField DataField="IMDS3" HeaderText="DESCRIPTION 3" ItemStyle-Width="10%" SortExpression="IMDS3" />
                                                                        <asp:BoundField DataField="CATDESC" HeaderText="CATEGORY DESC" ItemStyle-Width="15%" SortExpression="CATDESC" />
                                                                        <asp:BoundField DataField="subcatdesc" HeaderText="SUBCATEGORY DESC" ItemStyle-Width="7%" SortExpression="subcatdesc" />
                                                                        <asp:BoundField DataField="mindsc" HeaderText="MINOR DESC" ItemStyle-Width="7%" SortExpression="mindsc" />
                                                                    </Columns>
                                                                    <HeaderStyle BackColor="#95B4CA" ForeColor="White" />
                                                                </asp:GridView>
                                                            </div>
                                                        </td>
                                                    </tr>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" PageButtonCount="10" />
                                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    <FooterStyle CssClass="footer-style" HorizontalAlign="Center" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="reloadGrid" class="container hideProp">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-4 fullTextBox centered">
                        <asp:LinkButton ID="lnkReloadGrid" class="boxed-btn-layout btn-rounded btnFullSize" OnClick="lnkReloadGrid_Click" runat="server">
                         <i class="fa fa-retweet fa-1x" aria-hidden="true"> </i> <span>RELOAD LAST SEARCH</span>
                        </asp:LinkButton>
                    </div>
                    <div class="col-md-4 fullTextBox centered">
                        <asp:LinkButton ID="lnkReloadBack" class="boxed-btn-layout btn-rounded btnFullSize" OnClick="lnkReloadBack_Click" runat="server">
                         <i class="fa fa-retweet fa-1x" aria-hidden="true"> </i> <span>RELOAD ALL DATA</span> 
                        </asp:LinkButton>
                    </div>
                    <div class="col-md-2"></div>
                </div>

                <%--<div id="testgr">
                    <a><span class="">FILTER DATA  <i class="fa fa-angle-down faicon"></i></span></a>
                </div> --%>               
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <%--<asp:UpdatePanel ID="updatepnl2" UpdateMode="Conditional" runat="server">
        <ContentTemplate>            

        </ContentTemplate>
    </asp:UpdatePanel>--%>

    <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />  
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="bootstrap4-input-clearer.js"></script>

    <script type="text/javascript">

        function messageFormSubmitted(mensaje, show) {
            debugger
            messages.alert(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
        }   

        function messageConfirmForAssigment(mensaje,show) {
            messages.confirm1(mensaje, { type: show });
        }

        //function test(controlid, valuee1) {
        function test(valuee1) {
            debugger

            //var ppa = controlid;
            //console.log(ppa);
            console.log(valuee1);
            alert(valuee1);

            //if (valuee1 == "2021") {
            //    alert("pepe");
            //}
            //else {
            //    alert("papo");
            //}            
        }

        function divexpandcollapse(controlid, divname) {            
            debugger

            console.log(controlid);
            console.log(divname);

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

                            if (iClass == "fa fa-folder" && $('#<%=hdLinkExpand.ClientID %>').val() == "0") {

                                //iAccess.toggleClass("divCustomClass divCustomClassOk");
                                //iAccess.removeClass('divCustomClass');
                                //iAccess.addClass('divCustomClassOk');

                                //iValue.addClass('fa').removeClass('fa');
                                //iValue.toggleClass('fa-plus fa-minus');//.removeClass('fa-plus');                                

                                //iAccess.closest('td').removeClass('padding0');

                                $('#<%=hdLinkExpand.ClientID %>').val("1");

                            } else if (iClass == "fa fa-folder-open" && $('#<%=hdLinkExpand.ClientID %>').val() == "0") {                                

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

        function setCollapseSelection() {
            //debugger

            //MainContent_ddlVendAssign
            $('#<%=ddlVendAssign.ClientID %>').change(function () {
                $('#<%=hiddenId3.ClientID %>').val("2");
            });
        }

        function fixFooterColumns() {
            //debugger
            console.log("fixFooterColumns");

            $(".footer-style").filter(function () {
                //debugger

                var myTd = $(this).children("td:not('.footermark')");
                //var myTd = $(this).hasClass("footermark")
                //console.log(myTd)
                return myTd.addClass('hidecol');

                //if (myTd.children().length = 0) {
                //    debugger
                //    return myTd.addClass('hidecol');
                //}
                //if (!myTd.has("div")) {
                //    return myTd.addClass('hidecol');
                //}
            });
        }

        function afterDdlCheck(hdFieldId, divId) {      

            console.log("afterDdlCheck");

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }        

        function removeHideReload(value) {

            //debugger
            //MainContent_lnkReloadGrid
            $('#MainContent_lnkReloadGrid').closest('.container').removeClass('hideProp')

            messages.alert(value, { type: "info" });
        }    

        function afterRadioCheck(hdFieldId, divId) {
            //debugger

            <%--if (divId.className == "collapse show") {
                $('#<%=hiddenId1.ClientID %>').val("1");
            } else {
                $('#<%=hiddenId1.ClientID %>').val("0");
            }--%>
            console.log("afterRadioCheck");

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        function yesnoCheck(id) {
            //debugger

            console.log("yesnoCheck");

            if (id !== null && id !== "" && id !== undefined) {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowCategory", "rowVndName", "rowWL", "rowMajor", "rowLast12"];

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
                var collapse1 = document.getElementById('collapseOne');
                var collapse2 = document.getElementById('collapseOne_2');
                var collapse3 = document.getElementById('collapseOne_3');
                var collapse4 = document.getElementById('collapseOne_4');

                var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
                var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            //var hd1

            <%--if (hd2 == 1) {
                $('#<%=hiddenId1.ClientID %>').val("0");
                $('#<%=hiddenId4.ClientID %>').val("0");
                $('#<%=hiddenId5.ClientID %>').val("0");
                hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
                hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
            }--%>
            if (hd4 == 1) {              
                $('#<%=hiddenId5.ClientID %>').val("1");                
                hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
            }
            <%--if (hd5 == 1) {
                $('#<%=hiddenId1.ClientID %>').val("0");
                $('#<%=hiddenId4.ClientID %>').val("1");
                $('#<%=hiddenId2.ClientID %>').val("0");
                hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
                hd4 = document.getElementById('<%=hiddenId1.ClientID%>').value;
                hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            }--%>
                else { hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value; }

                afterRadioCheck(hd1, collapse1)
                afterRadioCheck(hd2, collapse2)
                afterRadioCheck(hd4, collapse3)
                afterRadioCheck(hd5, collapse4)
            //isActivePanel(collapse1, 1);
            //isActivePanel(collapse2, 2);
            }            
        }

        function yesnoCheckCustom(id) {
            //debugger

            console.log("yesnoCheckCustom");

            if (id !== null && id !== "" && id !== undefined) {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowCategory", "rowVndName", "rowWL", "rowMajor", "rowLast12"];

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

        function isActivePanel(activePanel, valorActive) {
            //debugger

            console.log("isActivePanel");

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;

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
                    $('#<%=hiddenId3.ClientID %>').val("0");
                    hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId2.ClientID %>').val("0");
                    $('#<%=hiddenId3.ClientID %>').val("0");
                    hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }                            
            }

            if (valorActive == 4) {
                if ($('#<%=hiddenId4.ClientID %>').val() == "0") {
                    $('#<%=hiddenId4.ClientID %>').val("1");
                    $('#<%=hiddenId3.ClientID %>').val("0");
                    hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId4.ClientID %>').val("0");
                    $('#<%=hiddenId3.ClientID %>').val("0");
                    hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            if (valorActive == 5) {
                if ($('#<%=hiddenId5.ClientID %>').val() == "0") {
                    $('#<%=hiddenId5.ClientID %>').val("1");
                    $('#<%=hiddenId3.ClientID %>').val("0");
                    hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
                else {
                    $('#<%=hiddenId5.ClientID %>').val("0");
                    $('#<%=hiddenId3.ClientID %>').val("0");
                    hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
                    //afterDdlCheck(hd2, activePanel)
                }
            }

            JSFunction();
        }

        function JSFunction() {

            console.log("JSFunction");

            __doPostBack('<%= updatepnl1.ClientID  %>', '');
        }       

        //begin show or hide user assigment div
        $('body').on('click', '.clickme', function (e) {

            var hdFile = document.getElementById('<%=hdShowUserAssignment.ClientID%>').value
            if (hdFile == "0") {                  
                $('#<%=hdShowUserAssignment.ClientID %>').val("1")                 
            }
        }); 

        $('body').on('click', '#MainContent_btnBack3', function (e) {

            var hdFile = document.getElementById('<%=hdShowUserAssignment.ClientID%>').value
            if (hdFile == "1") {
                $('#<%=hdShowUserAssignment.ClientID %>').val("0");
            }
        });

        //end show or hide user assigment div

        $('body').on('click', '#MainContent_btnUpdate3', function (e) {

            var hdFile = document.getElementById('<%=hdShowUserAssignment.ClientID%>').value;
            if (hdFile == "1") {
                $('#<%=hdShowUserAssignment.ClientID %>').val("0");
            }
        });

        $('body').on('click', '.GridHeaderStyle a', function (e) {
            //debugger
            <%--var hdUser = document.getElementById('<%=hdShowUserAssignment.ClientID%>').value
            if (hdUser == "1") {
                $('#<%=hdShowUserAssignment.ClientID %>').val("0")
            }
            else {
                $('#<%=hdShowUserAssignment.ClientID %>').val("1")
            }--%>

            //var strs = "If you want to assign a person to work with that part, click YES and go over the part number in order to do that?"
            //messageConfirmForAssigment(strs, "info")
            //alert("pepi");            
        });        

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

        $('body').on('click', '.input-group .input-clearer', function () {
            $('#<%=hdCloseAction.ClientID %>').val("1");

            //JSFunction();
        });        

        $('body').on('click', 'click-in', function () {
            alert("pepe");
        });

        //$('body').on('click', '#MainContent_btnPdf', function (e) {

        //    messageFormSubmitted("This functionality is in testing process!", "info");

        //});

        function ShowAssignUser() {            

            //var hdCNS = document.getElementById('<%=hdShowUserAssignment.ClientID%>').value;
            //if (hdCNS != "0") {
              //  $('#MainContent_addPerPech').closest('.container').removeClass('hideProp');                
            //}

            //$('#<%=hdShowUserAssignment.ClientID %>').val("0")

            //alert("answer yes");
        }

        function ForcePostBack() {

            //__doPostBack('grvLostSales', 'OnRowCommand');
            //$('#<%=hdShowUserAssignment.ClientID %>').val("0");
            //alert("answer cancel");
        }

        //$('body').on('change', "#<%=hdShowUserAssignment.ClientID %>", function () {
        $('body').on('change', "#<%=ddlUser2.ClientID %>", function () {
            var value = document.getElementById("<%=ddlUser2.ClientID %>");
            var gettext = value.options[value.selectedIndex].text;
            var getindex = value.options[value.selectedIndex].value;
            $('#<%=hdUserAssigment.ClientID %>').val(getindex);

            var value1 = document.getElementById("<%=ddlUser2.ClientID %>").id;
            //$('#<%=hdUserAssigment.ClientID %>').val(value1);

        });

        $('body').on('change', "#<%=ddlCategory.ClientID %>", function () {
            var value = document.getElementById("<%=ddlCategory.ClientID %>");
            var gettext = value.options[value.selectedIndex].text;
            var getindex = value.options[value.selectedIndex].value;
            $('#<%=hdCategory.ClientID %>').val(getindex);

            var value1 = document.getElementById("<%=ddlCategory.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1); 
        });

        $('body').on('change', "#<%=ddlWishList.ClientID %>", function () {
            debugger

            var value = document.getElementById("<%=ddlWishList.ClientID %>");            
            var gettext = value.options[value.selectedIndex].text;
            var getindex = value.options[value.selectedIndex].value;  
            $('#<%=hdWishList.ClientID %>').val(getindex);  

            var value1 = document.getElementById("<%=ddlWishList.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1);

        });

        $('body').on('change', "#<%=ddlMajor.ClientID %>", function () {
            debugger

            var value = document.getElementById("<%=ddlMajor.ClientID %>");
            var gettext = value.options[value.selectedIndex].text;
            var getindex = value.options[value.selectedIndex].value;
            $('#<%=hdMajor.ClientID %>').val(getindex);  

            var value1 = document.getElementById("<%=ddlMajor.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1);

        });

        $('body').on('change', "#<%=ddlSaleLast12.ClientID %>", function () {
            debugger

            var value = document.getElementById("<%=ddlSaleLast12.ClientID %>");
            var gettext = value.options[value.selectedIndex].text;
            var getindex = value.options[value.selectedIndex].value;
            $('#<%=hdSaleLast12.ClientID %>').val(getindex);  

            var value1 = document.getElementById("<%=ddlSaleLast12.ClientID %>").id;
            $('#<%=selectedFilter.ClientID %>').val(value1);

        });


        <%--$("#<%=ddlVendAssign.ClientID %>").bind('change', function () {
            alert("3");
        });

        $("#<%=ddlVendAssign.ClientID %>").on("change", function () {
            alert("1");
        });

        $("#<%=ddlVendAssign.ClientID %>").change(function () {
            console.log("test");
            alert("2");
        });--%>

        

        //$(document).on('click', '#accordion_2 h5 a', function (e) {
        //    e.stopPropagation();
        //});


        //$('#accordion_2').on('click', 'h5 a', function () { 
        //    debugger
        //    $('#accordion_2 h5 a').bind('click', function () {
        //        //debugger
        //        var collapse1 = document.getElementById('collapseOne_2');
        //        event.stopPropagation()
        //        isActivePanel(collapse1, 2);
        //    });  

        //});


        $(function () {     
            debugger            
            //alert("function");  

            console.log("function");

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd5, collapse4);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd4, collapse3);

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);

            var collapse1 = document.getElementById('collapseOne');
            afterDdlCheck(hd1, collapse1);                       

            //$("#tqr10").on('ifToggled', function (event) {
            //    debugger
            //    __doPostBack('tqr10', '');
            //});            

            //$("#tqr10").click(function () {
            //    debugger
            //    $("[id*=btnSubmit]").click();
            //});

            var watermark = 'Search...';

            $('#MainContent_txtSearch').val(watermark).addClass('watermark');

            $('#MainContent_txtSearch').blur(function () {
                if ($(this).val().length == 0) {
                    $(this).val(watermark).addClass('watermark');
                }
            });

            $('#MainContent_txtSearch').focus(function () {
                if ($(this).val() == watermark) {
                    $(this).val('').removeClass('watermark');
                }
            });

            $('select').clearer();
            $('#MainContent_txtSearch').clearer();  

            <%--$("#<%=ddlVendAssign.ClientID %>").change(function () {
                alert("Handler for .change() called.");
            });--%>

             //MainContent_ddlVendAssign
            $('body').on('change', "#<%=ddlVendAssign.ClientID %>", function () {
                $('#<%=hiddenId3.ClientID %>').val("2");
                //alert("ok");
            });

            //btn submit click
            $('body').on('click', "#<%=submit.ClientID%>", function () {
                $('#<%=hdSubmit.ClientID%>').val("1");
            });

            fixFooterColumns();
        })       

        function pageLoad(event, args) {
            debugger     
            //alert("pageload"); 
            console.log("pageLoad");

            if (args.get_isPartialLoad()) {
                debugger
                console.log("Enter to partial Load");
                //alert("partial");

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

                var hdUpd = document.getElementById('<%=hdShowUserAssignment.ClientID%>').value
                if (hdUpd == "1") {
                    debugger
                    $('#MainContent_addPerPech').closest('.container').removeClass('hideProp')
                }
                else {
                    debugger
                    $('#MainContent_addPerPech').closest('.container').addClass('hideProp')
                }

                console.log("Previous to Fix Footer");
                fixFooterColumns();
                console.log("After to Fix Footer");

                var hdWelcome = document.getElementById('<%=hdWelcomeMess.ClientID%>').value
                $('#<%=lblUserLogged.ClientID %>').val(hdWelcome); 
            }  

            var hdWelcome = document.getElementById('<%=hdWelcomeMess.ClientID%>').value
            $('#<%=lblUserLogged.ClientID %>').val(hdWelcome); 

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;

            var hdName = document.getElementById('<%=hiddenName.ClientID%>').value;
            yesnoCheckCustom(hdName)

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd5, collapse4);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd4, collapse3);

            var collapse2 = document.getElementById('collapseOne_2');            
            afterDdlCheck(hd2, collapse2);

            var collapse1 = document.getElementById('collapseOne');            
            afterDdlCheck(hd1, collapse1);

            $('select').clearer();
            $('#MainContent_txtSearch').clearer();

            console.log("Previous to Pageload Fix Footer");
            fixFooterColumns();
            console.log("After to Pageload Fix Footer");
        }

    </script>

</asp:Content>
