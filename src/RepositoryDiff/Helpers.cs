using System.Collections.Generic;
using System.Linq;

namespace KsWare.RepositoryDiff
{
    internal static class Helpers
    {
        public static void CreateHierarchy(IList<CompareResult> results)
        {
            foreach (var result in results)
            {
                result.Children = results.Where(x => x.Directory==result.RelativPath).ToList();
            }
        }

        public static string GetDirectory(string relativPath)
        {
            if (string.IsNullOrEmpty(relativPath)) return null;
            var components = relativPath.Split('\\');
            if (components.Length == 1) return "";
            return string.Join("\\", components.Take(components.Length - 1));
        }
    }
}