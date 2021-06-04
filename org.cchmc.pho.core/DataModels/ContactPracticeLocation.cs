namespace org.cchmc.pho.core.DataModels
{
    public class ContactPracticeLocation
    {
        public int PracticeId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string OfficePhone { get; set; }
        public string Fax { get; set; }
        public string County { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}
