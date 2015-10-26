<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>StocksList</title>
</head>
<body>
    <div>

        <table id="watchedStocks" class="table table-bordered table-hover">

            <thead>
                <tr>
                    <th>Ticker</th>
                    <th>Current Price</th>
                    <th>Shares</th>
                    <th>Total Investment</th>
                    <th>Stock Value</th>
                    <th>Net Worth</th>
                </tr>
            </thead>

            <tbody>
                <% foreach (var stock in ViewBag.sList)
                   {
                     %>
                <tr onclick="stockSelected('<%: stock.Ticker  %>')">
                    <th><%: stock.Ticker  %></th>
                    <% if (stock.CurrentPrice >= stock.OpeningPrice)
                       { %>
                        <th style="color: green">$<%: stock.CurrentPrice  %></th>
                      <%}
                    else
                    {%>
                        <th style="color: red">$<%: stock.CurrentPrice  %></th>
                    <%}%>
                    <th><%: stock.Shares  %></th>
                    <% if (stock.Investment >= 0)
                       {%>
                    <th style="color: green">$<%: stock.Investment %></th>
                    <%}
                       else
                       { %>
                    <th style="color: red">$<%: stock.Investment %></th>
                    <% } %>

                    <% if (stock.StockValue >= 0)
                       {%>
                    <th style="color: green">$<%: stock.StockValue %></th>
                    <%}
                       else
                       { %>
                    <th style="color: red">$<%: stock.StockValue %></th>
                    <% } %>

                    <% if (stock.NetWorth >= 0)
                       {%>
                    <th style="color: green">$<%: stock.NetWorth %></th>
                    <%}
                       else
                       { %>
                    <th style="color: red">$<%: stock.NetWorth %></th>
                    <% } %>
                </tr>
                <%}%>
            </tbody>

        </table>

    </div>
</body>
</html>

<script>

    // Populate the right side of the page when a stock row is clicked
    function stockSelected(stockTicker) {
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "<%= Url.Action("StockInfo", "Stocks") %>",
            data: { name: stockTicker },
            success: function (data) {
                $('#rightSide').html(data);
            }
        });
    }

    $('#watchedStocks').on('click', 'tbody tr', function (event) {
        $(this).addClass('highlight').siblings().removeClass('highlight');
    });


</script>