using MESWebDev.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace MESWebDev.Services
{
    public class TranslationServiceOld : ITranslationService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "Translations_";

        public TranslationServiceOld(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public string GetTranslation(string key, string languageCode)
        {
            // Tạo key cache duy nhất cho từng ngôn ngữ
            var cacheKey = $"{CacheKeyPrefix}{languageCode}";

            // Kiểm tra xem dữ liệu đã có trong cache chưa
            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, string> translations))
            {
                // Nếu không có trong cache, lấy từ CSDL
                var languageId = _context.Languages
                    .Where(l => l.Code == languageCode)
                    .Select(l => l.LanguageId)
                    .FirstOrDefault();

                translations = _context.Translations
                    .Where(t => t.LanguageId == languageId)
                    .ToDictionary(t => t.Keyvalue, t => t.Value);

                var cts = new CancellationTokenSource();
                // Cấu hình tùy chọn cache
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    // Thời gian sống tuyệt đối: 30 phút
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    // Thời gian sống trượt: Nếu không truy cập trong 10 phút, xóa
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    // Kích thước của mục cache (dùng cho SizeLimit)
                    Size = translations.Count,
                    // Ưu tiên: Low (có thể bị xóa sớm nếu bộ nhớ đầy)
                    Priority = CacheItemPriority.Low
                }.AddExpirationToken(new CancellationChangeToken(cts.Token));

                // Lưu CancellationTokenSource vào cache với MemoryCacheEntryOptions
                var ctsCacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Size = 1, // Đặt Size cho CancellationTokenSource
                    Priority = CacheItemPriority.Low
                };

                _cache.Set($"{cacheKey}_cts", cts, ctsCacheOptions); // Lưu CancellationTokenSource vào cache
                _cache.Set(cacheKey, translations, cacheEntryOptions); // Lưu translations
            }

            // Lấy chuỗi dịch từ dictionary trong cache
            return translations.TryGetValue(key, out var value) ? value : key;
        }

        public void ClearCache(string languageCode)
        {
            var cacheKey = $"{CacheKeyPrefix}{languageCode}";
            if (_cache.TryGetValue($"{cacheKey}_cts", out CancellationTokenSource cts))
            {
                cts.Cancel(); // Hủy cache
                _cache.Remove($"{cacheKey}_cts");
            }
            _cache.Remove(cacheKey);
        }

        public string Trans(string key)
        {
            throw new NotImplementedException();
        }
    }
}