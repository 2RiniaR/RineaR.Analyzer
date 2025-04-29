using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FirstOrDefaultAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PROHIBIT_FIRST_OR_DEFAULT";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "FirstOrDefaultの使用は禁止されています",
            messageFormat: "FirstOrDefaultの使用は禁止されています。代わりに専用の拡張メソッドを使用してください。",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "FirstOrDefaultは禁止です。拡張メソッドを利用してください。");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            // メソッド名を取得
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.Identifier.Text;

                if (methodName == "FirstOrDefault")
                {
                    // LINQ拡張か、直接メソッド呼び出しなのかは問わず、とりあえずFirstOrDefaultを禁止
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        memberAccess.Name.GetLocation());

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}