using System.Threading.Tasks;
using Bll.Models;

namespace Bll.Interfaces
{
    public enum ExpectedResultEnum
    {
        Ok,
        BusinessRuleException,
        ConflictException,
        ContractException,
        NotFoundException,
        UnauthorizedException,
        AssertionFailedException,
        NotImplementedException,
        TryAgainException
    }

    public interface IDalTicketClient
    {
        Task<Ticket> GetTicketAsync(string ticketId, ExpectedResultEnum expectedFacadeResult);
    }
}