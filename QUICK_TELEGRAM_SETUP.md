# ⚡ Быстрый запуск Telegram Mini App

## 🎯 Цель
За 5 минут получить работающий Telegram Mini App с HTTPS URL.

## 🚀 Пошаговая инструкция

### 1. Запуск проекта (2 минуты)

```bash
# Backend (терминал 1)
cd backend
dotnet run

# Frontend (терминал 2)  
cd frontend
npm install
npm run dev
```

### 2. Получение HTTPS URL (1 минута)

```bash
# Терминал 3 - Frontend туннель
cloudflared tunnel --url http://localhost:5173

# Терминал 4 - Backend туннель  
cloudflared tunnel --url http://localhost:5000
```

**Скопируйте полученные URLs:**
- Frontend: `https://xxxxx.trycloudflare.com`
- Backend: `https://yyyyy.trycloudflare.com`

### 3. Настройка переменных (1 минута)

```bash
# Создайте файл frontend/.env
echo "VITE_API_BASE_URL=https://yyyyy.trycloudflare.com" > frontend/.env

# Пересоберите frontend
cd frontend
npm run build
```

### 4. Настройка Telegram Bot (1 минута)

1. **Найдите @BotFather в Telegram**
2. **Создайте бота:**
   ```
   /newbot
   TMK Mini App
   tmk_mini_app_bot
   ```
3. **Настройте домен:**
   ```
   /setdomain
   @tmk_mini_app_bot
   https://xxxxx.trycloudflare.com
   ```

### 5. Тестирование

1. **Найдите вашего бота в Telegram**
2. **Отправьте `/start`**
3. **Нажмите на кнопку меню**
4. **Проверьте функциональность:**
   - ✅ Каталог загружается
   - ✅ Товары отображаются
   - ✅ Корзина работает
   - ✅ Telegram кнопки работают
   - ✅ Оформление заказа работает

## 🎉 Готово!

Ваш Telegram Mini App работает! 

## 🔧 Устранение проблем

**Проблема: "Сайт пустой"**
- Проверьте, что backend туннель работает
- Убедитесь, что `VITE_API_BASE_URL` правильный
- Пересоберите frontend: `npm run build`

**Проблема: "Telegram кнопки не работают"**
- Убедитесь, что приложение открыто в Telegram
- Проверьте, что домен настроен в BotFather

**Проблема: "API не работает"**
- Проверьте, что backend туннель активен
- Убедитесь, что URL в `.env` правильный

## 📱 Что дальше?

1. **Настройте постоянный домен** (Cloudflare Pages, Vercel, Render)
2. **Добавьте бота в группу** для тестирования
3. **Настройте уведомления** в backend
4. **Добавьте аналитику** и мониторинг

---

**Время выполнения:** ~5 минут  
**Сложность:** ⭐⭐☆☆☆  
**Результат:** Работающий Telegram Mini App с HTTPS
