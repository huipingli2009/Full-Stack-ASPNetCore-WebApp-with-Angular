using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.Mappings
{
    public class SearchResultsMappings:Profile
    {
        public SearchResultsMappings()
        {
            CreateMap(typeof(SearchResults<>), typeof(SearchResultsViewModel<>));
        }
        
    }
}
