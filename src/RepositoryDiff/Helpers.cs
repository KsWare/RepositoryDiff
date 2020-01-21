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

        public static void HideRecursive(IEnumerable<CompareResult> children)
        {
            foreach (var child in children)
            {
                child.IsHidden = true;
                HideRecursive(child.Children);
            }
        }

        public static void ShowRecursive(IEnumerable<CompareResult> children)
        {
            foreach (var child in children)
            {
                child.IsHidden = false;
                if(child.IsExpanded) ShowRecursive(child.Children); else HideRecursive(child.Children);
            }
        }
        
    }
}