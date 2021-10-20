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

        //H?r kan vi l?gga in metoder som r?r Customer, bara f?r start en metod f?r att h?mta alla customers som finns lagrade 
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public void AddCustomer(Customer customer)
        {
            _context.Add(customer);
        }

        public void RemoveCustomer(Customer customer)
        {
            _context.Remove(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            _context.Update(customer);
        }

        public async Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer)
        {

            return await _context.Orders.Where(o => o.Dispatched == true && customer.Id == o.CustomerId).ToListAsync();
            
        }

        public async Task<List<Order>> DisplayActiveCustomerOrder(Customer customer)
        {
            
            return await _context.Orders.Where(o => o.Dispatched == false && customer.Id == o.CustomerId).ToListAsync();
        }

    }
}
