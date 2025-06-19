namespace MESWebDev.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Nên mã hóa
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? LanguageId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Language Language { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}