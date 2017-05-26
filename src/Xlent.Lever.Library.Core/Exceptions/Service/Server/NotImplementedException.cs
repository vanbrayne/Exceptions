using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Server
{
    /// <summary>
    /// The request requires some functionality that deliberately has been left out, at least for the moment.
    /// </summary>
    /// <example>
    /// During testing, there could be parts of the code that hasn't been developed yet. Then throwing this exception is appropriate.
    /// </example>
    public class NotImplementedException : ServerException
    {
        public const string ExceptionTypeId = "573f806f-56b9-48a0-86d9-15d3bf6c9c6c";
        public NotImplementedException() : this((string)null, null) { }
        public NotImplementedException(string message) : this(message, null) { }
        public NotImplementedException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            // TODO: Set the following properties if they haven't been set already: TechnicalMessage, FriendlyMessage, MoreInfoUrl, FriendlyMessageId
        }
    }
}
