using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The request conflicted with a business rule.
    /// </summary>
    public class BusinessRuleException : FulcrumException, IClientException
    {
        public static BusinessRuleException Create(string message, Exception innerException)
        {
            return new BusinessRuleException(message, innerException);
        }
        public const string ExceptionTypeId = "f4ebb36f-1c1c-4f9e-bc4d-9b1d3e000823";
        public BusinessRuleException() : this((string)null, null) { }
        public BusinessRuleException(string message) : this(message, null) { }

        public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            FriendlyMessage =
                "The request conflicted with a business rules. Please make changes accordingly and try again.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#BusinessRuleException";
        }
    }
}
