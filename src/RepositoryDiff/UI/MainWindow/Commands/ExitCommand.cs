using System;
using System.Windows;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class ExitCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ExitCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            Application.Current.Shutdown(0);
        }

        public event EventHandler CanExecuteChanged;
    }
}