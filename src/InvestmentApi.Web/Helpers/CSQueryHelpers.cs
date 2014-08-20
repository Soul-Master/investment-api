using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using CsQuery;
using NPOI.SS.UserModel;

namespace InvestmentApi.Web.Helpers
{
    public static class CsQueryHelpers
    {
        public static string Text(this IDomObject obj)
        {
            var text = obj.InnerText;

            if(text == null) return null;
            text = HttpUtility.HtmlDecode(text.Trim());

            return text == "-" ? null : text.TrimText();
        }

        public static CQ GetUrl(string url, Encoding encoder)
        {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            
            if(stream == null) return null;
            
            var reader = new StreamReader(stream, encoder);
            {
                var responseFromServer = reader.ReadToEnd();
                
                return CQ.Create(responseFromServer);
            }
        }

        public static CQ PostToUrl(string url, Encoding encoder, List<KeyValuePair<string, string>> formData = null)
        {
            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";

            if (formData != null)
            {
                var postData = new StringBuilder();

                foreach (var item in formData)
                {
                    postData.Append(HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&");
                }

                postData.Remove(postData.Length - 1, 1);
                request.ContentLength = postData.Length;

                var ascii = new ASCIIEncoding();
                var postBytes = ascii.GetBytes(postData.ToString());
                var postStream = request.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
                postStream.Close();
            }

            var response = request.GetResponse();
            var stream = response.GetResponseStream();

            if (stream == null) return null;

            var reader = new StreamReader(stream, encoder);
            {
                var responseFromServer = reader.ReadToEnd();

                return CQ.Create(responseFromServer);
            }
        }
    }
}
