using System;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to failure events.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <c>ErrorEventArgs</c> class.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        protected ErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// Exception related to the event.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Flag that indicates whether the exception was handled.
        /// If handler sets this to <c>true</c> then engine continues execution,
        /// otherwise exception is rethrown and terminates engine's execution.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}