using System.Text.Json.Serialization;

namespace KsWare.RepositoryDiff.Data
{
    public class FileFilter
    {
        public string Version { get; set; } = "1.0";
        public string Name { get; set; }
        public string Description { get; set; }
        
        public string Include { get; set; }
        public string Exclude { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }


        public static FileFilter Default=new FileFilter
        {
            Name="Visual Studio",
            Description="This is a directory/file filter template for RepositoryDiff",
            Exclude = @":: (Inline comments begin with ""::"" and extend to the end of the line)
(^|\\)\.
^BuildOutput$
^packages$
^_ReSharper.Caches$
\\(bin|obj|build)$
\.user$
\.suo$
\.sln\.docstates$
\.pdb$
\.vspscc$
\.vssscc$
\.log
(^|\\)_UpgradeReport_Files$
(^|\\)Backup
(^|\\)UpgradeLog.*\.xml$
(^|\\)UpgradeLog.*\.htm$
(^|\\)Desktop\.ini$
(^|\\)CodeGraphData$
\.rtflow\.xml$
(^|\\)Thumbs.db$
(^|\\)ehthumbs.db$
(^|\\)\$RECYCLE\.BIN$
\.tmp$
"};
    }
}
