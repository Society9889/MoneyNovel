using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MoneyNovel.Models;
using System.Text.RegularExpressions;

namespace MoneyNovel.Controllers
{
    public class StocksController : BaseController
    {

        //
        // GET: /Stocks/Index

        public ActionResult Index()
        {
            getNetworth();
            return View();
        }

        //fill a viewbag with the net worth of the user
        public void getNetworth()
        {
            double totalNetValue = 0.0;

            Dictionary<string, List<StockTransaction>> transactionDictonary = new Dictionary<string, List<StockTransaction>>();

            List<Stock> viewStockList = new List<Stock>();

            using (StockContext db = new StockContext())
            {
                foreach (StockTransaction trans in db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]))
                {
                    if (transactionDictonary.ContainsKey(trans.Ticker))
                    {
                        List<StockTransaction> transactionList = transactionDictonary[trans.Ticker];
                        transactionList.Add(trans);
                        transactionDictonary[trans.Ticker] = transactionList;
                    }
                    else
                    {
                        List<StockTransaction> transactionList = new List<StockTransaction>();
                        transactionList.Add(trans);
                        transactionDictonary.Add(trans.Ticker, transactionList);
                    }

                }


                foreach (KeyValuePair<string, List<StockTransaction>> pair in transactionDictonary)
                {
                    Stock stock = getOneStock(pair.Key);
                    stock.Shares = 0;
                    stock.Investment = 0;

                    //add up total shares and investment using stocktransaction history
                    foreach (StockTransaction action in pair.Value)
                    {
                        stock.Shares += action.Shares;
                        stock.Investment += action.Price * Math.Abs(action.Shares);

                    }

                    stock.Investment = Math.Round(stock.Investment, 2);
                    stock.StockValue = Math.Round(stock.Shares * stock.CurrentPrice, 2);
                    stock.NetWorth = Math.Round(stock.StockValue + stock.Investment, 2);
                    totalNetValue += stock.NetWorth;

                    viewStockList.Add(stock);
                }

            }

            ViewBag.totalNetValue = totalNetValue;
        }

        //Save a comment to the database
        public void SaveComment(String comment, String ticker)
        {

            Debug.WriteLine(comment + " " + ticker);

            String[] test = ticker.Split(' ');

            Debug.WriteLine(test[test.Length-2]);

            //this grabs the ticker from the label.
            String tic = test[test.Length -2];

            if (tic.Length < 5 && tic != "None")
            {
                using (CommentContext db = new CommentContext())
                {
                    

                    StockComment com = new StockComment();
                    com.FBID = (String)Session["FBID"];
                    com.Ticker = tic;
                    com.Comment = comment;
                    Debug.WriteLine("WHAT");
                    if (db.StockComments.Find(com.FBID, com.Ticker) != null)
                    {
                        db.StockComments.Remove(db.StockComments.Find(com.FBID, com.Ticker));
                        db.StockComments.Add(com);
                        db.SaveChanges();
                    } else {
                    db.StockComments.Add(com);
                    db.SaveChanges();
                
                    }
                }
            }
        }

        public ActionResult History()
        {
            using (StockContext db = new StockContext())
            {
                List<StockTransaction> sList = db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]).ToList();
                if (sList.Count() == 0)
                {
                    ViewBag.hasHistory = false;
                }
                else
                {
                    ViewBag.hasHistory = true;
                }
            }
            return View();
        }

        public ActionResult StockHistory(String name)
        {
            using (StockContext db = new StockContext())
            {
                if (name == null || name == "")
                {
                    ViewBag.sList = db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]);
                }
                else
                {
                    ViewBag.sList = db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"] & t.Ticker == name.ToUpper());
                }
            }
            return PartialView();
        }

        public ActionResult ClearHistory()
        {
            using (StockContext db = new StockContext())
            {
                IEnumerable<StockTransaction> transactions = db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]);
                foreach(StockTransaction t in transactions){
                    db.StockTransactions.Remove(t);
                }
                db.SaveChanges();
            }

            //return RedirectToLocal("/Stocks/History");
            return RedirectToAction("History", "Stocks");
        }

        public ActionResult DownloadHistory()
        {
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=stockHistory.csv");
            Response.ContentType = "text/csv";

            // Convert List to CSV
            string csv = "";
            using (StockContext db = new StockContext())
            {
                IEnumerable<StockTransaction> transactions = db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]);
                csv = String.Join("\r\n", transactions.Select(x => x.ToCSV()).ToArray());
            }
            Response.Write(csv);
            Response.End();

            return Content(String.Empty);
        }

        //Uploads the history of stock transactions from a csv file.
        [HttpPost]
        public ActionResult UploadHistory(HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null)
            {
                //return RedirectToLocal("/Stocks/History");
                return RedirectToAction("History", "Stocks");
            }
            StreamReader csvreader = new StreamReader(uploadFile.InputStream);

            using (StockContext db = new StockContext())
            {
                IEnumerable<StockTransaction> transactions = db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]);
                foreach (StockTransaction t in transactions)
                {
                    db.StockTransactions.Remove(t);
                }
                db.SaveChanges();
            }

            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(',');
                Debug.WriteLine(values[2] + " " + values[3] + " " + values[4]);
                Debug.WriteLine(line);
                using (StockContext db = new StockContext())
                {
                    StockTransaction st = new StockTransaction { FBID = (String)Session["FBID"], Ticker = values[2], Price = Convert.ToDouble(values[4]), Shares = Convert.ToInt32(values[3]), TransactionDate = Convert.ToDateTime(values[5])};
                    db.StockTransactions.Add(st);
                    db.SaveChanges();
                } 
            }
            //return RedirectToLocal("/Stocks/History");
            return RedirectToAction("History", "Stocks");
        }


        //
        // POST: /Stocks/BuyStock

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BuyStock(TransactionModel model)
        {
            if (ModelState.IsValid && model.Ticker != "None")
            {
                using (StockContext db = new StockContext())
                {
                    Stock s = getOneStock(model.Ticker);
                    StockTransaction st = new StockTransaction { FBID = (String)Session["FBID"], Ticker = model.Ticker, Price = s.CurrentPrice * -1, Shares = model.Amount, TransactionDate = DateTime.Now };
                    db.StockTransactions.Add(st);
                    db.SaveChanges();
                }
             //   getNetworth();
                //return RedirectToLocal("/Stocks/Index");
                return RedirectToAction("Index", "Stocks");
            }

            // If we got this far, something failed, redisplay form
         //   ModelState.AddModelError("Amount", "The amount provided is invalid.");
         //   return View("Index", model);
            return RedirectToAction("Index", "Stocks");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SellStock(TransactionModel model)
        {
            if (ModelState.IsValid && model.Ticker != "None" && model.Ticker != "NA")
            {
                Dictionary<string, List<StockTransaction>> transactionDictonary = new Dictionary<string, List<StockTransaction>>();

                List<Stock> viewStockList = new List<Stock>();

                using (StockContext db = new StockContext())
                {
                    foreach (StockTransaction trans in db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]))
                    {
                        if (transactionDictonary.ContainsKey(trans.Ticker))
                        {
                            List<StockTransaction> transactionList = transactionDictonary[trans.Ticker];
                            transactionList.Add(trans);
                            transactionDictonary[trans.Ticker] = transactionList;
                        }
                        else
                        {
                            List<StockTransaction> transactionList = new List<StockTransaction>();
                            transactionList.Add(trans);
                            transactionDictonary.Add(trans.Ticker, transactionList);
                        }

                    }

                    int shares = 0;
                    if (transactionDictonary.ContainsKey(model.Ticker))
                    {


                        foreach (StockTransaction trans in transactionDictonary[model.Ticker])
                        {
                            shares += trans.Shares;
                        }

                        if (shares != 0 && shares > model.Amount)
                        {
                            Stock s = getOneStock(model.Ticker);
                            StockTransaction st = new StockTransaction { FBID = (String)Session["FBID"], Ticker = model.Ticker, Price = s.CurrentPrice, Shares = model.Amount * -1, TransactionDate = DateTime.Now };
                            db.StockTransactions.Add(st);
                            db.SaveChanges();
                        }
                        else if (shares != 0 && shares < model.Amount)
                        {
                            Stock s = getOneStock(model.Ticker);
                            StockTransaction st = new StockTransaction { FBID = (String)Session["FBID"], Ticker = model.Ticker, Price = s.CurrentPrice, Shares = shares * -1, TransactionDate = DateTime.Now };
                            db.StockTransactions.Add(st);
                            db.SaveChanges();
                        }
                    }
                }
                //return RedirectToLocal("/Stocks/Index");
                return RedirectToAction("Index", "Stocks");
            }

            // If we got this far, something failed, redisplay form
      //      ModelState.AddModelError("Amount", "The amount provided is invalid.");
          //  return View("Index", model);
            return RedirectToAction("Index", "Stocks");
        }

        // Get the list of all stocks that we are currently 'watching'
        public ActionResult StocksList()
        {
            Dictionary<string, List<StockTransaction>> transactionDictonary = new Dictionary<string, List<StockTransaction>>();

            List<Stock> viewStockList = new List<Stock>();

            using (StockContext db = new StockContext())
            {
                foreach (StockTransaction trans in db.StockTransactions.ToList().Where(t => t.FBID == (String)Session["FBID"]))
                {
                    if(transactionDictonary.ContainsKey(trans.Ticker)){
                        List<StockTransaction> transactionList = transactionDictonary[trans.Ticker];
                        transactionList.Add(trans);
                        transactionDictonary[trans.Ticker] = transactionList;
                    }
                    else
                    {
                        List<StockTransaction> transactionList = new List<StockTransaction>();
                        transactionList.Add(trans);
                        transactionDictonary.Add(trans.Ticker, transactionList);
                    }
                    
                }

              
                 foreach(KeyValuePair<string, List<StockTransaction>> pair in transactionDictonary){
                     Stock stock = getOneStock(pair.Key);
                     stock.Shares = 0;
                     stock.Investment = 0;

                     //add up total shares and investment using stocktransaction history
                     foreach (StockTransaction action in pair.Value)
                     {
                         stock.Shares += action.Shares;
                         stock.Investment += action.Price*Math.Abs(action.Shares);

                     }

                     stock.Investment = Math.Round(stock.Investment, 2);
                     stock.StockValue = Math.Round(stock.Shares * stock.CurrentPrice, 2);
                     stock.NetWorth = Math.Round(stock.StockValue + stock.Investment, 2);

                     viewStockList.Add(stock);
                 }

            }

            using(CommentContext db = new CommentContext())
            {
                foreach (StockComment com in db.StockComments.ToList())
                {
                    if(com.FBID.Replace(" ","") == ((String)Session["FBID"]).Replace(" ", ""))
                    {
                        if (com.Comment != null && com.Comment != "") 
                        {
                            Boolean found = false;

                            foreach (Stock s in viewStockList)
                            {
                                if (s.Ticker.Replace(" ", "") == com.Ticker.Replace(" ", "")) 
                                {
                                    found = true;
                                }
                            }
                            if (found == false)
                            {
                                Stock stock = getOneStock(com.Ticker);
                                    stock.Shares = 0;
                                    stock.Investment = 0;
                                    viewStockList.Add(stock);
                            }
                        }
                    }
                    
                }
                ViewBag.slist = viewStockList;

            }

            return PartialView();
        }

        public string getStocks()
        {
            //Currently hard coded from viewing a list of stocks
            String url = "http://download.finance.yahoo.com/d/quotes.csv?s=";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "%40%5EDJI,GOOG&f=nsl1h0o&e=.csv");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        public Stock getOneStock(String name)
        {
            //Currently hard coded from viewing a list of stocks
            String url = "http://download.finance.yahoo.com/d/quotes.csv?s=";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + name + "&f=nsl1h0o");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string result = sr.ReadToEnd();
            sr.Close();

            Stock stock = new Stock();

            using(CommentContext db = new CommentContext())
            {
                StockComment com = db.StockComments.Find((String)Session["FBID"],name);
                if (com != null && com.Comment != null)
                {
                    stock.Comment = com.Comment;
                }
                else
                {
                    stock.Comment = "";
                }
                
            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                String sn = result.Replace("\"", "").Replace(", ", " ");
                // Regex.Split(, "\r\n");
                string[] stockInfo = sn.Split(',');
                if (!sn.Contains("N/A") && stockInfo.Length > 4)
                {
                    stock.Ticker = stockInfo[1];
                    stock.Name = stockInfo[0];
                    stock.OpeningPrice = Convert.ToDouble(stockInfo[4]);
                    stock.CurrentPrice = Convert.ToDouble(stockInfo[2]);
                    stock.HighPrice = Convert.ToDouble(stockInfo[3]);
                }
                else
                {
                    stock.Ticker = "NA";
                    stock.Name = "Not found";
                    stock.OpeningPrice = 0;
                    stock.CurrentPrice = 0;
                    stock.HighPrice = 0;
                }
            }

            return stock;
        }

        public ActionResult StockInfo(String name)
        {
            if (name != null)
            {
                name = name.Replace(" ", "");
            }
            
            String oneDay = "http://chart.finance.yahoo.com/z?s=" + name + "&t=1d&q=l&l=on&z=m";
            String oneMonth = "http://chart.finance.yahoo.com/z?s=" + name + "&t=1m&q=l&l=on&z=m";
            String threeMonth = "http://chart.finance.yahoo.com/z?s=" + name + "&t=3m&q=l&l=on&z=m";
            String oneYear = "http://chart.finance.yahoo.com/z?s=" + name + "&t=1y&q=l&l=on&z=m";

           // ViewBag.stock = getOneStock(name);
            TransactionModel model = new TransactionModel();
            if (name != null)
            {
                Debug.WriteLine("The name is " + name);
                Stock s = getOneStock(name.ToUpper());;
                ViewBag.stock = s;
                model.Ticker = s.Ticker;
            }
            else
            {
                Stock dumbStock = new Stock();
                dumbStock.Name = "No name";
                dumbStock.Ticker = "None";
                dumbStock.CurrentPrice = 0;
                dumbStock.HighPrice = 1;
                dumbStock.OpeningPrice = 0;
                dumbStock.Comment = "No comment";
                ViewBag.stock = dumbStock;
                model.Ticker = dumbStock.Ticker;
            }
            ViewBag.oneDay = oneDay;
            ViewBag.oneMonth = oneMonth;
            ViewBag.threeMonth = threeMonth;
            ViewBag.oneYear = oneYear;

            return PartialView(model);
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

        #endregion

    }
}
