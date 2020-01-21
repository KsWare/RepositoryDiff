using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace KsWare.RepositoryDiff.Commands
{
    public class DiffCommand : ICommand
    {
        private readonly CompareResult _compareResult;

        public DiffCommand(CompareResult compareResult)
        {
            _compareResult = compareResult;
        }

        public bool CanExecute(object parameter)
        {
            if (_compareResult.C == null) // 2-way
            {
                return _compareResult.A != null && _compareResult.B != null && _compareResult.A.Exists && _compareResult.B.Exists;
            }
            else // 3-way
            {
                return true; //TODO
            }
            
        }

        public void Execute(object argument)
        {
            var merged = (string) null;
            var file = (string) null;
            var arguments = "";

            switch (argument as string)
            {
                case "SemanticMergeAB" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.A.FullName}\" -d \"{_compareResult.B.FullName}\"";
                    break;
                case "SemanticMergeAC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.A.FullName}\" -d \"{_compareResult.C.FullName}\"";
                    break;
                case "SemanticMergeBC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.B.FullName}\" -d \"{_compareResult.C.FullName}\"";
                    break;
                case "SemanticMerge3Way" : 
                    merged = Path.GetTempFileName();
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-s \"{_compareResult.A.FullName}\" -d \"{_compareResult.B.FullName}\" -b \"{_compareResult.C.FullName}\" -r \"{merged}\"";
                    break;

                case "MergeToolSelectorAB" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool diff -s \"{_compareResult.A.FullName}\" -d \"{_compareResult.B.FullName}\"";
                    break;
                case "MergeToolSelectorAC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool diff -s \"{_compareResult.A.FullName}\" -d \"{_compareResult.C.FullName}\"";
                    break;
                case "MergeToolSelectorBC" : 
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool diff -s \"{_compareResult.B.FullName}\" -d \"{_compareResult.C.FullName}\"";
                    break;
                case "MergeToolSelector3Way" : 
                    merged = Path.GetTempFileName();
                    file = @"C:\Program Files\SemanticMerge\semanticmergetool.exe";
                    arguments = $"-tool merge -s \"{_compareResult.A.FullName}\" -d \"{_compareResult.B.FullName}\" -b \"{_compareResult.C.FullName}\" -r \"{merged}\"";
                    break;
                default:
                    if (_compareResult.NameC == null) goto case "SemanticMergeAB";
                    else goto case "SemanticMerge3Way";
            }

            Process.Start(file, arguments);
        }

        public event EventHandler CanExecuteChanged;
    }
}