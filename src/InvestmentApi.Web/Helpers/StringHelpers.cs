using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace InvestmentApi.Web.Helpers
{
    public static class StringHelpers
    {
        public static string TrimText(this string text)
        {
            text = text.Trim();

            return text != "" ? text : null;
        }
    }
}