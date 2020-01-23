using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using KsWare.RepositoryDiff.Data;
using KsWare.RepositoryDiff.UI.Options;
using Microsoft.Win32;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class LoadProjectCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public LoadProjectCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var dlg=new OpenFileDialog
            {
                Title = "Load project...",
                Filter = "Project files (*.proj)|*.proj",
                FilterIndex = 1,
                DefaultExt = ".proj"
            };

            if(dlg.ShowDialog()!=true) return;
            using var r=File.OpenText(dlg.FileName);
            var project = JsonSerializer.Deserialize<Project>(r.ReadToEnd());
            project.FileName = dlg.FileName;
            _mainWindowViewModel.Project = project;
            _mainWindowViewModel.A = project.A;
            _mainWindowViewModel.B = project.B;
            _mainWindowViewModel.C = project.C;
        }

        public event EventHandler CanExecuteChanged;
    }
}