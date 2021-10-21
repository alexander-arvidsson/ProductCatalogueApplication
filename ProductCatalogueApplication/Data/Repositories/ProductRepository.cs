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

        //H�r kan vi l�gga in metoder som r�r Customer, bara f�r start en metod f�r att h�mta alla customers som finns lagrade 
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public void AddProduct(Product p)
        {
            _context.Products.Add(p);
            _context.SaveChanges();
        }
        public void RemoveProduct(Product p)
        {
            _context.Products.Remove(p);
            _context.SaveChanges();
        }
        public void UpdateProduct(Product p)
        {
            _context.Products.Update(p);
            _context.SaveChanges();
        }
        public async Task<List<Product>> DisplayZeroStockAsync()
        {
            return await _context.Products.Where(p => p.Stock == 0).ToListAsync();
        }
    }
}
