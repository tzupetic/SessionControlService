namespace SessionControlService.Controllers.ControllerModels
{
    public class SessionSearchRequest
    {
        public string Content { get; set; } = "";
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
