using System;
using System.Diagnostics;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.Commands
{
    public class OpenInExplorerCommand : ICommand
    {
        public OpenInExplorerCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            // return parameter != null; parameter is always null!?
            return true;
        }

        public void Execute(object parameter)
        {
            var file = @"Explorer.exe";
            var arguments = (string)parameter;
            arguments = "/select, \"" + arguments +"\"";
            Process.Start(file, arguments);
        }

        public event EventHandler CanExecuteChanged;
    }
}