using System;
using System.Windows;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.Commands
{
    public class CopyTextToClipboardCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object text)
        {
            Clipboard.SetText((string)text);

        }

        public event EventHandler CanExecuteChanged;
    }
}