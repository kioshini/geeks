/**
 * Интерфейс для товара в каталоге
 */
export interface Product {
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

/**
 * Режимы отображения каталога
 */
export type ViewMode = 'grid' | 'list';

/**
 * Элемент корзины
 */
export interface CartItem {
  product: Product;
  quantity: number;
}

/**
 * Пропсы для компонента каталога
 */
export interface CatalogProps {
  products: Product[];
  onAddToCart: (product: Product, quantity: number) => void;
  onRemoveFromCart: (productId: string) => void;
  onUpdateQuantity: (productId: string, quantity: number) => void;
  cartItems: CartItem[];
}

/**
 * Пропсы для карточки товара
 */
export interface ProductCardProps {
  product: Product;
  viewMode: ViewMode;
  cartQuantity: number;
  onAddToCart: (product: Product, quantity: number) => void;
  onRemoveFromCart: (productId: string) => void;
  onUpdateQuantity: (productId: string, quantity: number) => void;
  onProductClick: (product: Product) => void;
}

/**
 * Пропсы для модального окна товара
 */
export interface ProductModalProps {
  product: Product | null;
  isOpen: boolean;
  onClose: () => void;
  cartQuantity: number;
  onAddToCart: (product: Product, quantity: number) => void;
  onRemoveFromCart: (productId: string) => void;
  onUpdateQuantity: (productId: string, quantity: number) => void;
}

/**
 * Пропсы для иконки корзины
 */
export interface CartIconProps {
  totalItems: number;
  onClick?: () => void;
}
