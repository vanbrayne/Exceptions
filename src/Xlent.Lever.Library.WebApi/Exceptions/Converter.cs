using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Client;
using Xlent.Lever.Library.Core.Exceptions.Server;
using Xlent.Lever.Library.WebApi.Exceptions.Client;
using NotImplementedException = Xlent.Lever.Library.Core.Exceptions.Server.NotImplementedException;

namespace Xlent.Lever.Library.WebApi.Exceptions
{

    // TODO: Handle nested exceptions

    /// <summary>
    /// This class has conversion methods for converting between unsuccessful HTTP responses and Fulcrum exceptions.
    /// Fulcrum is only using four HTTP status codes for errors; 400, 401, 500 and 503.
    /// This was based on the following blog article http://blog.restcase.com/rest-api-error-codes-101/
    /// </summary>
    public class Converter
    {
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
            FulcrumException exception;
            switch (error.TypeId)
            {
                case BusinessRuleException.ExceptionTypeId:
                    exception = new BusinessRuleException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case ConflictException.ExceptionTypeId:
                    exception = new ConflictException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case ServerContractException.ExceptionTypeId:
                    exception = new ServerContractException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case NotFoundException.ExceptionTypeId:
                    exception = new NotFoundException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case UnauthorizedException.ExceptionTypeId:
                    exception = new UnauthorizedException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case AssertionFailedException.ExceptionTypeId:
                    exception = new AssertionFailedException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case NotImplementedException.ExceptionTypeId:
                    exception = new NotImplementedException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case TryAgainException.ExceptionTypeId:
                    exception = new TryAgainException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                default:
                    exception = null;
                    break;

            }
            if (exception == null)
            {
                var message = $"The TypeId ({error.TypeId}) was not recognized: {error.ToJsonString(Formatting.Indented)}";
                return new AssertionFailedException(message, ToFulcrumException(error.InnerError));
            }
            exception.CopyFrom(error);
            return exception;
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

        private static HttpStatusCode? ToHttpStatusCode(IFulcrumError error)
        {
            switch (error.TypeId)
            {
                case BusinessRuleException.ExceptionTypeId:
                case ServerContractException.ExceptionTypeId:
                case NotFoundException.ExceptionTypeId:
                    return HttpStatusCode.BadRequest;
                case ConflictException.ExceptionTypeId:
                    return HttpStatusCode.Conflict;
                case UnauthorizedException.ExceptionTypeId:
                    return HttpStatusCode.Unauthorized;
                case AssertionFailedException.ExceptionTypeId:
                    return HttpStatusCode.InternalServerError;
                case NotImplementedException.ExceptionTypeId:
                    return HttpStatusCode.InternalServerError;
                case TryAgainException.ExceptionTypeId:
                    return HttpStatusCode.ServiceUnavailable;
                default:
                    return null;
            }
        }
    }
}
