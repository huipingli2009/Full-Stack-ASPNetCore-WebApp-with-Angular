using System.Collections.Generic;


namespace org.cchmc.pho.core.Models
{
    public class SearchResults<T> where T : class
    {
        public int ResultCount { get; set; }
        public List<T> Results { get; set; }
    }
}
