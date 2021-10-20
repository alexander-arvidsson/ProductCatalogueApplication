using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IProductRepository
    {
        public Task<List<Product>> GetProductsAsync();
        public void AddProductAsync(Product p);
        public void RemoveProductAsync(Product p);

        public void UpdateProductAsync(Product p);

        public Task<List<Product>> DisplayZeroStockAsync();
    }
}
