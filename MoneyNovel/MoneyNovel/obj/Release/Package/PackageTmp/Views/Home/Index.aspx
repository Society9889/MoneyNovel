<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoneyNovel.Models.StatusModel>" %>



<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page - MoneyNovel

</asp:Content>

<asp:Content ID="indexFeatured" ContentPlaceHolderID="FeaturedContent" runat="server">
    <section class="featured">
    </section>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Placeholder divs using the bootstrap grid system for layout. The background colors and static heights
         are included just to show the size/location of the divs, remove them once you enter content. -->
    <div class="container-fluid">
        <div class="row-fluid">

            <div id="left" class="col-md-3">

                <div id="stocks">
                    <h3>Top Stocks</h3>
                    <table class="table table-bordered">

                        <thead>
                            <tr>
                                <th>Ticker</th>
                                <th>Price</th>
                                <th>Shares</th>
                            </tr>
                        </thead>

                        <tbody>
                            <% foreach(var stock in ViewBag.stockList){ %>
                                  <tr class="stockrow">
                                      <th><%: stock.Ticker  %></th>
                                      <% if (stock.CurrentPrice >= stock.OpeningPrice){ %>
                                            <th style="color:green"><%: stock.CurrentPrice  %></th>
                                      <%}else{ %>
                                            <th style="color:red"><%: stock.CurrentPrice  %></th>
                                      <%}%>
                                      <th><%: stock.Shares  %></th>
                                  </tr>
                            <% } %> 
                         </tbody>

                    </table>
                </div>
            </div>


            <div id="center" class="col-md-6" style="height:100%">

                <!-- Add Photo Modal, hidden until clicked -->
                <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                <h4 class="modal-title" id="myModalLabel">Image Uploader</h4>
                            </div>
                            <div class="modal-body">
                            <style>
                              article, aside, figure, footer, header, hgroup, 
                              menu, nav, section { display: block; }
                            </style>
                            <div style="clear:right">
                              <input type='file' id="imageLocation" onchange="readURL(this);" />
                                <div style="min-height:50px">
                                    <img id="blah" src="#" alt="Your Image" />
                                </div>
                            </div>
                            <text>Description: </text>
                            <input type="text" id ="description" />
                            </div>
                            <div class="modal-footer">
                                <div style="clear:both">
                                    <text style="float:left">Status: </text>
                                    <a id ="postStatus" style ="float:left"></a>
                                </div>
                                <button type="button" class="btn btn-default" onclick="reset()" data-dismiss="modal">Cancel</button>
                                <button type="button" onclick="postPic()" class="btn btn-primary">Post</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="statusupdate" style="clear:right">
                    <h3>Update Status</h3>

                    <input class="form-control" placeholder="What's on your mind?" id="fbposttext" style="width:65%; float:left"/>
                    <button class="btn btn-primary" id="fbpost" style="width:18%">Post</button>
                    <!-- Button trigger modal -->
                    <button class="btn btn-link" id="picpost" data-toggle="modal" data-target="#myModal" style="width:15%">
                        Add Photos
                    </button>

                </div>

                <div id="newsfeed-wrapper">
                    <h3>NewsFeed</h3>
                    <div id="newsfeed" style="height:500px; overflow:auto">
                        <div style="text-align: center">
                            Loading newsfeed...<br />
                            <img style="align-content: center" src="../Images/loading.gif" />
                        </div>
                    </div>
                </div>

            </div>

            <div id="events" class="col-md-3">
                <h3>Todays Events</h3>
                <div id="dailyCalendar">
                    <label id="eventLabel"></label>
                </div>
            </div>

        </div>
    </div>

    <script>

        var accessToken;
        var pic_to_upload;
        //To render a preview of the image selected
        function readURL(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    pic_to_upload = e.target.result;
                    $('#blah')
                        .attr('src', e.target.result)
                        .width(200)
                        .height(200);
                };

                reader.readAsDataURL(input.files[0]);
            }
        }

        function postPic() {
            var reader = new FileReader();
            var pic = pic_to_upload.replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
            var mime = document.getElementById('imageLocation').files[0].type;
            try {
                blob = dataURItoBlob(pic, mime);
            } catch (e) {
                console.log(e);
                document.getElementById('postStatus').textContent = "Error";
                document.getElementById('postStatus').style.color = "Red";
            }
            var fd = new FormData();
            fd.append("access_token", accessToken);
            fd.append("source", blob);
            fd.append("message", document.getElementById('description').value);
            try {
                $.ajax({
                    url: "https://graph.facebook.com/" + "me/photos?access_token=" + accessToken,
                    type: "POST",
                    data: fd,
                    processData: false,
                    contentType: false,
                    cache: false,
                    success: function (data) {
                        console.log("success " + data);
                    },
                    error: function (shr, status, data) {
                        console.log("error " + data + " Status " + shr.status);
                    },
                    complete: function () {
                        console.log("Ajax Complete");
                    }
                    
                });
                success();
            } catch (e) {
                console.log(e);
                document.getElementById('postStatus').textContent = "Error";
                document.getElementById('postStatus').style.color = "Red";
            }
        }

        function dataURItoBlob(dataURI, mime) {
            // convert base64 to raw binary data held in a string
            // doesn't handle URLEncoded DataURIs

            var byteString = window.atob(dataURI);

            // separate out the mime component


            // write the bytes of the string to an ArrayBuffer
            //var ab = new ArrayBuffer(byteString.length);
            var ia = new Uint8Array(byteString.length);
            for (var i = 0; i < byteString.length; i++) {
                ia[i] = byteString.charCodeAt(i);
            }
            // write the ArrayBuffer to a blob, and you're done
            var blob = new Blob([ia], { type: mime });

            return blob;
        }

        //reset the input of the fileinputer
        function reset() {
            document.getElementById('imageLocation').value = "";
            document.getElementById('postStatus').textContent = "";
            document.getElementById('description').value = "";
            $('#blah')
                        .attr('src', '#')
                        .width(0)
                        .height(0);
        }

        function success() {
            document.getElementById('imageLocation').value = "";
            document.getElementById('postStatus').textContent = "Successful post";
            document.getElementById('postStatus').style.color = "green";
            document.getElementById('description').value = "";
            $('#blah')
                        .attr('src', '#')
                        .width(0)
                        .height(0);
        }

        // Code that will post a status to facebook
        $("#fbpost").click(function (e) {
            var body = fbposttext.value;
            fbposttext.value = "";
            console.log(body);
            //var access_token = FB.getAuthResponse()['accessToken'];
                FB.api('/me/feed', 'post', { message: body }, function (response) {
                    console.log(response);
                    if (!response || response.error) {
                        //alert('Error occured');
                    } else {
                        //message sent
                        // Add post to newsfeed:
                        var item_div = "<div class='newsfeeditem'>";
                        item_div += "<b>" + userFullName + "</b><br>";
                        item_div += body;
                        item_div += "</div>";
                        $("#newsfeed").prepend(item_div);
                    }
                });
        });

        // Call this method when FB.api is ready to recieve calls
        var auth_status_change_callback = function (response) {
            loadNewsFeed(response);
        }

        function loadNewsFeed(response) {
            var authResponse = response.authResponse;
            if (authResponse == null) {
                console.log("null");
                setTimeout(loadNewsFeed, 100);
            } else {
                accessToken = authResponse['accessToken'];
                FB.api('/me/home', 'get', { access_token: accessToken }, function (response) {
                    console.log(response);
                    $("#newsfeed").html("");
                    addNewsFeedItems(response);
                });
            }

        }

        var nextNewsFeed = "";

        function addNewsFeedItems(response) {
            for (var newsItem in response.data) {
                var item_div = "<div class='newsfeeditem'>";
                item_div += "<b>" + response.data[newsItem].from.name;
                if (response.data[newsItem].status_type == "wall_post" && response.data[newsItem].to != null) {
                    item_div += " > " + response.data[newsItem].to.data[0].name;
                }
                item_div += "</b><br>";
                if (response.data[newsItem].message != null) {
                    item_div += response.data[newsItem].message;
                } else if (response.data[newsItem].description != null) {
                    item_div += response.data[newsItem].description;
                } else if (response.data[newsItem].caption != null) {
                    item_div += response.data[newsItem].caption;
                }
                if (response.data[newsItem].picture != null) {
                    item_div += "<br><a target='_BLANK' href=" + response.data[newsItem].link + ">";
                    item_div += "<img src='" + response.data[newsItem].picture + "' /></a>";
                }
                item_div += "</div>";
                $("#newsfeed").append(item_div);
            }
            if (response.paging && response.paging.next) {
                nextNewsFeed = response.paging.next;
                $("#newsfeed").append("<div id='next_newsfeed'><a href='javascript:void(0)'>Load more..</a></div>");
                $("#next_newsfeed").click(function () {
                    $("#next_newsfeed").remove();
                    FB.api(response.paging.next, 'get', { access_token: accessToken }, function (response) {
                        addNewsFeedItems(response);
                    });
                });
            } else {
                $("#newsfeed").append("<div style='text-align:center; width:100%; padding: 20px'>End of newsfeed</div>");
            }
        }


        $(document).on("facebook:ready", function () {
            FB.Event.subscribe('auth.statusChange', auth_status_change_callback);
            //loadNewsFeed();
        });

    </script>

    <script>
        $(document).ready(function () {
            $('#dailyCalendar').fullCalendar({
                theme: true,
                height: 250,
                header: false,
                defaultView: 'basicDay',
                events: "<%= Url.Action("GetEventsForToday", "Calendar") %>",
                eventAfterAllRender: function (view) {
                    if ($('.fc-view-basicDay').has('.fc-event').length === 0) {
                        // show 'no events' message
                        $('#eventLabel').text("No Events Today");
                    } else {
                        // do nothing
                    }
                }
            })
        });
    </script>

</asp:Content>
