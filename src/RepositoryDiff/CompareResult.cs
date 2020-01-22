using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using KsWare.RepositoryDiff.Commands;
using KsWare.RepositoryDiff.Common;

namespace KsWare.RepositoryDiff
{
    public class CompareResult : NotifyPropertyChangedBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private static int _lastId;
        private string _result;
        private bool? _isDirectory;

        private bool _isExpanded=true;
        private bool _isHidden;
        private string _directory;
        private string _name;
        private string _nameA,_nameB,_nameC;
        private string _indentedNameA,_indentedNameB,_indentedNameC;
        private int? _level;
        private string _indent;
        private string _resultA,_resultB,_resultC;
        private string _fileExtension;
        private bool _isHiddenBecauseCollapsed;
        private bool _isHiddenBecauseFilter;
        private bool _isHiddenByUser;
        private bool _isHiddenBecauseAllChildsAreHidden;


        public CompareResult(string relativPath, FileSystemInfo a, FileSystemInfo b, FileSystemInfo c, string result, MainWindowViewModel mainWindowViewModel)
            :this()
        {
            _mainWindowViewModel = mainWindowViewModel;
            Id = ++_lastId;
            A = a;
            B = b;
            C = c;
            Result = result;
            RelativPath = relativPath;
        }

        public CompareResult()
        {
            // after import _mainWindowViewModel is null!
            CopyFullPathCommand = new CopyFullPathCommand();
            DiffCommand = new DiffCommand(this, _mainWindowViewModel);
            OpenInExplorerCommand = new OpenInExplorerCommand();
            LeftDoubleClickCommand = new LeftDoubleClickCommand(this);
            CopyABCommand = new CopyABCommand(this);
            DeleteBCommand = new DeleteBCommand(this);
        }

        public int Id { get; set; }

        public string RelativPath { get; set; }

        [JsonIgnore]
        public FileSystemInfo A { get; private set; }
        [JsonIgnore]
        public FileSystemInfo B { get; private set; }
        [JsonIgnore]
        public FileSystemInfo C { get; private set; }

        public bool IsDirectory
        {
            get => _isDirectory ??= (A ?? B ?? C) is DirectoryInfo;
            set => _isDirectory = value;
        }

        public string FullNameA
        {
            get => A.FullName;
            set => A = IsDirectory ? (FileSystemInfo)new DirectoryInfo(value) : new FileInfo(value);
        }
        public string FullNameB
        {
            get => B.FullName;
            set => B = IsDirectory ? (FileSystemInfo)new DirectoryInfo(value) : new FileInfo(value);
        }
        public string FullNameC
        {
            get => C?.FullName;
            set => C = value != null ? (IsDirectory ? (FileSystemInfo)new DirectoryInfo(value) : new FileInfo(value)) : null;
        }

        public string Result
        {
            get => _result;
            set
            {
                Set(ref _result, value);
                _resultA = null;
                _resultB = null;
                _resultC = null;
                OnPropertyChanged(nameof(ResultA));
                OnPropertyChanged(nameof(ResultB));
                OnPropertyChanged(nameof(ResultC));
            }
        }

        public string ResultA => _resultA ??= CalcResultA();

        private string CalcResultA()
        {
            if (Result.Length == 2)
            {
                return "?"; //TODO;
            }
            else if (Result.Length==3)
            {
                switch (Result[0])
                {
                    case '=' : return "Unchanged";
                    case '#' : return "Changed";
                    case 'x' : return "Deleted";
                    case '*' : return "Created";
                    case '-' : return ""; // does not exist, used together with new (*,-)
                    default  : return Result[1].ToString();
                }
            }
            else
            {
                return "";
            }
            
        }

        public string ResultB => _resultB ??= CalcResultB();

        private string CalcResultB()
        {
            if (Result.Length == 2)
            {
                return "?"; //TODO;
            }
            else if (Result.Length==3)
            {
                switch (Result[2])
                {
                    case '=' : return "Unchanged";
                    case '#' : return "Changed";
                    case 'x' : return "Deleted";
                    case '*' : return "Created";
                    case '-' : return ""; // does not exist, used together with new (*,-)
                    default  : return Result[1].ToString();
                }
            }
            else
            {
                return "";
            }
        }

        public string ResultC => _resultC ??= CalcResultC();

        private string CalcResultC()
        {
            if (Result.Length == 2)
            {
                return "";
            }
            else if (Result.Length==3)
            {
                switch (Result[1])
                {
                    case '=' : return "Equal";
                    case '!' : return "Conflict";
                    case ',':case '|' : return "";
                    default  : return Result[1].ToString();
                }
            }
            else
            {
                return "";
            }
        }


        public int Level => _level ??= RelativPath.Split('\\').Length - 1;
        public string IndentedNameA => _indentedNameA ??= NameA!=null ? Indent + NameA : (string)null;
        public string IndentedNameB => _indentedNameB ??= NameB!=null ? Indent + NameB : (string)null;
        public string IndentedNameC => _indentedNameC ??= NameC!=null ? Indent + NameC : (string)null;

        public string Indent => _indent ??= new string(' ', Level * 4);

        public string Name => _name ??= (A ?? B ?? C).Name;
        public string NameA => _nameA ??= A.Exists ? Name : null;
        public string NameB => _nameB ??= B.Exists ? Name : null;
        public string NameC => _nameC ??= C?.Exists ?? false ? Name : null;

        public void UpdateNameB()
        {
            _nameB = null;
            OnPropertyChanged(nameof(NameB));
        }

        public CopyFullPathCommand CopyFullPathCommand { get; } 
        public DiffCommand DiffCommand { get; } 
        public OpenInExplorerCommand OpenInExplorerCommand { get; }

        public LeftDoubleClickCommand LeftDoubleClickCommand { get; }

        [JsonIgnore]
        public IList<CompareResult> Children { get; set; } = new List<CompareResult>();

        [JsonIgnore]
        public bool IsHidden { get => _isHidden; private set => Set(ref _isHidden, value); }

        [JsonIgnore]
        private bool IsHiddenByUser
        {
            get => _isHiddenByUser;
            set
            {
                _isHiddenByUser = value;
                UpdateIsHidden();
            }
        }

        [JsonIgnore]
        private bool IsHiddenBecauseCollapsed
        {
            get => _isHiddenBecauseCollapsed;
            set
            {
                _isHiddenBecauseCollapsed = value;
                UpdateIsHidden();
            }
        }

        [JsonIgnore]
        private bool IsHiddenBecauseFilter
        {
            get => _isHiddenBecauseFilter;
            set
            {
                _isHiddenBecauseFilter = value;
                UpdateIsHidden();
            }
        }

        [JsonIgnore]
        private bool IsHiddenBecauseAllChildsAreHidden
        {
            get => _isHiddenBecauseAllChildsAreHidden;
            set
            {
                _isHiddenBecauseAllChildsAreHidden = value;
                UpdateIsHidden();
            }
        }

        private void UpdateIsHidden()
        {
            IsHidden = _isHiddenBecauseFilter || _isHiddenBecauseCollapsed || _isHiddenByUser || _isHiddenBecauseAllChildsAreHidden;
        }

        [JsonIgnore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if(!Set(ref _isExpanded, value)) return;
                if(value) ShowRecursive(Children); else HideRecursive(Children);
            }
        }

        public string Directory => _directory ??= Helpers.GetDirectory(RelativPath);

        public CopyABCommand CopyABCommand { get; }

        public DeleteBCommand DeleteBCommand { get; }

        public string FileExtension => _fileExtension ??= Path.GetExtension(Name);

        public void InitImport(MainWindowViewModel mainWindowViewModel)
        {
            DiffCommand.MainWindowViewModel=_mainWindowViewModel;
        }

        private static void HideRecursive(IEnumerable<CompareResult> children)
        {
            foreach (var child in children)
            {
                child.IsHiddenBecauseCollapsed = true;
                HideRecursive(child.Children);
            }
        }

        private static void ShowRecursive(IEnumerable<CompareResult> children)
        {
            foreach (var child in children)
            {
                child.IsHiddenBecauseCollapsed = false;
                if(child.IsExpanded) ShowRecursive(child.Children); else HideRecursive(child.Children);
            }
        }

        public void UpdateFilter(FilterViewModel filter)
        {
            IsHiddenBecauseFilter = !filter.Match(this);
            Children.ForEach(c=>c.UpdateFilter(filter));
            IsHiddenBecauseAllChildsAreHidden = Children.Count > 0 && Children.All(c => c.IsHidden);
            
        }
    }
}