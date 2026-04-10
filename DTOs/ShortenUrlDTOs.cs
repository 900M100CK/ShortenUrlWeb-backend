using System;

namespace ShortenUrlWeb.DTOs
{
    public class ShortenUrlResponse
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; }

        public string ShortCode { get; set; }

        /// <summary>
        /// Đường dẫn rút gọn hoàn chỉnh để hiển thị cho người dùng (VD: https://domain.com/abc12x)
        /// </summary>
        public string ShortUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ClickCount { get; set; }
    }
}