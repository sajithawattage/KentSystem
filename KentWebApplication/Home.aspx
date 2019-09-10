<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="KentWebApplication.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <!-- Main content -->
    <section class="content">

        <div class="row">
            <div class="col-md-12">
                <div class="box box-solid">
                    <div class="box-body">
                        <h4>Welcome
                            <asp:Literal ID="litUserName" runat="server"></asp:Literal></h4>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlDetails" runat="server">
            <asp:Repeater ID="rpSites" runat="server" OnItemDataBound="rpSites_ItemDataBound">
                <ItemTemplate>
                    <div class="col-lg-4">
                        <div class="box box-success" style="min-height: 200px;">
                            <div class="box-header">
                                <h3 class="box-title">
                                    <asp:Literal ID="litSiteTitle" runat="server" Text='<%# Eval("JobName") %>'></asp:Literal>
                                </h3>
                            </div>
                            <div class="box-body">
                                <p>
                                    <asp:Literal ID="litCustomerName" runat="server" Text='<%# Eval("CustomerName") %>'></asp:Literal>
                                </p>
                                <p>
                                    <asp:Literal ID="litDescription" runat="server" Text='<%# Eval("Place") %>'></asp:Literal>
                                </p>
                                <asp:HiddenField ID="hfJId" runat="server" Value='<%# Eval("JobCode") %>' />
                                <asp:HiddenField ID="hfEId" runat="server" Value='<%# Eval("EngineerNo") %>' />
                                <asp:HiddenField ID="hfMId" runat="server" Value='<%# Eval("ManagerNo") %>' />
                                <asp:HiddenField ID="hfCId" runat="server" Value='<%# Eval("CustomerCode") %>' />
                                <asp:HiddenField ID="hfType" runat="server" Value='<%# Eval("EstimateStatus") %>' />
                                <asp:HiddenField ID="hfMStatus" runat="server" Value='<%# Eval("ManagerStatus") %>' />
                                <asp:HiddenField ID="hfEStatus" runat="server" Value='<%# Eval("EngineerStatus") %>' />
                                <asp:HiddenField ID="hfESMStatus" runat="server" Value='<%# Eval("ManagerESStatus") %>' />
                                <asp:HiddenField ID="hfESEStatus" runat="server" Value='<%# Eval("EngineerESStatus") %>' />
                                <asp:HiddenField ID="hfMra" runat="server" Value='<%# Eval("MRActive") %>' />
                                <ul style="list-style-type: none;">
                                    <li>
                                        <asp:HyperLink ID="hlEstimation" runat="server"></asp:HyperLink></li>
                                    <li>
                                        <div>
                                            <asp:HyperLink ID="hlQuotaionEstimate" runat="server" Text="Sub Estimate" CssClass="btn bg-olive margin"></asp:HyperLink>
                                            <asp:HyperLink ID="hlQuotaionEstimateList" runat="server" Text="Sub Estimate List" CssClass="btn bg-orange margin"></asp:HyperLink>
                                        </div>
                                    </li>
                                </ul>
                            </div>
                            <div class="box-footer">
                                <asp:HyperLink ID="hlMaterialRequest" runat="server" Text="New Material Request" CssClass="btn bg-olive margin"></asp:HyperLink>
                                <asp:HyperLink ID="hlMaterialRequestList" runat="server" Text="Material Request List" CssClass="btn bg-purple margin"></asp:HyperLink>
                            </div>
                        </div>
                    </div>

                </ItemTemplate>

            </asp:Repeater>
        </asp:Panel>
    </section>

</asp:Content>
