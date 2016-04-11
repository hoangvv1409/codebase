using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure
{
    [Serializable]
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException() { }

        public ConcurrencyException(string message) : base(message) { }

        public ConcurrencyException(string message, Exception inner) : base(message, inner) { }

        protected ConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
