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
            var fileA = _compareResult.Data.A.FullName;
            var fileB = _compareResult.Data.B.FullName;
            var fileC = _compareResult.Data.C?.FullName;

            bool a1 = _compareResult.Data.A.Exists, a0 = !a1;
            bool b1 = _compareResult.Data.B.Exists, b0 = !b1;
            bool c1 = _compareResult.Data.C?.Exists??false, c0 = !c1;

            switch (argument as string)
            {
                case "SemanticMergeAB" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{fileA}\" -d \"{fileB}\"";
                    break;
                case "SemanticMergeAC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{fileA}\" -d \"{fileC}\"";
                    break;
                case "SemanticMergeBC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{fileB}\" -d \"{fileC}\"";
                    break;
                case "SemanticMerge3Way" :
                    if (c0 && b0 && a0) ;
                    if (c0 && b0 && a1) goto case "SemanticMergeA";
                    if (c0 && b1 && a0) goto case "SemanticMergeB";
                    if (c0 && b1 && a1) goto case "SemanticMergeAB";
                    if (c1 && b0 && a0) goto case "SemanticMergeC";
                    if (c1 && b0 && a1) goto case "SemanticMergeAC";
                    if (c1 && b1 && a0) goto case "SemanticMergeBC";
                    if (c1 && b1 && a1) ;

                    merged = GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name));
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{fileA}\" -d \"{fileB}\" -b \"{fileC}\" -r \"{merged}\"";
                    arguments2 = $"-s \"{fileB}\" -d \"{merged}\"";
                    break;
                case "SemanticMergeA" : 
                    fileC=GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name),true);
                    goto case "SemanticMergeAC";
                case "SemanticMergeB" : 
                    fileC=GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name),true);
                    goto case "SemanticMergeBC";
                case "SemanticMergeC" : 
                    fileB=GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name),true);
                    goto case "SemanticMergeBC";

                case "MergeToolSelectorAB" : 
                    file = @"C:\Program Files (x86)\KsWare\MergeToolSelector\MergeToolSelector.exe";
                    arguments = $"-tool diff -s \"{fileA}\" -d \"{fileB}\"";
                    break;
                case "MergeToolSelectorAC" : 
                    file = @"C:\Program Files (x86)\KsWare\MergeToolSelector\MergeToolSelector.exe";
                    arguments = $"-tool diff -s \"{fileA}\" -d \"{fileC}\"";
                    break;
                case "MergeToolSelectorBC" : 
                    file = @"C:\Program Files (x86)\KsWare\MergeToolSelector\MergeToolSelector.exe";
                    arguments = $"-tool diff -s \"{fileB}\" -d \"{fileC}\"";
                    break;
                case "MergeToolSelector3Way" : 
                    merged = GetTempFileName(Path.GetExtension(_compareResult.Data.B.Name));
                    file = @"C:\Program Files (x86)\KsWare\MergeToolSelector\MergeToolSelector.exe";
                    arguments = $"-tool merge -s \"{fileA}\" -d \"{fileB}\" -b \"{fileC}\" -r \"{merged}\"";
                    arguments2 = $"-tool diff -s \"{fileB}\" -d \"{merged}\"";
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

        private string GetTempFileName(string extension, bool create=false)
        {
            var path=Path.Combine(Path.GetTempPath(), "merged_"+Guid.NewGuid().ToString("N") + extension);
            if (create)File.Create(path).Close();
            return path;
        }

        public event EventHandler CanExecuteChanged;
    }
}