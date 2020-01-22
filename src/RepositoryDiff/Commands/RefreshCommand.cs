using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace KsWare.RepositoryDiff
{
    public class RefreshCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public RefreshCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            foreach (var c in _mainWindowViewModel.Results.Where(x=>x.Level==0))
            {
                c.UpdateFilter(_mainWindowViewModel.Filter, false);
            }
            var collectionView = CollectionViewSource.GetDefaultView(_mainWindowViewModel.Results);
            collectionView.Refresh();
        }

        public event EventHandler CanExecuteChanged;
    }
}