using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace MoneyNovel.Models
{
    public class Stock
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public int Shares { get; set; }
        public double CurrentPrice { get; set; }
        public double OpeningPrice { get; set; }
        public double HighPrice { get; set; }
        public double Investment { get; set; }
        public double StockValue { get; set; }
        public double NetWorth { get; set; }
        public string Comment { get; set; }
    }

    public class StockContext : DbContext
    {
        public StockContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<StockTransaction> StockTransactions { get; set; }
    }

    public class CommentContext : DbContext
    {
        public CommentContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<StockComment> StockComments { get; set; }
    }

    [Table("StockComments")]
    public class StockComment{
        [Key, Column(Order=0)]
        public string FBID {get; set;}
        [Key, Column(Order=1)]
        public string Ticker{get; set;}
        public string Comment{get; set;}

    }

    [Table("StockTransaction")]
    public class StockTransaction
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }
        public string FBID { get; set; }
        public string Ticker { get; set; }
        public int Shares { get; set; }
        public Double Price { get; set; }
        public DateTime TransactionDate { get; set; }

        public string ToCSV()
        {
            return TransactionID + "," + FBID + "," + Ticker + "," + Shares + "," +
                Price + "," + TransactionDate;
        }
    }


    public class TransactionModel
    {
        [Required]
        [Display(Name = "Amount")]
        public int Amount { get; set; }

        [Required]
        public String Ticker { get; set; }

        public String Comment { get; set; }
    }

}