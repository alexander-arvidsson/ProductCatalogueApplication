using Microsoft.EntityFrameworkCore;
using ProductCatalogueApplication.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            return await _context.OrderLines.ToListAsync();
        }

        public void AddNewOrder(Order newOrder)
        {
            //_context.Add(newOrder);        //en order m?ste ha en kund, om den inte finns s? kan den inte skapas s? vi g?r det nu
            newOrder.Customer = _context.Customers.Where(ol => ol.Id == newOrder.CustomerId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            _context.Orders.Add(newOrder);
            _context.SaveChanges();
        }

        public void AddNewOrderLine(OrderLine newOrderLine)
        {
            //_context.Add(newOrder);        //en order m?ste ha en kund, om den inte finns s? kan den inte skapas s? vi g?r det nu
            //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            Order getOrderId = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault(); //letar efter matchande?keys f?r att tillslut hitta Produkten som ?r kopppad s? vi kan f? stock som ?r en produkt egenksap
            newOrderLine.OrderId = getOrderId.Id;
            //Vi tar fram ordern som matchar via nycklarna f?r att d? f? fram dens order id och kunna ge till orderLinen
            newOrderLine.Order = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            newOrderLine.Product = _context.Products.Where(ol => ol.Id == newOrderLine.ProductId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId

            _context.OrderLines.Add(newOrderLine);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order toBeUpdated) //check att den stock och att den har payment completed om b?da ?r sanna s? blir det dispatched, om det inte ?r sant s? blir den pending
        {
            //OrderLine checkProduct = (OrderLine)_context.OrderLines.Where(ol=>ol.OrderId == toBeUpdated.Id);
            OrderLine checkProduct = _context.OrderLines.Where(ol => ol.OrderId == toBeUpdated.Id).FirstOrDefault(); //letar efter matchande?keys f?r att tillslut hitta Produkten som ?r kopppad s? vi kan f? stock som ?r en produkt egenksap
           
            Product checkPro = _context.Products.Where(ol => ol.Id == checkProduct.ProductId).FirstOrDefault();
            if (toBeUpdated.Dispatched == false)
            {
                if (checkPro.Stock != 0) // ska finnas en ytterliggare check f?r att kolla om stocken ?r noll den metoden finns i Porduct Repo
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
            _context.SaveChanges();
            
        }
    }
}
