using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Input;
using KsWare.RepositoryDiff.Commands;

namespace KsWare.RepositoryDiff
{
    public class CompareResult : NotifyPropertyChangedBase
    {
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


        public CompareResult(string relativPath, FileSystemInfo a, FileSystemInfo b, FileSystemInfo c, string result)
            :this()
        {
            Id = ++_lastId;
            A = a;
            B = b;
            C = c;
            Result = result;
            RelativPath = relativPath;
        }

        public CompareResult()
        {
            CopyFullPathCommand = new CopyFullPathCommand(this);
            DiffCommand = new DiffCommand(this);
            OpenInExplorerCommand = new OpenInExplorerCommand(this);
            LeftDoubleClickCommand = new LeftDoubleClickCommand(this);
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

        public string Result { get => _result; set => Set(ref _result, value); }


        public int Level => _level ??= RelativPath.Split('\\').Length - 1;
        public string IndentedNameA => _indentedNameA ??= NameA!=null ? Indent + NameA : (string)null;
        public string IndentedNameB => _indentedNameB ??= NameB!=null ? Indent + NameB : (string)null;
        public string IndentedNameC => _indentedNameC ??= NameC!=null ? Indent + NameC : (string)null;

        public string Indent => _indent ??= new string(' ', Level * 4);

        public string Name => _name ??= (A ?? B ?? C).Name;
        public string NameA => _nameA ??= A.Exists ? Name : null;
        public string NameB => _nameB ??= B.Exists ? Name : null;
        public string NameC => _nameC ??= C?.Exists ?? false ? Name : null;



        public CopyFullPathCommand CopyFullPathCommand { get; } 
        public DiffCommand DiffCommand { get; } 
        public OpenInExplorerCommand OpenInExplorerCommand { get; }

        public LeftDoubleClickCommand LeftDoubleClickCommand { get; }

        [JsonIgnore]
        public IList<CompareResult> Children { get; set; } = new List<CompareResult>();

        [JsonIgnore]
        public bool IsHidden { get => _isHidden; internal set => Set(ref _isHidden, value); }

        [JsonIgnore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if(!Set(ref _isExpanded, value)) return;
                if(value) Helpers.ShowRecursive(Children); else Helpers.HideRecursive(Children);
            }
        }

        public string Directory => _directory ??= Helpers.GetDirectory(RelativPath);

        
    }

    public class LeftDoubleClickCommand : ICommand
    {
        private readonly CompareResult _compareResult;

        public LeftDoubleClickCommand(CompareResult compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (_compareResult.IsDirectory) _compareResult.IsExpanded = !_compareResult.IsExpanded;
            else _compareResult.DiffCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}