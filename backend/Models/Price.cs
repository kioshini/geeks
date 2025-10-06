namespace TMKMiniApp.Models
{
    public class Price
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        public decimal Value { get; set; }
        
        public string? Currency { get; set; } = "RUB";
        
        public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
        
        public DateTime? ValidTo { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
