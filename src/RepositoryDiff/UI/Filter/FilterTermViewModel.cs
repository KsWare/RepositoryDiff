using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KsWare.RepositoryDiff.Common;

namespace KsWare.RepositoryDiff.UI.Filter
{
    public class FilterTermViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<FilterTermViewModel> _allTerms;
        private FilterTermViewModel _firstTerm;

        public string Not { get => Get<string>(); set => Set(value); }
        public string Value { get => Get<string>(); set => Set(value); }
        public string Operator { get => Get<string>(); set => UpdateNextIf(Set(value)); }

        public IEnumerable<string> NotOperatorItems { get; } = new[] {"", "NOT"};

        public IEnumerable<string> BoolOperatorItems { get; } = new[] {"", "OR", "AND"};

        public bool Match(string value)
        {
            var m = string.IsNullOrEmpty(Value) || value.Equals(Value) != IsNot;
            switch (Operator)
            {
                case "OR" : return m || Next.Match(Value);
                case "AND": return m && Next.Match(Value);
                default: return m;
            }
        }

        private bool IsNot => Not == "NOT";

        public FilterTermViewModel Next { get => Get<FilterTermViewModel>(); set => Set(value); }  
        public FilterTermViewModel Previous { get => Get<FilterTermViewModel>(); set => Set(value); }  

        [SuppressMessage("ReSharper", "FlagArgument",Justification = "ok")]
        private void UpdateNextIf(bool changed)
        {
            if(!changed) return;

            if ( string.IsNullOrEmpty(Operator))
            {
                if (Next != null)
                {
                    Next.Operator = string.Empty;
                    Next.Previous = null;
                    ((IList<FilterTermViewModel>) AllTerms).Remove(Next);
                    Next = null;
                }
                
            }
            else
            {
                if (Next == null)
                {
                    Next=new FilterTermViewModel();
                    Next.Previous = this;
                    ((IList<FilterTermViewModel>) AllTerms).Add(Next);
                }
                
                 AllTerms.Where(x => x != this && x.Next != null).ForEach(x => x.Operator = Operator);
                
            }
        }

        public IEnumerable<FilterTermViewModel> AllTerms => _allTerms ??= UpdateAllTerms();

        private ObservableCollection<FilterTermViewModel> UpdateAllTerms()
        {
            if (FirstTerm != this)
                return (ObservableCollection<FilterTermViewModel>) FirstTerm.AllTerms;

            var list = new ObservableCollection<FilterTermViewModel>();
            var o = FirstTerm;
            while (true)
            {
                list.Add(o);
                o = o.Next;
                if (o == null) break;
            }

            return list;
            
        }

        public FilterTermViewModel FirstTerm => _firstTerm ??= GetFirstTerm();

        private FilterTermViewModel GetFirstTerm() => Previous == null ? this : Previous.FirstTerm;
    }
}
