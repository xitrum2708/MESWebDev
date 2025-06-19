namespace MESWebDev.Models.VM
{
    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int PermissionId { get; set; }
        public string PermissionKey { get; set; }
        public DateTime GrantedAt { get; set; }
    }
}