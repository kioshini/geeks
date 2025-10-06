# Каталог товаров - React компоненты

Набор React компонентов для создания интерактивного каталога товаров с поддержкой переключения режимов отображения, модальных окон и управления корзиной. Специально разработан для отображения трубной продукции ТМК.

## Компоненты

### 1. Catalog
Основной компонент каталога товаров с поддержкой:
- Переключения между режимами "Сетка" (4 товара в ряд) и "Список" (1 товар в ряд)
- Поиска по названию, производителю, марке стали, ГОСТ
- Фильтрации по типу производства, производителю, марке стали
- Управления корзиной с реактивными обновлениями
- Анимаций появления товаров с framer-motion

### 2. ProductCard
Карточка товара с поддержкой:
- Двух режимов отображения (сетка/список)
- Отображения всех технических характеристик (диаметр, толщина стенки, марка стали, производитель)
- Наличия в тоннах и метрах
- Цен за тонну и за метр
- Управления количеством товара с кнопками +/- и ручным вводом
- Добавления/удаления из корзины
- Клика для открытия модального окна
- Современных иконок из lucide-react

### 3. ProductModal
Модальное окно с подробной информацией о товаре:
- Полная техническая информация (ГОСТ, диаметр, толщина стенки, марка стали)
- Информация о производителе и складе
- Способы оплаты (наличные/карта)
- Наличие в тоннах и метрах
- Средняя длина и вес трубы
- Управление корзиной с тем же функционалом
- Закрытие по Escape или клику вне модалки
- Плавные анимации появления/исчезновения

### 4. CartIcon
Иконка корзины с счетчиком товаров:
- Современная иконка ShoppingCart из lucide-react
- Отображение общего количества товаров
- Поддержка больших чисел (99+)
- Клик для перехода к корзине

## Использование

### Базовый пример

```tsx
import { Catalog } from './components/catalog/Catalog';
import type { Product, CartItem } from './types/catalog';

function App() {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  
  const products: Product[] = [
    {
      id: "10001",
      name: "Бесшовные холоднодеформированные, ТУ 14-162-68-2000, D 140.00ММ, S 3.50ММ, МСт 09Г2С-15, СинТЗ",
      gost: "ТУ 14-162-68-2000",
      manufacturer: "СинТЗ",
      steelGrade: "09Г2С-15",
      diameter: 140.0,
      pipeWallThickness: 3.5,
      inStockT: 34.01,
      inStockM: 338.40,
      priceT: 23000,
      priceM: 2300,
      stockName: "Екатеринбург",
      address: "поселок Шувакиш, ул Зеленая, 50а",
      schedule: "пн - чт 08.00-17.00, пт 08.00-15.45",
      cashPayment: true,
      cardPayment: true,
      avgTubeLength: 12.0,
      avgTubeWeight: 45.2,
      ownerShortName: "ТМК",
      productionType: "Бесшовные холоднодеформированные",
      formOfLength: "НК",
      status: 1,
      koef: 0.011782032
    }
  ];

  const handleAddToCart = (product: Product, quantity: number) => {
    setCartItems(prevItems => {
      const existingItem = prevItems.find(item => item.product.id === product.id);
      if (existingItem) {
        return prevItems.map(item =>
          item.product.id === product.id
            ? { ...item, quantity: item.quantity + quantity }
            : item
        );
      } else {
        return [...prevItems, { product, quantity }];
      }
    });
  };

  const handleRemoveFromCart = (productId: string) => {
    setCartItems(prevItems => prevItems.filter(item => item.product.id !== productId));
  };

  const handleUpdateQuantity = (productId: string, quantity: number) => {
    if (quantity <= 0) {
      handleRemoveFromCart(productId);
    } else {
      setCartItems(prevItems =>
        prevItems.map(item =>
          item.product.id === productId
            ? { ...item, quantity }
            : item
        )
      );
    }
  };

  return (
    <Catalog
      products={products}
      onAddToCart={handleAddToCart}
      onRemoveFromCart={handleRemoveFromCart}
      onUpdateQuantity={handleUpdateQuantity}
      cartItems={cartItems}
    />
  );
}
```

### Демонстрация

Для просмотра демонстрации компонентов перейдите по адресу `/catalog-demo` в приложении.

## Типы данных

### Product
```typescript
interface Product {
  id: string;
  name: string;
  gost: string;
  manufacturer: string;
  steelGrade: string;
  diameter: number;
  pipeWallThickness: number;
  inStockT: number; // Наличие в тоннах
  inStockM: number; // Наличие в метрах
  priceT: number; // Цена за тонну
  priceM: number; // Цена за метр
  stockName: string;
  address: string;
  schedule: string;
  cashPayment: boolean;
  cardPayment: boolean;
  avgTubeLength: number;
  avgTubeWeight: number;
  ownerShortName: string;
  productionType: string;
  formOfLength: string;
  status: number;
  koef: number;
}
```

### CartItem
```typescript
interface CartItem {
  product: Product;
  quantity: number;
}
```

### ViewMode
```typescript
type ViewMode = 'grid' | 'list';
```

## Особенности

1. **Адаптивность**: Компоненты полностью адаптивны и корректно отображаются на всех устройствах
2. **Accessibility**: Поддержка клавиатурной навигации и ARIA-атрибутов
3. **TypeScript**: Полная типизация для безопасности разработки
4. **Tailwind CSS**: Использование utility-first CSS фреймворка
5. **Модульность**: Каждый компонент может использоваться независимо

## Стилизация

Компоненты используют Tailwind CSS классы. Для кастомизации можно:
- Переопределить CSS переменные
- Добавить собственные классы через `className` пропсы
- Использовать CSS-in-JS решения

## Требования

- React 18+
- TypeScript 4.5+
- Tailwind CSS 3.0+
- lucide-react (для иконок)
- framer-motion (для анимаций)

## Особенности

1. **Адаптивность**: Компоненты полностью адаптивны и корректно отображаются на всех устройствах, включая мобильные (Telegram WebApp)
2. **Accessibility**: Поддержка клавиатурной навигации и ARIA-атрибутов
3. **TypeScript**: Полная типизация для безопасности разработки
4. **Tailwind CSS**: Использование utility-first CSS фреймворка
5. **Модульность**: Каждый компонент может использоваться независимо
6. **Анимации**: Плавные анимации с framer-motion
7. **Современные иконки**: Использование lucide-react для консистентного дизайна
8. **Реактивность**: Все состояния синхронизированы между компонентами
