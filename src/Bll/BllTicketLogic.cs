using System.Threading.Tasks;
using Bll.Interfaces;
using Bll.Models;
using Xlent.Lever.Library.Core.Assert;

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
            BllContract.Require(nameof(expectedFacadeResult), expectedFacadeResult, x => x != ExpectedResultEnum.ContractException);
            var ticket = await _dalClient.GetTicketAsync(ticketId, expectedFacadeResult);
            return ticket;
        }
    }
}
