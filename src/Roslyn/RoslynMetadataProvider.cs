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
using Task = System.Threading.Tasks.Task;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynMetadataProvider : IMetadataProvider
    {
        private readonly Workspace workspace;

        public RoslynMetadataProvider()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            workspace = componentModel?.GetService<VisualStudioWorkspace>();
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
            return Task.WhenAll(workspace.CurrentSolution.Projects.Where(p => settings.IncludedProjects.Contains(p.FilePath)).Select(p => p.GetCompilationAsync()))
                .Result
                .Where(c => c != null).ToList().SelectMany(compilation => compilation.SyntaxTrees.Select(syntaxTree => compilation.GetSemanticModel(syntaxTree)))
                .SelectMany(semanticModel => semanticModel.SyntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Where(invocationExpression =>
                {
                    if (invocationExpression.Expression.ToString().Contains("RegisterService"))
                        return true;

                    if (invocationExpression.Expression.ToString().Contains(".State"))
                        return invocationExpression.Expression.ToString().Contains(".Trigger");

                    return false;
                }).Select(invocationExpression => new
                {
                    syntax = invocationExpression,
                    symbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol,
                    semanticModel
                })).ToList().Where(m =>
                {
                    if (m.symbol?.Name == "RegisterService" && m.symbol.Kind == SymbolKind.Method && ((IMethodSymbol)m.symbol).ReceiverType.Name == "IRequestHandlerFactory")
                        return true;
                    if (m.symbol?.Name == "Trigger" && m.symbol.Kind == SymbolKind.Method)
                        return ((IMethodSymbol)m.symbol).ReceiverType.Name.Contains("IStateConfigurationBuilder");
                    return false;
                }).Select(m => new RoslynServiceRegistrationMetadata(m.syntax, (IMethodSymbol)m.symbol, m.semanticModel)).ToImmutableList();
        }
    }
}
