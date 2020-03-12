using System.Collections.Generic;


namespace org.cchmc.pho.api.ViewModels
{
    public class SearchResultsViewModel <T> where T:class
    {
        public int ResultCount { get; set; }
        public List<T> Results { get; set; }
    }
}
