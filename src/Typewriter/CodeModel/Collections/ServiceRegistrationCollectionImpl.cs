using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.CodeModel.Collections
{
    public class ServiceRegistrationCollectionImpl : ItemCollectionImpl<ServiceRegistration>, ServiceRegistrationCollection
    {
        public ServiceRegistrationCollectionImpl(IEnumerable<ServiceRegistration> values) : base(values)
        {
        }
    }
}
