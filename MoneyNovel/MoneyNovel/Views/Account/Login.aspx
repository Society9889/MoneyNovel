<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoneyNovel.Models.LoginModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Login
</asp:Content>



<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 style="text-align:center;">Welcome to MoneyNovel</h1>
    <h3 style="text-align:center; padding-top:50px">Please login below using Facebook</h3>

        <div style="text-align:center; padding-top:25px;">
            <%: Html.Action("ExternalLoginsList", new { ReturnUrl = ViewBag.ReturnUrl }) %>
        </div>
    
    <script>
        // Code to log in through the javascript sdk for facebook
        $("#fblogin").click(function (e) {
            FB.login(function (response) {
                // handle the response
                if (response.authResponse) {
                    FB.api('/me', function (response) {
                        $("#name").text(response.name);
                    });

                    location.href = '<%= Url.Action("Index", "Home") %>';

                } else {
                    console.log('User cancelled login or did not fully authorize.');
                }
            // Get permissions
            }, { scope: 'publish_actions,read_stream' });
        });

        //user shouldnt be able to navigate site while not logged in
        $("#navbar").hide();
        
        $(function () {
            //$("body > div.ui-widget.ui-corner-top.ui-chatbox").hide();  // hide chat box
            $("#chat_div").chatbox("option", "hidden", true);
        });
        
    </script>


</asp:Content>

<asp:Content ID="scriptsContent" ContentPlaceHolderID="ScriptsSection" runat="server">
    <%: Scripts.Render("~/bundles/jqueryval") %>
</asp:Content>
