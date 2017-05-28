using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The specified item could not be found.
    /// </summary>
    /// <example>
    /// A request for a person with a specified Id that doesn't exist should always throw this exception.
    /// </example>
    /// <example>
    /// If a person exists, and the request is for a list of e-mail addresses and the person doesn't have any, 
    /// you should not throw this exception, but return an empty list.
    /// </example>
    public class NotFoundException : FulcrumException, IClientException
    {
        public static NotFoundException Create(string message, Exception innerException)
        {
            return new NotFoundException(message, innerException);
        }
        public const string ExceptionTypeId = "8108ca0d-14a3-4732-8b58-eb4151fb222d";
        public NotFoundException() : this((string)null, null) { }
        public NotFoundException(string message) : this(message, null) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => true;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            if (RecommendedWaitTimeInSeconds <= 0.0) RecommendedWaitTimeInSeconds = 60;
            // TODO: Set the following properties if they haven't been set already: TechnicalMessage, FriendlyMessage, MoreInfoUrl, FriendlyMessageId
        }
    }
}
