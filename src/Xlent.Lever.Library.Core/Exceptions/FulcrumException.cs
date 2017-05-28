using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The base class for all Fulcrum exceptions
    /// </summary>
    public abstract class FulcrumException : Exception, IFulcrumException
    {
        /// <summary>
        /// The current servent name. Can be set by calling <see cref="Initialize"/>.
        /// Will automaticall be copied to the the field <see cref="ServerTechnicalName"/> for every new error.
        /// </summary>
        private static string _serverTechnicalName;

        /// <summary>
        /// Mandatory technical information that a developer might find useful.
        /// This is where you might include exception messages, stack traces, or anything else that you
        /// think will help a developer.
        /// </summary>
        /// <remarks>
        /// This message is not expected to contain any of the codes or identifiers that are already contained
        /// in this error type, sucha as the error <see cref="Code"/> or the <see cref="InstanceId"/>.
        /// </remarks>
        /// <remarks>
        /// If this property has not been set, the recommendation is to treat the <see cref="System.Exception.Message"/>
        /// property as the technical message.
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
        public virtual bool IsRetryMeaningful { get; internal set; }

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
        public string InstanceId { get; private set; }

        /// <summary>
        /// An optional error code for this specific part of the code that reported the error. Will typically
        /// be a part of the <see cref="MoreInfoUrl"/>.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Errors are grouped into different types, such as "BusinessRule", "NotFound", "Unavailable".
        /// TypeId is a mandatory unique id for the type of error. The recommendation is to use a constant GUID.
        /// </summary>
        public virtual string TypeId { get; private set; }

        /// <summary>
        /// All calls that were involved in the chain that led to this error (successful calls or not) will
        /// all be marked in the logs with this optional CorrelationId. It is valuable if someone wants to track down
        /// exactly what happened.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// An optional unique id for the format string for the <see cref="FriendlyMessage"/>. The intentation is that
        /// it can be used for translating the message. Recommended to be a constant GUID.
        /// </summary>
        public string FriendlyMessageId { get; set; }



        protected FulcrumException() : this((string)null, null) { }
        protected FulcrumException(string message) : this(message, null) { }
        protected FulcrumException(string message, Exception innerException) : base(message, innerException)
        {
            TechnicalMessage = message;
            var error = innerException as IFulcrumError;
            if (error == null)
            {
                InstanceId = Guid.NewGuid().ToString();
                return;
            }
            FriendlyMessage = error.FriendlyMessage;
            MoreInfoUrl = error.MoreInfoUrl;
            RecommendedWaitTimeInSeconds = error.RecommendedWaitTimeInSeconds;
            InstanceId = error.InstanceId;
            CorrelationId = error.CorrelationId;
            FriendlyMessageId = error.FriendlyMessageId;
        }

        public void CopyFrom(IFulcrumError fulcrumError)
        {
            TechnicalMessage = fulcrumError.TechnicalMessage;
            FriendlyMessage = fulcrumError.FriendlyMessage;
            MoreInfoUrl = fulcrumError.MoreInfoUrl;
            IsRetryMeaningful = fulcrumError.IsRetryMeaningful;
            RecommendedWaitTimeInSeconds = fulcrumError.RecommendedWaitTimeInSeconds;
            ServerTechnicalName = fulcrumError.ServerTechnicalName;
            InstanceId = fulcrumError.InstanceId;
            Code = fulcrumError.Code;
            TypeId = fulcrumError.TypeId;
            CorrelationId = fulcrumError.CorrelationId;
            FriendlyMessageId = fulcrumError.FriendlyMessageId;
        }

        public static void Initialize(string serverTechnicalName)
        {
            if (serverTechnicalName == null) throw new ArgumentNullException(nameof(serverTechnicalName));
            serverTechnicalName = serverTechnicalName.ToLower();
            if (_serverTechnicalName != null && _serverTechnicalName != serverTechnicalName) throw new ApplicationException("Once the server name has been set, it can't be changed.");
            _serverTechnicalName = serverTechnicalName;
        }
    }
}
