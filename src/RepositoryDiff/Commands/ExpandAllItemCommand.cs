using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class ExpandAllItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = true;
            //TODO recursive
        }

        public event EventHandler CanExecuteChanged;
    }
}