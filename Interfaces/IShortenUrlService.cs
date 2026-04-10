using System.Collections.Generic;
using System.Threading.Tasks;
using ShortenUrlWeb.DTOs;

namespace ShortenUrlWeb.Services
{
    public interface IShortenUrlService
    {
        // Nhận Request từ người dùng và trả về Response chứa link đã rút gọn
        Task<ShortenUrlResponse> CreateShortUrlAsync(CreateShortenUrlRequest request);

        // Lấy URL gốc dựa vào mã rút gọn (dùng để chuyển hướng)
        Task<string> GetOriginalUrlAsync(string shortCode);

        // Lấy danh sách tất cả các URL (dùng cho trang quản trị)
        Task<IEnumerable<ShortenUrlResponse>> GetAllAsync();
    }
}