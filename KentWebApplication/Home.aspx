<%@ page title="" language="C#" masterpagefile="~/Site.Master" autoeventwireup="true" codebehind="Home.aspx.cs" inherits="KentWebApplication.Home" %>

<asp:content id="Content1" contentplaceholderid="head" runat="Server">
</asp:content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="Server">

    <!-- Main content -->
    <section class="content">

        <div class="row">
            <div class="col-md-12">
                <div class="box box-solid">
                    <div class="box-body">
                        <h4>Welcome <asp:literal id="litUserName" runat="server"></asp:literal></h4>
                    </div>
                </div>
            </div>
        </div>

        <asp:panel id="pnlDetails" runat="server">
            <asp:repeater id="rpSites" runat="server" onitemdatabound="rpSites_ItemDataBound" OnItemCommand="rpSites_ItemCommand">
                <itemtemplate>
                    <div class="col-lg-4">
                        <div class="box box-success" style="min-height:200px;">
                            <div class="box-header">
                                <h3 class="box-title">
                                    <asp:literal id="litSiteTitle" runat="server" text='<%# Eval("JobName") %>'></asp:literal>
                                </h3>
                            </div>
                            <div class="box-body">
                                <p>
                                    <asp:literal id="litCustomerName" runat="server" text='<%# Eval("CustomerName") %>'></asp:literal>
                                </p>
                                <p>
                                    <asp:literal id="litDescription" runat="server" text='<%# Eval("Place") %>'></asp:literal>
                                </p>
                                <asp:hiddenfield id="hfJId" runat="server" value='<%# Eval("JobCode") %>' />
                                <asp:hiddenfield id="hfEId" runat="server" value='<%# Eval("EngineerNo") %>' />
                                <asp:hiddenfield id="hfMId" runat="server" value='<%# Eval("ManagerNo") %>' />
                                <asp:hiddenfield id="hfCId" runat="server" value='<%# Eval("CustomerCode") %>' />
                                <asp:hiddenfield id="hfType" runat="server" value='<%# Eval("EstimateStatus") %>' />
                                <asp:hiddenfield id="hfMStatus" runat="server" value='<%# Eval("ManagerStatus") %>' />
                                <asp:hiddenfield id="hfEStatus" runat="server" value='<%# Eval("EngineerStatus") %>' />
                                <asp:hiddenfield id="hfESMStatus" runat="server" value='<%# Eval("ManagerESStatus") %>' />
                                <asp:hiddenfield id="hfESEStatus" runat="server" value='<%# Eval("EngineerESStatus") %>' />
                                <asp:hiddenfield id="hfMra" runat="server" value='<%# Eval("MRActive") %>' />
                                <ul style="list-style-type: none;">
                                    <li>
                                        <asp:hyperlink id="hlEstimation" runat="server"></asp:hyperlink></li>
                                    <li>
                                        <div>
                                             <asp:hyperlink id="hlQuotaionEstimate" runat="server" text="Sub Estimate" cssclass="btn bg-olive margin" ></asp:hyperlink>
                                            <asp:hyperlink id="hlQuotaionEstimateList" runat="server" text="Sub Estimate List" cssclass="btn bg-orange margin" ></asp:hyperlink>
                                        </div>
                                    </li>
                                </ul>
                            </div>
                            <div class="box-footer">
                                <asp:hyperlink id="hlMaterialRequest" runat="server" text="New Material Request" cssclass="btn bg-olive margin" ></asp:hyperlink>
                                <asp:hyperlink id="hlMaterialRequestList" runat="server" text="Material Request List" cssclass="btn bg-purple margin"></asp:hyperlink>
                            </div>
                        </div>
                    </div>

                </itemtemplate>

            </asp:repeater>
        </asp:panel>
    </section>

</asp:content>
