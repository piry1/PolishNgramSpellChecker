using System;
using Newtonsoft.Json;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.HttpApi.Server.Controllers
{
    internal class SpellCheckerController
    {
        public SpellChecker _spellChecker = new SpellChecker();

        public HttpResponse Check(string body)
        {
            //SpellCheckerParams param = new SpellCheckerParams();
            //Console.WriteLine(JsonConvert.SerializeObject(param));

            var a = JsonConvert.DeserializeObject<SpellCheckerParams>(body);
            Console.WriteLine(a);
            return HttpResponse.ReturnJson(a);
        }
    }
}