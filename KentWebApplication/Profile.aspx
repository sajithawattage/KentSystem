<%@ page title="" language="C#" masterpagefile="~/Site.Master" autoeventwireup="true" codebehind="Profile.aspx.cs" inherits="KentWebApplication.Profile.Profile" %>

<asp:content id="Content1" contentplaceholderid="head" runat="server">
</asp:content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
    <!-- Main content -->
    <section class="content">
        <asp:panel id="pnlEmployeeDetails" runat="server">
            <div class="form-group" runat="server" id="dvErrorMessages">
                <div class="alert alert-danger alert-dismissable">
                    <i class="fa fa-ban"></i>
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                    <b>Error</b>
                    <asp:literal id="litErrorMessage" runat="server"></asp:literal>
                </div>
            </div>
            <div class="form-group" runat="server" id="dvSuccessMessages">
                <div class="alert alert-success alert-dismissable">
                    <i class="fa fa-ban"></i>
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                    <b>Success</b>
                    <asp:literal id="litSuccessMessage" runat="server"></asp:literal>
                </div>
            </div>
            <div class="col-md-6">
                <div class="box box-default">
                    <div class="box-header with-border">
                        <div class="box-title">My Profile</div> 
                        <div class="pull-right">
                            
                        </div>
                    </div>
                    <div class="box-body">
                        <div class="form-group">
                            <label for="">Name</label>
                            <asp:textbox id="txtEngineerName" runat="server" cssclass="form-control"></asp:textbox>
                        </div>
                        <div class="form-group">
                            <label for="">Telephone</label>
                            <asp:textbox id="txtTelephone" runat="server" cssclass="form-control"></asp:textbox>
                        </div>
                        <div class="form-group">
                            <label for="">Email Address</label>
                            <asp:textbox id="txtEmailAddress" runat="server" cssclass="form-control"></asp:textbox>
                        </div>
                    </div>
                    <div class="box box-footer">
                        <asp:button id="btnUpdateDetails" runat="server" text="Update Profile Details"
                            cssclass="btn btn-success" onclick="btnUpdateDetails_Click" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="box box-default">
                    <div class="box-header with-border">
                        <div class="box-title">Reset your password</div>
                    </div>
                    <div class="box-body">
                        <p class="text-aqua">
                            Please choose a new password to reset your current password, and confirm it on the "confirm password"
                            area. A good password should contain 8 or more characters in length English upper case (A-Z),English lower case (a-z),
                            Numeric (0-9) and some special characters without spaces
                        </p>
                        <div class="form-group">
                            <label for="">New Password</label>
                            <asp:textbox id="txtNewPassword" runat="server" cssclass="form-control"
                                textmode="Password"></asp:textbox>
                        </div>
                        <div class="form-group">
                            <label for="">Confirm Password</label>
                            <asp:textbox id="txtConfrimPassword" runat="server" cssclass="form-control"
                                textmode="Password"></asp:textbox>
                        </div>
                    </div>
                    <div class="box box-footer">
                        <asp:button id="btnUpdatePassword" runat="server" text="Update Password"
                            cssclass="btn btn-info" onclick="btnUpdatePassword_Click" />
                    </div>
                </div>
            </div>
        </asp:panel>
    </section>
</asp:content>
<asp:content id="Content3" contentplaceholderid="footer" runat="server">
</asp:content>
