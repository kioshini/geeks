import type { ProductDto } from './api';
import type { Product } from '../types/catalog';

/**
 * Адаптер для преобразования ProductDto в Product
 * Преобразует данные из API в формат, используемый новым каталогом
 */
export function adaptProductDtoToProduct(dto: ProductDto): Product {
  return {
    id: dto.id,
    name: dto.name,
    gost: 'Не указан', // Заглушка, так как gost нет в ProductDto
    manufacturer: 'Не указан', // Заглушка, так как manufacturer нет в ProductDto
    steelGrade: dto.material || 'Не указан',
    diameter: dto.diameter || 0,
    pipeWallThickness: dto.thickness || 0,
    inStockT: dto.stockQuantity || 0,
    inStockM: dto.stockQuantity || 0, // Для демонстрации используем то же значение
    priceT: dto.price || 0, // Цена за тонну (основная цена)
    priceM: dto.price ? dto.price * 0.1 : 0, // Цена за метр (примерно 10% от цены за тонну)
    stockName: 'Екатеринбург', // Заглушка
    address: 'поселок Шувакиш, ул Зеленая, 50а', // Заглушка
    schedule: 'пн - чт 08.00-17.00, пт 08.00-15.45', // Заглушка
    cashPayment: true,
    cardPayment: true,
    avgTubeLength: 12.0, // Заглушка
    avgTubeWeight: 45.2, // Заглушка
    ownerShortName: 'ТМК',
    productionType: dto.type || 'Не указан',
    formOfLength: 'НК', // Заглушка
    status: dto.isAvailable ? 1 : 0,
    koef: 0.011782032 // Заглушка
  };
}

/**
 * Преобразует массив ProductDto в массив Product
 */
export function adaptProductDtosToProducts(dtos: ProductDto[]): Product[] {
  return dtos.map(adaptProductDtoToProduct);
}
