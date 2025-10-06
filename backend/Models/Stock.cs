namespace TMKMiniApp.Models
{
    public class Stock
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        public int Quantity { get; set; }
        
        public int ReservedQuantity { get; set; }
        
        public int AvailableQuantity => Quantity - ReservedQuantity;
        
        public string? Location { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
