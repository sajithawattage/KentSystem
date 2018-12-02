<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="KentWebApplication.Menu" %>

<ul class="sidebar-menu">
    <li><asp:hyperlink id="hlLocation" runat="server" navigateurl="~/Home.aspx"><i class="fa fa-dashboard"></i><span>Home</span></asp:hyperlink></li>
    <li><asp:hyperlink id="Hyperlink1" runat="server" navigateurl="~/Profile.aspx"><i class="fa fa-user"></i><span>Profile</span></asp:hyperlink></li>
</ul>
