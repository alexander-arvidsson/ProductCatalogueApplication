using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data
{
    public class OrderLineRepository
    {

        private readonly WarehouseAutomationContext _context;


        public OrderLineRepository(WarehouseAutomationContext context)
        {
            _context = context;
        }

        //Här kan vi lägga in metoder som rör Customer, bara för start en metod för att hämta alla customers som finns lagrade 
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            return await _context.OrderLines.ToListAsync();
        }
    }
}
