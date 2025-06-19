namespace MESWebDev.Models.VM
{
    public class UserRoleViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}