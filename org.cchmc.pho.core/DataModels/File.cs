using System;
using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{
    public class File
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
        public List<FileTag> Tags { get; set; }
        public string Description { get; set; }
    }

    public class FileTag
    {
        public string Name { get; set; }
    }
    public class FileType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Initiative
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
