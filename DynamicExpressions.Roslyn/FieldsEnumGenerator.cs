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
                .Select(x => x.Identifier.ValueText).ToArray();

            var generatedEnum = EnumDeclaration(typeDeclaration.Identifier.ValueText + "Field")
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .AddMembers(propertiesNames.Select(EnumMemberDeclaration).ToArray());

            var helperClass = ClassDeclaration(typeDeclaration.Identifier.ValueText + "ExpressionHelper")
                .AddMembers(FieldDeclaration(
                        VariableDeclaration(
                                GenericName(
                                        Identifier("System.Collections.Generic.Dictionary"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SeparatedList<TypeSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    IdentifierName(generatedEnum.Identifier),
                                                    Token(SyntaxKind.CommaToken),
                                                    GenericName(
                                                            Identifier("System.Linq.Expressions.Expression"))
                                                        .WithTypeArgumentList(
                                                            TypeArgumentList(
                                                                SingletonSeparatedList<TypeSyntax>(
                                                                    GenericName(
                                                                            Identifier("Func"))
                                                                        .WithTypeArgumentList(
                                                                            TypeArgumentList(
                                                                                SeparatedList<TypeSyntax>(
                                                                                    new SyntaxNodeOrToken[]
                                                                                    {
                                                                                        IdentifierName(typeDeclaration
                                                                                            .Identifier),
                                                                                        Token(SyntaxKind.CommaToken),
                                                                                        PredefinedType(
                                                                                            Token(SyntaxKind
                                                                                                .ObjectKeyword))
                                                                                    }))))))
                                                }))))
                            .WithVariables(
                                SingletonSeparatedList(
                                    VariableDeclarator(
                                            Identifier("FieldExpressions"))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                ObjectCreationExpression(
                                                        GenericName(
                                                                Identifier("System.Collections.Generic.Dictionary"))
                                                            .WithTypeArgumentList(
                                                                TypeArgumentList(
                                                                    SeparatedList<TypeSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            IdentifierName(generatedEnum.Identifier),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            GenericName(
                                                                                    Identifier("System.Linq.Expressions.Expression"))
                                                                                .WithTypeArgumentList(
                                                                                    TypeArgumentList(
                                                                                        SingletonSeparatedList<
                                                                                            TypeSyntax>(
                                                                                            GenericName(
                                                                                                    Identifier("Func"))
                                                                                                .WithTypeArgumentList(
                                                                                                    TypeArgumentList(
                                                                                                        SeparatedList<
                                                                                                            TypeSyntax>(
                                                                                                            new
                                                                                                                SyntaxNodeOrToken
                                                                                                                []
                                                                                                                {
                                                                                                                    IdentifierName(
                                                                                                                        typeDeclaration
                                                                                                                            .Identifier),
                                                                                                                    Token(
                                                                                                                        SyntaxKind
                                                                                                                            .CommaToken),
                                                                                                                    PredefinedType(
                                                                                                                        Token(
                                                                                                                            SyntaxKind
                                                                                                                                .ObjectKeyword))
                                                                                                                }))))))
                                                                        }))))
                                                    .WithInitializer(
                                                        InitializerExpression(
                                                            SyntaxKind.ObjectInitializerExpression,
                                                            SeparatedList<ExpressionSyntax>(
                                                                propertiesNames.Select(propName =>
                                                                        (SyntaxNodeOrToken) AssignmentExpression(
                                                                            SyntaxKind.SimpleAssignmentExpression,
                                                                            ImplicitElementAccess()
                                                                                .WithArgumentList(
                                                                                    BracketedArgumentList(
                                                                                        SingletonSeparatedList<
                                                                                            ArgumentSyntax>(
                                                                                            Argument(
                                                                                                MemberAccessExpression(
                                                                                                    SyntaxKind
                                                                                                        .SimpleMemberAccessExpression,
                                                                                                    IdentifierName(
                                                                                                        generatedEnum
                                                                                                            .Identifier),
                                                                                                    IdentifierName(
                                                                                                        propName)))))),
                                                                            SimpleLambdaExpression(
                                                                                Parameter(
                                                                                    Identifier("x")),
                                                                                MemberAccessExpression(
                                                                                    SyntaxKind
                                                                                        .SimpleMemberAccessExpression,
                                                                                    IdentifierName("x"),
                                                                                    IdentifierName(propName)))))
                                                                    .SelectMany(
                                                                        x => new[]
                                                                        {
                                                                            x, Token(SyntaxKind.CommaToken)
                                                                        })))))))))
                    .WithModifiers(
                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))));

            return Task.FromResult(List<MemberDeclarationSyntax>().Add(generatedEnum).Add(helperClass));
        }
    }
}
