<%@ page title="" language="C#" masterpagefile="~/Site.Master" autoeventwireup="true" codebehind="EstimateQuotationList.aspx.cs" inherits="KentWebApplication.EstimateQuotationList" %>

<asp:content id="Content1" contentplaceholderid="head" runat="server">
</asp:content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
    <section class="content-header">
        <div class="alert alert-danger alert-dismissable" runat="server" id="error_alert" visible="false">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
            <b>Alert!</b>
            <asp:literal id="litErrorMessage" runat="server"></asp:literal>
        </div>
        <div class="alert alert-success alert-dismissable" runat="server" id="success_alert" visible="false">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
            <b>Alert!</b>
            <asp:literal id="litSuccessMessage" runat="server"></asp:literal>
        </div>
    </section>
    <section class="content">
        <asp:panel id="pnlDetails" runat="server">
            <div class="row">
                <div class="col-lg-12">
                    <h2 class="page-header">
                        <i class="fa fa-globe"></i>
                        <asp:literal id="litProjectName" runat="server"></asp:literal>
                    </h2>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 table-responsive">
                    <asp:gridview id="grdMaterialRequest" runat="server"
                        cssclass="Grid"
                        enablemodelvalidation="True"
                        autogeneratecolumns="false"
                        allowpaging="true"
                        allowsorting="false"
                        emptydatatext="No records Found"
                        datakeynames="MRNumber,CustomerCode, JobCode, EngineerStatus, ManagerStatus"
                        onrowcommand="grdMaterialRequest_RowCommand"
                        onpageindexchanging="grdMaterialRequest_PageIndexChanging"
                        pagesize="10">
                        <columns>
                            <asp:boundfield datafield="MRNumber" headertext="Sub Estimate No"
                                itemstyle-width="15%" />
                            <asp:boundfield datafield="ReceivedDate"
                                headertext="Order Date"
                                itemstyle-width="15%" 
                                DataFormatString="{0:yyyy/MM/dd}"/>
                            <asp:boundfield datafield="RequiredDate"
                                headertext="Required Date"
                                itemstyle-width="15%"
                                DataFormatString="{0:yyyy/MM/dd}" />
                            <asp:boundfield datafield="Remarks" headertext="Remarks"
                                itemstyle-width="30%"/>
                            <asp:boundfield datafield="ManagerStatus" headertext="Manager Status"
                                itemstyle-width="15%" />
                            <asp:templatefield>
                                <itemstyle width="5%" horizontalalign="Center" />
                                <itemtemplate>
                                    <asp:linkbutton runat="server" id="btnRemove"
                                        commandargument='<%# Eval("MRNumber") %>'
                                        commandname="view"
                                        text="<i class='fa fa-desktop fa-fw'></i>"
                                        cssclass="btn bg-maroon margin" />
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:templatefield>
                                <itemstyle width="5%" horizontalalign="Center" />
                                <itemtemplate>
                                    <asp:linkbutton runat="server" id="btnPrint"
                                        commandargument='<%# Eval("MRNumber") %>'
                                        commandname="download"
                                        text="<i class='fa fa-download fa-fw'></i>"
                                        cssclass="btn bg-olive margin" />
                                </itemtemplate>
                            </asp:templatefield>
                        </columns>
                    </asp:gridview>
                </div>
            </div>
        </asp:panel>
    </section>
</asp:content>
<asp:content id="Content3" contentplaceholderid="footer" runat="server">
</asp:content>
