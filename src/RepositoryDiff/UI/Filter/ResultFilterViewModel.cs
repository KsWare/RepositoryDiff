using KsWare.RepositoryDiff.Common;

namespace KsWare.RepositoryDiff.UI.Filter
{
    public class ResultFilterViewModel : NotifyPropertyChangedBase
    {
        public ResultFilterData Data { get; set; }

        public bool Unchanged // =
        {
            get => Data.Unchanged;
            set => Set(() => Data.Unchanged, v => Data.Unchanged = v, value);
        }

        public bool Changed // #
        {
            get => Data.Changed;
            set => Set(() => Data.Changed, v => Data.Changed = v, value);
        }

        public bool Deleted // -
        {
            get => Data.Deleted;
            set => Set(() => Data.Deleted, v => Data.Deleted = v, value);
        }

        public bool Created // +
        {
            get => Data.Created;
            set => Set(() => Data.Created, v => Data.Created = v, value);
        }

        public bool Conflict // !
        {
            get => Data.Conflict;
            set => Set(() => Data.Conflict, v => Data.Conflict = v, value);
        }

        public bool Mixed // *
        {
            get => Data.Mixed;
            set => Set(() => Data.Mixed, v => Data.Mixed = v, value);
        }

        public bool Match(string result)
        {
            if (Unchanged == false && (result == "Unchanged" || result == "Equal" || result == "=")) return false;
            if (Changed == false && (result == "Changed" || result == "#")) return false;
            if (Deleted == false && (result == "Deleted" || result == "-")) return false;
            if (Created == false && (result == "Created" || result == "+")) return false;
            if (Conflict == false && (result == "Conflict" || result == "!")) return false;
            if (Mixed == false && (result == "Mixed" || result == "*")) return false;
            //if (X == false && (result == "xxx" || result == ".")) return false;
            //if (X == false && (result == "xxx" || result == "~")) return false;
            return true;
        }
    }

    public class ResultFilterData
    {
        public bool Unchanged { get; set; } = true;

        public bool Changed { get; set; } = true;

        public bool Deleted { get; set; } = true;

        public bool Conflict { get; set; } = true;

        public bool Created { get; set; } = true;

        public bool Mixed { get; set; } = true;
    }
}
