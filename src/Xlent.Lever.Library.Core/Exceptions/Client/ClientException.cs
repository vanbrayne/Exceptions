namespace Xlent.Lever.Library.Core.Exceptions.Client
{
    /// <summary>
    /// Base class for exceptions that indicates that error is on the client side, i.e. the caller
    /// </summary>
    public abstract class ClientException : FulcrumException
    {
        protected ClientException() : this((string)null, null) { }
        protected ClientException(string message) : this(message, null) { }
        protected ClientException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
