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
        public const string ExceptionType = "Xlent.Fulcrum.Contract";
        public ContractException() : this((string)null, null) { }
        public ContractException(string message) : this(message, null) { }
        public ContractException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string Type => ExceptionType;

        private void SetProperties()
        {
        }
    }
}
