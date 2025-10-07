import { Api } from './api';
import type { PricesEl, RemnantsEl } from './api';

/**
 * Сервис для работы с дельтовыми обновлениями цен и остатков
 * Обеспечивает реальное время обновления данных без перезагрузки страницы
 */

export class DeltaUpdatesService {
  private static instance: DeltaUpdatesService;
  private updateInterval: number | null = null;
  private isMonitoring = false;

  private constructor() {}

  /**
   * Получает экземпляр сервиса (Singleton)
   */
  public static getInstance(): DeltaUpdatesService {
    if (!DeltaUpdatesService.instance) {
      DeltaUpdatesService.instance = new DeltaUpdatesService();
    }
    return DeltaUpdatesService.instance;
  }

  /**
   * Запускает мониторинг дельтовых обновлений
   * @param intervalMs - Интервал проверки в миллисекундах (по умолчанию 30 секунд)
   */
  public startMonitoring(intervalMs: number = 30000): void {
    if (this.isMonitoring) {
      console.log('Мониторинг дельтовых обновлений уже запущен');
      return;
    }

    console.log(`Запуск мониторинга дельтовых обновлений с интервалом ${intervalMs}ms`);
    this.isMonitoring = true;

    // Запускаем периодическую проверку
    this.updateInterval = setInterval(async () => {
      try {
        await this.checkForUpdates();
      } catch (error) {
        console.error('Ошибка при проверке обновлений:', error);
      }
    }, intervalMs);

    // Проверяем сразу при запуске
    this.checkForUpdates();
  }

  /**
   * Останавливает мониторинг дельтовых обновлений
   */
  public stopMonitoring(): void {
    if (!this.isMonitoring) {
      console.log('Мониторинг дельтовых обновлений не запущен');
      return;
    }

    console.log('Остановка мониторинга дельтовых обновлений');
    this.isMonitoring = false;

    if (this.updateInterval) {
      clearInterval(this.updateInterval);
      this.updateInterval = null;
    }
  }

  /**
   * Проверяет наличие новых обновлений
   */
  private async checkForUpdates(): Promise<void> {
    try {
      // В реальной реализации здесь должен быть API для получения дельт
      // Пока что используем тестовые данные
      console.log('Проверка дельтовых обновлений...');
      
      // Симулируем получение дельт (в реальной системе это будет API вызов)
      const mockPriceDeltas: PricesEl[] = [];
      const mockRemnantDeltas: RemnantsEl[] = [];

      if (mockPriceDeltas.length > 0 || mockRemnantDeltas.length > 0) {
        console.log(`Получены дельты: ${mockPriceDeltas.length} цен, ${mockRemnantDeltas.length} остатков`);
        this.notifyUpdateListeners(mockPriceDeltas, mockRemnantDeltas);
      }
    } catch (error) {
      console.error('Ошибка при проверке обновлений:', error);
    }
  }

  /**
   * Обработчики обновлений
   */
  private updateListeners: Array<(priceDeltas: PricesEl[], remnantDeltas: RemnantsEl[]) => void> = [];

  /**
   * Добавляет обработчик обновлений
   * @param listener - Функция-обработчик
   */
  public addUpdateListener(listener: (priceDeltas: PricesEl[], remnantDeltas: RemnantsEl[]) => void): void {
    this.updateListeners.push(listener);
  }

  /**
   * Удаляет обработчик обновлений
   * @param listener - Функция-обработчик
   */
  public removeUpdateListener(listener: (priceDeltas: PricesEl[], remnantDeltas: RemnantsEl[]) => void): void {
    const index = this.updateListeners.indexOf(listener);
    if (index > -1) {
      this.updateListeners.splice(index, 1);
    }
  }

  /**
   * Уведомляет всех слушателей об обновлениях
   * @param priceDeltas - Дельты цен
   * @param remnantDeltas - Дельты остатков
   */
  private notifyUpdateListeners(priceDeltas: PricesEl[], remnantDeltas: RemnantsEl[]): void {
    this.updateListeners.forEach(listener => {
      try {
        listener(priceDeltas, remnantDeltas);
      } catch (error) {
        console.error('Ошибка в обработчике обновлений:', error);
      }
    });
  }

  /**
   * Тестирует дельту цены
   * @param currentValue - Текущее значение
   * @param delta - Дельта
   * @returns Результат тестирования
   */
  public async testPriceDelta(currentValue: number, delta: number): Promise<{currentValue: number, delta: number, newValue: number, message: string}> {
    try {
      return await Api.testPriceDelta(currentValue, delta);
    } catch (error) {
      console.error('Ошибка при тестировании дельты цены:', error);
      throw error;
    }
  }

  /**
   * Тестирует дельту остатка
   * @param currentValue - Текущее значение
   * @param delta - Дельта
   * @returns Результат тестирования
   */
  public async testStockDelta(currentValue: number, delta: number): Promise<{currentValue: number, delta: number, newValue: number, message: string}> {
    try {
      return await Api.testStockDelta(currentValue, delta);
    } catch (error) {
      console.error('Ошибка при тестировании дельты остатка:', error);
      throw error;
    }
  }

  /**
   * Обрабатывает файл с дельтами цен вручную
   * @param filePath - Путь к файлу
   * @returns Результат обработки
   */
  public async processPricesFile(filePath: string): Promise<{message: string}> {
    try {
      return await Api.processPricesFile(filePath);
    } catch (error) {
      console.error('Ошибка при обработке файла цен:', error);
      throw error;
    }
  }

  /**
   * Обрабатывает файл с дельтами остатков вручную
   * @param filePath - Путь к файлу
   * @returns Результат обработки
   */
  public async processRemnantsFile(filePath: string): Promise<{message: string}> {
    try {
      return await Api.processRemnantsFile(filePath);
    } catch (error) {
      console.error('Ошибка при обработке файла остатков:', error);
      throw error;
    }
  }

  /**
   * Запускает мониторинг на backend
   * @returns Результат запуска
   */
  public async startBackendMonitoring(): Promise<{message: string}> {
    try {
      return await Api.startDeltaMonitoring();
    } catch (error) {
      console.error('Ошибка при запуске мониторинга на backend:', error);
      throw error;
    }
  }

  /**
   * Останавливает мониторинг на backend
   * @returns Результат остановки
   */
  public async stopBackendMonitoring(): Promise<{message: string}> {
    try {
      return await Api.stopDeltaMonitoring();
    } catch (error) {
      console.error('Ошибка при остановке мониторинга на backend:', error);
      throw error;
    }
  }

  /**
   * Получает статус мониторинга
   * @returns true если мониторинг активен
   */
  public isMonitoringActive(): boolean {
    return this.isMonitoring;
  }

  /**
   * Очищает ресурсы при уничтожении сервиса
   */
  public destroy(): void {
    this.stopMonitoring();
    this.updateListeners = [];
  }
}

// Экспортируем экземпляр сервиса
export const deltaUpdatesService = DeltaUpdatesService.getInstance();
