<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Notification.ascx.cs" Inherits="KentWebApplication.Common.Controls.Notification" %>

<asp:placeholder id="phErrorMessages" runat="server">
    
    <div class="alert alert-danger alert-dismissable" runat="server" id="error_alert" visible="false">
        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
        <b>Alert!</b>
        <asp:literal id="litErrorMessage" runat="server"></asp:literal>
    </div>

</asp:placeholder>

<asp:placeholder id="phInfoMessage" runat="server">

    <div class="alert alert- alert-dismissable" runat="server" id="Div1" visible="false">
        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
        <b>Alert!</b>
        <asp:literal id="Literal1" runat="server"></asp:literal>
    </div>

</asp:placeholder>
