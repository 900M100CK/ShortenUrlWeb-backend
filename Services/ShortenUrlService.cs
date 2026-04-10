using ShortenUrlWeb.DTOs;
using ShortenUrlWeb.Interfaces;
using ShortenUrlWeb.Models;
using Microsoft.Extensions.Configuration;

namespace ShortenUrlWeb.Services
{
    public class ShortenUrlService : IShortenUrlService
    {
        private readonly IShortenUrlRepository _repository;
        private readonly string _baseUrl;

        public ShortenUrlService(IShortenUrlRepository repository, IConfiguration config)
        {
            _repository = repository;
            _baseUrl = config["App:BaseUrl"]
                       ?? throw new InvalidOperationException("Thiếu App:BaseUrl trong config.");
        }

        public async Task<ShortenUrlResponse> CreateShortUrlAsync(CreateShortenUrlRequest request)
        {
            string shortCode;
            do
            {
                shortCode = GenerateRandomString(6);
            }
            while (await _repository.IsShortCodeExistsAsync(shortCode));

            var newUrlModel = new ShortenUrlModel
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.Now,
                ClickCount = 0
            };

            var savedModel = await _repository.AddAsync(newUrlModel);

            return new ShortenUrlResponse
            {
                Id = int.TryParse(savedModel.Id, out var parsedId) ? parsedId : 0,
                OriginalUrl = savedModel.OriginalUrl,
                ShortCode = savedModel.ShortCode,
                ShortUrl = $"{_baseUrl}/{savedModel.ShortCode}",
                CreatedAt = savedModel.CreatedAt,
                ClickCount = savedModel.ClickCount
            };
        }

        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            var urlInfo = await _repository.GetByShortCodeAsync(shortCode);
            if (urlInfo == null) return null;

            await _repository.IncrementClickCountAsync(shortCode);
            return urlInfo.OriginalUrl;
        }

        public async Task<IEnumerable<ShortenUrlResponse>> GetAllAsync()
        {
            var urls = await _repository.GetAllAsync();
            return urls.Select(u => new ShortenUrlResponse
            {
                Id = int.TryParse(u.Id, out var parsedId) ? parsedId : 0,
                OriginalUrl = u.OriginalUrl,
                ShortCode = u.ShortCode,
                ShortUrl = $"{_baseUrl}/{u.ShortCode}",
                CreatedAt = u.CreatedAt,
                ClickCount = u.ClickCount
            });
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}