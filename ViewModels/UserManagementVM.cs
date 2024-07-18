namespace Task4_registration_and_authentication.ViewModels
{
    public class UserManagementVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public bool IsBlocked { get; set; }
    }
}
