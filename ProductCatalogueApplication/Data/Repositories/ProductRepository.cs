using Microsoft.EntityFrameworkCore;
using ProductCatalogueApplication.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data
{
    public class ProductRepository : IProductRepository
    {

        private readonly WarehouseAutomationContext _context;


        public ProductRepository(WarehouseAutomationContext context)
        {
            _context = context;
        }

        //Här kan vi lägga in metoder som rör Customer, bara för start en metod för att hämta alla customers som finns lagrade 
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task AddProduct(Product p)
        {
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveProduct(Product p)
        {
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateProduct(Product p)
        {
            _context.Products.Update(p);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> DisplayZeroStockAsync()
        {
            return await _context.Products.Where(p => p.Stock == 0).ToListAsync();
        }
    }
}
