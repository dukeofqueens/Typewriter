using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public class ServiceRegistrationImpl : ServiceRegistration
    {
        private readonly IServiceRegistrationMetadata _metadata;

        public override string RequestTypeName => _metadata.RequestTypeName;

        public override string ResponseTypeName => _metadata.ResponseTypeName;

        public override string ServiceName => _metadata.ServiceName;

        public override string WorkflowName => _metadata.WorkflowName;

        ServiceRegistrationImpl(IServiceRegistrationMetadata metadata)
        {
            _metadata = metadata;            
        }

        public static ServiceRegistrationCollection FromMetadata(IEnumerable<IServiceRegistrationMetadata> metadata)
        {
            return new ServiceRegistrationCollectionImpl(metadata.Select(i => new ServiceRegistrationImpl(i)));
        }
    }
}
