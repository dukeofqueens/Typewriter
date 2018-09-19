using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynServiceRegistrationMetadata : IServiceRegistrationMetadata
    {
        private readonly IMethodSymbol _methodSymbol;

        public RoslynServiceRegistrationMetadata(InvocationExpressionSyntax syntax, IMethodSymbol methodSymbol)
        {
            _methodSymbol = methodSymbol;
            if(_methodSymbol.Parameters.Length == 3)
            {
                RequestTypeName = _methodSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                ResponseTypeName = _methodSymbol.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                ServiceName = syntax.ArgumentList.Arguments[0].GetFirstToken().ValueText;
            }
        }

        public string RequestTypeName { get; set; }
        public string ResponseTypeName { get; set; }
        public string ServiceName { get; set; }
    }
}
