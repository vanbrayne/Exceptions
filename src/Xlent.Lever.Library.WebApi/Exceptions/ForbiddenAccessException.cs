using System;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// Authorization existed, but access was forbidden.
    /// </summary>
    public class ForbiddenAccessException : FulcrumException, IClientException
    {
        public static ForbiddenAccessException Create(string message, Exception innerException)
        {
            return new ForbiddenAccessException(message, innerException);
        }
        public const string ExceptionType = "Xlent.Fulcrum.ForbiddenAccess";
        public ForbiddenAccessException() : this((string)null, null) { }
        public ForbiddenAccessException(string message) : this(message, null) { }
        public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string Type => ExceptionType;

        private void SetProperties()
        {
            FriendlyMessage =
                "The system could identify you, but you did not have right to access the resource.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#ForbiddenAccessException";
        }
    }
}
