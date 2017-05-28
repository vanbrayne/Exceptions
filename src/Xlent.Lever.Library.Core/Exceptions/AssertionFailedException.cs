using System;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.Core.Exceptions
{
    /// <summary>
    /// The server failed to execute the request due to an assertion made by the programmer that proved to be wrong.
    /// </summary>
    /// <example>
    /// The programmer was sure that a certain condition would never be met, so the programmer just added an if-statement with this exception.
    /// </example>
    /// <remarks>
    /// This is basically a "Programmers Error", a bug on the server side.
    /// </remarks>
    public class AssertionFailedException : FulcrumException, IServerException
    {
        public const string ExceptionTypeId = "f736f9dd-db95-427e-a231-48e9361af921";
        public AssertionFailedException() : this((string)null, null) { }
        public AssertionFailedException(string message) : this(message, null) { }
        public AssertionFailedException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string TypeId => ExceptionTypeId;

        private void SetProperties()
        {
            // TODO: Set the following properties if they haven't been set already: TechnicalMessage, FriendlyMessage, MoreInfoUrl, FriendlyMessageId
        }
    }
}
