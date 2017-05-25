namespace Xlent.Lever.Library.Core.Exceptions.Service
{
    /// <summary>
    /// Base class for exceptions that indicates that error is on the client side, i.e. the caller
    /// </summary>
    public abstract class ClientException : FulcrumException
    {
        protected ClientException() : this(null, null) { }
        protected ClientException(string message) : this(message, null) { }
        protected ClientException(IError error) : base(error) { }
        protected ClientException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
