using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class LeftDoubleClickCommand : ICommand
    {
        private readonly CompareResult _compareResult;

        public LeftDoubleClickCommand(CompareResult compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (_compareResult.IsDirectory) _compareResult.IsExpanded = !_compareResult.IsExpanded;
            else _compareResult.DiffCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}