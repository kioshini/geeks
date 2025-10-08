# 🚀 Развертывание Telegram Mini App

## 📋 Обзор

Этот проект представляет собой Telegram Mini App для каталога товаров с корзиной и оформлением заказов. Приложение использует React + Vite для frontend и .NET для backend.

## 🛠️ Локальная разработка

### Предварительные требования

- Node.js 18+ 
- .NET 7.0+
- Git

### Запуск проекта

1. **Клонируйте репозиторий:**
```bash
git clone https://github.com/kioshini/geeks.git
cd geeks
```

2. **Установите зависимости frontend:**
```bash
cd frontend
npm install
```

3. **Запустите frontend в режиме разработки:**
```bash
npm run dev
```

4. **Запустите backend (в отдельном терминале):**
```bash
cd backend
dotnet run
```

5. **Откройте приложение:**
- Frontend: http://localhost:5173
- Backend API: http://localhost:5000

## 🌐 Развертывание в production

### Вариант 1: Cloudflare Pages (Рекомендуется)

1. **Подготовьте проект:**
```bash
cd frontend
npm run build
```

2. **Загрузите на Cloudflare Pages:**
   - Зайдите на https://pages.cloudflare.com
   - Создайте новый проект
   - Загрузите папку `dist` или подключите GitHub репозиторий
   - Установите Build command: `npm run build`
   - Установите Build output directory: `dist`

3. **Настройте переменные окружения:**
   - `VITE_API_BASE_URL`: URL вашего backend API

4. **Обновите `_redirects` файл:**
   - Замените `your-backend-url.com` на реальный URL вашего backend

### Вариант 2: Vercel

1. **Подготовьте проект:**
```bash
cd frontend
npm run build
```

2. **Разверните на Vercel:**
   - Установите Vercel CLI: `npm i -g vercel`
   - Выполните: `vercel --prod`
   - Следуйте инструкциям

3. **Настройте переменные окружения:**
   - `VITE_API_BASE_URL`: URL вашего backend API

### Вариант 3: Render

1. **Создайте Static Site на Render:**
   - Подключите GitHub репозиторий
   - Установите Build command: `cd frontend && npm run build`
   - Установите Publish directory: `frontend/dist`

2. **Настройте переменные окружения:**
   - `VITE_API_BASE_URL`: URL вашего backend API

## 🔗 Получение HTTPS URL

### Cloudflare Tunnel (Рекомендуется)

1. **Установите cloudflared:**
```bash
# Linux
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
sudo dpkg -i cloudflared-linux-amd64.deb

# macOS
brew install cloudflared

# Windows
# Скачайте с https://github.com/cloudflare/cloudflared/releases
```

2. **Создайте туннель для frontend:**
```bash
cloudflared tunnel --url http://localhost:5173
```

3. **Создайте туннель для backend:**
```bash
cloudflared tunnel --url http://localhost:5000
```

4. **Получите HTTPS URLs:**
   - Frontend: `https://xxxxx.trycloudflare.com`
   - Backend: `https://yyyyy.trycloudflare.com`

### Альтернативные туннели

**localtunnel:**
```bash
npx localtunnel --port 5173
npx localtunnel --port 5000
```

**serveo:**
```bash
ssh -R 80:localhost:5173 serveo.net
ssh -R 80:localhost:5000 serveo.net
```

## 🤖 Настройка Telegram Bot

### 1. Создание бота

1. **Найдите @BotFather в Telegram**
2. **Создайте нового бота:**
   - Отправьте `/newbot`
   - Введите имя бота
   - Введите username бота
   - Сохраните токен бота

### 2. Настройка Mini App

1. **Установите домен для Mini App:**
```
/setdomain
@your_bot_username
https://your-frontend-url.com
```

2. **Создайте команды бота:**
```
/setcommands
@your_bot_username
start - Запустить приложение
help - Помощь
```

3. **Настройте меню бота:**
```
/setmenubutton
@your_bot_username
Открыть каталог
https://your-frontend-url.com
```

### 3. Настройка backend

1. **Добавьте токен бота в `appsettings.json`:**
```json
{
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN",
    "ChatId": "YOUR_CHAT_ID"
  }
}
```

2. **Получите Chat ID:**
   - Отправьте сообщение боту
   - Выполните: `GET https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates`
   - Найдите `chat.id` в ответе

## 🧪 Тестирование Mini App

### 1. Локальное тестирование

1. **Запустите туннели:**
```bash
# Frontend
cloudflared tunnel --url http://localhost:5173

# Backend  
cloudflared tunnel --url http://localhost:5000
```

2. **Обновите переменные окружения:**
```bash
# В frontend/.env
VITE_API_BASE_URL=https://your-backend-tunnel-url.com
```

3. **Пересоберите frontend:**
```bash
cd frontend
npm run build
```

4. **Обновите домен в BotFather:**
```
/setdomain
@your_bot_username
https://your-frontend-tunnel-url.com
```

### 2. Тестирование в Telegram

1. **Найдите вашего бота в Telegram**
2. **Отправьте `/start`**
3. **Нажмите на кнопку меню или отправьте команду**
4. **Проверьте функциональность:**
   - Просмотр каталога
   - Добавление в корзину
   - Оформление заказа
   - Telegram кнопки (MainButton, BackButton)
   - Тактильная обратная связь

## 📱 Особенности Telegram Mini App

### Реализованные функции

- ✅ **Telegram WebApp SDK** - полная интеграция
- ✅ **MainButton** - кнопка оформления заказа
- ✅ **BackButton** - навигация назад
- ✅ **Haptic Feedback** - тактильная обратная связь
- ✅ **User Data** - получение данных пользователя
- ✅ **Theme Support** - поддержка темной/светлой темы
- ✅ **Responsive Design** - адаптивный дизайн

### Telegram-специфичные компоненты

- **AppLayout** - основной макет с Telegram интеграцией
- **Cart** - корзина с MainButton
- **Checkout** - оформление заказа с BackButton
- **Telegram SDK** - обертка для WebApp API

## 🔧 Настройка переменных окружения

### Frontend (.env)
```env
VITE_API_BASE_URL=https://your-backend-url.com
```

### Backend (appsettings.json)
```json
{
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN",
    "ChatId": "YOUR_CHAT_ID"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://your-frontend-url.com",
      "https://your-frontend-url.vercel.app",
      "https://your-frontend-url.pages.dev"
    ]
  }
}
```

## 🚨 Устранение неполадок

### Проблема: Mini App не загружается
- Проверьте HTTPS URL
- Убедитесь, что домен настроен в BotFather
- Проверьте CORS настройки backend

### Проблема: API не работает
- Проверьте переменную `VITE_API_BASE_URL`
- Убедитесь, что backend доступен по HTTPS
- Проверьте настройки прокси в `_redirects` или `vercel.json`

### Проблема: Telegram кнопки не работают
- Убедитесь, что приложение запущено в Telegram
- Проверьте, что Telegram WebApp SDK загружен
- Проверьте консоль браузера на ошибки

## 📚 Полезные ссылки

- [Telegram Mini Apps Documentation](https://core.telegram.org/bots/webapps)
- [Telegram WebApp SDK](https://core.telegram.org/bots/webapps#initializing-web-apps)
- [Cloudflare Pages](https://pages.cloudflare.com)
- [Vercel](https://vercel.com)
- [Render](https://render.com)

## 🎯 Следующие шаги

1. **Разверните frontend** на выбранной платформе
2. **Разверните backend** на сервере или в облаке
3. **Настройте домен** в BotFather
4. **Протестируйте** Mini App в Telegram
5. **Настройте мониторинг** и логирование

---

**Готово!** 🎉 Ваш Telegram Mini App готов к использованию!
