using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using KsWare.RepositoryDiff.Common;
using KsWare.RepositoryDiff.UI.MainWindow;

namespace KsWare.RepositoryDiff.UI.Results.Commands
{
    public class DiffCommand : ICommand
    {
        private readonly CompareResultViewModel _compareResult;
        private MainWindowViewModel _mainWindowViewModel;

        public DiffCommand(CompareResultViewModel compareResult, MainWindowViewModel mainWindowViewModel)
        {
            _compareResult = compareResult ?? throw new ArgumentNullException(nameof(compareResult));
            _mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set { _mainWindowViewModel = value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_compareResult.Data.C == null) // 2-way
            {
                return _compareResult.Data.A != null && _compareResult.Data.B != null && _compareResult.Data.A.Exists && _compareResult.Data.B.Exists;
            }
            else // 3-way
            {
                return true; //TODO
            }
            
        }

        public async void Execute(object argument)
        {
            var merged = (string) null;
            var file = (string) null;
            var arguments = "";
            var arguments2 = "";

            switch (argument as string)
            {
                case "SemanticMergeAB" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.Data.A.FullName}\" -d \"{_compareResult.Data.B.FullName}\"";
                    break;
                case "SemanticMergeAC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.Data.A.FullName}\" -d \"{_compareResult.Data.C.FullName}\"";
                    break;
                case "SemanticMergeBC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.Data.B.FullName}\" -d \"{_compareResult.Data.C.FullName}\"";
                    break;
                case "SemanticMerge3Way" : 
                    merged = GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name));
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.Data.A.FullName}\" -d \"{_compareResult.Data.B.FullName}\" -b \"{_compareResult.Data.C.FullName}\" -r \"{merged}\"";
                    arguments2 = $"-s \"{_compareResult.Data.B.FullName}\" -d \"{merged}\"";
                    break;

                case "MergeToolSelectorAB" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool diff -s \"{_compareResult.Data.A.FullName}\" -d \"{_compareResult.Data.B.FullName}\"";
                    break;
                case "MergeToolSelectorAC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool diff -s \"{_compareResult.Data.A.FullName}\" -d \"{_compareResult.Data.C.FullName}\"";
                    break;
                case "MergeToolSelectorBC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool diff -s \"{_compareResult.Data.B.FullName}\" -d \"{_compareResult.Data.C.FullName}\"";
                    break;
                case "MergeToolSelector3Way" : 
                    merged = GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name));
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool merge -s \"{_compareResult.Data.A.FullName}\" -d \"{_compareResult.Data.B.FullName}\" -b \"{_compareResult.Data.C.FullName}\" -r \"{merged}\"";
                    arguments2 = $"-tool diff -s \"{_compareResult.Data.B.FullName}\" -d \"{merged}\"";
                    break;
                default:
                    if (_compareResult.NameC == null) goto case "SemanticMergeAB";
                    else goto case "SemanticMerge3Way";
            }

            var p = Process.Start(file, arguments);
            await p.WaitForExitAsync();
            if (p.ExitCode == 0 && merged != null)
            {
                if (_mainWindowViewModel.Options.ShowMergedDiff)
                {
                    p = Process.Start(file, arguments2);
                    await p.WaitForExitAsync();
                }
                if (_mainWindowViewModel.Options.AcknowledgeMerge && MessageBox.Show("Accept merge and update your file?", "Acknowledge", MessageBoxButton.OKCancel) !=
                    MessageBoxResult.OK)
                {
                    File.Delete(merged);
                    merged = null;
                }
                if(merged!=null) File.Copy(merged,_compareResult.Data.B.FullName,true);
            }
            if(merged!=null) File.Delete(merged);
        }

        private string GetTempFileName(string extension)
        {
            var path=Path.Combine(Path.GetTempPath(), "merged_"+Guid.NewGuid().ToString("N") + extension);
            return path;
        }

        public event EventHandler CanExecuteChanged;
    }
}