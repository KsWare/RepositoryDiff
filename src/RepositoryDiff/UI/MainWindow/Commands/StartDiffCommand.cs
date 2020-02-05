using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using KsWare.RepositoryDiff.UI.Results;

namespace KsWare.RepositoryDiff.UI.MainWindow.Commands
{
    public class StartDiffCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public StartDiffCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            RecursiveScanFolders(_mainWindowViewModel.A, _mainWindowViewModel.B, _mainWindowViewModel.C);
        }

        public event EventHandler CanExecuteChanged;


        private void RecursiveScanFolders(string folderA, string folderB, string folderC)
        {
            Results.Clear();
            var da = new DirectoryInfo(folderA);
            var db = new DirectoryInfo(folderB);
            var dc =  !string.IsNullOrEmpty(folderC) ? new DirectoryInfo(folderC) : null;
            _mainWindowViewModel.RootA = da.FullName;
            _mainWindowViewModel.RootB = db.FullName;
            _mainWindowViewModel.RootC = dc?.FullName;
            
            RecursiveScanFolders(da, db, dc);
            Helpers.CreateHierarchy(Results);

            _mainWindowViewModel.Filter.RefreshCommand.Execute(null);
        }

        private string RootA => _mainWindowViewModel.RootA;
        private string RootB => _mainWindowViewModel.RootB;
        private string RootC => _mainWindowViewModel.RootC;
        private IList<CompareResultViewModel> Results => _mainWindowViewModel.Results;

        private string RecursiveScanFolders(DirectoryInfo folderA, DirectoryInfo folderB, DirectoryInfo folderC)
        {
            string folderName = folderA.Name;
            string relativeName = GetRelativeName(RootA,folderA.FullName);

            if (IsExcluded(relativeName)) return null;

            System.Diagnostics.Debug.WriteLine($"{relativeName}");

            if (folderC == null) // 2-way
            {
                if (folderA.Exists && folderB.Exists)
                {
                    var result = new CompareResultViewModel(relativeName, folderA, folderB, null, "", _mainWindowViewModel);
                    if(relativeName!="") Results.Add(result);
                    result.Result = ScanFilesAndSubFolders(folderA, folderB, null);
                    return result.Result;
                }
                if (!folderA.Exists && folderB.Exists)
                {
                    var result = new CompareResultViewModel(relativeName, folderA, folderB, null, "<<", _mainWindowViewModel);
                    Results.Add(result);
                    ScanFilesAndSubFolders(folderA, folderB, null);
                    return result.Result;
                }
                if (folderA.Exists && !folderB.Exists)
                {
                    var result = new CompareResultViewModel(relativeName, folderA, folderB, null, ">>", _mainWindowViewModel);
                    Results.Add(result);
                    ScanFilesAndSubFolders(folderA, folderB, null);
                    return result.Result;
                }
                throw new NotImplementedException();
            }
            else // 3-way
            {
                var result = new CompareResultViewModel(relativeName, folderA, folderB, folderC, "", _mainWindowViewModel);
                if(relativeName!="") Results.Add(result);
                result.Result =  ScanFilesAndSubFolders(folderA, folderB, folderC);

                var a1 = folderA.Exists;
                var a0 = !a1;
                var b1 = folderB.Exists;
                var b0 = !b1;
                var c1 = folderC.Exists;
                var c0 = !c1;

                if (c0 && a0 && b0) ; //                             ---
                if (c0 && a0 && b1) result.Result = "~~+"; // neu auf B
                if (c0 && a1 && b0) result.Result = "+~~"; // neu auf A
                if (c0 && a1 && b1) result.Result = "+.+"; // neu auf beiden Seiten; 
                if (c1 && a0 && b0) result.Result = "-=-"; // gelöscht auf beiden Seiten
                if (c1 && a0 && b1) result.Result = "-??"; // gelöscht auf A;
                if (c1 && a1 && b0) result.Result = "??-"; // gelöscht auf B;
                if (c1 && a1 && b1) ; // 


                return result.Result;
            }
        }

        private string ScanFilesAndSubFolders(DirectoryInfo folderA, DirectoryInfo folderB, DirectoryInfo folderC)
        {
            var f = ScanFiles(folderA, folderB, folderC);
            var d = RecursiveScanSubFolders(folderA, folderB, folderC);
            return folderC==null ? CombineResult2Way(new[]{f,d}) : CombineResult3Way(new[]{f,d});
        }


        private string RecursiveScanSubFolders(DirectoryInfo folderA, DirectoryInfo folderB, DirectoryInfo folderC)
        {
            var aEntries = folderA.Exists ? folderA.GetDirectories() : new DirectoryInfo[0];
            var bEntries = folderB.Exists ? folderB.GetDirectories() : new DirectoryInfo[0];
            var cEntries = folderC?.Exists??false ? folderC?.GetDirectories() : new DirectoryInfo[0];
            var names = GetUniqueNames(aEntries, bEntries, cEntries);
            var results = new List<string>();
            foreach (var name in names)
            {
                var a = aEntries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        ?? new DirectoryInfo(Path.Combine(folderA.FullName, name));
                var b = bEntries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        ?? new DirectoryInfo(Path.Combine(folderB.FullName, name));
                var c = folderC != null
                    ? cEntries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                      ?? new DirectoryInfo(Path.Combine(folderC.FullName, name))
                    : null;
                var r = RecursiveScanFolders(a, b, c);
                results.Add(r);
            }

            return folderC==null ? CombineResult2Way(results) : CombineResult3Way(results);
        }

        private string ScanFiles(DirectoryInfo folderA, DirectoryInfo folderB, DirectoryInfo folderC)
        {
            var aEntries = folderA.Exists ? folderA.GetFiles() : new FileInfo[0];
            var bEntries = folderB.Exists ? folderB.GetFiles() : new FileInfo[0];
            var cEntries = folderC?.Exists ?? false ? folderC.GetFiles() : new FileInfo[0];
            var names = GetUniqueNames(aEntries, bEntries, cEntries);
            var results = new List<string>();
            foreach (var name in names)
            {
                var a = aEntries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        ?? new FileInfo(Path.Combine(folderA.FullName, name));
                var b = bEntries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        ?? new FileInfo(Path.Combine(folderB.FullName, name));
                var c = folderC != null // only 2-way compare
                    ? cEntries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                      ?? new FileInfo(Path.Combine(folderC.FullName, name))
                    :null;
                var r = CompareFile(a, b, c);
                results.Add(r);
            }

            return folderC==null ? CombineResult2Way(results) : CombineResult3Way(results);
        }

        private string CombineResult2Way(IEnumerable<string> results)
        {
            char a='=', b='=';
            foreach (var r in results)
            {
                if(r==null) continue; // excluded
                if (r[0] == '!' || a == '!') a = '!'; else if (a == '=') a = r[0]; else if (a != r[0]) a = '*';
                if (r[1] == '!' || b == '!') b = '!'; else if (b == '=') b = r[1]; else if (b != r[1]) b = '*';
            }
            return $"{a}{b}";

        }
        private string CombineResult3Way(IEnumerable<string> results)
        {
            char a='=', b='=', c='=';
            foreach (var r in results)
            {
                if(r==null) continue; // excluded
                if (r[0] == '!' || a == '!') a = '!'; else if (a == '=') a = r[0]; else if (a != r[0]) a = '*';
                if (r[1] == '!' || c == '!') c = '!'; else if (c == '=') c = r[1]; else if (c != r[1]) c = '*';
                if (r[2] == '!' || b == '!') b = '!'; else if (b == '=') b = r[2]; else if (b != r[2]) b = '*';
            }
            return $"{a}{c}{b}";
        }

        private string CompareFile(FileInfo a, FileInfo b, FileInfo c)
        {
            string relativeName = GetRelativeName(RootA,a.FullName);
            if (IsExcluded(relativeName)) return null;

            if (c == null) // 2-way
            {
                var result = "";
                if (a.Exists && b.Exists)
                {
                    result = CompareBinaryFileContent(a, b, c);
                }
                else if (!a.Exists && b.Exists)
                {
                    result = "<<";
                }
                else if (a.Exists && !b.Exists)
                {
                    result = ">>";
                }
                else
                {
                    throw new NotImplementedException();
                }
                Results.Add(new CompareResultViewModel(relativeName, a, b, c, result, _mainWindowViewModel));
                return result;
            }
            else // 3-way
            {
                var result = "";
                var a1 = a.Exists;
                var a0 = !a1;
                var b1 = b.Exists;
                var b0 = !b1;
                var c1 = c.Exists;
                var c0 = !c1;
                if      (c0 && a0 && b0) result = "???"; //                             ---
                else if (c0 && a0 && b1) result = "~~+"; //                             neu auf B                   ~~+
                else if (c0 && a1 && b0) result = "+~~"; //                             neu auf A                   +~~
                else if (c0 && a1 && b1) result = CompareBinaryFileContent(a, b, c); // neu auf beiden Seiten;      +=+ +!+
                else if (c1 && a0 && b0) result = "-=-"; //                             gelöscht auf beiden Seiten  -=-
                else if (c1 && a0 && b1) result = CompareBinaryFileContent(a, b, c); // gelöscht auf A;             -!# -!=
                else if (c1 && a1 && b0) result = CompareBinaryFileContent(a, b, c); // gelöscht auf B;             #!- =!-
                else if (c1 && a1 && b1) result = CompareBinaryFileContent(a, b, c); //                             === #.= =.# #!#

                Results.Add(new CompareResultViewModel(relativeName, a, b, c, result, _mainWindowViewModel));
                return result;
            }
        }

        private string CompareBinaryFileContent(FileInfo a, FileInfo b, FileInfo c)
        {
            if (c == null) return CompareBinaryFileContent(a, b);

            if (a.Exists && b.Exists && c.Exists)
            {
                var minLength = Math.Min(Math.Min(a.Length, b.Length), c.Length);
                const int BYTES_TO_READ = sizeof(Int64);
                int iterations = (int) Math.Ceiling((double) minLength / BYTES_TO_READ);

                var acResult = a.Length==c.Length ? "==" : "!!";
                var bcResult = b.Length==c.Length ? "==" : "!!";
                var abResult = a.Length==b.Length ? "==" : "!!";

                using (FileStream fs1 = a.OpenRead())
                using (FileStream fs2 = b.OpenRead())
                using (FileStream fs3 = c.OpenRead())
                {
                    byte[] ac = new byte[BYTES_TO_READ];
                    byte[] bc = new byte[BYTES_TO_READ];
                    byte[] cc = new byte[BYTES_TO_READ];


                    for (int i = 0; i < iterations; i++)
                    {
                        fs1.Read(ac, 0, BYTES_TO_READ);
                        fs2.Read(bc, 0, BYTES_TO_READ);
                        fs3.Read(cc, 0, BYTES_TO_READ);

                        var av = BitConverter.ToInt64(ac, 0);
                        var bv = BitConverter.ToInt64(bc, 0);
                        var cv = BitConverter.ToInt64(cc, 0);

                        if (av != cv) acResult = "!!";
                        if (bv != cv) bcResult = "!!";
                        if (av != bv) abResult = "!!";
                        if(acResult=="!!" &&bcResult=="!!" && abResult=="!!") break;
                    }
                }
                if (acResult == "==" && bcResult == "==") return "==="; // keine Änderungen
                if (acResult == "!!" && bcResult == "==") return "#.="; // A geändert
                if (acResult == "==" && bcResult == "!!") return "=.#"; // B geändert
                if (abResult == "=="                    ) return "#=#"; // gleiche Änderungen auf A und B
                /*                                     */ return "#!#"; // A und B geändert, Konflikt
            }

            if (a.Exists && c.Exists)
            {
                return CompareBinaryFileContent(a, c) == "==" 
                    ? "=.-"  // A unverändert, B gelöscht   => nix zu tun
                    : "#!-"; // A verändert, B gelöscht     => Konflikt
            }
            if (b.Exists && c.Exists)
            {
                return CompareBinaryFileContent(b, c) == "==" 
                    ? "-.=" // A gelöscht, B unverändert   => B löschen
                    : "-!#";// A gelöscht, B geändert,     => Konflikt
            }
            if (a.Exists && b.Exists)
            {
                return CompareBinaryFileContent(a, b) == "==" 
                    ? "+=+" // A und B neu, A gleich B      => nix zu tun
                    : "+!+";// A und B neu, A ungleich B    => Konflikt
            }

            throw new NotImplementedException();
        }

        private string CompareBinaryFileContent(FileInfo a, FileInfo b)
        {
            const int BYTES_TO_READ = sizeof(Int64);


            if (a.Length != b.Length) return "!!";


            int iterations = (int) Math.Ceiling((double) a.Length / BYTES_TO_READ);

            using (FileStream fs1 = a.OpenRead())
            using (FileStream fs2 = b.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return "!!";
                }
            }

            return "==";
        }

        public string[] GetUniqueNames(IEnumerable<FileSystemInfo> aFolders, IEnumerable<FileSystemInfo> bFolders,
            IEnumerable<FileSystemInfo> cFolders)
        {
            var names = ((aFolders ?? new FileSystemInfo[0]).Select(x => x.Name))
                .Concat((bFolders ?? new FileSystemInfo[0]).Select(x => x.Name))
                .Concat((cFolders ?? new FileSystemInfo[0]).Select(x => x.Name))
                .OrderBy(x => x, StringComparer.CurrentCultureIgnoreCase)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            return names;
        }

        internal static string GetRelativeName(string root, string path)
        {
            if (root == null || path == null) return null;
            var p = path.Substring(root.Length).TrimStart('\\');
            return p;
        }

        private bool IsExcluded(string relativeName)
        {
            foreach (var regex in _mainWindowViewModel.Excludes)
            {
                if (regex.IsMatch(relativeName)) return true;
            }

            return false;
        }
    }
}