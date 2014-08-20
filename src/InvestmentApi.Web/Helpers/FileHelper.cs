using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace InvestmentApi.Web.Helpers
{
    public static class FileHelper
    {
        public static string DownloadFile(string url, string saveFilePath = null)
        {
            var client = new WebClient();
            var tempfile = saveFilePath ?? Path.GetTempFileName();

            client.DownloadFile(new Uri(url), tempfile);

            return tempfile;
        }
    }
}