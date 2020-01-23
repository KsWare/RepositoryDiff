using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KsWare.RepositoryDiff.Data
{
    public class Project
    {
        public string Version { get; set; } = "1.0";

        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }
    }
}
