using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AccessorAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PROHIBIT_ACCESSOR_OPERATORS";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "禁止された演算子または構文の使用",
            messageFormat: "演算子 '{0}' の使用は禁止されています",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "[] の使用は禁止されています。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeElementAccessExpression, SyntaxKind.ElementAccessExpression);
        }

        private static void AnalyzeElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (Utility.IsTarget(context) == false) return;
            
            var node = (ElementAccessExpressionSyntax)context.Node;
            // 通常のインデクサアクセス: []
            var openBracketToken = node.ArgumentList.OpenBracketToken;
            ReportDiagnostic(context, openBracketToken, "[]");
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken token, string operatorText)
        {
            var diagnostic = Diagnostic.Create(Rule, token.GetLocation(), operatorText);
            context.ReportDiagnostic(diagnostic);
        }
    }
}