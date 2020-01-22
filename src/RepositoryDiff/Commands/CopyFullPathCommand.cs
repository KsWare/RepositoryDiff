﻿using System;
using System.Windows;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.Commands
{
    public class CopyFullPathCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            Clipboard.SetText((string)parameter);

        }

        public event EventHandler CanExecuteChanged;
    }
}