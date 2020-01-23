using System;
using System.Windows.Input;
using Microsoft.Win32;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class SaveProjectAsCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public SaveProjectAsCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var dlg = new SaveFileDialog()
            {
                Title = "Save project...",
                Filter = "Project files (*.proj)|*.proj",
                FilterIndex = 1,
                OverwritePrompt = true,
            };

            if(dlg.ShowDialog()!=true) return;
            _mainWindowViewModel.Project.FileName = dlg.FileName;
            _mainWindowViewModel.SaveProjectCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}