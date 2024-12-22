using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using CRUDApp.Models;

namespace CRUDApp.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly string connectionString;
        
        [BindProperty]
        public Product Product { get; set; } = new Product(); // Initialize here
        public List<Product> Products { get; set; } = new List<Product>(); // Initialize here

        public IndexModel(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            // Initialize Product if it hasn't been initialized yet
            if (Product == null)
            {
                Product = new Product();
            }
        }

        public void OnGet()
        {
            LoadProducts();
        }

        public IActionResult OnPostCreate()
        {
            if (!ModelState.IsValid) return Page();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Products (ProductName, Price, Quantity) VALUES (@ProductName, @Price, @Quantity)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ProductName", Product.ProductName ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Price", Product.Price);
                        cmd.Parameters.AddWithValue("@Quantity", Product.Quantity);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        TempData["SuccessMessage"] = "Product added successfully!";
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Error adding product: {ex.Message}";
                    }
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid) return Page();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Products SET ProductName=@ProductName, Price=@Price, Quantity=@Quantity WHERE ProductID=@ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ProductID", Product.ProductID);
                        cmd.Parameters.AddWithValue("@ProductName", Product.ProductName ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Price", Product.Price);
                        cmd.Parameters.AddWithValue("@Quantity", Product.Quantity);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        TempData["SuccessMessage"] = "Product updated successfully!";
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Error updating product: {ex.Message}";
                    }
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE ProductID=@ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ProductID", id);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        TempData["SuccessMessage"] = "Product deleted successfully!";
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Error deleting product: {ex.Message}";
                    }
                }
            }

            return RedirectToPage();
        }

        private void LoadProducts()
        {
            Products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products", conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    Quantity = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Error loading products: {ex.Message}";
                    }
                }
            }
        }
    }
}