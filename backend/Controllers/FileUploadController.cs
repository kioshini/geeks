using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TMKMiniApp.Controllers
{
    /// <summary>
    /// Контроллер для загрузки файлов обновлений в папку updates
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly string _updatesPath;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private const string ApiKey = "TMK-UPDATE-2024"; // Простой API ключ для демонстрации

        public FileUploadController(ILogger<FileUploadController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
            _updatesPath = Path.Combine(_environment.ContentRootPath, "..", "updates");
            
            // Создаем папку updates если её нет
            Directory.CreateDirectory(_updatesPath);
        }

        /// <summary>
        /// Загружает JSON файл обновления в папку updates
        /// </summary>
        /// <param name="file">Загружаемый файл</param>
        /// <param name="apiKey">API ключ для авторизации</param>
        /// <returns>Результат загрузки файла</returns>
        [HttpPost("upload-update-file")]
        public async Task<ActionResult<FileUploadResponse>> UploadUpdateFile(
            [FromForm] IFormFile file,
            [FromHeader(Name = "X-API-Key")] string? apiKey)
        {
            try
            {
                // Проверка авторизации
                if (string.IsNullOrEmpty(apiKey) || apiKey != ApiKey)
                {
                    _logger.LogWarning("Попытка загрузки файла без авторизации или с неверным API ключом");
                    return Unauthorized(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Неверный или отсутствующий API ключ"
                    });
                }

                // Проверка наличия файла
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Попытка загрузки пустого файла");
                    return BadRequest(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Файл не выбран или пуст"
                    });
                }

                // Проверка размера файла
                if (file.Length > MaxFileSize)
                {
                    _logger.LogWarning("Попытка загрузки файла размером {FileSize} байт, превышающим лимит {MaxSize} байт", 
                        file.Length, MaxFileSize);
                    return BadRequest(new FileUploadResponse
                    {
                        Success = false,
                        Message = $"Размер файла превышает максимально допустимый ({MaxFileSize / 1024 / 1024}MB)"
                    });
                }

                // Проверка расширения файла
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (fileExtension != ".json")
                {
                    _logger.LogWarning("Попытка загрузки файла с недопустимым расширением: {Extension}", fileExtension);
                    return BadRequest(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Разрешены только JSON файлы"
                    });
                }

                // Проверка имени файла на соответствие паттерну
                var fileName = file.FileName;
                if (!IsValidUpdateFileName(fileName))
                {
                    _logger.LogWarning("Попытка загрузки файла с недопустимым именем: {FileName}", fileName);
                    return BadRequest(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Имя файла должно соответствовать паттерну: prices__HH__MM__.json или remnants__HH__MM__.json"
                    });
                }

                // Проверка содержимого файла (должен быть валидный JSON)
                if (!await IsValidJsonFileAsync(file))
                {
                    _logger.LogWarning("Попытка загрузки файла с невалидным JSON содержимым: {FileName}", fileName);
                    return BadRequest(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Файл содержит невалидный JSON"
                    });
                }

                // Генерация уникального имени файла с timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var uniqueFileName = $"{timestamp}_{fileName}";
                var filePath = Path.Combine(_updatesPath, uniqueFileName);

                // Проверка на существование файла с таким же именем
                if (System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("Файл с именем {FileName} уже существует", uniqueFileName);
                    return Conflict(new FileUploadResponse
                    {
                        Success = false,
                        Message = $"Файл с именем {uniqueFileName} уже существует"
                    });
                }

                // Сохранение файла
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Логирование успешной загрузки
                _logger.LogInformation("Файл {FileName} успешно загружен в папку updates. Размер: {FileSize} байт, Время: {UploadTime}", 
                    uniqueFileName, file.Length, DateTime.Now);

                return Ok(new FileUploadResponse
                {
                    Success = true,
                    Message = "Файл успешно загружен",
                    FileName = uniqueFileName,
                    FileSize = file.Length,
                    UploadTime = DateTime.Now,
                    FilePath = filePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файла {FileName}", file?.FileName);
                return StatusCode(500, new FileUploadResponse
                {
                    Success = false,
                    Message = "Внутренняя ошибка сервера при загрузке файла"
                });
            }
        }

        /// <summary>
        /// Получает список загруженных файлов в папке updates
        /// </summary>
        /// <param name="apiKey">API ключ для авторизации</param>
        /// <returns>Список файлов</returns>
        [HttpGet("list-files")]
        public ActionResult<FileListResponse> ListFiles([FromHeader(Name = "X-API-Key")] string? apiKey)
        {
            try
            {
                // Проверка авторизации
                if (string.IsNullOrEmpty(apiKey) || apiKey != ApiKey)
                {
                    _logger.LogWarning("Попытка получения списка файлов без авторизации или с неверным API ключом");
                    return Unauthorized(new FileListResponse
                    {
                        Success = false,
                        Message = "Неверный или отсутствующий API ключ"
                    });
                }

                var files = Directory.GetFiles(_updatesPath, "*.json")
                    .Select(filePath => new FileInfo(filePath))
                    .Select(fileInfo => new FileInfoDto
                    {
                        FileName = fileInfo.Name,
                        FileSize = fileInfo.Length,
                        CreatedTime = fileInfo.CreationTime,
                        ModifiedTime = fileInfo.LastWriteTime
                    })
                    .OrderByDescending(f => f.CreatedTime)
                    .ToList();

                _logger.LogInformation("Получен список файлов в папке updates. Количество: {Count}", files.Count);

                return Ok(new FileListResponse
                {
                    Success = true,
                    Message = "Список файлов получен успешно",
                    Files = files,
                    TotalCount = files.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка файлов");
                return StatusCode(500, new FileListResponse
                {
                    Success = false,
                    Message = "Внутренняя ошибка сервера при получении списка файлов"
                });
            }
        }

        /// <summary>
        /// Удаляет файл из папки updates
        /// </summary>
        /// <param name="fileName">Имя файла для удаления</param>
        /// <param name="apiKey">API ключ для авторизации</param>
        /// <returns>Результат удаления файла</returns>
        [HttpDelete("delete-file/{fileName}")]
        public ActionResult<FileUploadResponse> DeleteFile(string fileName, [FromHeader(Name = "X-API-Key")] string? apiKey)
        {
            try
            {
                // Проверка авторизации
                if (string.IsNullOrEmpty(apiKey) || apiKey != ApiKey)
                {
                    _logger.LogWarning("Попытка удаления файла без авторизации или с неверным API ключом");
                    return Unauthorized(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Неверный или отсутствующий API ключ"
                    });
                }

                // Проверка безопасности имени файла
                if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    _logger.LogWarning("Попытка удаления файла с небезопасным именем: {FileName}", fileName);
                    return BadRequest(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Недопустимое имя файла"
                    });
                }

                var filePath = Path.Combine(_updatesPath, fileName);
                
                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("Попытка удаления несуществующего файла: {FileName}", fileName);
                    return NotFound(new FileUploadResponse
                    {
                        Success = false,
                        Message = "Файл не найден"
                    });
                }

                System.IO.File.Delete(filePath);
                
                _logger.LogInformation("Файл {FileName} успешно удален", fileName);

                return Ok(new FileUploadResponse
                {
                    Success = true,
                    Message = "Файл успешно удален",
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла {FileName}", fileName);
                return StatusCode(500, new FileUploadResponse
                {
                    Success = false,
                    Message = "Внутренняя ошибка сервера при удалении файла"
                });
            }
        }

        /// <summary>
        /// Проверяет, соответствует ли имя файла паттерну для файлов обновлений
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>True, если имя файла соответствует паттерну</returns>
        private static bool IsValidUpdateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // Проверяем паттерн: prices__HH__MM__.json или remnants__HH__MM__.json
            var pricesPattern = @"^prices__\d{2}__\d{2}__\.json$";
            var remnantsPattern = @"^remnants__\d{2}__\d{2}__\.json$";
            
            return System.Text.RegularExpressions.Regex.IsMatch(fileName, pricesPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                   System.Text.RegularExpressions.Regex.IsMatch(fileName, remnantsPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Проверяет, является ли содержимое файла валидным JSON
        /// </summary>
        /// <param name="file">Файл для проверки</param>
        /// <returns>True, если файл содержит валидный JSON</returns>
        private static async Task<bool> IsValidJsonFileAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();
                
                // Пытаемся десериализовать JSON
                JsonDocument.Parse(content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Ответ на загрузку файла
    /// </summary>
    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public DateTime? UploadTime { get; set; }
        public string? FilePath { get; set; }
    }

    /// <summary>
    /// Ответ со списком файлов
    /// </summary>
    public class FileListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<FileInfoDto> Files { get; set; } = new();
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Информация о файле
    /// </summary>
    public class FileInfoDto
    {
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}
