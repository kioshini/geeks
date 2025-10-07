using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramService _telegramService;

        public TelegramController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        /// <summary>
        /// Получить chat_id для настройки Telegram бота
        /// </summary>
        /// <returns>Ответ от Telegram API с информацией о чатах</returns>
        [HttpGet("get-chat-id")]
        public async Task<IActionResult> GetChatId()
        {
            try
            {
                var result = await _telegramService.GetChatIdAsync();
                return Ok(new { message = "Ответ от Telegram API:", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
