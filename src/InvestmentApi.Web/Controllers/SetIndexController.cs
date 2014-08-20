using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Antlr.Runtime;
using CsQuery;
using InvestmentApi.Web.Helpers;

namespace InvestmentApi.Web.Controllers
{
    [RoutePrefix("Set/Index")]
    public class SetIndexController : ApiController
    {
        /// <summary>
        /// รายชื่อหลักทรัพย์ที่ใช้คำนวณดัชนี SET50 และ SET100
        /// </summary>
        [HttpGet]
        [Route("Set50List")]
        public IEnumerable<object> Set50List()
        {
            var dom = CsQueryHelpers.GetUrl("http://www.set.or.th/th/market/constituents.html", Encoding.GetEncoding("tis-620"));
            var ul = dom.Select("li ul:eq(0)");
            var children = ul.Children();

            for (var i = 0; i < children.Length; i++)
            {
                var li = CQ.Create(children[i]);

                yield return li.Text().Trim();
            }
        }
    }
}