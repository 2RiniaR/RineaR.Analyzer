using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    public static class Utility
    {
        public static bool IsTarget(SyntaxNodeAnalysisContext context)
        {
            var settings = Settings.Load(context.Options, context.CancellationToken);
            if (settings == null)
            {
                return true;
            }

            return settings.IsTarget(context.Compilation.AssemblyName, context.Node.SyntaxTree.FilePath);
        }
    }
}