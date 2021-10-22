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
        public void AddNewOrder(Order newOrder)
        {
        }
        public void AddNewOrderLine(OrderLine newOrderLine, Order matchingOrder)
        {
        }

        //en metod för att hämta alla Orders som finns i databasen

        public Task<List<Order>> GetOrdersAsync()
        {
            throw new Exception();
        }
        public async Task<List<OrderLine>> GetOrderLinesAsync()
        {
            throw new Exception();
        }

        //en metod för att uppdatera en existerande order

        public void UpdateOrder(List<Order> allOrders) //check att den stock och att den har payment completed om båda är sanna så blir det dispatched, om det inte är sant så blir den pending
        {

        }
        public void AddMoreStock( Product giveProStock, int neededStock)
        {

        }
        //Visa dispatched och pending orders separat (ska finnas mer info kring när saker kan skeppas osv)

        public List<Order> GetDispatchedAndPending(bool dispatched, bool paymentCompleted, int stock)
        {
            throw new Exception();
            //kolla så att ett objekt har paymentCompleted till true och att stocken är på 0
            //eller att dispatched är true

        }

    }
}
