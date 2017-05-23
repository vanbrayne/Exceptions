using System;

namespace Xlent.Lever.Library.Core.Exceptions.Service
{
    /// <summary>
    /// The base class for all Fulcrum exceptions
    /// </summary>
    public abstract class FulcrumException : System.Exception
    {
        private static string _serverTechnicalName;

        /// <summary>
        /// The technical name of the current running server instance.
        /// </summary>
        public string ServerTechnicalName { get; set; }
        /// <summary>
        /// A globally unique identifier for this exact instance of the exception.
        /// </summary>
        public string InstanceId { get; set; }
        /// <summary>
        /// A globally unique identifier for this type of exception.
        /// </summary>
        public abstract string TypeId { get; }
        /// <summary>
        /// If the same request is repeated, you will most likely get the same exception if this property is true.
        /// False means that you may try again in a moment.
        /// </summary>
        public abstract bool IsIdempotent { get; }

        protected FulcrumException() : base()
        {
            SetBasicInformation();
        }

        protected FulcrumException(string message) : base(message)
        {
            SetBasicInformation();
        }
        protected FulcrumException(string message, System.Exception innerException) : base(message, innerException)
        {
            SetBasicInformation();
        }

        public virtual FulcrumException FromServerToClient(string serverTechnicalName)
        {
            return this;
        }

        public static void Initialize(string serverTechnicalName)
        {
            if(serverTechnicalName == null) throw new ArgumentNullException(nameof(serverTechnicalName));
            serverTechnicalName = serverTechnicalName.ToLower();
            if (_serverTechnicalName != null && _serverTechnicalName != serverTechnicalName) throw new ApplicationException("Once the server name has been set, it can't be changed.");
            _serverTechnicalName = serverTechnicalName;
        }

        private void SetBasicInformation()
        {
            InstanceId = Guid.NewGuid().ToString();
        }
    }
}
