using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IProductRepository
    {
        //Metod för att få alla produkter till en lista.
        Task<List<Product>> GetProductsAsync();

        //Metod för att lägga in en produkt.
        Task AddProduct(Product p);

        //Metod för att ta bort en produkt.
        Task RemoveProduct(Product p);

        //Metod för att uppdatera en produkt.
        Task UpdateProduct(Product p);

        //Metod som genererar en lista med alla produkter utan någon stock.
        Task<List<Product>> DisplayZeroStockAsync();
    }
}
