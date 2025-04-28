using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullConditionalAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PROHIBIT_NULLABLE_OPERATORS";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "禁止された null 関連演算子の使用",
            messageFormat: "演算子 '{0}' の使用は禁止されています",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "?.、??、??=、?[] の使用は禁止されています。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeConditionalAccess, SyntaxKind.ConditionalAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeCoalesceExpression, SyntaxKind.CoalesceExpression);
            context.RegisterSyntaxNodeAction(AnalyzeCoalesceAssignmentExpression, SyntaxKind.CoalesceAssignmentExpression);
        }

        private static void AnalyzeConditionalAccess(SyntaxNodeAnalysisContext context)
        {
            if (Utility.IsTarget(context) == false) return;
            
            var node = (ConditionalAccessExpressionSyntax)context.Node;

            if (node.WhenNotNull is InvocationExpressionSyntax)
            {
                ReportDiagnostic(context, node.OperatorToken, "?.");
            }
            else if (node.WhenNotNull is ElementBindingExpressionSyntax)
            {
                ReportDiagnostic(context, node.OperatorToken, "?[]");
            }
            else
            {
                ReportDiagnostic(context, node.OperatorToken, "?.");
            }
        }

        private static void AnalyzeCoalesceExpression(SyntaxNodeAnalysisContext context)
        {
            if (Utility.IsTarget(context) == false) return;
            
            var node = (BinaryExpressionSyntax)context.Node;
            ReportDiagnostic(context, node.OperatorToken, "??");
        }

        private static void AnalyzeCoalesceAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            if (Utility.IsTarget(context) == false) return;
            
            var node = (AssignmentExpressionSyntax)context.Node;
            ReportDiagnostic(context, node.OperatorToken, "??=");
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken token, string operatorText)
        {
            var diagnostic = Diagnostic.Create(Rule, token.GetLocation(), operatorText);
            context.ReportDiagnostic(diagnostic);
        }
    }
}

