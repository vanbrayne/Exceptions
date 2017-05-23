using System;
using Xlent.Lever.Library.Core.Exceptions.Service.Server;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Client
{
    /// <summary>
    /// Authorization was missing or not accepted.
    /// </summary>
    public class UnauthorizedException : ClientException
    {
        public const string ExceptionTypeId = "8f4c4dde-7faf-43e8-9f7b-36ff9a24e56d";
        public UnauthorizedException() : base() { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => true;

        public override FulcrumException FromServerToClient(string serverTechnicalName)
        {
            serverTechnicalName = serverTechnicalName ?? "server";
            var e = new AssertionFailedException($"Made an unauthorized call to {serverTechnicalName}: {Message}")
            {
                InstanceId = InstanceId
            };
            return e;
        }
    }
}
