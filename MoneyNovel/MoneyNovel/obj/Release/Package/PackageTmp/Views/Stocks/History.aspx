
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Modal -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="myModalLabel">Stock History Uploader</h4>
                </div>
                <div class="modal-body">
                        <% using (Html.BeginForm("UploadHistory", "Stocks", FormMethod.Post, new { @id = "upldFrm", @enctype = "multipart/form-data" }))
                           { %>
                            <input id="uploadFile" name="uploadFile" type="file" />
                         <%} %>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="subit">Submit</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="deleteWarning" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="ModalWarning">Stock History Uploader</h4>
                </div>
                     Uploading history will overwrite previous history, are you sure you want to delete your history?
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button data-dismiss="modal" data-toggle="modal" data-target="#myModal" class="btn btn-primary">Continue</button>
                </div>
            </div>
        </div>
    </div>

     <!-- Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="Modal">Stock History Uploader</h4>
                </div>
                     Are you sure you want to delete your history?
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button onclick="location.href='<%: Url.Action("ClearHistory", "Stocks") %>';" class="btn btn-danger">Clear History</button>
                </div>
            </div>
        </div>
    </div>

    <h2>History</h2>
    <div class="container-fluid">
        <div class="row-fluid">
            <div id="Center" class="col-md-">
                <div style="clear: right">


                    <input class="form-control" id="TickerSearch" placeholder="GOOG, AAPL, etc..." maxlength="5" style="width: 80%; float: left">
                    <button onclick="clicked()" type="submit" id="search" class="btn btn-default" style="width: 20%">Search</button>


                </div>
                <div id="HistoryList" style="height: 500px; overflow: auto">
                    <% Html.RenderAction("StockHistory", "Stocks");%>
                </div>
                <button onclick="location.href='<%= Url.Action("DownloadHistory", "Stocks") %>';" class="btn btn-success">Download History</button>
                <button class="btn btn-danger" data-toggle="modal" data-target="#deleteModal">Clear History</button>
                <!-- Button trigger modal -->
                <% if(ViewBag.hasHistory == true) {
                        %>
                <button class="btn btn-primary" data-toggle="modal" data-target="#deleteWarning">
                    Upload History
                </button>
                <%} else {
                        %>
                 <button class="btn btn-primary" data-toggle="modal" data-target="#myModal">
                    Upload History
                </button>
                <%} %>
            </div>
        </div>
    </div>

    <script>
       
            $('#subit').click(function () {
                $('#upldFrm').submit();
            });

            $(document).ready(function () {
                $('.form-control').keydown(function (event) {
                    if (event.keyCode == 13) {
                        clicked();
                        document.getElementById("TickerSearch").value = "";
                        return false;
                    }
                });
            });
        
            function clicked() {
                var tickerString = $('#TickerSearch').val();
                $.ajax({
                    cache: false,
                    async: true,
                    type: "GET",
                    url: "<%= Url.Action("StockHistory", "Stocks") %>",
                    data: { name: tickerString },
                    success: function (data) {
                        $('#HistoryList').html(data);
                    }
                }); 
            }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
