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


        /// <summary>
        /// A method that gets the list of orders from the database.
        /// </summary>
        /// <returns>Order-list</returns>
        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        /// <summary>
        /// A method that matches a customerID to an order from the database.
        /// </summary>
        /// <param name="id">The customer ID</param>
        /// <returns>All customerIDs with orders linked to them.</returns>
        public async Task<List<Order>> GetOrdersAsync(int id)
        {
            return await _context.Orders.Where(c => c.CustomerId == id).ToListAsync();
        }

        /// <summary>
        /// A method that gets the list of orderlines from the database.
        /// </summary>
        /// <returns>Order-list</returns>
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            return await _context.OrderLines.ToListAsync();
        }

        /// <summary>
        /// A method that adds a new order linked to a customer ID.
        /// </summary>
        /// <param name="newOrder">A new object from the Order class.</param>
        /// <returns>No return because of async task method</returns>
        public async Task AddNewOrder(Order newOrder)
        {
            newOrder.OrderDate = DateTime.Now;
            newOrder.Customer = await _context.Customers.Where(ol => ol.Id == newOrder.CustomerId).FirstOrDefaultAsync(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that adds a new orderline to an (already existing??) order
        /// </summary>
        /// <param name="newOrderLine">A new object from the OrderLine class.</param>
        /// <param name="OrderToMatch">A new object from the Order class???</param>
        /// <returns>No return because of async task method</returns>
        public async Task AddNewOrderLine(OrderLine newOrderLine, Order OrderToMatch)
        {
            newOrderLine.OrderId = OrderToMatch.Id;
            newOrderLine.Order = await _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefaultAsync();

            newOrderLine.Product = await _context.Products.Where(ol => ol.Id == newOrderLine.ProductId).FirstOrDefaultAsync(); //vi hittar v?ran customer fr?n att vi s?ker efter customerId

            bool noDuplicateProduct = true; //bool som sätts till false ifall produkten redan existerar i ordern

            foreach (OrderLine ol in OrderToMatch.Items) //kollar alla orderlines i ordern
            {
                if (ol.Product == newOrderLine.Product) //jämför orderlinens produkt med den nya produkten
                {
                    ol.Quantity += newOrderLine.Quantity; //ifall produkten redan finns i ordern adderas den nya kvantiteten till den ordinarie orderlinen
                    noDuplicateProduct = false;
                    await _context.SaveChangesAsync();
                }
            }
            if (noDuplicateProduct == true) //om produkten inte fanns med i ordern skapas en ny orderline med den nya produkten
            {
                await _context.OrderLines.AddAsync(newOrderLine);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that processes all the dispatchable orders and resets the restocking date to a default value.
        /// </summary>
        /// <param name="allOrders">A list of all the orders stored in the database</param>
        /// <returns>No return because of async task method</returns>
        public async Task BatchAndProcess(List<Order> allOrders)
        {
            await ProcessOrders(allOrders);
            await ResetRestockDays(allOrders);
        }

        /// <summary>
        /// A method that adds missing stock of a product to fit the order requirement, and adds 20 extra items of the product as well.
        /// </summary>
        /// <param name="giveItMoreStock">The product that needs more stock</param>
        /// <param name="neededStock">the missing stock number</param>
        /// <returns>No return because of async task method</returns>
        public async Task AddMoreStock(Product giveItMoreStock, int neededStock)
        {
            giveItMoreStock.Stock = giveItMoreStock.Stock + neededStock + 20; //lägger även in 20 extra.
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that updates the order information.
        /// </summary>
        /// <param name="specOrder">The specific order</param>
        /// <returns>No return because of async task method</returns>
        public async Task UpdateOrder(Order specOrder)
        {
            _context.Orders.Update(specOrder);
            await _context.SaveChangesAsync();
            
        }

        /// <summary>
        /// A method that deletes a specific orderline in an order, and deletes the whole order if there are no items inside it.
        /// </summary>
        /// <param name="toBeDeleted">The orderline that should be deleted.</param>
        /// <returns>No return because of async task method</returns>
        public async Task DeleteItem(OrderLine toBeDeleted)
        {
            _context.OrderLines.Remove(toBeDeleted);
            if(toBeDeleted.Order.Items.Count() == 1) //vi raderar hela ordern om inga items finnsinuti
            {
                _context.Orders.Remove(toBeDeleted.Order);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer)
        {
            return await _context.Orders.Where(o => o.Dispatched == true && customer.Id == o.CustomerId).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<List<Order>> DisplayActiveCustomerOrder(Customer customer)
        {
            return await _context.Orders.Where(o => o.Dispatched == false && customer.Id == o.CustomerId).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payedOrNot"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task SetPayment(bool payedOrNot, Order order)
        {
            if (payedOrNot == false)
            {
                order.PaymentCompleted = true;
            }
            else
            {
                order.PaymentCompleted = false;
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task DeleteNoItemsOrders()
        {
            List<Order> noItemsOrders = new List<Order>();
            noItemsOrders = _context.Orders.Where(b => b.Items.Count() == 0).ToList();
            foreach (Order or in noItemsOrders)
            {
                _context.Orders.Remove(or);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetDispatched()
        {
            return await _context.Orders.Where(ord => ord.Dispatched == true).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task <List<Order>> GetPending()
        {
            List<Order> filtered = new List<Order>();
            List<OrderLine> shortestRestockDateOL = new List<OrderLine>();
            List<OrderLine> getPending = new List<OrderLine>();
            Product checkPro = new Product();

            filtered = await _context.Orders.Where(b => b.Dispatched == false).ToListAsync();
            foreach (Order ord in filtered)
            {
                getPending = await _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToListAsync();
                shortestRestockDateOL.AddRange(getPending);

            }
            shortestRestockDateOL = shortestRestockDateOL.OrderByDescending(s => s.Product.RestockingDate).ToList(); //får en lista av orderlines där vi sorterar utefter restockDate
                                                                                                                     //LÄGG in så att filtered är sorterade utefter restockDate
            List<Order> filtered2 = new List<Order>();
            foreach (OrderLine ordLine in shortestRestockDateOL) //filtrering så att vi får orders sorterade efter deras restocking date, eftersom orderlines med kortast restocking date kommer först i ordLine
            {
                Order filteredOrder = await _context.Orders.Where(o => o.Id == ordLine.OrderId).FirstOrDefaultAsync(); //Vi skapar en order för varje orderline
                filtered2.Add(filteredOrder);


            }
            filtered2 = filtered2.Distinct().ToList();
            filtered = filtered2;


            return filtered;
        }

        /** 
         ** Helper methods for Batch & Process
         **/

        private async Task ProcessOrders(List<Order> allOrders)
        {
            bool dispatchWholeOrder = true;
            List<int> stocksToReturn = new List<int>();
            List<Order> sortedByLongest = allOrders
                .Where(b => b.Dispatched == false && b.PaymentCompleted == true)
                .OrderBy(b => b.OrderDate)
                .ToList();

            foreach (Order ord in sortedByLongest)
            {
                stocksToReturn.Clear();
                List<OrderLine> checkProducts = await _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToListAsync();

                foreach (OrderLine ordLine in checkProducts)
                {
                    Product checkProduct = await _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefaultAsync();

                    int stockQuantity = await GetProcessedOrderQuantity(ordLine, checkProduct);
                    stocksToReturn.Add(stockQuantity);
                    dispatchWholeOrder = stockQuantity != 0;
                }

                 BatchOrders(dispatchWholeOrder, ord, checkProducts, stocksToReturn);
            }
            await _context.SaveChangesAsync();
        }

        private async Task<int> GetProcessedOrderQuantity(OrderLine takenQuantity, Product prod)
        {
            if (prod.Stock - takenQuantity.Quantity >= 0)
            {
                prod.Stock -= takenQuantity.Quantity;
                return takenQuantity.Quantity;
            }
            else
            {
                return await GetStockedProductAndQuantityNumber(takenQuantity, prod);
            }
        }

        private async Task<int> GetStockedProductAndQuantityNumber(OrderLine takenQuantity, Product prod)
        {
            if (prod.RestockingDate.ToString().Equals("0001-01-01 00:00:00"))
            {
                prod.RestockingDate = DateTime.Now.AddDays(10);
                return 0;
            }
            else 
            {
                if (DateTime.Now >= prod.RestockingDate) 
                {
                    int neededStock = prod.Stock - takenQuantity.Quantity;
                    neededStock = Math.Abs(neededStock);
                    await AddMoreStock(prod, neededStock);
                    prod.Stock -= takenQuantity.Quantity; 

                    return takenQuantity.Quantity;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void BatchOrders(bool dispatchWholeOrder, Order ord, List<OrderLine> checkProducts, List<int> stockList)
        {
            if (dispatchWholeOrder == true)
            {
                ord.Dispatched = true;
            }
            else
            {
                int counter = 0;
                 foreach (OrderLine ordLine2 in checkProducts)
                {
                    ordLine2.Product.Stock = ordLine2.Product.Stock + stockList[counter];
                    counter++;
                }
            }
        }

        private async Task ResetRestockDays(List<Order> resetRestockDays)
        {
            foreach (Order rest in resetRestockDays)
            {
                List<OrderLine> checkProducts = _context.OrderLines.Where(ol => ol.OrderId == rest.Id).ToList();
                foreach (OrderLine ordLine in checkProducts)
                {
                    Product checkPro = await _context.Products.Where(ol => ol.Id == ordLine.ProductId).FirstOrDefaultAsync();

                    if (DateTime.Now >= checkPro.RestockingDate)
                    {
                        checkPro.RestockingDate = DateTime.Parse("0001-01-01 00:00:00");
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

    }
}

