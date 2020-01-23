using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
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


            UpdateFiles();
        }

        private void UpdateFiles()
        {
            var filesViewModel = _mainWindowViewModel.Files;
            var list1 = _mainWindowViewModel.CollectionView.OfType<CompareResultViewModel>().Where(x => !x.IsDirectory);
            IEnumerable<string> list2;

            switch (filesViewModel.PathType)
            {
                case "RelativePath":
                {
                    list2 = filesViewModel.FromResult switch
                    {
                        "A" => list1.Where(x => x.A.Exists).Select(x => x.RelativPath),
                        "B" => list1.Where(x => x.B.Exists).Select(x => x.RelativPath),
                        "C" => list1.Where(x => x.C.Exists).Select(x => x.RelativPath),
                        _ => list1.Select(x => x.RelativPath)
                    };
                    break;
                }
                default:
                {
                    list2 = filesViewModel.FromResult switch
                    {
                        "A" => list1.Where(x => x.A.Exists).Select(x => x.A.FullName),
                        "B" => list1.Where(x => x.B.Exists).Select(x => x.B.FullName),
                        "C" => list1.Where(x => x.C?.Exists??false).Select(x => x.C.FullName),
                        _ => list1.Select(x => x.B.FullName)
                    };
                    break;
                }
            }

            _mainWindowViewModel.Files.FileNamesAsText = string.Join("\r\n",list2);
        }

        public event EventHandler CanExecuteChanged;
    }
}