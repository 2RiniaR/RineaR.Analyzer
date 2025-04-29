using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace RineaR.Analyzer.Tests
{
    public class NullConditionalAnalyzerTests
    {
        private async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
        {
            var test = new CSharpAnalyzerTest<NullConditionalAnalyzer, XUnitVerifier>
            {
                TestCode = source
            };

            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync();
        }

        [Fact]
        public async Task ReportsDiagnostic_ForConditionalAccess()
        {
            var source = @"
class C
{
    void M(string s)
    {
        var len = s?.Length;
    }
}";
            var expected = new DiagnosticResult(NullConditionalAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(6, 20, 6, 21) // ?. の位置
                .WithArguments("?.");
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task ReportsDiagnostic_ForCoalesceOperator()
        {
            var source = @"
class C
{
    void M(string s)
    {
        var value = s ?? ""default"";
    }
}";
            var expected = new DiagnosticResult(NullConditionalAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(6, 23, 6, 25) // ?? の位置
                .WithArguments("??");
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task ReportsDiagnostic_ForNullConditionalIndexer()
        {
            var source = @"
class C
{
    void M(string[] s)
    {
        var item = s?[0];
    }
}";
            var expected = new DiagnosticResult(NullConditionalAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(6, 21, 6, 22) // ?[ の位置（?の部分）
                .WithArguments("?[]");
            await VerifyAnalyzerAsync(source, expected);
        }

        [Fact]
        public async Task NoDiagnostic_WhenNoProhibitedOperators()
        {
            var source = @"
class C
{
    void M(string s)
    {
        var len = s.Length;
        var result = s + ""suffix"";
    }
}";
            await VerifyAnalyzerAsync(source /* ← No expected diagnostics */);
        }
    
        [Fact]
        public async Task ReportsDiagnostic_ForCoalesceAssignmentOperator()
        {
            var source = @"
class C
{
    void M(string s)
    {
        s ??= ""default"";
    }
}";
            var expected = new DiagnosticResult(NullConditionalAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(6, 11, 6, 14) // ??= の位置
                .WithArguments("??=");
            await VerifyAnalyzerAsync(source, expected);
        }
    }
}