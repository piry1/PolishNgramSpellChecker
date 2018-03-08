using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;

namespace PolishNgramSpellChecker.HttpApi.Server
{
    public class HttpServer
    {
        private static readonly HttpListener HttpListener = new HttpListener();
        private static Thread _responseThread;
        private static readonly Router Router = new Router();
        public static int Port { get; } = 5432;
        public static string Url { get; } = $"http://localhost:{Port}/";

        public static void StartServer()
        {
            Console.WriteLine("Starting server...");

            try
            {
                HttpListener.Prefixes.Add(Url);
                //HttpListener.Prefixes.Add("http://*:5432/");
                HttpListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Server started at " + Url);
            _responseThread = new Thread(ResponseThread);
            _responseThread.Start();
        }

        public static void StopServer()
        {
            _responseThread.Abort();
        }

        private static void ResponseThread()
        {
            while (true)
            {
                try
                {
                    var context = HttpListener.GetContext();
                    context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    QueueUserWorkItem(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void QueueUserWorkItem(HttpListenerContext context)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var request = ExtractRequest(context);

                    HttpResponse ret = Router.RouteApiMethod(request);

                    //context.Request.InputStream.rea
                    context.Response.ContentType = ret.Type;
                    context.Response.OutputStream.Write(ret.Content, 0, ret.Content.Length);
                    context.Response.StatusCode = ret.Code;
                }
                catch (Exception e)
                {
                    var errorRes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                    context.Response.OutputStream.Write(errorRes, 0, errorRes.Length);
                    context.Response.StatusCode = 500;
                }

                context.Response.KeepAlive = false;
                context.Response.Close();
            });
        }

        private static Request ExtractRequest(HttpListenerContext context)
        {
            byte[] bytes = new byte[1024];
            var count = context.Request.InputStream.Read(bytes, 0, bytes.Length);
            var url = context.Request.Url;

            return new Request
            {
                Body = Encoding.UTF8.GetString(bytes, 0, count),
                Controller = url.Segments[1].Replace("/", ""),
                Method = url.Segments[2].Replace("/", ""),
                Params = url.Segments.Skip(3).Select(s => s.Replace("/", "")).ToArray()
            };
        }
    }
}