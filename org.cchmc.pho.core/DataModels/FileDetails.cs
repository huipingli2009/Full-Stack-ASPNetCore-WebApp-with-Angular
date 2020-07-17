using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class FileDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ResourceType ResourceType { get; set; }
        public Initiative Initiative { get; set; }
        public FileType FileType { get; set; }
        public string Author { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastViewed { get; set; }
        public bool WatchFlag { get; set; }
        public string FileURL { get; set; }
        public List<FileTag> Tags { get; set; }
        public string Description { get; set; }
        public bool? PublishFlag { get; set; }
        public bool? PracticeOnly { get; set; }
        public bool? CreateAlert { get; set; }
        public WebPlacement WebPlacement { get; set; }
    }
}
