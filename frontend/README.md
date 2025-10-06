# TMK Mini App — Frontend

Современный фронтенд для Telegram Mini App (продажа труб), построенный на React + TypeScript + Vite + Tailwind CSS, с интеграцией Telegram WebApp SDK и работой с JSON API backend (ASP.NET Core).

## Стек
- React 18 + TypeScript
- Vite
- Tailwind CSS (утилитарные стили)
- axios (HTTP)
- react-router-dom (маршрутизация)
- Zustand (глобальный стейт корзины)
- Telegram WebApp SDK (`@twa-dev/sdk`)

## Быстрый старт

Требования:
- Node.js 18+
- Запущенный backend на `http://localhost:5000` (см. папку `backend/`)

Установка и запуск:
```bash
cd frontend
npm install
# (опционально) указать базовый URL backend:
# VITE_API_BASE_URL=http://localhost:5000 \
npm run dev
```
Vite запустит dev-сервер, по умолчанию: `http://localhost:5173` (или ближайший свободный порт).

Production-сборка:
```bash
npm run build
npm run preview
```

## Переменные окружения
Используются переменные `VITE_*`:
- `VITE_API_BASE_URL` — базовый URL API backend (по умолчанию `http://localhost:5000`).
- `VITE_APP_NAME` — отображаемое имя приложения (необязательно).

Примеры:
```bash
# локально одноразово
VITE_API_BASE_URL=http://localhost:5000 npm run dev

# или добавьте в .env/.env.local (не входят в VCS)
VITE_API_BASE_URL=http://localhost:5000
VITE_APP_NAME=TMK Mini App
```

## Структура проекта
```
frontend/
├── index.html
├── postcss.config.cjs
├── tailwind.config.cjs
├── tsconfig.json
├── vite.config.ts
├── src/
│   ├── main.tsx               # Точка входа, маршруты и layout
│   ├── index.css              # Tailwind подключения
│   ├── lib/
│   │   ├── api.ts            # axios-клиент и типы DTO
│   │   └── telegram.ts       # Обертка над Telegram WebApp SDK
│   ├── store/
│   │   └── cart.ts           # Zustand: корзина и экшены
│   ├── shared/
│   │   └── AppLayout.tsx     # Хедер/нижняя навигация, слот контента
│   ├── components/
│   │   ├── ProductCard.tsx   # Карточка товара
│   │   └── CategoryChips.tsx # Чипсы категорий
│   └── pages/
│       ├── Catalog.tsx       # Каталог с фильтрами и поиском
│       ├── Cart.tsx          # Корзина
│       └── Checkout.tsx      # Оформление заказа
```

## Интеграция с Telegram Mini App
Используется пакет `@twa-dev/sdk`. Базовые вызовы реализованы в `src/lib/telegram.ts`:
- `Telegram.ready()` и `Telegram.expand()` — инициализация и разворот окна
- `Telegram.user()` — безопасное извлечение пользователя из `initDataUnsafe`
- `Telegram.haptic()` — тактильная обратная связь (опционально)

Фронтенд ожидает, что Mini App запущен внутри Telegram. Локально, вне Telegram, пользователь может не определиться — страницы обрабатывают это состояние (сообщения «Ожидание Telegram пользователя…»).

## Работа с API
Клиент `axios` и DTO находятся в `src/lib/api.ts`. Используемые endpoints backend:
- Продукты:
  - `GET /api/products` — список
  - `GET /api/products/{id}` — детально (используется в клиентах по необходимости)
  - `GET /api/products/search?q=...` — поиск
  - `GET /api/products/types` — типы (категории)
- Корзина:
  - `GET /api/cart/{userId}` — корзина пользователя
  - `POST /api/cart/{userId}/items` — добавить товар
  - `PUT /api/cart/{userId}/items/{itemId}` — изменить количество
  - `DELETE /api/cart/{userId}/items/{itemId}` — удалить позицию
  - `DELETE /api/cart/{userId}` — очистить корзину
- Заказы:
  - `POST /api/orders` — создать заказ
  - `GET /api/orders/user/{userId}` — заказы пользователя

Базовый URL берется из `import.meta.env.VITE_API_BASE_URL`.

## Навигация и страницы
Маршруты определены в `src/main.tsx` через `react-router-dom`:
- `/` — Каталог (поиск, фильтрация по типам, добавление в корзину)
- `/cart` — Корзина (изменение количества, удаление, очистка)
- `/checkout` — Оформление (форма + отправка заказа на backend)

## Tailwind CSS
Tailwind подключен через PostCSS:
- `postcss.config.cjs` — `@tailwindcss/postcss` + `autoprefixer`
- `tailwind.config.cjs` — пути и кастомные цвета
- `src/index.css` — `@tailwind base; @tailwind components; @tailwind utilities;`

## Разработка
- HMR Vite включен по умолчанию
- Консоль браузера помогает увидеть ошибки API/сетевые проблемы
- Для теста вне Telegram можно временно «захардкодить» userId, однако в приложении userId поступает из Telegram initData

## Деплой
1. Сборка: `npm run build` — выходные файлы в `dist/`
2. Раздача статики через любой CDN/Nginx/статический хостинг
3. Настройте `VITE_API_BASE_URL` для целевой среды (например, публичный URL вашего backend)
4. Для встраивания в Telegram Mini App настройте URL Mini App в BotFather на адрес, где доступна ваша сборка

## Полезные команды
```bash
# запуск дев-сервера
npm run dev

# линт/типизация (если добавите ESLint)
# npm run lint

# сборка и предпросмотр production
npm run build
npm run preview
```

## Лицензия
Проект создан для демонстрационных целей в рамках кейса/хакатона.
