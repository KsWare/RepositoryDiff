using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using KsWare.RepositoryDiff.Common;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class ImportCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ImportCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _mainWindowViewModel.Results.Clear();
            using var r = File.OpenText("export.json");
            var results = JsonSerializer.Deserialize<CompareResultData[]>(r.ReadToEnd());
            var resultVMs = results.Select(data => new CompareResultViewModel(data, _mainWindowViewModel)).ToArray();
            Helpers.CreateHierarchy(resultVMs);
            _mainWindowViewModel.Results.Clear();
            resultVMs.ForEach(_mainWindowViewModel.Results.Add);
            _mainWindowViewModel.Filter.RefreshCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}