# TMK Mini App - Telegram Mini App для заказа трубной продукции

## Описание проекта

TMK Mini App - это современное веб-приложение для заказа трубной продукции через Telegram Mini App. Проект включает в себя полнофункциональный каталог товаров, корзину покупок, систему заказов и адаптивный дизайн в корпоративном стиле TMK.

## Технологический стек

### Backend
- **.NET 7** - основной фреймворк
- **ASP.NET Core 7.0** - веб-API
- **C# 10** - язык программирования
- **Dependency Injection** - внедрение зависимостей
- **In-memory data storage** - хранение данных в памяти
- **Swagger UI** - документация API
- **Rate Limiting** - ограничение запросов
- **Input Validation** - валидация входных данных

### Frontend
- **React 18** - пользовательский интерфейс
- **TypeScript** - типизированный JavaScript
- **Vite** - сборщик и dev-сервер
- **Tailwind CSS** - утилитарный CSS фреймворк
- **Zustand** - управление состоянием
- **React Router** - маршрутизация
- **Framer Motion** - анимации
- **Lucide React** - иконки
- **Axios** - HTTP клиент

### Дополнительные технологии
- **Telegram WebApp SDK** - интеграция с Telegram
- **Docker** - контейнеризация
- **Nginx** - веб-сервер для фронтенда

## Функциональность

### Основные возможности
- 📱 **Адаптивный дизайн** - работает на всех устройствах
- 🛒 **Каталог товаров** - просмотр и поиск трубной продукции
- 🔍 **Фильтрация** - по типу производства, производителю, марке стали
- 📦 **Корзина покупок** - добавление, изменение количества, удаление товаров
- 💰 **Система заказов** - оформление заказов с контактными данными
- 🎨 **Корпоративный дизайн** - в стиле TMK с фирменными цветами
- 📊 **Единицы измерения** - переключение между тоннами и метрами
- 🔒 **Базовая защита** - rate limiting, валидация данных

### API Endpoints

#### Продукты
- `GET /api/products` - получить все продукты
- `GET /api/products/{id}` - получить продукт по ID
- `GET /api/products/search?q={query}` - поиск продуктов
- `GET /api/products/categories` - получить категории

#### Корзина
- `GET /api/cart/{userId}` - получить корзину пользователя
- `POST /api/cart/{userId}/items` - добавить товар в корзину
- `PUT /api/cart/{userId}/items/product/{productId}` - обновить количество
- `DELETE /api/cart/{userId}/items/product/{productId}` - удалить товар
- `DELETE /api/cart/{userId}` - очистить корзину

#### Заказы
- `POST /api/orders` - создать заказ
- `GET /api/orders/user/{userId}` - получить заказы пользователя

#### Системные
- `GET /api/health` - проверка состояния
- `GET /api/datasync/status` - статус синхронизации данных

## Установка и запуск

### Предварительные требования
- .NET 7 SDK
- Node.js 18+
- npm или yarn

### Локальная разработка

1. **Клонирование репозитория**
```bash
git clone <repository-url>
cd RealHackaton
```

2. **Запуск Backend**
```bash
cd backend
dotnet restore
dotnet run
```
Backend будет доступен на `http://localhost:5000`

3. **Запуск Frontend**
```bash
cd frontend
npm install
npm run dev
```
Frontend будет доступен на `http://localhost:5173`

### Docker (рекомендуется)

1. **Запуск всех сервисов**
```bash
docker-compose up --build
```

2. **Доступ к приложению**
- Frontend: `http://localhost:3000`
- Backend API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

## Структура проекта

```
RealHackaton/
├── backend/                 # .NET 7 Web API
│   ├── Controllers/         # API контроллеры
│   ├── Models/             # Модели данных
│   ├── Services/           # Бизнес-логика
│   ├── Data/              # JSON файлы с данными
│   └── Program.cs         # Точка входа
├── frontend/               # React приложение
│   ├── src/
│   │   ├── components/     # React компоненты
│   │   ├── pages/         # Страницы приложения
│   │   ├── lib/           # Утилиты и API клиент
│   │   ├── store/         # Zustand store
│   │   └── types/         # TypeScript типы
│   └── public/            # Статические файлы
├── docker-compose.yml      # Docker конфигурация
├── Dockerfile.backend     # Dockerfile для backend
├── Dockerfile.frontend    # Dockerfile для frontend
└── README.md              # Документация
```

## Особенности реализации

### Backend
- **In-memory storage** - данные хранятся в памяти для демонстрации
- **Dependency Injection** - все сервисы регистрируются через DI
- **Rate Limiting** - защита от злоупотреблений
- **Input Validation** - валидация всех входящих данных
- **CORS** - настроен для работы с фронтендом
- **Swagger** - автоматическая документация API

### Frontend
- **Responsive Design** - адаптивный дизайн для всех устройств
- **State Management** - Zustand для глобального состояния
- **TypeScript** - полная типизация
- **Modern UI** - современный интерфейс с анимациями
- **Telegram Integration** - интеграция с Telegram WebApp
- **Mobile First** - оптимизация для мобильных устройств

## Цветовая схема TMK

- **Основной цвет**: `#FF6B00` (оранжевый TMK)
- **Тёмный фон**: `#1A1A1A` (угольно-чёрный)
- **Светло-серый**: `#E0E0E0` (границы и фоны)
- **Тёмно-серый**: `#2C2C2C` (текст и элементы)

## Разработка

### Добавление новых функций
1. Создайте модель в `backend/Models/`
2. Добавьте сервис в `backend/Services/`
3. Создайте контроллер в `backend/Controllers/`
4. Обновите API клиент в `frontend/src/lib/api.ts`
5. Создайте компоненты в `frontend/src/components/`

### Тестирование
- Backend: используйте Swagger UI для тестирования API
- Frontend: используйте браузерные инструменты разработчика

## Лицензия

Этот проект создан для хакатона по промышленной цифровизации.

## Контакты

Для вопросов по проекту обращайтесь к команде разработки.