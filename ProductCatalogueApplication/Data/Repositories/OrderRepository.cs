using Microsoft.EntityFrameworkCore;
using ProductCatalogueApplication.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data
{
    public class OrderRepository : IOrderRepository
    {

        private readonly WarehouseAutomationContext _context;


        public OrderRepository(WarehouseAutomationContext context)
        {
            _context = context;
        }

        //H?r kan vi l?gga in metoder som r?r Customer, bara f?r start en metod f?r att h?mta alla customers som finns lagrade 
        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public void AddNewOrder(Order newOrder)
        {
            //_context.Add(newOrder);
            _context.Orders.Add(newOrder);
        }

        public void UpdateOrder(Order toBeUpdated, Product product) //check att den stock och att den har payment completed om b?da ?r sanna s? blir det dispatched, om det inte ?r sant s? blir den pending
        {
            if(toBeUpdated.Dispatched == false)
            {
                if (product.Stock != 0) // ska finnas en ytterliggare check f?r att kolla om stocken ?r noll den metoden finns i Porduct Repo
                    //antingen avn?nds en metod i en annan klass  eller skriv in en egen check
                {
                    toBeUpdated.Dispatched = true;
                    toBeUpdated.PaymentCompleted = true;
                }
                else
                {
                    toBeUpdated.Dispatched = false; //den blir annars pending om den inte ?r betald

                }
            }
            
        }
    }
}
