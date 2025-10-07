# 🚀 Руководство по деплою проекта

## 📋 Подготовка к деплою

### 1. Настройка переменных окружения

Скопируйте `.env.example` в `.env` и настройте переменные:

```bash
cd frontend
cp .env.example .env
```

Отредактируйте `.env`:
```env
# API Configuration
VITE_API_BASE_URL=https://your-api-domain.com

# App Configuration
VITE_APP_NAME=TMK Mini App
VITE_APP_VERSION=1.0.0
```

### 2. Сборка проекта

```bash
cd frontend
npm run build
```

После сборки в папке `dist/` будет готовый для деплоя проект.

## 🌐 Деплой на различные платформы

### Vercel

1. **Подключите репозиторий** к Vercel
2. **Настройте переменные окружения** в Vercel Dashboard:
   - `VITE_API_BASE_URL` = ваш API URL
3. **Деплой автоматический** - Vercel использует `vercel.json`

### Netlify

1. **Подключите репозиторий** к Netlify
2. **Настройте сборку**:
   - Build command: `cd frontend && npm run build`
   - Publish directory: `frontend/dist`
3. **Настройте переменные окружения** в Netlify Dashboard
4. **Деплой автоматический** - Netlify использует `_redirects`

### GitHub Pages

1. **Соберите проект**:
   ```bash
   cd frontend
   npm run build
   ```
2. **Загрузите содержимое папки `dist/`** в GitHub Pages
3. **Настройте переменные окружения** в GitHub Secrets

### Apache/Nginx хостинг

1. **Соберите проект**:
   ```bash
   cd frontend
   npm run build
   ```
2. **Загрузите содержимое папки `dist/`** на сервер
3. **Настройте веб-сервер** для SPA (используйте `.htaccess` для Apache)

### Docker

1. **Соберите Docker образ**:
   ```bash
   cd frontend
   docker build -f Dockerfile.production -t tmk-frontend .
   ```
2. **Запустите контейнер**:
   ```bash
   docker run -p 80:80 tmk-frontend
   ```

## 🔧 Настройка API

### Backend должен быть доступен по HTTPS

Убедитесь, что ваш API сервер:
- Работает по HTTPS
- Имеет настроенный CORS для вашего домена
- Доступен по публичному URL

### Пример настройки CORS для backend

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://your-frontend-domain.com",
            "https://your-vercel-app.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
```

## ✅ Проверка деплоя

После деплоя проверьте:

1. **Главная страница** загружается
2. **Маршруты работают** (`/cart`, `/checkout`)
3. **Обновление страницы** не дает 404
4. **API запросы** работают
5. **Переменные окружения** загружены

## 🐛 Решение проблем

### 404 при обновлении страницы

- **Vercel**: Проверьте `vercel.json`
- **Netlify**: Проверьте `_redirects`
- **Apache**: Проверьте `.htaccess`
- **Nginx**: Проверьте конфигурацию

### API не работает

- Проверьте `VITE_API_BASE_URL`
- Проверьте CORS настройки backend
- Проверьте доступность API по HTTPS

### Статические файлы не загружаются

- Проверьте `base: './'` в `vite.config.ts`
- Проверьте настройки веб-сервера
- Проверьте пути к файлам

## 📝 Готовые файлы для деплоя

- `vercel.json` - для Vercel
- `_redirects` - для Netlify  
- `.htaccess` - для Apache
- `nginx.conf` - для Nginx
- `Dockerfile.production` - для Docker
