using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IProductRepository
    {
        public Task<List<Product>> GetProductsAsync();
        public void AddProduct(Product p);
        public void RemoveProduct(Product p);

        public void UpdateProduct(Product p);

        public Task<List<Product>> DisplayZeroStockAsync();
    }
}
