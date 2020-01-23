using System;
using System.IO;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.UI.Results.Commands
{
    public class CopyABCommand : ICommand
    {
        private readonly CompareResultViewModel _compareResult;

        public CopyABCommand(CompareResultViewModel compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            File.Copy(_compareResult.Data.A.FullName,_compareResult.Data.B.FullName,true);
            _compareResult.Result = "=,=";
            _compareResult.UpdateNameB();
        }

        public event EventHandler CanExecuteChanged;
    }
}