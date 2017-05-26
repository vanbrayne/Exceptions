using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Service;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// Information that will be returned when a REST service returns a non successful HTTP status code
    /// </summary>
    /// <remarks>
    /// Inspired by the follwing articles
    /// http://blog.restcase.com/rest-api-error-codes-101/
    /// https://stormpath.com/blog/spring-mvc-rest-exception-handling-best-practices-part-1
    /// </remarks>
    public class Error : IError
    {
        /// <summary>
        /// Mandatory technical information that a developer calling your REST API might find useful.
        /// This is where you might include exception messages, stack traces, or anything else that you
        /// think will help a developer.
        /// </summary>
        /// <remarks>
        /// This message is not expected to contain any of the codes or identifiers that are already contained
        /// in this error type, sucha as the error <see cref="Code"/> or the <see cref="InstanceId"/>.
        /// </remarks>
        public string TechnicalMessage { get; set; }

        /// <summary>
        /// An optional human readable error message that can potentially be shown directly to an application
        /// end user (not a developer). It should be friendly and easy to understand and convey a concise
        /// reason as to why the error occurred.  It should probaby not contain technical information. 
        /// </summary>
        public string FriendlyMessage { get; set; }

        /// <summary>
        /// An optional URL that anyone seeing the error message can click (or copy and paste) in a browser.
        /// The target web page should describe the error condition fully, as well as potential solutions
        /// to help them resolve the error condition.
        /// </summary>
        public string MoreInfoUrl { get; set; }

        /// <summary>
        /// Mandatory indication for if it would be meaningful to try sending the request again.
        /// </summary>
        public bool IsRetryMeaningful { get; set; }

        /// <summary>
        /// If <see cref="IsRetryMeaningful"/> is true, then this optional property can give a recommended
        /// interval to wait before the request is sent again. A value less or equal to 0.0 means that
        /// no recommendation was given.
        /// </summary>
        public double RecommendedWaitTimeInSeconds { get; set; }

        /// <summary>
        /// An optional technical name for the server that created this error information.
        /// </summary>
        /// <remarks>
        /// Useful when for a call that is "deep", i.e. the call was relayed to another server.
        /// </remarks>
        public string ServerTechnicalName { get; set; }

        /// <summary>
        /// A mandatory unique identifier for this particular instance of the error. Ideally, the same identifier
        /// should not be used ever again. The recommendation is to use a newly created GUID.
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// An optional error code for this specific part of the code that reported the error. Will typically
        /// be a part of the <see cref="MoreInfoUrl"/>.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Errors are grouped into different types, such as "BusinessRule", "NotFound", "Unavailable".
        /// TypeId is a mandatory unique id for the type of error. The recommendation is to use a constant GUID.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// All calls that were involved in the chain that led to this error (successful calls or not) will
        /// all be marked in the logs with this mandatory CorrelationId. It is valuable if someone wants to track down
        /// exactly what happened.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// An optional unique id for the format string for the <see cref="FriendlyMessage"/>. The intentation is that
        /// it can be used for translating the message. Recommended to be a constant GUID.
        /// </summary>
        public string FriendlyMessageId { get; set; }

        /// <summary>
        /// An optional error that was the cause of this error.
        /// </summary>
        public Error InnerError { get; set; }

        public void CopyFrom(IError error)
        {
            TechnicalMessage = error.TechnicalMessage;
            FriendlyMessage = error.FriendlyMessage;
            MoreInfoUrl = error.MoreInfoUrl;
            IsRetryMeaningful = error.IsRetryMeaningful;
            RecommendedWaitTimeInSeconds = error.RecommendedWaitTimeInSeconds;
            ServerTechnicalName = error.ServerTechnicalName;
            InstanceId = error.InstanceId;
            Code = error.Code;
            TypeId = error.TypeId;
            CorrelationId = error.CorrelationId;
            FriendlyMessageId = error.FriendlyMessageId;

            var innerError = (error as Exception)?.InnerException as IError;
            if (innerError == null) return;
            InnerError = new Error();
            InnerError.CopyFrom(innerError);
        }

        public string ToJsonString(Formatting formatting)
        {
            return JObject.FromObject(this).ToString(formatting);
        }

        public static Error Parse(string s)
        {
            if (s == null) return null;
            try
            {
                var json = JObject.Parse(s);
                return json.ToObject<Error>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
