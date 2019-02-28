<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EstimateQuotation.aspx.cs" Inherits="KentWebApplication.EstimateQuotation" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/chosen.css" rel="stylesheet" />
    <link rel="stylesheet" href="css/jquery-ui.css">
    <style>
        .completionList {
            z-index: 99999;
        }

        .ui-autocomplete {
            max-height: 150px;
            overflow-y: auto;
            /* prevent horizontal scrollbar */
            overflow-x: hidden;
        }
        /* IE 6 doesn't support max-height
       * we use height instead, but this forces the menu to always be this tall
       */
        * html .ui-autocomplete {
            height: 150px;
        }

        .modal {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }

        .loading {
            font-family: Arial;
            font-size: 10pt;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="content-header">
        <div class="alert alert-danger alert-dismissable" runat="server" id="error_alert" visible="false">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
            <b>Function Failed!</b>
            <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
        </div>
        <div class="alert alert-success alert-dismissable" runat="server" id="success_alert" visible="false">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
            <b>Success Notification!</b>
            <asp:Literal ID="litSuccessMessage" runat="server"></asp:Literal>
            <button type="button" class="btn btn-default" data-dismiss="alert" aria-hidden="true">OK</button>
        </div>
        <div id="specialMessages">
            <div class="alert alert-danger alert-dismissable" runat="server" id="duplicate_alert" visible="false">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <b>Function Failed! </b>
                <asp:Literal ID="litMessage" runat="server" Text="Item Already Exists"></asp:Literal>
                <asp:Button ID="btnMsg" runat="server" Text="Edit" OnClick="btnMsg_OnClick" CssClass="btn btn-default btn-sm" />
                <asp:HiddenField ID="hfEditingItemNo" runat="server" />
            </div>
        </div>
    </section>
    <!-- Main Content-->
    <section class="content invoice">
        <div class="row">
            <div class="col-xs-10">
                <h2 class="page-header">
                    <i class="fa fa-globe"></i>
                    Sub Estimate - 
                    <asp:Literal ID="litProjectName" runat="server"></asp:Literal>
                </h2>
            </div>
            <div class="col-xs-2">
                <h2 class="page-header">Sub Estimate No - 
                    <asp:Literal ID="litSubEstimateID" runat="server"></asp:Literal>
                </h2>
            </div>
        </div>
        <div class="row invoice-info">
            <div class="col-sm-2 invoice-col">
                <b>
                    <label for="lblEngineer">Engineer : </label>
                    <asp:TextBox ID="txtEngineer" runat="server"
                        placeholder="Engineer" onkeyup="txtEngineerValidation();"
                        CssClass="form-control input-sm" Enabled="false"></asp:TextBox>
                    <label id="lblEngineer" class="control-label" for="inputWarning"></label>
                </b>
                <b>
                    <label for="lblManager">Manager : </label>
                    <asp:TextBox ID="txtManager" name="txtManager" runat="server"
                        placeholder="Manager" onkeyup="txtManagerValidation();"
                        CssClass="form-control input-sm validate[required]" Enabled="false"></asp:TextBox>
                    <label id="lblManager" class="control-label" for="inputWarning"></label>
                </b>
                <b>
                    <label for="lblDeliverLocation">Deliver Location : </label>
                    <asp:TextBox ID="txtDeliverLocation" runat="server"
                        placeholder="Deliver Location" CssClass="form-control input-sm validate[required]"
                        Enabled="true"></asp:TextBox>
                    <label id="lblDeliverLocation" class="control-label" for="inputWarning"></label>
                </b>
            </div>
            <div class="col-sm-2 invoice-col">
                <b style="display: none">
                    <label for="lblSite">Estimation Number : </label>
                    <asp:TextBox ID="txtEstimationId" name="txtManager"
                        runat="server" placeholder="Estimation ID" Enabled="false"
                        onkeyup="txtEstimationValidation();"
                        CssClass="form-control input-sm validate[required]" MaxLength="10"></asp:TextBox>
                    <label id="lblEstimation" class="control-label" for="inputWarning"></label>
                </b>
                <b>
                    <label for="lblDate">Required Date : </label>
                    <asp:TextBox ID="dtApplyDate" runat="server" placeholder="Required Date"
                        change="dtApplyDateValidation();"
                        onkeyup="dtApplyDateValidation();"
                        CssClass="form-control input-sm datepicker">
                    </asp:TextBox>
                    <i class="fa fa-calendar"></i>
                    <label id="lblapplydate" class="control-label" for="inputWarning"></label>
                </b>
                <b style="display: none;">
                    <label for="txtReceivedDate">Received Date : </label>
                    <asp:TextBox ID="dtReceivedDate" runat="server"
                        placeholder="Received Date"
                        change="dtApplyDateValidation();"
                        onkeyup="dtApplyDateValidation();"
                        CssClass="form-control input-sm validate[required] datepicker"></asp:TextBox>
                    <label id="lblReceivedDate" class="control-label" for="inputWarning"></label>
                </b>
                <b>
                    <label for="txtRemarks">Remarks : </label>
                    <asp:TextBox ID="txtComments" runat="server" placeholder="Remarks"
                        CssClass="form-control input-sm validate[required]" TextMode="MultiLine">
                    </asp:TextBox>
                    <label id="lblRemarks" class="control-label" for="inputWarning"></label>
                </b>
            </div>
            <div class="col-sm-5 invoice-col">
                <p class="lead">Item Request Details</p>
                <div>
                    <div class="form-group">
                        <label class="col-sm-6 " for="lblApproveQty">Estimated Quantity : </label>
                        <div class="col-sm-6">
                            <asp:Label runat="server" ID="lblApproveQty"
                                CssClass="form-control input-sm" ReadOnly="true"></asp:Label>
                        </div>
                    </div>
                </div>
                <br />
                <div>
                    <div class="form-group">
                        <label class="col-sm-6">Issued Quantity : </label>
                        <div class="col-sm-6">
                            <asp:Label runat="server" ID="litIssuedQty"
                                CssClass="form-control input-sm" ReadOnly="true"></asp:Label>
                        </div>
                    </div>
                </div>
                <br />
                <div>
                    <div class="form-group">
                        <label class="col-sm-6">pending Quantity : </label>
                        <div class="col-sm-6">
                            <asp:Label runat="server" ID="litPendingQty"
                                CssClass="form-control input-sm" ReadOnly="true"></asp:Label>
                        </div>
                    </div>
                </div>
                <br />
                <div>
                    <div class="form-group">
                        <label class="col-sm-6">Balance Quantity : </label>
                        <div class="col-sm-6">
                            <asp:Label runat="server" ID="litBalanceQty"
                                CssClass="form-control input-sm" ReadOnly="true"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-3 invoice-col">
                <p class="lead">Item Count</p>
                <div class="table-responsive">
                    <table class="table">
                        <tbody>
                            <tr>
                                <th style="width: 50%">
                                    <label for="lbltotItmLabel">Total Item Count : </label>
                                </th>
                                <td>
                                    <asp:Label runat="server" ID="txtTotalItem"></asp:Label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <b>
                    <asp:Button ID="btnSubmitForApproval" runat="server" Text="Submit for approval"
                        CssClass="btn btn-lg bg-maroon-gradient" OnClick="btnSubmitForApproval_Click"
                        OnClientClick="return confirm('Are you sure you want to confirm the Material Request. This cannot be undone?')" />
                </b>
            </div>
        </div>
        <div class="row" style="padding-top: 10px;">
            <div class="box box-solid box-info collapsed-box">
                <div class="box-header with-border">
                    <h3 class="box-title">Click on the + mark to search items</h3>
                    <div class="box-tools pull-right">
                        <button class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-plus"></i></button>
                    </div>
                    <!-- /.box-tools -->
                </div>
                <!-- /.box-header -->
                <div class="box-body">
                    <div class="row" >
                        <div class="form-group">
                            <div class="form-group col-sm-6">
                                <asp:TextBox ID="txtSearchQuery" runat="server" class="form-control"
                                    placeholder="Enter Item name to start search">
                                </asp:TextBox>
                            </div>

                            <div class="col-sm-3">
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary pull-left"
                                    OnClick="btnSearch_OnClick" Text="Search" />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- /.box-body -->
            </div>
            <!-- /.box -->
           <div class="filter-summary">
               <asp:Panel id="pnlFilterSummary" runat="server" Visible="false">
                   <asp:Label id="lblFilterSummary" runat="server"></asp:Label>
                   <asp:LinkButton ID="btnClearSearch" runat="server" CssClass="btn bg-aqua pull-left"
                                   OnClick="btnClearSearch_OnClick" Text="Clear" />
               </asp:Panel>
           </div> 
        </div>
        <div class="row">
            <p class="text-aqua">
                <i class="fa fa-fw fa-info"></i>Please type the Item name on the "Item Name" area and select the appropriate Product from the list.
            </p>
            <div class="form-group">
                <div class="form-group col-sm-1">
                    <asp:TextBox ID="txtItemId" runat="server" class="form-control"
                        placeholder="Item Id">
                    </asp:TextBox>
                </div>
                <div class="form-group col-sm-4">
                    <asp:TextBox ID="txtItemName" runat="server" class="form-control" placeholder="Item Name" OnTextChanged="txtItemName_TextChanged">
                    </asp:TextBox>
                </div>
                <div class="form-group col-sm-1">
                    <asp:TextBox ID="txtUnitofMeasure" runat="server" class="form-control"
                        placeholder="Unit of Measure">
                    </asp:TextBox>
                </div>
                <div class="form-group col-sm-1 hidden">
                    <asp:TextBox ID="txtRequestQty" runat="server" class="form-control"
                        placeholder="Request Qty" OnTextChanged="txtRequestQty_TextChanged">
                    </asp:TextBox>
                </div>
                <div class="form-group col-sm-1">
                    <asp:TextBox ID="txtQty" runat="server" class="form-control"
                        placeholder="Qty" OnTextChanged="txtQty_TextChanged1"></asp:TextBox>
                </div>
                <div class="form-group col-sm-2">
                    <asp:TextBox ID="txtRemarks" runat="server" class="form-control" ReadOnly="true" 
                        placeholder="Remarks"></asp:TextBox>
                </div>
                <div class="col-sm-1">
                    <asp:HiddenField ID="hfApprovedQty" runat="server" />
                    <asp:HiddenField ID="hfIssuedQty" runat="server" />
                    <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary pull-left"
                        OnClick="btnAdd_Click" Text="Add" />
                </div>
            </div>
            <div class="col-xs-12 table-responsive">
                <div id="example1_wrapper" class="dataTables_wrapper form-inline" role="grid">
                    <asp:GridView ID="grdItems" runat="server"
                        CssClass="Grid"
                        EnableModelValidation="True"
                        AutoGenerateColumns="false"
                        AllowPaging="true"
                        AllowSorting="false"
                        EmptyDataText="No records Found"
                        OnRowCommand="grdItems_RowCommand"
                        PageSize="10"
                        OnPageIndexChanging="grdItems_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="OrderNo" HeaderText="ID"
                                ItemStyle-Width="5%" />
                            <asp:BoundField DataField="ItemCode" HeaderText="Item Id"
                                ItemStyle-Width="10%"
                                ItemStyle-HorizontalAlign="Center"
                                ItemStyle-VerticalAlign="Middle" />
                            <asp:BoundField DataField="ItemName" HeaderText="Item Name"
                                ItemStyle-Width="40%" />
                            <asp:BoundField DataField="MainMeasure" HeaderText="Unit of Measure"
                                ItemStyle-Width="10%"
                                ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Qty" HeaderText="Quantity"
                                ItemStyle-Width="10%"
                                ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Remarks" HeaderText="Remarks"
                                ItemStyle-Width="15%" ReadOnly="true" />
                            <asp:TemplateField>
                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="btnRemove"
                                        OnClientClick="if (!confirm('Are you sure you want delete this item?')) return false;"
                                        CommandArgument='<%# Eval("ItemCode") %>'
                                        CommandName="Remove"
                                        Text="<i class='fa fa-trash-o'></i>"
                                        CssClass="btn btn-default btn-danger" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle Width="5%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="btnEdit"
                                        CommandArgument='<%# Eval("ItemCode") %>'
                                        CommandName="Change"
                                        Text="<i class='fa fa-pencil fa-fw'></i>"
                                        CssClass="btn btn-default btn-success" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div class="row no-print">
            <div class="col-xs-12">

                <asp:Button ID="btnSave" runat="server" Text="Save"
                    CssClass="btn bg-green-gradient" OnClick="btnSave_Click" OnClientClick="return ValidateSubmit();" />
            </div>
        </div>
        <div>
            <asp:HiddenField ID="hfEstimationId" runat="server" />
        </div>
    </section>
    <div class="loading" align="center">
        Processing. Please wait...<br />
        <br />
        <img src="img/loading.gif" alt="" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
    <script src="Validation/Page/Estimation.js"></script>
    <script type="text/javascript">

        function ShowProgress() {
            setTimeout(function () {
                var modal = $('.loading');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                loading.css("display", "block");
            }, 200);
        }

        $('form').on("submit", function () {
            ShowProgress();
        });

        $(document).ready(function () {
            $("#<%= txtItemName.ClientID %>").focus();
        });

        $("#<%= txtItemName.ClientID %>").on('keyup keypress', function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode === 13) {
                e.preventDefault();
                return false;
            }
        });


        $('#<%= txtItemName.ClientID %>').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/EstimateQuotation.aspx/GetEstimationDetails")%>',
                    data: "{ 'prefix': '" + escape(request.term) + "', 'estimationId' : 1 }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('~')[1],
                                val: item.split('~')[0],
                                unit: item.split('~')[2],
                                qty: item.split('~')[3],
                                final: item.split('~')[4],
                                issued: item.split('~')[5],
                                req: item.split('~')[6]
                            }
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                $("#<%=txtItemId.ClientID %>").val(i.item.val);
                $("#<%=txtUnitofMeasure.ClientID %>").val(i.item.unit);
                $("#<%=txtRequestQty.ClientID %>").val(i.item.rqt);
                $("#<%=hfApprovedQty.ClientID %>").val(i.item.final);
                $("#<%=hfIssuedQty.ClientID %>").val(i.item.issued);

                $("#<%=txtQty.ClientID %>").focus();
                
                $("#<%=lblApproveQty.ClientID %>").text(i.item.final);
                $("#<%=litIssuedQty.ClientID %>").text(i.item.issued);
                if (parseFloat(i.item.req) < parseFloat(i.item.issued)) {
                    $("#<%=litPendingQty.ClientID %>").text(0);
                    $("#<%=litBalanceQty.ClientID %>").text(parseFloat(i.item.final) - parseFloat(i.item.issued));
                } else {
                    $("#<%=litPendingQty.ClientID %>").text(parseFloat(i.item.req) - parseFloat(i.item.issued));
                    $("#<%=litBalanceQty.ClientID %>").text(parseFloat(i.item.final) - parseFloat(i.item.req));
                }
            },
            minLength: 3
        });

        $("#<%=txtQty.ClientID %>").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == 13) {
                $("#<%=btnAdd.ClientID %>").focus();
                return false;
            }
        });

        $("#<%=txtRemarks.ClientID %>").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == 13) {
                return false;
            }
        });


        $(document).ready(function () {

            $('#<%= btnAdd.ClientID %>').click(function () {
                var quantity = $('#<%= txtQty.ClientID %>').val();
                if (quantity == "") {
                    alert("Please enter a valid quantity. Quantity should not be empty.");
                    return false;
                }
                else if (quantity == 0) {
                    alert("Please enter a valid quantity. Quantity should not be zero.");
                    return false;
                } else {
                    return true;
                }
            });

        });

        $("#<%= txtItemId.ClientID %>").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == 13) {

                var itemCode = $('#<%= txtItemId.ClientID %>').val();

                var label;
                var val;
                var unit;
                var rqt;

                $.ajax({
                    url: '<%=ResolveUrl("~/EstimateQuotation.aspx/GetEstimationItemByCode")%>',
                    data: "{ 'itemCode': '" + itemCode + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        console.log(data);

                        var str = data.d;
                        var res = str.split("~");

                        $("#<%=txtItemName.ClientID %>").val(res[1]);
                        $("#<%=txtUnitofMeasure.ClientID %>").val(res[2]);
                        $("#<%=txtRequestQty.ClientID %>").val(res[3]);

                        $("#<%=txtQty.ClientID %>").focus();


                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });

                $("#<%=btnAdd.ClientID %>").focus();
                return false;
            }
        });

    </script>

</asp:Content>
