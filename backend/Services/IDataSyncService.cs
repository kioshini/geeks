using TMKMiniApp.Models;

namespace TMKMiniApp.Services
{
    public interface IDataSyncService
    {
        Task SyncNomenclatureAsync(string jsonData);
        Task SyncPricesAsync(string jsonData);
        Task SyncRemnantsAsync(string jsonData);
        Task SyncStocksAsync(string jsonData);
        Task SyncTypesAsync(string jsonData);
        Task<bool> IsDataUpToDateAsync();
        Task<DateTime> GetLastSyncTimeAsync();
    }
}
