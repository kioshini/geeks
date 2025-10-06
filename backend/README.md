# TMK Mini App Backend

Backend-проект на .NET 7 Web API для Telegram Mini App, предназначенный для цифровизации и автоматизации консультаций и заказов трубной продукции.

## 🚀 Возможности

- **Управление продукцией**: CRUD операции для трубной продукции
- **Корзина покупок**: Добавление, обновление и удаление товаров
- **Система заказов**: Создание и управление заказами
- **Синхронизация данных**: Импорт данных из внешних источников
- **RESTful API**: Полноценное REST API с документацией Swagger
- **In-Memory хранилище**: Быстрая работа без необходимости настройки БД

## 🛠 Технологии

- **.NET 7** - Основная платформа
- **ASP.NET Core 7.0** - Web API фреймворк
- **C# 10** - Язык программирования
- **Swagger/OpenAPI** - Документация API
- **Dependency Injection** - Внедрение зависимостей
- **Newtonsoft.Json** - Работа с JSON

## 📁 Структура проекта

```
backend/
├── Controllers/           # API контроллеры
│   ├── ProductsController.cs
│   ├── CartController.cs
│   ├── OrdersController.cs
│   └── DataSyncController.cs
├── Models/               # Модели данных
│   ├── Product.cs
│   ├── Order.cs
│   ├── CartItem.cs
│   ├── User.cs
│   └── DTOs/            # DTO модели
├── Services/            # Бизнес-логика
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── ICartService.cs
│   ├── CartService.cs
│   ├── IOrderService.cs
│   ├── OrderService.cs
│   ├── IDataSyncService.cs
│   └── DataSyncService.cs
├── Data/               # JSON файлы с данными
├── Program.cs          # Точка входа
├── appsettings.json    # Конфигурация
└── TMKMiniApp.csproj   # Файл проекта
```

## 🚀 Запуск проекта

### Предварительные требования

- .NET 7 SDK
- Visual Studio 2022 или VS Code (рекомендуется)

### Установка и запуск

1. **Клонирование репозитория**
   ```bash
   git clone <repository-url>
   cd RealHackaton/backend
   ```

2. **Восстановление зависимостей**
   ```bash
   dotnet restore
   ```

3. **Запуск приложения**
   ```bash
   dotnet run
   ```

4. **Открытие в браузере**
   - API: `https://localhost:7000` или `http://localhost:5000`
   - Swagger UI: `https://localhost:7000` (автоматически откроется)

## 📚 API Endpoints

### Продукты (`/api/products`)
- `GET /api/products` - Получить все товары
- `GET /api/products/{id}` - Получить товар по ID
- `GET /api/products/type/{type}` - Получить товары по типу
- `GET /api/products/search?q={query}` - Поиск товаров
- `POST /api/products` - Создать товар
- `PUT /api/products/{id}` - Обновить товар
- `DELETE /api/products/{id}` - Удалить товар
- `PUT /api/products/{id}/stock` - Обновить остатки
- `GET /api/products/types` - Получить типы товаров

### Корзина (`/api/cart`)
- `GET /api/cart/{userId}` - Получить корзину пользователя
- `POST /api/cart/{userId}/items` - Добавить товар в корзину
- `PUT /api/cart/{userId}/items/{itemId}` - Обновить товар в корзине
- `DELETE /api/cart/{userId}/items/{itemId}` - Удалить товар из корзины
- `DELETE /api/cart/{userId}` - Очистить корзину
- `GET /api/cart/{userId}/items/check?productId={id}` - Проверить наличие товара

### Заказы (`/api/orders`)
- `GET /api/orders` - Получить все заказы
- `GET /api/orders/user/{userId}` - Получить заказы пользователя
- `GET /api/orders/{id}` - Получить заказ по ID
- `POST /api/orders` - Создать заказ
- `PUT /api/orders/{id}/status` - Обновить статус заказа
- `DELETE /api/orders/{id}` - Удалить заказ
- `GET /api/orders/status/{status}` - Получить заказы по статусу

### Синхронизация данных (`/api/datasync`)
- `POST /api/datasync/nomenclature` - Синхронизировать номенклатуру
- `POST /api/datasync/prices` - Синхронизировать цены
- `POST /api/datasync/remnants` - Синхронизировать остатки
- `POST /api/datasync/stocks` - Синхронизировать склады
- `POST /api/datasync/types` - Синхронизировать типы
- `GET /api/datasync/status` - Проверить статус синхронизации

## 🔧 Конфигурация

Настройки приложения находятся в файле `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AppSettings": {
    "AppName": "TMK Mini App",
    "Version": "1.0.0"
  },
  "Telegram": {
    "BotToken": "",
    "WebhookUrl": ""
  }
}
```

## 📊 Модели данных

### Product (Товар)
- ID, название, код, описание
- Тип, материал, размеры
- Цена, остатки, доступность

### Order (Заказ)
- ID пользователя, контактные данные
- Список товаров, общая стоимость
- Статус заказа, заметки

### CartItem (Элемент корзины)
- ID пользователя, ID товара
- Количество, цена

## 🧪 Тестирование

Для тестирования API используйте Swagger UI, который доступен по адресу:
- `https://localhost:7000` (в режиме разработки)

Или используйте любой HTTP-клиент (Postman, curl, etc.)

### Пример запроса

```bash
# Получить все товары
curl -X GET "https://localhost:7000/api/products" -H "accept: application/json"

# Создать заказ
curl -X POST "https://localhost:7000/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 123456789,
    "firstName": "Иван",
    "lastName": "Петров",
    "phone": "+7-999-123-45-67",
    "items": [
      {
        "productId": 1,
        "quantity": 2
      }
    ]
  }'
```

## 🔄 Синхронизация данных

Проект поддерживает синхронизацию данных из внешних источников через JSON файлы:
- `nomenclature.json` - Номенклатура товаров
- `prices.json` - Цены
- `remnants.json` - Остатки
- `stocks.json` - Складские данные
- `types.json` - Типы товаров

## 📝 Логирование

Приложение использует встроенную систему логирования .NET:
- Информационные сообщения о работе сервисов
- Предупреждения о потенциальных проблемах
- Ошибки с детальной информацией для отладки

## 🤝 Вклад в проект

1. Форкните репозиторий
2. Создайте ветку для новой функции
3. Внесите изменения
4. Создайте Pull Request

## 📄 Лицензия

Этот проект создан для хакатона и предназначен для демонстрационных целей.

## 📞 Поддержка

При возникновении вопросов или проблем, создайте Issue в репозитории проекта.
