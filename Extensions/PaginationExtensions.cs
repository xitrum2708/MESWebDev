using MESWebDev.Common;

namespace MESWebDev.Extensions
{
    public static class PaginationExtensions
    {
        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query, int page, int pageSize, string searchTerm = null)
        {
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            // Đếm tổng số bản ghi
            result.TotalItems = query.Count();

            // If there are no items, return an empty result
            if (result.TotalItems == 0)
            {
                result.CurrentPage = 1;
                result.TotalPages = 0;
                result.Items = new List<T>();
                return result;
            }

            // Tính tổng số trang
            result.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);

            // Đảm bảo page hợp lệ
            result.CurrentPage = page < 1 ? 1 : page;
            result.CurrentPage = page > result.TotalPages ? result.TotalPages : page;

            // Lấy dữ liệu cho trang hiện tại
            result.Items = query
                .Skip((result.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return result;
        }
    }
}