using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xlent.Lever.Library.Core.Assert;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;
using NotImplementedException = Xlent.Lever.Library.Core.Exceptions.NotImplementedException;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// This class has conversion methods for converting between unsuccessful HTTP responses and Fulcrum exceptions.
    /// Fulcrum is only using three HTTP status codes for errors; 400, 500 and 503.
    /// This was based on the following blog article http://blog.restcase.com/rest-api-error-codes-101/
    /// </summary>
    public class Converter
    {
        private static readonly Dictionary<string, Func<string, Exception, FulcrumException>> FactoryMethodsCache = new Dictionary<string, Func<string, Exception, FulcrumException>>();
        private static readonly Dictionary<string, HttpStatusCode> HttpStatusCodesCache = new Dictionary<string, HttpStatusCode>();

        public static void AddFulcrumException(Type fulcrumExceptionType, HttpStatusCode? statusCode = null)
        {
            var methodInfo = fulcrumExceptionType.GetMethod("Create");
            Func<string, Exception, FulcrumException> createInstanceDelegate;
            try
            {
                createInstanceDelegate =
                    (Func<string, Exception, FulcrumException>)Delegate.CreateDelegate(
                        typeof(Func<string, Exception, FulcrumException>), methodInfo);
            }
            catch (Exception e)
            {
                throw new ContractException(
                    $"The type {fulcrumExceptionType.FullName} must have a factory method Create(string message, Exception innerException).",
                    e);
            }
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once RedundantCast
            var exception = createInstanceDelegate("test", (Exception)null);
            FactoryMethodsCache.Add(exception.TypeId, createInstanceDelegate);
            if (statusCode != null) HttpStatusCodesCache.Add(exception.TypeId, statusCode.Value);
        }

        static Converter()
        {
            // Core
            AddFulcrumException(typeof(AssertionFailedException), HttpStatusCode.InternalServerError);
            AddFulcrumException(typeof(BusinessRuleException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(ConflictException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(ContractException));
            AddFulcrumException(typeof(NotFoundException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(NotImplementedException), HttpStatusCode.InternalServerError);
            AddFulcrumException(typeof(TryAgainException), HttpStatusCode.ServiceUnavailable);

            // WebApi
            AddFulcrumException(typeof(ServerContractException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(UnauthorizedException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(ForbiddenAccessException), HttpStatusCode.BadRequest);
        }

        public static async Task<FulcrumException> ToFulcrumExceptionAsync(HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (response.IsSuccessStatusCode) return null;
            if (response.Content == null) return null;
            var contentAsString = await response.Content?.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contentAsString))
            {
                throw new AssertionFailedException(
                    $"Received an HTTP response with status code {response.StatusCode}. Expected a JSON formatted error as content, but the content was empty.");
            }
            var error = FulcrumError.Parse(contentAsString);
            if (error == null)
            {
                throw new AssertionFailedException(
                    $"Received an HTTP response with status code {response.StatusCode}. Expected a JSON formatted error as content, but the content was \"{contentAsString}\".");
            }
            ValidateStatusCode(response.StatusCode, error);
            var fulcrumException = ToFulcrumException(error);
            if (fulcrumException != null) return fulcrumException;
            var message = $"The TypeId ({error.TypeId}) was not recognized: {error.ToJsonString(Formatting.Indented)}";
            return new AssertionFailedException(message, ToFulcrumException(error.InnerError));
        }

        public static FulcrumException ToFulcrumException(FulcrumError error)
        {
            if (error == null) return null;
            var fulcrumException = CreateFulcrumException(error);
            fulcrumException.CopyFrom(error);
            return fulcrumException;
        }

        public static HttpResponseMessage ToHttpResponseMessage(Exception e, bool mustMatchCoreExceptions = false)
        {
            bool firstTime = true;
            while (true)
            {
                var error = ToFulcrumError(e);
                if (error == null)
                {
                    var message = $"The exception {e.GetType().FullName} was not recognized as a Fulcrum Exception. Message: {e.Message}";
                    if (!firstTime) throw new ApplicationException(message);
                    firstTime = false;
                    e = new AssertionFailedException(message, e);
                    continue;
                }
                var statusCode = ToHttpStatusCode(error);
                if (statusCode == null)
                {
                    var message =
                        $"The TypeId of the following error could not be converted to an HTTP status code: {error.ToJsonString(Formatting.Indented)}.";
                    if (!firstTime) throw new ApplicationException(message);
                    firstTime = false;
                    if (!mustMatchCoreExceptions) return null;
                    e = new AssertionFailedException(message, e);
                    continue;
                }
                var content = error.ToJsonString(Formatting.Indented);
                var stringContent = new StringContent(content);
                var response = new HttpResponseMessage(statusCode.Value)
                {
                    Content = stringContent
                };
                return response;
            }
        }

        public static FulcrumError ToFulcrumError(Exception e)
        {
            var fulcrumException = e as FulcrumException;
            if (fulcrumException == null) return null;
            var error = new FulcrumError();
            error.CopyFrom(fulcrumException);
            error.InnerError = ToFulcrumError(fulcrumException.InnerException);
            return error;
        }

        public static FulcrumException FromDalToBll(FulcrumException source, string serverTechnicalName)
        {
            if (source == null) return null;
            switch (source.TypeId)
            {
                case AssertionFailedException.ExceptionTypeId:
                case NotImplementedException.ExceptionTypeId:
                    return new AssertionFailedException($"Did not expect {serverTechnicalName ?? "server"} to return the following error: {source.Message}", source);
                case ServerContractException.ExceptionTypeId:
                    return new AssertionFailedException($"Bad call to { serverTechnicalName ?? "Server" }: { source.Message}", source);
                case UnauthorizedException.ExceptionTypeId:
                    return new AssertionFailedException($"Unauthorized call to {serverTechnicalName ?? "server"}: {source.Message}", source);
                default:
                    source.ServerTechnicalName = serverTechnicalName;
                    return source;
            }
        }

        public static FulcrumException FromBllToServer(FulcrumException source)
        {
            if (source == null) return null;
            switch (source.TypeId)
            {
                case ContractException.ExceptionTypeId:
                    return new AssertionFailedException($"Unexpected error: {source.Message}", source);
                default:
                    return source;
            }
        }

        private static void ValidateStatusCode(HttpStatusCode statusCode, FulcrumError error)
        {
            var expectedStatusCode = ToHttpStatusCode(error);
            if (expectedStatusCode == null)
            {
                throw new AssertionFailedException(
                    $"The TypeId of the content could not be converted to an HTTP status code: {error.ToJsonString(Formatting.Indented)}.");
            }
            if (expectedStatusCode != statusCode)
            {
                throw new AssertionFailedException(
                    $"The HTTP error response had status code {statusCode}, but was expected to have {expectedStatusCode.Value}, due to the TypeId in the content: \"{error.ToJsonString(Formatting.Indented)}");
            }
        }

        private static FulcrumException CreateFulcrumException(FulcrumError error, bool okIfNotExists = false)
        {
            if (!FactoryMethodsCache.ContainsKey(error.TypeId))
            {
                if (okIfNotExists) return null;
                var message = $"The TypeId ({error.TypeId}) was not recognized: {error.ToJsonString(Formatting.Indented)}. It must be added to {typeof(Converter).FullName}.";
                return new AssertionFailedException(message, ToFulcrumException(error.InnerError));
            }
            var factoryMethod = FactoryMethodsCache[error.TypeId];
            var fulcrumException = factoryMethod(error.TechnicalMessage, ToFulcrumException(error.InnerError));
            return fulcrumException;
        }

        private static HttpStatusCode? ToHttpStatusCode(FulcrumError error)
        {
            if (!HttpStatusCodesCache.ContainsKey(error.TypeId)) return null;
            return HttpStatusCodesCache[error.TypeId];
        }
    }
}
