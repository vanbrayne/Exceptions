using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Bll.Interfaces;
using Bll.Models;
using CompositionRoot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.WebApi.Contract;
using Xlent.Lever.Library.WebApi.Exceptions;

namespace Service.WebApi
{
    public class ServiceTicket
    {
        private static readonly BllTicketLogic TicketLogic = new BllTicketLogic(DependencyResolver.GetDalTicketClient());

        public async Task<HttpResponseMessage> GetTicketAsync(string ticketId, string expectedFacadeResult)
        {
            HttpResponseMessage response;
            try
            {
                var ticket = ToContract(await TicketLogic.GetTicketAsync(ticketId, ExpectedResultFromContract(expectedFacadeResult)));
                var json = JObject.FromObject(ticket);
                response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json.ToString(Formatting.Indented), Encoding.UTF8,
                        "application/json")
                };
            
            }
            catch (Exception e)
            {
                response = Converter.ToHttpResponseMessage(e, true);
            }
            return response;
        }

        private ExpectedResultEnum ExpectedResultFromContract(string expectedFacadeResult)
        {
            switch (expectedFacadeResult)
            {
                case "Ok":
                    return ExpectedResultEnum.Ok;
                case "BusinessRuleException":
                    return ExpectedResultEnum.BusinessRuleException;
                case "ConflictException":
                    return ExpectedResultEnum.ConflictException;
                case "ContractException":
                    return ExpectedResultEnum.ContractException;
                case "NotFoundException":
                    return ExpectedResultEnum.NotFoundException;
                case "UnauthorizedException":
                    return ExpectedResultEnum.UnauthorizedException;
                case "AssertionFailedException":
                    return ExpectedResultEnum.AssertionFailedException;
                case "NotImplementedException":
                    return ExpectedResultEnum.NotImplementedException;
                case "TryAgainException":
                    return ExpectedResultEnum.TryAgainException;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedFacadeResult));
            }
        }

        private static ContractTicket ToContract(Ticket source)
        {
            if (source == null) return null;
            return new ContractTicket
            {
                Id = source.Id
            };
        }
    }
}
