using System;
using System.Windows.Input;
using KsWare.RepositoryDiff.UI.MainWindow;

namespace KsWare.RepositoryDiff.UI.Results.Commands
{
    public class HideCommand : ICommand
    {
        private readonly CompareResultViewModel _compareResult;
        private MainWindowViewModel _mainWindowViewModel;

        public HideCommand(CompareResultViewModel compareResult, MainWindowViewModel mainWindowViewModel)
        {
            _compareResult = compareResult ?? throw new ArgumentNullException(nameof(compareResult));
            _mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _compareResult.IsHiddenByUser = !_compareResult.IsHiddenByUser;
            _mainWindowViewModel.Filter.RefreshCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}