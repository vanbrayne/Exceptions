using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// There was something wrong with the request itself, i.e. syntax, values out of range, etc.
    /// </summary>
    public class ContractException : FulcrumException, IClientException
    {
        public static ContractException Create(string message, Exception innerException)
        {
            return new ContractException(message, innerException);
        }
        public const string ExceptionTypeId = "e83dd5f2-e489-49bf-8991-799c953db2f6";
        public ContractException() : this((string)null, null) { }
        public ContractException(string message) : this(message, null) { }
        public ContractException(string message, Exception innerException) : base(message, innerException)
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
