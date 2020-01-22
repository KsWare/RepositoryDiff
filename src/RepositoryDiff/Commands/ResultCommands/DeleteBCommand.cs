using System;
using System.Windows;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class DeleteBCommand : ICommand
    {
        private readonly CompareResult _compareResult;

        public DeleteBCommand(CompareResult compareResult)
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