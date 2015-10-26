var currentUpdateEvent;
var addStartDate;
var addEndDate;
var globalAllDay;

function formatDate(dateToFormat) {
    var dateValue = new Date(dateToFormat);
    var monthValue = dateValue.getMonth() + 1;
    var dayValue = dateValue.getDate();
    var yearValue = dateValue.getFullYear();
    var hoursValue = dateValue.getHours();
    var minutesValue = dateValue.getMinutes();
    var secondsValue = dateValue.getSeconds();

    if (monthValue < 10)
        monthValue = '0' + monthValue;
    if (dayValue < 10)
        dayValue = '0' + dayValue;
    if (hoursValue < 10)
        hoursValue = '0' + hoursValue;
    if (minutesValue < 10)
        minutesValue = '0' + minutesValue;
    if (secondsValue < 10)
        secondsValue = '0' + secondsValue;

    // dd-mm-yyyy hh:mm:ss
    return (dayValue + '-' + monthValue + '-' + yearValue + ' ' + hoursValue + ':' + minutesValue + ':' + secondsValue);
}

function updateEvent(event, element) {

    //if ($(this).data("qtip")) $(this).qtip("destroy");

    currentUpdateEvent = event;

    $('#updateDialog').dialog('open');

    $("#eventName").val(event.title);
    $("#eventDescription").val(event.description);
    $("#eventLocation").val(event.location);
    $("#eventStartDate").text("Start Time: " + event.start.toLocaleString());
    $("#eventEndDate").text("End Time: " + event.end.toLocaleString());

    /*
    if (event.end === null) {
        $("#eventEnd").text("");
    }
    else {
        $("#eventEnd").text("" + event.end.toLocaleString());
    }
    */

}

function addSuccess(eventToAdd, newID) {
    // if eventToAdd is -1, means event was not added
    if (eventToAdd != -1) {
        $('#calendar').fullCalendar('renderEvent',
						{
						    title: eventToAdd.title,
						    start: addStartDate,
						    end: addEndDate,
						    id: newID,
						    description: eventToAdd.description,
                            location: eventToAdd.location,
						    allDay: globalAllDay
						},
						true // make the event "stick"
					);

        $('#calendar').fullCalendar('unselect');
    }

}

function updateSuccess(updateResult) {
    $('#calendar').fullCalendar('updateEvent', updateResult);
}

function deleteSuccess(eventID) {
    $('#calendar').fullCalendar('removeEvents', eventID);
}

function UpdateTimeSuccess(updateResult) {
    //alert(updateResult);
}

function updateEventOnDropResize(event, allDay) {

    var startDate = formatDate(event.start);
    var endDate = formatDate(event.end)

    // create an event to pass to the server
    var eventToUpdate = {
        id: event.id,
        title: event.title,
        description: event.description,
        location: event.location,
        start: startDate,
        end: endDate,
        allDay: allDay
    };

    if (event.end === null) { //allDay event that is one day long
        eventToUpdate.end = eventToUpdate.start;
    }

    $.ajax({
        url: aspUrls.UpdateCalendarEventOnDrop,
        data: eventToUpdate
        //success: updateSuccess(event)
    });

}

function eventDropped(event, dayDelta, minuteDelta, allDay, revertFunc) {
    updateEventOnDropResize(event, allDay);
}

function eventResized(event, dayDelta, minuteDelta, revertFunc) {
    updateEventOnDropResize(event, false);
}

function checkForSpecialChars(stringToCheck) {
    var pattern = /[^A-Za-z0-9 ]/;
    return pattern.test(stringToCheck);
}

// LONG HAIR DONT CARE
// Creates a download link and simulates a mouse click to open it
function downloadSuccess(csv){
    a = document.createElement('a');
    a.textContent = 'download';
    a.download = "calendarevent.csv";
    a.href = 'data:text/csv;charset=utf-8,' + escape(csv);
    ev = document.createEvent("MouseEvents");
    ev.initMouseEvent("click", true, false, self, 0, 0, 0, 0, 0,
                  false, false, false, false, 0, null);
    a.dispatchEvent(ev);
}


$(document).ready(function () {

    //add dialog
    $('#addDialog').dialog({
        autoOpen: false,
        width: 500,
        modal: true,
        buttons: {
            "Add Event": function () {

                var startDate = formatDate(addStartDate);
                var endDate = formatDate(addEndDate)

                var eventToAdd = {
                    id: 0,
                    title: $("#addEventName").val(),
                    description: $("#addEventDescription").val(),
                    location: $("#addEventLocation").val(),
                    start: startDate,
                    end: endDate,
                    allDay: globalAllDay
                };

                if (checkForSpecialChars(eventToAdd.title) || checkForSpecialChars(eventToAdd.description)) {
                    alert("please enter characters: A to Z, a to z, 0 to 9, spaces");
                }
                else {

                    $.ajax({
                        url: aspUrls.AddCalendarEvent,
                        data: eventToAdd,
                        success: function (data) { // data should be the id of the new calendar object
                            addSuccess(eventToAdd, data);
                        }
                    });

                    $(this).dialog("close");
                }

            }

        }
    });

    // update dialog
    $('#updateDialog').dialog({
        autoOpen: false,
        width: 500,
        modal: true,
        buttons: {
            "Download Event": function () {

                var eventToDownload = { id: currentUpdateEvent.id};

                $.ajax({
                    url: aspUrls.DownloadCalendarEvent,
                    data: eventToDownload,
                    success: function (data) {
                        downloadSuccess(data);
                    }
                });

                $(this).dialog("close");

            },
            "Delete Event": function () {

                if (confirm("Do you really want to delete this event?")) {

                    var eventToDelete = { id: currentUpdateEvent.id};

                    $.ajax({
                        url: aspUrls.DeleteCalendarEvent,
                        data: eventToDelete,
                        success: deleteSuccess(eventToDelete.id)
                    });

                    $(this).dialog("close");

                }

            },
            "Update Event": function () {

                var startDate = formatDate(currentUpdateEvent.start);
                var endDate = formatDate(currentUpdateEvent.end)

                currentUpdateEvent.title = $("#eventName").val();
                currentUpdateEvent.description = $("#eventDescription").val();
                currentUpdateEvent.location = $("#eventLocation").val();

                // create an event to pass to the server
                var eventToUpdate = {
                    id: currentUpdateEvent.id,
                    title: currentUpdateEvent.title,
                    description: currentUpdateEvent.description,
                    location: currentUpdateEvent.location,
                    start: startDate,
                    end: endDate,
                    allDay: currentUpdateEvent.allDay
                };

                if (checkForSpecialChars(eventToUpdate.title) || checkForSpecialChars(eventToUpdate.description)) {
                    alert("please enter characters: A to Z, a to z, 0 to 9, spaces");
                }
                else {

                    $.ajax({
                        url: aspUrls.UpdateCalendarEvent,
                        data: eventToUpdate,
                        success: updateSuccess(currentUpdateEvent)
                    });

                    $(this).dialog("close");
                }

            }

        }
    });

    $('#calendar').fullCalendar({
        theme: true,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month, agendaWeek, agendaDay'
        },
        height: 600,
        defaultView: 'agendaWeek',
        editable: true,
        selectable: true,
        selectHelper: true,
        select: selectionMade,
        eventClick: updateEvent,
        eventResize: eventResized,
        eventDrop: eventDropped,
        slowMinutes: 15,
        events: aspUrls.CalendarData
    })

});

function selectionMade(startDate, endDate, allDay) {
    $('#addDialog').dialog('open');

    clearAddDialog();

    $("#addEventStartDate").text("Start Time: " + startDate.toLocaleString());
    $("#addEventEndDate").text("End Time: " + endDate.toLocaleString());

    addStartDate = startDate;
    addEndDate = endDate;
    globalAllDay = allDay;
}

function clearAddDialog() {
    $("#addEventName").val("");
    $("#addEventDescription").val("");
    $("#addEventLocation").val("");
}