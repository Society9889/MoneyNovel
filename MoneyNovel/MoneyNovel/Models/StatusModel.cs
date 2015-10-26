using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace MoneyNovel.Models
{
    public class StatusModel
    {
        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}