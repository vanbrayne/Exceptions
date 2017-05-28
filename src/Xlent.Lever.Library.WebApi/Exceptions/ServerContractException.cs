﻿using System;
using Xlent.Lever.Library.Core.Exceptions;
using Xlent.Lever.Library.Core.Exceptions.Interfaces;

namespace Xlent.Lever.Library.WebApi.Exceptions
{
    /// <summary>
    /// There was something wrong with the request itself, i.e. syntax, values out of range, etc.
    /// </summary>
    public class ServerContractException : FulcrumException, IClientException
    {
        public static ServerContractException Create(string message, Exception innerException)
        {
            return new ServerContractException(message, innerException);
        }
        public const string ExceptionTypeId = "659f879a-299a-4c11-921f-466fde971c13";
        public ServerContractException() : this((string)null, null) { }
        public ServerContractException(string message) : this(message, null) { }
        public ServerContractException(string message, Exception innerException) : base(message, innerException)
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
