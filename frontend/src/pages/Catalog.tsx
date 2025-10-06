import { useEffect, useState } from 'react';
import { Api } from '../lib/api';
import { useCartStore } from '../store/cart';
import { Telegram } from '../lib/telegram';
import { Catalog } from '../components/catalog/Catalog';
import { adaptProductDtosToProducts } from '../lib/catalogAdapter';
import type { Product, CartItem } from '../types/catalog';

export function CatalogPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const { setUserId, add, updateByProductId, removeByProductId, cart } = useCartStore();

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

    // Загружаем продукты
    Promise.all([Api.getProducts()]).then(([p]) => {
      const adaptedProducts = adaptProductDtosToProducts(p);
      setProducts(adaptedProducts);
    }).finally(() => setLoading(false));
  }, [setUserId]);

  // Обработчики для работы с корзиной
  const handleAddToCart = (product: Product, quantity: number) => {
    add({ productId: parseInt(product.id), quantity });
  };

  const handleRemoveFromCart = (productId: string) => {
    removeByProductId(parseInt(productId));
  };

  const handleUpdateQuantity = (productId: string, quantity: number) => {
    if (quantity <= 0) {
      handleRemoveFromCart(productId);
    } else {
      updateByProductId(parseInt(productId), { quantity });
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
      id: item.productId.toString(),
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
    />
  );
}