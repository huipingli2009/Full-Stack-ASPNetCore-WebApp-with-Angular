using System;
using System.ComponentModel.DataAnnotations;
namespace org.cchmc.pho.core.Models
{
    public class CustomOptions
    {

        public static readonly string SECTION_KEY = "custom_options";

        public string Option1 { get; set; }
        public int Option2Int { get; set; }

        [Required]
        public string RequiredOption { get; set; }


    }
}
