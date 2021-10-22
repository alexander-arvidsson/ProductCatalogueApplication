using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface ICustomerRepository
    {
        Task AddCustomer(Customer customer);
        Task RemoveCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        Task<List<Customer>> GetCustomersAsync();
    }
}
