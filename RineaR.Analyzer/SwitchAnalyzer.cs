using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SwitchAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PROHIBIT_COMPLEX_SWITCH";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "複雑な switch 文は禁止されています",
            messageFormat: "switch文が複雑すぎます（caseが{0}個、中身が最大{1}行）",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "caseが10個以上、またはcase内部が10行以上のswitch文は禁止です。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
        }

        private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            var sectionCount = switchStatement.Sections.Count;
            if (sectionCount == 0)
            {
                return; // caseがないswitchは無視
            }

            // case数チェック
            bool tooManyCases = sectionCount >= 10;

            // case内行数チェック
            bool tooLongCase = switchStatement.Sections
                .Select(CountLines)
                .Any(lineCount => lineCount >= 10);

            if (tooManyCases || tooLongCase)
            {
                // 最初のswitchキーワードにエラーを出す
                var diagnostic = Diagnostic.Create(
                    Rule,
                    switchStatement.SwitchKeyword.GetLocation(),
                    sectionCount,
                    switchStatement.Sections.Select(CountLines).Max()
                );

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static int CountLines(SwitchSectionSyntax section)
        {
            if (section.Statements.Count == 0)
            {
                return 0;
            }

            var first = section.Statements.First().GetLocation().GetLineSpan().StartLinePosition.Line;
            var last = section.Statements.Last().GetLocation().GetLineSpan().EndLinePosition.Line;
            return last - first + 1;
        }
    }
}