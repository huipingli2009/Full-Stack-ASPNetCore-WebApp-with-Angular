using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class SearchResultsViewModel <T> where T:class
    {
        public int ResultCount { get; set; }
        public List<T> Results { get; set; }
    }
}
