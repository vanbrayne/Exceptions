﻿using System;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// Authorization existed, but access was forbidden.
    /// </summary>
    public class ForbiddenAccessException : FulcrumException, IClientException
    {
        public static ForbiddenAccessException Create(string message, Exception innerException)
        {
            return new ForbiddenAccessException(message, innerException);
        }
        public const string ExceptionTypeId = "47792923-5412-4574-949a-bf2201c94748";
        public ForbiddenAccessException() : this((string)null, null) { }
        public ForbiddenAccessException(string message) : this(message, null) { }
        public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException)
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