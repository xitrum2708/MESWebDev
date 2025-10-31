namespace WEB_HRProject.Models.UserDTO
{
    public class UserRequestDTO
    {
        public string UserId { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
