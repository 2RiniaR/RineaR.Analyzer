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

            var asseblyName = context.Compilation.AssemblyName;
            if (asseblyName == null)
            {
                return false;
            }
            return settings.IsTarget(asseblyName, context.Node.SyntaxTree.FilePath);
        }
    }
}