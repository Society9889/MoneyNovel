using MoneyNovel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Microsoft.Web.WebPages.OAuth;

namespace MoneyNovel.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            
            getTopFiveStocks();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void getTopFiveStocks()
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

                foreach (KeyValuePair<string, List<StockTransaction>> pair in transactionDictonary)
                {
                    Stock stock = new Stock();
                    stock.Ticker = pair.Key;
                    stock.CurrentPrice = getOneStock(pair.Key).CurrentPrice;
                    stock.Shares = 0;

                    //add up total shares and investment using stocktransaction history
                    foreach (StockTransaction action in pair.Value)
                    {
                        stock.Shares += action.Shares;
                    }

                    stock.StockValue = Math.Round(stock.Shares * stock.CurrentPrice, 2);

                    viewStockList.Add(stock);
                }

            }

            for (int i = 1; i < viewStockList.Count; i++) // Iterate beginning at 1, because we assume that 0 is already sorted
            {
                for (int j = i; j > 0; j--) // Iterate backwards, starting from 'i'
                {
                    Stock cur = viewStockList[j - 1];
                    Stock tbs = viewStockList[j]; // 'tbs' == "to be sorted"
                    if (cur.StockValue < tbs.StockValue) // usually, classes that implement 'CompareTo()' also implement 'operator <()', 'operator >()' and 'operator ==()', so you could have just written 'cur < tbs'
                    {
                        Stock temp = viewStockList[j];
                        viewStockList[j] = viewStockList[j - 1];
                        viewStockList[j - 1] = temp;
                    }
                    else
                        break; // since 'tbs' is no longer > 'cur', it is part of our sorted list. We don't need to sort that particular 'tbs' any further
                }
            }

            ViewBag.stockList = viewStockList.Take(5);
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

            if (!string.IsNullOrWhiteSpace(result))
            {
                String sn = result.Replace("\"", "").Replace(", ", " ");
                string[] stockInfo = sn.Split(',');
                if (!sn.Contains("N/A") && stockInfo.Length > 4)
                {
                    stock.Ticker = stockInfo[1];
                    stock.CurrentPrice = Convert.ToDouble(stockInfo[2]);
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

        public string getStocks()
        {
            //Currently hard coded from viewing a list of stocks
            String url = "http://download.finance.yahoo.com/d/quotes.csv?s=";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "GOOG,MSFT,INTU,GERN,AAPL&f=nsl1h0o&e=.csv");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        public ActionResult Friends()
        {
            return View();
        }
    }
}
