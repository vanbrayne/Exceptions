using System;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// There was something wrong with the request itself, i.e. syntax, values out of range, etc.
    /// </summary>
    public class ServerContractException : FulcrumException, IClientException
    {
        public static ServerContractException Create(string message, Exception innerException)
        {
            return new ServerContractException(message, innerException);
        }
        public const string ExceptionTypeId = "ff6ebb39-23eb-42c3-997d-17a45bd75cf3";
        public ServerContractException() : this((string)null, null) { }
        public ServerContractException(string message) : this(message, null) { }
        public ServerContractException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            FriendlyMessage =
                "The request contained data that was syntactically wrong, had values out of range, or something similar.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#ServerContractException";
        }
    }
}
