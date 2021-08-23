<%@ Page Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeBehind="Lost-Sales1.aspx.vb" Inherits="WebApplication1.Lost_Sales1" %>

<%--EnableViewState="true" ViewStateMode="Disabled"--%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="updatepnl1" UpdateMode="Conditional" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="submit" />             
        </Triggers>
        <ContentTemplate>  

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

            <div class="container">          
                <div id="rowFilters" class="row">
                    <div class="col-md-1"></div>
                    <div class="col-md-4">
                        <div class="accordion-wrapper">
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
                                            <ul class="checklist">
                                                <li><i class="fa fa-check"></i><span id="spnCountItems">COUNT ITEMS:</span><asp:Label ID="lblItemsCount" runat="server"></asp:Label></li>
                                                <li><i class="fa fa-check"></i><span id="spnTimesQuotes">TIMES QUOTE: (Current value): </span><asp:Label ID="lblTimesQuote" runat="server"></asp:Label> </li>
                                            </ul>
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
                                        <div id="collapseOne_2" class="collapse show" aria-labelledby="headingOne_2" data-parent="#accordion_2" style="">
                                            <div class="card-body">
                                                <div id="rowRadios" class="row">
                                                    <%--<div style="width:100%">--%>
                                                        <input type="hidden" name="oldtimesquoted" value="100">
                                                        <div class="form-group col-md-3 radio-toolbar">
                                                            <label class="form-check"> <p>10+</p> 
                                                                <asp:RadioButton id="tqr10" GroupName="radio" OnCheckedChanged="tqr10_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark"></span>
                                                            </label> 
                                                        </div>
                                                        <div class="form-group col-md-3 radio-toolbar">
                                                            <label class="form-check"> <p>30+</p>
                                                                <asp:RadioButton id="tqr30" GroupName="radio" OnCheckedChanged="tqr30_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark"></span>
                                                            </label> 
                                                        </div>
                                                        <div class="form-group col-md-3 radio-toolbar">
                                                            <label class="form-check"> <p>50+</p>
                                                                <asp:RadioButton id="tqr50" GroupName="radio" OnCheckedChanged="tqr50_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark"></span>
                                                            </label>  
                                                        </div>
                                                        <div class="form-group col-md-3 radio-toolbar">
                                                            <label class="form-check"> <p>100+</p>
                                                                <asp:RadioButton id="tqr100" GroupName="radio" OnCheckedChanged="tqr100_CheckedChanged" AutoPostBack="true" runat="server"></asp:RadioButton>
                                                                <span class="checkmark"></span>
                                                            </label>                                                    
                                                        </div>                                               
                                                    <%--</div>--%>
                                                </div>                                        
                                                <div class="row">
                                                    <!--SHORT WAY-->
                                                    <!--times quote : rendering-->
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lbltqId" Text="TIMES QUOTE:10-150" runat="server"></asp:Label>
                                                        <asp:TextBox name="num-tq" id="tqId" class="form-control" TextMode="Number" min="10" max="150" OnTextChanged="tqId_TextChanged" runat="server" title="TIMES QUOTE:10-150"></asp:TextBox>       
                                                        <%--<asp:RangeValidator ErrorMessage="The value must be from 10 to 150!" ControlToValidate="tqId" MinimumValue="10" MaximumValue="150" Type="Integer" EnableClientScript="false" runat="server" />--%>
                                                    </div>

                                                    <!--LONG WAY-->
                                                    <!--vendor assigned : rendering-->
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lblVndAss" Text="Vendors Assigned" runat="server"></asp:Label>    
                                                        <asp:DropDownList ID="ddlVendAssign" name="sel-vndassigned" AutoPostBack="true" class="form-control" title="Both: It shows Parts with vendors assigned and without vendors assigned at the same time." runat="server"></asp:DropDownList>                                                                                           </div>
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
                                            </div>                                    
                                        </div>
                                        <!-- COLLAPSE CONTENT END -->                            
                                </div>
                                <!-- CARD END -->
                            </div>
                            <!-- ACCORDION END -->
                        </div>

                    </div>
                    <div class="col-md-1"></div>
                </div>
            </div>


            </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="updatepnl2" UpdateMode="Conditional" runat="server">
        <ContentTemplate>

            <div class="container-fluid">
                    <div class="row">
                        <div class="col-md-3">
                            <div id="rowPageSize" class="row">
                                <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" ><asp:Label ID="lblText1" Text="Show " runat="server"></asp:Label></div>
                                <div class="col-xs-12 col-sm-6 flex-item-2 padd-fixed"><asp:DropDownList name="ddlPageSize" ID="ddlPageSize" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" class="form-control" runat="server"></asp:DropDownList></div>
                                <div class="col-xs-12 col-sm-3 flex-item-1 padd-fixed" ><asp:Label ID="lblText2" Text=" entries." runat="server"></asp:Label></div>
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
                                <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
                            </div>
                        </div>
                    </div>        
                </div>

            <div class="row"  style="display: none !important;">
                <asp:DropDownList ID="ddlSaleLast12Foot" OnSelectedIndexChanged ="ddlSaleLast12Foot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlVndNameFoot" OnSelectedIndexChanged ="ddlVndNameFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlWLFoot" OnSelectedIndexChanged ="ddlWLFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlMajorFoot" OnSelectedIndexChanged ="ddlMajorFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:DropDownList ID="ddlCategoryFoot" OnSelectedIndexChanged ="ddlCategoryFoot_SelectedIndexChanged" AutoPostBack="true" class="form-control-custom-sel" EnableViewState="true" ViewStateMode="Enabled" runat="server" ></asp:DropDownList>
                <asp:Button ID="ButtonAdd1" class="btn btn-inverse btn-primary btn-sm" CommandName="AddAll" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" runat="server" Text="Add Selected" />
                <asp:Label ID="lblGrvGroup" Text="test" runat="server"></asp:Label>
                <table id="ndtt" runat="server"></table>

                <asp:HiddenField ID="hiddenId1" Value="0" runat="server" />
                <asp:HiddenField ID="hiddenId2" Value="0" runat="server" />     
                <asp:HiddenField ID="hiddenId3" Value="0" runat="server" />  

                <asp:HiddenField ID="hdLinkExpand" value="0" runat="server" />
                <asp:HiddenField ID="hdTriggeredControl" value="" runat="server" />
                <asp:HiddenField ID="hdLaunchControl" value="" runat="server" />
                <asp:HiddenField ID="hdSelectedClass" value="" runat="server" />

                <asp:HiddenField ID="hdVendorAssigned" value="0" runat="server" />
            </div>

            <div class="container-fluid">
                <div class="panel panel-default">
                    <div class="panel-body">                
                        <div class="form-horizontal"> 

                            <div id="rowGridView">
                                <asp:GridView ID="grvLostSales" runat="server" AutoGenerateColumns="false"
                                    PageSize="10" CssClass="table table-striped table-bordered" AllowPaging="True" AllowCustomPaging="true" AllowSorting="true"
                                    GridLines="None" OnRowCommand="grvLostSales_RowCommand" OnPageIndexChanging="grvLostSales_PageIndexChanging"
                                    OnRowDataBound="grvLostSales_RowDataBound" OnSorting="grvLostSales_Sorting" ShowHeader="true" ShowFooter="true" 
                                    OnRowUpdating="grvLostSales_RowUpdating" DataKeyNames="IMPTN"  >
                                    <Columns>
                                        <asp:TemplateField ItemStyle-Width="3%">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkAll" Text="" Visible="true" Checked="False" runat="server" OnCheckedChanged="chkAll_CheckedChanged" AutoPostBack="True"
                                                    ToolTip="Select All" EnableViewState="true" ViewStateMode="Enabled"></asp:CheckBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <span style="padding: 10px;">
                                                    <asp:CheckBox ID="chkSingleAdd" runat="server" Checked="False" ToolTip="Select to Wish List" />
                                                </span>
                                            </ItemTemplate>
                                            <FooterStyle HorizontalAlign="Right" />
                                            <FooterTemplate>
                                                <asp:Button ID="ButtonAdd" class="btn btn-inverse btn-primary btn-sm" CommandName="AddAll" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" 
                                                     runat="server" Text="Add Selected" />
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
                                                    ToolTip="Add Single Reference">
                                                    <span id="Span1" aria-hidden="true" runat="server">
                                                        <i class="fa fa-plus"></i></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                 
                                        <asp:BoundField DataField="IMPTN" HeaderText="PART NUMBER" ItemStyle-Width="5%" SortExpression="IMPTN" /><%-- 2 --%>
                                        <asp:BoundField DataField="IMDSC" HeaderText="DESCRIPTION" ItemStyle-Width="6%" SortExpression="IMDSC" />
                                        <asp:BoundField DataField="IMDS2" HeaderText="DESCRIPTION 2" ItemStyle-Width="7%" SortExpression="IMDS2" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="IMDS3" HeaderText="DESCRIPTION 3" ItemStyle-Width="7%" SortExpression="IMDS3" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />
                                        <asp:BoundField DataField="TQUOTE" HeaderText="QTY QTE" ItemStyle-Width="3%" SortExpression="TQUOTE" />
                                        <asp:BoundField DataField="TIMESQ" HeaderText="TIMES QTE" ItemStyle-Width="4%" SortExpression="TIMESQ" />
                                        <asp:BoundField DataField="NCUS" HeaderText="CUSTS.QUOTE" ItemStyle-Width="4%" SortExpression="NCUS" />   
                                        <asp:BoundField DataField="QTYSOLD" HeaderText="SALES LAST12" ItemStyle-Width="5%" SortExpression="QTYSOLD" />
                                        <asp:BoundField DataField="VENDOR" HeaderText="VND NO" ItemStyle-Width="3%" SortExpression="VENDOR" />
                                        <asp:BoundField DataField="VENDORNAME" HeaderText="VND NAME" ItemStyle-Width="7%" SortExpression="VENDORNAME" />
                                        <asp:BoundField DataField="PAGENT" HeaderText="P.AGENT" ItemStyle-Width="4%" SortExpression="PAGENT" /><%-- 12 --%>
                                        <asp:BoundField DataField="IMPRC" HeaderText="LIST PRICE" ItemStyle-Width="4%" SortExpression="IMPRC" />
                                        <asp:BoundField DataField="WLIST" HeaderText="WL" ItemStyle-Width="4%" SortExpression="WLIST" />
                                        <asp:BoundField DataField="PROJECT" HeaderText="DEV.PROJ" ItemStyle-Width="4%" SortExpression="PROJECT" />
                                        <asp:BoundField DataField="PROJSTATUS" HeaderText="DEV.STATUS" ItemStyle-Width="4%" SortExpression="PROJSTATUS" />
                                        <asp:BoundField DataField="F20" HeaderText="LOC.20" ItemStyle-Width="3%" SortExpression="F20" />
                                        <asp:BoundField DataField="FOEM" HeaderText="OEM VND" ItemStyle-Width="3%" SortExpression="FOEM" />
                                        <asp:BoundField DataField="IMPC1" HeaderText="MAJOR" ItemStyle-Width="4%" SortExpression="IMPC1" />
                                        <asp:BoundField DataField="CATDESC" HeaderText="CATEGORY" ItemStyle-Width="5%" SortExpression="CATDESC" />
                                        <asp:BoundField DataField="IMPC2" HeaderText="MINOR" ItemStyle-Width="3%" SortExpression="IMPC2" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol" />
                                        <asp:BoundField DataField="MINDSC" HeaderText="DESC" ItemStyle-Width="9%" SortExpression="MINDSC" ItemStyle-CssClass="hidecol"  HeaderStyle-CssClass="hidecol"  />    
                                        <asp:TemplateField HeaderText="Total Clients" ItemStyle-Width="3%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <HeaderTemplate>Total Clients</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTClients" Text="" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Total Countries" ItemStyle-Width="3%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <HeaderTemplate>Total Countries</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTCountries" Text="" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="OEM Part" ItemStyle-Width="3%">
                                            <HeaderStyle CssClass="GridHeaderStyle" />
                                            <ItemStyle CssClass="GridHeaderStyle" />
                                            <HeaderTemplate>OEM Part</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblOEMPart" Text="" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DETAILS">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDetails" runat="server" TabIndex="1" ToolTip="Get Reference Detail" CssClass="click-in" CommandName="show" CausesValidation="false"
                                                    OnClientClick='<%# String.Format("return divexpandcollapse(this, {0});", "BT339") %>'>
                                                    <span id="Span12" aria-hidden="true" runat="server">
                                                        <i class="fa fa-folder"></i>
                                                    </span>
                                                </asp:LinkButton>
                                                </td>
			                                        <tr>
                                                        <td colspan="23" class="padding0">
                                                            <div id="div<%# Trim(Eval("IMPTN").ToString()) %>" class="divCustomClass">
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
                                    <PagerSettings  Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" pagebuttoncount="10"  />
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

        function messageFormSubmitted(mensaje, show) {
            debugger
            messages.alert(mensaje, { type: show });
            //setTimeout(function () {
            //    $("#myModal").hide();
            //}, 3000);
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

        function fixFooterColumns() {
            debugger

            $(".footer-style").filter(function () {
                debugger

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
            if (hdFieldId == 1) {
                divId.className = "collapse show"
            } else {
                divId.className = "collapse"
            }
        }        

        function removeHideReload(value) {

            debugger
            //MainContent_lnkReloadGrid
            $('#MainContent_lnkReloadGrid').closest('.container').removeClass('hideProp')

            messages.alert(value, { type: "info" });
        }        

        function isActivePanel(activePanel, valorActive) {
            debugger

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
                        $('#<%=hiddenId3.ClientID %>').val("1");
                        hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                        //afterDdlCheck(hd2, activePanel)
                    }
                    else {
                        $('#<%=hiddenId2.ClientID %>').val("0");
                        $('#<%=hiddenId3.ClientID %>').val("1");
                        hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;
                        //afterDdlCheck(hd2, activePanel)
                    }
                }  

            JSFunction();
        }

        function JSFunction() {
            __doPostBack('<%= updatepnl1.ClientID  %>', '');
        }

        $('body').on('click', '#accordion_2 h5 a', function () {
            //debugger
            //alert("pepe");
            var collapse1 = document.getElementById('collapseOne_2');
            isActivePanel(collapse1, 2);

            //refreshAccordion();
        });

        $('body').on('click', '#accordion h5 a', function () {
            //debugger
            //alert("pepi");
            var collapse2 = document.getElementById('collapseOne');
            isActivePanel(collapse2, 1);

            //refreshAccordion();
        });  

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

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;

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

            fixFooterColumns();

            //refreshAccordion();
        })       

        function pageLoad(event, args) {
            debugger     
            //alert("pageload");   

            //refreshAccordion();

            if (args.get_isPartialLoad()) {

                //alert("partial");

                // nested gridview

                var hd = document.getElementById('<%=hdLinkExpand.ClientID%>').value;
                var hd1 = document.getElementById('<%=hdTriggeredControl.ClientID%>').value;
                var hd2 = document.getElementById('<%=hiddenId1.ClientID%>').value;


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

                fixFooterColumns();
            }           

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;

            var collapse2 = document.getElementById('collapseOne_2');            
            afterDdlCheck(hd2, collapse2);

            var collapse1 = document.getElementById('collapseOne');            
            afterDdlCheck(hd1, collapse1);

            $('select').clearer();
            $('#MainContent_txtSearch').clearer();

            fixFooterColumns();
        }

        function refreshAccordion() {
            debugger

            //var prm = Sys.WebForms.PageRequestManager.getInstance();
            if (typeof (Sys) != "undefined") {
                //alert("nuevo");
                Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(complexCall);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(complexCall);
            }
            else {
                alert("serio");
            }
        }

        function complexCall() {
            debugger

            var hd1 = document.getElementById('<%=hiddenId1.ClientID%>').value;
            var hd2 = document.getElementById('<%=hiddenId2.ClientID%>').value;

            var collapse2 = document.getElementById('collapseOne_2');
            afterDdlCheck(hd2, collapse2);

            var collapse1 = document.getElementById('collapseOne');
            afterDdlCheck(hd1, collapse1); 
        }

    </script>

</asp:Content>
