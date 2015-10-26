<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MoneyNovel.Models.TransactionModel>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>StockInfo</title>

</head>
<body>
    <div>
            
            <h1 id="name" style="text-align:center;"> <%:ViewBag.stock.Name%> - <%:ViewBag.stock.Ticker%> </h1>
            <div style="height: 36px; margin-left: 200px">
                <button class="btn btn-info btn-xs" value=<%:ViewBag.oneDay %> onclick=swapPic(this.value) type="button">1d</button>
                <button class="btn btn-info btn-xs" value=<%:ViewBag.oneMonth %> onclick=swapPic(this.value) type="button">1m</button>
                <button class="btn btn-info btn-xs" value=<%:ViewBag.threeMonth %> onclick=swapPic(this.value) type="button">3m</button>
                <button class="btn btn-info btn-xs" value=<%:ViewBag.oneYear %> onclick=swapPic(this.value) type="button">1y</button>
            </div>
            <img id="img" style="display:block; margin-left:auto; margin-right:auto" src="<%:ViewBag.oneDay%>" />
            <br />
            <br />
           <table class="table table-bordered table-hover">

                <thead>
                    <tr>
                        <th>Ticker</th>
                        <th>Current Price</th>
                        <th>High Price</th>
                        <th>Opening Price</th>
                    </tr>
                </thead>
                <tbody>
                   <% if(ViewBag.stock.CurrentPrice >= ViewBag.stock.OpeningPrice)
                       { %>
                           <tr class="stockrow">
                              <th><%: ViewBag.stock.Ticker  %></th>
                              <th style="color:green">$<%: ViewBag.stock.CurrentPrice  %></th>
                              <th>$<%: ViewBag.stock.HighPrice  %></th>
                              <th>$<%: ViewBag.stock.OpeningPrice  %></th>
                           </tr>
                       <%}
                       else
                       { %>
                           <tr class="stockrow">
                              <th><%: ViewBag.stock.Ticker  %></th>
                              <th style="color:red">$<%: ViewBag.stock.CurrentPrice  %></th>
                              <th>$<%: ViewBag.stock.HighPrice  %></th>
                              <th>$<%: ViewBag.stock.OpeningPrice  %></th>
                           </tr>
                       <%}%>
              </tbody>

        </table>
        <div style="display:inline-block">
            <div style="display:inline-block; float:right; width: 349px; margin-left: 34px;">
                <textarea id="Notes" class="notes" runat="server" style="width:100%"><%: ViewBag.stock.Comment %></textarea>
                <br />
                <br />
                <button class="btn btn-info" type="submit" onclick="saveComment()" >Save Comment</button>
                <br />
            </div>
            <div style="display:inline-block; float:left">
                
                <section id="buyForm">
                    <% using (Html.BeginForm("BuyStock", "Stocks")) { %>
                        <%: Html.AntiForgeryToken() %>
                        <%: Html.ValidationSummary(true) %>
                        <%: Html.HiddenFor(m => m.Ticker) %>

                        <fieldset>
                            <legend>Buy Stock Form</legend>
                            <ol>
                                <li>
                                    <%: Html.LabelFor(m => m.Amount) %>
                                    <%: Html.TextBoxFor(m => m.Amount) %>
                                    <%: Html.ValidationMessageFor(m => m.Amount) %>
                                </li>
                            </ol>
                            <button class="btn btn-success" type="submit" value="Buy">Buy</button>
                        </fieldset>
                    <% } %>
                    </section>

                   <section id="sellForm">
                    <% using (Html.BeginForm("SellStock", "Stocks")) { %>
                        <%: Html.AntiForgeryToken() %>
                        <%: Html.ValidationSummary(true) %>
                        <%: Html.HiddenFor(m => m.Ticker) %>

                        <fieldset>
                            <legend>Buy Stock Form</legend>
                            <ol>
                                <li>
                                    <%: Html.LabelFor(m => m.Amount) %>
                                    <%: Html.TextBoxFor(m => m.Amount) %>
                                    <%: Html.ValidationMessageFor(m => m.Amount) %>
                                </li>
                            </ol>
                            <button class="btn btn-danger" type="submit" value="Sell">Sell</button>
                        </fieldset>
                    <% } %>
                    </section>
            </div>
        </div>
        </div>
        <script>

            function swapPic(pic_src) {

                document.getElementById("img").src = pic_src;
            }

            function saveComment() {
                var com = document.getElementById("Notes").value;

                var info = {
                    comment: com,
                    ticker: document.getElementById("name").textContent
                }

                $.ajax({
                    url: "<%= Url.Action("SaveComment", "Stocks") %>",
                    data: info,
                    complete: function () {
                        updateLeft();
                    }
                });

            }
            // Populate the left side of the page when a stock row is clicked
            function updateLeft() {
                $.ajax({
                    cache: false,
                    async: true,
                    type: "GET",
                    url: "<%= Url.Action("StockList", "Stocks") %>",
                    success: function () {
                        $('#leftSide').html();
                    }
                });
            }
    </script>
</body>
</html>
