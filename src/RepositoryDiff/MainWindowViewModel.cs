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

        public MainWindowViewModel()
        {
            A = Options.A;
            B = Options.B;
            C = Options.C;

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
            CollectionView.Filter=Filter.FilterFunction;

            DiffCommand = new StartDiffCommand(this);
            ExportCommand = new ExportCommand(this);
            CollapseItemCommand = new CollapseItemCommand();
            ExpandItemCommand = new ExpandItemCommand();
            CollapseAllItemCommand = new CollapseAllItemCommand();
            ExpandAllItemCommand = new ExpandAllItemCommand();

            Filter.RefreshCommand = new RefreshCommand(this);

            // if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            // {
                Results.Add(new CompareResult(@"C",new FileInfo(@"C:\"),new FileInfo(@"D:\"),null,"!!", this));
                Results[0].IsDirectory = true;
                Results.Add(new CompareResult(@"C\a",new FileInfo(@"C:\a"),new FileInfo(@"D:\a"),null,"!!", this));
                Results.Add(new CompareResult(@"C\a\b",new FileInfo(@"C:\a\b"),new FileInfo(@"D:\a\b"),null,"!!", this));
                Helpers.CreateHierarchy(Results);
            // }

            Application.Current.Exit+= (s,e) => Options.Save();
            Application.Current.SessionEnding+=(s, e) => Options.Save();
        }

        public FilterViewModel Filter { get; } =new FilterViewModel();

        public IList<string> Excludes { get; }=new List<string>();

        public string A { get => Options.A; set => Set(() => Options.A, v => Options.A = v, value); }

        public string B { get => Options.B; set => Set(() => Options.B, v => Options.B = v, value); }
        public string C { get => Options.C; set => Set(() => Options.C, v => Options.C = v, value); }

        public ICommand DiffCommand { get; }

        public ICommand ExportCommand { get; }

        public IList<CompareResult> Results { get; set; } = new ObservableCollection<CompareResult>();
        public string FileNamesAsText { get => _fileNamesAsText; set => Set(ref _fileNamesAsText, value); }

        public ICollectionView CollectionView { get; set; }

        public string RootA { get; set; }
        public string RootB { get; set; }
        public string RootC { get; set; }

        public ICommand CollapseItemCommand { get; }
        public ICommand ExpandItemCommand { get; }
        public ICommand CollapseAllItemCommand { get; }
        public ICommand ExpandAllItemCommand { get; }

        public OptionsViewModel Options { get; } = OptionsViewModel.LoadOrCreate();
    }
}
