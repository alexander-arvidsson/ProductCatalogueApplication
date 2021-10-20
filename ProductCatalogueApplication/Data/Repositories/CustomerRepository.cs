using Microsoft.EntityFrameworkCore;
using ProductCatalogueApplication.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        //Customer Add metod
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

        public async Task<List<Customer>> DisplayArchivedCustomerOrder(Order order)
        {
            Task<List<Customer>> CustomerList =_context.Customers.ToListAsync();

            CustomerList.Result

                //where ...bool is true??

            return await ...;
        }

        public async Task<List<Customer>> DisplayActiveCustomerOrder(Order order)
        {
            //where ...bool is true??
            return await ...;
        }



    }
}
