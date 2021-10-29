using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalogueApplication.Data.Interfaces;


namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IOrderRepository
    {
        /// <summary>
        /// Method that adds an order.
        /// </summary>
        Task AddNewOrder(Order newOrder);

        /// <summary>
        /// Method that adds an orderline to an order.
        /// </summary>
        Task AddNewOrderLine(OrderLine newOrderLine, Order matchingOrder);

        /// <summary>
        /// Method to get all the orders in a list.
        /// </summary>
        Task<List<Order>> GetOrdersAsync();

        /// <summary>
        /// Method to get all the orders matched with an id in a list.
        /// </summary>
        Task<List<Order>> GetOrdersAsync(int id);

        /// <summary>
        /// Method to get all the orderlines in a list.
        /// </summary>
        Task<List<OrderLine>> GetOrderLinesAsync();

        /// <summary>
        /// Method to dispatch the orders.
        /// </summary>
        Task BatchAndProcess(List<Order> allOrders);

        /// <summary>
        /// Method to update the order information.
        /// </summary>
        Task UpdateOrder(Order specOrder);

        /// <summary>
        /// Method to delete items from an order.
        /// </summary>
        Task DeleteItem(OrderLine itemToBeDeleted);

        /// <summary>
        /// Method that adds more stock to a product.
        /// </summary>
        Task AddMoreStock(Product giveProStock, int neededStock);

        /// <summary>
        /// Method to list all the dispatched orders.
        /// </summary>
        Task<List<Order>> GetDispatched();

        /// <summary>
        /// Method to list all the undispatched orders.
        /// </summary>
        Task<List<Order>> GetPending();

        /// <summary>
        /// Method to switch the value of a paid or unpaid order.
        /// </summary>
        Task SetPayment(bool payedOrNot, Order order);

        /// <summary>
        /// Method that deletes the orders without any items in it.
        /// </summary>
        Task DeleteNoItemsOrders();

        /// <summary>
        /// Method that displayes all the dispatched orders.
        /// </summary>
        Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer);

        /// <summary>
        /// Method that displayes all the undispatched orders.
        /// </summary>
        Task<List<Order>> DisplayActiveCustomerOrder(Customer customer);

    }
}
