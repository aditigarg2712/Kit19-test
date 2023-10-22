using Kit19_test.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductSearchView
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Size { get; set; }
    public decimal? Price { get; set; }
    public DateTime? MfgDate { get; set; }
    public string Category { get; set; }
    public string SelectedConjunction { get; set; }
    public List<SelectListItem> Conjunctions { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "AND", Text = "AND" },
        new SelectListItem { Value = "OR", Text = "OR" },
    };
    public List<Product> SearchResults { get; set; }
}
