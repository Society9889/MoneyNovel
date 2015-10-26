<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <header>
        <!-- Needed to ensure the jquery dialog appears in front of the fullcalendar -->
        <style>
            .ui-dialog
            {
                z-index: 99999 !important;
            }
            .ui-front { 
                z-index: 99999 !important; 
            }
        </style>
    </header>

    <div id='calendar' style="width:80%"></div> 

    <button class="btn btn-primary" data-toggle="modal" data-target="#uploadModal" style="margin-top:20px">
        Upload Calendar Event
    </button>

    <div id="addDialog" title="Add Event" style="z-index:99999; background:#ffffff">
        <form>
            <fieldset>
            <label style="font-size:medium" for="name">Event Title</label>
            <input type="text" name="name" id="addEventName" placeholder="Event Title" class="form-control">
            <label style="font-size:medium" for="location">Location</label>
            <input type="text" name="location" id="addEventLocation" placeholder="Location" class="form-control">
            <label style="font-size:medium" for="description">Description</label>
            <textarea name="description" rows="3" id="addEventDescription" placeholder="Description" class="form-control"></textarea>
            <label style="font-size:medium" id="addEventStartDate"></label>
            <label style="font-size:medium" id="addEventEndDate"></label>
            </fieldset>
        </form>
    </div>

    <div id="updateDialog" title="Update Event" style="z-index:99999; background:#ffffff">
        <form>
            <fieldset>
            <label style="font-size:medium" for="name">Event Title</label>
            <input type="text" name="name" id="eventName" placeholder="Event Title" class="form-control">
            <label style="font-size:medium" for="location">Location</label>
            <input type="text" name="location" id="eventLocation" placeholder="Location" class="form-control">
            <label style="font-size:medium" for="description">Description</label>
            <textarea name="description" rows="3" id="eventDescription" placeholder="Description" class="form-control"></textarea>
            <label style="font-size:medium" id="eventStartDate"></label>
            <label style="font-size:medium" id="eventEndDate"></label>
            </fieldset>
        </form>
    </div>

    <div class="modal fade" id="uploadModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="myModalLabel">Calendar Event Uploader</h4>
                </div>
                <div class="modal-body">
                        <% using (Html.BeginForm("UploadCalendarEvent", "Calendar", FormMethod.Post, new { @id = "uploadForm", @enctype = "multipart/form-data" }))
                           { %>
                            <input id="uploadFile" name="uploadFile" type="file" />
                         <%} %>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="submit">Submit</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FeaturedContent" runat="server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsSection" runat="server">

    <%: Scripts.Render("~/bundles/calendarload") %>

    <script>
        $('#submit').click(function () {
            $('#uploadForm').submit();
        });
    </script>

</asp:Content>
