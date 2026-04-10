using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShortenUrlWeb.DTOs;
using ShortenUrlWeb.Services;

namespace ShortenUrlWeb.Controllers
{
    [Route("api/shorten")] // Đặt tên route ngắn gọn là api/shorten
    [ApiController]
    public class ShortenUrlController : ControllerBase
    {
        private readonly IShortenUrlService _service;

        // Tiêm (Inject) Service vào Controller
        public ShortenUrlController(IShortenUrlService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo một URL rút gọn mới
        /// </summary>
        // POST: api/shorten
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShortenUrlRequest request)
        {
            // Kiểm tra xem dữ liệu gửi lên có hợp lệ không (đã gắn [Url] và [Required] ở DTO)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi Service để xử lý và lấy kết quả trả về
            var response = await _service.CreateShortUrlAsync(request);

            return Ok(response); // Trả về HTTP 200 kèm cục dữ liệu JSON
        }

        /// <summary>
        /// Lấy danh sách tất cả các URL đã rút gọn (Dùng cho Admin)
        /// </summary>
        // GET: api/shorten
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var urls = await _service.GetAllAsync();
            return Ok(urls);
        }

        /// <summary>
        /// Chức năng quan trọng nhất: Chuyển hướng người dùng khi họ truy cập link rút gọn
        /// </summary>
        // GET: /{shortCode} 
        // Dấu "/" ở đầu rất quan trọng, nó giúp route bắt thẳng từ domain gốc. VD: https://localhost:5001/abc12x
        [HttpGet("/{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var originalUrl = await _service.GetOriginalUrlAsync(shortCode);

            if (string.IsNullOrEmpty(originalUrl))
            {
                // Trả về lỗi 404 nếu mã không tồn tại
                return NotFound("Không tìm thấy đường dẫn hoặc liên kết đã bị xóa.");
            }

            // Thực hiện HTTP Redirect (Chuyển hướng 302) tới đường dẫn gốc
            return Redirect(originalUrl);
        }
    }
}