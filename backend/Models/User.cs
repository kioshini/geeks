using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models
{
    public class User
    {
        public long Id { get; set; }
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? Username { get; set; }
        
        public string? LanguageCode { get; set; }
        
        public bool IsBot { get; set; }
        
        public string? Phone { get; set; }
        
        public string? Email { get; set; }
        
        public string? INN { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
    }
}
