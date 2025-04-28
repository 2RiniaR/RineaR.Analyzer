using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfNestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PROHIBIT_DEEP_IF_NEST";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "if文のネストが深すぎます",
            messageFormat: "if文が{0}段にネストされています（最大2段まで許可）",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "if文のネストは2段以内に制限されています。3段以上は禁止です。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (Utility.IsTarget(context) == false) return;
            
            var ifStatement = (IfStatementSyntax)context.Node;

            int nestLevel = 0;
            SyntaxNode? current = ifStatement;

            // 親ノードをたどりながら、if文をカウント
            while (current != null)
            {
                if (current is IfStatementSyntax)
                {
                    nestLevel++;
                }
                current = current.Parent;
            }

            if (nestLevel >= 3)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    ifStatement.IfKeyword.GetLocation(),
                    nestLevel
                );

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}