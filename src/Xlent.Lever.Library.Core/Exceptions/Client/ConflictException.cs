using System;

namespace Xlent.Lever.Library.Core.Exceptions.Client
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
    public class ConflictException : ClientException
    {
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
            // TODO: Set the following properties if they haven't been set already: TechnicalMessage, FriendlyMessage, MoreInfoUrl, FriendlyMessageId
        }
    }
}
