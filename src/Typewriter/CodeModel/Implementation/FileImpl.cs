﻿using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class FileImpl : File
    {
        private readonly IFileMetadata _metadata;

        public FileImpl(IFileMetadata metadata)
        {
            _metadata = metadata;
        }

        public override string Name => _metadata.Name;
        public override string FullName => _metadata.FullName;

        private ClassCollection _classes;
        public override ClassCollection Classes => _classes ?? (_classes = ClassImpl.FromMetadata(_metadata.Classes, this));

        private DelegateCollection _delegates;
        public override DelegateCollection Delegates => _delegates ?? (_delegates = DelegateImpl.FromMetadata(_metadata.Delegates, this));

        private EnumCollection _enums;
        public override EnumCollection Enums => _enums ?? (_enums = EnumImpl.FromMetadata(_metadata.Enums, this));

        private InterfaceCollection _interfaces;
        public override InterfaceCollection Interfaces => _interfaces ?? (_interfaces = InterfaceImpl.FromMetadata(_metadata.Interfaces, this));

        private ServiceRegistrationCollection _serviceRegistrations;
        public override ServiceRegistrationCollection ServiceRegistrations => _serviceRegistrations ?? (_serviceRegistrations =  ServiceRegistrationImpl.FromMetadata(_metadata.ServiceRegistrations));

        private WorkflowCollection _workflows;
        public override WorkflowCollection Workflows => _workflows ?? (_workflows = new WorkflowCollectionImpl(ServiceRegistrations.Where(sr => sr.WorkflowName != null).GroupBy(sr => sr.WorkflowName).Select(g => new WorkflowImpl(g.Key,new ServiceRegistrationCollectionImpl(g.GroupBy(sr => sr.ServiceName).Select(srg => srg.First()).ToList())))));

        public override string ToString()
        {
            return Name;
        }
    }
}
