using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.Metadata.Interfaces
{
    public interface IServiceRegistrationMetadata
    {
        string RequestTypeName { get; set; }
        string ResponseTypeName { get; set; }
        string ServiceName { get; set; }
        string WorkflowName { get; set; }
        bool IsPublic {get;set;}
    }
}
