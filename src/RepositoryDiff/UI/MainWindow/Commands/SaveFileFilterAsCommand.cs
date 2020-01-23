using System;
using System.Windows.Input;
using Microsoft.Win32;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class SaveFileFilterAsCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public SaveFileFilterAsCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var dlg = new SaveFileDialog()
            {
                Title = "Save project...",
                Filter = "Filter files (*.flt)|*.flt",
                FilterIndex = 1,
                OverwritePrompt = true,
            };

            if(dlg.ShowDialog()!=true) return;
            _mainWindowViewModel.FileFilter.FileName = dlg.FileName;
            _mainWindowViewModel.SaveFileFilterCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}