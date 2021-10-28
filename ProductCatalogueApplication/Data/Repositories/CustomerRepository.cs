using Microsoft.EntityFrameworkCore;
using ProductCatalogueApplication.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ProductCatalogueApplication.Data
{
    public class CustomerRepository : ICustomerRepository
    {

        private readonly WarehouseAutomationContext _context;

        public CustomerRepository(WarehouseAutomationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// A method that fetches all the customers stored in the database.
        /// </summary>
        /// <returns>A list of customers</returns>
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        /// <summary>
        /// A method that adds a new customer to the database.
        /// </summary>
        /// <param name="customer">A specific customer</param>
        /// <returns>Task</returns>
        public async Task AddCustomer(Customer customer)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that removes a customer from the database.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Task</returns>
        public async Task RemoveCustomer(Customer customer)
        {
            _context.Remove(customer);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that updates customer information.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Task</returns>
        public async Task UpdateCustomer(Customer customer)
        {
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }

    }
}
