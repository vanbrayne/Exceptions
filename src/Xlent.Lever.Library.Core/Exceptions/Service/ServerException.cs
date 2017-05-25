namespace Xlent.Lever.Library.Core.Exceptions.Service
{
    /// <summary>
    /// Base class for exceptions that indicates that error is on the server side, i.e. the called function/service.
    /// </summary>
    public abstract class ServerException : FulcrumException
    {
        protected ServerException() : this(null, null) { }
        protected ServerException(string message) : this(message, null) { }
        protected ServerException(IError error) : base(error) { }
        protected ServerException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
