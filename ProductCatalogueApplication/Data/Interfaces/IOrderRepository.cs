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

        public void Batchorders(List<Order> allOrders) //check att den stock och att den har payment completed om båda är sanna så blir det dispatched, om det inte är sant så blir den pending
        {

        }
        public void UpdateOrder(Order specOrder) //check att den stock och att den har payment completed om båda är sanna så blir det dispatched, om det inte är sant så blir den pending
        {

        }
        public void AddMoreStock( Product giveProStock, int neededStock)
        {

        }
        //Visa dispatched och pending orders separat (ska finnas mer info kring när saker kan skeppas osv)

        public List<Order> GetDispatchedAndPending(List<Order> allOrders, bool choice)
        {
            //dispatched är bara de som är dispatch true och pending är de ordrar till är kopplade till produkter som har restocking date (inte riktigt sant dock eftersom den kan vara en mindre order som räcker med stock)
            //pending är där paymentCompleted är sant men quantity är mer är stock
            throw new Exception();

        }

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
