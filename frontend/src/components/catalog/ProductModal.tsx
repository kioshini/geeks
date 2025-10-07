import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  X, 
  MapPin, 
  Clock, 
  Building, 
  Scale, 
  Ruler, 
  CreditCard, 
  Banknote,
  Plus,
  Minus,
  ShoppingCart
} from 'lucide-react';
import type { ProductModalProps } from '../../types/catalog';
import { useUnit } from '../../contexts/UnitContext';

/**
 * Модальное окно с подробной информацией о товаре
 * 
 * @param props - Пропсы компонента
 * @returns JSX элемент модального окна
 */
export function ProductModal({
  product,
  isOpen,
  onClose,
  cartQuantity,
  onAddToCart,
  onRemoveFromCart,
  onUpdateQuantity
}: ProductModalProps) {
  const [quantity, setQuantity] = useState(1);
  const { selectedUnit } = useUnit();

  // Сброс количества при открытии модалки
  useEffect(() => {
    if (isOpen) {
      setQuantity(1);
    }
  }, [isOpen]);

  // Обработка нажатия Escape
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleEscape);
      // Блокируем скролл страницы
      document.body.style.overflow = 'hidden';
    }

    return () => {
      document.removeEventListener('keydown', handleEscape);
      document.body.style.overflow = 'unset';
    };
  }, [isOpen, onClose]);

  if (!product) {
    return null;
  }

  const handleBackdropClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
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

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.2 }}
          className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50"
          onClick={handleBackdropClick}
        >
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 20 }}
            transition={{ duration: 0.3, ease: "easeOut" }}
            className="bg-white rounded-lg shadow-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto mx-2 sm:mx-4"
          >
            {/* Заголовок модалки */}
            <div className="flex items-center justify-between p-3 sm:p-6 border-b border-grayLight">
              <h2 className="text-lg sm:text-xl lg:text-2xl font-bold text-[#171A1F]">Информация о товаре</h2>
              <button
                onClick={onClose}
                className="text-grayDark hover:text-dark transition-colors p-2 hover:bg-grayLight rounded-full"
                aria-label="Закрыть модальное окно"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            {/* Содержимое модалки */}
            <div className="p-3 sm:p-6">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6 lg:gap-8">
                {/* Левая колонка - изображение и основная информация */}
                <div className="space-y-6">
                  {/* Информация о товаре */}
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <span className="px-3 py-1 bg-primary/10 text-primary text-sm font-semibold rounded">
                        {product.productionType}
                      </span>
                      {!isInStock && (
                        <span className="px-3 py-1 bg-red-100 text-red-800 text-sm font-semibold rounded">
                          Нет в наличии
                        </span>
                      )}
                    </div>
                  </div>

                  {/* Основная информация */}
                  <div className="space-y-4">
                    <div>
                      <h3 className="text-2xl font-bold text-[#171A1F] mb-2">
                        {product.name}
                      </h3>
                      <div className="flex items-center gap-2 text-[#565D6D]">
                        <Building className="w-4 h-4" />
                        <span>{product.manufacturer}</span>
                      </div>
                    </div>

                    {/* Цены */}
                    <div className="bg-primary/5 rounded-lg p-4 border border-primary/20">
                      <h4 className="text-lg font-semibold text-[#171A1F] mb-3">Цены</h4>
                      <div className="space-y-2">
                        {selectedUnit === 'т' ? (
                          <div className="flex items-center justify-between">
                            <span className="text-[#565D6D]">За тонну:</span>
                            <span className="text-xl font-medium text-[#171A1F]">
                              {product.priceT.toLocaleString('ru-RU')} ₽
                            </span>
                          </div>
                        ) : (
                          <div className="flex items-center justify-between">
                            <span className="text-[#565D6D]">За метр:</span>
                            <span className="text-xl font-medium text-[#E64A19]">
                              {product.priceM.toLocaleString('ru-RU')} ₽
                            </span>
                          </div>
                        )}
                      </div>
                    </div>
                  </div>
                </div>

                {/* Правая колонка - подробная информация */}
                <div className="space-y-6">
                  {/* Технические характеристики */}
                  <div>
                    <h4 className="text-lg font-semibold text-[#171A1F] mb-4 flex items-center gap-2">
                      <Ruler className="w-5 h-5" />
                      Технические характеристики
                    </h4>
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-3">
                        <div>
                          <span className="text-sm text-[#171A1F]">ГОСТ:</span>
                          <p className="font-medium text-[#171A1F]">{product.gost}</p>
                        </div>
                        <div>
                          <span className="text-sm text-[#171A1F]">Диаметр:</span>
                          <p className="font-medium text-[#171A1F]">{product.diameter} мм</p>
                        </div>
                        <div>
                          <span className="text-sm text-[#171A1F]">Толщина стенки:</span>
                          <p className="font-medium text-[#171A1F]">{product.pipeWallThickness} мм</p>
                        </div>
                        <div>
                          <span className="text-sm text-[#171A1F]">Марка стали:</span>
                          <p className="font-medium text-[#171A1F]">{product.steelGrade}</p>
                        </div>
                      </div>
                      <div className="space-y-3">
                        <div>
                          <span className="text-sm text-[#565D6D]">Форма длины:</span>
                          <p className="font-medium text-[#171A1F]">{product.formOfLength}</p>
                        </div>
                        <div>
                          <span className="text-sm text-[#565D6D]">Средняя длина трубы:</span>
                          <p className="font-medium text-[#171A1F]">{product.avgTubeLength} м</p>
                        </div>
                        <div>
                          <span className="text-sm text-[#565D6D]">Средний вес трубы:</span>
                          <p className="font-medium text-[#171A1F]">{product.avgTubeWeight} кг</p>
                        </div>
                        <div>
                          <span className="text-sm text-[#565D6D]">Коэффициент:</span>
                          <p className="font-medium text-[#171A1F]">{product.koef}</p>
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Наличие */}
                  <div>
                    <h4 className="text-lg font-semibold text-[#171A1F] mb-4 flex items-center gap-2">
                      <Scale className="w-5 h-5" />
                      Наличие на складе
                    </h4>
                    <div className="grid grid-cols-2 gap-4">
                      {product.inStockT > 0 && (
                        <div className="bg-green-50 rounded-lg p-3">
                          <div className="text-sm text-[#565D6D]">В тоннах</div>
                          <div className="text-xl font-bold text-[#22C55E]">
                            {product.inStockT.toFixed(2)} т
                          </div>
                        </div>
                      )}
                      {product.inStockM > 0 && (
                        <div className="bg-blue-50 rounded-lg p-3">
                          <div className="text-sm text-[#565D6D]">В метрах</div>
                          <div className="text-xl font-bold text-blue-600">
                            {product.inStockM.toFixed(2)} м
                          </div>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Информация о складе */}
                  <div>
                    <h4 className="text-lg font-semibold text-[#171A1F] mb-4 flex items-center gap-2">
                      <MapPin className="w-5 h-5" />
                      Информация о складе
                    </h4>
                    <div className="space-y-3">
                      <div>
                        <span className="text-sm text-[#565D6D]">Склад:</span>
                        <p className="font-medium text-[#171A1F]">{product.stockName}</p>
                      </div>
                      <div>
                        <span className="text-sm text-[#565D6D]">Адрес:</span>
                        <p className="font-medium text-[#171A1F]">{product.address}</p>
                      </div>
                      <div className="flex items-start gap-2">
                        <Clock className="w-4 h-4 text-[#565D6D] mt-0.5" />
                        <div>
                          <span className="text-sm text-[#565D6D]">Режим работы:</span>
                          <p className="font-medium text-[#171A1F]">{product.schedule}</p>
                        </div>
                      </div>
                      <div>
                        <span className="text-sm text-[#565D6D]">Владелец:</span>
                        <p className="font-medium text-[#171A1F]">{product.ownerShortName}</p>
                      </div>
                    </div>
                  </div>

                  {/* Способы оплаты */}
                  <div>
                    <h4 className="text-lg font-semibold text-[#171A1F] mb-4 flex items-center gap-2">
                      <CreditCard className="w-5 h-5" />
                      Способы оплаты
                    </h4>
                    <div className="flex gap-4">
                      {product.cashPayment && (
                        <div className="flex items-center gap-2 text-green-600">
                          <Banknote className="w-4 h-4" />
                          <span className="font-medium">Наличные</span>
                        </div>
                      )}
                      {product.cardPayment && (
                        <div className="flex items-center gap-2 text-blue-600">
                          <CreditCard className="w-4 h-4" />
                          <span className="font-medium">Карта</span>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Управление корзиной */}
                  <div className="border-t border-grayLight pt-6">
                    <h4 className="text-lg font-semibold text-[#171A1F] mb-4">Добавить в корзину</h4>
                    
                    {!isInCart ? (
                      // Кнопка добавления в корзину
                      <div className="space-y-4">
                        <div className="flex items-center space-x-4">
                          <span className="text-sm font-medium text-[#565D6D]">Количество:</span>
                          <div className="flex items-center space-x-2">
                            <button
                              onClick={handleDecrement}
                              className="w-10 h-10 bg-grayLight hover:bg-grayDark hover:text-white rounded-full flex items-center justify-center transition-colors"
                              disabled={quantity <= 1}
                            >
                              <Minus className="w-5 h-5" />
                            </button>
                            
                            <input
                              type="number"
                              value={quantity}
                              onChange={handleInputChange}
                              className="w-20 text-center border border-[#DEE1E6] rounded-md px-3 py-2 text-lg focus:outline-none focus:ring-2 focus:ring-primary"
                              min="1"
                              max="999"
                            />
                            
                            <button
                              onClick={handleIncrement}
                              className="w-10 h-10 bg-grayLight hover:bg-grayDark hover:text-white rounded-full flex items-center justify-center transition-colors"
                            >
                              <Plus className="w-5 h-5" />
                            </button>
                          </div>
                        </div>
                        
                        <button
                          onClick={handleAddToCart}
                          disabled={!isInStock}
                          className="w-full bg-[#171A1F] hover:bg-[#1F1A1F] disabled:bg-gray-300 disabled:cursor-not-allowed text-white font-medium py-3 px-6 rounded-md transition text-lg flex items-center justify-center gap-2"
                        >
                          <ShoppingCart className="w-5 h-5" />
                          {isInStock ? 'Добавить в корзину' : 'Нет в наличии'}
                        </button>
                      </div>
                    ) : (
                      // Управление количеством в корзине
                      <div className="space-y-4">
                        <div className="flex items-center justify-between p-4 bg-green-50 rounded-lg">
                          <span className="text-green-800 font-medium">Товар уже в корзине</span>
                          <span className="text-green-600 font-bold text-lg">{cartQuantity} шт.</span>
                        </div>
                        
                        <div className="flex items-center space-x-4">
                          <span className="text-sm font-medium text-[#565D6D]">Изменить количество:</span>
                          <div className="flex items-center space-x-2">
                            <button
                              onClick={() => onUpdateQuantity(product.id, cartQuantity - 1)}
                              className="w-10 h-10 bg-red-100 hover:bg-red-200 text-red-600 rounded-full flex items-center justify-center transition-colors touch-manipulation"
                            >
                              <Minus className="w-5 h-5" />
                            </button>
                            
                            <input
                              type="number"
                              value={cartQuantity}
                              onChange={(e) => {
                                const value = parseInt(e.target.value) || 1;
                                onUpdateQuantity(product.id, Math.max(1, Math.min(999, value)));
                              }}
                              className="w-20 text-center border border-[#DEE1E6] rounded-md px-3 py-2 text-lg focus:outline-none focus:ring-2 focus:ring-primary touch-manipulation"
                              min="1"
                              max="999"
                            />
                            
                            <button
                              onClick={() => onUpdateQuantity(product.id, cartQuantity + 1)}
                              className="w-10 h-10 bg-green-100 hover:bg-green-200 text-green-600 rounded-full flex items-center justify-center transition-colors touch-manipulation"
                            >
                              <Plus className="w-5 h-5" />
                            </button>
                          </div>
                        </div>
                        
                        <button
                          onClick={() => onRemoveFromCart(product.id)}
                          className="w-full bg-[#F44336] hover:bg-red-600 text-white font-medium py-3 px-6 rounded-md transition touch-manipulation"
                        >
                          Убрать из корзины
                        </button>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}