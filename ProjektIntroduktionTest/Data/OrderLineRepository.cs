using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIntroduktionTest.Data
{
    public class OrderLineRepository
    {

        private readonly Context _context;


        public OrderLineRepository(Context context)
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
