using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using SearchTrade.WebAPI.Helpers;
using SearchTrade.WebAPI.RestSharp;
using SearchTrade.WebAPI.ViewModels;
using System.Xml;

namespace SearchTrade.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        [HttpGet("SearchAllData")]
        public async Task<IActionResult> GetAllData(string word, int? page=1)
        {
            try
            {
                if (page != null && page < 1)
                {
                    page = 1;
                }

                var pageSize = 100;
                string apiResponse = string.Empty;

                //I have send static value to "S" parameter. This parameter is value of text which we are searching for.
                //But in this website changes value of my text and sends that parameter 

                //For example ,
                // 86e4a92e-62e6-4715-a925-1cea2105e0ad  = 'abc'

                string defaultWord = "86e4a92e-62e6-4715-a925-1cea2105e0ad"; //abc

                var url = $"https://search.ipaustralia.gov.au/trademarks/search/result?s={defaultWord}";

                //if page is bigger than 1  then we must send P parameter
                if (page > 1)
                {
                    url = $"https://search.ipaustralia.gov.au/trademarks/search/result?s={defaultWord}&p={page - 1}";
                }

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(
                               $"{url}"))
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                }

                // This is Web Scraping
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(apiResponse);

                //we have only one table
                HtmlNode table =
                    doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'fetch-table trademark-list')]");

                var data = new List<TableDataVM>();
                int countData = 0;
                //But many tbody tags
                foreach (var item in table.SelectNodes("./tbody"))
                {

                    //for substring status 1 and status 2 by :  characters
                    var statusIndex = item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'status')]").InnerText.ToString().IndexOf(':');


                    var response = new TableDataVM()
                    {
                        No = ++countData,
                        //number
                        Number = item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'number')]/a[contains(@class, 'number qa-tm-number')]").InnerText,

                        //trademark words
                        TradeMark = item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'trademark words')]").InnerText,

                        // trademark image 
                        LogoUrl = item.SelectSingleNode("./tr")
                                        .SelectSingleNode("./td[contains(@class, 'trademark image')]") != null ?
                                        $"https://search.ipaustralia.gov.au/" + item.SelectSingleNode("./tr")
                                        .SelectSingleNode("./td[contains(@class, 'trademark image')]")
                                        .SelectSingleNode("//img").Attributes["src"].Value : "",

                        //classes 
                        Classes = item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'classes ')]").InnerText,

                        //status 1                        
                        Status1 = (statusIndex != -1) ? item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'status')]").InnerText.Substring(0, statusIndex) : item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'status')]").InnerText,

                        //status 2
                        Status2 = (statusIndex != -1) ? item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'status')]").InnerText.Substring(statusIndex + 1) : item.SelectSingleNode("./tr")
                                    .SelectSingleNode("./td[contains(@class, 'status')]").InnerText,

                        //data-markurl attribute
                        DetailsPageUrl = $"https://search.ipaustralia.gov.au/" + item.SelectSingleNode("./tr").Attributes["data-markurl"].Value
                    };

                    //Finally, we add each of them to list
                    data.Add(response);
                };


                // We send all count of data 
                var allCount = await GetDataCount(word);
                var result = new PaginatedList<TableDataVM>(data, allCount, allCount, page ?? 1, 100);

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }  
        }

        [HttpGet("GetAllDataCount")]
        public async Task<IActionResult> GetAllDataCount(string word)
        {
            return Ok(await GetDataCount(word));
        }

        [NonAction]
        public async Task<int> GetDataCount(string word)
        {
            // the value we send to the last parameter is the value of the text we're searching for

            //5D =abc

            var fullApiUrl = $"https://search.ipaustralia.gov.au/trademarks/search/count?_csrf=a421c726-84a4-4014-90b7-4cb3546313d8&_sw=on&ct=A&dateType=LODGEMENT_DATE&ieOp%5B0%5D=AND&ieOp%5B1%5D=AND&irOp=AND&it%5B0%5D=PART&it%5B1%5D=PART&it%5B2%5D=PART&it%5B3%5D=PART&nameField%5B0%5D=OWNER&undefined=false&weOp%5B0%5D=AND&weOp%5B1%5D=AND&wps=false&wrOp=AND&wt%5B0%5D=PART&wt%5B1%5D=PART&wt%5B2%5D=PART&wt%5B3%5D=PART&wv%5B0%5D={word}";

            var Client = new RestClient(fullApiUrl);

            var request = new RestRequest("", Method.Get);
            request.AddHeader("Content-Type", "application/json");

            var result = (await Client.ExecuteAsync<CountData>(request)).Data;

            if (result != null)
            {
                return result.count;
            }
            return 0;
        }
    }
}
