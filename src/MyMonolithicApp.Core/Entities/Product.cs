namespace MyMonolithicApp.Core.Entities
{
    public class Product : Entity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Business rules
        public bool IsInStock => StockQuantity > 0;
        public bool IsAvailable => IsActive && IsInStock;

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative.", nameof(quantity));
            
            StockQuantity = quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be positive.", nameof(newPrice));
            
            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}