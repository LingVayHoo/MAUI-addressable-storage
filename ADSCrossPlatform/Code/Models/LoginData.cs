namespace ADSCrossPlatform.Code.Models
{
    public class LoginData
    {
        public string UserName { get; set; } = string.Empty;
        public string OtherData { get; set; } = string.Empty;
        public Dictionary<string, string>? Storages { get; set; }
    }
}
