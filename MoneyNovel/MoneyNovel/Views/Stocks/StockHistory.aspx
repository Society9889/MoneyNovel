<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>StockHistoryList</title>
</head>
<body>
    <div>

        <table class="table table-bordered table-hover">

            <thead>
                <tr>
                    <th>Ticker</th>
                    <th>Transaction Type</th>
                    <th>Price</th>
                    <th>Shares</th>
                </tr>
            </thead>

            <tbody>
                <%foreach(var stock in ViewBag.slist){%>
                    <tr class="stockrow">
                        <th><%: stock.Ticker %></th>
                        <% if (stock.Price < 0){ %>
                             <th>Buy</th>
                            <% }else{ %>
                             <th>Sell</th>
                             <% } %>
                        <th>$<%: Math.Abs(stock.Price) %></th>
                        <th><%: Math.Abs(stock.Shares) %></th>
                    </tr>
                    <%} %>
            </tbody>

        </table>

    </div>
</body>
</html>
