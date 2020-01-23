using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using KsWare.RepositoryDiff.Data;
using KsWare.RepositoryDiff.UI.Options;
using Microsoft.Win32;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class LoadFileFilterCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public LoadFileFilterCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var dlg=new OpenFileDialog
            {
                Title = "Load project...",
                Filter = "Filter files (*.flt)|*.flt",
                FilterIndex = 1,
                DefaultExt = ".flt"
            };

            if(dlg.ShowDialog()!=true) return;

            using var r=File.OpenText(dlg.FileName);
            var filter = JsonSerializer.Deserialize<FileFilter>(r.ReadToEnd());
            filter.FileName = dlg.FileName;
            _mainWindowViewModel.FileFilter = filter;
            _mainWindowViewModel.UpdateFilePatterns();
        }

        public event EventHandler CanExecuteChanged;
    }
}