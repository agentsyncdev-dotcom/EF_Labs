using System.ComponentModel.DataAnnotations;

namespace EF_Lab.Models
{
    public class Product
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Category { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a positive number.")]
        public int StockQuantity { get; set; }
    }
}
