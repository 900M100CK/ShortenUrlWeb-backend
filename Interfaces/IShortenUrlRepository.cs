using System.Collections.Generic;
using System.Threading.Tasks;
using ShortenUrlWeb.Models;

namespace ShortenUrlWeb.Interfaces
{
    public interface IShortenUrlRepository
    {
        // Lấy thông tin URL dựa vào mã rút gọn
        Task<ShortenUrlModel> GetByShortCodeAsync(string shortCode);

        // Thêm mới một URL rút gọn vào Database
        Task<ShortenUrlModel> AddAsync(ShortenUrlModel model);

        // Tăng số lượt click lên 1 khi có người truy cập
        Task IncrementClickCountAsync(string shortCode);

        // (Tùy chọn) Lấy danh sách tất cả các URL đã tạo
        Task<IEnumerable<ShortenUrlModel>> GetAllAsync();

        // (Tùy chọn) Kiểm tra xem mã rút gọn đã tồn tại chưa để tránh trùng lặp
        Task<bool> IsShortCodeExistsAsync(string shortCode);
    }
}