using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The request requires some functionality that deliberately has been left out, at least for the moment.
    /// </summary>
    /// <example>
    /// During testing, there could be parts of the code that hasn't been developed yet. Then throwing this exception is appropriate.
    /// </example>
    public class NotImplementedException : FulcrumException, IServerException
    {
        public static NotImplementedException Create(string message, Exception innerException)
        {
            return new NotImplementedException(message, innerException);
        }
        public const string ExceptionType = "573f806f-56b9-48a0-86d9-15d3bf6c9c6c";
        public NotImplementedException() : this((string)null, null) { }
        public NotImplementedException(string message) : this(message, null) { }
        public NotImplementedException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string Type => ExceptionType;

        private void SetProperties()
        {
            FriendlyMessage =
                "The request requires some functionality that deliberately has been left out, at least for the moment.";
            FriendlyMessage += "Please report the following:";
            FriendlyMessage += $"\rCorrelactionId: {CorrelationId}";
            FriendlyMessage += $"\rInstanceId: {InstanceId}";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#NotImplementedException";
        }
    }
}
