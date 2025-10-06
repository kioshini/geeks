import { useState, useMemo } from 'react';
import { motion } from 'framer-motion';
import { Grid, List, Search, Filter, X } from 'lucide-react';
import type { CatalogProps, Product, ViewMode } from '../../types/catalog';
import { ProductCard } from './ProductCard';
import { ProductModal } from './ProductModal';

/**
 * Основной компонент каталога товаров с переключением режимов отображения
 * 
 * @param props - Пропсы компонента
 * @returns JSX элемент каталога
 */
export function Catalog({
  products,
  onAddToCart,
  onRemoveFromCart,
  onUpdateQuantity,
  cartItems
}: CatalogProps) {
  const [viewMode, setViewMode] = useState<ViewMode>('grid');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [selectedManufacturer, setSelectedManufacturer] = useState<string>('');
  const [selectedSteelGrade, setSelectedSteelGrade] = useState<string>('');

  // Получаем уникальные категории, производителей и марки стали для фильтрации
  const categories = useMemo(() => {
    const uniqueCategories = Array.from(new Set(products.map(p => p.productionType).filter(Boolean)));
    return uniqueCategories.sort();
  }, [products]);

  const manufacturers = useMemo(() => {
    const uniqueManufacturers = Array.from(new Set(products.map(p => p.manufacturer).filter(Boolean)));
    return uniqueManufacturers.sort();
  }, [products]);

  const steelGrades = useMemo(() => {
    const uniqueSteelGrades = Array.from(new Set(products.map(p => p.steelGrade).filter(Boolean)));
    return uniqueSteelGrades.sort();
  }, [products]);

  // Фильтруем товары по поисковому запросу и фильтрам
  const filteredProducts = useMemo(() => {
    return products.filter(product => {
      const matchesSearch = !searchQuery || 
        product.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        product.manufacturer.toLowerCase().includes(searchQuery.toLowerCase()) ||
        product.steelGrade.toLowerCase().includes(searchQuery.toLowerCase()) ||
        product.gost.toLowerCase().includes(searchQuery.toLowerCase());
      
      const matchesCategory = !selectedCategory || product.productionType === selectedCategory;
      const matchesManufacturer = !selectedManufacturer || product.manufacturer === selectedManufacturer;
      const matchesSteelGrade = !selectedSteelGrade || product.steelGrade === selectedSteelGrade;
      
      return matchesSearch && matchesCategory && matchesManufacturer && matchesSteelGrade;
    });
  }, [products, searchQuery, selectedCategory, selectedManufacturer, selectedSteelGrade]);


  // Получаем количество конкретного товара в корзине
  const getCartQuantity = (productId: string): number => {
    const cartItem = cartItems.find(item => item.product.id === productId);
    return cartItem ? cartItem.quantity : 0;
  };

  // Обработчики событий
  const handleProductClick = (product: Product) => {
    setSelectedProduct(product);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedProduct(null);
  };

  const handleViewModeChange = (mode: ViewMode) => {
    setViewMode(mode);
  };

  const handleClearFilters = () => {
    setSearchQuery('');
    setSelectedCategory('');
    setSelectedManufacturer('');
    setSelectedSteelGrade('');
  };

  const hasActiveFilters = searchQuery || selectedCategory || selectedManufacturer || selectedSteelGrade;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-2 sm:px-4 lg:px-8 py-4 sm:py-6 lg:py-8">
      {/* Заголовок и навигация */}
      <div className="mb-4 sm:mb-6 lg:mb-8">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 sm:mb-6">
          <div>
            <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold text-dark mb-1 sm:mb-2">Каталог труб</h1>
            <p className="text-sm sm:text-base text-grayDark">
              Найдено товаров: {filteredProducts.length} из {products.length}
            </p>
          </div>
        </div>

        {/* Панель управления */}
        <div className="bg-white rounded-lg shadow-lg border border-grayLight p-2 sm:p-4 mb-4 sm:mb-6">
          <div className="space-y-3 sm:space-y-4">
            {/* Поиск */}
            <div className="relative">
              <div className="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                <Search className="h-4 w-4 sm:h-5 sm:w-5 text-grayDark" />
              </div>
              <input
                type="text"
                placeholder="Поиск по названию, производителю..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-3 sm:pl-4 pr-9 sm:pr-10 py-2 sm:py-3 text-sm sm:text-base border border-grayLight rounded-lg focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent"
              />
            </div>

            {/* Фильтры */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-2 sm:gap-4">
              {/* Фильтр по категориям */}
              {categories.length > 0 && (
                <div>
                  <label className="block text-xs sm:text-sm font-medium text-grayDark mb-1 sm:mb-2">Тип производства</label>
                  <select
                    value={selectedCategory}
                    onChange={(e) => setSelectedCategory(e.target.value)}
                    className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-grayLight rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  >
                    <option value="">Все типы</option>
                    {categories.map(category => (
                      <option key={category} value={category}>{category}</option>
                    ))}
                  </select>
                </div>
              )}

              {/* Фильтр по производителям */}
              {manufacturers.length > 0 && (
                <div>
                  <label className="block text-xs sm:text-sm font-medium text-grayDark mb-1 sm:mb-2">Производитель</label>
                  <select
                    value={selectedManufacturer}
                    onChange={(e) => setSelectedManufacturer(e.target.value)}
                    className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-grayLight rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  >
                    <option value="">Все производители</option>
                    {manufacturers.map(manufacturer => (
                      <option key={manufacturer} value={manufacturer}>{manufacturer}</option>
                    ))}
                  </select>
                </div>
              )}

              {/* Фильтр по маркам стали */}
              {steelGrades.length > 0 && (
                <div>
                  <label className="block text-xs sm:text-sm font-medium text-grayDark mb-1 sm:mb-2">Марка стали</label>
                  <select
                    value={selectedSteelGrade}
                    onChange={(e) => setSelectedSteelGrade(e.target.value)}
                    className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-grayLight rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  >
                    <option value="">Все марки</option>
                    {steelGrades.map(steelGrade => (
                      <option key={steelGrade} value={steelGrade}>{steelGrade}</option>
                    ))}
                  </select>
                </div>
              )}
            </div>

            {/* Кнопка очистки фильтров */}
            {hasActiveFilters && (
              <div className="flex justify-end">
                <button
                  onClick={handleClearFilters}
                  className="flex items-center gap-2 px-4 py-2 text-sm text-grayDark hover:text-dark border border-grayLight rounded-lg hover:bg-grayMedium transition-colors"
                >
                  <X className="w-4 h-4" />
                  Очистить фильтры
                </button>
              </div>
            )}
          </div>
        </div>

        {/* Переключатель режимов отображения */}
        <div className="flex items-center justify-between mb-4 sm:mb-6">
          <div className="flex items-center space-x-2">
            <span className="text-xs sm:text-sm font-medium text-grayDark hidden sm:inline">Режим отображения:</span>
            <div className="flex bg-grayLight rounded-lg p-1">
              <button
                onClick={() => handleViewModeChange('grid')}
                className={`flex items-center space-x-1 sm:space-x-2 px-2 sm:px-3 py-2 rounded-md text-xs sm:text-sm font-medium transition-colors ${
                  viewMode === 'grid'
                    ? 'bg-white text-primary shadow-sm'
                    : 'text-grayDark hover:text-dark'
                }`}
              >
                <Grid className="w-3 h-3 sm:w-4 sm:h-4" />
                <span className="hidden sm:inline">Сетка</span>
              </button>
              <button
                onClick={() => handleViewModeChange('list')}
                className={`flex items-center space-x-1 sm:space-x-2 px-2 sm:px-3 py-2 rounded-md text-xs sm:text-sm font-medium transition-colors ${
                  viewMode === 'list'
                    ? 'bg-white text-primary shadow-sm'
                    : 'text-grayDark hover:text-dark'
                }`}
              >
                <List className="w-3 h-3 sm:w-4 sm:h-4" />
                <span className="hidden sm:inline">Список</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Сетка товаров */}
      {filteredProducts.length > 0 ? (
        <motion.div 
          className={
            viewMode === 'grid'
              ? "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3 sm:gap-4 lg:gap-6"
              : "space-y-3 sm:space-y-4"
          }
          layout
        >
          {filteredProducts.map((product, index) => (
            <motion.div
              key={product.id}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.3, delay: index * 0.05 }}
              layout
            >
              <ProductCard
                product={product}
                viewMode={viewMode}
                cartQuantity={getCartQuantity(product.id)}
                onAddToCart={onAddToCart}
                onRemoveFromCart={onRemoveFromCart}
                onUpdateQuantity={onUpdateQuantity}
                onProductClick={handleProductClick}
              />
            </motion.div>
          ))}
        </motion.div>
      ) : (
        /* Пустое состояние */
        <motion.div 
          className="text-center py-12"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.3 }}
        >
          <div className="mx-auto w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mb-4">
            <Filter className="w-12 h-12 text-gray-400" />
          </div>
          <h3 className="mt-2 text-lg font-medium text-gray-900">Товары не найдены</h3>
          <p className="mt-1 text-sm text-gray-500">
            {hasActiveFilters 
              ? 'Попробуйте изменить параметры поиска или фильтры'
              : 'В каталоге пока нет товаров'
            }
          </p>
          {hasActiveFilters && (
            <button
              onClick={handleClearFilters}
              className="mt-4 px-4 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors"
            >
              Очистить фильтры
            </button>
          )}
        </motion.div>
      )}

        {/* Модальное окно товара */}
        <ProductModal
          product={selectedProduct}
          isOpen={isModalOpen}
          onClose={handleCloseModal}
          cartQuantity={selectedProduct ? getCartQuantity(selectedProduct.id) : 0}
          onAddToCart={onAddToCart}
          onRemoveFromCart={onRemoveFromCart}
          onUpdateQuantity={onUpdateQuantity}
        />
      </div>
    </div>
  );
}