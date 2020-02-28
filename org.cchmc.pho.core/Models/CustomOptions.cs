using System;
using System.ComponentModel.DataAnnotations;
namespace org.cchmc.pho.core.Models
{
    public class CustomOptions
    {

        public static readonly string SECTION_KEY = "custom_options";

        public int MinimumPasswordLength { get; set; }
        public bool RequireNonAlphaNumeric { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireDigit { get; set; }
        public int MaximumAttemptsBeforeLockout { get; set; }
    }
}
