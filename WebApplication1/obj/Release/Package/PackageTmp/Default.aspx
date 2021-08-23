<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="WebApplication1._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-9">
            <asp:HiddenField id="hdWelcomeMess" Value="" runat="server" />
        </div>
        <div class="col-md-2">
            <asp:Label ID="lblUserLogged" Text="" runat="server" ></asp:Label>
        </div>
        <div class="col-md-1">
            <asp:LinkButton ID="lnkLogout" Text="Click to Logout." OnClick="lnkLogout_Click" runat="server"></asp:LinkButton>
        </div>
    </div>

    <div class="container-fluid">
        <div id="img-carousel"></div>
    </div>    

    <div class="container">
        <div id="content-area">
            <div class="row">
                <h1>WE ARE COMMITTED TO DELIVERING EXCELLENCE</h1>
                <p id="text-content-area">
                    Costex Tractor Parts CTP® is a worldwide quality supplier of New Replacement Parts for Caterpillar® and
            Komatsu® Equipment and Engines. At CTP, we not only offer you  premium parts  but  also an exceptional 
            service, outstanding savings and the support you need to get your order quickly and accurately. At CTP, 
            our focus is to provide you with great quality parts at a good value. Our new replacement parts undergo
            strenuous and strict inspection procedures to ensure the quality of  our product. Our basic  principle: 
            Quality with Value Guaranteed. Whether you are mining copper ore at high altitudes in the Chilean Andes,
            or digging an irrigation canal on the family farm, you can be rest assured that our whole team is 
            behind you every step of the way.              

            <a class="base-color nav-link" href="https://www.costex.com" target="_blank">Costex Online Site »</a>
                </p>
            </div>
        </div>
    </div>

    <script type="text/javascript">

        function pageLoad(event, args) {

            if (args.get_isPartialLoad()) {

                var hdWelcome = document.getElementById('<%=hdWelcomeMess.ClientID%>').value
                $('#<%=lblUserLogged.ClientID %>').val(hdWelcome); 

            }

            var hdWelcome = document.getElementById('<%=hdWelcomeMess.ClientID%>').value
            $('#<%=lblUserLogged.ClientID %>').val(hdWelcome); 
        }

    </script>
    

</asp:Content>
