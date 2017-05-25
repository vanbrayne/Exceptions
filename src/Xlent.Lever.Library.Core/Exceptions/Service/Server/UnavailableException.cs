using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Server
{
    /// <summary>
    /// The service is either overloaded or unavailable at the moment.
    /// </summary>
    public class UnavailableException : ServerException
    {
        public const string ExceptionTypeId = "e97cb54a-1da4-4a5c-901e-ccee0e833b18";
        public TimeSpan RetryAfterTimeSpan { get; set; }
        public UnavailableException() : this(null, null) { }
        public UnavailableException(string message) : this(message, null) { }
        public UnavailableException(IError error) : base(error)
        {
            SetProperties();
        }
        public UnavailableException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => true;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            if (RecommendedWaitTimeInSeconds <= 0.0) RecommendedWaitTimeInSeconds = 10;
            // TODO: Set the following properties if they haven't been set already: TechnicalMessage, FriendlyMessage, MoreInfoUrl, FriendlyMessageId
        }
    }
}
