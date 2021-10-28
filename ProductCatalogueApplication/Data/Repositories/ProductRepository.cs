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

       
        /// <summary>
        /// A method that fetches all the products stored in the database.
        /// </summary>
        /// <returns>A list of products</returns>
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// A method that adds products to the database.
        /// </summary>
        /// <param name="p">A specific product</param>
        /// <returns>No return value in async task method</returns>
        public async Task AddProduct(Product p)
        {
            await _context.Products.AddAsync(p);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that removes products from the database.
        /// </summary>
        /// <param name="p">A specific product</param>
        /// <returns>No return value in async task method</returns>
        public async Task RemoveProduct(Product p)
        {
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that updates the values of a product.
        /// </summary>
        /// <param name="p">A specific product</param>
        /// <returns>No return value in async task method</returns>
        public async Task UpdateProduct(Product p)
        {
            _context.Products.Update(p);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that displayes all the products where the stock number is 0.
        /// </summary>
        /// <returns>No return value in async task method</returns>
        public async Task<List<Product>> DisplayZeroStockAsync()
        {
            return await _context.Products.Where(p => p.Stock == 0).ToListAsync();
        }
    }
}