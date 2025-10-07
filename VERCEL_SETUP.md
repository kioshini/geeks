# 🚀 Настройка деплоя на Vercel

## 📋 Пошаговая инструкция

### 1. Подключение репозитория

1. Зайдите на [vercel.com](https://vercel.com)
2. Нажмите "New Project"
3. Выберите "Import Git Repository"
4. Выберите ваш репозиторий `kioshini/geeks`

### 2. Настройка проекта

**Framework Preset:** `Other`
**Root Directory:** `frontend`
**Build Command:** `npm run build`
**Output Directory:** `dist`

### 3. Переменные окружения

В настройках проекта добавьте:

```
VITE_API_BASE_URL = https://your-api-domain.com
```

### 4. Настройки сборки

Vercel автоматически определит настройки из `vercel.json`:

```json
{
  "version": 2,
  "builds": [
    {
      "src": "frontend/package.json",
      "use": "@vercel/static-build",
      "config": {
        "distDir": "dist"
      }
    }
  ],
  "routes": [
    { "src": "/(.*)", "dest": "/index.html" }
  ]
}
```

### 5. Деплой

1. Нажмите "Deploy"
2. Vercel автоматически соберет проект
3. После успешного деплоя получите URL

## ✅ Проверка

После деплоя проверьте:

- [ ] Главная страница загружается
- [ ] Маршруты `/cart` и `/checkout` работают
- [ ] Обновление страницы не дает 404
- [ ] API запросы работают (если настроен)

## 🔧 Решение проблем

### "Failed to locate package.json"

- Убедитесь, что Root Directory установлен в `frontend`
- Проверьте, что `frontend/package.json` существует

### 404 при обновлении страницы

- Проверьте, что `vercel.json` содержит правильные routes
- Убедитесь, что используется `@vercel/static-build`

### API не работает

- Проверьте переменную `VITE_API_BASE_URL`
- Убедитесь, что API доступен по HTTPS
- Проверьте CORS настройки на backend

## 📝 Альтернативный способ

Если автоматическая настройка не работает:

1. **Root Directory:** оставьте пустым
2. **Build Command:** `cd frontend && npm run build`
3. **Output Directory:** `frontend/dist`
4. **Install Command:** `cd frontend && npm install`

## 🌐 Домен

После успешного деплоя:
- Получите URL вида: `https://your-project.vercel.app`
- Можете подключить собственный домен
- Настройте переменные окружения для production
