namespace Xlent.Lever.Library.Core.Exceptions.Service
{
    /// <summary>
    /// Base class for exceptions that indicates that error is on the server side, i.e. the called function/service.
    /// </summary>
    public abstract class ServerException : FulcrumException
    {
        protected ServerException() { }
        protected ServerException(string message) : base(message) { }
        protected ServerException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
