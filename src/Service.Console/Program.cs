using System.Threading.Tasks;
using Bll;
using CompositionRoot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service.Console
{
    class Program
    {
        private static readonly BllTicketLogic TicketLogic = new BllTicketLogic(DependencyResolver.GetDalTicketClient());

        static void Main(string[] args)
        {
            var ticket = TicketLogic.GetTicketAsync("23").Result;
            System.Console.WriteLine(JToken.FromObject(ticket).ToString(Formatting.Indented));
            System.Console.ReadKey(false);
        }
    }
}
