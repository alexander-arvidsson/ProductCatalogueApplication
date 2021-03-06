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
        /// A method that matches customerID to orders from the database.
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
        /// <returns>Orderline-list</returns>
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            return await _context.OrderLines.ToListAsync();
        }

        /// <summary>
        /// A method that adds a new order linked to a customer ID.
        /// </summary>
        /// <param name="newOrder">An object from the Order class.</param>
        /// <returns>No return because of async task method</returns>
        public async Task AddNewOrder(Order newOrder)
        {
            newOrder.OrderDate = DateTime.Now;
            newOrder.Customer = await _context.Customers.Where(ol => ol.Id == newOrder.CustomerId).FirstOrDefaultAsync();
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that adds a new orderline to an already existing order.
        /// If the product already exists in the order then the number of the same product added will be added to the quantity.
        /// If not then a new orderline of that product will simply be added.
        /// </summary>
        /// <param name="newOrderLine">An object from the OrderLine class.</param>
        /// <param name="OrderToMatch">An object from the Order class</param>
        /// <returns>No return because of async task method</returns>
        public async Task AddNewOrderLine(OrderLine newOrderLine, Order OrderToMatch)
        {
            newOrderLine.OrderId = OrderToMatch.Id;
            newOrderLine.Order = await _context.Orders.Where(ol => ol.Id == newOrderLine.OrderId).FirstOrDefaultAsync();

            newOrderLine.Product = await _context.Products.Where(ol => ol.Id == newOrderLine.ProductId).FirstOrDefaultAsync();

            bool noDuplicateProduct = true; //Bool that becomes false if the product already exists in the order.

            foreach (OrderLine ol in OrderToMatch.Items)
            {
                if (ol.Product == newOrderLine.Product)
                {
                    ol.Quantity += newOrderLine.Quantity; //If the product already exists in the order, the quantity is added.
                    noDuplicateProduct = false;
                    await _context.SaveChangesAsync();
                }
            }
            if (noDuplicateProduct == true) //If the product doesn't exist in the order, a new product is added.
            {
                await _context.OrderLines.AddAsync(newOrderLine);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that processes all the pending orders and resets the restocking date to a default value.
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
            giveItMoreStock.Stock = giveItMoreStock.Stock + neededStock + 20; //Also adds 20 extra stock for convenience.
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
            if (toBeDeleted.Order.Items.Count() == 1)
            {
                _context.Orders.Remove(toBeDeleted.Order);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// A method that displayes all the dispatched customer orders.
        /// </summary>
        /// <param name="customer">A customer object</param>
        /// <returns>A list of all the dispatched customer orders</returns>
        public async Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer)
        {
            return await _context.Orders.Where(o => o.Dispatched == true && customer.Id == o.CustomerId).ToListAsync();
        }

        /// <summary>
        /// A method that displayes all the pending customer orders.
        /// </summary>
        /// <param name="customer">A customer object</param>
        /// <returns>A list of all the pending customer orders</returns>
        public async Task<List<Order>> DisplayActiveCustomerOrder(Customer customer)
        {
            return await _context.Orders.Where(o => o.Dispatched == false && customer.Id == o.CustomerId).ToListAsync();
        }

        /// <summary>
        /// A method that switches the boolean value of the payment from false to true and vice versa.
        /// </summary>
        /// <param name="payedOrNot">a bool that's true if paid, false if not payed</param>
        /// <param name="order">An order object</param>
        /// <returns>Nothing because of async Task method type</returns>
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
        /// A method that deletes orders with 0 items in it.
        /// </summary>
        /// <returns>Nothing because of async Task method</returns>
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
        /// A method that finds all the dispatched orders.
        /// </summary>
        /// <returns>A list of all the dispatched orders.</returns>
        public async Task<List<Order>> GetDispatched()
        {
            return await _context.Orders.Where(ord => ord.Dispatched == true).ToListAsync();
        }

        /// <summary>
        /// A method that filters out all the pending orders and then checks each order and sorts
        /// them on which orderline has the shortest restocking date. The order with the closest restocking date will be at the top.
        /// </summary>
        /// <returns>The filtered and sorted list of pending orders</returns>
        public async Task<List<Order>> GetPending()
        {
            List<Order> filtered = new List<Order>();
            List<OrderLine> shortestRestockDateOL = new List<OrderLine>();
            List<OrderLine> getPending = new List<OrderLine>();
            List<Order> unique = new List<Order>();

            filtered = await _context.Orders.Where(b => b.Dispatched == false).ToListAsync();
            foreach (Order ord in filtered)
            {
                getPending = await _context.OrderLines.Where(ol => ol.OrderId == ord.Id).ToListAsync();
                shortestRestockDateOL.AddRange(getPending);
            }
            shortestRestockDateOL = shortestRestockDateOL.OrderByDescending(s => s.Product.RestockingDate).ToList(); //Sorts all orderlines by highest to lowest restock date.

            foreach (OrderLine filtOrd in shortestRestockDateOL) //We extract one order from every orderline but avoid duplicates.
            {
                if (!unique.Contains(filtOrd.Order))
                {
                    unique.Add(filtOrd.Order);
                }
            }
            filtered = unique;
            filtered.Reverse(); //We reverse the list so that the order with the closest restock date is at the top.
            return filtered;
        }

        /** 
         ** Helper methods for Batch & Process
         **/

        //Processes all orders, wether they are supposed to be dispatched or not.
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

        //Calculates wether there is enough stock for the orderline to be dispatched.
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

        //Assigns restocking date and if the restocking date has arrived we return the stock required for the process.
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

        //Checks if we can dispatch the order, if we can not dispatch the order the stock will be returned.
        private void BatchOrders(bool dispatchWholeOrder, Order ord, List<OrderLine> itemsToReturn, List<int> stockList)
        {
            if (dispatchWholeOrder == true)
            {
                ord.Dispatched = true;
            }
            else
            {
                int counter = 0;
                foreach (OrderLine ordLine2 in itemsToReturn)
                {
                    ordLine2.Product.Stock = ordLine2.Product.Stock + stockList[counter];
                    counter++;
                }
            }
        }

        //If restock date has arrived we reset the restock date.
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

