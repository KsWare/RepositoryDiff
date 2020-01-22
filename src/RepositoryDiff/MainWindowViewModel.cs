using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using KsWare.RepositoryDiff.Commands;

namespace KsWare.RepositoryDiff
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private string _fileNamesAsText;
        private string _a;
        private string _b;
        private string _c;

        public MainWindowViewModel()
        {
            

            Excludes.Add(@"(^|\\)\.");
            Excludes.Add(@"^BuildOutput$");
            Excludes.Add(@"^packages$");
            Excludes.Add(@"^_ReSharper.Caches$");
            Excludes.Add(@"^packages$");
            Excludes.Add(@"\\(bin|obj|build)$");
            Excludes.Add(@"\.user$");
            Excludes.Add(@"\.suo$");
            Excludes.Add(@"\.sln\.docstates$");
            Excludes.Add(@"\.pdb$");
            Excludes.Add(@"\.vspscc$");
            Excludes.Add(@"\.vssscc$");
            Excludes.Add(@"\.log");
            Excludes.Add(@"(^|\\)_UpgradeReport_Files$");
            Excludes.Add(@"(^|\\)Backup");
            Excludes.Add(@"(^|\\)UpgradeLog.*\.xml$");
            Excludes.Add(@"(^|\\)UpgradeLog.*\.htm$");
            Excludes.Add(@"(^|\\)Desktop\.ini$");
            Excludes.Add(@"(^|\\)CodeGraphData$");
            Excludes.Add(@"\.rtflow\.xml$");
            Excludes.Add(@"(^|\\)Thumbs.db$");
            Excludes.Add(@"(^|\\)ehthumbs.db$");
            Excludes.Add(@"(^|\\)\$RECYCLE\.BIN$");

            CollectionView = CollectionViewSource.GetDefaultView(Results);
            CollectionView.Filter=Filter;

            DiffCommand = new StartDiffCommand(this);
            ExportCommand = new ExportCommand(this);
            CollapseItemCommand = new CollapseItemCommand();
            ExpandItemCommand = new ExpandItemCommand();
            CollapseAllItemCommand = new CollapseAllItemCommand();
            ExpandAllItemCommand = new ExpandAllItemCommand();

            // if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            // {
                Results.Add(new CompareResult(@"C",new FileInfo(@"C:\"),new FileInfo(@"D:\"),null,"!!" ));
                Results[0].IsDirectory = true;
                Results.Add(new CompareResult(@"C\a",new FileInfo(@"C:\a"),new FileInfo(@"D:\a"),null,"!!" ));
                Results.Add(new CompareResult(@"C\a\b",new FileInfo(@"C:\a\b"),new FileInfo(@"D:\a\b"),null,"!!" ));
                Helpers.CreateHierarchy(Results);
            // }
        }

        public IList<string> Excludes { get; }=new List<string>();

        public string A { get => _a; set => Set(ref _a, value); }
        public string B { get => _b; set => Set(ref _b, value); }
        public string C { get => _c; set => Set(ref _c, value); }

        public ICommand DiffCommand { get; }

        public ICommand ExportCommand { get; }

        public IList<CompareResult> Results { get; set; } = new ObservableCollection<CompareResult>();
        public string FileNamesAsText { get => _fileNamesAsText; set => Set(ref _fileNamesAsText, value); }

        private bool Filter(object obj)
        {
            var c = (CompareResult) obj;
            if (c.Result == "==" || c.Result == "===") return false;
            return true;
        }

        public ICollectionView CollectionView { get; set; }

        public string RootA { get; set; }
        public string RootB { get; set; }
        public string RootC { get; set; }

        public ICommand CollapseItemCommand { get; }
        public ICommand ExpandItemCommand { get; }
        public ICommand CollapseAllItemCommand { get; }
        public ICommand ExpandAllItemCommand { get; }
    }

    public class CollapseItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = false;
        }

        public event EventHandler CanExecuteChanged;
    }
    public class ExpandItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class CollapseAllItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = false;
            //TODO recursive
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ExpandAllItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = true;
            //TODO recursive
        }

        public event EventHandler CanExecuteChanged;
    }
}
