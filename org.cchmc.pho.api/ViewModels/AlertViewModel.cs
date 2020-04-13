namespace org.cchmc.pho.api.ViewModels
{
    public class AlertViewModel
    {
        public int AlertId { get; set; }
        public int AlertScheduleId { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string LinkText { get; set; }
        public string Definition { get; set; }
        public string Target { get; set; }
        public string FilterType { get; set; }
        public string FilterName { get; set; }
        public int ? FilterValue { get; set; }
    }
}
