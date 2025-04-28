using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RineaR.Analyzer
{
    public class Settings
    {
        public string[] TargetAssemblies { get; set; }
        public string[] ExcludePaths { get; set; }

        public static Settings Load(AnalyzerOptions options, CancellationToken ct)
        {
            var file = options.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == "RineaRAnalyzerSettings.json");
            if (file == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Settings>(file.GetText(ct).ToString());
        }

        public bool IsTarget(string assemblyName, string path)
        {
            if (TargetAssemblies == null || TargetAssemblies.Contains(assemblyName) == false)
            {
                return false;
            }

            if (ExcludePaths == null)
            {
                return true;
            }
            
            foreach (var excludePath in ExcludePaths)
            {
                if (path.Contains(excludePath))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

