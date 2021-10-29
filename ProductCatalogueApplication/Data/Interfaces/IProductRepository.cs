using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IProductRepository
    {
        /// <summary>
        /// Method to get all the products in a list.
        /// </summary>
        Task<List<Product>> GetProductsAsync();

        /// <summary>
        /// Method for adding a product.
        /// </summary>
        Task AddProduct(Product p);

        /// <summary>
        /// Method for deleting a product.
        /// </summary>
        Task RemoveProduct(Product p);

        /// <summary>
        /// Method for updating a product
        /// </summary>
        Task UpdateProduct(Product p);

        /// <summary>
        /// Method that generates a list of products without stock.
        /// </summary>
        Task<List<Product>> DisplayZeroStockAsync();
    }
}
