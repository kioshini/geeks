# 🚀 Быстрый старт TMK Mini App

## Локальная разработка

### 1. Backend (C# .NET 7.0)
```bash
cd backend
dotnet restore
dotnet run --urls="http://localhost:5000"
```

### 2. Frontend (React)
```bash
cd frontend
npm install
npm run dev
```

### 3. Доступ к приложению
- **Frontend:** http://localhost:5173
- **Backend API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

## Docker

### Запуск всех сервисов
```bash
docker-compose up --build
```

### Доступ к приложению
- **Frontend:** http://localhost
- **Backend API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

## Тестирование API

### Проверка здоровья
```bash
curl http://localhost:5000/api/health
```

### Получение товаров
```bash
curl http://localhost:5000/api/products
```

### Добавление в корзину
```bash
curl -X POST http://localhost:5000/api/cart/999001/items \
  -H "Content-Type: application/json" \
  -d '{"productId": "1", "quantity": 2, "unit": "шт"}'
```

### Загрузка файла обновления
```bash
curl -X POST http://localhost:5000/api/fileupload/upload-update-file \
  -H "X-API-Key: TMK-UPDATE-2024" \
  -F "file=@prices__12__30__.json"
```

## Структура данных

### JSON файлы (backend/Data/)
- `nomenclature.json` - номенклатура товаров
- `prices.json` - цены
- `remnants.json` - остатки
- `stocks.json` - склады
- `types.json` - типы товаров

### Дельты (updates/)
- `prices__HH__MM__.json` - дельты цен
- `remnants__HH__MM__.json` - дельты остатков

## Основные функции

1. **Фильтрация товаров** - по складу, типу, диаметру, толщине, ГОСТу, марке стали
2. **Корзина** - добавление с единицами измерения (шт, м, т)
3. **Скидки** - автоматический расчёт по объёму
4. **Заказы** - оформление с валидацией данных
5. **Обновления** - автоматическое применение дельт

## Troubleshooting

### Backend не запускается
- Проверьте, что порт 5000 свободен
- Убедитесь, что установлен .NET 7.0 SDK

### Frontend не подключается к API
- Проверьте, что backend запущен на порту 5000
- Убедитесь, что переменная `VITE_API_BASE_URL` настроена правильно

### Docker не собирается
- Убедитесь, что Docker и Docker Compose установлены
- Проверьте, что все файлы на месте