using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IProductRepository
    {
        //Metod för att få alla produkter till en lista.
        public Task<List<Product>> GetProductsAsync();

        //Metod för att lägga in en produkt.
        public void AddProduct(Product p);

        //Metod för att ta bort en produkt.
        public void RemoveProduct(Product p);

        //Metod för att uppdatera en produkt.
        public void UpdateProduct(Product p);

        //Metod som genererar en lista med alla produkter utan någon stock.
        public Task<List<Product>> DisplayZeroStockAsync();
    }
}
