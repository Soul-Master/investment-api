using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using CsQuery;
using InvestmentApi.Web.Helpers;

namespace InvestmentApi.Web.Controllers
{
    [RoutePrefix("Set")]
    public class SetController : ApiController
    {
        /// <summary>
        /// รายชื่อบริษัทจดทะเบียนในตลาดหลักทรัพย์ (ปรับปรุงข้อมูลทุกวันทำการแรกของแต่ละสัปดาห์)
        /// </summary>
        [HttpGet]
        [Route("ListedCompanies")]
        public IEnumerable<CompanyInfo> ListedCompanies()
        {
            var dom = CsQueryHelpers.GetUrl("http://www.set.or.th/listedcompany/static/listedCompanies_th_TH.xls", Encoding.GetEncoding("tis-620"));
            var rows = dom.Select("tr");

            for (var i = 2; i < rows.Length; i++)
            {
                var td = CQ.Create(rows[i]).Select("td");
                var model = new CompanyInfo
                {
                    Symbol = td[0].Text(),
                    Name = td[1].Text(),
                    MarketName = td[2].Text(),
                    IndustryGroup = td[3].Text(),
                    BusinessSector = td[4].Text(),
                    Address = td[5].Text(),
                    PostCode = td[6].Text(),
                    TelephoneNo = td[7].Text(),
                    FaxNo = td[8].Text(),
                    WebSite = td[9].Text(),
                };

                yield return model;
            }
        }

        /// <summary>
        /// สัญลักษณ์ของบริษัทจดทะเบียนในตลาดหลักทรัพย์ (ปรับปรุงข้อมูลทุกวันทำการแรกของแต่ละสัปดาห์)
        /// </summary>
        [HttpGet]
        [Route("ListedCompanySymbols")]
        public HttpResponseMessage ListedCompanySymbols()
        {
            var result = ListedCompanies().Select(x => x.Symbol).Aggregate(string.Empty, (current, item) =>
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
        /// รายชื่อบริษัทจดทะเบียนที่ถูกเพิกถอนในตลาดหลักทรัพย์
        /// </summary>
        [HttpGet]
        [Route("DelistedCompanies")]
        public IEnumerable<DelistedCompanyInfo> DelistedCompanies()
        {
            var dom = CsQueryHelpers.GetUrl("http://www.set.or.th/listedcompany/static/delistedSecurities_th_TH.xls", Encoding.GetEncoding("tis-620"));
            var rows = dom.Select("tr");
            var thaiCulture = new CultureInfo("th-TH");

            for (var i = 2; i < rows.Length; i++)
            {
                var td = CQ.Create(rows[i]).Select("td");
                var model = new DelistedCompanyInfo
                {
                    Symbol = td[0].Text(),
                    Name = td[1].Text(),
                    FirstTradingDate = DateTime.ParseExact(td[2].Text(), "dd MMM yyyy", thaiCulture),
                    DelistedDate = DateTime.ParseExact(td[3].Text(), "dd MMM yyyy", thaiCulture),
                    MarketName = td[4].Text(),
                    IndustryGroup = td[5].Text(),
                    BusinessSector = td[6].Text(),
                    DelistedReason = td[7].Text()
                };

                yield return model;
            }
        }

        /// <summary>
        /// สัญลักษณ์ของบริษัทจดทะเบียนที่ถูกเพิกถอนในตลาดหลักทรัพย์
        /// </summary>
        [HttpGet]
        [Route("DelistedCompanySymbols")]
        public HttpResponseMessage DelistedCompanySymbols()
        {
            var result = DelistedCompanies().Select(x => x.Symbol).Aggregate(string.Empty, (current, item) =>
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

    /// <summary>
    ///  ข้อมูลบริษัทจดทะเบียนในตลาดหลักทรัพย์
    /// </summary>
    public class CompanyInfo
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string MarketName { get; set; }
        public string IndustryGroup { get; set; }
        public string BusinessSector { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string WebSite { get; set; }
    }

    /// <summary>
    ///  ข้อมูลบริษัทที่ถูกเพิกถอนในตลาดหลักทรัพย์
    /// </summary>
    public class DelistedCompanyInfo
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string MarketName { get; set; }
        public string IndustryGroup { get; set; }
        public string BusinessSector { get; set; }
        public DateTime FirstTradingDate { get; set; }
        public DateTime DelistedDate { get; set; }
        public string DelistedReason { get; set; }
    }
}
