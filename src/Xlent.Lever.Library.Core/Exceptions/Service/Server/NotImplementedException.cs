using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Server
{
    /// <summary>
    /// The request requires some functionality that deliberately has been left out, at least for the moment.
    /// </summary>
    /// <example>
    /// During testing, there could be parts of the code that hasn't been developed yet. Then throwing this exception is appropriate.
    /// </example>
    public class NotImplementedException : ServerException
    {
        public const string ExceptionTypeId = "573f806f-56b9-48a0-86d9-15d3bf6c9c6c";
        public NotImplementedException() : base() { }
        public NotImplementedException(string message) : base(message) { }
        public NotImplementedException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => true;
    }
}
