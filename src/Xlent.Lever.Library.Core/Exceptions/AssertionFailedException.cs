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
        public static AssertionFailedException Create(string message, Exception innerException)
        {
            return new AssertionFailedException(message, innerException);
        }

        public const string ExceptionType = "Xlent.Fulcrum.AssertionFailed";
        public AssertionFailedException() : this((string)null, null) { }
        public AssertionFailedException(string message) : this(message, null) { }
        public AssertionFailedException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        public override bool IsRetryMeaningful => false;
        public override string Type => ExceptionType;

        private void SetProperties()
        {

            FriendlyMessage =
                "An assertion made by the programmer proved to be wrong and the request couldn't be properly fulfilled.";
            FriendlyMessage += "Please report the following:";
            FriendlyMessage += $"\rCorrelactionId: {CorrelationId}";
            FriendlyMessage += $"\rInstanceId: {InstanceId}";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#AssertFailedException";
        }
    }
}
