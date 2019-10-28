using System;
using System.Diagnostics;

namespace Lenic.DI.Core
{
    /// <summary>
    /// Parse Exception
    /// </summary>
    [DebuggerStepThrough]
    public sealed class ParseException : Exception
    {
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="position">The position.</param>
        internal ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public int Position
        {
            get { return position; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} (at index {1})", Message, position);
        }
    }
}
