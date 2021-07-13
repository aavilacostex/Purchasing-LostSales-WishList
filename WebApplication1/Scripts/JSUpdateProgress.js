Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginReq);
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endReq);

function beginReq(sender, args) {
    // shows the Popup
    //$find(ventanaModal).show();
    //$find("<%=ventanaModal.ClientID%>").show();

    if ($find(ventanaModal) != null) {
        $find(ventanaModal).show();
    }
}

function endReq(sender, args) {
    //  shows the Popup
    if ($find(ventanaModal) != null) {
        $find(ventanaModal).hide();
    }
} 
