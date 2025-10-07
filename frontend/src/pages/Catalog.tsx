import { useEffect, useState, useCallback } from 'react';
import { useLocation } from 'react-router-dom';
import { Api } from '../lib/api';
import { useCartStore } from '../store/cart';
import { Telegram } from '../lib/telegram';
import { Catalog } from '../components/catalog/Catalog';
import { adaptProductDtosToProducts } from '../lib/catalogAdapter';
import { 
  adaptJsonDataToProducts, 
  getUpdatedProductsWithDeltas
} from '../lib/jsonDataAdapter';
import { deltaUpdatesService } from '../lib/deltaUpdatesService';
import { useUnit } from '../contexts/UnitContext';
import type { Product, CartItem } from '../types/catalog';
import type { PricesEl, RemnantsEl } from '../lib/api';

export function CatalogPage() {
  const location = useLocation();
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [resetFiltersTrigger, setResetFiltersTrigger] = useState(0);
  const { selectedUnit } = useUnit();
  const { setUserId, add, updateByProductId, removeByProductId, cart } = useCartStore();

  // Сбрасываем фильтры при переходе на главную страницу (когда URL = "/")
  useEffect(() => {
    if (location.pathname === '/') {
      setResetFiltersTrigger(prev => prev + 1);
    }
  }, [location.pathname]);

  // Функция для загрузки данных из JSON
  const loadJsonData = useCallback(async () => {
    try {
      console.log('Загрузка данных из JSON...');
      
      // Загружаем все JSON данные параллельно
      const [nomenclature, prices, remnants, stocks, types] = await Promise.all([
        Api.getNomenclature(),
        Api.getPrices(),
        Api.getRemnants(),
        Api.getStocks(),
        Api.getTypes()
      ]);

      console.log('JSON данные загружены:', {
        nomenclature: nomenclature.ArrayOfNomenclatureEl.length,
        prices: prices.ArrayOfPricesEl.length,
        remnants: remnants.ArrayOfRemnantsEl.length,
        stocks: stocks.ArrayOfStockEl.length,
        types: types.ArrayOfTypeEl.length
      });

      // Преобразуем JSON данные в товары каталога
      const adaptedProducts = adaptJsonDataToProducts(nomenclature, prices, remnants, stocks, types);
      setProducts(adaptedProducts);
      
      console.log(`Преобразовано ${adaptedProducts.length} товаров из JSON данных`);
    } catch (error) {
      console.error('Ошибка при загрузке JSON данных:', error);
      
      // Fallback на старый API
      try {
        console.log('Переключение на старый API...');
        const [p] = await Promise.all([Api.getProducts()]);
        const adaptedProducts = adaptProductDtosToProducts(p);
        setProducts(adaptedProducts);
        console.log(`Загружено ${adaptedProducts.length} товаров через старый API`);
      } catch (fallbackError) {
        console.error('Ошибка при загрузке через старый API:', fallbackError);
        setProducts([]);
      }
    }
  }, []);

  // Функция для обновления данных с дельтами
  const updateDataWithDeltas = useCallback(async (priceDeltas: PricesEl[], remnantDeltas: RemnantsEl[]) => {
    try {
      console.log('Обновление данных с дельтами:', {
        priceDeltas: priceDeltas.length,
        remnantDeltas: remnantDeltas.length
      });

      const updatedProducts = getUpdatedProductsWithDeltas(priceDeltas, remnantDeltas);
      setProducts(updatedProducts);
      
      console.log(`Обновлено ${updatedProducts.length} товаров с дельтами`);
    } catch (error) {
      console.error('Ошибка при обновлении с дельтами:', error);
    }
  }, []);

  // Обработчик дельтовых обновлений
  const handleDeltaUpdates = useCallback((priceDeltas: PricesEl[], remnantDeltas: RemnantsEl[]) => {
    console.log('Получены дельтовые обновления:', {
      priceDeltas: priceDeltas.length,
      remnantDeltas: remnantDeltas.length
    });

    // Обновляем товары с учетом дельт
    updateDataWithDeltas(priceDeltas, remnantDeltas);
  }, [updateDataWithDeltas]);

  useEffect(() => {
    // Определяем user id из Telegram или fallback demo id для локального тестирования
    const tgUser = Telegram.user();
    const demoStored = typeof window !== 'undefined' ? window.localStorage.getItem('demoUserId') : null;
    let uid: number | null = tgUser?.id ?? (demoStored ? Number(demoStored) : null);
    if (!uid) {
      uid = 999001; // demo user id
      try { window.localStorage.setItem('demoUserId', String(uid)); } catch {}
    }
    setUserId(uid);

    // Загружаем данные
    loadJsonData().finally(() => setLoading(false));

    // Запускаем мониторинг дельтовых обновлений
    deltaUpdatesService.addUpdateListener(handleDeltaUpdates);
    deltaUpdatesService.startMonitoring(30000); // Проверяем каждые 30 секунд

    // Очистка при размонтировании
    return () => {
      deltaUpdatesService.removeUpdateListener(handleDeltaUpdates);
      deltaUpdatesService.stopMonitoring();
    };
  }, [setUserId, loadJsonData, handleDeltaUpdates]);

  // Обработчики для работы с корзиной
  const handleAddToCart = (product: Product, quantity: number) => {
    console.log('CatalogPage: Получен запрос на добавление в корзину', { 
      productId: product.id, 
      quantity, 
      productName: product.name, 
      unit: selectedUnit,
      userId: cart?.userId || 'не установлен',
      addFunction: typeof add
    });
    try {
      if (typeof add === 'function') {
        console.log('CatalogPage: Вызываем add из store...');
        add({ productId: product.id.toString(), quantity, unit: selectedUnit });
        console.log('CatalogPage: Товар успешно добавлен в корзину');
      } else {
        console.error('CatalogPage: add не является функцией!', add);
      }
    } catch (error) {
      console.error('CatalogPage: Ошибка при добавлении в корзину:', error);
    }
  };

  const handleRemoveFromCart = (productId: number) => {
    console.log('CatalogPage: Удаление из корзины', { productId });
    removeByProductId(productId.toString());
  };

  const handleUpdateQuantity = (productId: number, quantity: number) => {
    console.log('CatalogPage: Обновление количества', { productId, quantity, unit: selectedUnit });
    if (quantity <= 0) {
      handleRemoveFromCart(productId);
    } else {
      updateByProductId(productId.toString(), { quantity, unit: selectedUnit });
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500 mx-auto mb-4"></div>
          <p className="text-gray-600">Загрузка каталога...</p>
        </div>
      </div>
    );
  }

  // Преобразуем данные корзины из глобального состояния в формат для Catalog
  const cartItems: CartItem[] = cart?.items.map(item => ({
    product: {
      id: parseInt(item.productId, 10),
      name: item.product?.name || `Товар #${item.productId}`,
      gost: 'Не указан',
      manufacturer: 'Не указан',
      steelGrade: item.product?.material || 'Не указан',
      diameter: item.product?.diameter || 0,
      pipeWallThickness: item.product?.thickness || 0,
      inStockT: item.product?.stockQuantity || 0,
      inStockM: item.product?.stockQuantity || 0,
      priceT: item.product?.price || 0,
      priceM: item.product?.price ? item.product.price * 0.1 : 0,
      stockName: 'Екатеринбург',
      address: 'поселок Шувакиш, ул Зеленая, 50а',
      schedule: 'пн - чт 08.00-17.00, пт 08.00-15.45',
      cashPayment: true,
      cardPayment: true,
      avgTubeLength: 12.0,
      avgTubeWeight: 45.2,
      ownerShortName: 'ТМК',
      productionType: item.product?.type || 'Не указан',
      formOfLength: 'НК',
      status: item.product?.isAvailable ? 1 : 0,
      koef: 0.011782032
    },
    quantity: item.quantity,
  })) || [];

  return (
    <Catalog
      products={products}
      onAddToCart={handleAddToCart}
      onRemoveFromCart={handleRemoveFromCart}
      onUpdateQuantity={handleUpdateQuantity}
      cartItems={cartItems}
      resetFiltersTrigger={resetFiltersTrigger}
    />
  );
}