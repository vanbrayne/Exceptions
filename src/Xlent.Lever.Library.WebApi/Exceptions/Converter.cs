using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
    public class Converter
    {
        public static async Task<FulcrumException> ToFulcrumExceptionAsync(HttpResponseMessage response, bool convertFromServerToClient = false, string serverName = null)
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
            if (fulcrumException == null)
            {
                var message = $"The TypeId ({error.TypeId}) was not recognized: {error.ToJsonString(Formatting.Indented)}";
                return new AssertionFailedException(message, ToFulcrumException(error.InnerError));
            }
            if (convertFromServerToClient) fulcrumException = fulcrumException.FromServerToClient(serverName);
            return fulcrumException;
        }

        public static FulcrumException ToFulcrumException(FulcrumError fulcrumError)
        {
            if (fulcrumError == null) return null;
            FulcrumException fulcrumException;
            switch (fulcrumError.TypeId)
            {
                case BusinessRuleException.ExceptionTypeId:
                    fulcrumException = new BusinessRuleException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case ConflictException.ExceptionTypeId:
                    fulcrumException = new ConflictException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case InputException.ExceptionTypeId:
                    fulcrumException = new InputException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case NotFoundException.ExceptionTypeId:
                    fulcrumException = new NotFoundException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case UnauthorizedException.ExceptionTypeId:
                    fulcrumException = new UnauthorizedException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case AssertionFailedException.ExceptionTypeId:
                    fulcrumException = new AssertionFailedException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case NotImplementedException.ExceptionTypeId:
                    fulcrumException = new NotImplementedException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                case TryAgainException.ExceptionTypeId:
                    fulcrumException = new TryAgainException(fulcrumError.TechnicalMessage, ToFulcrumException(fulcrumError.InnerError));
                    break;
                default:
                    fulcrumException = null;
                    break;

            }
            if (fulcrumException == null)
            {
                var message = $"The TypeId ({fulcrumError.TypeId}) was not recognized: {fulcrumError.ToJsonString(Formatting.Indented)}";
                return new AssertionFailedException(message, ToFulcrumException(fulcrumError.InnerError));
            }
            fulcrumException.CopyFrom(fulcrumError);
            return fulcrumException;
        }

        public static HttpResponseMessage ToHttpResponseMessage(Exception e, bool mustMatchCoreExceptions = false)
        {
            var fulcrumException = e as FulcrumException;
            

            var error = ToFulcrumError(e);
            if (error == null)
            {
                var message = $"The exception {e.GetType().FullName} was not recognized as a Fulcrum Exception. Message: {e.Message}";
                error = ToFulcrumError(new AssertionFailedException(message, e));
                if (error == null) throw new ApplicationException("Failed to convert AssertionFailedException to a FulcrumError.");
            }
            var statusCode = ToHttpStatusCode(error);
            if (statusCode == null)
            {
                if (!mustMatchCoreExceptions) return null;
                throw new AssertionFailedException(
                    $"The TypeId of the following error could not be converted to an HTTP status code: {error.ToJsonString(Formatting.Indented)}.");
            }
            var content = error.ToJsonString(Formatting.Indented);
            var stringContent = new StringContent(content);
            var response = new HttpResponseMessage(statusCode.Value)
            {
                Content = stringContent
            };
            return response;
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

        private static HttpStatusCode? ToHttpStatusCode(IFulcrumError fulcrumError)
        {
            switch (fulcrumError.TypeId)
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

        private static void ValidateStatusCode(HttpStatusCode statusCode, FulcrumError fulcrumError)
        {
            var expectedStatusCode = ToHttpStatusCode(fulcrumError);
            if (expectedStatusCode == null)
            {
                throw new AssertionFailedException(
                    $"The TypeId of the content could not be converted to an HTTP status code: {fulcrumError.ToJsonString(Formatting.Indented)}.");
            }
            if (expectedStatusCode != statusCode)
            {
                throw new AssertionFailedException(
                    $"The HTTP error response had status code {statusCode}, but was expected to have {expectedStatusCode.Value}, due to the TypeId in the content: \"{fulcrumError.ToJsonString(Formatting.Indented)}");
            }
        }
    }
}
