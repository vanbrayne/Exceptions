using System.Threading.Tasks;
using Bll.Interfaces;
using Bll.Models;
using Facade;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Library.WebApi.Exceptions;

namespace Dal
{
    public class DalTicketClient : IDalTicketClient
    {
        private readonly FacadeTicketClient _ticketFacadeClient = new FacadeTicketClient(); 
        public async Task<Ticket> GetTicketAsync(string ticketId, ExpectedResultEnum expectedFacadeResult)
        {
            var response = _ticketFacadeClient.GetTicket(ticketId, expectedFacadeResult);
            var exception = await Converter.ToFulcrumExceptionAsync(response, true);
            if (exception != null)
            {
                throw exception;
            }
            var s = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(s);
            var ticket = json.ToObject<Ticket>();
            return ticket;
        }
    }
}
