namespace csharpwebsite.Server.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string UploadPath { get; set; }
        public string DefaultAvatarPath { get; set; }
    }
}