using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace RineaR.Analyzer.Tests
{
    public class IfNestAnalyzerTests
    {
        private async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
        {
            var test = new CSharpAnalyzerTest<IfNestAnalyzer, XUnitVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync();
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenIfNestedTooDeep()
        {
            var source = @"
class C
{
    void M()
    {
        if (true)
        {
            if (true)
            {
                if (true)
                {
                }
            }
        }
    }
}";
            var expected = new DiagnosticResult(IfNestAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(10, 17, 10, 19) // 3段目の if の位置
                .WithArguments(3);
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task NoDiagnostic_WhenIfNestedTwice()
        {
            var source = @"
class C
{
    void M()
    {
        if (true)
        {
            if (true)
            {
            }
        }
    }
}";
            await VerifyAnalyzerAsync(source);
        }
    }
}
