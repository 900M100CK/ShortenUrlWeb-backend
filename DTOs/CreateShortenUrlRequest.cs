using System.ComponentModel.DataAnnotations;

namespace ShortenUrlWeb.DTOs
{
    public class CreateShortenUrlRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập URL cần rút gọn.")]
        [Url(ErrorMessage = "Định dạng URL không hợp lệ. Vui lòng nhập đúng định dạng (VD: https://example.com)")]
        public string OriginalUrl { get; set; }
    }
}