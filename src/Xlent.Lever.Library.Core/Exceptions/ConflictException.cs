using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The request conflicted with the current state of the resource.
    /// </summary>
    /// <example>
    /// Someone else has edited the resource (The Update with ETag scenario).
    /// </example>
    /// <example>
    /// Someone else has already created the resource (The Create or Insert scenario with duplicates).
    /// </example>
    public class ConflictException : FulcrumException, IClientException
    {
        public static ConflictException Create(string message, Exception innerException)
        {
            return new ConflictException(message, innerException);
        }
        public const string ExceptionTypeId = "8ca23bea-e1f9-4c35-a181-e16738122a75";
        public ConflictException() : this((string)null, null) { }
        public ConflictException(string message) : this(message, null)
        {
            SetProperties();
        }
        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            FriendlyMessage =
                "The request conflicted with a request made by someone else. Please reload your data and make a new request.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#ConflictException";
        }
    }
}
