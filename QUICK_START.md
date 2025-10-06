# Быстрый запуск TMK Mini App

## 🚀 Запуск через Docker (рекомендуется)

```bash
# Клонирование репозитория
git clone <repository-url>
cd RealHackaton

# Запуск всех сервисов
docker-compose up --build

# Приложение будет доступно на:
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

## 🛠 Локальная разработка

### Backend (.NET 7)
```bash
cd backend
dotnet restore
dotnet run
# API: http://localhost:5000
```

### Frontend (React)
```bash
cd frontend
npm install
npm run dev
# App: http://localhost:5173
```

## 📱 Функции приложения

- **Каталог товаров** - просмотр трубной продукции
- **Фильтрация** - по типу, производителю, марке стали
- **Корзина** - добавление и управление товарами
- **Заказы** - оформление заказов
- **Адаптивный дизайн** - работает на всех устройствах
- **TMK стиль** - корпоративный дизайн

## 🔧 Технологии

- **Backend**: .NET 7, ASP.NET Core, C#
- **Frontend**: React, TypeScript, Tailwind CSS
- **База данных**: In-memory (для демо)
- **Контейнеризация**: Docker + Docker Compose

## 📖 Документация

- Полная документация: `README.md`
- API документация: http://localhost:5000/swagger
- Настройка Git: `GIT_SETUP.md`
