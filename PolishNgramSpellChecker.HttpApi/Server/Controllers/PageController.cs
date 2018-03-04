namespace PolishNgramSpellChecker.HttpApi.Server.Controllers
{
    internal class PageController
    {
        public HttpResponse Help()
        {
            return HttpResponse.ReturnPage(@"HtmlPages\help.html");
        }
    }
}