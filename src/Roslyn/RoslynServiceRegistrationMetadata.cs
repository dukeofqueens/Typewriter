using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.PlatformUI;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynServiceRegistrationMetadata : IServiceRegistrationMetadata
    {
        private readonly IMethodSymbol _methodSymbol;

        public RoslynServiceRegistrationMetadata(InvocationExpressionSyntax syntax, IMethodSymbol methodSymbol, SemanticModel model)
        {
            this._methodSymbol = methodSymbol;
            ImmutableArray<IParameterSymbol> parameters;
            if (syntax.Expression.ToString().Contains("RegisterService"))
            {
                parameters = _methodSymbol.Parameters;
                if (parameters.Length == 3)
                {
                    this.RequestTypeName = _methodSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    this.ResponseTypeName = _methodSymbol.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    this.ServiceName = syntax.ArgumentList.Arguments[0].GetFirstToken().ValueText;
                }
            }

            ImmutableArray<ITypeSymbol> typeArguments;
            if (syntax.Expression.ToString().Contains("RegisterService"))
            {
                parameters = this._methodSymbol.Parameters;
                if (parameters.Length == 1)
                {
                    typeArguments = this._methodSymbol.TypeArguments;
                    INamedTypeSymbol namedTypeSymbol = typeArguments[0].AllInterfaces.FirstOrDefault(ts =>
                   {
                       if (ts.Name == "IRequestResponseHandler" && ts.IsGenericType)
                           return ts.TypeArguments.Length == 2;
                       return false;
                   });
                    typeArguments = namedTypeSymbol.TypeArguments;
                    RequestTypeName = typeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    typeArguments = namedTypeSymbol.TypeArguments;
                    ResponseTypeName = typeArguments[1].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    ServiceName = syntax.ArgumentList.Arguments[0].GetFirstToken().ValueText;
                }
            }

            if (!syntax.Expression.ToString().Contains("Trigger"))
                return;

            typeArguments = methodSymbol.TypeArguments;
            RequestTypeName = typeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            typeArguments = methodSymbol.TypeArguments;
            ResponseTypeName = typeArguments[1].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            if (syntax.ArgumentList.Arguments[0].Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                ServiceName = model.GetConstantValue(syntax.ArgumentList.Arguments[0].Expression).Value.ToString();
            else
                ServiceName = syntax.ArgumentList.Arguments[0].GetFirstToken().ValueText;

            var node = syntax.Parent;

            while (node != null)
            {
                if (node.Kind() == SyntaxKind.ClassDeclaration)
                {
                    WorkflowName = ((ClassDeclarationSyntax) node).Identifier.ValueText;
                    break;
                }
                node = node.Parent;
            }
        }


        

        public string RequestTypeName { get; set; }
        public string ResponseTypeName { get; set; }
        public string ServiceName { get; set; }
        public string WorkflowName { get; set; }
    }
}
