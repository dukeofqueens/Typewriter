using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.Metadata.Interfaces
{
    public interface ITypeWriterLog
    {
        void Debug(string message, params object[] parameters);
    }
}
