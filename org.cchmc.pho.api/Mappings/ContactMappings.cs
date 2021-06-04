﻿using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class ContactMappings : Profile
    {
        public ContactMappings()
        {
            CreateMap<Contact, ContactViewModel>();
            CreateMap<ContactPracticeDetails, ContactPracticeDetailsVidewModel>();
            CreateMap<ContactPracticeLocation, ContactPracticeLocationViewModel>(); 
        }
    }
}
