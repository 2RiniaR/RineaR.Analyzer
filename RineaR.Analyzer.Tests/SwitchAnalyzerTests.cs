using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace RineaR.Analyzer.Tests
{
    public class SwitchAnalyzerTests
    {
        private async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
        {
            var test = new CSharpAnalyzerTest<SwitchAnalyzer, XUnitVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync();
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenTooManyCases()
        {
            var source = @"
class C
{
    void M(int x)
    {
        switch (x)
        {
            case 1: break;
            case 2: break;
            case 3: break;
            case 4: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 8: break;
            case 9: break;
            case 10: break;
        }
    }
}";
            var expected = new DiagnosticResult(SwitchAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(6, 9, 6, 15) // switch の位置
                .WithArguments(10, 1); // 10個のcase、最大1行
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenCaseBodyTooLong()
        {
            var source = @"
class C
{
    void M(int x)
    {
        switch (x)
        {
            case 1:
                int a = 0;
                int b = 1;
                int c = 2;
                int d = 3;
                int e = 4;
                int f = 5;
                int g = 6;
                int h = 7;
                int i = 8;
                int j = 9;
                break;
        }
    }
}";
            var expected = new DiagnosticResult(SwitchAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(6, 9, 6, 15) // switch の位置
                .WithArguments(1, 11); // 1個のcase、最大11行
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task NoDiagnostic_WhenNormalSwitch()
        {
            var source = @"
class C
{
    void M(int x)
    {
        switch (x)
        {
            case 1: break;
            case 2: break;
            case 3: break;
            case 4: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 8: break;
            case 9: break;
        }
    }
}";
            await VerifyAnalyzerAsync(source);
        }
    }
}
