using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace RineaR.Analyzer.Tests
{
    public class TooManyParametersAnalyzerTests
    {
        private async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
        {
            var test = new CSharpAnalyzerTest<TooManyParametersAnalyzer, XUnitVerifier>
            {
                TestCode = source
            };
            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync();
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenMethodHasTooManyParameters()
        {
            var source = @"
class C
{
    void M(int a, int b, int c, int d, int e, int f, int g, int h, int i, int k) {}
}";
            var expected = new DiagnosticResult(TooManyParametersAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                .WithSpan(4, 10, 4, 11) // メソッド名 M の位置
                .WithArguments(10);
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task NoDiagnostic_WhenMethodHasFourParameters()
        {
            var source = @"
class C
{
    void M(int a, int b, int c, int d, int e, int f, int g, int h, int i) {}
}";
            await VerifyAnalyzerAsync(source);
        }
    }
}