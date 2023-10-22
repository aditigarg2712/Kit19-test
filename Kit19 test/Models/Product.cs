using System.ComponentModel.DataAnnotations;

namespace Kit19_test.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public DateTime MfgDate { get; set; }
        public string Category { get; set; }
    }
}
