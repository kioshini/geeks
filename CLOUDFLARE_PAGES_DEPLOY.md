# 🚀 Деплой на Cloudflare Pages

## 📋 Обзор

Проект настроен для автоматического деплоя на Cloudflare Pages с поддержкой Telegram Mini App.

## ⚙️ Конфигурация

### 1. wrangler.json
```json
{
  "name": "geeks-mini-app",
  "compatibility_date": "2025-10-08",
  "assets": { "directory": "./frontend/dist" }
}
```

### 2. package.json
```json
{
  "scripts": {
    "build": "cd frontend && npm install && npm run build",
    "prebuild": "npm run install:frontend"
  }
}
```

### 3. _redirects (frontend/_redirects)
```
# Cloudflare Pages redirects for Telegram Mini App
/api/* https://your-backend-url.com/api/:splat 200
/* /index.html 200
```

## 🚀 Деплой через Cloudflare Dashboard

### 1. Подготовка репозитория
```bash
# Убедитесь, что все изменения закоммичены
git add .
git commit -m "Add Cloudflare Pages configuration"
git push origin main
```

### 2. Настройка в Cloudflare Pages

1. **Зайдите на https://pages.cloudflare.com**
2. **Создайте новый проект:**
   - Выберите "Connect to Git"
   - Подключите ваш GitHub репозиторий
   - Выберите ветку `main`

3. **Настройте Build Settings:**
   - **Build command:** `npm run build`
   - **Build output directory:** `frontend/dist`
   - **Root directory:** `/` (корень проекта)

4. **Настройте Environment Variables:**
   - `NODE_VERSION`: `18`
   - `VITE_API_BASE_URL`: `https://your-backend-url.com`

### 3. Настройка домена

1. **Получите URL проекта:** `https://your-project.pages.dev`
2. **Настройте кастомный домен (опционально):**
   - В настройках проекта → Custom domains
   - Добавьте ваш домен

## 🔧 Деплой через Wrangler CLI

### 1. Установка Wrangler
```bash
npm install -g wrangler
```

### 2. Авторизация
```bash
wrangler login
```

### 3. Деплой
```bash
# Сборка проекта
npm run build

# Деплой на Cloudflare Pages
wrangler pages deploy frontend/dist --project-name=geeks-mini-app
```

## 🌐 Настройка Telegram Bot

### 1. Обновите домен в BotFather
```
/setdomain
@your_bot_username
https://your-project.pages.dev
```

### 2. Протестируйте Mini App
1. Найдите вашего бота в Telegram
2. Отправьте `/start`
3. Нажмите на кнопку меню
4. Проверьте функциональность

## 🔄 Автоматический деплой

### GitHub Actions (опционально)
Создайте `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Cloudflare Pages

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          
      - name: Install dependencies
        run: npm run install:frontend
        
      - name: Build project
        run: npm run build
        
      - name: Deploy to Cloudflare Pages
        uses: cloudflare/pages-action@v1
        with:
          apiToken: ${{ secrets.CLOUDFLARE_API_TOKEN }}
          accountId: ${{ secrets.CLOUDFLARE_ACCOUNT_ID }}
          projectName: geeks-mini-app
          directory: frontend/dist
```

## 🛠️ Устранение проблем

### Проблема: Build failed
- Проверьте, что Node.js версии 18+
- Убедитесь, что все зависимости установлены
- Проверьте логи сборки в Cloudflare Dashboard

### Проблема: Mini App не загружается
- Проверьте, что домен настроен в BotFather
- Убедитесь, что _redirects файл корректный
- Проверьте CORS настройки backend

### Проблема: API не работает
- Обновите `VITE_API_BASE_URL` в Environment Variables
- Проверьте, что backend доступен по HTTPS
- Убедитесь, что _redirects перенаправляет /api/* на backend

## 📊 Мониторинг

### Cloudflare Analytics
- Зайдите в Cloudflare Dashboard
- Выберите ваш проект
- Перейдите в Analytics для просмотра статистики

### Логи
- В Cloudflare Dashboard → Functions → Logs
- Просматривайте логи в реальном времени

## 🎯 Результат

После успешного деплоя вы получите:

- ✅ **HTTPS URL:** `https://your-project.pages.dev`
- ✅ **Автоматический деплой** при push в main
- ✅ **CDN** от Cloudflare
- ✅ **Готовый Telegram Mini App**

## 📚 Полезные ссылки

- [Cloudflare Pages Documentation](https://developers.cloudflare.com/pages/)
- [Wrangler CLI](https://developers.cloudflare.com/workers/wrangler/)
- [Telegram Mini Apps](https://core.telegram.org/bots/webapps)

---

**Готово!** 🎉 Ваш Telegram Mini App развернут на Cloudflare Pages!
