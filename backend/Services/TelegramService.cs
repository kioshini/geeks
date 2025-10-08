using System.Text;
using System.Text.Json;
using TMKMiniApp.Models;

namespace TMKMiniApp.Services
{
    public interface ITelegramService
    {
        Task SendOrderAsync(Order order);
        Task<string> GetChatIdAsync();
    }

    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramService> _logger;
        private readonly string _botToken;
        private readonly string? _chatId;

        public TelegramService(HttpClient httpClient, IConfiguration configuration, ILogger<TelegramService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? _configuration["Telegram:BotToken"];
            _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID") ?? _configuration["Telegram:ChatId"];
            
            if (string.IsNullOrEmpty(_chatId))
            {
                _logger.LogWarning("⚠️  TELEGRAM CHAT_ID НЕ НАСТРОЕН!");
                _logger.LogWarning("Для получения chat_id выполните следующие шаги:");
                _logger.LogWarning("1. Найдите бота @your_bot_name в Telegram");
                _logger.LogWarning("2. Отправьте боту любое сообщение");
                _logger.LogWarning("3. Выполните запрос: GET https://api.telegram.org/bot{0}/getUpdates", _botToken);
                _logger.LogWarning("4. Найдите 'chat':{'id': НУЖНЫЙ_ID} в ответе");
                _logger.LogWarning("5. Добавьте этот ID в appsettings.json как 'Telegram:ChatId'");
            }
        }

        public async Task SendOrderAsync(Order order)
        {
            _logger.LogInformation("🔍 Попытка отправить заказ #{OrderId} в Telegram", order.Id);
            _logger.LogInformation("🔑 BotToken: {BotToken}", string.IsNullOrEmpty(_botToken) ? "НЕ НАСТРОЕН" : "НАСТРОЕН");
            _logger.LogInformation("💬 ChatId: {ChatId}", string.IsNullOrEmpty(_chatId) ? "НЕ НАСТРОЕН" : _chatId);

            if (string.IsNullOrEmpty(_botToken))
            {
                _logger.LogError("❌ Не удалось отправить заказ: BotToken не настроен");
                return;
            }

            if (string.IsNullOrEmpty(_chatId))
            {
                _logger.LogError("❌ Не удалось отправить заказ: ChatId не настроен");
                return;
            }

            try
            {
                var message = FormatOrderMessage(order);
                _logger.LogInformation("📝 Сформировано сообщение для Telegram: {MessageLength} символов", message.Length);
                
                await SendMessageAsync(message);
                _logger.LogInformation("✅ Заказ #{OrderId} успешно отправлен в Telegram", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка при отправке заказа #{OrderId} в Telegram", order.Id);
            }
        }

        public async Task<string> GetChatIdAsync()
        {
            try
            {
                var url = $"https://api.telegram.org/bot{_botToken}/getUpdates";
                var response = await _httpClient.GetStringAsync(url);
                
                _logger.LogInformation("Ответ от Telegram API:");
                _logger.LogInformation(response);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении chat_id");
                return $"Ошибка: {ex.Message}";
            }
        }

        private string FormatOrderMessage(Order order)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("📦 **НОВЫЙ ЗАКАЗ**");
            sb.AppendLine($"ID: {order.Id}");
            sb.AppendLine($"Дата: {order.CreatedAt:yyyy-MM-dd HH:mm} (UTC)");
            sb.AppendLine($"Имя: {order.FirstName} {order.LastName}");
            sb.AppendLine($"ИНН: {order.INN}");
            sb.AppendLine($"Телефон: {order.Phone}");
            sb.AppendLine($"Email: {order.Email}");
            
            
            if (!string.IsNullOrWhiteSpace(order.Comment))
                sb.AppendLine($"\nПримечание: {order.Comment}");
            
            sb.AppendLine("\n**Товары:**");
            int idx = 1;
            foreach (var item in order.Items)
            {
                var unitReadable = GetUnitDisplay(item.Unit);
                var posSum = item.Price * (decimal)item.Quantity;
                sb.AppendLine($"{idx}. {item.Name} (ID:{item.ID}) — {item.Quantity} {unitReadable} × {item.Price:N2} = {posSum:N2} ₽");
                idx++;
            }
            
            sb.AppendLine($"\n💰 **Общая стоимость: {order.TotalPrice:N2} ₽**");
            
            return sb.ToString();
        }

        private string GetUnitDisplay(string unit)
        {
            return unit switch
            {
                "m" => "метров",
                "т" => "тонн",
                "t" => "тонн",
                "м" => "метров",
                _ => "шт"
            };
        }

        private async Task SendMessageAsync(string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            _logger.LogInformation("🌐 Отправка запроса на URL: {Url}", url);
            
            var payload = new
            {
                chat_id = _chatId,
                text = message,
                parse_mode = "Markdown"
            };

            var json = JsonSerializer.Serialize(payload);
            _logger.LogInformation("📤 Payload: {Payload}", json);
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            _logger.LogInformation("📥 Получен ответ: StatusCode={StatusCode}", response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("📄 Содержимое ответа: {ResponseContent}", responseContent);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("❌ Ошибка Telegram API: {StatusCode} - {ErrorContent}", response.StatusCode, responseContent);
                throw new Exception($"Ошибка Telegram API: {response.StatusCode} - {responseContent}");
            }
            
            _logger.LogInformation("✅ Сообщение успешно отправлено в Telegram");
        }
    }
}
