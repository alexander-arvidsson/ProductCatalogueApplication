using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalogueApplication.Data.Interfaces;


namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IOrderRepository
    {
        Task AddNewOrder(Order newOrder);

        Task AddNewOrderLine(OrderLine newOrderLine, Order matchingOrder);
        Task<List<Order>> GetOrdersAsync();

        Task<List<Order>> GetOrdersAsync(int id);

        Task<List<OrderLine>> GetOrderLinesAsync();

        Task BatchAndProcess(List<Order> allOrders);

        Task UpdateOrder(Order specOrder);

        Task DeleteItem(OrderLine itemToBeDeleted);
        Task AddMoreStock(Product giveProStock, int neededStock);

        Task <List<Order>> GetDispatched();

        Task <List<Order>> GetPending();

        Task SetPayment(bool payedOrNot, Order order);

        Task DeleteNoItemsOrders();

        Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer);

        Task<List<Order>> DisplayActiveCustomerOrder(Customer customer);

    }
}
