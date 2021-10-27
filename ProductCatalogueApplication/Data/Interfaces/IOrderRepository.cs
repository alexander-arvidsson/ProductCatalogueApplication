using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalogueApplication.Data.Interfaces;


namespace ProductCatalogueApplication.Data.Interfaces
{
    interface IOrderRepository
    {

        //en metod för att addera en ny order
        Task AddNewOrder(Order newOrder);
        
        Task AddNewOrderLine(OrderLine newOrderLine, Order matchingOrder);
        

        //en metod för att hämta alla Orders som finns i databasen

        public Task<List<Order>> GetOrdersAsync()
        {
            throw new Exception();
        }

        public Task<List<Order>> GetOrdersAsync(int id)
        {
            throw new Exception();
        }
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            throw new Exception();
        }

        //en metod för att uppdatera en existerande order

        public void ProcessBatchorders(List<Order> allOrders) //check att den stock och att den har payment completed om båda är sanna så blir det dispatched, om det inte är sant så blir den pending
        {

        }
        public void UpdateOrder(Order specOrder) //check att den stock och att den har payment completed om båda är sanna så blir det dispatched, om det inte är sant så blir den pending
        {

        }
        void DeleteItem(OrderLine itemToBeDeleted);
        public void AddMoreStock( Product giveProStock, int neededStock)
        {

        }
        //Visa dispatched och pending orders separat (ska finnas mer info kring när saker kan skeppas osv)

        List<Order> GetDispatched(List<Order> allOrders);
        List<Order> GetPending(List<Order> allOrders);


        public void SetPayment(bool payedOrNot, Order order)
        {
           
        }

        public void deleteNoItemsOrders()
        {

        }

        Task<List<Order>> DisplayArchivedCustomerOrder(Customer customer);
        Task<List<Order>> DisplayActiveCustomerOrder(Customer customer);

    }
}
