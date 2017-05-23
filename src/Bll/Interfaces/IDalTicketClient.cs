using System.Threading.Tasks;
using Bll.Models;

namespace Bll.Interfaces
{
    public enum ExpectedResultEnum
    {
        Ok,
        BusinessRuleException,
        ConflictException,
        InputException,
        NotFoundException,
        UnauthorizedException,
        AssertionFailedException,
        NotImplementedException,
        UnavailableException
    }

    public interface IDalTicketClient
    {
        Task<Ticket> GetTicketAsync(string ticketId, ExpectedResultEnum expectedFacadeResult);
    }
}