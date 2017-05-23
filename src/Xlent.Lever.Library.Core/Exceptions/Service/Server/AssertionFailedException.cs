﻿using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service.Server
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
    public class AssertionFailedException : ServerException
    {
        public const string ExceptionTypeId = "f736f9dd-db95-427e-a231-48e9361af921";
        public AssertionFailedException() : base() { }
        public AssertionFailedException(string message) : base(message) { }
        public AssertionFailedException(string message, Exception innerException) : base(message, innerException) { }
        public override string TypeId => ExceptionTypeId;
        public override bool IsIdempotent => true;
    }
}
