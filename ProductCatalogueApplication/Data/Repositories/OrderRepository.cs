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

        public async Task AddNewOrder(Order newOrder)
        {
            //_context.Add(newOrder);        //en order m?ste ha en kund, om den inte finns s? kan den inte skapas s? vi g?r det nu
            newOrder.OrderDate = DateTime.Now;
            newOrder.Customer = _context.Customers.Where(ol => ol.Id == newOrder.CustomerId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
        }

        public async Task AddNewOrderLine(OrderLine newOrderLine, Order OrderToMatch)
        {
            //_context.Add(newOrder);        //en order m?ste ha en kund, om den inte finns s? kan den inte skapas s? vi g?r det nu
            //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            //Order getOrderId = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault(); //letar efter matchande?keys f?r att tillslut hitta Produkten som ?r kopppad s? vi kan f? stock som ?r en produkt egenksap
            //newOrderLine.OrderId = getOrderId.Id;

            //måste få korrekt order Id innan jag hämtar order
            //newOrderLine.Id = 0;
            newOrderLine.OrderId = OrderToMatch.Id;
            newOrderLine.Order = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault();


            //Vi tar fram ordern som matchar via nycklarna f?r att d? f? fram dens order id och kunna ge till orderLinen
            //newOrderLine.Order = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            newOrderLine.Product = _context.Products.Where(ol => ol.Id == newOrderLine.ProductId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId

            _context.OrderLines.Add(newOrderLine);
            await _context.SaveChangesAsync();
        }

        public void ProcessBatchorders(List<Order> allOrders) //check att den stock och att den har payment completed om b?da ?r sanna s? blir det dispatched, om det inte ?r sant s? blir den pending
        {
            //OrderLine checkProduct = (OrderLine)_context.OrderLines.Where(ol=>ol.OrderId == toBeUpdated.Id);
            //gör en lista som sorterar utefter längst datum
            List<Order> sortedByLongest = allOrders.Where(b => b.Dispatched == true).OrderBy(b => b.OrderDate).ToList(); //vi servar först de som har väntat längst och vi är bara intresserade av dispatched
            bool dispatchWholeOrder = true;
            List<int> tempStocks = new List<int>();


            foreach (Order ord in sortedByLongest)
            {

                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList();
                tempStocks.Clear();
                dispatchWholeOrder = true;

                
                foreach (OrderLine ordLine in checkProducts) //vi kollar varje orderline hos de olika odrarna
                {
                    Product checkPro = _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefault();
                        
                    if (checkPro.Stock - ordLine.Quantity >= 0) // vi kollar om stock räcker

                    {
                        if (ord.PaymentCompleted == true) //den är betald och stock räcker
                        {
                            tempStocks.Add(ordLine.Quantity);                                //vi memorerar hur mycket stock som tas
                            checkPro.Stock = checkPro.Stock - ordLine.Quantity; //FÖRSTA SCENARIOT DÅ EN ORDERLINE ÄR OK
                        }
                        else // det finns stock men ordern är inte betald
                        {
                            dispatchWholeOrder = false;
                            tempStocks.Add(0); //vi memorer att vi inte tar något
                        }
                            
                    }
                    else //stock räcker inte så vi kollar på restock date
                    {
                                                        
                        if (checkPro.RestockingDate.ToString().Equals("0001-01-01 00:00:00")) //betyder att den inte har ett restocking date och vi sätter ett
                        {
                            checkPro.RestockingDate = DateTime.Now.AddDays(10);
                            tempStocks.Add(0); //vi tar då inget stock eftersom det inte finns ett restock date än
                            dispatchWholeOrder = false; //vi sätter också boolen dispatchWholeOrder till falsk eftersom inte kan skickas
                        }
                        else //vi har ett restockdate, om det är nu så kan vi restocka, annars väntar vi
                        {
                            if (DateTime.Now >= checkPro.RestockingDate) //vi har nått Restocking Date och fyller på med så många som behövs samt skickar iväg ordern
                            {

                                int neededStock = checkPro.Stock - ordLine.Quantity;
                                neededStock = Math.Abs(neededStock);
                                tempStocks.Add(ordLine.Quantity); //vi memorerar hur mcyket stock som tas, om en produkt blir restocked så tar vi endast quantoity vilket betyder att den kan bli ett överskott 
                                AddMoreStock(checkPro, neededStock); //vi addar så mcyket som behövs
                                checkPro.Stock = checkPro.Stock - ordLine.Quantity;  //ANDRA SCENARIOT DÅ EN ORDERLINE ÄR OK


                            }
                            else //dagen har inte kommit ännu...
                            {
                                tempStocks.Add(0); //vi tar då inget stock eftersom vi inte har kommit till vårat restock date än
                                dispatchWholeOrder = false;//vi har inte nått fram ännu till restock dagen

                            }
                        }
                    }
                }
                
                if (dispatchWholeOrder == true && ord.PaymentCompleted == true) //ordern är betald och ännu inte skickad samt att bolen inte säger ifrån 
                {
                    ord.Dispatched = true;

                }
                else if (dispatchWholeOrder == false)//betyder att en av orderlinsen inte gick att skicka och då  måste vi lämna tillbaka stocks för nästa order
                {
                    int counter = 0;
                    foreach (OrderLine ordLine2 in checkProducts)
                    {
                        ordLine2.Product.Stock = ordLine2.Product.Stock + tempStocks[counter]; //vi adderar tillbaka de som tagits ifrån stock eftersom ordern inte gick igenom
                        counter++;
                        //Vi vill ge tillbaka stock till de ställen där vi tog stock ifrån                        
                    }
                }
                _context.SaveChanges();     
            }
            ResetRestockDays(allOrders); //en metod för att reseta restock days om de är uppnådda efteråt

        }
        private void ResetRestockDays(List<Order> resetRestockDays)
        {
            foreach(Order rest in resetRestockDays)
            {
                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == rest.Id).ToList();
                foreach(OrderLine ordLine in checkProducts)
                {
                    Product checkPro = _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefault();

                    if (DateTime.Now >= checkPro.RestockingDate)
                    {
                        checkPro.RestockingDate = DateTime.Parse("0001-01-01 00:00:00"); //när vi kommit fram till restockDate så sätter vi det till standardvärdet igen så vi börjar om

                    }
                }
                
            }
            _context.SaveChanges();

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
        public void DeleteItem(OrderLine toBeDeleted)
        {
            _context.OrderLines.Remove(toBeDeleted);
            if(toBeDeleted.Order.Items.Count() == 1) //vi raderar hela ordern om inga items finnsinuti
            {
                _context.Orders.Remove(toBeDeleted.Order);
            }
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
            _context.SaveChanges();
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
        public List<Order> GetDispatched(List<Order> allOrders)
        {
            List<Order> filtered = new List<Order>();
 
            filtered = allOrders.Where(b => b.Dispatched == true).ToList();

            return filtered;
        }


        public List<Order> GetPending(List<Order> allOrders)
        {
            List<Order> filtered = new List<Order>();
            List<OrderLine> shortestRestockDateOL = new List<OrderLine>();
            List<OrderLine> getPending = new List<OrderLine>();
            Product checkPro = new Product();
  
            filtered = allOrders.Where(b => b.Dispatched == false).ToList();
            foreach (Order ord in filtered)
            {
                getPending = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList();
                shortestRestockDateOL.AddRange(getPending);
                
            }
            shortestRestockDateOL = shortestRestockDateOL.OrderByDescending(s => s.Product.RestockingDate).ToList(); //får en lista av orderlines där vi sorterar utefter restockDate
                                                                                                                        //LÄGG in så att filtered är sorterade utefter restockDate
            List<Order> filtered2 = new List<Order>();
            foreach (OrderLine ordLine in shortestRestockDateOL) //filtrering så att vi får orders sorterade efter deras restocking date, eftersom orderlines med kortast restocking date kommer först i ordLine
            {
                Order filteredOrder = allOrders.Where(o => o.Id == ordLine.OrderId).FirstOrDefault(); //Vi skapar en order för varje orderline
                filtered2.Add(filteredOrder);


            }
            filtered2 = filtered2.Distinct().ToList();
            filtered = filtered2;
            

            return filtered;
        }


    }
}
