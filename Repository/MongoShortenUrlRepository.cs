using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ShortenUrlWeb.Interfaces;
using ShortenUrlWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShortenUrlWeb.Repositories
{
    public class MongoShortenUrlRepository : IShortenUrlRepository
    {
        private readonly IMongoCollection<ShortenUrlModel> _collection;

        public MongoShortenUrlRepository(IMongoDatabase database)
        {

            _collection = database.GetCollection<ShortenUrlModel>("ShortenUrls");
        }

        public async Task<ShortenUrlModel> GetByShortCodeAsync(string shortCode)
        {
            return await _collection.Find(x => x.ShortCode == shortCode).FirstOrDefaultAsync();
        }

        public async Task<ShortenUrlModel> AddAsync(ShortenUrlModel model)
        {
            await _collection.InsertOneAsync(model);
            return model;
        }

        public async Task IncrementClickCountAsync(string shortCode)
        {
            // Điểm mạnh của MongoDB: Update nguyên tử (Atomic Update)
            // Không cần tải data lên rồi mới lưu xuống, báo Mongo tự tăng ClickCount lên 1
            var filter = Builders<ShortenUrlModel>.Filter.Eq(x => x.ShortCode, shortCode);
            var update = Builders<ShortenUrlModel>.Update.Inc(x => x.ClickCount, 1);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<ShortenUrlModel>> GetAllAsync()
        {
            return await _collection.Find(_ => true)
                                     .SortByDescending(x => x.CreatedAt)
                                     .ToListAsync();
        }

        public async Task<bool> IsShortCodeExistsAsync(string shortCode)
        {
            var count = await _collection.CountDocumentsAsync(x => x.ShortCode == shortCode);
            return count > 0;
        }
    }
}