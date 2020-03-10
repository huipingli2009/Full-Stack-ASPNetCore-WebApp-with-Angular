using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace org.cchmc.pho.core.Models
{
    public class CustomOptions
    {

        public static readonly string SECTION_KEY = "custom_options";

        [Required]
        public int MinimumPasswordLength { get; set; }
        [Required]
        public bool RequireNonAlphaNumeric { get; set; }
        [Required]
        public bool RequireLowercase { get; set; }
        [Required]
        public bool RequireUppercase { get; set; }
        [Required]
        public bool RequireDigit { get; set; }
        [Required]
        public int MaximumAttemptsBeforeLockout { get; set; }
        [Required]
        public List<string> DoNotLogMetaDataForPaths { get; set; }
    }
}
