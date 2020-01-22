using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace KsWare.RepositoryDiff
{
    public class OptionsViewModel : NotifyPropertyChangedBase
    {
        private bool _showMergedDiff = true;
        private bool _acknowledgeMerge=true;
        private string _a;
        private string _b;
        private string _c;

        public bool ShowMergedDiff
        {
            get => _showMergedDiff;
            set => Set(ref _showMergedDiff, value);
        }

        public bool AcknowledgeMerge
        {
            get => _acknowledgeMerge;
            set => Set(ref _acknowledgeMerge, value);
        }

        public string A
        {
            get => _a;
            set => Set(ref _a, value);
        }

        public string B
        {
            get => _b;
            set => Set(ref _b, value);
        }

        public string C
        {
            get => _c;
            set => Set(ref _c, value);
        }

        static string _fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KsWare",
            "RepositoryDiff", "config.json");

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fileName));
            var json=JsonSerializer.Serialize(this,new JsonSerializerOptions{WriteIndented = true});
            using var w=new StreamWriter(File.Create(_fileName),Encoding.UTF8);
            w.Write(json);
        }

        public static OptionsViewModel LoadOrCreate()
        {
            
            if (File.Exists(_fileName))
            {
                using var r=File.OpenText(_fileName);
                var d = JsonSerializer.Deserialize<OptionsViewModel>(r.ReadToEnd());
                return d;
            }
            else
            {
                var d = new OptionsViewModel();
                d.Save();
                return d;
            }
        }
    }
}
