using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TooManyParametersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PROHIBIT_TOO_MANY_PARAMETERS";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "メソッドの引数が多すぎます",
            messageFormat: "メソッドの引数が{0}個あります（最大9個まで許可）",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "メソッドには9引数以内しか許可されません。10引数以上は禁止です。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            int parameterCount = methodDeclaration.ParameterList.Parameters.Count;

            if (parameterCount >= 10)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDeclaration.Identifier.GetLocation(),
                    parameterCount
                );

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}