namespace org.cchmc.pho.core.DataModels
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int AlertScheduleId { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string LinkText { get; set; }
        public string Definition { get; set; }
    }
}
