/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#FF6B00',
        dark: '#1A1A1A',
        grayLight: '#E0E0E0',
        grayDark: '#2C2C2C',
        // Дополнительные оттенки для лучшей градации
        darkLighter: '#2A2A2A',
        grayMedium: '#F5F5F5',
        primaryLight: '#FF8A33',
        primaryDark: '#E55A00',
        // Новые цвета согласно требованиям
        'text-primary': '#171A1F',
        'text-secondary': '#565D6D',
        'bg-gray': '#F3F4F6',
        'border-gray': '#DEE1E6',
        'text-green': '#22C55E',
        'text-orange': '#E64A19',
        'bg-red': '#F44336',
      },
    },
  },
  plugins: [],
}

