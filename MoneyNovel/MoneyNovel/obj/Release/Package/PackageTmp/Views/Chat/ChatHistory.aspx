<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: ViewBag.title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% if(ViewBag.messages.Count == 0){ %>
        <div style="text-align: center; padding: 20px"><i>No history found!</i></div>
    <% } else { %>
    <table class="chatHistoryTable">
        <tr>
            <th>From</th>
            <th>Time Sent</th>
            <th>Message</th>
        </tr>
        <% foreach(var cm in ViewBag.messages){ %>
        <tr>
            <td><%: cm.UserName %></td>
            <td><%: cm.TimeSent %></td>
            <td><%: cm.Message %></td>
        </tr>
        <% } %>
    </table>
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
  
</asp:Content>
