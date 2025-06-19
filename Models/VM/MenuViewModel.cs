namespace MESWebDev.Models.VM
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public string PermissionKey { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; } // Thêm ParentId để xác định menu cha
        public string Icon { get; set; } // Thêm Icon để hiển thị biểu tượng
        public int Level { get; set; } = 0; // <- thêm dòng này
        public List<MenuViewModel> Children { get; set; } = new List<MenuViewModel>(); // Danh sách menu con
        public List<MenuViewModel> AvailableParents { get; set; } = new List<MenuViewModel>(); // Danh sách menu cha để chọn
        public List<PermissionViewModel> AvailablePermissions { get; set; } = new List<PermissionViewModel>();

        public IQueryable<MenuViewModel> ApplySearch(IQueryable<MenuViewModel> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            return query.Where(t => t.Title.Contains(searchTerm) ||
                               t.PermissionKey.Contains(searchTerm) ||
                               t.Url.Contains(searchTerm));
        }
    }
}