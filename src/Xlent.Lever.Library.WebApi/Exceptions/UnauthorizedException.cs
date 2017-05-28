using System;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// Authorization was missing or not accepted.
    /// </summary>
    public class UnauthorizedException : FulcrumException, IClientException
    {
        public static UnauthorizedException Create(string message, Exception innerException)
        {
            return new UnauthorizedException(message, innerException);
        }
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
            FriendlyMessage =
                "Authorization was missing or not accepted.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#UnauthorizedException";
        }
    }
}
