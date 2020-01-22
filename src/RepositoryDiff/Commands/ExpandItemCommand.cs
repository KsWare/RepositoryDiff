using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class ExpandItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = true;
        }

        public event EventHandler CanExecuteChanged;
    }
}