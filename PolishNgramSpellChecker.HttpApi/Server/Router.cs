using System;
using System.Linq;
using System.Reflection;
using PolishNgramSpellChecker.HttpApi.Server.Controllers;

namespace PolishNgramSpellChecker.HttpApi.Server
{
    public class Router
    {
        //private DesktopController desktop = new DesktopController();
        //private IconController icon = new IconController();
        //private FileController file = new FileController();
        //private DatabaseController database = new DatabaseController();
        private PageController page = new PageController();
        private SpellCheckerController spellchecker = new SpellCheckerController();

        public HttpResponse RouteApiMethod(Request request)
        {
            var tmp = request.Params.ToList();
            tmp.Insert(0, request.Body);
            request.Params = tmp.ToArray();
            return RouteApiMethod(request.Controller, request.Method, request.Params);
        }

        public HttpResponse RouteApiMethod(string controllerName, string methodName, string[] strParams)
        {
            MethodInfo method;
            Object param;
            object[] @params;

            try
            {
                param = GetType().GetField(controllerName, BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(this);

                method = param?.GetType()
                                    .GetMethods()
                                    .First(m => m.Name.ToLower() == methodName && m.GetParameters().Count() == strParams.Count());

                @params = method?.GetParameters()
                                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                    .ToArray();

                string par = "";
                strParams.ToList().ForEach(x => par += x + " ");
                Console.WriteLine("controller: " + controllerName + " - " + methodName + " | " + par);
            }
            catch
            {
                var resp = page.Help();
                resp.Code = 400;
                return resp;
            }

            return method?.Invoke(param, @params) as HttpResponse;
        }
    }
}