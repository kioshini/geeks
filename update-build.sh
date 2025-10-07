#!/bin/bash

# Скрипт для обновления готовой сборки
echo "🚀 Обновление production сборки..."

# Переходим в папку frontend
cd frontend

# Устанавливаем зависимости
echo "📦 Установка зависимостей..."
npm ci

# Собираем проект
echo "🔨 Сборка проекта..."
npm run build

# Переходим в корень проекта
cd ..

# Очищаем старую сборку
echo "🧹 Очистка старой сборки..."
rm -rf build/*

# Копируем новую сборку
echo "📋 Копирование новой сборки..."
cp -r frontend/dist/* build/

# Добавляем в Git
echo "📝 Добавление в Git..."
git add build/

# Создаем коммит
echo "💾 Создание коммита..."
git commit -m "chore: обновлена production сборка $(date '+%Y-%m-%d %H:%M:%S')"

# Отправляем на GitHub
echo "☁️ Отправка на GitHub..."
git push origin main

echo "✅ Сборка обновлена и загружена на Git!"
echo "📁 Готовая сборка находится в папке build/"
