<%@ page title="" language="C#" masterpagefile="~/Site.Master" autoeventwireup="true" codebehind="Estimation.aspx.cs" inherits="KentWebApplication.Pages.Estimation" %>

<asp:Content id="Content1" contentplaceholderid="head" runat="server">
    <link rel="stylesheet" href="css/jquery-ui.css">
    <style>
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
    </style>
</asp:Content>

<asp:Content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
    <section class="content-header">
        <div class="alert alert-danger alert-dismissable" runat="server" id="error_alert" visible="false">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
            <b>Alert!</b>
            <asp:Literal id="litErrorMessage" runat="server"></asp:Literal>
        </div>
        <div class="alert alert-success alert-dismissable" runat="server" id="success_alert" visible="false">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
            <b>Alert!</b>
            <asp:Literal id="litSuccessMessage" runat="server"></asp:Literal>
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
            <div class="col-xs-12">
                <h2 class="page-header">
                    <i class="fa fa-globe"></i>
                    Material Estimation
                    <asp:Literal id="litProjectName" runat="server"></asp:Literal>
                </h2>
            </div>
        </div>
        <div class="row invoice-info">
            <div class="col-sm-4 invoice-col">
                <b style="display:none;">
                    <label for="lblEngineer">Engineer : </label>
                    <asp:TextBox id="txtEngineer" runat="server"
                        placeholder="Engineer" onkeyup="txtEngineerValidation();"
                        cssclass="form-control input-sm" enabled="false"></asp:TextBox>
                    <label id="lblEngineer" class="control-label" for="inputWarning"></label>
                </b>
                <b>
                    <label for="lblManager">Manager : </label>
                    <asp:TextBox id="txtManager" name="txtManager" runat="server"
                        placeholder="Manager" onkeyup="txtManagerValidation();"
                        cssclass="form-control input-sm validate[required]" enabled="false"></asp:TextBox>
                    <label id="lblManager" class="control-label" for="inputWarning"></label>
                </b>
            </div>
            <div class="col-sm-4 invoice-col">
                <b style="display:none;">
                    <label for="lblSite">Estimation Number : </label>
                    <asp:TextBox id="txtEstimationId" name="txtManager"
                        runat="server" placeholder="Estimation ID" enabled="false"
                        onkeyup="txtEstimationValidation();"
                        cssclass="form-control input-sm validate[required]" maxlength="10"></asp:TextBox>
                    <label id="lblEstimation" class="control-label" for="inputWarning"></label>
                </b>
                <b>
                    <label for="lblDate">Date : </label>
                    <asp:TextBox id="dtApplyDate" runat="server" placeholder="Apply Date"
                        change="dtApplyDateValidation();"
                        onkeyup="dtApplyDateValidation();"
                        cssclass="form-control input-sm datepicker">
                    </asp:TextBox>
                    <i class="fa fa-calendar"></i>
                    <label id="lblapplydate" class="control-label" for="inputWarning"></label>
                </b>
            </div>
            <div class="col-sm-4 invoice-col">
                <p class="lead">Item Count</p>
                <div class="table-responsive">
                    <table class="table">
                        <tbody>
                            <tr>
                                <th style="width: 50%">
                                    <label for="lbltotItmLabel">Total Item Count : </label>
                                </th>
                                <td>
                                    <asp:Label runat="server" id="txtTotalItem"></asp:Label></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
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
                    <div class="row">

                        <div class="form-group">
                            <div class="form-group col-sm-6">
                                <asp:TextBox ID="txtSearchQuery" runat="server" class="form-control"
                                    placeholder="Enter Item name to start search">
                                </asp:TextBox>
                            </div>

                            <div class="col-sm-3">
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn bg-aqua pull-left"
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
                <div class="form-group col-xs-2">
                    <asp:TextBox id="txtItemId" runat="server" class="form-control" placeholder="Item Id">
                    </asp:TextBox>
                </div>
                <div class="form-group col-xs-4">
                    <asp:TextBox id="txtItemName" runat="server" class="form-control" placeholder="Item Name">
                    </asp:TextBox>
                </div>
                <div class="form-group col-xs-2">
                    <asp:TextBox id="txtUnitofMeasure" runat="server" class="form-control" placeholder="Unit of Measure">
                    </asp:TextBox>
                </div>
                <div class="form-group col-xs-1">
                    <asp:TextBox id="txtQty" runat="server" class="form-control" placeholder="Qty"
                        onkeyup="Total();" OnTextChanged="txtQty_TextChanged1"></asp:TextBox>
                </div>
                <div class="form-group col-sm-2">
                    <asp:TextBox id="txtRemarks" runat="server" class="form-control"  ReadOnly="true"
                        placeholder="Remarks"></asp:TextBox>
                </div>
                <div class="col-xs-1">
                    <asp:LinkButton id="btnAdd" runat="server" cssclass="btn btn-primary pull-left"
                        onclick="btnAddItem_Click" text="Add" />
                </div>
            </div>
            <div class="col-xs-12 table-responsive">
                <div id="example1_wrapper" class="dataTables_wrapper form-inline" role="grid">
                    <asp:GridView id="grdItems" runat="server"
                        cssclass="Grid"
                        enablemodelvalidation="true"
                        autogeneratecolumns="false"
                        allowpaging="true"
                        allowsorting="false"
                        emptydatatext="No records Found" 
                        onrowcommand="grdClosedExams_RowCommand"
                        onpageindexchanging="grdItems_PageIndexChanging"
                        ondatabound="grdItems_DataBound"
                        pagesize="10" OnSelectedIndexChanged="grdItems_SelectedIndexChanged" >
                        <columns>
                            <asp:BoundField datafield="ItemIndex" headertext="ID"
                                itemstyle-Width="5%" />
                            <asp:BoundField datafield="ItemCode" headertext="Item Id"
                                itemstyle-Width="15%"
                                itemstyle-HorizontalAlign="Center"
                                itemstyle-VerticalAlign="Middle" />
                            <asp:BoundField datafield="ItemName" headertext="Item Name"
                                itemstyle-Width="48%" />
                            <asp:BoundField datafield="MainMeasure" headertext="Unit of Measure"
                                itemstyle-Width="10%" 
                                itemstyle-HorizontalAlign="Center"
                                headerstyle-HorizontalAlign="Center" />
                            <asp:BoundField datafield="Qty" headertext="Engineer Quantity"
                                itemstyle-Width="15%" />
                            <asp:BoundField datafield="RequestedQty"
                                itemstyle-Width="15%" Visible="false"  />
                            <asp:BoundField datafield="Amount" headertext="Amount"
                                itemstyle-Width="1%" visible="false" />
                            <asp:BoundField datafield="Total" headertext="Total"
                                itemstyle-Width="1%" visible="false" />
                             <asp:BoundField datafield="Remarks" headertext="Remarks"
                                itemstyle-Width="15%" ReadOnly="true" />
                            <asp:TemplateField>
                                <itemstyle width="5%" horizontalalign="Center" /> 
                                <itemtemplate>
                                    <asp:LinkButton runat="server" id="btnRemove" 
                                        commandargument=<%# Eval("ItemCode") %>
                                        commandname="Remove" 
                                        text="<i class='fa fa-trash-o'></i>"
                                        cssclass="btn btn-default btn-danger"
                                        onclientclick="return confirm('Do you really want to remove this item?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <itemstyle width="5%" horizontalalign="Center" /> 
                                <itemtemplate>
                                    <asp:LinkButton runat="server" id="btnEdit" 
                                        commandargument=<%# Eval("ItemCode") %>
                                        commandname="Change" 
                                        text="<i class='fa fa-pencil fa-fw'></i>"
                                        cssclass="btn btn-default btn-success" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div class="row no-print">
            <div class="col-xs-12">
                <asp:Button id="btnSubmitForApproval" runat="server" text="Submit for approval"
                    cssclass="btn btn-success btn-sm" 
                    onclick="btnSubmitforApproval_Click" 
                    OnClientClick="return confirm('Are you sure you want to confirm the Estimation. This cannot be undone?')" />
                <asp:Button id="btnSave" runat="server" text="Save"
                    cssclass="btn btn-success btn-sm" onclick="btnSave_Click" onclientclick="return ValidateSubmit();" />
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content id="Content3" contentplaceholderid="footer" runat="server">
    <script src="Validation/Page/Estimation.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#<%= txtItemName.ClientID %>").focus();
        });

        $(document).ready(function () {

            var cache = {};

            $('#<%= txtItemName.ClientID %>').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '<%=ResolveUrl("~/Estimation.aspx/GetAllItems")%>',
                        data: "{ 'prefix': '" + escape(request.term) + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.split('~')[1],
                                    val: item.split('~')[0],
                                    unit: item.split('~')[2]
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
                    $("#<%=txtQty.ClientID %>").focus();
                },
                minLength: 3
            });

            $("#<%=txtQty.ClientID %>").keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == 13) {
                    return false;
                }
            });

            ///handle keypress events
            $('#<%=txtItemId.ClientID%>').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == 13) {
                    $("#<%=txtQty.ClientID %>").focus();
                    return false;
                }
            });

            $('#<%=txtItemName.ClientID%>').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == 13) {
                    $("#<%=txtQty.ClientID %>").focus();
                    return false;
                }
            });

            $('#<%=txtQty.ClientID%>').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == 13) {
                    $("#<%=btnAdd.ClientID %>").focus();
                    return false;
                }
            });

            $('#<%=txtRemarks.ClientID%>').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode == 13) {
                    return false;
                }
            });

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

        function txtManagerValidation() {
            return this.Validation('<%= txtManager.ClientID %>', "Cannot be empty Manager", 'lblmanager', 'div_Managr');
        }
        function txtEstimationValidation() {
            return this.Validation('<%= txtEstimationId.ClientID %>', "Cannot be empty EstimationId", 'lblEstimation', 'div_Estimation');
        }
        function txtEngineerValidation() {
            return this.Validation('<%= txtEngineer.ClientID %>', "Cannot be empty Engineer", 'lblEngineer', 'div_Engineer');
        }
        function dtApplyDateValidation() {
            return this.Validation('<%= dtApplyDate.ClientID %>', "Cannot be empty Date", 'lblapplydate', 'div_Date');
        }
        function ValidateSubmit() {
            txtEngineerValidation();
            txtManagerValidation();
            txtEstimationValidation();
            dtApplyDateValidation();
            if (!txtManagerValidation()) {
                return false;
            }
            if (!txtEstimationValidation()) {
                return false;
            }
            if (!txtEngineerValidation()) {
                return false;
            }
            if (!dtApplyDateValidation()) {
                return false;
            }
        }


    </script>
</asp:Content>
