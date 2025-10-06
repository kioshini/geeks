namespace TMKMiniApp.Models
{
    public class Remnant
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        public int Quantity { get; set; }
        
        public string? Location { get; set; }
        
        public string? Condition { get; set; }
        
        public string? Notes { get; set; }
        
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
