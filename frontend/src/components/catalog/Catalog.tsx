import { useState, useMemo, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Grid, List, Search, Filter, X } from 'lucide-react';
import type { CatalogProps, Product, ViewMode } from '../../types/catalog';
import { ProductCard } from './ProductCard';
import { ProductModal } from './ProductModal';
import { useUnit } from '../../contexts/UnitContext';

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
  cartItems,
  resetFiltersTrigger = 0
}: CatalogProps) {
  const [viewMode, setViewMode] = useState<ViewMode>('grid');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [selectedManufacturer, setSelectedManufacturer] = useState<string>('');
  const [selectedSteelGrade, setSelectedSteelGrade] = useState<string>('');
  const [selectedStock, setSelectedStock] = useState<string>('');
  const [selectedDiameter, setSelectedDiameter] = useState<string>('');
  const [selectedThickness, setSelectedThickness] = useState<string>('');
  const [selectedGost, setSelectedGost] = useState<string>('');
  const { selectedUnit, setSelectedUnit } = useUnit();

  // Функция для сброса всех фильтров
  const resetFilters = () => {
    setSearchQuery('');
    setSelectedCategory('');
    setSelectedManufacturer('');
    setSelectedSteelGrade('');
    setSelectedStock('');
    setSelectedDiameter('');
    setSelectedThickness('');
    setSelectedGost('');
  };

  // Сбрасываем фильтры при изменении списка продуктов или при срабатывании триггера
  useEffect(() => {
    // Сбрасываем фильтры только если продукты изменились и не пустые
    if (products.length > 0) {
      resetFilters();
    }
  }, [products.length, resetFiltersTrigger]); // Срабатывает при изменении количества продуктов или триггера

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

  const stocks = useMemo(() => {
    const uniqueStocks = Array.from(new Set(products.map(p => p.stockName).filter(Boolean)));
    return uniqueStocks.sort();
  }, [products]);

  const diameters = useMemo(() => {
    const uniqueDiameters = Array.from(new Set(products.map(p => p.diameter).filter(d => d > 0)));
    return uniqueDiameters.sort((a, b) => a - b);
  }, [products]);

  const thicknesses = useMemo(() => {
    const uniqueThicknesses = Array.from(new Set(products.map(p => p.pipeWallThickness).filter(t => t > 0)));
    return uniqueThicknesses.sort((a, b) => a - b);
  }, [products]);

  const gosts = useMemo(() => {
    const uniqueGosts = Array.from(new Set(products.map(p => p.gost).filter(Boolean)));
    return uniqueGosts.sort();
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
      const matchesStock = !selectedStock || product.stockName === selectedStock;
      const matchesDiameter = !selectedDiameter || product.diameter === parseFloat(selectedDiameter);
      const matchesThickness = !selectedThickness || product.pipeWallThickness === parseFloat(selectedThickness);
      const matchesGost = !selectedGost || product.gost === selectedGost;
      
      return matchesSearch && matchesCategory && matchesManufacturer && matchesSteelGrade && 
             matchesStock && matchesDiameter && matchesThickness && matchesGost;
    });
  }, [products, searchQuery, selectedCategory, selectedManufacturer, selectedSteelGrade, selectedStock, selectedDiameter, selectedThickness, selectedGost]);


  // Получаем количество конкретного товара в корзине
  const getCartQuantity = (productId: number): number => {
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
    resetFilters();
  };

  const hasActiveFilters = searchQuery || selectedCategory || selectedManufacturer || selectedSteelGrade || 
                          selectedStock || selectedDiameter || selectedThickness || selectedGost;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-2 sm:px-4 lg:px-8 py-4 sm:py-6 lg:py-8">
      {/* Заголовок */}
      <div className="mb-4 sm:mb-6 lg:mb-8">
        <h1 className="text-[#171A1F] text-2xl font-semibold mb-4">Каталог труб</h1>
        <p className="text-sm sm:text-base text-[#565D6D] mb-4">
          Найдено товаров: {filteredProducts.length} из {products.length}
        </p>

        {/* Панель управления - поиск и фильтры в одну строку */}
        <div className="bg-white rounded-lg shadow-lg border border-gray-300 p-2 sm:p-4 mb-4 sm:mb-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            {/* Поиск */}
            <div className="relative flex-1 max-w-md">
              <div className="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                <Search className="h-4 w-4 sm:h-5 sm:w-5 text-[#565D6D]" />
              </div>
              <input
                type="text"
                placeholder="Поиск по названию, производителю..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-3 sm:pl-4 pr-9 sm:pr-10 py-2 sm:py-3 text-sm sm:text-base text-[#565D6D] border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
              />
            </div>

            {/* Фильтры и единицы измерения */}
            <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center">
              {/* Фильтры */}
              <div className="flex flex-wrap gap-2 sm:gap-4">
                {/* Фильтр по категориям */}
                {categories.length > 0 && (
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">Тип производства</label>
                    <select
                      value={selectedCategory}
                      onChange={(e) => setSelectedCategory(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
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
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">Производитель</label>
                    <select
                      value={selectedManufacturer}
                      onChange={(e) => setSelectedManufacturer(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
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
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">Марка стали</label>
                    <select
                      value={selectedSteelGrade}
                      onChange={(e) => setSelectedSteelGrade(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
                    >
                      <option value="">Все марки</option>
                      {steelGrades.map(steelGrade => (
                        <option key={steelGrade} value={steelGrade}>{steelGrade}</option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Фильтр по складам */}
                {stocks.length > 0 && (
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">Склад</label>
                    <select
                      value={selectedStock}
                      onChange={(e) => setSelectedStock(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
                    >
                      <option value="">Все склады</option>
                      {stocks.map(stock => (
                        <option key={stock} value={stock}>{stock}</option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Фильтр по диаметру */}
                {diameters.length > 0 && (
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">Диаметр (мм)</label>
                    <select
                      value={selectedDiameter}
                      onChange={(e) => setSelectedDiameter(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
                    >
                      <option value="">Все диаметры</option>
                      {diameters.map(diameter => (
                        <option key={diameter} value={diameter}>{diameter} мм</option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Фильтр по толщине стенки */}
                {thicknesses.length > 0 && (
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">Толщина (мм)</label>
                    <select
                      value={selectedThickness}
                      onChange={(e) => setSelectedThickness(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
                    >
                      <option value="">Все толщины</option>
                      {thicknesses.map(thickness => (
                        <option key={thickness} value={thickness}>{thickness} мм</option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Фильтр по ГОСТ/ТУ */}
                {gosts.length > 0 && (
                  <div className="min-w-[150px]">
                    <label className="block text-xs sm:text-sm font-medium text-[#171A1F] mb-1 sm:mb-2">ГОСТ/ТУ</label>
                    <select
                      value={selectedGost}
                      onChange={(e) => setSelectedGost(e.target.value)}
                      className="w-full px-2 sm:px-3 py-2 text-sm sm:text-base border border-[#DEE1E6] rounded-md focus:outline-none focus:ring-1 focus:ring-[#171A1F]"
                    >
                      <option value="">Все стандарты</option>
                      {gosts.map(gost => (
                        <option key={gost} value={gost}>{gost}</option>
                      ))}
                    </select>
                  </div>
                )}
              </div>

              {/* Выбор единиц измерения */}
              <div className="flex flex-col sm:flex-row gap-2">
                <span className="text-xs sm:text-sm font-medium text-[#171A1F] whitespace-nowrap">Единицы:</span>
                <div className="flex bg-[#F3F4F6] border border-[#F3F4F6] rounded-lg p-1">
                  <button
                    onClick={() => setSelectedUnit('т')}
                    className={`px-3 py-1 text-sm font-medium rounded-md transition-colors ${
                      selectedUnit === 'т'
                        ? 'bg-white text-[#171A1F] shadow-sm border border-[#F3F4F6]'
                        : 'text-[#565D6D] hover:text-[#171A1F]'
                    }`}
                  >
                    Тонны
                  </button>
                  <button
                    onClick={() => setSelectedUnit('м')}
                    className={`px-3 py-1 text-sm font-medium rounded-md transition-colors ${
                      selectedUnit === 'м'
                        ? 'bg-white text-[#171A1F] shadow-sm border border-[#F3F4F6]'
                        : 'text-[#565D6D] hover:text-[#171A1F]'
                    }`}
                  >
                    Метры
                  </button>
                </div>
              </div>
            </div>

          </div>

          {/* Кнопка очистки фильтров */}
          {hasActiveFilters && (
            <div className="flex justify-end mt-4">
              <button
                onClick={handleClearFilters}
                className="flex items-center gap-2 px-4 py-2 text-sm text-[#565D6D] hover:text-[#171A1F] border border-[#DEE1E6] rounded-md hover:bg-[#F3F4F6] transition-colors"
              >
                <X className="w-4 h-4" />
                Очистить фильтры
              </button>
            </div>
          )}
        </div>

        {/* Переключатель режимов отображения */}
        <div className="flex items-center justify-between mb-4 sm:mb-6">
          <div className="flex items-center space-x-2">
            <span className="text-xs sm:text-sm font-medium text-[#565D6D] hidden sm:inline">Режим отображения:</span>
            <div className="flex bg-[#F3F4F6] border border-[#F3F4F6] rounded-lg p-1">
              <button
                onClick={() => handleViewModeChange('grid')}
                className={`flex items-center space-x-1 sm:space-x-2 px-2 sm:px-3 py-2 rounded-md text-xs sm:text-sm font-medium transition-colors ${
                  viewMode === 'grid'
                    ? 'bg-white text-[#171A1F] shadow-sm'
                    : 'text-[#565D6D] hover:text-[#171A1F]'
                }`}
              >
                <Grid className="w-3 h-3 sm:w-4 sm:h-4" />
                <span className="hidden sm:inline">Сетка</span>
              </button>
              <button
                onClick={() => handleViewModeChange('list')}
                className={`flex items-center space-x-1 sm:space-x-2 px-2 sm:px-3 py-2 rounded-md text-xs sm:text-sm font-medium transition-colors ${
                  viewMode === 'list'
                    ? 'bg-white text-[#171A1F] shadow-sm'
                    : 'text-[#565D6D] hover:text-[#171A1F]'
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
      </div>

      {/* Модальное окно товара - вынесено за пределы основного контейнера */}
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
  );
}