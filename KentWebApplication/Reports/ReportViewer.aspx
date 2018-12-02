<%@ page title="" language="C#" masterpagefile="~/Site.Master" autoeventwireup="true" codebehind="ReportViewer.aspx.cs" inherits="KentWebApplication.Reports.ReportViewer" %>

<%@ register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<asp:content id="Content1" contentplaceholderid="head" runat="server">
</asp:content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
    <section class="content">
        <asp:panel id="pnlDetails" runat="server">
            <div class="row">
                <div class="col-lg-12">
                    <rsweb:reportviewer id="ReportViewer1" runat="server" width="100%" height="600px">
                    </rsweb:reportviewer>
                </div>
            </div>
        </asp:panel>
    </section>
</asp:content>
<asp:content id="Content3" contentplaceholderid="footer" runat="server">
</asp:content>
