using System;
using System.Windows;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.Commands
{
    public class CopyFullPathCommand : ICommand
    {
        private readonly CompareResult _compareResult;

        public CopyFullPathCommand(CompareResult compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Clipboard.SetText((string)parameter);

        }

        public event EventHandler CanExecuteChanged;
    }
}