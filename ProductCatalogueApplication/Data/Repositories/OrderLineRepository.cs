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

        //H�r kan vi l�gga in metoder som r�r Customer, bara f�r start en metod f�r att h�mta alla customers som finns lagrade 
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            return await _context.OrderLines.ToListAsync();
        }
    }
}
