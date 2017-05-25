using System;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Client
{
    /// <summary>
    /// There was something wrong with the request itself, i.e. syntax, values out of range, etc.
    /// </summary>
    public class InputException : ClientException
    {
        public const string ExceptionTypeId = "659f879a-299a-4c11-921f-466fde971c13";
        public InputException() : this(null, null) { }
        public InputException(string message) : this(message, null) { }
        public InputException(IError error) : base(error)
        {
            SetProperties();
        }
        public InputException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string TypeId => ExceptionTypeId;

        public override FulcrumException FromServerToClient(string serverTechnicalName)
        {
            ServerTechnicalName = serverTechnicalName;
            var e = new AssertionFailedException(
                $"The service sent bad parameters to {ServerTechnicalName ?? "server"}: {Message}", this);
            return e;
        }

        private void SetProperties()
        {
            // TODO: Set the following properties if they haven't been set already: TechnicalMessage, FriendlyMessage, MoreInfoUrl, FriendlyMessageId
        }
    }
}
