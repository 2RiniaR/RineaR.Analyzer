using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace RineaR.Analyzer.Tests
{
    public class FirstOrDefaultAnalyzerTests
    {
        [Fact]
        public async Task ReportDiagnostic_WhenUsingFirstOrDefaultWithoutPredicate()
        {
            var testCode = @"
using System.Linq;
using System.Collections.Generic;

class TestClass
{
    void Test()
    {
        var list = new List<int> { 1, 2, 3 };
        var value = list.{|#0:FirstOrDefault|}();
    }
}";

            var expected = new DiagnosticResult(FirstOrDefaultAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithLocation(0);

            var test = new CSharpAnalyzerTest<FirstOrDefaultAnalyzer, XUnitVerifier>
            {
                TestCode = testCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60, // ← System.Linq解決用
                ExpectedDiagnostics = { expected }
            };

            await test.RunAsync();
        }

        [Fact]
        public async Task ReportDiagnostic_WhenUsingFirstOrDefaultWithPredicate()
        {
            var testCode = @"
using System.Linq;
using System.Collections.Generic;

class TestClass
{
    void Test()
    {
        var list = new List<int> { 1, 2, 3 };
        var value = list.{|#0:FirstOrDefault|}(x => x > 1);
    }
}";

            var expected = new DiagnosticResult(FirstOrDefaultAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithLocation(0);

            var test = new CSharpAnalyzerTest<FirstOrDefaultAnalyzer, XUnitVerifier>
            {
                TestCode = testCode,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                ExpectedDiagnostics = { expected }
            };

            await test.RunAsync();
        }
    }
}