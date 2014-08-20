using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Antlr.Runtime;
using CsQuery;
using InvestmentApi.Web.Helpers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace InvestmentApi.Web.Controllers
{
    [RoutePrefix("Set/Index")]
    public class SetIndexController : ApiController
    {
        /// <summary>
        /// วันที่หลักทรัพย์ถูกใช้คำนวณดัชนี SET50
        /// </summary>
        [HttpGet]
        [Route("Set50ListedDate")]
        public IEnumerable<DateRange> Set50ListedDate()
        {
            var thaiCulture = new CultureInfo("th-TH");
            var dom = CsQueryHelpers.GetUrl("http://www.set.or.th/th/market/constituents.html",
                Encoding.GetEncoding("tis-620"));
            var ul = dom.Select("li ul:eq(0)");
            var children = ul.Children();

            for (var i = 0; i < children.Length; i++)
            {
                var li = CQ.Create(children[i]);
                var text = li.Text().Replace("ระหว่าง", "").Trim();
                var tokens = text.Split(new[] {"ถึง"}, StringSplitOptions.RemoveEmptyEntries);
                var link = li.Select("a");
                var model = new DateRange
                {
                    StartDate = DateTime.ParseExact(tokens[0].Trim(), "d MMM yyyy", thaiCulture),
                    EndDate = DateTime.ParseExact(tokens[1].Trim(), "d MMM yyyy", thaiCulture),
                    DownloadLink = "http://www.set.or.th/th/market/" + link.Attr("href").Trim()
                };

                yield return model;
            }
        }

        /// <summary>
        /// วันที่หลักทรัพย์ถูกใช้คำนวณดัชนี SET100
        /// </summary>
        [HttpGet]
        [Route("Set100ListedDate")]
        public IEnumerable<DateRange> Set100ListedDate()
        {
            return Set50ListedDate();
        }

        /// <summary>
        /// รายชื่อหลักทรัพย์ที่ใช้คำนวณดัชนี SET50
        /// </summary>
        [HttpGet]
        [Route("Set50")]
        public IEnumerable<IndexCompanyInfo> Set50(DateTime? listedDate = null)
        {
            var availableDates = Set50ListedDate();
            listedDate = DateTime.Now;

            var selectedRange = availableDates
                .Where(x => listedDate >= x.StartDate && listedDate <= x.EndDate)
                .OrderByDescending(x => x.StartDate)
                .First();

            using (var temp = new TempFile())
            {
                FileHelper.DownloadFile(selectedRange.DownloadLink, temp.Path);
                var wordbook = NPOIHelpers.OpenWorkbook(temp.Path);

                var sheet = wordbook.GetSheet("Set50-TH");
                for (var i = 3; i < 53; i++)
                {
                    var row = sheet.GetRow(i);
                    var model = new IndexCompanyInfo
                    {
                        Symbol = row.GetCellText(1),
                        Name = row.GetCellText(2),
                        BusinessSector = row.GetCellText(3),
                        IndustryGroup = row.GetCellText(4),
                        Comment = row.GetCellText(5)
                    };

                    yield return model;
                }
            }
        }
        
        /// <summary>
        /// สัญลักษณ์ของรายชื่อหลักทรัพย์ที่ใช้คำนวณดัชนี SET50
        /// </summary>
        [HttpGet]
        [Route("Set50Symbols")]
        public HttpResponseMessage Set50Symbols(DateTime? listedDate = null)
        {
            var result = Set50(listedDate).Select(x => x.Symbol).Aggregate(string.Empty, (current, item) =>
            {
                if (current == string.Empty) return item;

                return current + Environment.NewLine + item;
            });

            return new HttpResponseMessage
            {
                Content = new StringContent(result, Encoding.UTF8, "text/plain")
            };
        }

        /// <summary>
        /// รายชื่อหลักทรัพย์ที่ใช้คำนวณดัชนี Set100
        /// </summary>
        [HttpGet]
        [Route("Set100")]
        public IEnumerable<IndexCompanyInfo> Set100(DateTime? listedDate = null)
        {
            var availableDates = Set100ListedDate();
            listedDate = DateTime.Now;

            var selectedRange = availableDates
                .Where(x => listedDate >= x.StartDate && listedDate <= x.EndDate)
                .OrderByDescending(x => x.StartDate)
                .First();

            using (var temp = new TempFile())
            {
                FileHelper.DownloadFile(selectedRange.DownloadLink, temp.Path);
                var wordbook = NPOIHelpers.OpenWorkbook(temp.Path);

                var sheet = wordbook.GetSheet("Set100-TH");
                for (var i = 3; i < 103; i++)
                {
                    var row = sheet.GetRow(i);
                    var model = new IndexCompanyInfo
                    {
                        Symbol = row.GetCellText(1),
                        Name = row.GetCellText(2),
                        BusinessSector = row.GetCellText(3),
                        IndustryGroup = row.GetCellText(4),
                        Comment = row.GetCellText(5)
                    };

                    yield return model;
                }
            }
        }

        /// <summary>
        /// สัญลักษณ์ของรายชื่อหลักทรัพย์ที่ใช้คำนวณดัชนี SET100
        /// </summary>
        [HttpGet]
        [Route("Set100Symbols")]
        public HttpResponseMessage Set100Symbols(DateTime? listedDate = null)
        {
            var result = Set100(listedDate).Select(x => x.Symbol).Aggregate(string.Empty, (current, item) =>
            {
                if (current == string.Empty) return item;

                return current + Environment.NewLine + item;
            });

            return new HttpResponseMessage
            {
                Content = new StringContent(result, Encoding.UTF8, "text/plain")
            };
        }
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        internal string DownloadLink { get; set; }
    }

    /// <summary>
    ///  ข้อมูลบริษัทจดทะเบียนในตลาดหลักทรัพย์
    /// </summary>
    public class IndexCompanyInfo
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string IndustryGroup { get; set; }
        public string BusinessSector { get; set; }
        public string Comment { get; set; }

        public bool IsNewEntry
        {
            get { return Comment == "New Entry"; }
        }
    }
}