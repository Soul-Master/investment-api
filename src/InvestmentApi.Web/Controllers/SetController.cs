using System;
using System.Collections.Generic;
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
        /// สัญลักษณ์บริษัทจดทะเบียนในตลาดหลักทรัพย์ (ปรับปรุงข้อมูลทุกวันทำการแรกของแต่ละสัปดาห์)
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
    }

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
}
