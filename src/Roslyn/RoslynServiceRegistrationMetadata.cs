using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynServiceRegistrationMetadata : IServiceRegistrationMetadata
    {
        private readonly IMethodSymbol _methodSymbol;

        public string RequestTypeName { get; set; }
        public string ResponseTypeName { get; set; }
        public string ServiceName { get; set; }
        public string WorkflowName { get; set; }
        public bool IsPublic {get;set;}

        public RoslynServiceRegistrationMetadata(InvocationExpressionSyntax syntax, IMethodSymbol methodSymbol, SemanticModel model)
        {
            this._methodSymbol = methodSymbol;
            ImmutableArray<ITypeSymbol> typeArguments;
            ImmutableArray<IParameterSymbol> parameters;

            if (syntax.Expression.ToString().Contains("RegisterService"))
            {
                parameters = _methodSymbol.Parameters;
                typeArguments = this._methodSymbol.TypeArguments;
                
                //RegisterService()
                //Add support for this if required.
                if(typeArguments.Length == 0)
                {
                    return;                    
                    ServiceName = syntax.ArgumentList.Arguments[0].GetFirstToken().ValueText;
                }

                //RegisterService<THandler>
                if(typeArguments.Length == 1)
                {
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

                //RegisterService<TRequest, TResponse, THandler>
                if(typeArguments.Length == 3)
                {
                    RequestTypeName = _methodSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    ResponseTypeName = _methodSymbol.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    ServiceName = syntax.ArgumentList.Arguments[0].GetFirstToken().ValueText;
                }

                IsPublic = syntax.ToString().Contains("ServiceVisibility.Public");
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

               

        
    }
}
