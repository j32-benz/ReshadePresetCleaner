using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

class ReshadePresetCleaner
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: ReshadePresetCleaner <path-to-ini-file> [--keep-layer]");
            return;
        }

        string filePath = args[0];
        bool keepLayer = args.Contains("--keep-layer");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        var iniLines = File.ReadAllLines(filePath);
        var techniques = ExtractTechniques(iniLines);

        if (!keepLayer)
        {
            techniques.Remove("Layer.fx");
        }

        var cleanedLines = RemoveUnnecessarySections(iniLines, techniques, keepLayer);

        string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;
        string cleanedFilePath = Path.Combine(directoryPath, "cleaned_" + Path.GetFileName(filePath));
        File.WriteAllLines(cleanedFilePath, cleanedLines);
        Console.WriteLine($"Cleaned INI file saved to {cleanedFilePath}");
    }

    private static HashSet<string> ExtractTechniques(string[] lines)
    {
        var techniques = new HashSet<string>();
        foreach (var line in lines)
        {
            if (line.StartsWith("Techniques=", StringComparison.OrdinalIgnoreCase))
            {
                var techniqueLine = line.Substring("Techniques=".Length);
                foreach (var technique in techniqueLine.Split(','))
                {
                    var match = Regex.Match(technique, @"@(\w+\.fx)");
                    if (match.Success)
                    {
                        techniques.Add(match.Groups[1].Value);
                    }
                }
                break;
            }
        }
        return techniques;
    }

    private static IEnumerable<string> RemoveUnnecessarySections(string[] lines, HashSet<string> techniques, bool keepLayer)
    {
        var cleanedLines = new List<string>();
        bool inRelevantSection = true;
        string? sectionName = null;

        foreach (var line in lines)
        {
            if (line.StartsWith("Techniques=", StringComparison.OrdinalIgnoreCase))
            {
                var techniquesLine = line.Substring("Techniques=".Length);
                var filteredTechniques = string.Join(",", techniquesLine.Split(',')
                    .Where(t => keepLayer || !t.Contains("Layer@Layer.fx")));

                cleanedLines.Add("Techniques=" + filteredTechniques);
                cleanedLines.Add(string.Empty);
            }

            var sectionMatch = Regex.Match(line, @"^\[(\w+\.fx)\]$");
            if (sectionMatch.Success)
            {
                sectionName = sectionMatch.Groups[1].Value;
                inRelevantSection = (keepLayer && sectionName == "Layer.fx") || techniques.Contains(sectionName);
            }

            if (inRelevantSection && sectionMatch.Success)
            {
                cleanedLines.Add(line);
            }
            else if (inRelevantSection && sectionName != null && !sectionMatch.Success)
            {
                cleanedLines.Add(line);
            }
        }

        return cleanedLines;
    }
}
