using System.Threading.Tasks;
using Bll.Interfaces;
using Bll.Models;

namespace Bll
{
    public class BllTicketLogic
    {
        private readonly IDalTicketClient _dalClient;

        public BllTicketLogic(IDalTicketClient dalClient)
        {
            _dalClient = dalClient;
        }
        public async Task<Ticket> GetTicketAsync(string ticketId, ExpectedResultEnum expectedFacadeResult)
        {
            var ticket = await _dalClient.GetTicketAsync(ticketId, expectedFacadeResult);
            return ticket;
        }
    }
}
