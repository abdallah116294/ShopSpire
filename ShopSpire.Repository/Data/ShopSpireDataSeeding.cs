using ShopSpireCore.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopSpire.Repository.Data
{
    public static class ShopSpireDataSeeding
    {
        public static async Task SeedAsync(ShopSpireDbContext context)
        {
            try
            {
                await SeedCategories(context);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error during data seeding: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        public static async Task SeedCategories(ShopSpireDbContext context)
        {
            try
            {
                if (!context.Categories.Any())
                {
                    var filePath = GetJsonFilePath("categories.json");
                    var CategoriesData = await File.ReadAllTextAsync(filePath);
                    // Deserialize the JSON data
                   
                    var categories = JsonSerializer.Deserialize<IEnumerable<Category>>(CategoriesData);
                    if (categories != null&&categories.Any()) 
                    {
                        await context.Categories.AddRangeAsync(categories);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        Console.WriteLine("No Categoried data found in JSON file.");
                    }

                }
                else
                {
                    Console.WriteLine("Categories data already exists. Skipping seeding.");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Categories.json file not found. Please check the file path.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
        }
        private static string GetJsonFilePath(string fileName)
        {
            // Try multiple possible paths
            var possiblePaths = new[]
            {
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "DataSeed", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "ShopSpire.Repository", "Data", "DataSeed", fileName),
            Path.Combine(AppContext.BaseDirectory, "Data", "DataSeed", fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "ShopSpire.Repository", "Data", "DataSeed", fileName)
        };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    Console.WriteLine($"Found JSON file at: {path}");
                    return path;
                }
            }

            Console.WriteLine($"JSON file '{fileName}' not found in any of the expected locations:");
            foreach (var path in possiblePaths)
            {
                Console.WriteLine($"  - {path}");
            }

            return possiblePaths[0]; // Return first path as default
        }
    }
}
