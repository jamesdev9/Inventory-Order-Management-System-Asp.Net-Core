namespace Indotalent.AppSettings
{
    public class ApplicationConfiguration
    {
        public string AppName { get; set; } = string.Empty;
        public bool IsDemoVersion { get; set; } = false;
        public bool IsDevelopment { get; set; } = true;
        public string Homepage { get; set; } = string.Empty;
        public string LoginPage { get; set; } = string.Empty;
        public string LogoutPage { get; set; } = string.Empty;
        public string AccessDeniedPage { get; set; } = string.Empty;
        public string AccountProfilePage { get; set; } = string.Empty;
        public string InternalUserRegistrationPage { get; set; } = string.Empty;
        public string DefaultAdminEmail { get; set; } = string.Empty;
        public string DefaultAdminFullName { get; set; } = string.Empty;
        public string DefaultPassword { get; set; } = string.Empty;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool ExternalLoginEnabled { get; set; } = false;
        public string RoleInternalName { get; set; } = string.Empty;
        public string RoleCustomerName { get; set; } = string.Empty;
        public string RoleVendorName { get; set; } = string.Empty;
    }
}
