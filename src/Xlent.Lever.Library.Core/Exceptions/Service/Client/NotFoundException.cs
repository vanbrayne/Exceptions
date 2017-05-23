using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Client
{
    /// <summary>
    /// The specified item could not be found.
    /// </summary>
    /// <example>
    /// A request for a person with a specified Id that doesn't exist should always throw this exception.
    /// </example>
    /// <example>
    /// If a person exists, and the request is for a list of e-mail addresses and the person doesn't have any, 
    /// you should not throw this exception, but return an empty list.
    /// </example>
    public class NotFoundException : ClientException
    {
        public const string ExceptionTypeId = "8108ca0d-14a3-4732-8b58-eb4151fb222d";
        public NotFoundException() : base() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => false;
    }
}
