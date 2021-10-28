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
        /// En metod som hämtar alla customers som finns lagrade.
        /// </summary>
        /// <returns>Task<List></returns>
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        /// <summary>
        /// En metod som lägger till en ny kund i databasen
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Task</returns>
        public async Task AddCustomer(Customer customer)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// En metod som tar bort en kund ur databasen
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Task</returns>
        public async Task RemoveCustomer(Customer customer)
        {
            _context.Remove(customer);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// En metod som uppdaterar en kunds värden i databasen
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
