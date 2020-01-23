using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using KsWare.RepositoryDiff.Commands;
using KsWare.RepositoryDiff.Common;
using KsWare.RepositoryDiff.UI.Filter;
using KsWare.RepositoryDiff.UI.MainWindow;
using KsWare.RepositoryDiff.UI.Results.Commands;

namespace KsWare.RepositoryDiff.UI.Results
{
    public class CompareResultViewModel : NotifyPropertyChangedBase
    {
        private MainWindowViewModel _mainWindowViewModel;
        private static int _lastId;

        private bool _isExpanded = true;
        private bool _isHidden;
        private string _directory;
        private string _name;
        private string _nameA, _nameB, _nameC;
        private string _resultA, _resultB, _resultC;
        private string _indentedNameA, _indentedNameB, _indentedNameC;
        private int? _level;
        private string _indent;
        private string _fileExtension;
        private bool _isHiddenBecauseCollapsed;
        private bool _isHiddenBecauseFilter;
        private bool _isHiddenByUser;
        private bool _isHiddenBecauseAllChildsAreHidden;
        private bool _isHiddenBecauseParentIsHidden;


        public CompareResultViewModel(string relativPath, FileSystemInfo a, FileSystemInfo b, FileSystemInfo c,
            string result, MainWindowViewModel mainWindowViewModel)
            : this(new CompareResultData
            {
                Id = ++_lastId,
                RelativPath = relativPath,
                A = new FileSystemInfoLite(a),
                B = new FileSystemInfoLite(b),
                C = c != null ? new FileSystemInfoLite(c) : null,
                IsDirectory = (a ?? b ?? c) is DirectoryInfo,
                Result = result,
            }, mainWindowViewModel)
        {
        }

        public CompareResultViewModel(CompareResultData data, MainWindowViewModel mainWindowViewModel)
        {
            Data = data;
            _mainWindowViewModel = mainWindowViewModel;

            CopyFullPathCommand = new CopyFullPathCommand();
            DiffCommand = new DiffCommand(this, _mainWindowViewModel);
            OpenInExplorerCommand = new OpenInExplorerCommand();
            LeftDoubleClickCommand = new LeftDoubleClickCommand(this);
            CopyABCommand = new CopyABCommand(this);
            DeleteBCommand = new DeleteBCommand(this);
        }

        public CompareResultData Data { get; }

        public int Id => Data.Id;

        public string RelativPath => Data.RelativPath;

        public FileSystemInfoLite A => Data.A;
        public FileSystemInfoLite B => Data.B;
        public FileSystemInfoLite C => Data.C;

        public bool IsDirectory => Data.IsDirectory;

        public string Result
        {
            get => Data.Result;
            set
            {
                Set(() => Data.Result, v => Data.Result = v, value);
                _resultA = null;
                _resultB = null;
                _resultC = null;
                OnPropertyChanged(nameof(ResultA));
                OnPropertyChanged(nameof(ResultB));
                OnPropertyChanged(nameof(ResultC));
            }
        }


        public string ResultA => _resultA ??= CalcResultA();
        public string ResultB => _resultB ??= CalcResultB();
        public string ResultC => _resultC ??= CalcResultC();


        public int Level => _level ??= RelativPath.Split('\\').Length - 1;

        public string IndentedNameA => _indentedNameA ??= NameA != null ? Indent + NameA : (string) null;

        public string IndentedNameB => _indentedNameB ??= NameB != null ? Indent + NameB : (string) null;

        public string IndentedNameC => _indentedNameC ??= NameC != null ? Indent + NameC : (string) null;


        public string Indent => _indent ??= new string(' ', Level * 4);


        public string Name => _name ??= (Data.A ?? Data.B ?? Data.C).Name;

        public string NameA => _nameA ??= Data.A.Exists ? Name : null;

        public string NameB => _nameB ??= Data.B.Exists ? Name : null;

        public string NameC => _nameC ??= Data.C?.Exists ?? false ? Name : null;

        public void UpdateNameB()
        {
            _nameB = null;
            OnPropertyChanged(nameof(NameB));
        }

        public CopyFullPathCommand CopyFullPathCommand { get; set; }

        public DiffCommand DiffCommand { get; set; }

        public OpenInExplorerCommand OpenInExplorerCommand { get; set; }

        public LeftDoubleClickCommand LeftDoubleClickCommand { get; set; }

        public IList<CompareResultViewModel> Children { get; set; } = new List<CompareResultViewModel>();


        public bool IsHidden
        {
            get => _isHidden;
            private set => Set(ref _isHidden, value);
        }

        private bool IsHiddenByUser
        {
            get => _isHiddenByUser;
            set
            {
                _isHiddenByUser = value;
                UpdateIsHidden();
            }
        }

        private bool IsHiddenBecauseCollapsed
        {
            get => _isHiddenBecauseCollapsed;
            set
            {
                _isHiddenBecauseCollapsed = value;
                UpdateIsHidden();
            }
        }


        private bool IsHiddenBecauseFilter
        {
            get => _isHiddenBecauseFilter;
            set
            {
                _isHiddenBecauseFilter = value;
                UpdateIsHidden();
            }
        }


        private bool IsHiddenBecauseAllChildsAreHidden
        {
            get => _isHiddenBecauseAllChildsAreHidden;
            set
            {
                _isHiddenBecauseAllChildsAreHidden = value;
                UpdateIsHidden();
            }
        }

        private bool IsHiddenBecauseParentIsHidden
        {
            get => _isHiddenBecauseParentIsHidden;
            set
            {
                _isHiddenBecauseParentIsHidden = value;
                UpdateIsHidden();
            }
        }

        private void UpdateIsHidden()
        {
            IsHidden = _isHiddenBecauseFilter || _isHiddenBecauseCollapsed || _isHiddenByUser ||
                       _isHiddenBecauseAllChildsAreHidden || _isHiddenBecauseParentIsHidden;
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (!Set(ref _isExpanded, value)) return;
                if (value) ShowRecursive(Children);
                else HideRecursive(Children);
            }
        }

        public string Directory => _directory ??= Helpers.GetDirectory(RelativPath);

        public CopyABCommand CopyABCommand { get; set; }

        public DeleteBCommand DeleteBCommand { get; set; }

        [JsonIgnore] public string FileExtension => _fileExtension ??= Path.GetExtension(Name);

        private static void HideRecursive(IEnumerable<CompareResultViewModel> children)
        {
            foreach (var child in children)
            {
                child.IsHiddenBecauseCollapsed = true;
                HideRecursive(child.Children);
            }
        }

        private static void ShowRecursive(IEnumerable<CompareResultViewModel> children)
        {
            foreach (var child in children)
            {
                child.IsHiddenBecauseCollapsed = false;
                if (child.IsExpanded) ShowRecursive(child.Children);
                else HideRecursive(child.Children);
            }
        }

        public void UpdateFilter(FilterViewModel filter, bool isHiddenBecauseParentIsHidden)
        {
            IsHiddenBecauseParentIsHidden = isHiddenBecauseParentIsHidden;
            IsHiddenBecauseFilter = !filter.Match(this);
            if (IsDirectory)
            {
                Children.ForEach(c => c.UpdateFilter(filter, IsHiddenBecauseFilter));
                IsHiddenBecauseAllChildsAreHidden =
                    Children.Count == 0 || (Children.Count > 0 && Children.All(c => c.IsHidden));
            }
        }

        private string CalcResultA()
        {
            if (Result.Length == 2)
            {
                return "?"; //TODO;
            }
            else if (Result.Length == 3)
            {
                switch (Result[0])
                {
                    case '=': return "Unchanged";
                    case '#': return "Changed";
                    case 'x': return "Deleted";
                    case '*': return "Created";
                    case '-': return ""; // does not exist, used together with new (*,-)
                    default: return Result[1].ToString();
                }
            }
            else
            {
                return "";
            }
        }


        private string CalcResultB()
        {
            if (Result.Length == 2)
            {
                return "?"; //TODO;
            }
            else if (Result.Length == 3)
            {
                switch (Result[2])
                {
                    case '=': return "Unchanged";
                    case '#': return "Changed";
                    case 'x': return "Deleted";
                    case '*': return "Created";
                    case '-': return ""; // does not exist, used together with new (*,-)
                    default: return Result[1].ToString();
                }
            }
            else
            {
                return "";
            }
        }


        private string CalcResultC()
        {
            if (Result.Length == 2)
            {
                return "";
            }
            else if (Result.Length == 3)
            {
                switch (Result[1])
                {
                    case '=': return "Equal";
                    case '!': return "Conflict";
                    case ',':
                    case '|': return "";
                    default: return Result[1].ToString();
                }
            }
            else
            {
                return "";
            }
        }
    }
}