using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyNovel.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace MoneyNovel.Controllers
{
    public class CalendarController : BaseController
    {
        //
        // GET: /Calendar/

        public ActionResult Index()
        {
            return View();
        }

        // Get the calendar events for the logged in user and return them as JSON
        public ActionResult CalendarData()
        {
            IList<CalendarDTO> eventList = new List<CalendarDTO>();

            using (CalendarContext db = new CalendarContext())
            {
                // For each calendar event, create a CalendarDTO object we can pass to the view as JSON
                foreach (CalendarEvent calEvent in db.CalendarEvents.ToList().Where(ce => ce.FBID == (String)Session["FBID"]))
                {
                    eventList.Add(new CalendarDTO
                    {
                        id = calEvent.EventID,
                        title = calEvent.Title,
                        start = ToUnixTimespan(calEvent.StartTime),
                        end = ToUnixTimespan(calEvent.EndTime),
                        allDay = calEvent.AllDay,
                        description = calEvent.Description,
                        location = calEvent.Location
                    });
                }
            }

            return Json(eventList, JsonRequestBehavior.AllowGet);
        }

        // Get the calendar events for the logged in user and return them as JSON
        public ActionResult GetEventsForToday()
        {
            IList<CalendarDTO> eventList = new List<CalendarDTO>();
            DateTime today = DateTime.Now;

            using (CalendarContext db = new CalendarContext())
            {
                // For each calendar event, create a CalendarDTO object we can pass to the view as JSON
                foreach (CalendarEvent calEvent in db.CalendarEvents.ToList().Where(ce => (ce.StartTime.Day == today.Day || ce.EndTime.Day == today.Day) && ce.FBID == (String)Session["FBID"]))
                {
                    eventList.Add(new CalendarDTO
                    {
                        id = calEvent.EventID,
                        title = calEvent.Title,
                        start = ToUnixTimespan(calEvent.StartTime),
                        end = ToUnixTimespan(calEvent.EndTime),
                        allDay = calEvent.AllDay,
                        description = calEvent.Description,
                        location = calEvent.Location
                    });
                }
            }

            return Json(eventList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCalendarEvent(string title, string description, string location, string start, string end, string allDay)
        {
            int newID;

            // Convert start and end to DateTime objects
            DateTime startTime = DateTime.ParseExact(start, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(end, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);

            // Create CalendarEvent to be passed to database
            CalendarEvent calEvent = new CalendarEvent
            {
                FBID = (String)Session["FBID"],
                Title = title,
                Description = description,
                Location = location,
                StartTime = startTime,
                EndTime = endTime,
                Recurring = false,
                AllDay = Boolean.Parse(allDay)
            };

            using (CalendarContext db = new CalendarContext())
            {
                // Add CalendarEvent to database
                db.CalendarEvents.Add(calEvent);
                db.SaveChanges();
                newID = calEvent.EventID;
            }

            // Return the new ID value so we can set it on the new calendar object
            return Json(newID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCalendarEvent(string id, string title, string description, string location, string start, string end, string allDay)
        {
            // Convert start and end to DateTime objects
            /*
            DateTime startTime = DateTime.ParseExact(start, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(end, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            */
            int eventID = Convert.ToInt32(id);

            using (CalendarContext db = new CalendarContext())
            {
                // Update CalendarEvent in database
                CalendarEvent calEventToUpdate = db.CalendarEvents.Where(ce => ce.EventID == eventID).FirstOrDefault();

                // Create CalendarEvent to be passed to database
                // Only title, description, and location can be updated currently
                CalendarEvent calEvent = new CalendarEvent
                {
                    EventID = Convert.ToInt32(id),
                    FBID = (String)Session["FBID"],
                    Title = title,
                    Description = description,
                    Location = location,
                    StartTime = calEventToUpdate.StartTime,
                    EndTime = calEventToUpdate.EndTime,
                    Recurring = false,
                    AllDay = calEventToUpdate.AllDay
                };

                if (calEventToUpdate != null)
                {
                    db.Entry(calEventToUpdate).CurrentValues.SetValues(calEvent);
                }
                db.SaveChanges();
            }

            return View();
        }

        public ActionResult UpdateCalendarEventOnDrop(string id, string title, string description, string location, string start, string end, string allDay)
        {
            // Convert start and end to DateTime objects
            DateTime startTime = DateTime.ParseExact(start, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(end, "dd-MM-yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);

            int eventID = Convert.ToInt32(id);

            using (CalendarContext db = new CalendarContext())
            {
                // Update CalendarEvent in database
                CalendarEvent calEventToUpdate = db.CalendarEvents.Where(ce => ce.EventID == eventID).FirstOrDefault();

                // Create CalendarEvent to be passed to database
                // Only title, description, and location can be updated currently
                CalendarEvent calEvent = new CalendarEvent
                {
                    EventID = Convert.ToInt32(id),
                    FBID = (String)Session["FBID"],
                    Title = title,
                    Description = description,
                    Location = location,
                    StartTime = startTime,
                    EndTime = endTime,
                    Recurring = false,
                    AllDay = calEventToUpdate.AllDay
                };

                if (calEventToUpdate != null)
                {
                    db.Entry(calEventToUpdate).CurrentValues.SetValues(calEvent);
                }
                db.SaveChanges();
            }

            return View();
        }

        public ActionResult DeleteCalendarEvent(string id)
        {
            int eventID = Convert.ToInt32(id);

            using (CalendarContext db = new CalendarContext())
            {
                // Delete CalendarEvent from database
                CalendarEvent calEventToDelete = db.CalendarEvents.Where(ce => ce.EventID == eventID).FirstOrDefault();
                db.CalendarEvents.Remove(calEventToDelete);

                db.SaveChanges();
            }

            return View();
        }

        // Download a single CalendarEvent as a .csv file
        public ActionResult DownloadCalendarEvent(string id)
        {
            int eventID = Convert.ToInt32(id);

            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=calendarevent.csv");
            Response.ContentType = "text/csv";

            // Convert List to CSV
            string csv = "";
            using (CalendarContext db = new CalendarContext())
            {
                IEnumerable<CalendarEvent> calEvent = db.CalendarEvents.ToList().Where(ce => ce.EventID == eventID);
                csv = String.Join("\r\n", calEvent.Select(x => x.ToCSV()).ToArray());
            }
            Response.Write(csv);
            Response.End();

            return Content(String.Empty);
        }

        // Upload a single CalendarEvent from a .csv file
        [HttpPost]
        public ActionResult UploadCalendarEvent(HttpPostedFileBase uploadFile)
        {
            int newID = 0;

            if (uploadFile == null)
            {
                return RedirectToLocal("/Calendar/Index");
            }
            StreamReader csvreader = new StreamReader(uploadFile.InputStream);

            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(',');

                try
                {
                    // Create new CalendarEvent to pass to database
                    CalendarEvent calEvent = new CalendarEvent
                    {
                        FBID = (String)Session["FBID"],
                        Title = values[0],
                        Description = values[1],
                        Location = values[2],
                        StartTime = Convert.ToDateTime(values[3]),
                        EndTime = Convert.ToDateTime(values[4]),
                        Recurring = Boolean.Parse(values[5]),
                        AllDay = Boolean.Parse(values[6])
                    };

                    using (CalendarContext db = new CalendarContext())
                    {
                        db.CalendarEvents.Add(calEvent);
                        db.SaveChanges();
                        newID = calEvent.EventID;
                    }
                }
                catch (Exception e) // If we can't read the file or it is in the wrong format
                {
                    return RedirectToLocal("/Calendar/Index");
                }

            }

            // Return the new ID value so we can set it on the new calendar object
            //return Json(newID, JsonRequestBehavior.AllowGet);
            return RedirectToLocal("/Calendar/Index");
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Converts a DateTime object to a long so it can be read by the calendar
        private long ToUnixTimespan(DateTime date)
        {
            TimeSpan tspan = date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)Math.Truncate(tspan.TotalSeconds);
        }

        #endregion

    }
}
