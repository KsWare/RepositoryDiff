using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.Results.Commands
{
    public class LeftDoubleClickCommand : ICommand
    {
        private readonly CompareResultViewModel _compareResult;

        public LeftDoubleClickCommand(CompareResultViewModel compareResult)
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