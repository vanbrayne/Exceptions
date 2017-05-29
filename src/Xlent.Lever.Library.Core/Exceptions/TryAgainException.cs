using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The resource was temporarily locked, please try again in the recommended time span (<see cref="FulcrumException.RecommendedWaitTimeInSeconds"/>).
    /// </summary>
    public class TryAgainException : FulcrumException, IServerException
    {
        public static TryAgainException Create(string message, Exception innerException)
        {
            return new TryAgainException(message, innerException);
        }
        public const string ExceptionTypeId = "5350b10d-0ac5-4802-aa9b-a9016c7bf636";
        public TimeSpan RetryAfterTimeSpan { get; set; }
        public TryAgainException() : this((string)null, null) { }
        public TryAgainException(string message) : this(message, null) { }
        public TryAgainException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => true;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            if (RecommendedWaitTimeInSeconds <= 0.0) RecommendedWaitTimeInSeconds = 2;

            FriendlyMessage =
                "The resource was temporarily locked, please try again.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#TryAgainException";
        }
    }
}
