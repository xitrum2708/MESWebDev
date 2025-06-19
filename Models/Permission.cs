namespace MESWebDev.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string PermissionKey { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}