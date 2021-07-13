<%--<%@ Page Language="vb" AutoEventWireup="true" CodeFile="AutoComplete.aspx.vb" Inherits="AutoComplete" %>--%>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>AutoComplete Box with jQuery</title>
    <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/jquery-ui.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $(".autosuggest").autocomplete({
               source: function (request, response) {
                   $.ajax({
                       type: "POST",
                       contentType: "application/json;charset=utf-8",
                       url: "AutoComplete.aspx/GetAutoCompleteData",
                       data: "{'prefixText':'" + document.getElementById('txtSearch').value + "'}",
                       dataType: "json",
                       success: function (data) {
                            response(data.d);
                       },
                       error: function (result) {
                            alert("Error");
                       }
                   });
               }
            });
        });
 
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ui-widget">
            <label>Enter Value: </label>
            <asp:TextBox runat="server" ID="txtSearch" CssClass="autosuggest"></asp:TextBox>
        </div>
    </form>
</body>
</html>