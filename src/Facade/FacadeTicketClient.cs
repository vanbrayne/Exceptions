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
using Xlent.Lever.Library.WebApi.Exceptions;
using Xlent.Lever.Library.WebApi.Exceptions.Service.Client;

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
                response = Converter.ToHttpResponseMessage(e, true);
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
                    fulcrumException = new ContractException("Input exception");
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
                default:
                    fulcrumException = new AssertionFailedException($"Unexpected switch value: {expectedFacadeResult}");
                    break;
            }
            // This is to be able to test that the properties are copied all the way back to the test case.
            fulcrumException.Code = fulcrumException.InstanceId;
            throw fulcrumException;
        }
    }
}