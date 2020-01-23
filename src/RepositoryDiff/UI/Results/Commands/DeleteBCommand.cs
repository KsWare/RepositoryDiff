using System;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.Results.Commands
{
    public class DeleteBCommand : ICommand
    {
        private readonly CompareResultViewModel _compareResult;

        public DeleteBCommand(CompareResultViewModel compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter) => false;

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}