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
            newOrder.OrderDate = DateTime.Now;
            newOrder.Customer = _context.Customers.Where(ol => ol.Id == newOrder.CustomerId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            _context.Orders.Add(newOrder);
            _context.SaveChanges();
        }

        public void AddNewOrderLine(OrderLine newOrderLine, Order OrderToMatch)
        {
            //_context.Add(newOrder);        //en order m?ste ha en kund, om den inte finns s? kan den inte skapas s? vi g?r det nu
            //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            //Order getOrderId = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault(); //letar efter matchande?keys f?r att tillslut hitta Produkten som ?r kopppad s? vi kan f? stock som ?r en produkt egenksap
            //newOrderLine.OrderId = getOrderId.Id;

            //måste få korrekt order Id innan jag hämtar order
            newOrderLine.OrderId = OrderToMatch.Id;
            newOrderLine.Order = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault();


            //Vi tar fram ordern som matchar via nycklarna f?r att d? f? fram dens order id och kunna ge till orderLinen
            //newOrderLine.Order = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            newOrderLine.Product = _context.Products.Where(ol => ol.Id == newOrderLine.ProductId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId

            _context.OrderLines.Add(newOrderLine);
            _context.SaveChanges();
        }

        public void UpdateOrder(List<Order> allOrders) //check att den stock och att den har payment completed om b?da ?r sanna s? blir det dispatched, om det inte ?r sant s? blir den pending
        {
            //OrderLine checkProduct = (OrderLine)_context.OrderLines.Where(ol=>ol.OrderId == toBeUpdated.Id);
            //gör en lista som sorterar utefter längst datum
            List<Order> sortedByLongest = allOrders.OrderBy(b => b.OrderDate).ToList(); //vi servar först de som har väntat längst

            foreach (Order ord in sortedByLongest)
            {
                OrderLine checkProduct = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).FirstOrDefault(); //letar efter matchande?keys f?r att tillslut hitta Produkten som ?r kopppad s? vi kan f? stock som ?r en produkt egenksap

                Product checkPro = _context.Products.Where(ol => ol.Id == checkProduct.ProductId).FirstOrDefault();

                if (ord.Dispatched == false && ord.PaymentCompleted == true)
                {
                    if (checkPro.Stock - checkProduct.Quantity >= 0) // vi kollar så att stocken räcker
                                                                     //antingen avn?nds en metod i en annan klass  eller skriv in en egen check
                    {
                        checkPro.Stock = checkPro.Stock - checkProduct.Quantity;

                        ord.Dispatched = true;
                        _context.SaveChanges();
                        //toBeUpdated.PaymentCompleted = true;
                    }
                    else
                    {
                        //gör en metod som efterfrågar som adderar så mycket som efterfrågas och inte går att förse
                        int neededStock = checkPro.Stock - checkProduct.Quantity;
                        neededStock = Math.Abs(neededStock);
                        if(checkPro.RestockingDate.ToString().Equals("0001-01-01 00:00:00"))//betyder att den inte har ett restocking date så sätter ett
                        {
                            checkPro.RestockingDate = DateTime.Now.AddDays(10);

                            
                        }
                        else //betyder att det finns ett så vi kollar om datumet är uppnått eller inte
                        {
                            if (DateTime.Now >= checkPro.RestockingDate) //vi har nått Restocking Date och fyller på med så många som behövs
                            {
                                AddMoreStock(checkPro, neededStock);

                            }

                        }


                        ord.Dispatched = false; //den blir annars pending om den inte ?r betald

                    }
                }
                _context.SaveChanges();
            }
            
            
            

            
            
        }


        public void AddMoreStock(Product giveItMoreStock, int neededStock)
        {
            //Vi har nått dagen då stock kommer in och vi räknar då hur mycket som ska in kanske inte super realistiskt eftersom räkningen borde ske innan men finns ingen sån egenskap i databasen
            giveItMoreStock.Stock = giveItMoreStock.Stock + neededStock;

        }
    }
}
