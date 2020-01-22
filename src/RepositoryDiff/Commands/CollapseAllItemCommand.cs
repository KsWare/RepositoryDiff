using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class CollapseAllItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = false;
            //TODO recursive
        }

        public event EventHandler CanExecuteChanged;
    }
}