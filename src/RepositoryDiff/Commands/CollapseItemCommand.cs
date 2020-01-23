using System;
using System.Windows.Input;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.Commands
{
    public class CollapseItemCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter==null) return;
            ((CompareResultViewModel) parameter).IsExpanded = false;
        }

        public event EventHandler CanExecuteChanged;
    }
}