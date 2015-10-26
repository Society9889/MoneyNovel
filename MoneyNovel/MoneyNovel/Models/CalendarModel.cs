using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace MoneyNovel.Models
{

    public class CalendarContext : DbContext
    {
        public CalendarContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<CalendarEvent> CalendarEvents { get; set; }
    }

    [Table("CalendarEvent")]
    public class CalendarEvent
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }
        public string FBID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean Recurring { get; set; }
        public Boolean AllDay { get; set; }

        public string ToCSV()
        {
            return Title + "," + Description + "," + Location + "," + 
                StartTime + "," + EndTime + "," + Recurring + "," + AllDay;
        }
    }

    public class CalendarDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public bool allDay { get; set; }
        public string description { get; set; }
        public string location { get; set; }
    }

}