<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ICollection<Microsoft.Web.WebPages.OAuth.AuthenticationClientData>>" %>

<% if (Model.Count == 0) { %>
    <div class="message-info">
        <p>There are no external authentication services configured. See <a href="http://go.microsoft.com/fwlink/?LinkId=252166">this article</a>
        for details on setting up this ASP.NET application to support logging in via external services.</p>
    </div>
<% } else {
    using (Html.BeginForm("ExternalLogin", "Account", new { ReturnUrl = ViewBag.ReturnUrl })) { %>
    <%: Html.AntiForgeryToken() %>
    <fieldset id="socialLoginList">
        <legend>Log in using another service</legend>
        <p>
        <% foreach (Microsoft.Web.WebPages.OAuth.AuthenticationClientData p in Model) { %>
            <!--<button class="btn btn-primary" type="submit" name="provider" value="<%: p.AuthenticationClient.ProviderName %>" title="Log in using your <%: p.DisplayName %> account"
                style="margin-left: auto; margin-right: auto; width:100px"><%: p.DisplayName%></button>-->
            <button class="btn btn-primary" type="submit" name="provider" value="<%: p.AuthenticationClient.ProviderName %>" title="Log in using your <%: p.DisplayName %> account"
                style="width:100px">Login</button>
        <% } %>
        </p>
    </fieldset>
    <% }
} %>
