using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.HttpApi.Server;

namespace PolishNgramSpellChecker.HttpApi
{
    class Program
    {
        static void Main(string[] args)
        {       
            HttpServer.StartServer();         
        }
    }
}
