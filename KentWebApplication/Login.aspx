<%@ page language="C#" autoeventwireup="true" codebehind="Login.aspx.cs" inherits="KentWebApplication.Login" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="UTF-8">
    <title>Kent Engineering | Log in</title>
    <meta content='width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no' name='viewport'>
    <!-- bootstrap 3.0.2 -->
    <link href="css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <!-- font Awesome -->
    <link href="css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <!-- Theme style -->
    <link href="css/AdminLTE.css" rel="stylesheet" type="text/css" />

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.3.0/respond.min.js"></script>
    <![endif]-->
</head>
<body class="login-page">
    <form id="form1" runat="server">

        <div class="login-box">

            <div class="login-logo">
                <img class="center-block" alt="" src="img/logo.png" />
            </div>
            <!-- /.login-logo -->

            <div class="login-box-body">
                <p class="login-box-msg">Sign in to start your session</p>
                <div class="form-group" runat="server" id="dvErrorMessages">
                    <div class="alert alert-danger alert-dismissable">
                        <i class="fa fa-ban"></i>
                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                        <b>Error</b>
                        <asp:literal id="litErrorMessage" runat="server"></asp:literal>
                    </div>
                </div>
                <div class="form-group has-feedback">
                    <asp:textbox id="txtUserName" runat="server" cssclass="form-control" placeholder="User ID"></asp:textbox>
                    <label id="user_name" class="control-label" for="inputWarning"></label>
                </div>
                <div class="form-group has-feedback">
                    <asp:textbox id="txtPassword" runat="server" class="form-control" placeholder="Password" textmode="Password"></asp:textbox>
                    <label id="password" class="control-label" for="inputWarning"></label>
                </div>
                <div class="row">
                    <div class="col-xs-8">
                        <div class="checkbox icheck">
                            <label>
                                <input type="checkbox">
                                Remember Me
                            </label>
                        </div>
                    </div>
                    <div class="col-xs-4" >
                        <asp:button id="btnLogin" runat="server" text="Sign in" class="btn bg-blue btn-block" onclick="btnLogin_Click" />
                    </div>
                    <!-- /.col -->
                </div>

            </div>


            <div class="margin text-center" style="color:#ffffff">
                <span>Kent Engineers - &copy; 2015</span>
            </div>
        </div>

        <!-- jQuery 2.0.2 -->
        <script src="js/jquery-1.11.2.min.js" type="text/javascript"></script>
        <!-- Bootstrap -->
        <script src="js/bootstrap.min.js" type="text/javascript"></script>

        <script type="text/javascript">

            $(document).ready(function () {
                $('#user_name').css("display", "none");
                $('#password').css("display", "none");
            });

            $('#<%= btnLogin.ClientID %>').click(function () {
                if (Validation()) {
                    return true;
                } else {
                    return false;
                }

            });

            function Validation() {
                var status = true;
                var user_name = $('#<%= txtUserName.ClientID %>').val();
                var password = $('#<%= txtPassword.ClientID %>').val();

                if (user_name == "" || user_name == undefined) {
                    FlagWarning("USER_NAME", "User Name cannot be empty.");
                    status = false;
                } else {
                    ResetWarning("USER_NAME");
                }

                if (password == "" || password == undefined) {
                    FlagWarning("PASSWORD", "Password cannot be empty.");
                    status = false;
                } else {
                    ResetWarning("PASSWORD");
                }
                return status;
            }

            function FlagWarning(controller, message) {
                switch (controller) {
                    case "USER_NAME":
                        $('#user_name').html('<i class="fa fa-times-circle-o"></i> ' + message + '');
                        $('#user_name').parent().addClass('has-error');
                        $('#user_name').css("display", "block");
                        break;
                    case "PASSWORD":
                        $('#password').html('<i class="fa fa-times-circle-o"></i> ' + message + '');
                        $('#password').parent().addClass('has-error');
                        $('#password').css("display", "block");
                        break;
                }
            }

            function ResetWarning(controller) {
                switch (controller) {
                    case "USER_NAME":
                        $('#user_name').css("display", "none");
                        break;
                    case "PASSWORD":
                        $('#password').css("display", "none");
                        break;
                }
            }

        </script>
    </form>
</body>
</html>
