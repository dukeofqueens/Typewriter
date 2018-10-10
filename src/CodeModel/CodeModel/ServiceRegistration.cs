using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("ServiceRegistration", "ServiceRegistrations")]
    public abstract class ServiceRegistration : Item
    {
        public abstract string RequestTypeName { get;  }
        public abstract string ResponseTypeName { get;  }
        public abstract string ServiceName { get;  }
        public abstract string WorkflowName { get;  }
    }

    public interface ServiceRegistrationCollection : ItemCollection<ServiceRegistration>
    {
    }
}
