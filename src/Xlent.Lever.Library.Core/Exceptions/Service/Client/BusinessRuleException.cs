using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Client
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
    public class BusinessRuleException : ClientException
    {
        public const string ExceptionTypeId = "f4ebb36f-1c1c-4f9e-bc4d-9b1d3e000823";
        public BusinessRuleException() : base() { }
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => true;
    }
}
