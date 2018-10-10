using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.CodeModel.Implementation
{
    public class WorkflowImpl : Workflow
    {
        private string _name { get; set; }
        
        public override string Name => _name;

        private ServiceRegistrationCollection _services;
        public override ServiceRegistrationCollection Services => _services;

        public WorkflowImpl(string name, ServiceRegistrationCollection services)
        {
            _name = name;
            _services = services;
        }
    }
}
