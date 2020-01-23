using System.Collections.Generic;
using System.Linq;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff
{
    internal static class Helpers
    {
        public static void CreateHierarchy(IEnumerable<CompareResultViewModel> results)
        {
            foreach (var result in results.Where(x=>x.IsDirectory))
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