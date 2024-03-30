namespace Indotalent.AppSettings
{
    public class IdentitySettings
    {
        public const string IdentitySettingsName = "IdentitySettings";
        public bool RequireConfirmedAccount { get; set; }
        public bool RequireDigit { get; set; }
        public int RequiredLength { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public TimeSpan DefaultLockoutTimeSpan { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }
}
