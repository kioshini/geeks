# 🚀 TMK Telegram Mini App

## 📖 Описание проекта

**Telegram Mini App** для автоматизации заказов трубной продукции с полной интеграцией Telegram WebApp SDK.  
Разработано для хакатона РадиоХак 2.0 (ИРИТ-РТФ, ТМК, 2025).  
Цель — цифровизация процесса поиска, выбора и оформления заказов на трубную продукцию через Telegram.

## 🤖 Telegram Mini App Features

- ✅ **Telegram WebApp SDK** - полная интеграция с Telegram
- ✅ **MainButton** - кнопка оформления заказа в Telegram
- ✅ **BackButton** - навигация назад в Telegram
- ✅ **Haptic Feedback** - тактильная обратная связь
- ✅ **User Data** - автоматическое получение данных пользователя
- ✅ **Theme Support** - поддержка темной/светлой темы Telegram
- ✅ **Responsive Design** - адаптивный дизайн для мобильных устройств

## ⚙️ Функциональные возможности

- **Фильтрация продукции:** поиск по складу, виду продукции, диаметру, толщине стенки, ГОСТу и марке стали
- **Корзина:** добавление продукции с выбором единицы измерения (метры / тонны)
- **Динамические скидки:** автоматический перерасчёт цены в зависимости от объёма заказа
- **Актуальные цены:** обновление в течение дня через JSON-дельты из папки `updates`
- **Оформление заказа:** ввод имени, фамилии, ИНН, телефона, email
- **Автоматизированное обновление данных:** мониторинг папки `updates` и применение дельт в реальном времени
- **API для загрузки файлов:** безопасная загрузка JSON-файлов с валидацией

## 🧩 Архитектура проекта

### Backend (C# .NET 7.0)
- **API Controllers:**
  - `ProductsController` - управление товарами
  - `CartController` - управление корзиной
  - `OrdersController` - оформление заказов
  - `JsonDataController` - работа с JSON данными
  - `FileUploadController` - загрузка файлов обновлений
  - `SyncController` - управление синхронизацией данных
  - `HealthController` - мониторинг состояния

- **Services:**
  - `ProductService` - бизнес-логика товаров
  - `CartService` - управление корзиной
  - `OrderService` - обработка заказов
  - `DiscountService` - расчёт скидок
  - `JsonDataService` - работа с JSON файлами
  - `DynamicDeltaUpdatesService` - обработка дельт в реальном времени
  - `AutomatedDataSyncService` - автоматическая синхронизация

- **Data Models:**
  - JSON модели для nomenclature, prices, remnants, stocks, types
  - DTO для API взаимодействия
  - Модели заказов и корзины

### Frontend (React + TypeScript + TailwindCSS)
- **Pages:**
  - `Catalog` - каталог товаров с фильтрацией
  - `Cart` - корзина покупок
  - `Checkout` - оформление заказа

- **Components:**
  - `ProductCard` - карточка товара
  - `ProductModal` - детальная информация о товаре
  - `Catalog` - компонент каталога с фильтрами
  - `CartIcon` - иконка корзины

- **Services:**
  - `api.ts` - API клиент
  - `deltaUpdatesService.ts` - сервис обновлений
  - `jsonDataAdapter.ts` - адаптер данных

### Data
- **JSON файлы:** nomenclature, prices, remnants, stocks, types в папке `/backend/Data`
- **Дельты:** файлы обновлений в папке `/updates` (формат: `prices__HH__MM__.json`, `remnants__HH__MM__.json`)

## 🐳 Docker-контейнеризация

Проект готов к запуску в Docker с помощью `docker-compose`.

### Backend Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["backend/TMKMiniApp.csproj", "backend/"]
RUN dotnet restore "backend/TMKMiniApp.csproj"
COPY . .
WORKDIR "/src/backend"
RUN dotnet build "TMKMiniApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TMKMiniApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TMKMiniApp.dll"]
```

### Frontend Dockerfile
```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY frontend/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
```

### docker-compose.yml
```yaml
version: '3.8'
services:
  backend:
    build:
      context: .
      dockerfile: backend/Dockerfile
    ports:
      - "5000:5000"
    volumes:
      - ./backend/Data:/app/Data
      - ./updates:/app/updates
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000

  frontend:
    build:
      context: .
      dockerfile: frontend/Dockerfile
    ports:
      - "80:80"
    depends_on:
      - backend
    environment:
      - VITE_API_BASE_URL=http://localhost:5000
```

## 🚀 Быстрый старт

### Локальная разработка

1. **Backend:**
```bash
cd backend
dotnet restore
dotnet run --urls="http://localhost:5000"
```

2. **Frontend:**
```bash
cd frontend
npm install
npm run dev
```

3. **Доступ:**
- Frontend: http://localhost:5173
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

### Docker

```bash
docker-compose up --build
```

## 🌐 Развертывание как Telegram Mini App

### 1. Получение HTTPS URL

**Cloudflare Tunnel (Рекомендуется):**
```bash
# Установите cloudflared
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
sudo dpkg -i cloudflared-linux-amd64.deb

# Создайте туннели
cloudflared tunnel --url http://localhost:5173  # Frontend
cloudflared tunnel --url http://localhost:5000  # Backend
```

**Альтернативные туннели:**
```bash
# localtunnel
npx localtunnel --port 5173
npx localtunnel --port 5000

# serveo
ssh -R 80:localhost:5173 serveo.net
ssh -R 80:localhost:5000 serveo.net
```

### 2. Настройка Telegram Bot

1. **Создайте бота через @BotFather:**
   - Отправьте `/newbot`
   - Введите имя и username бота
   - Сохраните токен

2. **Настройте Mini App:**
```
/setdomain
@your_bot_username
https://your-frontend-url.com
```

3. **Создайте команды:**
```
/setcommands
@your_bot_username
start - Запустить приложение
help - Помощь
```

### 3. Настройка переменных окружения

**Frontend (.env):**
```env
VITE_API_BASE_URL=https://your-backend-url.com
```

**Backend (appsettings.json):**
```json
{
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN",
    "ChatId": "YOUR_CHAT_ID"
  }
}
```

### 4. Сборка для production

```bash
cd frontend
npm run build
```

### 5. Развертывание

**Cloudflare Pages (Рекомендуется):**
- Подключите GitHub репозиторий к Cloudflare Pages
- Build command: `npm run build`
- Build output directory: `frontend/dist`
- Настройте переменные окружения

**Vercel:**
```bash
npm i -g vercel
vercel --prod
```

**Render:**
- Создайте Static Site
- Подключите GitHub репозиторий
- Build command: `cd frontend && npm run build`
- Publish directory: `frontend/dist`

## 📱 Тестирование Mini App

1. **Найдите вашего бота в Telegram**
2. **Отправьте `/start`**
3. **Нажмите на кнопку меню или отправьте команду**
4. **Проверьте функциональность:**
   - Просмотр каталога
   - Добавление в корзину
   - Оформление заказа
   - Telegram кнопки (MainButton, BackButton)
   - Тактильная обратная связь

## 📚 Подробная документация

- **Развертывание:** [TELEGRAM_MINIAPP_DEPLOYMENT.md](./TELEGRAM_MINIAPP_DEPLOYMENT.md)
- **Cloudflare Pages:** [CLOUDFLARE_PAGES_DEPLOY.md](./CLOUDFLARE_PAGES_DEPLOY.md)
- **Быстрый старт:** [QUICK_TELEGRAM_SETUP.md](./QUICK_TELEGRAM_SETUP.md)
- **Использование:** [TELEGRAM_MINIAPP_USAGE.md](./TELEGRAM_MINIAPP_USAGE.md)

## 📊 API Endpoints

### Основные
- `GET /api/health` - проверка состояния
- `GET /api/products` - список товаров
- `GET /api/jsondata/*` - данные из JSON файлов

### Корзина
- `GET /api/cart/{userId}` - получить корзину
- `POST /api/cart/{userId}/items` - добавить товар
- `PUT /api/cart/{userId}/items/{itemId}` - обновить товар
- `PUT /api/cart/{userId}/items/product/{productId}` - обновить по ID товара
- `DELETE /api/cart/{userId}/items/{itemId}` - удалить товар
- `DELETE /api/cart/{userId}/items/product/{productId}` - удалить по ID товара
- `POST /api/cart/calculate-discount` - расчёт скидки

### Заказы
- `POST /api/orders` - создать заказ
- `GET /api/orders/{userId}` - получить заказы пользователя

### Синхронизация данных
- `GET /api/sync/status` - статус синхронизации
- `POST /api/sync/trigger-nightly-sync` - запуск полной синхронизации
- `POST /api/sync/process-price-delta` - обработка дельты цен
- `POST /api/sync/process-stock-delta` - обработка дельты остатков

### Загрузка файлов
- `POST /api/fileupload/upload-update-file` - загрузка файла обновления
- `GET /api/fileupload/list-files` - список файлов
- `DELETE /api/fileupload/delete-file/{fileName}` - удаление файла

## 🔧 Технические особенности

### Динамические обновления
- Мониторинг папки `updates` в реальном времени
- Автоматическое применение дельт к ценам и остаткам
- Формула: `new_value = current_value + delta` (минимум 0)
- Архивирование обработанных файлов

### Система скидок
- Двухуровневые скидки по тоннам и метрам
- Автоматический расчёт на основе `PriceLimitT1/T2` и `PriceM1/M2`
- Поддержка единиц измерения (шт, м, т)

### Валидация данных
- Проверка форматов JSON файлов
- Валидация имён файлов (паттерн: `prices__HH__MM__.json`)
- API ключ для загрузки файлов
- Ограничения размера файлов (10MB)

## 📁 Структура проекта

```
RealHackaton/
├── backend/                 # C# .NET Backend
│   ├── Controllers/         # API контроллеры
│   ├── Services/           # Бизнес-логика
│   ├── Models/             # Модели данных
│   ├── Data/               # JSON файлы данных
│   └── Dockerfile
├── frontend/               # React Frontend
│   ├── src/
│   │   ├── components/     # React компоненты
│   │   ├── pages/         # Страницы приложения
│   │   ├── lib/           # Утилиты и сервисы
│   │   └── types/         # TypeScript типы
│   └── Dockerfile
├── updates/                # Папка для дельт
├── docker-compose.yml      # Docker конфигурация
└── README.md              # Документация
```

## 🛡️ Безопасность

- API ключ для загрузки файлов (`TMK-UPDATE-2024`)
- Валидация типов файлов (только JSON)
- Проверка имён файлов по паттерну
- Ограничения размера загружаемых файлов
- Обработка ошибок и логирование

## 📈 Мониторинг и логирование

- Подробное логирование всех операций
- Health check endpoint
- Мониторинг состояния синхронизации
- Отслеживание ошибок и исключений

## 🎯 Результаты хакатона

✅ **Полная реализация функционала:**
- Фильтрация продукции по всем критериям
- Корзина с единицами измерения
- Динамические скидки
- Актуальные цены через дельты
- Оформление заказов с валидацией
- Автоматизированное обновление данных
- API для загрузки файлов

✅ **Техническое качество:**
- Чистая архитектура
- Подробная документация
- Docker-контейнеризация
- Обработка ошибок
- Логирование

✅ **Готовность к продакшену:**
- Безопасность
- Масштабируемость
- Мониторинг
- Документация API

---

**Команда:** РадиоХак 2.0  
**Дата:** 2025  
**Технологии:** C# .NET 7.0, React, TypeScript, TailwindCSS, Docker