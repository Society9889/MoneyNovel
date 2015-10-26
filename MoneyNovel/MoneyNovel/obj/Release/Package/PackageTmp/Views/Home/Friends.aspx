<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Friends
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
      <div class="container-fluid">
        <div class="row-fluid">

            <div id="left" class="col-md-3">
            </div>

            <div id="center" class="col-md-6" style="height:100%">

                <div id="statusupdate" style="clear:right">
                    <h3>Search for a Friend</h3>
                    <input class="form-control" placeholder="Name of friend" id="search_text" style="width:80%; float:left"/>
                    <button class="btn btn-default" id="search_btn" style="width:20%">Search</button>
                </div>

                <div id="friends_results_container" style="visibility:hidden">
                    <h3>Results</h3>
                    <div id="friends_results" style="height:500px; overflow:auto">
                    </div>
                </div>

            </div>

            <div id="right" class="col-md-3">
            </div>

        </div>
    </div>

    <script type="text/javascript">
        var usersId = 0;

        function renderUser(user) {
            var htmlString = "<div style='margin: 5px'>";
            htmlString += "<img src='" + user.picture.data.url + "' style='margin-right: 5px' />";
            htmlString += "<b>" + user.name + "</b>";
            htmlString += "<button id='addUser" + usersId + "' class='btn btn-default' style='float:right'>Add Friend</button>";
            htmlString += "</div>";

            $("#friends_results").append(htmlString);

            $("#addUser" + usersId).click(function (e) {
                FB.ui({
                    method: 'friends',
                    id: user.id
                }, function (response) { console.log(response); });
            });

            usersId += 1;
        }

        function addUsers(response) {
            console.log(response);
            for (user in response.data) {
                renderUser(response.data[user]);
            }

            if (response.paging && response.paging.next) {
                $("#friends_results").append("<div id='next_friends'><a href='javascript:void(0)'>Load more..</a></div>");
                $("#next_friends").click(function () {
                    $("#next_friends").remove();
                    FB.api(response.paging.next, 'get', function (response) {
                        addUsers(response);
                    });
                });
            } else {
                $("#friends_results").append("<div style='text-align:center; width:100%; padding: 20px'>End of results</div>");
            }
        }

        $("#search_btn").click(function (e) {
            $("#friends_results_container").css("visibility", "visible");
            $("#friends_results").html("");
            var search = $("#search_text").val();
            //search?q=Jake&type=user&limit=10&fields=id,name,picture
            FB.api('/search', 'get', { q: search, type: "user", limit: 10, fields: "id,name,picture" }, function (response) {
                addUsers(response);
            });
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
