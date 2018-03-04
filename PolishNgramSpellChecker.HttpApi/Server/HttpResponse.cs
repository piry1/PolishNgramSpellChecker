using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PolishNgramSpellChecker.HttpApi.Server
{
    public class HttpResponse
    {
        public string Type { get; set; }
        public byte[] Content { get; set; }
        public int Code { get; set; } = 200;

        public static HttpResponse ReturnJson(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var response = new HttpResponse
            {
                Type = "text/json",
                Content = Encoding.UTF8.GetBytes(json)
            };
            return response;
        }

        public static HttpResponse ReturnPage(string path)
        {
            byte[] content;

            try
            {
                content = File.ReadAllBytes(path);
            }
            catch
            {
                content = Encoding.UTF8.GetBytes("Page not found");
            }

            var response = new HttpResponse
            {
                Type = "text/html",
                Content = content
            };
            return response;
        }
    }
}