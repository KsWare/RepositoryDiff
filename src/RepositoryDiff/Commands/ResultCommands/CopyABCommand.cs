using System;
using System.IO;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class CopyABCommand : ICommand
    {
        private readonly CompareResult _compareResult;

        public CopyABCommand(CompareResult compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            File.Copy(_compareResult.A.FullName,_compareResult.B.FullName,true);
            _compareResult.Result = "=,=";
            _compareResult.UpdateNameB();
        }

        public event EventHandler CanExecuteChanged;
    }
}