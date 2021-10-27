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

            //m�ste f� korrekt order Id innan jag h�mtar order
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
            //g�r en lista som sorterar utefter l�ngst datum
            List<Order> sortedByLongest = allOrders.Where(b => b.Dispatched == true).OrderBy(b => b.OrderDate).ToList(); //vi servar f�rst de som har v�ntat l�ngst och vi �r bara intresserade av dispatched
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
                        
                    if (checkPro.Stock - ordLine.Quantity >= 0) // vi kollar om stock r�cker

                    {
                        if (ord.PaymentCompleted == true) //den �r betald och stock r�cker
                        {
                            tempStocks.Add(ordLine.Quantity);                                //vi memorerar hur mycket stock som tas
                            checkPro.Stock = checkPro.Stock - ordLine.Quantity; //F�RSTA SCENARIOT D� EN ORDERLINE �R OK
                        }
                        else // det finns stock men ordern �r inte betald
                        {
                            dispatchWholeOrder = false;
                            tempStocks.Add(0); //vi memorer att vi inte tar n�got
                        }
                            
                    }
                    else //stock r�cker inte s� vi kollar p� restock date
                    {
                                                        
                        if (checkPro.RestockingDate.ToString().Equals("0001-01-01 00:00:00")) //betyder att den inte har ett restocking date och vi s�tter ett
                        {
                            checkPro.RestockingDate = DateTime.Now.AddDays(10);
                            tempStocks.Add(0); //vi tar d� inget stock eftersom det inte finns ett restock date �n
                            dispatchWholeOrder = false; //vi s�tter ocks� boolen dispatchWholeOrder till falsk eftersom inte kan skickas
                        }
                        else //vi har ett restockdate, om det �r nu s� kan vi restocka, annars v�ntar vi
                        {
                            if (DateTime.Now >= checkPro.RestockingDate) //vi har n�tt Restocking Date och fyller p� med s� m�nga som beh�vs samt skickar iv�g ordern
                            {

                                int neededStock = checkPro.Stock - ordLine.Quantity;
                                neededStock = Math.Abs(neededStock);
                                tempStocks.Add(ordLine.Quantity); //vi memorerar hur mcyket stock som tas, om en produkt blir restocked s� tar vi endast quantoity vilket betyder att den kan bli ett �verskott 
                                AddMoreStock(checkPro, neededStock); //vi addar s� mcyket som beh�vs
                                checkPro.Stock = checkPro.Stock - ordLine.Quantity;  //ANDRA SCENARIOT D� EN ORDERLINE �R OK


                            }
                            else //dagen har inte kommit �nnu...
                            {
                                tempStocks.Add(0); //vi tar d� inget stock eftersom vi inte har kommit till v�rat restock date �n
                                dispatchWholeOrder = false;//vi har inte n�tt fram �nnu till restock dagen

                            }
                        }
                    }
                }
                
                if (dispatchWholeOrder == true && ord.PaymentCompleted == true) //ordern �r betald och �nnu inte skickad samt att bolen inte s�ger ifr�n 
                {
                    ord.Dispatched = true;

                }
                else if (dispatchWholeOrder == false)//betyder att en av orderlinsen inte gick att skicka och d�  m�ste vi l�mna tillbaka stocks f�r n�sta order
                {
                    int counter = 0;
                    foreach (OrderLine ordLine2 in checkProducts)
                    {
                        ordLine2.Product.Stock = ordLine2.Product.Stock + tempStocks[counter]; //vi adderar tillbaka de som tagits ifr�n stock eftersom ordern inte gick igenom
                        counter++;
                        //Vi vill ge tillbaka stock till de st�llen d�r vi tog stock ifr�n                        
                    }
                }
                _context.SaveChanges();     
            }
            ResetRestockDays(allOrders); //en metod f�r att reseta restock days om de �r uppn�dda efter�t

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
                        checkPro.RestockingDate = DateTime.Parse("0001-01-01 00:00:00"); //n�r vi kommit fram till restockDate s� s�tter vi det till standardv�rdet igen s� vi b�rjar om

                    }
                }
                
            }
            _context.SaveChanges();

        }

        public void AddMoreStock(Product giveItMoreStock, int neededStock)
        {
            //Vi har n�tt dagen d� stock kommer in och vi r�knar d� hur mycket som ska in kanske inte super realistiskt eftersom r�kningen borde ske innan men finns ingen s�n egenskap i databasen
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

        public void deleteNoItemsOrders() //vi har en funktion f�r att ta bort ordrar som inte har n�gra items eftersom de inte d� fyller n�gon funktion och kan st�ka till i Databasen
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
            shortestRestockDateOL = shortestRestockDateOL.OrderByDescending(s => s.Product.RestockingDate).ToList(); //f�r en lista av orderlines d�r vi sorterar utefter restockDate
                                                                                                                        //L�GG in s� att filtered �r sorterade utefter restockDate
            List<Order> filtered2 = new List<Order>();
            foreach (OrderLine ordLine in shortestRestockDateOL) //filtrering s� att vi f�r orders sorterade efter deras restocking date, eftersom orderlines med kortast restocking date kommer f�rst i ordLine
            {
                Order filteredOrder = allOrders.Where(o => o.Id == ordLine.OrderId).FirstOrDefault(); //Vi skapar en order f�r varje orderline
                filtered2.Add(filteredOrder);


            }
            filtered2 = filtered2.Distinct().ToList();
            filtered = filtered2;
            

            return filtered;
        }


    }
}
