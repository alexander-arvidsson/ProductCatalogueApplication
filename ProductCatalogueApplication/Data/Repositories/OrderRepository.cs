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

        public void BatchAndProcess(List<Order> allOrders)
        {
            ProcessOrders(allOrders);
            ResetRestockDays(allOrders);
        }    

        public void AddMoreStock(Product giveItMoreStock, int neededStock)
        {
            //Vi har n�tt dagen d� stock kommer in och vi r�knar d� hur mycket som ska in kanske inte super realistiskt eftersom r�kningen borde ske innan men finns ingen s�n egenskap i databasen
            giveItMoreStock.Stock = giveItMoreStock.Stock + neededStock + 20; //lägger även in 20 extra.
            _context.SaveChanges();
        }
        public void UpdateOrder(Order specOrder)
        {
            _context.Orders.Update(specOrder);
            _context.SaveChanges();
            
        }

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
                }
                if (dispatchWholeOrder == true && ord.PaymentCompleted == true && ord.Dispatched == false) //ordern är betald och ännu inte skickad samt att bolen inte säger ifrån 
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

        }

        /** 
         ** Helper methods for Batch & Process
         **/

        private void ProcessOrders(List<Order> allOrders)
        {
            bool dispatchWholeOrder = true;
            List<int> stocksToReturn = new List<int>();
            List<Order> sortedByLongest = allOrders
                .Where(b => b.Dispatched == false)
                .OrderBy(b => b.OrderDate)
                .ToList();

            foreach (Order ord in sortedByLongest)
            {
                stocksToReturn.Clear();
                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToList();

                foreach (OrderLine ordLine in checkProducts)
                {
                    Product checkProduct = _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefault();

                    int stockQuantity = GetProcessedOrderQuantity(ord, ordLine, checkProduct);
                    stocksToReturn.Add(stockQuantity);
                    dispatchWholeOrder = stockQuantity != 0;
                }

                BatchOrders(dispatchWholeOrder, ord, checkProducts, stocksToReturn);
            }
            _context.SaveChanges();
        }

        private int GetProcessedOrderQuantity(Order ord, OrderLine checkQuantity, Product prod)
        {
            if (prod.Stock - checkQuantity.Quantity >= 0) // vi kollar om stock r�cker
            {
                return GetPayedAndQuantityNumber(ord, checkQuantity, prod);
            }
            else //stock r�cker inte s� vi kollar p� restock date
            {
                return GetStockedProductAndQuantityNumber(checkQuantity, prod);
            }
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
        private int GetPayedAndQuantityNumber(Order ord, OrderLine checkQuantity, Product prod)
        {
            prod.Stock -= checkQuantity.Quantity;
            return ord.PaymentCompleted == true ? checkQuantity.Quantity : 0;
        }

        private int GetStockedProductAndQuantityNumber(OrderLine ordLine, Product prod)
        {
            if (prod.RestockingDate.ToString().Equals("0001-01-01 00:00:00")) //betyder att den inte har ett restocking date och vi s�tter ett
            {
                prod.RestockingDate = DateTime.Now.AddDays(10);
                return 0;
            }
            else //vi har ett restockdate, om det �r nu s� kan vi restocka, annars v�ntar vi
            {
                if (DateTime.Now >= prod.RestockingDate) //vi har n�tt Restocking Date och fyller p� med s� m�nga som beh�vs samt skickar iv�g ordern
                {
                    int neededStock = prod.Stock - ordLine.Quantity;
                    neededStock = Math.Abs(neededStock);
                    AddMoreStock(prod, neededStock); //vi addar s� mcyket som beh�vs
                    prod.Stock -= ordLine.Quantity;  //ANDRA SCENARIOT D� EN ORDERLINE �R OK

                    return ordLine.Quantity;
                }
                else
                {
                    return 0;
                }
            }
            _context.SaveChanges();
        }

        private void BatchOrders(bool dispatchWholeOrder, Order ord, List<OrderLine> checkProducts, List<int> stockList)
        {
            if (dispatchWholeOrder == true && ord.PaymentCompleted == true)
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
                ord.Dispatched = true;
            }
            else if (dispatchWholeOrder == false)
            {
                int counter = 0;
                foreach (OrderLine ordLine2 in checkProducts)
                {
                    ordLine2.Product.Stock = ordLine2.Product.Stock + stockList[counter];
                    counter++;
                }
            }
        }

        private void ResetRestockDays(List<Order> resetRestockDays)
        {
            foreach (Order rest in resetRestockDays)
            {
                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == rest.Id).ToList();
                foreach (OrderLine ordLine in checkProducts)
                {
                    Product checkPro = _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefault();

                    if (DateTime.Now >= checkPro.RestockingDate)
                    {
                        checkPro.RestockingDate = DateTime.Parse("0001-01-01 00:00:00");
                    }
                }
            }
            _context.SaveChanges();
        }


    }
}

