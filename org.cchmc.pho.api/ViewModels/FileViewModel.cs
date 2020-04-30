using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class FileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastViewed { get; set; }
        public bool WatchFlag { get; set; }
        public string FileURL { get; set; }
        public int FileTypeId { get; set; }
        public string FileType { get; set; }
        public bool? PublishFlag { get; set; }
        public List<FileTagViewModel> Tags { get; set; }
        public string Description { get; set; }
        public string IconImage { get; set; }
    }
    public class PopularFileViewModel : FileViewModel
    {
        public int ViewCount { get; set; }
    }
    public class FileTagViewModel
    {
        public string Name { get; set; }
    }
    public class FileTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ResourceTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class InitiativeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ResourceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
