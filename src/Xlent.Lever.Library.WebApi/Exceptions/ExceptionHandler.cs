using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Service;
using Xlent.Lever.Library.Core.Exceptions.Service.Client;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;
using NotImplementedException = Xlent.Lever.Library.Core.Exceptions.Service.Server.NotImplementedException;

namespace Xlent.Lever.Library.WebApi.Exceptions
{

    // TODO: Handle nested exceptions

    /// <summary>
    /// This class has conversion methods for converting between unsuccessful HTTP responses and Fulcrum exceptions.
    /// Fulcrum is only using four HTTP status codes for errors; 400, 401, 500 and 503.
    /// This was based on the following blog article http://blog.restcase.com/rest-api-error-codes-101/
    /// </summary>
    public class ExceptionHandler
    {
        public static async Task<FulcrumException> HttpResponseMessageToFulcrumException(HttpResponseMessage response, bool convertFromServerToClient = false, string serverName = null)
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
            var error = Error.Parse(contentAsString);
            if (error == null)
            {
                throw new AssertionFailedException(
                    $"Received an HTTP response with status code {response.StatusCode}. Expected a JSON formatted error as content, but the content was \"{contentAsString}\".");
            }
            ValidateStatusCode(response.StatusCode, error);
            var fulcrumException = ToFulcrumException(error);
            if (fulcrumException == null)
            {
                var message = $"The TypeId ({error.TypeId}) was not recognized: {error.ToJsonString(Formatting.Indented)}";
                return new AssertionFailedException(message, ToFulcrumException(error.InnerError));
            }
            if (convertFromServerToClient) fulcrumException = fulcrumException.FromServerToClient(serverName);
            return fulcrumException;
        }

        private static void ValidateStatusCode(HttpStatusCode statusCode, Error error)
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

        public static HttpResponseMessage ExceptionToHttpResponseMessage(Exception e, bool mustMatchCoreExceptions = false)
        {
            HttpResponseMessage response = null;
            var fulcrumException = e as FulcrumException;
            if (fulcrumException == null)
            {
                var message = $"The exception {e.GetType().FullName} was not recognized as a Fulcrum Exception. Message: {e.Message}";
                fulcrumException = new AssertionFailedException(message, e);
            }

            var error = new Error();
            error.CopyFrom(fulcrumException);
            var content = error.ToJsonString(Formatting.Indented);
            var stringContent = new StringContent(content);
            if (
                fulcrumException is BusinessRuleException
                || fulcrumException is InputException
                || fulcrumException is ConflictException
                || fulcrumException is NotFoundException)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = stringContent
                };
            }
            else if (fulcrumException is UnauthorizedException)
            {
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = stringContent
                };

            }
            else if (
                fulcrumException is AssertionFailedException
                || fulcrumException is NotImplementedException)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = stringContent
                };

            }
            else if (mustMatchCoreExceptions)
            {
                var message = $"Unexpected exception: {fulcrumException.GetType().FullName}: {fulcrumException.Message}";
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(message)
                };
            }
            return response;
        }

        private static HttpStatusCode? ToHttpStatusCode(IError error)
        {
            switch (error.TypeId)
            {
                case BusinessRuleException.ExceptionTypeId:
                case InputException.ExceptionTypeId:
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

        private static FulcrumException ToFulcrumException(Error error)
        {
            if (error == null) return null;
            FulcrumException fulcrumException;
            switch (error.TypeId)
            {
                case BusinessRuleException.ExceptionTypeId:
                    fulcrumException = new BusinessRuleException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case ConflictException.ExceptionTypeId:
                    fulcrumException = new ConflictException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case InputException.ExceptionTypeId:
                    fulcrumException = new InputException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case NotFoundException.ExceptionTypeId:
                    fulcrumException = new NotFoundException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case UnauthorizedException.ExceptionTypeId:
                    fulcrumException = new UnauthorizedException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case AssertionFailedException.ExceptionTypeId:
                    fulcrumException = new AssertionFailedException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case NotImplementedException.ExceptionTypeId:
                    fulcrumException = new NotImplementedException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                case TryAgainException.ExceptionTypeId:
                    fulcrumException = new TryAgainException(error.TechnicalMessage, ToFulcrumException(error.InnerError));
                    break;
                default:
                    return null;

            }
            fulcrumException.CopyFrom(error);
            return fulcrumException;
        }
    }
}
