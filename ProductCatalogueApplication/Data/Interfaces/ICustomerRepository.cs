using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface ICustomerRepository
    {
        void AddCustomer(Customer customer);
        void RemoveCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        public Task<List<Customer>> DisplayArchivedCustomerOrder(Order order);
        public Task<List<Customer>> DisplayActiveCustomerOrder(Order order);
    }
}
