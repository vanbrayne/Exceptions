using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            if (string.IsNullOrWhiteSpace(contentAsString)) throw new AssertionFailedException($"Received an HTTP response with status code {response.StatusCode}. Expected a JSON formatted error as content, but the content was empty.");
            var error = Error.Parse(contentAsString);
            if (error == null) throw new AssertionFailedException($"Received an HTTP response with status code {response.StatusCode}. Expected a JSON formatted error as content, but the content was \"{contentAsString}\".");
            FulcrumException fulcrumException;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    switch (error.TypeId)
                    {
                        case BusinessRuleException.ExceptionTypeId:
                            fulcrumException = new BusinessRuleException(error);
                            break;
                        case ConflictException.ExceptionTypeId:
                            fulcrumException = new ConflictException(error);
                            break;
                        case InputException.ExceptionTypeId:
                            fulcrumException = new InputException(error);
                            break;
                        case NotFoundException.ExceptionTypeId:
                            fulcrumException = new NotFoundException(error);
                            break;
                        default:
                            fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({error.TypeId}) for status code {response.StatusCode}. (Content was \"{contentAsString}\".");
                            break;
                    }
                    break;
                case HttpStatusCode.Unauthorized:
                    if (error.TypeId == UnauthorizedException.ExceptionTypeId)
                    {
                        fulcrumException = new UnauthorizedException(error);
                    }
                    else
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({error.TypeId}) for status code {response.StatusCode}. (Content was \"{contentAsString}\".");
                    }
                    break;
                case HttpStatusCode.InternalServerError:
                    switch (error.TypeId)
                    {
                        case AssertionFailedException.ExceptionTypeId:
                            fulcrumException = new AssertionFailedException(error);
                            break;
                        case NotImplementedException.ExceptionTypeId:
                            fulcrumException = new NotImplementedException(error);
                            break;
                        default:
                            fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({error.TypeId}) for status code {response.StatusCode}. (Content was \"{contentAsString}\".");
                            break;
                    }
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    switch (error.TypeId)
                    {
                        case UnavailableException.ExceptionTypeId:
                            fulcrumException = new UnavailableException(error);
                            break;
                        case TryAgainException.ExceptionTypeId:
                            fulcrumException = new TryAgainException(error);
                            break;
                        default:
                            fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({error.TypeId}) for status code {response.StatusCode}. (Content was \"{contentAsString}\".");
                            break;
                    }
                    break;
                default:
                    fulcrumException = new AssertionFailedException($"Unexpeced status code {response.StatusCode}. (Content was \"{contentAsString}\".");
                    break;
            }
            if (convertFromServerToClient) fulcrumException = fulcrumException.FromServerToClient(serverName);
            return fulcrumException;
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
            else if (fulcrumException is UnavailableException
                || fulcrumException is TryAgainException)
            {
                response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
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
    }
}
