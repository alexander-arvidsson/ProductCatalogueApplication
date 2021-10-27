using Microsoft.EntityFrameworkCore;
using ProductCatalogueApplication.Data.Interfaces;
using System;
using System.Collections.Generic;
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

            newOrderLine.OrderId = OrderToMatch.Id;
            newOrderLine.Order = _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefault();

            newOrderLine.Product = _context.Products.Where(ol => ol.Id == newOrderLine.ProductId).FirstOrDefault(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId

            _context.OrderLines.Add(newOrderLine);
            await _context.SaveChangesAsync();
        }

        public void Batchorders(List<Order> allOrders) //check att den stock och att den har payment completed om b?da ?r sanna s? blir det dispatched, om det inte ?r sant s? blir den pending
        {
            //g�r en lista som sorterar utefter l�ngst datum
            List<Order> sortedByLongest = allOrders.OrderBy(b => b.OrderDate).ToList(); //vi servar f�rst de som har v�ntat l�ngst
            bool dispatchWholeOrder = true;
            List<int> tempStocks = new List<int>();

            foreach (Order ord in sortedByLongest)
            {
                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList();
                tempStocks.Clear();
                dispatchWholeOrder = true;

                if(ord.Dispatched == false) // vi �r endast intresserade av ordrar som inte �r skickade
                {

                }
                if (dispatchWholeOrder == true && ord.PaymentCompleted == true && ord.Dispatched == false) //ordern �r betald och �nnu inte skickad samt att bolen inte s�ger ifr�n 
                {
                    ord.Dispatched = true;

                }
                else if (ord.Dispatched == false && dispatchWholeOrder == false)//betyder att en av orderlinsen inte gick att skicka och d�  m�ste vi l�mna tillbaka stocks f�r n�sta order
                {
                    int counter = 0;
                    foreach (OrderLine ordLine2 in checkProducts)
                    {
                        ordLine2.Product.Stock = ordLine2.Product.Stock + tempStocks[counter]; //vi adderar tillbaka de som tagits ifr�n stock eftersom ordern inte gick igenom
                        counter++;
                        //Vi vill ge tillbaka stock till de st�llen d�r vi tog stock ifr�n                        
                    }
                }

                if (DateTime.Now >= checkPro.RestockingDate) //vi har n�tt Restocking Date och fyller p� med s� m�nga som beh�vs samt skickar iv�g ordern
                {

                    int neededStock = checkPro.Stock - ordLine.Quantity;
                    neededStock = Math.Abs(neededStock);
                    AddMoreStock(checkPro, neededStock); //vi addar s� mcyket som beh�vs
                    checkPro.Stock = checkPro.Stock - ordLine.Quantity;  //ANDRA SCENARIOT D� EN ORDERLINE �R OK

                }

                _context.SaveChanges();     
            }
            ResetRestockDays(allOrders); //en metod f�r att reseta restock days om de �r uppn�dda efter�t
        }

        private void ProcessOrders(List<Order> allOrders)
        {
            List<Order> sortedByLongest = allOrders
                .Where(b => b.Dispatched == false)
                .OrderBy(b => b.OrderDate)
                .ToList();

            foreach (Order ord in sortedByLongest)
            {
                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList();

                foreach(OrderLine ordLine in checkProducts)
                {
                    Product checkProduct = _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefault();
                    GetProcessedOrder(ordLine, checkProduct);
                }
            }
        }

        public void BatchAndProcess()
        {

        }

        private Product GetProcessedOrder(OrderLine checkQuantity, Product prod)

        {
                if (prod.Stock - checkQuantity.Quantity >= 0) // vi kollar om stock r�cker

                {
                    prod.Stock -= checkQuantity.Quantity; //F�RSTA SCENARIOT D� EN ORDERLINE �R OK

                return prod;
                }
                else //stock r�cker inte s� vi kollar p� restock date
                {
                    if (prod.RestockingDate.ToString().Equals("0001-01-01 00:00:00")) //betyder att den inte har ett restocking date och vi s�tter ett
                    {
                    prod.RestockingDate = DateTime.Now.AddDays(10);
                    }
                
                return prod;
                }
            }
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
            giveItMoreStock.Stock += neededStock;

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
        public List<Order> GetDispatchedAndPending(List<Order> allOrders, bool choice)
        {
            List<Order> filtered = new List<Order>();
            List<OrderLine> shortestRestockDateOL = new List<OrderLine>();
            List<OrderLine> getPending = new List<OrderLine>();
            Product checkPro = new Product();
            DateTime compare = DateTime.MinValue;

            if (choice == true) //dispatched
            {
                filtered = allOrders.Where(b => b.Dispatched == true).ToList();
            }
            else //pending
            {
                filtered = allOrders.Where(b => b.Dispatched == false).ToList();
                foreach(Order ord in filtered)
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
            }
            
            return filtered;
        }

    }
}
