using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class SaveFileFilterCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public SaveFileFilterCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var data = _mainWindowViewModel.FileFilter;
            if (data.FileName == null)
            {
                _mainWindowViewModel.SaveFileFilterAsCommand.Execute(null);
                return;
            }

            var json=JsonSerializer.Serialize(data, new JsonSerializerOptions{WriteIndented = true});
            using var w=new StreamWriter(File.Create(data.FileName),Encoding.UTF8);
            w.Write(json);
        }

        public event EventHandler CanExecuteChanged;
    }
}