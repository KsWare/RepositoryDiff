using System.Linq;
using System.Text.RegularExpressions;
using KsWare.RepositoryDiff.Common;
using KsWare.RepositoryDiff.UI.MainWindow.Commands;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.UI.Filter
{
    public class FilterViewModel : NotifyPropertyChangedBase
    {
        private FilterData _data;

        public FilterViewModel()
        {
            Data=new FilterData();
        }     
        
        public bool ShowEmptyFolders { get =>_data.ShowEmptyFolders; set => Set(()=>_data.ShowEmptyFolders,v=>_data.ShowEmptyFolders=v,value); }
        public bool HideEqual { get => _data.HideEqual; set => Set(()=>_data.HideEqual,v=>_data.HideEqual=v,value); }

        public ResultFilterViewModel ResultA { get; } = new ResultFilterViewModel();
        public ResultFilterViewModel ResultB { get; } = new ResultFilterViewModel();
        public ResultFilterViewModel ResultC { get; } = new ResultFilterViewModel();
        public FilterTermViewModel FileExtensions { get; } = new FilterTermViewModel();
        public string NameRegEx { get => _data.NameRegEx; set => Set(()=>_data.NameRegEx,v=>_data.NameRegEx=v,value); }

        public FilterData Data
        {
            get
            {
                _data.ResultA = ResultA.Data;
                _data.ResultB = ResultB.Data;
                _data.ResultC = ResultC.Data;
                return _data;
            }
            set
            { 
                _data = value;
                ResultA.Data=_data.ResultA;
                ResultB.Data=_data.ResultB;
                ResultC.Data=_data.ResultC;

                OnPropertyChanged(string.Empty);
            } 
        }


        public bool FilterFunction(object obj)
        {
            var c = (CompareResultViewModel) obj;
            return !c.IsHidden;
        }

        public bool Match(CompareResultViewModel c)
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

    public class FilterData
    {
        public bool ShowEmptyFolders { get; set; }

        public bool HideEqual { get; set; }

        public string NameRegEx { get; set; }

        public ResultFilterData ResultA { get; set; } = new ResultFilterData();
        public ResultFilterData ResultB { get; set; } = new ResultFilterData();
        public ResultFilterData ResultC { get; set; } = new ResultFilterData();
    }

    public class FilterTermData
    {
        public string Not { get; set; }

        public string Value { get; set; }

        public string Operator { get; set; }
    }
}