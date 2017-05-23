using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Server
{
    /// <summary>
    /// The service is either overloaded or unavailable at the moment.
    /// </summary>
    public class UnavailableException : ServerException
    {
        public const string ExceptionTypeId = "e97cb54a-1da4-4a5c-901e-ccee0e833b18";
        public TimeSpan RetryAfterTimeSpan { get; set; }
        public UnavailableException() : base() { }
        public UnavailableException(string message) : base(message) { }
        public UnavailableException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => false;
    }
}
