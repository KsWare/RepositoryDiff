using KsWare.RepositoryDiff.Common;
using KsWare.RepositoryDiff.UI.MainWindow;
using KsWare.RepositoryDiff.UI.MainWindow.Commands;

namespace KsWare.RepositoryDiff.UI.Files
{
    public class FilesViewModel : NotifyPropertyChangedBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public FilesViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            PathType = "FullPath";
            FromResult = "B";
        }

        public RefreshCommand RefreshCommand => _mainWindowViewModel.Filter.RefreshCommand;

        public string PathType { get => Get<string>(); set => Set(value); }
        public string FromResult { get => Get<string>(); set => Set(value); }

        public string FileNamesAsText { get => Get<string>(); set => Set(value); }
    }
}
