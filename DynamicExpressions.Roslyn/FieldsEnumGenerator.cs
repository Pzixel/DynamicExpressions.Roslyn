using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DynamicExpressions.Roslyn
{
    public class FieldsEnumGenerator : ICodeGenerator
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public FieldsEnumGenerator(AttributeData _)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context,
            IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (!(context.ProcessingNode is TypeDeclarationSyntax typeDeclaration))
            {
                return Task.FromResult(List<MemberDeclarationSyntax>());
            }

            var propertiesNames = typeDeclaration.Members.OfType<PropertyDeclarationSyntax>()
                .Select(x => x.Identifier.ValueText);

            var generatedEnum = EnumDeclaration(typeDeclaration.Identifier.ValueText + "Field")
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .AddMembers(
                propertiesNames.Select(EnumMemberDeclaration).ToArray());

            return Task.FromResult(List<MemberDeclarationSyntax>().Add(generatedEnum));
        }
    }
}
