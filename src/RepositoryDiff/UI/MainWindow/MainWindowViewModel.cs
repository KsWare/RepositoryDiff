using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using KsWare.RepositoryDiff.Commands;
using KsWare.RepositoryDiff.Common;
using KsWare.RepositoryDiff.Data;
using KsWare.RepositoryDiff.UI.Files;
using KsWare.RepositoryDiff.UI.Filter;
using KsWare.RepositoryDiff.UI.MainWindow.Commands;
using KsWare.RepositoryDiff.UI.Options;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.UI.MainWindow
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private string _fileNamesAsText;

        public MainWindowViewModel()
        {
            A = Options.A;
            B = Options.B;
            C = Options.C;

            CollectionView = CollectionViewSource.GetDefaultView(Results);
            CollectionView.Filter=Filter.FilterFunction;

            DiffCommand = new StartDiffCommand(this);
            ExportCommand = new ExportCommand(this);
            ImportCommand = new ImportCommand(this);
            CollapseItemCommand = new CollapseItemCommand();
            ExpandItemCommand = new ExpandItemCommand();
            CollapseAllItemCommand = new CollapseAllItemCommand();
            ExpandAllItemCommand = new ExpandAllItemCommand();
            SaveProjectCommand = new SaveProjectCommand(this);
            SaveProjectAsCommand = new SaveProjectAsCommand(this);
            LoadProjectCommand = new LoadProjectCommand(this);
            ExitCommand = new ExitCommand(this);
            SaveFileFilterCommand = new SaveFileFilterCommand(this);
            SaveFileFilterAsCommand = new SaveFileFilterAsCommand(this);
            LoadFileFilterCommand = new LoadFileFilterCommand(this);
            Files=new FilesViewModel(this);
            FileFilter = FileFilter.Default;
            UpdateFilePatterns();

            Filter.RefreshCommand = new RefreshCommand(this);

            Application.Current.Exit+= (s,e) => Options.Save();
            Application.Current.SessionEnding+=(s, e) => Options.Save();
        }

        public FilterViewModel Filter { get; } =new FilterViewModel();

        public List<Regex> Excludes { get; set; } = new List<Regex>();

        public string A { get => Options.A; set => Set(() => Options.A, v => Options.A = v, value); }

        public string B { get => Options.B; set => Set(() => Options.B, v => Options.B = v, value); }
        public string C { get => Options.C; set => Set(() => Options.C, v => Options.C = v, value); }

        public StartDiffCommand DiffCommand { get; }

        public ExportCommand ExportCommand { get; }

        public ImportCommand ImportCommand { get; }


        public SaveProjectCommand SaveProjectCommand { get; }
        public SaveProjectAsCommand SaveProjectAsCommand { get; }
        public LoadProjectCommand LoadProjectCommand { get; }
        public ExitCommand ExitCommand { get;}
        public SaveFileFilterCommand SaveFileFilterCommand { get;}
        public SaveFileFilterAsCommand SaveFileFilterAsCommand { get;}
        public LoadFileFilterCommand LoadFileFilterCommand { get;}


        public IList<CompareResultViewModel> Results { get; } = new ObservableCollection<CompareResultViewModel>();
        public ICollectionView CollectionView { get; }

        public FilesViewModel Files { get;}

        public string RootA { get; set; }
        public string RootB { get; set; }
        public string RootC { get; set; }

        public ICommand CollapseItemCommand { get; }
        public ICommand ExpandItemCommand { get; }
        public ICommand CollapseAllItemCommand { get; }
        public ICommand ExpandAllItemCommand { get; }

        public OptionsViewModel Options { get; } = OptionsViewModel.LoadOrCreate();

        public Project Project { get; set; } = new Project();

        public FileFilter FileFilter { get; set; }

        public void UpdateFilePatterns()
        {
            var lines = FileFilter.Exclude.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).Where(x => !x.StartsWith("::"));
            Excludes = lines.Select(x => new Regex(x, RegexOptions.IgnoreCase | RegexOptions.Compiled)).ToList();

        }
    }
}
