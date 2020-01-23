namespace KsWare.RepositoryDiff.UI.Results
{
    public class CompareResultData
    {
        public int Id { get; set; }

        public string RelativPath { get; set; }

        public bool IsDirectory { get; set; }

        public string Result { get; set; }

        public FileSystemInfoLite A { get; set; }
        public FileSystemInfoLite B { get; set; }
        public FileSystemInfoLite C { get; set; }
    }
}