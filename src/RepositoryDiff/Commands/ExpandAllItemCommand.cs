using System;
using System.Windows.Input;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.Commands
{
    public class ExpandAllItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResultViewModel) parameter).IsExpanded = true;
            //TODO recursive
        }

        public event EventHandler CanExecuteChanged;
    }
}