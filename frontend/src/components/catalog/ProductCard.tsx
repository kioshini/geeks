import { useState } from 'react';
import { Plus, Minus, ShoppingCart, MapPin } from 'lucide-react';
import type { ProductCardProps } from '../../types/catalog';
import { useUnit } from '../../hooks/useUnit';

/**
 * Компонент карточки товара с поддержкой режимов сетки и списка
 * 
 * @param props - Пропсы компонента
 * @returns JSX элемент карточки товара
 */
export function ProductCard({
  product,
  viewMode,
  cartQuantity,
  cartUnit,
  onAddToCart,
  onRemoveFromCart,
  onUpdateQuantity,
  onProductClick
}: ProductCardProps) {
  const [quantity, setQuantity] = useState(1);
  const { selectedUnit } = useUnit();

  const handleCardClick = (e: React.MouseEvent) => {
    // Не открываем модалку при клике на кнопки
    if ((e.target as HTMLElement).closest('button')) {
      return;
    }
    onProductClick(product);
  };

  const handleAddToCart = () => {
    try {
      onAddToCart(product, quantity);
      setQuantity(1);
    } catch (error) {
      console.error('Ошибка при добавлении в корзину:', error);
    }
  };

  const handleQuantityChange = (newQuantity: number) => {
    const validQuantity = Math.max(1, Math.min(999, newQuantity));
    setQuantity(validQuantity);
  };

  const handleIncrement = () => {
    handleQuantityChange(quantity + 1);
  };

  const handleDecrement = () => {
    handleQuantityChange(quantity - 1);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value) || 1;
    handleQuantityChange(value);
  };

  const isInCart = cartQuantity > 0;
  const isInStock = product.inStockT > 0 || product.inStockM > 0;

  // Стили для разных режимов отображения - используем белый фон для карточек товаров для контраста с серым фоном приложения
  const cardClasses = viewMode === 'grid' 
    ? "bg-white rounded-lg shadow-lg hover:shadow-xl transition-all duration-200 cursor-pointer overflow-hidden border border-gray-300 hover:border-orange-500/30"
    : "bg-white rounded-lg shadow-lg hover:shadow-xl transition-all duration-200 cursor-pointer overflow-hidden border border-gray-300 hover:border-orange-500/30 flex";

  const contentClasses = viewMode === 'grid'
    ? "p-2 sm:p-4"
    : "p-2 sm:p-4 flex-1 flex flex-col justify-between";

  return (
    <div 
      className={cardClasses}
      onClick={handleCardClick}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          onProductClick(product);
        }
      }}
    >

      {/* Содержимое карточки */}
      <div className={contentClasses}>
        {/* Название и основные характеристики */}
        <div className="mb-2 sm:mb-3">
          <div className="flex items-center justify-between mb-1 sm:mb-2">
            <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-orange-100 text-orange-600 text-xs font-semibold rounded">
              {product.productionType}
            </span>
            {!isInStock && (
              <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-red-100 text-red-800 text-xs font-semibold rounded">
                Нет в наличии
              </span>
            )}
          </div>
          <h3 className="text-sm sm:text-base lg:text-lg font-semibold text-[#171A1F] mb-1 sm:mb-2 line-clamp-2">
            {product.name}
          </h3>
          
          {/* Основные характеристики */}
          <div className="space-y-0.5 sm:space-y-1 mb-2 sm:mb-3">
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-[#171A1F]">Диаметр:</span>
              <span className="font-medium text-[#171A1F]">{product.diameter} мм</span>
            </div>
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-[#171A1F]">Толщина стенки:</span>
              <span className="font-medium text-[#171A1F]">{product.pipeWallThickness} мм</span>
            </div>
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-[#171A1F]">Марка стали:</span>
              <span className="font-medium text-[#171A1F]">{product.steelGrade}</span>
            </div>
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-[#171A1F]">Производитель:</span>
              <span className="font-medium text-[#171A1F]">{product.manufacturer}</span>
            </div>
          </div>

          {/* Наличие */}
          <div className="flex items-center justify-between text-xs sm:text-sm mb-1 sm:mb-2">
            <span className="text-[#565D6D]">Наличие:</span>
            <div className="flex gap-1 sm:gap-2">
              {product.inStockT > 0 && (
                <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-green-100 text-[#22C55E] rounded text-xs">
                  {product.inStockT.toFixed(1)} т
                </span>
              )}
              {product.inStockM > 0 && (
                <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-orange-100 text-orange-600 rounded text-xs">
                  {product.inStockM.toFixed(1)} м
                </span>
              )}
            </div>
          </div>

          {/* Склад */}
          <div className="flex items-center gap-1 text-xs text-[#565D6D] mb-2">
            <MapPin className="w-3 h-3" />
            <span className="truncate">{product.stockName}</span>
          </div>
        </div>

        {/* Разделительная черта */}
        <div className="border-b border-[#DEE1E6] mt-2 mb-2"></div>

        {/* Цены */}
        <div className="mb-2 sm:mb-4">
          {selectedUnit === 'т' ? (
            <div className="flex items-center justify-between">
              <span className="text-xs sm:text-sm text-[#565D6D]">Цена за тонну:</span>
              <span className="text-sm sm:text-base lg:text-lg font-medium text-[#171A1F]">
                {product.priceT.toLocaleString('ru-RU')} ₽
              </span>
            </div>
          ) : (
            <div className="flex items-center justify-between">
              <span className="text-xs sm:text-sm text-[#565D6D]">Цена за метр:</span>
              <span className="text-sm sm:text-base lg:text-lg font-medium text-[#E64A19]">
                {product.priceM.toLocaleString('ru-RU')} ₽
              </span>
            </div>
          )}
        </div>

        {/* Кнопки управления корзиной */}
        <div className="space-y-2">
          {!isInCart ? (
            // Кнопка добавления в корзину
            <div className="space-y-2">
              <div className="flex items-center space-x-1 sm:space-x-2">
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleDecrement();
                  }}
                  className="w-10 h-10 sm:w-8 sm:h-8 bg-gray-300 hover:bg-gray-600 hover:text-white rounded-full flex items-center justify-center transition-colors touch-manipulation"
                  disabled={quantity <= 1}
                >
                  <Minus className="w-5 h-5 sm:w-4 sm:h-4" />
                </button>
                
                <input
                  type="number"
                  value={quantity}
                  onChange={handleInputChange}
                  onClick={(e) => e.stopPropagation()}
                  className="w-16 sm:w-16 text-center border border-[#DEE1E6] rounded-md px-2 py-2 sm:py-1 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 touch-manipulation"
                  min="1"
                  max="999"
                />
                
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleIncrement();
                  }}
                  className="w-10 h-10 sm:w-8 sm:h-8 bg-gray-300 hover:bg-gray-600 hover:text-white rounded-full flex items-center justify-center transition-colors touch-manipulation"
                >
                  <Plus className="w-5 h-5 sm:w-4 sm:h-4" />
                </button>
              </div>
              
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  handleAddToCart();
                }}
                disabled={!isInStock}
                className={`w-full py-3 sm:py-2 px-4 rounded-md transition flex items-center justify-center gap-2 touch-manipulation font-semibold ${
                  isInStock 
                    ? 'bg-orange-500 hover:bg-orange-600 text-white' 
                    : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                }`}
              >
                <ShoppingCart className="w-5 h-5 sm:w-4 sm:h-4" />
                <span className="text-sm sm:text-base" style={{ color: 'white', fontWeight: 'bold', display: 'block' }}>{isInStock ? 'Добавить в корзину' : 'Нет в наличии'}</span>
              </button>
            </div>
          ) : (
            // Управление количеством в корзине
            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-sm text-gray-700">В корзине:</span>
                <span className="font-semibold text-orange-500">{cartQuantity} {cartUnit === 'т' ? 'т' : 'м'}</span>
              </div>
              
              <div className="flex items-center space-x-1 sm:space-x-2">
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onUpdateQuantity(product.id, cartQuantity - 1);
                  }}
                  className="w-10 h-10 sm:w-8 sm:h-8 bg-red-100 hover:bg-red-200 text-red-600 rounded-full flex items-center justify-center transition-colors touch-manipulation"
                >
                  <Minus className="w-5 h-5 sm:w-4 sm:h-4" />
                </button>
                
                <input
                  type="number"
                  value={cartQuantity}
                  onChange={(e) => {
                    const value = parseInt(e.target.value) || 1;
                    onUpdateQuantity(product.id, Math.max(1, Math.min(999, value)));
                  }}
                  onClick={(e) => e.stopPropagation()}
                  className="w-16 text-center border border-[#DEE1E6] rounded-md px-2 py-2 sm:py-1 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 touch-manipulation"
                  min="1"
                  max="999"
                />
                
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onUpdateQuantity(product.id, cartQuantity + 1);
                  }}
                  className="w-10 h-10 sm:w-8 sm:h-8 bg-green-100 hover:bg-green-200 text-green-600 rounded-full flex items-center justify-center transition-colors touch-manipulation"
                >
                  <Plus className="w-5 h-5 sm:w-4 sm:h-4" />
                </button>
              </div>
              
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  onRemoveFromCart(product.id);
                }}
                className="w-full bg-red-500 hover:bg-red-600 text-white font-semibold py-3 sm:py-2 px-4 rounded-md transition touch-manipulation"
              >
                <span className="text-sm sm:text-base" style={{ color: 'white', fontWeight: 'bold', display: 'block' }}>Убрать из корзины</span>
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}