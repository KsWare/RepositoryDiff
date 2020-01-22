﻿using System;
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
            CopyFullPathCommand = new CopyFullPathCommand(this);
            DiffCommand = new DiffCommand(this, _mainWindowViewModel);
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

        public void InitImport(MainWindowViewModel mainWindowViewModel)
        {
            DiffCommand.MainWindowViewModel=_mainWindowViewModel;
        }
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