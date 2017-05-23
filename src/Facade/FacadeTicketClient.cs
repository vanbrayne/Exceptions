using System.Net;
using System.Net.Http;
using System.Text;
using Bll.Interfaces;
using Facade.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Library.Core.Exceptions.Service;
using Xlent.Lever.Library.Core.Exceptions.Service.Client;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;
using Xlent.Lever.Library.WebApi;

namespace Facade
{
    public class FacadeTicketClient
    {
        public HttpResponseMessage GetTicket(string ticketId, ExpectedResultEnum expectedFacadeResult)
        {
            HttpResponseMessage response;
            try
            {
                var ticket = GetTicketWithExceptions(ticketId, expectedFacadeResult);
                var json = JObject.FromObject(ticket);
                response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json.ToString(Formatting.Indented), Encoding.UTF8,
                        "application/json")
                };
            }
            catch (System.Exception e)
            {
                response = ExceptionHandler.ExceptionToHttpResponseMessage(e);
            }
            return response;
        }

        private Ticket GetTicketWithExceptions(string ticketId, ExpectedResultEnum expectedFacadeResult)
        {
            FulcrumException fulcrumException = null;
            switch (expectedFacadeResult)
            {
                case ExpectedResultEnum.Ok:
                    return new Ticket
                    {
                        Id = ticketId
                    };
                case ExpectedResultEnum.BusinessRuleException:
                    fulcrumException = new BusinessRuleException("Business rule exception");
                    break;
                case ExpectedResultEnum.ConflictException:
                    fulcrumException = new ConflictException("Conflict exception");
                    break;
                case ExpectedResultEnum.InputException:
                    fulcrumException = new InputException("Input exception");
                    break;
                case ExpectedResultEnum.NotFoundException:
                    fulcrumException = new NotFoundException("Not found exception");
                    break;
                case ExpectedResultEnum.UnauthorizedException:
                    fulcrumException = new UnauthorizedException("Unauthorized exception");
                    break;
                case ExpectedResultEnum.AssertionFailedException:
                    fulcrumException = new AssertionFailedException("Assertion failed exception");
                    break;
                case ExpectedResultEnum.NotImplementedException:
                    fulcrumException = new NotImplementedException("Not implemented exception");
                    break;
                case ExpectedResultEnum.UnavailableException:
                    fulcrumException = new UnavailableException("Unavailable exception");
                    break;
                default:
                    fulcrumException = new AssertionFailedException($"Unexpected switch value: {expectedFacadeResult}");
                    break;
            }
            fulcrumException.InstanceId = "75573277-52e0-4ece-b9f2-79d7bc7d0658";
            throw fulcrumException;
        }
    }
}