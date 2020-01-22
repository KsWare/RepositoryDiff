using System.Text.RegularExpressions;

namespace KsWare.RepositoryDiff
{
    public class FilterViewModel : NotifyPropertyChangedBase
    {
        public bool HideEqual { get => Get<bool>(); set => Set(value); }

        public FilterTermViewModel ResultA { get; } = new FilterTermViewModel();
        public FilterTermViewModel ResultB { get; } = new FilterTermViewModel();
        public FilterTermViewModel ResultC { get; } = new FilterTermViewModel();
        public FilterTermViewModel FileExtensions { get; } = new FilterTermViewModel();

        public string NameRegEx { get => Get<string>(); set => Set(value); }

        
        public FilterViewModel()
        {

        }

        public bool FilterFunction(object obj)
        {
            var c = (CompareResult) obj;
            return !c.IsHidden;
        }

        public bool Match(CompareResult c)
        {
            var is3Way = c.Result?.Length == 3;
            var is2Way = c.Result?.Length == 2;
            var isFile = !c.IsDirectory;

            if (HideEqual && (c.Result == "==" || c.Result == "===")) return false;
            if (isFile && ResultA.Match(c.ResultA)==false) return false;
            if (isFile && ResultB.Match(c.ResultB)==false) return false;
            if (isFile && ResultC.Match(c.ResultC)==false) return false;
            if (isFile && FileExtensions.Match(c.FileExtension)==false) return false;
            if (isFile && !string.IsNullOrWhiteSpace(NameRegEx) && Regex.IsMatch(c.Name, NameRegEx, RegexOptions.IgnoreCase) == false) return false;
            return true;
        }

        public RefreshCommand RefreshCommand { get; set; }
    }
}