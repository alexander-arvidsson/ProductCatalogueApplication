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
            //Vi har nått dagen då stock kommer in och vi räknar då hur mycket som ska in kanske inte super realistiskt eftersom räkningen borde ske innan men finns ingen sån egenskap i databasen
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
        public void deleteNoItemsOrders() //vi har en funktion för att ta bort ordrar som inte har några items eftersom de inte då fyller någon funktion och kan stöka till i Databasen
        {
            List<Order> noItemsOrders = new List<Order>();
            noItemsOrders = _context.Orders.Where(b => b.Items.Count() == 0).ToList();
            foreach (Order or in noItemsOrders)
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
            }

            return filtered;
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
            if (prod.Stock - checkQuantity.Quantity >= 0) // vi kollar om stock räcker
            {
                return GetPayedAndQuantityNumber(ord, checkQuantity, prod);
            }
            else //stock räcker inte så vi kollar på restock date
            {
                return GetStockedProductAndQuantityNumber(checkQuantity, prod);
            }
        }
        private int GetPayedAndQuantityNumber(Order ord, OrderLine checkQuantity, Product prod)
        {
            prod.Stock -= checkQuantity.Quantity;
            return ord.PaymentCompleted == true ? checkQuantity.Quantity : 0;
        }

        private int GetStockedProductAndQuantityNumber(OrderLine ordLine, Product prod)
        {
            if (prod.RestockingDate.ToString().Equals("0001-01-01 00:00:00")) //betyder att den inte har ett restocking date och vi sätter ett
            {
                prod.RestockingDate = DateTime.Now.AddDays(10);
                return 0;
            }
            else //vi har ett restockdate, om det är nu så kan vi restocka, annars väntar vi
            {
                if (DateTime.Now >= prod.RestockingDate) //vi har nått Restocking Date och fyller på med så många som behövs samt skickar iväg ordern
                {
                    int neededStock = prod.Stock - ordLine.Quantity;
                    neededStock = Math.Abs(neededStock);
                    AddMoreStock(prod, neededStock); //vi addar så mcyket som behövs
                    prod.Stock -= ordLine.Quantity;  //ANDRA SCENARIOT DÅ EN ORDERLINE ÄR OK

                    return ordLine.Quantity;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void BatchOrders(bool dispatchWholeOrder, Order ord, List<OrderLine> checkProducts, List<int> stockList)
        {
            if (dispatchWholeOrder == true && ord.PaymentCompleted == true)
            {
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

