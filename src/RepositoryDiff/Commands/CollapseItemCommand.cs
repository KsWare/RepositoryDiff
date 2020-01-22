using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class CollapseItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResult) parameter).IsExpanded = false;
        }

        public event EventHandler CanExecuteChanged;
    }
}