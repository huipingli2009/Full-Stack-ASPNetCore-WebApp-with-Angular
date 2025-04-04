﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class FileDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ResourceTypeViewModel ResourceType { get; set; }
        public InitiativeViewModel Initiative { get; set; }
        public string Author { get; set; }
        public FileTypeViewModel FileType { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastViewed { get; set; }
        public bool WatchFlag { get; set; }
        public string FileURL { get; set; }
        public List<FileTagViewModel> Tags { get; set; }
        public string Description { get; set; }
        public bool? PublishFlag { get; set; }
        public bool? PracticeOnly { get; set; }
        public bool? CreateAlert { get; set; }
        public WebPlacementViewModel WebPlacement { get; set; }
    }
}
