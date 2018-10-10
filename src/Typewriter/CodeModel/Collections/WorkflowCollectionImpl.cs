using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.CodeModel.Collections
{
    public class WorkflowCollectionImpl : ItemCollectionImpl<Workflow>, WorkflowCollection
    {
        public WorkflowCollectionImpl(IEnumerable<Workflow> values) : base(values)
        {
        }
    }
}
