using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Typewriter.Configuration;
using Typewriter.Metadata.Interfaces;
using Typewriter.Metadata.Providers;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynMetadataProvider : IMetadataProvider
    {
        private readonly Workspace workspace;

        public RoslynMetadataProvider()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            this.workspace = componentModel?.GetService<VisualStudioWorkspace>();
        }

        public IFileMetadata GetFile(string path, Settings settings, Action<string[]> requestRender)
        {
            var document = workspace.CurrentSolution.GetDocumentIdsWithFilePath(path).FirstOrDefault();
            if (document != null)
            {
                var fileMetadata = new RoslynFileMetadata(workspace.CurrentSolution.GetDocument(document), settings, requestRender);

                fileMetadata.ServiceRegistrations = _registrations ?? GetServiceRegistrations(settings);

                return fileMetadata;
            }

            return null;
        }

        private IEnumerable<IServiceRegistrationMetadata> _registrations;

        public void RefreshRegistrations()
        {
            _registrations = null;
        }

        private IEnumerable<IServiceRegistrationMetadata> GetServiceRegistrations(Settings settings)
        {
           
            var compilations = System.Threading.Tasks.Task.WhenAll(workspace.CurrentSolution.Projects.Where(p => settings.IncludedProjects.Contains(p.FilePath)).Select(p => p.GetCompilationAsync())).Result.Where(c => c != null).ToList();

            var invocations = compilations.SelectMany(compilation => compilation.SyntaxTrees.Select(syntaxTree => compilation.GetSemanticModel(syntaxTree)))
                .SelectMany(
                    semanticModel => semanticModel
                        .SyntaxTree
                        .GetRoot()
                        .DescendantNodes()
                        .OfType<InvocationExpressionSyntax>().Where(invocationExpression => invocationExpression.Expression.ToString().Contains("RegisterService"))
                        .Select(invocationExpression => new { syntax = invocationExpression, symbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol})
                ).ToList();

            
            return invocations
                .Where(m => m.symbol?.Name == "RegisterService" && m.symbol.Kind == SymbolKind.Method && ((IMethodSymbol)m.symbol).ReceiverType.Name == "IRequestHandlerFactory")
                .Select(m => new RoslynServiceRegistrationMetadata(m.syntax, (IMethodSymbol)m.symbol))
                .ToImmutableList();


        }

    }
}
