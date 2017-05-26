using System;
using Xlent.Lever.Library.Core.Exceptions.Service;

namespace Xlent.Lever.Library.WebApi.Exceptions.Dal.Client
{
    /// <summary>
    /// Authorization was missing or not accepted.
    /// </summary>
    public class UnauthorizedException : ClientException
    {
        public const string ExceptionTypeId = "8f4c4dde-7faf-43e8-9f7b-36ff9a24e56d";
        public UnauthorizedException() : this((string)null, null) { }
        public UnauthorizedException(string message) : this(message, null) { }
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
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
