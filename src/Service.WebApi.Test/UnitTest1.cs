using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Library.Core.Exceptions.Service;
using Xlent.Lever.Library.Core.Exceptions.Service.Client;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;
using Xlent.Lever.Library.WebApi;

namespace Service.WebApi.Test
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly string TicketId = "23";
        private ServiceTicket _ticketService;

        [TestInitialize]
        public void Initialize()
        {
            _ticketService = new ServiceTicket();
        }

        [TestMethod]
        public async Task OkAsync()
        {
            var response = await _ticketService.GetTicketAsync(TicketId, "Ok");
            var exception = await ExceptionHandler.HttpResponseMessageToFulcrumException(response);
            if (exception != null) throw exception;
            var content = await GetContent(response);
            Assert.IsNotNull(content);
            var json = JObject.Parse(content);
            var ticket = json.ToObject<ContractTicket>();
            Assert.AreEqual(TicketId, ticket.Id);
        }

        #region Client exceptions

        [TestMethod]
        public async Task BusinessRuleException()
        {
            await VerifyException<BusinessRuleException, BusinessRuleException>();
        }

        [TestMethod]
        public async Task ConflictException()
        {
            await VerifyException<ConflictException, ConflictException>();
        }

        [TestMethod]
        public async Task InputException()
        {
            await VerifyException<InputException, AssertionFailedException>();
        }

        [TestMethod]
        public async Task NotFoundException()
        {
            await VerifyException<NotFoundException, NotFoundException>();
        }

        [TestMethod]
        public async Task UnauthorizedException()
        {
            await VerifyException<UnauthorizedException, AssertionFailedException>();
        }
        #endregion

        #region Server exceptions
        [TestMethod]
        public async Task AssertionFailedException()
        {
            await VerifyException<AssertionFailedException, AssertionFailedException>();
        }

        [TestMethod]
        public async Task NotImplementedException()
        {
            await VerifyException<NotImplementedException, NotImplementedException>();
        }

        [TestMethod]
        public async Task UnavailableException()
        {
            await VerifyException<UnavailableException, UnavailableException>();
        }
        #endregion

        private async Task VerifyException<TFacadeException, TBllException>()
            where TFacadeException : FulcrumException
            where TBllException : FulcrumException
        {
            var response = await _ticketService.GetTicketAsync(TicketId, typeof(TFacadeException).Name);
            var content = await GetContent(response);
            var exception = await ExceptionHandler.HttpResponseMessageToFulcrumException(response);
            Assert.IsNotNull(exception, $"Expected an exception. (Content was \"{content}\".");
            ValidateExceptionType<TBllException>(exception);
        }

        private static void ValidateExceptionType<T>(System.Exception e)
            where T : FulcrumException
        {
            Assert.IsNotNull(e as T, $"Expected Fulcrum exception {typeof(T).Name}. Exception was {e.GetType().FullName}. Message was {e.Message}.");
            var fulcrumException = e as FulcrumException;
            Assert.AreEqual("75573277-52e0-4ece-b9f2-79d7bc7d0658", fulcrumException?.InstanceId);
        }

        private static async Task<string> GetContent(HttpResponseMessage response)
        {
            return response.Content == null ? null : await response.Content?.ReadAsStringAsync();
        }
    }
}
