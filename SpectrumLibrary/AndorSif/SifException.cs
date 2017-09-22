using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    [Serializable]
    public class SifException : Exception
    {
        internal SifException() { }
        internal SifException(string message) : base(message) { }
        internal SifException(string message, Exception inner) : base(message, inner) { }


        protected SifException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
