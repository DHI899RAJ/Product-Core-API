namespace ProductManagementAPI.Models
{
    public class RequestLog
    {
        public int Id { get; set; }
        public string RequestMethod { get; set; } = string.Empty;
        public string RequestPath { get; set; } = string.Empty;
        public int? StatusCode { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
