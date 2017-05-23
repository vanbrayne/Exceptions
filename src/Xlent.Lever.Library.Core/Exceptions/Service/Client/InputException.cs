using System;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Client
{
    /// <summary>
    /// There was something wrong with the request itself, i.e. syntax, values out of range, etc.
    /// </summary>
    public class InputException : ClientException
    {
        public const string ExceptionTypeId = "659f879a-299a-4c11-921f-466fde971c13";
        public InputException() : base() { }
        public InputException(string message) : base(message) { }
        public InputException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => true;

        public override FulcrumException FromServerToClient(string serverTechnicalName)
        {
            serverTechnicalName = serverTechnicalName ?? "server";
            var e = new AssertionFailedException($"Bad parameters sent to {serverTechnicalName}: {Message}")
            {
                InstanceId = InstanceId
            };
            return e;
        }
    }
}
