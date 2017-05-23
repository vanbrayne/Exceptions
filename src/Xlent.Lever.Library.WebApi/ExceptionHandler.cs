using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Service;
using Xlent.Lever.Library.Core.Exceptions.Service.Client;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;
using NotImplementedException = Xlent.Lever.Library.Core.Exceptions.Service.Server.NotImplementedException;

namespace Xlent.Lever.Library.WebApi
{
    public class ExceptionHandler
    {
        private const string XFulcrumExceptionInstanceId = "X-Fulcrum-Exception-Instance-Id";
        private const string XFulcrumExceptionTypeId = "X-Fulcrum-Exception-Type-Id";

        public static async Task<FulcrumException> HttpResponseMessageToFulcrumException(HttpResponseMessage response, bool convertFromServerToClient = false, string serverName = null)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (response.IsSuccessStatusCode) return null;
            var content = await GetContent(response);
            var typeId = response.Headers.GetValues(XFulcrumExceptionTypeId).FirstOrDefault();
            typeId = string.IsNullOrWhiteSpace(typeId) ? null : typeId;
            var instanceId = response.Headers.GetValues(XFulcrumExceptionInstanceId).FirstOrDefault();
            instanceId = string.IsNullOrWhiteSpace(instanceId) ? null : instanceId;
            FulcrumException fulcrumException;
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    if (typeId == BusinessRuleException.ExceptionTypeId)
                    {
                        fulcrumException = new BusinessRuleException(content);
                    }
                    else if(typeId == InputException.ExceptionTypeId)
                    {
                        fulcrumException = new InputException(content);
                    }
                    else 
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({typeId}) for status code {response.StatusCode}. (Content was \"{content}\".");
                    }
                    break;
                case HttpStatusCode.Conflict:
                    if (typeId == ConflictException.ExceptionTypeId)
                    {
                        fulcrumException = new ConflictException(content);
                    }
                    else
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({typeId}) for status code {response.StatusCode}. (Content was \"{content}\".");
                    }
                    break;
                case HttpStatusCode.NotFound:
                    if (typeId == NotFoundException.ExceptionTypeId)
                    {
                        fulcrumException = new NotFoundException(content);
                    }
                    else
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({typeId}) for status code {response.StatusCode}. (Content was \"{content}\".");
                    }
                    break;
                case HttpStatusCode.Unauthorized:
                    if (typeId == UnauthorizedException.ExceptionTypeId)
                    {
                        fulcrumException = new UnauthorizedException(content);
                    }
                    else
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({typeId}) for status code {response.StatusCode}. (Content was \"{content}\".");
                    }
                    break;
                case HttpStatusCode.InternalServerError:
                    if (typeId == AssertionFailedException.ExceptionTypeId)
                    {
                        fulcrumException = new AssertionFailedException(content);
                    }
                    else if (typeId == NotImplementedException.ExceptionTypeId)
                    {
                        fulcrumException = new NotImplementedException(content);
                    }
                    else
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({typeId}) for status code {response.StatusCode}. (Content was \"{content}\".");
                    }
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    if (typeId == UnavailableException.ExceptionTypeId)
                    {
                        fulcrumException = new UnavailableException(content);
                    }
                    else
                    {
                        fulcrumException = new AssertionFailedException($"Unexpected ExceptionTypeId ({typeId}) for status code {response.StatusCode}. (Content was \"{content}\".");
                    }
                    break;
                default:
                    fulcrumException = new AssertionFailedException($"Unexpeced status code {response.StatusCode}. (Content was \"{content}\".");
                    break;
            }
            fulcrumException.InstanceId = string.IsNullOrWhiteSpace(instanceId) ? Guid.NewGuid().ToString() : instanceId;
            if (convertFromServerToClient) fulcrumException = fulcrumException.FromServerToClient(serverName);
            return fulcrumException;
        }

        public static HttpResponseMessage ExceptionToHttpResponseMessage(Exception e)
        {
            HttpResponseMessage response;
            var fulcrumException = e as FulcrumException;
            if (fulcrumException == null)
            {
                var message = $"The exception {e.GetType().FullName} was not recognized as a Fulcrum Exception. Message: {e.Message}";
                fulcrumException = new AssertionFailedException(message);
            }

            if (fulcrumException is BusinessRuleException || fulcrumException is InputException)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(fulcrumException.Message)
                };

            }
            else if (fulcrumException is ConflictException)
            {
                response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(fulcrumException.Message)
                };

            }
            else if (fulcrumException is NotFoundException)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(fulcrumException.Message)
                };

            }
            else if (fulcrumException is UnauthorizedException)
            {
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(fulcrumException.Message)
                };

            }
            else if(fulcrumException is AssertionFailedException || fulcrumException is NotImplementedException)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(fulcrumException.Message)
                };

            }
            else if (fulcrumException is UnavailableException)
            {
                response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent(fulcrumException.Message)
                };

            }
            else 
            {
                var message = $"Unexpected exception: {fulcrumException.GetType().FullName}: {fulcrumException.Message}";
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(message)
                };
            }
            response.Headers.Add(XFulcrumExceptionTypeId, fulcrumException.TypeId);
            response.Headers.Add(XFulcrumExceptionInstanceId, fulcrumException.InstanceId);
            return response;
        }

        private static async Task<string> GetContent(HttpResponseMessage response)
        {
            return response.Content == null ? null : await response.Content?.ReadAsStringAsync();
        }
    }
}
