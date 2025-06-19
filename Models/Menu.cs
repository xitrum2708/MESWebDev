namespace MESWebDev.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PermissionKey { get; set; }
        public Menu Parent { get; set; }
        public ICollection<Menu> Children { get; set; }
        public ICollection<MenuTranslation> MenuTranslations { get; set; }
    }
}