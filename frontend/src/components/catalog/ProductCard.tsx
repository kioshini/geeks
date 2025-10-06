import React, { useState } from 'react';
import { Plus, Minus, ShoppingCart, MapPin } from 'lucide-react';
import type { ProductCardProps } from '../../types/catalog';

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
  onAddToCart,
  onRemoveFromCart,
  onUpdateQuantity,
  onProductClick
}: ProductCardProps) {
  const [quantity, setQuantity] = useState(1);

  const handleCardClick = (e: React.MouseEvent) => {
    // Не открываем модалку при клике на кнопки
    if ((e.target as HTMLElement).closest('button')) {
      return;
    }
    onProductClick(product);
  };

  const handleAddToCart = () => {
    onAddToCart(product, quantity);
    setQuantity(1);
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

  // Стили для разных режимов отображения
  const cardClasses = viewMode === 'grid' 
    ? "bg-white rounded-lg shadow-lg hover:shadow-xl transition-all duration-200 cursor-pointer overflow-hidden border border-grayLight hover:border-primary/30"
    : "bg-white rounded-lg shadow-lg hover:shadow-xl transition-all duration-200 cursor-pointer overflow-hidden border border-grayLight hover:border-primary/30 flex";

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
            <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-primary/10 text-primary text-xs font-semibold rounded">
              {product.productionType}
            </span>
            {!isInStock && (
              <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-red-100 text-red-800 text-xs font-semibold rounded">
                Нет в наличии
              </span>
            )}
          </div>
          <h3 className="text-sm sm:text-base lg:text-lg font-semibold text-dark mb-1 sm:mb-2 line-clamp-2">
            {product.name}
          </h3>
          
          {/* Основные характеристики */}
          <div className="space-y-0.5 sm:space-y-1 mb-2 sm:mb-3">
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-grayDark">Диаметр:</span>
              <span className="font-medium text-dark">{product.diameter} мм</span>
            </div>
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-grayDark">Толщина стенки:</span>
              <span className="font-medium text-dark">{product.pipeWallThickness} мм</span>
            </div>
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-grayDark">Марка стали:</span>
              <span className="font-medium text-dark">{product.steelGrade}</span>
            </div>
            <div className="flex items-center justify-between text-xs sm:text-sm">
              <span className="text-grayDark">Производитель:</span>
              <span className="font-medium text-dark">{product.manufacturer}</span>
            </div>
          </div>

          {/* Наличие */}
          <div className="flex items-center justify-between text-xs sm:text-sm mb-1 sm:mb-2">
            <span className="text-grayDark">Наличие:</span>
            <div className="flex gap-1 sm:gap-2">
              {product.inStockT > 0 && (
                <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-green-100 text-green-800 rounded text-xs">
                  {product.inStockT.toFixed(1)} т
                </span>
              )}
              {product.inStockM > 0 && (
                <span className="px-1.5 sm:px-2 py-0.5 sm:py-1 bg-primary/10 text-primary rounded text-xs">
                  {product.inStockM.toFixed(1)} м
                </span>
              )}
            </div>
          </div>

          {/* Склад */}
          <div className="flex items-center gap-1 text-xs text-grayDark mb-2">
            <MapPin className="w-3 h-3" />
            <span className="truncate">{product.stockName}</span>
          </div>
        </div>

        {/* Цены */}
        <div className="mb-2 sm:mb-4">
          <div className="flex items-center justify-between mb-1">
            <span className="text-xs sm:text-sm text-grayDark">Цена за тонну:</span>
            <span className="text-sm sm:text-base lg:text-lg font-bold text-primary">
              {product.priceT.toLocaleString('ru-RU')} ₽
            </span>
          </div>
          <div className="flex items-center justify-between">
            <span className="text-xs sm:text-sm text-grayDark">Цена за метр:</span>
            <span className="text-sm sm:text-base lg:text-lg font-bold text-primary">
              {product.priceM.toLocaleString('ru-RU')} ₽
            </span>
          </div>
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
                  className="w-10 h-10 sm:w-8 sm:h-8 bg-grayLight hover:bg-grayDark hover:text-white rounded-full flex items-center justify-center transition-colors touch-manipulation"
                  disabled={quantity <= 1}
                >
                  <Minus className="w-5 h-5 sm:w-4 sm:h-4" />
                </button>
                
                <input
                  type="number"
                  value={quantity}
                  onChange={handleInputChange}
                  onClick={(e) => e.stopPropagation()}
                  className="w-16 sm:w-16 text-center border border-grayLight rounded px-2 py-2 sm:py-1 text-sm focus:outline-none focus:ring-2 focus:ring-primary touch-manipulation"
                  min="1"
                  max="999"
                />
                
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleIncrement();
                  }}
                  className="w-10 h-10 sm:w-8 sm:h-8 bg-grayLight hover:bg-grayDark hover:text-white rounded-full flex items-center justify-center transition-colors touch-manipulation"
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
                className="w-full bg-primary hover:bg-primaryDark disabled:bg-gray-300 disabled:cursor-not-allowed text-white font-semibold py-3 sm:py-2 px-4 rounded-lg transition-colors duration-200 flex items-center justify-center gap-2 touch-manipulation"
              >
                <ShoppingCart className="w-5 h-5 sm:w-4 sm:h-4" />
                <span className="text-sm sm:text-base">{isInStock ? 'Добавить в корзину' : 'Нет в наличии'}</span>
              </button>
            </div>
          ) : (
            // Управление количеством в корзине
            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-sm text-grayDark">В корзине:</span>
                <span className="font-semibold text-primary">{cartQuantity} шт.</span>
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
                  className="w-16 text-center border border-grayLight rounded px-2 py-2 sm:py-1 text-sm focus:outline-none focus:ring-2 focus:ring-primary touch-manipulation"
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
                className="w-full bg-red-500 hover:bg-red-600 text-white font-semibold py-3 sm:py-2 px-4 rounded-lg transition-colors duration-200 touch-manipulation"
              >
                <span className="text-sm sm:text-base">Удалить из корзины</span>
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}