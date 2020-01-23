using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class SaveProjectCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public SaveProjectCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var data = _mainWindowViewModel.Project;
            if (data.FileName == null)
            {
                _mainWindowViewModel.SaveProjectAsCommand.Execute(null);
                return;
            }

            _mainWindowViewModel.Project.A = _mainWindowViewModel.A;
            _mainWindowViewModel.Project.B = _mainWindowViewModel.B;
            _mainWindowViewModel.Project.C = _mainWindowViewModel.C;

            var json=JsonSerializer.Serialize(data,new JsonSerializerOptions{WriteIndented = true});
            using var w=new StreamWriter(File.Create(data.FileName),Encoding.UTF8);
            w.Write(json);
        }

        public event EventHandler CanExecuteChanged;
    }
}