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

        public async Task<List<Order>> GetOrdersAsync(int id)
        {
            return await _context.Orders.Where(c => c.CustomerId == id).ToListAsync();
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

        public void ProcessBatchorders(List<Order> allOrders) //check att den stock och att den har payment completed om b?da ?r sanna s? blir det dispatched, om det inte ?r sant s? blir den pending
        {
            //OrderLine checkProduct = (OrderLine)_context.OrderLines.Where(ol=>ol.OrderId == toBeUpdated.Id);
            //gör en lista som sorterar utefter längst datum
            List<Order> sortedByLongest = allOrders.OrderBy(b => b.OrderDate).ToList(); //vi servar först de som har väntat längst

            foreach (Order ord in sortedByLongest)
            {
                //OrderLine checkProduct = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).FirstOrDefault(); //letar efter matchande?keys f?r att tillslut hitta Produkten som ?r kopppad s? vi kan f? stock som ?r en produkt egenksap

                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList();

                foreach(OrderLine ordLine in checkProducts)
                {
                    Product checkPro = _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefault();

                    if (ord.Dispatched == false)
                    {
                        if (checkPro.Stock - ordLine.Quantity >= 0 && ord.PaymentCompleted == true) // vi kollar så att stocken räcker och att den är betald för
                                                                                                         //antingen avn?nds en metod i en annan klass  eller skriv in en egen check
                        {
                            checkPro.Stock = checkPro.Stock - ordLine.Quantity;

                            ord.Dispatched = true;
                        }
                        else
                        {
                            //gör en metod som efterfrågar som adderar så mycket som efterfrågas och inte går att förse

                            if (checkPro.RestockingDate.ToString().Equals("0001-01-01 00:00:00"))//betyder att den inte har ett restocking date så sätter ett, vi kan också ha ett gammalt
                            {
                                checkPro.RestockingDate = DateTime.Now.AddDays(10);


                            }

                            else //betyder att det finns ett så vi kollar om datumet är uppnått eller inte
                            {
                                if (DateTime.Now >= checkPro.RestockingDate) //vi har nått Restocking Date och fyller på med så många som behövs samt skickar iväg ordern
                                {
                                    int neededStock = checkPro.Stock - ordLine.Quantity;
                                    neededStock = Math.Abs(neededStock);
                                    AddMoreStock(checkPro, neededStock);
                                    checkPro.RestockingDate = DateTime.Parse("0001-01-01 00:00:00"); //när vi kommit fram till restockDate så sätter vi det till standardvärdet igen så vi börjar om

                                    if (ord.PaymentCompleted == true)
                                    {
                                        checkPro.Stock = checkPro.Stock - ordLine.Quantity;
                                        ord.Dispatched = true;
                                    }
                                }
                            }
                        }
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

        public void UpdateOrder(Order specOrder)
        {
            _context.SaveChanges();
            
        }

        public async Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer)
        {
            return await _context.Orders.Where(o => o.Dispatched == true && customer.Id == o.CustomerId).ToListAsync();
        }

        public async Task<List<Order>> DisplayActiveCustomerOrder(Customer customer)
        {
            return await _context.Orders.Where(o => o.Dispatched == false && customer.Id == o.CustomerId).ToListAsync();
        }
        

        public void SetPayment(bool payedOrNot, Order order)
        {
            if (payedOrNot == false)
            {
                order.PaymentCompleted = true;
            }
            else
            {
                order.PaymentCompleted = false;
            }
        }

        public void deleteNoItemsOrders() //vi har en funktion för att ta bort ordrar som inte har några items eftersom de inte då fyller någon funktion och kan stöka till i Databasen
        {
            List<Order> noItemsOrders = new List<Order>();
            noItemsOrders = _context.Orders.Where(b => b.Items.Count() == 0).ToList();
            foreach(Order or in noItemsOrders)
            {
                _context.Orders.Remove(or);
            }
            _context.SaveChanges();
        }
        public List<Order> GetDispatchedAndPending(List<Order> allOrders, bool choice)
        {
            List<Order> filtered = new List<Order>();
            List<OrderLine> shortestRestockDateOL = new List<OrderLine>();

            Product checkPro = new Product();
            bool pendingOrder = false;
            DateTime compare = DateTime.MinValue;

            if (choice == true) //dispatched
            {
                filtered = allOrders.Where(b => b.Dispatched == true).ToList();
            }
            else //pending
            {
                //kolla om kvantiteten räcker till för ordern annars är den pending
                //sort by kortast till restocking date
                foreach(Order ord in allOrders) 
                {
                    List<OrderLine> getPending = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList(); //vi får en lista med alla orderlines som är kopplade till en visst order
                                                                                                                 //vi antar att den blir först pending när vi klickar på process knappen
                                                                                                                 //getPending = getPending.Where(b => b.)
                    pendingOrder = false;

                    foreach (OrderLine or in getPending)
                    {
                        checkPro = _context.Products.Where(p => p.Id == or.ProductId).FirstOrDefault(); // vi får ett orderline objekt 
                        if(checkPro.RestockingDate.ToString() != "0001-01-01 00:00:00")//betyder att det finns ett restocking date på orderLinen
                        {

                            pendingOrder = true;
                            shortestRestockDateOL.Add(or);
                            
                            //vi vill sortera orderna utefter att de ska ha en orderline som har ett restockDate som är närmaste date.now
                        }
                        
                    }
                    if (pendingOrder == true)
                    {
                        filtered.Add(ord); // vi adderar ordern till våran lista med ordrar där en av produkterna ska restockas för att satisfya ordern
                    }
                     //filtered listan blir innehåller alla ordrar
                }

                //först en lista av orders och sen gör vi en forreach loop genom hela orderlistan
                //sen 

                //Product checkPro = _context.Products.Where(ol => ol.Id == checkProduct.ProductId).FirstOrDefault();
                shortestRestockDateOL = shortestRestockDateOL.OrderByDescending(s => s.Product.RestockingDate).ToList(); //får en lista av orderlines där vi sorterar utefter restockDate
                                                                                                               //LÄGG in så att filtered är sorterade utefter restockDate
                shortestRestockDateOL = shortestRestockDateOL.Distinct().ToList();

                List<Order> filtered2 = new List<Order>();
                foreach (OrderLine ordLine in shortestRestockDateOL) //filtrering så att vi får orders sorterade efter deras restocking date, eftersom orderlines med kortast restocking date kommer först i ordLine
                {
                    Order filteredOrder = allOrders.Where(o => o.Id == ordLine.OrderId).FirstOrDefault(); //Vi skapar en order för varje orderline
                    filtered2.Add(filteredOrder);
                    

                }
                filtered2 = filtered2.Distinct().ToList();
                filtered = filtered2;
            }
            
            return filtered;
        }

    }
}
