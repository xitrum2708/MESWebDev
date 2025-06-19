namespace MESWebDev.Models.VM
{
    public class LanguageViewModel
    {
        public int LanguageId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public IQueryable<LanguageViewModel> ApplySearch(IQueryable<LanguageViewModel> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            return query.Where(t => t.Name.Contains(searchTerm) ||
                               t.Code.Contains(searchTerm));
        }
    }
}