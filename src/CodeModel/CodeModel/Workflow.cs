using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Workflow", "WorkflowRegistrations")]
    public abstract class Workflow : Item
    {
        public abstract string Name { get; }
        public abstract ServiceRegistrationCollection Services { get; }
    }

    public interface WorkflowCollection : ItemCollection<Workflow>
    {

    }
}
