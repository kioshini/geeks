import type { 
  NomenclatureEl, 
  PricesEl, 
  RemnantsEl, 
  StockEl, 
  TypeEl,
  NomenclatureRoot,
  PricesRoot,
  RemnantsRoot,
  StocksRoot,
  TypesRoot
} from './api';
import type { Product } from '../types/catalog';

/**
 * Адаптер для преобразования JSON данных в формат каталога
 * Обеспечивает интеграцию с новым backend и поддержку дельтовых обновлений
 */

/**
 * Кэш для хранения данных JSON
 */
let nomenclatureCache: NomenclatureEl[] = [];
let pricesCache: PricesEl[] = [];
let remnantsCache: RemnantsEl[] = [];
let stocksCache: StockEl[] = [];
// let typesCache: TypeEl[] = []; // Пока не используется

/**
 * Обновляет кэш данных из JSON
 */
export function updateJsonDataCache(
  nomenclature: NomenclatureEl[],
  prices: PricesEl[],
  remnants: RemnantsEl[],
  stocks: StockEl[],
  _types: TypeEl[] // Пока не используется
) {
  nomenclatureCache = nomenclature;
  pricesCache = prices;
  remnantsCache = remnants;
  stocksCache = stocks;
  // typesCache = _types; // Пока не используется
}

/**
 * Применяет дельту к цене
 * @param currentPrice - Текущая цена
 * @param delta - Дельта (может быть отрицательной)
 * @returns Новая цена (минимум 0)
 */
export function applyPriceDelta(currentPrice: number, delta: number): number {
  return Math.max(0, currentPrice + delta);
}

/**
 * Применяет дельту к остатку
 * @param currentStock - Текущий остаток
 * @param delta - Дельта (может быть отрицательной)
 * @returns Новый остаток (минимум 0)
 */
export function applyStockDelta(currentStock: number, delta: number): number {
  return Math.max(0, currentStock + delta);
}

/**
 * Обновляет цены товаров с учетом дельт
 * @param priceDeltas - Массив дельт цен
 */
export function updatePricesWithDeltas(priceDeltas: PricesEl[]) {
  priceDeltas.forEach(delta => {
    const nomenclatureIndex = nomenclatureCache.findIndex(n => n.ID === delta.ID);
    if (nomenclatureIndex !== -1) {
      // Обновляем цены в кэше номенклатуры
      // В реальной реализации здесь должна быть логика обновления цен
      console.log(`Обновление цен для товара ${delta.ID}: PriceT=${delta.PriceT}, PriceM=${delta.PriceM}`);
    }
  });
}

/**
 * Обновляет остатки товаров с учетом дельт
 * @param remnantDeltas - Массив дельт остатков
 */
export function updateRemnantsWithDeltas(remnantDeltas: RemnantsEl[]) {
  remnantDeltas.forEach(delta => {
    const nomenclatureIndex = nomenclatureCache.findIndex(n => n.ID === delta.ID);
    if (nomenclatureIndex !== -1) {
      // Обновляем остатки в кэше номенклатуры
      // В реальной реализации здесь должна быть логика обновления остатков
      console.log(`Обновление остатков для товара ${delta.ID}: InStockT=${delta.InStockT}, InStockM=${delta.InStockM}`);
    }
  });
}

// Вспомогательные функции для поиска данных (пока не используются)
// function findStockById(stockId: string): StockEl | null {
//   return stocksCache.find(stock => stock.IDStock === stockId) || null;
// }

// function findTypeById(typeId: string): TypeEl | null {
//   return typesCache.find(type => type.IDType === typeId) || null;
// }

/**
 * Находит цену товара по ID номенклатуры и склада
 * @param nomenclatureId - ID номенклатуры
 * @param stockId - ID склада
 * @returns Данные цены или null
 */
function findPriceById(nomenclatureId: string, stockId: string): PricesEl | null {
  return pricesCache.find(price => price.ID === nomenclatureId && price.IDStock === stockId) || null;
}

/**
 * Находит остаток товара по ID номенклатуры и склада
 * @param nomenclatureId - ID номенклатуры
 * @param stockId - ID склада
 * @returns Данные остатка или null
 */
function findRemnantById(nomenclatureId: string, stockId: string): RemnantsEl | null {
  return remnantsCache.find(remnant => remnant.ID === nomenclatureId && remnant.IDStock === stockId) || null;
}

/**
 * Преобразует данные номенклатуры в товар каталога
 * @param nomenclature - Данные номенклатуры
 * @returns Товар каталога
 */
function nomenclatureToProduct(nomenclature: NomenclatureEl): Product {
  // Находим первый склад для демонстрации (в реальной системе может быть логика выбора)
  const firstStock = stocksCache[0];
  const stockId = firstStock?.IDStock || '1';
  
  // Находим цену и остаток для данного товара и склада
  const price = findPriceById(nomenclature.ID, stockId);
  const remnant = findRemnantById(nomenclature.ID, stockId);
  
  // Находим тип товара (пока не используется)
  // const type = findTypeById(nomenclature.IDType);
  
  return {
    id: parseInt(nomenclature.ID, 10),
    name: nomenclature.Name,
    gost: nomenclature.Gost,
    manufacturer: nomenclature.Manufacturer,
    steelGrade: nomenclature.SteelGrade,
    diameter: nomenclature.Diameter,
    pipeWallThickness: nomenclature.PipeWallThickness,
    inStockT: remnant?.InStockT || 0,
    inStockM: remnant?.InStockM || 0,
    priceT: price?.PriceT || 0,
    priceM: price?.PriceM || 0,
    stockName: firstStock?.StockName || 'Не указан',
    address: firstStock?.Address || 'Не указан',
    schedule: firstStock?.Schedule || 'Не указан',
    cashPayment: firstStock?.CashPayment || false,
    cardPayment: firstStock?.CardPayment || false,
    avgTubeLength: remnant?.AvgTubeLength || 0,
    avgTubeWeight: remnant?.AvgTubeWeight || 0,
    ownerShortName: firstStock?.OwnerShortName || 'Не указан',
    productionType: nomenclature.ProductionType,
    formOfLength: nomenclature.FormOfLength,
    status: nomenclature.Status,
    koef: nomenclature.Koef
  };
}

/**
 * Преобразует JSON данные в массив товаров каталога
 * @param nomenclatureRoot - Корневой объект номенклатуры
 * @param pricesRoot - Корневой объект цен
 * @param remnantsRoot - Корневой объект остатков
 * @param stocksRoot - Корневой объект складов
 * @param typesRoot - Корневой объект типов
 * @returns Массив товаров каталога
 */
export function adaptJsonDataToProducts(
  nomenclatureRoot: NomenclatureRoot,
  pricesRoot: PricesRoot,
  remnantsRoot: RemnantsRoot,
  stocksRoot: StocksRoot,
  typesRoot: TypesRoot
): Product[] {
  // Обновляем кэш
  updateJsonDataCache(
    nomenclatureRoot.ArrayOfNomenclatureEl,
    pricesRoot.ArrayOfPricesEl,
    remnantsRoot.ArrayOfRemnantsEl,
    stocksRoot.ArrayOfStockEl,
    typesRoot.ArrayOfTypeEl
  );

  // Преобразуем номенклатуру в товары
  return nomenclatureRoot.ArrayOfNomenclatureEl.map(nomenclatureToProduct);
}

/**
 * Получает обновленные товары с учетом дельт
 * @param priceDeltas - Дельты цен
 * @param remnantDeltas - Дельты остатков
 * @returns Обновленный массив товаров
 */
export function getUpdatedProductsWithDeltas(
  priceDeltas: PricesEl[],
  remnantDeltas: RemnantsEl[]
): Product[] {
  // Обновляем кэш с дельтами
  updatePricesWithDeltas(priceDeltas);
  updateRemnantsWithDeltas(remnantDeltas);

  // Возвращаем обновленные товары
  return nomenclatureCache.map(nomenclatureToProduct);
}

/**
 * Получает уникальные категории из номенклатуры
 * @returns Массив уникальных категорий
 */
export function getUniqueCategories(): string[] {
  const categories = Array.from(new Set(nomenclatureCache.map(n => n.ProductionType).filter(Boolean)));
  return categories.sort();
}

/**
 * Получает уникальных производителей из номенклатуры
 * @returns Массив уникальных производителей
 */
export function getUniqueManufacturers(): string[] {
  const manufacturers = Array.from(new Set(nomenclatureCache.map(n => n.Manufacturer).filter(Boolean)));
  return manufacturers.sort();
}

/**
 * Получает уникальные марки стали из номенклатуры
 * @returns Массив уникальных марок стали
 */
export function getUniqueSteelGrades(): string[] {
  const steelGrades = Array.from(new Set(nomenclatureCache.map(n => n.SteelGrade).filter(Boolean)));
  return steelGrades.sort();
}

/**
 * Инициализирует данные из JSON файлов
 * @returns Promise с массивом товаров
 */
export async function initializeJsonData(): Promise<Product[]> {
  try {
    // В реальной реализации здесь должны быть вызовы API
    // Пока что возвращаем пустой массив
    console.log('Инициализация JSON данных...');
    return [];
  } catch (error) {
    console.error('Ошибка при инициализации JSON данных:', error);
    return [];
  }
}
