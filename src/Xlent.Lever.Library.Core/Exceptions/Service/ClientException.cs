namespace Xlent.Lever.Library.Core.Exceptions.Service
{
    /// <summary>
    /// Base class for exceptions that indicates that error is on the client side, i.e. the caller
    /// </summary>
    public abstract class ClientException : FulcrumException
    {
        protected ClientException() { }
        protected ClientException(string message) : base(message) { }
        protected ClientException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
