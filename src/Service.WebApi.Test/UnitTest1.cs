using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Service.WebApi.Contract;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Service;
using Xlent.Lever.Library.Core.Exceptions.Service.Client;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;
using Xlent.Lever.Library.WebApi.Exceptions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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
            var exception = await Converter.ToFulcrumExceptionAsync(response);
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
        #endregion

        private async Task VerifyException<TFacadeException, TBllException>()
            where TFacadeException : FulcrumException
            where TBllException : FulcrumException, new()
        {
            var response = await _ticketService.GetTicketAsync(TicketId, typeof(TFacadeException).Name);
            var content = await GetContent(response);
            Assert.IsNotNull(content);
            var error = FulcrumError.Parse(content);
            Assert.IsNotNull(error, $"Expected a JSON formatted error. (Content was \"{content}\".");
            ValidateExceptionType<TBllException>(error);
            if (typeof(TFacadeException) == typeof(TBllException))
            {
                // The following condition has been specially prepared for in the mock service.
                // This would never happen in real life.
                Assert.AreEqual(error.InstanceId, error.Code);
            }
        }

        private static void ValidateExceptionType<T>(IFulcrumError fulcrumError)
            where T : FulcrumException, new()
        {
            var expectedException = new T();
            Assert.AreEqual(expectedException.TypeId, fulcrumError.TypeId,
                $"Expected error with Fulcrum exception type {typeof(T).Name} ({expectedException.TypeId}. Error had type {fulcrumError.TypeId}. Message was {fulcrumError.TechnicalMessage}.");
            Assert.AreEqual(expectedException.IsRetryMeaningful, fulcrumError.IsRetryMeaningful,
                $"Error with Fulcrum exception type {typeof(T).Name} ({expectedException.TypeId} unexpectedly had IsRetryMeaningful set to {fulcrumError.IsRetryMeaningful}.");
            Assert.AreEqual(expectedException.RecommendedWaitTimeInSeconds, fulcrumError.RecommendedWaitTimeInSeconds,
                $"Error with Fulcrum exception type {typeof(T).Name} ({expectedException.TypeId} unexpectedly had RecommendedWaitTimeInSeconds set to {fulcrumError.RecommendedWaitTimeInSeconds}.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(fulcrumError.InstanceId));
        }

        private static async Task<string> GetContent(HttpResponseMessage response)
        {
            return response.Content == null ? null : await response.Content?.ReadAsStringAsync();
        }
    }
}
