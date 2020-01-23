using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class ExportCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ExportCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var jsonProperties = new JsonSerializerOptions()
            {
                IgnoreReadOnlyProperties = true, 
                WriteIndented = true,
                Converters = { }
            };
            var json = JsonSerializer.Serialize(_mainWindowViewModel.Results.Select(x=>x.Data),jsonProperties);
            using var w = new StreamWriter(File.Open("export.json", FileMode.Create,FileAccess.Write));
            w.Write(json);
        }

        public event EventHandler CanExecuteChanged;
    }
}