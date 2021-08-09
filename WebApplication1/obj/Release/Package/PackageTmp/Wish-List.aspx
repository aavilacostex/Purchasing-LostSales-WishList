<%@ Page Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeBehind="Wish-List.aspx.vb" Inherits="WebApplication1.Wish_List" EnableEventValidation="false" ViewStateMode="Disabled" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">    

    <asp:UpdatePanel ID="updatepnl" runat="server">    
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />     
            <asp:AsyncPostBackTrigger ControlID="btnImportExcel" /> 
            <asp:AsyncPostBackTrigger ControlID="btnNewItem" /> 
            <asp:AsyncPostBackTrigger ControlID="ddlUser2" /> 
            <asp:AsyncPostBackTrigger ControlID="ddlStatus3" /> 
            <asp:PostBackTrigger ControlID="hdCustomerNoSelected" /> 
            <asp:PostBackTrigger ControlID="hdCustomerNoSelected1" />             
        </Triggers>
        <ContentTemplate>
            <script>
                //Sys.Application.add_load(jScript);
            </script>  
            
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
                                                    <span>WISH LIST</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="loadOptions" class="col-md-4">
                            <div class="row">
                                <div class="col-md-3">
                                    <asp:LinkButton ID="btnNewItem" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-plus fa-1x"" aria-hidden="true"> </i> NEW ITEM
                                                        </asp:LinkButton>
                                </div>
                                <div class="col-md-3">
                                    <asp:LinkButton ID="btnImportExcel" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-file-excel-o fa-1x" aria-hidden="true"> </i> IMPORT
                                                        </asp:LinkButton>
                                </div>
                                <div class="col-md-3">
                                    <asp:LinkButton ID="btnImportFromLs" class="boxed-btn-layout btn-rounded" runat="server" >
                                                            <i class="fa fa-list-alt fa-1x" aria-hidden="true"> </i> FROM LS
                                                        </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>     

            <div id="showActionsSection" class="container-fluid" runat="server">          
                <div id="rowFilters" class="row" runat="server">
                    <div class="col-md-2"></div>
                    <div class="col-md-3">
                        <asp:Panel ID="pnDefValues" CssClass="pnFilterStyles" GroupingText="ACTIONS" runat="server">
                            <div id="rowBtnFilters" class="row make-it-flex">
                                <div class="col-xs-12 col-sm-6 flex-item-1 padd-fixed" style="float: right;">
                                    <asp:LinkButton ID="btnWlTemplate" class="btn btn-primary btn-lg float-right btnFullSize" runat="server">
                                                            <i class="fa fa-1x fa-gear download" aria-hidden="true"> </i> WL TEMPLATE
                                    </asp:LinkButton>
                                </div>
                                <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed">
                                    <asp:Button ID="btnUpdate" class="btn btn-primary btn-lg btnFullSize" OnClick="btnUpdate_Click" runat="server" Text="UPDATE" />
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="accordion-wrapper hideProp">
                            <div id="accordion_2">
                                <div class="card">
                                    <div class="card-header" id="headingOne_2">
                                        <h5 class="mb-0">
                                            <a class="collapsed" data-toggle="collapse" data-target="#collapseOne_2" aria-expanded="false" aria-controls="collapseOne_2">
                                                <span class="">ACTIONS  <i class="fa fa-angle-down faicon"></i></span>
                                            </a>
                                        </h5>
                                    </div>

                                    <!--FORM TO GET DATA FOR FILTERING DATA-->
                                    <!--ACCORDION CONTENT-->
                                    <div id="collapseOne_2" class="collapse" aria-labelledby="headingOne_2" data-parent="#accordion_2" style="">
                                        <div class="card-body">
                                            <!-- cross site forgery's attack avoiding -->
                                            <div class="col-md-1">
                                                <input type="hidden" name="csrf" value="b3f24ac9359094f7b4629613138570a6-106b16695033660d3701da01a206aeba">
                                            </div>
                                            <!-- SUBMIT BUTTON AND CONVERT TO EXCEL THE ACTUAL PAGE -->
                                            
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
                        <asp:Panel ID="pnFilters" CssClass="pnFilterStyles" GroupingText="FILTERS" runat="server">
                            <div id="rowRadios1" class="form-group col-md-12">
                                <div class="row">
                                    <div class="form-group col-md-6 radio-toolbar">
                                        <label class="form-check">
                                            <p>Status</p>
                                            <asp:RadioButton ID="rdStatus" OnCheckedChanged="rdStatus_CheckedChanged" onclick="yesnoCheck('rowStatus');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>

                                    <div class="form-group col-md-6 radio-toolbar">
                                        <label class="form-check">
                                            <p>From</p>
                                            <asp:RadioButton ID="rdFrom" OnCheckedChanged="rdFrom_CheckedChanged" onclick="javascript:yesnoCheck('rowFrom');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>
                                </div>

                                <div id="rwAssigment" class="row">
                                    <div class="form-group col-md-6 radio-toolbar">
                                        <label class="form-check">
                                            <p>Assigment</p>
                                            <asp:RadioButton ID="rdAssigment" OnCheckedChanged="rdAssigment_CheckedChanged" onclick="yesnoCheck('rowAssigment');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                            <span class="checkmark"></span>
                                        </label>
                                    </div>

                                    <%--<div class="form-group col-md-6 radio-toolbar">
                                                        <label class="form-check">
                                                            <p>  </p>
                                                            <asp:RadioButton ID="rdWL" OnCheckedChanged="rdWL_CheckedChanged" onclick="yesnoCheck('rowWL');" class="form-check" GroupName="radiofilters" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                            <span class="checkmark"></span>
                                                        </label>
                                                    </div>--%>
                                </div>

                            </div>
                        </asp:Panel>
                        <div class="accordion-wrapper hideProp">
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
                                            
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Panel ID="pnFilterCriteria" CssClass="pnFilterStyles" GroupingText="FILTER CRITERIA" runat="server">
                            <!--search by Category-->
                            <div id="rowStatus" class="rowCategory" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Status</label>
                                    <br>
                                    <asp:DropDownList ID="ddlStatus" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Category." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <!--search by VendorName-->
                            <div id="rowFrom" class="rowVndName" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">From</label>
                                    <br>
                                    <asp:DropDownList ID="ddlFrom" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlFrom_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Vendor Name." runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <!--search by Major-->
                            <div id="rowAssigment" class="rowMajor" style="display: none;">
                                <div class="col-md-2"></div>
                                <div class="col-md-10">
                                    <label for="sel-vndassigned">Assigment</label>
                                    <br>
                                    <asp:DropDownList ID="ddlAssign" name="sel-vndassigned" class="form-control" OnSelectedIndexChanged="ddlAssign_SelectedIndexChanged" AutoPostBack="true" ViewStateMode="Enabled" title="Search by Major Code." runat="server"></asp:DropDownList>
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
                                    <asp:FileUpload ID="fuOPenEx" Name="flUpl1" CssClass="form-control" runat="server" />
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

            <div id="addNewPartManual" class="container hideProp" runat="server">
                <div class="row">
                    <div class="col-md-3"></div>
                    <div class="col-md-6">
                        <div id="pnAddPartManual" class="shadow-to-box">
                            <div class="row" style="padding: 30px 0;">
                                <div class="col-md-3"></div>
                                <div class="col-md-6"><span id="spnAddFile">new item</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="form-row">
                                <div class="col-md-3"></div>
                                <div class="form-group col-md-6">
                                    <asp:Label ID="lblPartNumber" Text="Part Number" runat="server"></asp:Label>
                                    <asp:TextBox ID="txtPartNumber" CssClass="form-control" runat="server" />
                                </div>
                                <div class="col-md-3"></div>
                            </div>                    
                            <div class="row" style="padding: 20px 0;">
                                <div class="col-md-3"></div>
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-6" style="float: right; text-align: right !important;">
                                            <asp:Button ID="btnSubmitItem" Text="Submit" class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                        <div class="col-md-6" style="float: left;">
                                            <asp:Button ID="btnBackItemm2" Text="   Back   " class="btn btn-primary btn-lg btnFullSize" runat="server" />
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

            <div id="addNewPartManual3" class="container hideProp" runat="server">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-8">
                        <div id="pnAddPartManual3" class="shadow-to-box">
                            <div class="row" style="padding: 30px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnAddFile3">new item</span></div>
                                <div class="col-md-3"></div>
                            </div>                            
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lbCode" CssClass="label-style" Text="Code" runat="server"></asp:Label>
                                        <asp:TextBox ID="txCode" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lbPartNo" CssClass="label-style" Text="Part Number" runat="server"></asp:Label>
                                        <asp:TextBox ID="txPartNo" CssClass="form-control" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lbDate" CssClass="label-style" Text="Date" runat="server"></asp:Label>
                                        <asp:TextBox ID="txDate" CssClass="form-control" runat="server" />
                                    </div> 
                                </div>
                            </div>                            
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lbUser" CssClass="label-style" Text="User" runat="server"></asp:Label>
                                        <asp:TextBox ID="txUser" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lbDesc" CssClass="label-style" Text="Description" runat="server"></asp:Label>
                                        <asp:TextBox ID="txDesc" CssClass="form-control" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lbPrice" CssClass="label-style" Text="Price" runat="server"></asp:Label>
                                        <asp:TextBox ID="txPrice" CssClass="form-control" runat="server" />
                                    </div> 
                                </div>
                            </div>                            
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lbModel" CssClass="label-style" Text="Model" runat="server"></asp:Label>
                                        <asp:TextBox ID="txModel" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <asp:Label ID="lbMajor" CssClass="label-style" Text="Major" runat="server"></asp:Label>
                                        <asp:TextBox ID="txMajor" CssClass="form-control" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <asp:Label ID="lbMinor" CssClass="label-style" Text="Minor" runat="server"></asp:Label>
                                        <asp:DropDownList ID="dlMinor" CssClass="form-control" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lbType" CssClass="label-style" Text="Type" runat="server"></asp:Label>
                                        <asp:DropDownList ID="dlType" CssClass="form-control" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                    </div> 
                                </div>
                            </div>  
                            <div class="form-row">                        
                                <div class="col-md-12">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lbComments" CssClass="label-style" Text="Comments" runat="server"></asp:Label>
                                        <asp:TextBox ID="txComments" CssClass="form-control fullTextBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                                    </div>                                    
                                </div>                                
                            </div> 
                            <div class="row" style="padding: 20px 0;">
                                <div class="col-md-5"></div>
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-6" style="float: right; text-align: right !important;">
                                            <asp:Button ID="btCreate" Text="Create" class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                        <div class="col-md-6" style="float: left;">
                                            <asp:Button ID="btBack" Text="   Back   " class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-1"></div>
                            </div> 
                        </div>
                    </div>
                    <div class="col-md-2"></div>
                </div>
            </div>

            <div id="addNewPartManual2" class="container hideProp" runat="server">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-8">
                        <div id="pnAddPartManual2" class="shadow-to-box">
                            <div class="row" style="padding: 30px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnAddFile2">new item</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="row">
                                <div class="col-md-5">
                                    <asp:Image ID="imgNewItem" runat="server" ImageUrl="~/Images/avatar-ctp.PNG" />
                                </div>
                                <div class="col-md-6">
                                    <div class="form-row">
                                        <div class="form-group col-md-2">
                                            <asp:Label ID="lblCode" CssClass="label-style" Text="Code" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtCode" CssClass="form-control" runat="server" />
                                        </div>
                                        <div class="form-group col-md-4">
                                            <asp:Label ID="lblUser" CssClass="label-style" Text="User" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtUser" CssClass="form-control" runat="server" />
                                        </div>
                                        <div class="form-group col-md-6">
                                            <asp:Label ID="lblDate" CssClass="label-style" Text="Date" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtDate" CssClass="form-control" runat="server" />
                                        </div>
                                    </div>
                                    <div class="form-row">
                                        <div class="form-group col-md-5">
                                            <asp:Label ID="lblPartNo" CssClass="label-style" Text="Part Number" runat="server"></asp:Label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtPartNo" CssClass="form-control autosuggestpart" runat="server" />
                                                <div class="input-group-append">
                                                    <asp:LinkButton ID="lnkSearchPartNo" class="" runat="server" ><i class="fa fa-search center-vert font-awesome-custom" aria-hidden="true"></i> </asp:LinkButton>
                                                </div>
                                            </div>
                                            
                                        </div>                                        
                                        <div class="form-group col-md-7">
                                            <asp:Label ID="lblDesc" CssClass="label-style" Text="Description" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtDesc" CssClass="form-control fullTextBox" runat="server" />
                                        </div>
                                    </div>
                                    <div class="form-row">
                                        <div class="form-group col-md-5">
                                            <asp:Label ID="lblVendor" CssClass="label-style" Text="Vendor" runat="server"></asp:Label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtvendor" CssClass="form-control" runat="server" />
                                                <div class="input-group-append">
                                                    <asp:LinkButton ID="lnkSearchVendorNo" class="" runat="server"><i class="fa fa-search center-vert font-awesome-custom" aria-hidden="true"></i> </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>                                        
                                        <div class="form-group col-md-7">
                                            <asp:Label ID="lblVndDesc" CssClass="label-style" Text="Vendor Description" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtVndDesc" CssClass="form-control fullTextBox autosuggestvendor" runat="server" />

                                            <%--<Atk:AutoCompleteExtender runat="server" ID="autoComplete1" TargetControlID="txtVndDesc"
                                                        ServiceMethod="GetAutocompleteSelectedVendorName" UseContextKey="True" MinimumPrefixLength="2"
                                                        CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20" DelimiterCharacters=""
                                                        OnClientItemSelected="OnContactSelected" ShowOnlyCurrentWordInCompletionListItem="true">
                                            </Atk:AutoCompleteExtender>
                                            <asp:HiddenField ID="hdnValue" runat="server" OnValueChanged="hdnValue_ValueChanged" />--%>
                                        </div>
                                    </div>
                                    <div class="form-row">
                                        <div class="form-group col-md-12">
                                            <asp:Label ID="lblType" CssClass="label-style" Text="Type" runat="server"></asp:Label>
                                            <asp:DropDownList ID="ddlType" class="form-control fullTextBox" EnableViewState="true" ViewStateMode="Enabled" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>                            
                                </div>
                                <div class="col-md-1"></div>
                            </div>   
                            <div class="row">                        
                                <div class="col-md-11">
                                    <div class="form-row">
                                        <div class="form-group col-md-12" style="padding-left: 25px;">
                                            <asp:Label ID="lblComments" CssClass="label-style" Text="Comments" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtComments" CssClass="form-control fullTextBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-1"></div>
                            </div>                                        
                            <div class="row" style="padding: 20px 0;">
                                <div class="col-md-5"></div>
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-6" style="float: right; text-align: right !important;">
                                            <asp:Button ID="btnSubmitItem2" Text="Add" class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                        <div class="col-md-6" style="float: left;">
                                            <asp:Button ID="btnBackItem2" Text="   Back   " class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-1"></div>
                            </div> 
                        </div>
                    </div>
                    <div class="col-md-2"></div>
                </div>  
            </div>

            <div id="updatePart" class="container-fluid hideProp" runat="server">
                <div class="row">
                    <div class="col-md-6">
                        <div id="pnUpdatePart" class="shadow-to-box">
                            <div class="row" style="padding: 30px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnUpdatePart">update item</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="form-row" style="padding: 0 25px;">
                                <div class="form-group col-md-4">
                                    <asp:Label ID="lblPartNumber2" CssClass="label-style" Text="part number" runat="server"></asp:Label>
                                    <asp:TextBox ID="txtPartNumber2" CssClass="form-control" runat="server" />
                                </div>
                                <div class="form-group col-md-4">
                                    <asp:Label ID="lblAssignedTo" CssClass="label-style" Text="assigned to" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlAssignedTo" CssClass="form-control" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                </div>
                                <div class="form-group col-md-4">
                                    <asp:Label ID="lblStatus2" CssClass="label-style" Text="status" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlStatus2" CssClass="form-control" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                </div>
                            </div>
                            <div class="form-row" style="padding: 0 25px;">
                                <div class="form-group col-md-12">
                                    <asp:Label ID="lblComments2" CssClass="label-style" Text="Comments" runat="server"></asp:Label>
                                    <asp:TextBox ID="txtComments2" CssClass="form-control fullTextBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row" style="padding: 0 25px;">
                                <div class="form-group col-md-3" style="float: right; text-align: right !important;">                                    
                                    <asp:LinkButton ID="btnUpdate2" class="btn btn-primary btn-lg btnFullSize" runat="server" ><i class="fa fa-edit fa-1x" aria-hidden="true"> </i> UPDATE</asp:LinkButton>
                                </div>
                                <div class="form-group col-md-3" style="float: right; text-align: right !important;">
                                    <asp:LinkButton ID="btnNewPD" class="btn btn-primary btn-lg btnFullSize" runat="server" ><i class="fa fa-newspaper-o fa-1x" aria-hidden="true"> </i> NEW PROD DEV</asp:LinkButton>
                                </div>
                                <div class="form-group col-md-3" style="float: right; text-align: right !important;">
                                    <asp:LinkButton ID="btnAddTo" class="btn btn-primary btn-lg btnFullSize" runat="server" ><i class="fa fa-plus-circle fa-1x" aria-hidden="true"> </i> ADD</asp:LinkButton>                                    
                                </div>
                                <div class="form-group col-md-3" style="float: left;">
                                    <asp:Button ID="btnBack2" Text="   Back   " class="btn btn-primary btn-lg btnFullSize" runat="server" />
                                </div>
                            </div>
                        </div>                
                    </div>
                    <div class="col-md-6">
                        <div id="pnDisplayImage" class="shadow-to-box hideProp">    
                            <div class="row" style="padding: 20px 0;">
                                <asp:Image ID="imgAllMap" ImageUrl="~/Images/mapall.PNG" style="padding: 0 50px;" runat="server" />
                            </div>                    
                        </div>
                    </div>
                </div>
            </div>

            <div id="updatePart2" class="container-fluid hideProp" runat="server">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-4">
                        <div id="pnUpdatePart2" class="shadow-to-box">
                            <div class="row" style="padding: 55px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnUpdatePart2">update user in charge & status</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="form-row" style="padding: 25px 25px;">                        
                                <div class="form-group col-md-6">
                                    <asp:Label ID="lblUser2" CssClass="label-style" Text="assigned to" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlUser2" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlUser2_SelectedIndexChanged" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                </div>
                                <div class="form-group col-md-6">
                                    <asp:Label ID="lblStatus3" CssClass="label-style" Text="status" runat="server"></asp:Label>
                                    <asp:DropDownList ID="ddlStatus3" OnSelectedIndexChanged="ddlStatus3_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control" EnableViewState="true" ViewStateMode="Enabled" runat="server" />
                                </div>
                            </div>                    
                            <div class="form-row">
                                <div class="form-group col-md-6" style="float: right; text-align: right !important;">
                                    <asp:Button ID="btnUpdate3" Text="update" class="btn btn-primary btn-lg btnMidSize" OnClick="btnUpdate3_Click" runat="server" />
                                </div>
                                <div class="form-group col-md-6" style="float: left;">
                                    <asp:Button ID="btnBack3" Text="   Back   " class="btn btn-primary btn-lg btnMidSize" runat="server" />
                                </div>
                            </div>
                        </div>                
                    </div>
                    <div class="col-md-6">
                        <div id="pnDisplayImage2" class="shadow-to-box hideProp">    
                            <div class="row" style="padding: 20px 0;">
                                <asp:Image ID="Image1" ImageUrl="~/Images/mapall.PNG" style="padding: 0 50px;" runat="server" />
                            </div>                    
                        </div>
                    </div>
                </div>
            </div>    

            <div id="addProdDev" class="container-fluid hideProp" runat="server">
                <div class="row">
                    <div class="col-md-6">
                        <div id="pnProdDev1" class="shadow-to-box">
                            <div class="row" style="padding: 5px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnProdDev1">new project</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <!-- Fisrt data -->
                            <div class="row">                                
                                <div class="col-md-4">
                                    <asp:Image ID="Image2" ImageUrl="~/Images/avatar-ctp.PNG" Style="padding: 0 25px; width: 90%; float: right;" runat="server" />
                                </div>
                                <div id="topFields" class="col-md-7" runat="server">
                                    <div class="form-row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label ID="lblPartNoPD" CssClass="label-style" Text="part number" runat="server"></asp:Label>
                                                <asp:TextBox ID="txtPartNoPD" CssClass="form-control" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label ID="lblCTPNoPD" CssClass="label-style" Text="ctp #" runat="server"></asp:Label>
                                                <asp:TextBox ID="txtCTPNoPD" CssClass="form-control" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <asp:Label ID="lblDescriptionPD1" CssClass="label-style" Text="description" runat="server"></asp:Label>
                                                <asp:TextBox ID="txtDescriptionPD1" TextMode="MultiLine" CssClass="form-control fullTextBox" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-1"></div>                                
                            </div>
                            <!-- Project Header -->
                            <div class="row" style="padding: 10px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnProdDev3">project data</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <!-- Project Data Handler -->
                            <div id="border-inner-panel">
                                <div class="col-md-1"></div>
                                <div id="inner-panel" class="col-md-10">
                                    <!-- Select the operation over the project -->
                                    <div class="form-row ctx-fr">
                                        <%--selection option, New Project or Existing Project--%>
                                        <div class="form-group col-md-6 radio-toolbar">
                                            <label class="form-check">
                                                <p>Create New Project Development</p>
                                                <asp:RadioButton ID="rdNewProject" CssClass="custom-radio1" GroupName="radio" OnCheckedChanged="rdNewProject_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                <span class="checkmark"></span>
                                            </label>
                                        </div>
                                        <div class="form-group col-md-6 radio-toolbar">
                                            <label class="form-check">
                                                <p>Add to existing Project Development</p>
                                                <asp:RadioButton ID="rdExistingProject" CssClass="custom-radio2" GroupName="radio" OnCheckedChanged="rdExistingProject_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                <span class="checkmark"></span>
                                            </label>
                                        </div>
                                    </div>
                                    <!-- Add to New Project Form -->
                                    <div id="dvNewProject" class="container-fluid hideProp" runat="server">
                                        <div class="form-row ctx-fr">
                                            <div class="col-md-4">
                                                <div class="form-group">
                                                    <asp:Label ID="lblProjectNamePD" CssClass="label-style" Text="project name" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtProjectNamePD" CssClass="form-control fullTextBox" runat="server" />
                                                </div>
                                            </div>
                                            <div class="col-md-8"></div>
                                        </div>
                                        <div class="form-row ctx-fr">
                                            <div class="col-md-4">
                                                <div class="form-group">
                                                    <h6>If want to update you can find the vendor by <strong>number</strong> or <strong>name</strong> in the next two boxes.</h6>
                                                </div>
                                            </div>
                                            <div class="col-md-8">
                                                <div class="form-row">
                                                    <div class="col-md-6">
                                                        <div class="form-group">

                                                            <div class="input-group">
                                                                <asp:Label ID="lblNewVendorNo" Text="new vendor no." placeholder="Vendor No." CssClass="label-style" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                                <asp:TextBox ID="txtNewVendorNo" CssClass="form-control fullTextBox" runat="server"></asp:TextBox>
                                                                <div class="input-group-append">
                                                                    <asp:LinkButton ID="lnkNewVendor" OnClick="lnkNewVendor_Click" runat="server">
                                                                        <span id="Span100" aria-hidden="true" runat="server">
                                                                            <i class="fa fa-search"></i>
                                                                        </span>
                                                                    </asp:LinkButton>
                                                                    <%--<button class="btn btn-outline-secondary" type="button" id="button-addon2">Button</button>--%>
                                                                </div>
                                                            </div>

                                                            <%--<asp:Label ID="lblNewVendorNo" text="new vendor no." CssClass="label-style" runat="server"></asp:Label>
                                                        <asp:TextBox ID="txtNewVendorNo" CssClass="form-control fullTextBox autosuggestvendor1" runat="server"></asp:TextBox>--%>
                                                        </div>
                                                    </div>
                                                    <%--<div class="col-md- padding0">
                                                    <asp:LinkButton ID="lnkNewVendor" runat="server">
                                                        <span id="Span100" aria-hidden="true" runat="server"> 
                                                        <i class="fa fa-search"></i>
                                                    </span>
                                                    </asp:LinkButton>
                                                </div>--%>
                                                    <div class="col-md-6">
                                                        <div class="form-group">
                                                            <asp:Label ID="lblNewVendorPD" CssClass="label-style" Text="new vendor name" runat="server"></asp:Label>
                                                            <asp:TextBox ID="txtNewVendorPD" name="txt-NewVendorPD" CssClass="form-control fullTextBox autosuggestvendor1" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                        <div class="form-row ctx-fr">
                                            <div class="col-md-12">
                                                <div class="form-group">
                                                    <asp:Label ID="lblProjectDevDescription" CssClass="label-style" Text="project comments" runat="server"></asp:Label>
                                                    <asp:TextBox ID="txtProjectDevDescription" TextMode="MultiLine" CssClass="form-control fullTextBox" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- Add to Existing Project Form -->
                                    <div id="dvExistingProject" class="container-fluid hideProp" runat="server">
                                        <!-- Search Project Data Fields -->
                                        <div class="form-row ctx-fr">
                                            <div class="col-md-3">
                                                <asp:Label ID="lblSearchBy" Text="Search Criteria" CssClass="label-style" runat="server"></asp:Label>
                                            </div>
                                            <div class="form-group col-md-3 radio-toolbar">
                                                <label class="form-check">
                                                    <p>Project Number</p>
                                                    <asp:RadioButton ID="rdSearchByNo" GroupName="radio" OnCheckedChanged="rdSearchByNo_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                    <span class="checkmark"></span>
                                                </label>                                                
                                            </div>
                                            <div class="form-group col-md-3 radio-toolbar" style="display: none !important;">
                                                <label class="form-check">
                                                    <p>Project Name</p>
                                                    <asp:RadioButton ID="rdSearchByName" GroupName="radio" OnCheckedChanged="rdSearchByName_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                    <span class="checkmark"></span>
                                                </label>                                                
                                            </div>
                                            <div class="col-md-5">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <asp:Label ID="lblSearchValue" Text="" placeholder="" CssClass="label-style" aria-label="Recipient's username" aria-describedby="button-addon2" runat="server"></asp:Label>
                                                        <asp:TextBox ID="txtSearchValue" CssClass="form-control fullTextBox" runat="server"></asp:TextBox>
                                                        <div class="input-group-append">
                                                            <asp:LinkButton ID="lnkSearchValue" OnClick="lnkSearchValue_Click" runat="server">
                                                                <span id="Span33" aria-hidden="true" runat="server">
                                                                    <i class="fa fa-search"></i>
                                                                </span>
                                                            </asp:LinkButton>                                                           
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Select Vendors if have more than one or if is different to the part vendor -->
                                        <div id="dvVndValidation" class="custom-div hideProp" runat="server">
                                            <div class="form-row ctx-fr">
                                                <div class="col-md-6" style="word-break: break-all;">
                                                    <asp:Label ID="lblProjNumber" Text="Project Number" CssClass="label-style" runat="server"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblProjectNumber" Text="" CssClass="label-style" runat="server"></asp:Label>
                                                    <br />
                                                    <span id="spnMessage" style="font-size: 11px;" runat="server">The selected project has the vendor shown in the box. If you want to keep this provider, click on it; otherwise, the supplier of the part will be used.</span>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblProjVendors" Text="Vendors In Project" CssClass="label-style" runat="server"></asp:Label>
                                                    <br />
                                                    <asp:ListBox ID="liProjVendors" OnSelectedIndexChanged="liProjVendors_SelectedIndexChanged" ViewStateMode="Enabled" EnableViewState="true" AutoPostBack="true" CssClass="form-control"  runat="server"></asp:ListBox>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Existing Project Data -->
                                        <div id="dvProjectInfo">
                                            <div id="dvProjectHeader" class="form-row ctx-fr">
                                                <div class="col-md-2">
                                                    <asp:Label ID="Label1" Text="Project Name" CssClass="label-style" runat="server"></asp:Label></div>
                                                <div class="col-md-2">
                                                    <asp:Label ID="Label3" Text="Project Date" CssClass="label-style" runat="server"></asp:Label></div>
                                                <div class="col-md-2">
                                                    <asp:Label ID="Label4" Text="Project Status" CssClass="label-style" runat="server"></asp:Label></div>
                                                <div class="col-md-2">
                                                    <asp:Label ID="Label5" Text="Project User" CssClass="label-style" runat="server"></asp:Label></div>
                                                <div class="col-md-4">
                                                    <asp:Label ID="Label6" Text="Project Info" CssClass="label-style" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <div id="dvProjectData" class="form-row ctx-fr">
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblProjName" Text="" CssClass="label-style" ViewStateMode="Enabled" EnableViewState="true" runat="server"></asp:Label></div>
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblProjCreationDate" Text="" CssClass="label-style" ViewStateMode="Enabled" EnableViewState="true" runat="server"></asp:Label></div>
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblProjStatus" Text="" CssClass="label-style" ViewStateMode="Enabled" EnableViewState="true" runat="server"></asp:Label></div>
                                                <div class="col-md-2">
                                                    <asp:Label ID="lblProjUser" Text="" CssClass="label-style" ViewStateMode="Enabled" EnableViewState="true" runat="server"></asp:Label></div>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblProjInfo" Text="" CssClass="label-style" ViewStateMode="Enabled" EnableViewState="true" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>   
                                    
                                </div>
                                <div class="col-md-1"></div>                                
                            </div>                            
                            <div class="form-row">
                                <div class="form-group col-md-3"></div>
                                <div class="form-group col-md-6" style="padding-top: 20px;">
                                    <div class="row">
                                        <div class="col-md-6" style="float: right; text-align: right !important;">
                                            <asp:Button ID="btnCreateProjectPD" Text="send to development" class="btn btn-primary btn-lg btnFullSize" OnClick="btnCreateProjectPD_Click" runat="server" />
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
                    <div class="col-md-6">
                        <div id="pnProdDev2" class="shadow-to-box">
                            <div class="row" style="padding: 55px 0;">
                                <div class="col-md-1"></div>
                                <div class="col-md-8"><span id="spnProdDev2">part info</span></div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lblWhlCode" CssClass="label-style" Text="wish list code" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtWhlCode" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lblCreationDate" CssClass="label-style" Text="creation date" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCreationDate" CssClass="form-control" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lblUserCreated" CssClass="label-style" Text="user created" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtUserCreated" CssClass="form-control" runat="server" />
                                    </div> 
                                </div>
                            </div>   
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lblCurrentVendor" CssClass="label-style" Text="current vendor" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCurrentVendor" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-8">
                                    <div class="form-group">
                                        <asp:Label ID="lblDescriptionPD" CssClass="label-style" Text="description" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtDescriptionPD" CssClass="form-control fullTextBox" runat="server" />
                                    </div>                                        
                                </div>                                
                            </div> 
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lblAssignedToPD" CssClass="label-style" Text="assigned to" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtAssignedToPD" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lblReasonTypePD" CssClass="label-style" Text="reason type" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtReasonTypePD" CssClass="form-control" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lblMinorCodePD" CssClass="label-style" Text="minor code" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtMinorCodePD" CssClass="form-control" runat="server" />
                                    </div> 
                                </div>
                            </div> 
                            <div class="form-row">
                                <div class="col-md-4">                                    
                                    <div class="form-group">
                                        <asp:Label ID="lblQtySoldPD" CssClass="label-style" Text="qty sold" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtQtySoldPD" CssClass="form-control" runat="server" />
                                    </div>                                                                           
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lblTimesQuoteLY" CssClass="label-style" Text="times quote ly" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtTimesQuoteLY" CssClass="form-control" runat="server" />
                                    </div>                                        
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <asp:Label ID="lblOEMPricePD" CssClass="label-style" Text="oem price" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtOEMPricePD" CssClass="form-control" runat="server" />
                                    </div> 
                                </div>
                            </div> 
                            <div class="form-row">
                                <div class="col-md-12"> 
                                    <div class="form-group">
                                        <asp:Label ID="lblCommentsPD" CssClass="label-style" Text="comments" runat="server"></asp:Label>
                                        <asp:TextBox ID="txtCommentsPD" TextMode="MultiLine" CssClass="form-control fullTextBox" runat="server" />
                                    </div> 
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>              
    
            <div id="integratedRow" class="container-fluid">
                <div class="row">
                    <div class="col-md-3">
                        <div id="rowPageSize" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" ><asp:Label ID="lblText1" Text="Show " runat="server"></asp:Label></div>
                            <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed"><asp:DropDownList name="ddlPageSize" ID="ddlPageSize" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" EnableViewState="true" ViewStateMode="Enabled" class="form-control" runat="server"></asp:DropDownList></div>
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" ><asp:Label ID="lblText2" Text=" entries." runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div id="rowBtnOpt" class="row">
                            <div class="col-xs-12 col-sm-3"></div>
                            <div class="col-xs-12 col-sm-2 flex-item-1 padd-fixed">
                                <asp:Button ID="btnExcel" class="btn btn-primary btn-lg float-right btnFullSize" OnClick="btnExcel_Click" runat="server" Text="Excel" />
                            </div>
                            <div class="col-xs-12 col-sm-2 flex-item-2 padd-fixed">
                                <asp:Button ID="btnPdf" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Pdf" />
                            </div>
                            <div class="col-xs-12 col-sm-2 flex-item-3 padd-fixed hideProp">
                                <asp:Button ID="btnCopy" class="btn btn-primary btn-lg btnFullSize" runat="server" Text="Copy" />
                            </div>
                            <div class="col-xs-12 col-sm-3"></div>
                        </div>                
                    </div>
                    <div class="col-md-3">
                        <div id="rowBtnSearch" class="row">
                            <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" style="float: right;"><asp:Label ID="lblSearch" Text="Search: " runat="server" Height="27px"></asp:Label></div>
                            <div class="col-xs-12 col-sm-5 flex-item-2 padd-fixed"><asp:TextBox name="txtSearch" ID="txtSearch" class="form-control" runat="server"></asp:TextBox></div>
                            <div class="col-xs-12 col-sm-3 flex-item-2 padd-fixed"><asp:Button name="btnSearch" ID="btnSearch" class="btn btn-primary btn-sm btnFullSize1" Text="Search" runat="server"></asp:Button></div>
                            <%--<div class="spinner-grow text-warning"></div>--%>
                        </div>  
                        <div id="notVisibleKeyPress" style="display:none" runat="server">
                            <asp:Button ID="Button1" runat="server" Text="Button" />
                        </div>
                    </div>
                </div>        
            </div>

            <div class="row">
                <asp:HiddenField ID="hiddenId1" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId2" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId4" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId5" Value="0" runat="server" />

                <asp:HiddenField ID="hdFileImportFlag" Value="0" runat="server" />
                <asp:HiddenField ID="hdUpdateFullRefFlag" Value="0" runat="server" />
                <asp:HiddenField ID="hdUpdateMedRefFlag" Value="0" runat="server" />
                <asp:HiddenField ID="hdNewRef1Flag" Value="0" runat="server" />
                <asp:HiddenField ID="hdNewRef2Flag" Value="0" runat="server" />
                <asp:HiddenField ID="hdNewRef3Flag" Value="0" runat="server" />               
                <asp:HiddenField ID="hdProdDevFlag" Value="0" runat="server" />

                <asp:HiddenField ID="hdPartNoSelected" OnValueChanged="hdPartNoSelected_ValueChanged" Value="0" runat="server" /> 
                <asp:HiddenField ID="hdCustomerNoSelected" OnValueChanged="hdCustomerNoSelected_ValueChanged" Value="0" runat="server" />
                <asp:HiddenField ID="hdCustomerNoSelected1" OnValueChanged="hdCustomerNoSelected1_ValueChanged" Value="0" runat="server" />
                <asp:HiddenField ID="hdHideMessage" Value="0" runat="server" />
                <asp:HiddenField ID="hdHideMessageVendor" Value="0" runat="server" />

                <asp:HiddenField ID="hdDdlType" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDlType" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDlMinor" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDdlStatus2" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDdlAssignedTo" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDdlStatus3" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDdlUser2" Value="-1" runat="server" />
                <asp:HiddenField ID="hdDdlPageSize" Value="-1" runat="server" />  
                
                <asp:HiddenField ID="hdLinkExpand" value="0" runat="server" />
                <asp:HiddenField ID="hdTriggeredControl" value="" runat="server" />
                <asp:HiddenField ID="hdLaunchControl" value="" runat="server" />
                <asp:HiddenField ID="hdSelectedClass" value="" runat="server" />

                <asp:HiddenField ID="refreshTxtValue" value="" runat="server" />

                <asp:HiddenField ID="hiddenName" Value="" runat="server" />

                <asp:HiddenField ID="hdWhlCode1" Value="0" runat="server" />

                <%--part info form--%>
                <asp:HiddenField ID="hdCommentsPD" value="" runat="server" />
                <asp:HiddenField ID="hdOEMPricePD" value="" runat="server" />
                <asp:HiddenField ID="hdTimesQuoteLY" value="" runat="server" />
                <asp:HiddenField ID="hdQtySoldPD" value="" runat="server" />
                <asp:HiddenField ID="hdMinorCodePD" value="" runat="server" />
                <asp:HiddenField ID="hdReasonTypePD" value="" runat="server" />
                <asp:HiddenField ID="hdAssignedToPD" value="" runat="server" />
                <asp:HiddenField ID="hdCurrentVendor" value="" runat="server" />
                <asp:HiddenField ID="hdDescriptionPD" value="" runat="server" />
                <asp:HiddenField ID="hdUserCreated" value="" runat="server" />
                <asp:HiddenField ID="hdCreationDate" value="" runat="server" />
                <asp:HiddenField ID="hdWhlCode" value="" runat="server" />

                <asp:HiddenField ID="hdUserVisibility" value="" runat="server" />
                <asp:HiddenField ID="hdSessionDefaultTimeOut" value="" runat="server" />
                <asp:HiddenField ID="hdSessionToCheckTimeOut" value="" runat="server" />
                <asp:HiddenField ID="hdBeginNotification" value="" runat="server" />

                <asp:HiddenField ID="hdNewProj" value="0" runat="server" />
                <asp:HiddenField ID="hdExistProj" value="0" runat="server" />

                <asp:HiddenField ID="hdDifVnd" value="" runat="server" />

                <asp:HiddenField id="hdWelcomeMess" Value="" runat="server" />
            </div>

            <div class="row" style="display: none !important;">
                <%--<asp:DropDownList ID="VndStorage" runat="server"></asp:DropDownList>--%>

                <asp:DropDownList ID="ddlStatusFoot" OnSelectedIndexChanged ="ddlStatusFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlFromFoot" OnSelectedIndexChanged ="ddlFromFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlAssignFoot" OnSelectedIndexChanged ="ddlAssignFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>

                <%--<asp:DropDownList ID="ddlStatus" OnSelectedIndexChanged ="ddlStatus_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlFrom" OnSelectedIndexChanged ="ddlFrom_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlAssign" OnSelectedIndexChanged ="ddlAssign_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>--%>

                <asp:HiddenField ID="selCheckbox" Value="0"  runat="server" />
                <table id="ndtt" runat="server"></table>
                <asp:Label ID="lblGrvGroup" Text="test" runat="server"></asp:Label>
            </div>

            <div id="gridSection" class="container-fluid" runat="server">
                <div class="panel panel-default">
                    <div class="panel-body">                
                        <div class="form-horizontal"> 
                            <div id="rowGridView">
                                <asp:GridView ID="grvWishList" runat="server" AutoGenerateColumns="false"
                                    PageSize="10" CssClass="table table-striped table-bordered" AllowPaging="True" AllowSorting="true"
                                    GridLines="None" OnRowCommand="grvWishList_RowCommand" OnPageIndexChanging="grvWishList_PageIndexChanging"
                                    OnRowDataBound="grvWishList_RowDataBound" OnSorting="grvWishList_Sorting" ShowHeader="true" ShowFooter="true" 
                                    OnRowUpdating="grvWishList_RowUpdating" DataKeyNames="IMPTN" >
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkAll" Text="" Visible="true" Checked="False" runat="server" OnCheckedChanged="chkAll_CheckedChanged" AutoPostBack="True"
                                                    ToolTip="Select All"></asp:CheckBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <span style="padding: 10px;">
                                                    <asp:CheckBox ID="chkSingleAdd" runat="server" Checked="False" ToolTip="Select to Wish List" />
                                                    <asp:HiddenField ID="hdnId" runat="server" Value='<%#Eval("WHLCODE") %>' /> 
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                
                                        <asp:TemplateField HeaderText="FROM" ItemStyle-Width="6%" >
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <EditItemTemplate>  
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("WHLFROM") %>' />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lbSourceFrom"
                                                    runat="server"
                                                    TabIndex="1" CommandName="ToolTip"
                                                    ToolTip="Coming From">
                                                    <span id="Span1" aria-hidden="true" runat="server">
                                                        <%--<i id="iconContainer" class="fa fa-plus" runat="server"> --%>
                                                            <asp:Label ID="textlbl" Text='<%# Bind("WHLFROM") %>' runat="server"></asp:Label>
                                                        <%--</i>--%>
                                                    </span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                 
                                        <asp:BoundField DataField="WHLCODE" HeaderText="ID" ItemStyle-Width="3%" SortExpression="WHLCODE" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol" />
                                        <%--<asp:BoundField DataField="WHLDATE" HeaderText="DATE" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="3%" />--%>
                                        <asp:TemplateField HeaderText="DATE" SortExpression="WHLDATE" >
                                            <ItemTemplate>
                                                <asp:Literal ID="Literal1" runat="server"
                                                    Text='<%#String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(Eval("WHLDATE"))) %>'>        
                                                </asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="WHLUSER" HeaderText="USER" ItemStyle-Width="5%" SortExpression="WHLUSER" />
                                        <asp:TemplateField HeaderText="PART NUMBER" ItemStyle-Width="5%" SortExpression="IMPTN" >
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
                                                    ToolTip="Update Part" CssClass="clickme" CommandArgument='<%#Eval("IMPTN") %>'>
                                                    <span id="Span2" aria-hidden="true" runat="server">
                                                         <asp:Label ID="txtPartName" Text='<%# Bind("IMPTN") %>' runat="server"></asp:Label>
                                                    </span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <%--<asp:BoundField DataField="IMPTN" HeaderText="PART NUMBER" ItemStyle-Width="4%" />--%>
                                        <asp:BoundField DataField="IMDSC" HeaderText="DESCRIPTION" ItemStyle-Width="15%" SortExpression="IMDSC" />
                                        <asp:BoundField DataField="WHLSTATUS" HeaderText="STATUS" ItemStyle-Width="3%" SortExpression="WHLSTATUS" />
                                        <asp:BoundField DataField="WHLSTATUSU" HeaderText="ASSIGNED" ItemStyle-Width="6%" SortExpression="WHLSTATUSU" />
                                        <asp:BoundField DataField="VENDOR" HeaderText="VENDOR" ItemStyle-Width="3%" SortExpression="VENDOR" /> 
                                        <asp:BoundField DataField="PA" HeaderText="PA" ItemStyle-Width="7%" SortExpression="PA" /> 
                                        <asp:BoundField DataField="PS" HeaderText="PS" ItemStyle-Width="7%" SortExpression="PS" /> 
                                        <asp:BoundField DataField="qtysold" HeaderText="YEAR SALES" ItemStyle-Width="5%" SortExpression="qtysold" />
                                        <asp:BoundField DataField="QTYQTE" HeaderText="QTYQTE" ItemStyle-Width="3%" SortExpression="QTYQTE" />
                                        <asp:BoundField DataField="TIMESQ" HeaderText="TIMESQ" ItemStyle-Width="3%" SortExpression="TIMESQ" />
                                        <asp:BoundField DataField="IMPRC" HeaderText="OEM PRICE" ItemStyle-Width="6%" SortExpression="IMPRC" />
                                        <asp:BoundField DataField="LOC20" HeaderText="LOC20" ItemStyle-Width="2%" SortExpression="LOC20" />
                                        <asp:BoundField DataField="IMMOD" HeaderText="MODEL" ItemStyle-Width="9%" SortExpression="IMMOD" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="IMCATA1" HeaderText="CATEGORY" ItemStyle-Width="10%" SortExpression="IMCATA1" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="SUBCAT" HeaderText="SUBCAT" ItemStyle-Width="3%" SortExpression="SUBCAT" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="IMPC1" HeaderText="MAJOR" ItemStyle-Width="3%" SortExpression="IMPC1" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="IMPC2" HeaderText="MINOR" ItemStyle-Width="3%" SortExpression="IMPC2" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />  
                                                                                 
                                        <asp:TemplateField HeaderText="DETAILS">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkExpander" runat="server" TabIndex="1" ToolTip="Get Reference Detail" CssClass="click-in" CommandName="show"
                                                    OnClientClick='<%# String.Format("return divexpandcollapse(this, {0});", Eval("WHLCODE")) %>'>
                                                    <span id="Span11" aria-hidden="true" runat="server">
                                                        <i class="fa fa-folder"></i>
                                                    </span>
                                                </asp:LinkButton>

                                                </td>
                                                    <tr>
                                                        <td colspan="17" class="padding0">
                                                            <div id="div<%# Eval("WHLCODE") %>" class="divCustomClass">
                                                                <asp:GridView ID="grvDetails" runat="server" AutoGenerateColumns="false" GridLines="None" >
                                                                    <Columns>
                                                                        <asp:BoundField DataField="IMMOD" HeaderText="MODEL" ItemStyle-Width="15%" SortExpression="IMMOD" />
                                                                        <asp:BoundField DataField="IMCATA1" HeaderText="CATEGORY" ItemStyle-Width="10%" SortExpression="IMCATA1" />
                                                                        <asp:BoundField DataField="subcatdesc" HeaderText="SUBCAT" ItemStyle-Width="15%" SortExpression="SUBCAT" />
                                                                        <asp:BoundField DataField="IMPC1" HeaderText="MAJOR" ItemStyle-Width="7%" SortExpression="IMPC1" />
                                                                        <asp:BoundField DataField="minordesc" HeaderText="MINOR" ItemStyle-Width="15%" SortExpression="IMPC2" />
                                                                        <asp:BoundField DataField="a3comment" HeaderText="COMMENT" ItemStyle-Width="25%" SortExpression="IMPC2" />
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
                                    <FooterStyle CssClass="footer-style" HorizontalAlign="Center" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="reloadGrid" class="container">
                <div class="row">
                    <div class="col-md-2"></div>
                    <div class="col-md-4 fullTextBox centered">
                        <asp:LinkButton ID="lnkReloadGrid" class="boxed-btn-layout btn-rounded btnFullSize" OnClick="lnkReloadGrid_Click" runat="server">
                         <i class="fa fa-retweet fa-1x" aria-hidden="true"> </i> <span>RESTORE DATA</span>
                        </asp:LinkButton>
                    </div>
                    <div class="col-md-4 fullTextBox centered">
                        <asp:LinkButton ID="lnkReloadBack" class="boxed-btn-layout btn-rounded btnFullSize" OnClick="lnkReloadBack_Click" runat="server">
                         <i class="fa fa-retweet fa-1x" aria-hidden="true"> </i> <span>RELOAD ALL DATA</span> 
                        </asp:LinkButton>
                    </div>
                    <div class="col-md-2"></div>
                </div>                
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>    
    <script type="text/javascript" src="bootstrap4-input-clearer.js"></script>
    
    <script type="text/javascript"> 

        function messageFormSubmitted(mensaje, show) {
            //debugger
            messages.alert(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
        }

        function messageFormSubmitted1(mensaje, show) {
            //debugger
            messages.alert1(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
        }

        function confirmFormSubmitted(mensaje, show) {
            //debugger
            messages.confirm(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
        }

        function removeHideReload(value) {

            //debugger
            //MainContent_lnkReloadGrid
            $('#MainContent_lnkReloadGrid').closest('.container').removeClass('hideProp')

            messages.alert(value, { type: "info" });
        }

        function afterDdlCheck(hdFieldId, divId) {
            //debugger        

            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }

        //$('body').on('click', '#accordion_2 h5 a', function () {
        //    //debugger
        //    //alert("pepe");
        //    var collapse1 = document.getElementById('collapseOne_2');
        //    isActivePanel(collapse1, 2);
        //});

        //$('body').on('click', '#accordion h5 a', function () {
        //    //debugger
        //    //alert("pepi");
        //    var collapse2 = document.getElementById('collapseOne');
        //    isActivePanel(collapse2, 1);
        //}); 

        function isActivePanel(activePanel, valorActive) {
            //debugger

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;

            if (valorActive == 1) {

                //debugger
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

        function jScriptImportExcel(value) {
            $('#MainContent_btnImportExcel').on('click', function (e) {
                //debugger
                //e.stopPropagation();               
                $('#MainContent_loadFileSection').closest('.container').removeClass('hideProp')
                //alert("pepepepepepepe");
                //$('#<%=hdFileImportFlag.ClientID %>').val("1");

            });
        }  

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

        function yesnoCheck(id) {
            debugger

            x = document.getElementById(id);
            xstyle = document.getElementById(id).style;

            var divs = ["rowStatus", "rowFrom", "rowAssigment"];

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
            
            var collapse2 = document.getElementById('collapseOne_2');
            var collapse3 = document.getElementById('collapseOne_3');
            var collapse4 = document.getElementById('collapseOne_4');

            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hdFileImportFlag.ClientID%>').value;
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
            
            afterRadioCheck(hd2, collapse2)
            afterRadioCheck(hd4, collapse3)
            afterRadioCheck(hd5, collapse4)
            //isActivePanel(collapse1, 1);
            //isActivePanel(collapse2, 2);
        }

        function yesnoCheckCustom(id) {
            //debugger

            if (id != "") {
                x = document.getElementById(id);
                xstyle = document.getElementById(id).style;

                var divs = ["rowAssigment", "rowFrom", "rowStatus"];

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

                            var val1 = $('#<%=hiddenId2.ClientID %>').val();

                            if (iClass == "fa fa-folder" && $('#<%=hiddenId4.ClientID %>').val() == "0") {

                                //iAccess.toggleClass("divCustomClass divCustomClassOk");
                                //iAccess.removeClass('divCustomClass');
                                //iAccess.addClass('divCustomClassOk');

                                //iValue.addClass('fa').removeClass('fa');
                                //iValue.toggleClass('fa-plus fa-minus');//.removeClass('fa-plus');                                

                                //iAccess.closest('td').removeClass('padding0');

                                $('#<%=hiddenId5.ClientID %>').val("1");

                            } else if (iClass == "fa fa-folder-open" && $('#<%=hiddenId1.ClientID %>').val() == "0") {                                

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

        function removeHideReload(value) {

            //debugger
            //MainContent_lnkReloadGrid
            $('#MainContent_lnkReloadGrid').closest('.container').removeClass('hideProp')

            messages.alert(value, { type: "info" });
        }

        function disableInputs() {

            //debugger

            $("#upPanel :input").attr("disabled", true);
            $("#upPanel :input").prop("disabled", true);

            $("#pnProdDev2 :input").attr("disabled", true);
            $("#pnProdDev2 :input").prop("disabled", true);
        }

        function checkHdImportExcel(control) {
            var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdFile == "0")
                $('#<%=hiddenId1.ClientID %>').val("1")
            else
                $('#<%=hiddenId5.ClientID %>').val("0")
        }

        function fixFooterColumns() {
            //debugger

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
            var collapse2 = document.getElementById('collapseOne_2');
            isActivePanel(collapse2, 2);
        });

        //update the user and status by code
        $('body').on('click', '#MainContent_btnUpdate3', function (e) {
            var hdFile = document.getElementById('<%=hiddenId5.ClientID%>').value
            if (hdFile == "1") {
                $('#<%=hiddenId5.ClientID %>').val("0")
            }
        });

        //back button for little update
        $('body').on('click', '#MainContent_btnBack3', function (e) {

            var hdFile = document.getElementById('<%=hdUpdateMedRefFlag.ClientID%>').value
            if (hdFile == "1") {
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")
                $('#<%=hdProdDevFlag.ClientID %>').val("0")
            }
        });
        
        //back buttomn from create new pd
        $('body').on('click', '#MainContent_btnPDBack', function (e) {

            var hdFile = document.getElementById('<%=hdProdDevFlag.ClientID%>').value
            if (hdFile == "1") {
                $('#<%=hdProdDevFlag.ClientID %>').val("0")
                $('#<%=hdExistProj.ClientID %>').val("0")
                $('#<%=hdNewProj.ClientID %>').val("0")   
                resetAllValues1()
                resetAllValues2()
            }
        });

        //show panel to two fields update
        $('body').on('click', '#MainContent_btnUpdate', function (e) {            

            //debugger
            var hdFile = document.getElementById('<%=hiddenId1.ClientID%>').value
            if (hdFile == "0") {
                <%--if ($('#<%=hiddenId5.ClientID%> input:checkbox:checked').length > 0) {--%>
                    //&& $('#<%=hdTriggeredControl.ClientID%> input:checkbox:checked').length < 2)

                    //var Parenttd = $('input[type="checkbox"]:checked').parents('td')[0];
                    //var HiddenValue = $('input[name$=hdnId]', Parenttd).val();
                    //$('#<%=hdLaunchControl.ClientID %>').val(HiddenValue)

                   <%-- $('#<%=hdSelectedClass.ClientID %>').val("0")
                    $('#<%=hdNewRef1Flag.ClientID %>').val("0")
                    $('#<%=hdNewRef2Flag.ClientID %>').val("0")
                    $('#<%=hdNewRef3Flag.ClientID %>').val("0")
                    $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")
                    $('#<%=hdUpdateMedRefFlag.ClientID %>').val("1")
                    $('#<%=hdProdDevFlag.ClientID %>').val("0")--%>
                //}
                //else
                //    messageFormSubmitted("You must select only one row to update.", "warning");                    
            }
        });

        //show add to prod dev form
        $('body').on('click', '#MainContent_btnNewPD', function (e) {
            var hdFile = document.getElementById('<%=hdProdDevFlag.ClientID%>').value
            if (hdFile == "0") {
                $('#<%=hdFileImportFlag.ClientID %>').val("0")
                $('#<%=hdNewRef1Flag.ClientID %>').val("0")
                $('#<%=hdNewRef2Flag.ClientID %>').val("0") 
                $('#<%=hdNewRef3Flag.ClientID %>').val("0")  
                $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")  
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")  
                $('#<%=hdProdDevFlag.ClientID %>').val("1")
            }
        });        

        // show update part from part selection on gridview
        $('body').on('click', '.clickme', function (e) {            

            var hdFile = document.getElementById('<%=hdUpdateFullRefFlag.ClientID%>').value
            if (hdFile == "0") {
                $('#<%=hdFileImportFlag.ClientID %>').val("0")
                $('#<%=hdNewRef1Flag.ClientID %>').val("0")
                $('#<%=hdNewRef2Flag.ClientID %>').val("0")  
                $('#<%=hdNewRef3Flag.ClientID %>').val("0")  
                $('#<%=hdUpdateFullRefFlag.ClientID %>').val("1")  
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")  
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")
            }
        });   

        $('body').on('click', '#MainContent_btnBack2', function (e) {
            
            var hdFile = document.getElementById('<%=hdUpdateFullRefFlag.ClientID%>').value
            if (hdFile == "1") {
                $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")
                $('#<%=hdProdDevFlag.ClientID %>').val("0")
            }
        });

        // show import excel panel
        $('body').on('click', '#MainContent_btnImportExcel', function (e) {
            //debugger            

            var hdFile = document.getElementById('<%=hdProdDevFlag.ClientID%>').value
            if (hdFile == "0") 
                $('#<%=hdFileImportFlag.ClientID %>').val("1")  
                $('#<%=hdNewRef1Flag.ClientID %>').val("0")  
                $('#<%=hdNewRef2Flag.ClientID %>').val("0")  
                $('#<%=hdNewRef3Flag.ClientID %>').val("0")  
                $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")  
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")  
               
        });

        //close import excel panel
        $('body').on('click', "#MainContent_btnBack", function (e) {
            //debugger           

            var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdFile == "1")                
                $('#<%=hdFileImportFlag.ClientID %>').val("0")             
        }); 

        // show new item 1 panel
        $('body').on('click', '#MainContent_btnNewItem', function (e) {
            //debugger

            var hdNew1 = document.getElementById('<%=hdNewRef1Flag.ClientID%>').value
            if (hdNew1 == "0") {
                $('#<%=hdFileImportFlag.ClientID %>').val("0")
                $('#<%=hdNewRef1Flag.ClientID %>').val("1")
                $('#<%=hdNewRef2Flag.ClientID %>').val("0")
                $('#<%=hdNewRef3Flag.ClientID %>').val("0") 
                $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")  
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")  
                $('#<%=hdProdDevFlag.ClientID %>').val("0")
            }
               
            var watermarkPart = 'must be created';

            $('#MainContent_txtPartNumber').val(watermarkPart).addClass('watermark');

            $('#MainContent_txtPartNumber').blur(function () {
                if ($(this).val().length == 0) {
                    $(this).val(watermarkPart).addClass('watermark');
                }
            });

            $('#MainContent_txtPartNumber').focus(function () {
                if ($(this).val() == watermarkPart) {
                    $(this).val('').removeClass('watermark');
                }
            });
        });

        //close panel with part header
        $('body').on('click', "#MainContent_btnBackItemm2", function (e) {
            //debugger

            var hdNew1 = document.getElementById('<%=hdNewRef1Flag.ClientID%>').value
            if (hdNew1 == "1")                
                $('#<%=hdNewRef1Flag.ClientID %>').val("0")

            $('#<%=hdProdDevFlag.ClientID %>').val("0")
            $("#pnAddPartManual").find("input[type=text]").val('');
            
        }); 

        //show panel with part details
        $('body').on('click', '#MainContent_btnSubmitItem', function (e) {
            //debugger

            if ($("#MainContent_txtPartNumber").val().length > 0) {                
                var hdNew3 = document.getElementById('<%=hdNewRef3Flag.ClientID%>').value
                if (hdNew3 == "0")
                    $('#<%=hdFileImportFlag.ClientID %>').val("0")
                $('#<%=hdNewRef1Flag.ClientID %>').val("0")
                $('#<%=hdNewRef2Flag.ClientID %>').val("0")
                $('#<%=hdNewRef3Flag.ClientID %>').val("1")
                $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")
                $('#<%=hdUpdateMedRefFlag.ClientID %>').val("0")
                $('#<%=hdProdDevFlag.ClientID %>').val("0")
            }
            else {

            }
        });

        //back from panel with part details to panel with part header
        $('body').on('click', "#MainContent_btnBackItem2", function (e) {
            //debugger

            var hdNew2 = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdNew2 == "1") {
                $('#<%=hdNewRef2Flag.ClientID %>').val("0")
                $('#<%=hdNewRef3Flag.ClientID %>').val("1")
            }                

            $('#<%=hdUpdateFullRefFlag.ClientID %>').val("0")
            $("#pnAddPartManual2").find("input[type=text]").val(''); 

            var watermarkPart = 'must be created';

            $('#MainContent_txtPartNumber').val(watermarkPart).addClass('watermark');

            $('#MainContent_txtPartNumber').blur(function () {
                if ($(this).val().length == 0) {
                    $(this).val(watermarkPart).addClass('watermark');
                }
            });

            $('#MainContent_txtPartNumber').focus(function () {
                if ($(this).val() == watermarkPart) {
                    $(this).val('').removeClass('watermark');
                }
            });
        }); 

        $('body').on('click', "#MainContent_btBack", function (e) {

            var hdNew3 = document.getElementById('<%=hdNewRef3Flag.ClientID%>').value
            if (hdNew3 == "1") {
                $('#<%=hdNewRef3Flag.ClientID %>').val("0")
                $('#<%=hdNewRef1Flag.ClientID %>').val("1")
            }  

            $("#pnAddPartManual3").find("input[type=text]").val(''); 
        });

        $('body').on('change', "#<%=rdNewProject.ClientID %>", function () {  

            $('#<%=hdExistProj.ClientID %>').val("0")
            $('#<%=hdNewProj.ClientID %>').val("1")           
            
        });

        $('body').on('change', "#<%=rdExistingProject.ClientID %>", function () {

            $('#<%=hdExistProj.ClientID %>').val("1")
            $('#<%=hdNewProj.ClientID %>').val("0")   
            
        });

        function PartNoAutoComplete() {
            $(".autosuggestpart").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "Wish-List.aspx/GetAutoCompleteDataPartNo",
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

        function CustomerNoAutoComplete() {
            //debugger
            $(".autosuggestvendor").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "Wish-List.aspx/GetAutocompleteSelectedVendorName",
                        data: "{'prefixVendorName':'" + document.getElementById("<%=txtVndDesc.ClientID %>").value + "'}",
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
                    $("#<%=hdCustomerNoSelected.ClientID %>").val(autocomplete_value.value);
                    __doPostBack("#<%=hdCustomerNoSelected.ClientID %>", "");
                    $('#MainContent_addNewPartManual2').closest('.container').removeClass('hideProp')
                }
            });
        }

        function CustomerNoAutoComplete1() {
            //debugger
            $(".autosuggestvendor1").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "Wish-List.aspx/GetAutocompleteSelectedVendorName",
                        data: "{'prefixVendorName':'" + document.getElementById("<%=txtNewVendorPD.ClientID %>").value + "'}",
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
                    $("#<%=hdCustomerNoSelected1.ClientID %>").val(autocomplete_value.value);
                    __doPostBack("#<%=hdCustomerNoSelected1.ClientID %>", "");
                    $('#MainContent_addProdDev').closest('.container-fluid').removeClass('hideProp')
                }
            });
        }

        function setUserVis() {
            //function to display the filter options by user privileges
            //debugger
            var hdUserVis = document.getElementById('<%=hdUserVisibility.ClientID%>').value
            if (hdUserVis == "0") {
                $('#accordion_2').css('display', 'inline');
                $('#rwAssigment').css('display', 'block');   
                $('#loadOptions').css('display', 'inline');
                //document.getElementById("MainContent_ddlStatus2").setAttribute("readonly", "false");
                //document.getElementById("MainContent_ddlAssignedTo").setAttribute("readonly", "false");
            }
            else {
                $('#accordion_2').css('display', 'none');
                $('#rwAssigment').css('display', 'none');
                $('#loadOptions').css('display', 'none');
                document.getElementById("MainContent_ddlStatus2").setAttribute("readonly", "true");
                document.getElementById("MainContent_ddlAssignedTo").setAttribute("readonly", "true");
            }
        }

        var timer = null;

        function keepAliveInterval() {
            debugger

            $.ajax(
                {
                    type: "GET",
                    url: "/KeepAlive.ashx",
                    dataType: "text",
                    success: function (response) {
                        confirmFormSubmitted(response, "Info");
                    },
                    error: function (response) {
                        messageFormSubmitted(response, "Error");
                    }
                });
        }

        function keepAlive() {

            debugger

            var currentdate = new Date();
            var param1 = String(currentdate.toLocaleString());
            var parameters = { p1: param1 };
            $.get('/KeepAlive.ashx', parameters
                //, function () { timer = setTimeout(keepAlive, 5000); //alert("entro");}
            )
                .done(function () {    
                    var hdTimeOut = parseInt(document.getElementById('<%=hdSessionToCheckTimeOut.ClientID%>').value)
                    timer = setTimeout(keepAlive, hdTimeOut);
                })
                .fail(function (response) {    
                    if (response != "") {
                        messageFormSubmitted1(response, "Error");
                        window.location.href = "https://localhost:44392/Wish-List.aspx";
                    }
                    
                    //alert(response);
                })
                .always(function (response) {
                    if (response != "") {
                        confirmFormSubmitted(response, "Info");
                    }
                    
                    //alert(response);
                })
            //timer = setTimeout(keepAliveInterval, 5000);
        }
        
        $(function () {
            
            debugger
            var watermarkPart = 'must be created';
            $('#MainContent_txtPartNumber').val(watermarkPart).addClass('watermark');

            $('#MainContent_txtPartNumber').blur(function () {
                if ($(this).val().length == 0) {
                    $(this).val(watermarkPart).addClass('watermark');
                }
            });

            $('#MainContent_txtPartNumber').focus(function () {
                if ($(this).val() == watermarkPart) {
                    $(this).val('').removeClass('watermark');
                }
            });

            var watermarkSearch = 'Search...';

            $('#MainContent_txtSearch').val(watermarkSearch).addClass('watermark');

            $('#MainContent_txtSearch').blur(function () {
                if ($(this).val().length == 0) {
                    $(this).val(watermarkSearch).addClass('watermark');
                }
            });

            $('#MainContent_txtSearch').focus(function () {
                if ($(this).val() == watermarkSearch) {
                    $(this).val('').removeClass('watermark');
                }
            });

            $('#MainContent_txtDate').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                });  

            $('#MainContent_txDate').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                });             

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CustomerNoAutoComplete);

            //PartNoAutoComplete()
            CustomerNoAutoComplete()
            CustomerNoAutoComplete1()

            var hdCNS = document.getElementById('<%=hdCustomerNoSelected.ClientID%>').value
            if (hdCNS != "0") {
                $('#MainContent_addNewPartManual2').closest('.container').removeClass('hideProp')
                $('#<%=txtVndDesc.ClientID %>').val(hdCNS)
            }

            var hdCNS1 = document.getElementById('<%=hdCustomerNoSelected1.ClientID%>').value
            if (hdCNS1 != "0") {
                $('#MainContent_addProdDev').closest('.container-fluid').removeClass('hideProp')
                $('#<%=txtNewVendorPD.ClientID %>').val(hdCNS1)
            }

            var hdMessage = document.getElementById('<%=hdHideMessage.ClientID%>').value
            if (hdMessage != "0") {
                messageFormSubmitted(hdMessage, "warning");
            }

            var hdMessageVnd = document.getElementById('<%=hdHideMessageVendor.ClientID%>').value
            if (hdMessageVnd != "0") {
                messageFormSubmitted(hdMessageVnd, "warning");
            }

            //var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;               
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd5, collapse4);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd4, collapse3);

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);            

            //var collapse1 = document.getElementById('collapseOne');
            //afterDdlCheck(hd1, collapse1);  

            //divexpandcollapse(divname);
            $('select').clearer();
            $('#MainContent_txtSearch').clearer();

            fixFooterColumns();            

            //var hdTimeOut = parseInt(document.getElementById('<%=hdSessionToCheckTimeOut.ClientID%>').value) 
            //timer = setTimeout(keepAlive, hdTimeOut);            

            //window.setInterval('keepAliveInterval()', 5000);

            //$('.footer-style').children("td:contains(' ')").addClass('hidecol');            

        })        

        //Sys.Application.add_init(function () {
            
        //}); 

        function resetAllValues1() {
            //clear new project inputs
            $('#MainContent_dvNewProject').find('input:text').val('');
            $('.custom-radio1').prop('checked', false);            
        }

        function resetAllValues2() {
            //clear existing project inputs
            $('#MainContent_dvExistingProject').find('input:text').val('');
            $('.custom-radio2').prop('checked', false);
        }

        function fillPartInfo() {
            //debugger

            var hdReason = $('#<%=hdReasonTypePD.ClientID %>').val()
            $('#<%=txtReasonTypePD.ClientID %>').val(hdReason)

            var hdComments = $('#<%=hdCommentsPD.ClientID %>').val()
            $('#<%=hdCommentsPD.ClientID %>').val(hdComments)

            var hdMinorCode = $('#<%=hdMinorCodePD.ClientID %>').val()
            $('#<%=txtMinorCodePD.ClientID %>').val(hdMinorCode)

            var hdOEMPrice = $('#<%=hdOEMPricePD.ClientID %>').val()
            $('#<%=txtOEMPricePD.ClientID %>').val(hdOEMPrice)

            var hdTimesQuote = $('#<%=hdTimesQuoteLY.ClientID %>').val()
            $('#<%=txtTimesQuoteLY.ClientID %>').val(hdTimesQuote)

            var hdQtySold = $('#<%=hdQtySoldPD.ClientID %>').val()
            $('#<%=txtQtySoldPD.ClientID %>').val(hdQtySold)

            var hdCurrentVend = $('#<%=hdCurrentVendor.ClientID %>').val()
            $('#<%=txtCurrentVendor.ClientID %>').val(hdCurrentVend)

            var hdAssignedTo = $('#<%=hdAssignedToPD.ClientID %>').val()
            $('#<%=txtAssignedToPD.ClientID %>').val(hdAssignedTo)

            var hdDescription = $('#<%=hdDescriptionPD.ClientID %>').val()
            $('#<%=txtDescriptionPD.ClientID %>').val(hdDescription)

            var hdUserCreat = $('#<%=hdUserCreated.ClientID %>').val()
            $('#<%=txtUserCreated.ClientID %>').val(hdUserCreat)

            var hdCreationDat = $('#<%=hdCreationDate.ClientID %>').val()
            $('#<%=txtCreationDate.ClientID %>').val(hdCreationDat)

            var hdWhlCod = $('#<%=hdWhlCode.ClientID %>').val()
            $('#<%=txtWhlCode.ClientID %>').val(hdWhlCod)
            
        }

        function pageLoad(event, args) {  

            //alert("pageload");
            //debugger

            $('#MainContent_txtDate').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                }); 

            $('#MainContent_txDate').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                }); 

           if (args.get_isPartialLoad()) {
                //debugger            
                //case fileExcel  
                var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
                if (hdFile == "1") {
                   $('#MainContent_loadFileSection').closest('.container').removeClass('hideProp')
               }   

               //case new part
               var hdNew1 = document.getElementById('<%=hdNewRef1Flag.ClientID%>').value
               if (hdNew1 == "1") {
                   $('#MainContent_addNewPartManual').closest('.container').removeClass('hideProp')
               }  

               //case new part
               var hdNew2 = document.getElementById('<%=hdNewRef2Flag.ClientID%>').value
               if (hdNew2 == "1") {
                   $('#MainContent_addNewPartManual2').closest('.container').removeClass('hideProp')
               }  

               //case new part recent
               var hdNew3 = document.getElementById('<%=hdNewRef3Flag.ClientID%>').value
               if (hdNew3 == "1") {
                   $('#MainContent_addNewPartManual3').closest('.container').removeClass('hideProp')
               }  

               var hdCNS = document.getElementById('<%=hdCustomerNoSelected.ClientID%>').value
               if (hdCNS != "0") {
                   $('#MainContent_addNewPartManual2').closest('.container').removeClass('hideProp')
               }

               var hdCNS1 = document.getElementById('<%=hdCustomerNoSelected1.ClientID%>').value
               if (hdCNS1 != "0") {
                   $('#MainContent_addProdDev').closest('.container').removeClass('hideProp')
               }

               var hdExistProjj = document.getElementById('<%=hdExistProj.ClientID%>').value
               if (hdExistProjj == "1") {
                   $('#MainContent_dvExistingProject').closest('.container-fluid').removeClass('hideProp')
                   $('#MainContent_dvNewProject').closest('.container-fluid').addClass('hideProp')    
                   resetAllValues1()
               }
               else {                   
                   $('#MainContent_dvExistingProject').closest('.container-fluid').addClass('hideProp')  
                   resetAllValues2()
               }

               var hdNewProjj = document.getElementById('<%=hdNewProj.ClientID%>').value
               if (hdNewProjj == "1") {
                   $('#MainContent_dvExistingProject').closest('.container-fluid').addClass('hideProp')
                   $('#MainContent_dvNewProject').closest('.container-fluid').removeClass('hideProp')
                   resetAllValues2()
               }
               else {
                   $('#MainContent_dvNewProject').closest('.container-fluid').addClass('hideProp')
                   resetAllValues1()
               }

               var hdDiffVnd = document.getElementById('<%=hdDifVnd.ClientID%>').value
               if (hdDiffVnd == "0") {
                   debugger
                   $('#MainContent_dvVndValidation').closest('.custom-div').removeClass('hideProp') 
               }
               else {
                   $('#MainContent_dvVndValidation').closest('.custom-div').addClass('hideProp')                   
               }

               var hdUpd = document.getElementById('<%=hdUpdateFullRefFlag.ClientID%>').value
               if (hdUpd == "1") {
                   $('#MainContent_updatePart').closest('.container-fluid').removeClass('hideProp')
               }
               else {
                   $('#MainContent_updatePart').closest('.container-fluid').addClass('hideProp')
               }

               var hdProdDev = document.getElementById('<%=hdProdDevFlag.ClientID%>').value
               if (hdProdDev == "1") {
                   $('#MainContent_addProdDev').closest('.container-fluid').removeClass('hideProp')
               }

               var hdUpd1 = document.getElementById('<%=hdUpdateMedRefFlag.ClientID%>').value
               if (hdUpd1 == "1") {
                   $('#MainContent_updatePart2').closest('.container-fluid').removeClass('hideProp')
               }

               var hdMessage = document.getElementById('<%=hdHideMessage.ClientID%>').value
               if (hdMessage != "0") {
                   messageFormSubmitted(hdMessage, "warning");
               }

               var hdMessageVnd = document.getElementById('<%=hdHideMessageVendor.ClientID%>').value
               if (hdMessageVnd != "0") {
                   messageFormSubmitted(hdMessageVnd, "warning");                   
               } 

               // nested gridview

               var hd = document.getElementById('<%=hdLinkExpand.ClientID%>').value;
               var hd1 = document.getElementById('<%=hdTriggeredControl.ClientID%>').value;
               var hd11 = document.getElementById('<%=hdLaunchControl.ClientID%>').value;

                var iAccess = $("#div" + hd1);
                var iContainer = $("#" + hd11);                

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

               fillPartInfo();

               fixFooterColumns();

               //$('.footer-style').children("td:contains(' ')").addClass('hidecol');

               var hdWelcome = document.getElementById('<%=hdWelcomeMess.ClientID%>').value
               $('#<%=lblUserLogged.ClientID %>').val(hdWelcome); 
            }

            var hdWelcome = document.getElementById('<%=hdWelcomeMess.ClientID%>').value
            $('#<%=lblUserLogged.ClientID %>').val(hdWelcome); 

            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
            var hd4 = document.getElementById('<%=hiddenId4.ClientID%>').value;
            var hd5 = document.getElementById('<%=hiddenId5.ClientID%>').value;


            var hdName = document.getElementById('<%=hiddenName.ClientID%>').value;
            yesnoCheckCustom(hdName)

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);

            var collapse4 = document.getElementById('collapseOne_4');
            afterDdlCheck(hd5, collapse4);

            var collapse3 = document.getElementById('collapseOne_3');
            afterDdlCheck(hd4, collapse3);    

            var hdFile = document.getElementById('<%=hdFileImportFlag.ClientID%>').value
            if (hdFile == "1") {
                $('#MainContent_loadFileSection').closest('.container').removeClass('hideProp')
            } 

            var hdDiffVnd = document.getElementById('<%=hdDifVnd.ClientID%>').value
            if (hdDiffVnd == "0") {
                debugger
                $('#MainContent_dvVndValidation').closest('.custom-div').removeClass('hideProp')
            }
            else {
                $('#MainContent_dvVndValidation').closest('.custom-div').addClass('hideProp')
            }

            setUserVis()
            //rer();

            $('#MainContent_txtSearch').clearer();     

            CustomerNoAutoComplete();
            CustomerNoAutoComplete1();

            disableInputs()

            fillPartInfo();

            fixFooterColumns();
            //$('.footer-style').children("td:contains(' ')").addClass('hidecol');

            //var collapse1 = document.getElementById('collapseOne');
            //afterDdlCheck(hd1, collapse1);  
        }    

    </script>

</asp:Content>
