using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data.Interfaces
{
    interface ICustomerRepository
    {
        /// <summary>
        /// Method that adds a customer.
        /// </summary>
        Task AddCustomer(Customer customer);

        /// <summary>
        /// Method that deletes a customer.
        /// </summary>
        Task RemoveCustomer(Customer customer);

        /// <summary>
        /// Method that updates a customer.
        /// </summary>
        Task UpdateCustomer(Customer customer);

        /// <summary>
        /// Method to get all the customers in a list.
        /// </summary>
        Task<List<Customer>> GetCustomersAsync();
    }
}
