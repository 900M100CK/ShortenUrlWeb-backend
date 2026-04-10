using ShortenUrlWeb.DTOs;
using ShortenUrlWeb.Interfaces;
using ShortenUrlWeb.Models;
using ShortenUrlWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShortenUrlWeb.Services
{
    public class ShortenUrlService : IShortenUrlService
    {
        private readonly IShortenUrlRepository _repository;
        private const string Domain = "https://localhost:7023/"; // Đổi thành domain thực tế của bạn sau này

        public ShortenUrlService(IShortenUrlRepository repository)
        {
            _repository = repository;
        }

        public async Task<ShortenUrlResponse> CreateShortUrlAsync(CreateShortenUrlRequest request)
        {
            // 1. Tạo mã rút gọn ngẫu nhiên và kiểm tra trùng lặp
            string shortCode;
            do
            {
                shortCode = GenerateRandomString(6); // Tạo chuỗi 6 ký tự
            }
            while (await _repository.IsShortCodeExistsAsync(shortCode));

            // 2. Map dữ liệu từ DTO sang Model để lưu vào Database
            var newUrlModel = new ShortenUrlModel
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.Now,
                ClickCount = 0
            };

            var savedModel = await _repository.AddAsync(newUrlModel);

            // 3. Map dữ liệu từ Model sang DTO để trả về cho Client
            return new ShortenUrlResponse
            {
                Id = int.TryParse(savedModel.Id, out var parsedId) ? parsedId : 0,
                OriginalUrl = savedModel.OriginalUrl,
                ShortCode = savedModel.ShortCode,
                ShortUrl = $"{Domain}{savedModel.ShortCode}", // Ghép domain với mã
                CreatedAt = savedModel.CreatedAt,
                ClickCount = savedModel.ClickCount
            };
        }

        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            var urlInfo = await _repository.GetByShortCodeAsync(shortCode);

            if (urlInfo == null)
            {
                return null; // Không tìm thấy
            }

            // Tăng số lượt click lên 1
            await _repository.IncrementClickCountAsync(shortCode);

            return urlInfo.OriginalUrl;
        }

        public async Task<IEnumerable<ShortenUrlResponse>> GetAllAsync()
        {
            var urls = await _repository.GetAllAsync();

            // Chuyển đổi danh sách Model sang danh sách DTO
            return urls.Select(u => new ShortenUrlResponse
            {
                Id = int.TryParse(u.Id, out var parsedId) ? parsedId : 0,
                OriginalUrl = u.OriginalUrl,
                ShortCode = u.ShortCode,
                ShortUrl = $"{Domain}{u.ShortCode}",
                CreatedAt = u.CreatedAt,
                ClickCount = u.ClickCount
            });
        }

        // Hàm Private hỗ trợ tạo chuỗi ngẫu nhiên
        private string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}