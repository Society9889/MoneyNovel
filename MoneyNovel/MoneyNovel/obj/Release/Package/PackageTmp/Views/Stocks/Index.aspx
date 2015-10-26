<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Stocks
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Styles.Render("~/Content/stocks") %>
    <script type="text/javascript">

        function clicked() {
            var tickerString = $('#TickerSearch').val();
            $.ajax({
                cache: false,
                async: true,
                type: "GET",
                url: "<%= Url.Action("StockInfo", "Stocks") %>",
                data: {name : tickerString},
                success: function (data) {
                    $('#rightSide').html(data);
                }
            });
        }

        $(document).ready(function () {
            $('.form-control').keydown(function (event) {
                if (event.keyCode == 13) {
                    clicked();
                    document.getElementById("TickerSearch").value = "";
                    return false;
                }
            });
            $("#History").click(function (evt) {
                evt.preventDefault();
                document.location.href = '<%= Url.Action("History", "Stocks") %>';;
            });
        });


</script>
    <div>
    <h2 style="display: inline-block"> Total net value: </h2>
    <% if(ViewBag.totalNetValue < 0)
       { %>
        <h2 style="color: red; display: inline-block" >$<%: ViewBag.totalNetValue %></h2>
    <% } else { %>
        <h2 style="color: green; display: inline-block" >$<%: ViewBag.totalNetValue %></h2>
    <%} %>
    </div>
    <div class="container-fluid">
        <div class="row-fluid">
            <div id="leftSide" class="col-md-6">
                <div style ="clear:right">

                   
                     <input class="form-control" id="TickerSearch" placeholder="GOOG, AAPL, etc..." maxlength="5" style="width:65%; float:left">
                         <button onclick="clicked()" type="submit" id="search" class="btn btn-default" style="width:17%">Search</button>
                        <button class="btn btn-info" type="button" id="History" style="width:17%">View History</button>
                   

                </div>
                <div id="leftside" style="height:500px; overflow:auto">
                    <% Html.RenderAction("StocksList", "Stocks"); %>
                </div>
            </div>
            <div id="rightSide" class="col-md-6">
                <% Html.RenderAction("StockInfo", "Stocks"); %> 
            </div>
            </div>
        </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
  
</asp:Content>
