using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using static NomadicBook.Utils.Global;
using NomadicBook.Models.db;

namespace NomadicBook.Utils
{
    public class Reptile : IReptile
    {
        public const string BOOKSTORE_URL = "https://search.books.com.tw";
        private static string STORE_URL = "http://www.ibon.com.tw";
        IConfiguration config;
        IBrowsingContext context;
        HttpClient httpClient;
        public Reptile()
        {
            config = Configuration.Default;
            context = BrowsingContext.New(config);
            httpClient = new HttpClient();
        }
        /// <summary>
        /// 取得網頁
        /// </summary>
        /// <param name="url">要取得網頁的網址</param>
        /// <returns>取得的網頁</returns>
        public IDocument GetWebPage(string url)
        {
            var responseMessage = httpClient.GetAsync(url).Result;
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
           {
                string responseResult = responseMessage.Content.ReadAsStringAsync().Result;
                var document = context.OpenAsync(res => res.Content(responseResult)).Result;
                return document;
            }
            return null;
        }
        /// <summary>
        /// 從博客來搜尋isbn結果的網頁中找到該書的產品業網址
        /// </summary>
        /// <param name="document">博客來搜尋isbn結果的網頁</param>
        /// <returns>該書的產品業網址</returns>
        public string GetHref(IDocument document)
        {
            var contents = document.QuerySelector("tbody>tr>td>h4>a");
            if (contents == null)
            {
                return null;
            }
            string bookUrl = $"https:{contents.GetAttribute("href")}";
            return bookUrl;
        }
        /// <summary>
        /// 獲得書本標題
        /// </summary>
        /// <param name="document">書的產品頁</param>
        /// <returns>標題</returns>
        public string GetTitle(IDocument document)
        {
            string path = "body > div.container_24.main_wrap.clearfix > div > div.mod.type02_p01_wrap.clearfix > div.grid_10 > div.mod.type02_p002.clearfix > h1";
            return document.QuerySelector(path).TextContent;
        }
        /// <summary>
        /// 有書基本資訊的html節點們
        /// </summary>
        /// <param name="path">節點路徑</param>
        /// <param name="document">要被搜索的網頁</param>
        /// <returns>html節點</returns>
        public IHtmlCollection<IElement> GetBookData(string path, IDocument document)
        {
            return document.QuerySelectorAll(path);
        }
        /// <summary>
        /// 從書的基本資訊獲得書的作者
        /// </summary>
        /// <param name="htmlNodes">含有基本資訊的html節點</param>
        /// <returns>作者</returns>
        public string GetAuthor(IHtmlCollection<IElement> htmlNodes)
        {
            int authorPosition = 0;
            var nodes = htmlNodes[authorPosition].QuerySelectorAll("a");
            string author = string.Empty;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].ParentElement.NodeName == "LI" && nodes[i].FirstChild.NodeName == "#text")
                {
                    author += $"{ nodes[i].TextContent},";
                }
            }
            int lastComma = author.Length - 1;
            return author.Remove(lastComma);
        }
        /// <summary>
        /// 從書的基本資訊獲得出版資訊
        /// </summary>
        /// <param name="htmlNodes">有基本資訊的html節點</param>
        /// <returns>出版社，出版日期</returns>
        public (string company, string date) GetPublishData(IHtmlCollection<IElement> htmlNodes)
        {
            string date = string.Empty;
            string company = string.Empty;
            for (int i = 1; i < htmlNodes.Length; i++)
            {
                string node = htmlNodes[i].FirstChild.NodeValue;
                if (node == "出版社：" || node == "原文出版社：")
                {
                    company = htmlNodes[i].QuerySelector("span").FirstChild.NodeValue;
                }
                if (node.Contains("出版日期："))
                {
                    int LastWord = 1;
                    date = node.Split("出版日期：")[LastWord];
                }
            }
            return (company, date.Replace("/","-"));
        }
        /// <summary>
        /// 從書的介紹的html節點獲得書的簡介
        /// </summary>
        /// <param name="htmlNodes">書的介紹的html節點</param>
        /// <returns>簡介</returns>
        public string GetIntroduction(IHtmlCollection<IElement> htmlNodes)
        {
            string introduction = string.Empty;
            for (int i = 0; i < htmlNodes.Length; i++)
            {
                if (htmlNodes[i].QuerySelector("h3") != null && htmlNodes[i].QuerySelector("h3").TextContent == "內容簡介")
                {
                    introduction = htmlNodes[i].QuerySelector("div > div.content").ToHtml();
                    break;
                }
            }
            return introduction;
        }
        /// <summary>
        /// 取得書的詳細資訊的節點
        /// </summary>
        /// <param name="document">書的產品頁</param>
        /// <returns>書的詳細資訊的節點</returns>
        public IElement GetBookDetail(IDocument document)
        {
            string path = "body > div.container_24.main_wrap.clearfix > div > div.mod.clearfix > div.grid_19.alpha > div.mod_b.type02_m058.clearfix > div";
            return document.QuerySelector(path);
        }
        /// <summary>
        /// 取得書的尺寸
        /// </summary>
        /// <param name="htmlNode">書的詳細資訊的節點</param>
        /// <returns>書的長，寬，高</returns>
        public (double bookLong, double bookWidth, double bookHigh) GetSize(IElement htmlNode)
        {
            var detailData = htmlNode.QuerySelectorAll("ul:nth-child(1) > li");
            double bookLong = 0;
            double bookWidth = 0;
            double bookHigh = 0;
            for (int i = 0; i < detailData.Count(); i++)
            {
                if (detailData[i].TextContent.Contains("規格："))
                {
                    var format = detailData[i].TextContent.Replace("規格：", string.Empty).Split("/");
                    for (int j = 0; j < format.Length; j++)
                    {
                        if (format[j].Contains("cm"))
                        {
                            var size = format[j].Replace("cm", string.Empty).Split("x");
                            bookLong = Convert.ToDouble(size[0]);
                            bookWidth = Convert.ToDouble(size[1]);
                            if (size.Length > 2)
                            {
                                bookHigh = Convert.ToDouble(size[2]);
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return (bookLong, bookWidth, bookHigh);
        }
        /// <summary>
        /// 取得書的類別
        /// </summary>
        /// <param name="htmlNode">詳細資訊的節點</param>
        /// <returns>書的id，書的類別名稱</returns>
        public (string categoryId, string categoryName) GetCategory(IElement htmlNode)
        {
            var category = htmlNode.QuerySelector(" ul.sort > li > a:nth-child(2)");
            if (category == null)
            {
                return (string.Empty, string.Empty);
            }
            string categoryId = category.GetAttribute("href").Split("_").Last().Replace("/", string.Empty);
            string categoryName = category.TextContent;
            return (categoryId, categoryName);
        }
        /// <summary>
        /// 用來取得BookCategory的資料
        /// </summary>
        /// <param name="path">博客來分類種覽的URL</param>
        /// <param name="mainId">要取得的大類ID，可見enum MainCategory</param>
        /// <returns>取得的中類名稱，小類名稱，及小類id</returns>
        public List<(string, string, string)> GetAllCategory(string path, byte mainId)
        {
            int first = 0;
            int idIsFour = 4;
            var document = GetWebPage(path);
            var nodes = document.QuerySelectorAll("body > div.container_24.main_wrap.clearfix > div > div.type02_s004.clearfix>div.mod_no.clearfix");
            List<(string, string, string)> list = new List<(string, string, string)>();
            foreach (var node in nodes)
            {
                string bigName = node.QuerySelector("h4").TextContent;
                var detailNodes = node.QuerySelectorAll(" table > tbody > tr > th > h5 > a");
                foreach (var detailNode in detailNodes)
                {
                    var detailUrls = detailNode.GetAttribute("href").Split("/?")[first];
                    string id = detailUrls.Substring(detailUrls.Length - idIsFour);
                    switch ((MainCategory)mainId)
                    {
                        case MainCategory.SimplifiedChinese:
                            id = $"midmechina{id}";
                            break;
                        case MainCategory.ForeignLanguage:
                            id = $"midmefbooks{id}";
                            break;
                    }
                    string detailName = detailNode.TextContent;
                    list.Add((id, bigName, detailName));
                }
            }
            return list;
        }
        /// <summary>
        /// 傳post要求給指定網頁
        /// </summary>
        /// <param name="requestUrl">網址</param>
        /// <param name="postParams">post的 formdata內容</param>
        /// <returns>網頁回應訊息</returns>
        public async Task<string> HtmlPost(string requestUrl, Dictionary<string, string> postParams)
        {
            string responseBody = string.Empty;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Method", "Post");
                httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
                httpClient.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11");
                HttpContent postContent = new FormUrlEncodedContent(postParams);
                HttpResponseMessage response = await httpClient.PostAsync(requestUrl, postContent);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            return responseBody;
        }
        /// <summary>
        /// 取得7-11的所有店家資料
        /// </summary>
        /// <returns>店家陣列</returns>
        public List<ConvenienceStore> GetStore()
        {
            int header = 1;
            int worst = 2;
            int shopId = 0;
            int shopName = 1;
            int shopAddress = 2;
            List<ConvenienceStore> storeList = new List<ConvenienceStore>();
            List<(string cityTag, string cityName)> cities = GetCity();
            foreach (var city in cities)
            {
                List<(string areaTag, string areaName)> areas = GetArea(city.cityTag);
                Dictionary<string, string> areaResults = new Dictionary<string, string>();
                foreach (var area in areas)
                {
                    if (areaResults.ContainsKey(area.areaTag))///因為不同區域有相同的區域代號，要把代號相同的區域名組再一起
                    {
                        areaResults[area.areaTag] = $"{areaResults[area.areaTag]}、{area.areaName}";
                    }
                    else
                    {
                        areaResults.Add(area.areaTag, area.areaName);
                    }
                }
                foreach (var area in areaResults)
                {
                    var document = GetDocument("ZIPCODE", area.Key);
                    var nodes = document.QuerySelectorAll("table > tbody > tr");
                    var stores = nodes.Skip(header);//第一筆是店名等表頭，是不需要的資料
                    foreach (var store in stores)
                    {
                        var storeData = store.TextContent.Trim().Split("    ");
                        if (storeData.Length > worst)//切開後陣列小於2的應該是查無資料
                        {
                            storeList.Add(new ConvenienceStore
                            {
                                ShopId = Int32.Parse(storeData[shopId].Trim()),//第一個值會是店家ID
                                ShopCity = city.cityName,
                                ShopArea = area.Value,
                                ShopName = storeData[shopName].Trim(),//第二個值會是店名
                                ShopAddress = storeData[shopAddress].Trim(),//第二個值會是店家地址
                            });
                        }
                    }
                }
            }
            return storeList;

        }
        /// <summary>
        /// 取得含7-11的門市資訊網頁內容
        /// </summary>
        /// <param name="field">要針對區域ID查資料時，用ZIPCODE，針對城市查時用COUNTY</param>
        /// <param name="keyWord">城市時填城市名，區域填區域代號</param>
        /// <returns>含7-11的門市資訊網頁內容</returns>
        public IDocument GetDocument(string field, string keyWord)
        {
            string responseStr = string.Empty;
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            postParams.Add("strTargetField", field);
            postParams.Add("strKeyWords", keyWord);
            responseStr = HtmlPost($"{STORE_URL}/retail_inquiry_ajax.aspx", postParams).Result;
            var document = context.OpenAsync(res => res.Content(responseStr)).Result;
            return document;
        }
        /// <summary>
        /// 取得7-11的所有城市
        /// </summary>
        /// <returns>城市名，及搜索區域時的城市標籤</returns>
        public List<(string, string)> GetCity()
        {
            var document = GetWebPage($"{STORE_URL}/retail_inquiry.aspx#gsc.tab=0");
            var elements = document.QuerySelectorAll("#Class1 > option");
            List<(string, string)> cities = new List<(string, string)>();
            int last = 1;
            var nodes = elements.SkipLast(last);//最後一筆是南海諸島，這沒有資料
            foreach (var node in nodes)
            {
                cities.Add((node.GetAttribute("value"), node.TextContent));
            }
            return cities;
        }
        /// <summary>
        /// 依城市取得該城市的所有區域資訊
        /// </summary>
        /// <param name="city">城市</param>
        /// <returns>區域名，及搜索店家時的區域標籤</returns>
        public List<(string, string)> GetArea(string city)
        {
            var document = GetWebPage($"{STORE_URL}/retail_inquiry_control.aspx?addressinfo={city}&ctrlId=2&parentCtrlId=1&treeDeep=2");
            List<(string, string)> areas = new List<(string, string)>();
            var nodes = document.Scripts.First().TextContent.Split(";");
            int start = 3;//第一筆是所有區域不需要，第二筆不含需要的值
            int jump = 2;//每兩筆，一筆不含需要的值，所以跳過一筆
            int first = 0;//第一筆，有區域標籤
            int second = 1;//第二筆，有區域名
            for (int i = start; i < nodes.Length - 1; i += jump)
            {
                var area = nodes[i].Split(",")[second].Split("\"")[second].Split("*");
                areas.Add((area[first], area[second]));
            }
            return areas;
        }

    }
}
