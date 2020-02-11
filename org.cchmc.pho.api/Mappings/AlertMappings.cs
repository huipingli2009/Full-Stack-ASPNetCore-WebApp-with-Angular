using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.Mappings
{
    public class AlertMappings : Profile
    {
        public AlertMappings()
        {
            CreateMap<Alert, AlertViewModel>();
        }
    }
}
