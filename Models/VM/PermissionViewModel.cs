using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.VM
{
    public class PermissionViewModel
    {
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "Please input PermissionKey.")]
        public string PermissionKey { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public IQueryable<PermissionViewModel> ApplySearch(IQueryable<PermissionViewModel> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            return query.Where(t => t.PermissionKey.Contains(searchTerm) ||
                               t.Description.Contains(searchTerm));
        }
    }
}