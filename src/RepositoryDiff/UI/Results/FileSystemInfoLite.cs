using System.IO;
using System.Text.Json.Serialization;

namespace KsWare.RepositoryDiff.UI.Results
{
    public class FileSystemInfoLite
    {
        private string _name;

        public FileSystemInfoLite(FileSystemInfo f)
        {
            FullName = f.FullName;
            IsDirectory = f is DirectoryInfo;
            Exists = f.Exists;
        }

        public FileSystemInfoLite()
        {
        }

        public string FullName { get; set; }
        public bool IsDirectory { get; set; }
        public bool Exists { get; set; }

        [JsonIgnore]  public string Name => _name ??= Path.GetFileName(FullName);
    }
}