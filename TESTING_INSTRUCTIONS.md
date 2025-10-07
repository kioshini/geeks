# Инструкции для тестирования проекта

## 🚀 Запуск проекта

### 1. Убедитесь, что Docker установлен и запущен
```bash
sudo systemctl status docker
```

### 2. Запустите проект
```bash
cd /home/kioshi/apps/RealHackaton
sudo docker compose up --build -d
```

### 3. Проверьте статус контейнеров
```bash
sudo docker compose ps
```

## 🔧 Настройка переменных окружения

### 1. Отредактируйте .env файл
```bash
nano .env
```

### 2. Установите ваши значения
```env
TELEGRAM_BOT_TOKEN=ваш_токен_бота
TELEGRAM_CHAT_ID=ваш_chat_id
```

### 3. Получите chat_id для Telegram
1. Найдите вашего бота в Telegram
2. Отправьте боту любое сообщение
3. Выполните запрос:
```bash
curl "https://api.telegram.org/bot<ВАШ_ТОКЕН>/getUpdates"
```
4. Найдите `"chat":{"id": НУЖНЫЙ_ID}` в ответе
5. Добавьте этот ID в .env файл

## 🧪 Тестирование функционала

### 1. Откройте приложение
- Frontend: http://localhost
- Backend API: http://localhost:5000

### 2. Проверьте основные функции
- [ ] Загрузка каталога товаров
- [ ] Переключение между метрами и тоннами
- [ ] Добавление товаров в корзину
- [ ] Удаление товаров из корзины
- [ ] Обновление количества товаров
- [ ] Оформление заказа
- [ ] Отправка заказа в Telegram

### 3. Проверьте API endpoints
```bash
# Проверка здоровья
curl http://localhost:5000/api/health

# Получение товаров
curl http://localhost:5000/api/products

# Получение корзины
curl http://localhost:5000/api/cart/1

# Получение chat_id для Telegram
curl http://localhost:5000/api/telegram/get-chat-id
```

## 🐛 Отладка

### 1. Просмотр логов
```bash
# Все сервисы
sudo docker compose logs

# Только backend
sudo docker compose logs backend

# Только frontend
sudo docker compose logs frontend

# Следить за логами в реальном времени
sudo docker compose logs -f
```

### 2. Проверка статуса контейнеров
```bash
sudo docker compose ps
```

### 3. Перезапуск сервисов
```bash
# Перезапуск всех сервисов
sudo docker compose restart

# Перезапуск только backend
sudo docker compose restart backend

# Перезапуск только frontend
sudo docker compose restart frontend
```

## 🔄 Остановка проекта

```bash
sudo docker compose down
```

## 📝 Известные проблемы и решения

### 1. Ошибка "permission denied" при запуске Docker
```bash
sudo usermod -aG docker $USER
newgrp docker
```

### 2. Порт 5000 занят
```bash
sudo lsof -i :5000
sudo kill -9 <PID>
```

### 3. Порт 80 занят
```bash
sudo lsof -i :80
sudo kill -9 <PID>
```

### 4. Ошибки компиляции
```bash
# Очистка и пересборка
sudo docker compose down
sudo docker system prune -f
sudo docker compose up --build -d
```

## ✅ Чек-лист для проверки

- [ ] Проект запускается без ошибок
- [ ] Frontend доступен по http://localhost
- [ ] Backend API доступен по http://localhost:5000
- [ ] Каталог товаров загружается
- [ ] Корзина работает корректно
- [ ] Заказы создаются успешно
- [ ] Telegram уведомления отправляются
- [ ] Все API endpoints отвечают
- [ ] Логи не содержат критических ошибок

## 🚀 Готово к продакшену

После успешного тестирования проект готов к загрузке на Git!
