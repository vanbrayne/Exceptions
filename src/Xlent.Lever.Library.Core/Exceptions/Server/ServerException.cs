﻿namespace Xlent.Lever.Library.Core.Exceptions.Server
{
    /// <summary>
    /// Base class for exceptions that indicates that error is on the server side, i.e. the called function/service.
    /// </summary>
    public abstract class ServerException : FulcrumException
    {
        protected ServerException() : this((string)null, null) { }
        protected ServerException(string message) : this(message, null) { }
        protected ServerException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
