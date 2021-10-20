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

        //Här kan vi lägga in metoder som rör Customer, bara för start en metod för att hämta alla customers som finns lagrade 
        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
    }
}
