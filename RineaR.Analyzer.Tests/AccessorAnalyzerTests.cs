using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace RineaR.Analyzer.Tests;

public class AccessorAnalyzerTests
{
    private async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expectedDiagnostics)
    {
        var test = new CSharpAnalyzerTest<AccessorAnalyzer, XUnitVerifier>
        {
            TestCode = source
        };

        test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
        await test.RunAsync();
    }
    
    [Fact]
    public async Task ReportsDiagnostic_ForIndexerAccess()
    {
        var source = @"
class C
{
    void M(string[] s)
    {
        var item = s[0];
    }
}";
        var expected = new DiagnosticResult(AccessorAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
            .WithSpan(6, 21, 6, 22) // [ の位置
            .WithArguments("[]");
        await VerifyAnalyzerAsync(source, expected);
    }
}