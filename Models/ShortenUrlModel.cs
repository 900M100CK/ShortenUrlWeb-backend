using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShortenUrlWeb.Models
{
    public class ShortenUrlModel
    {
        // Khai báo đây là khóa chính của MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ClickCount { get; set; } = 0;
    }
}