using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ProductCatalogueApplication.Data.Interfaces;

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
    }
}
