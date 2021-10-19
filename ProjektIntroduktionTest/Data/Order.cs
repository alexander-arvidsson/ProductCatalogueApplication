using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIntroduktionTest.Data
{
    public class Order
    {
        private int id;
        private int customerId;
        private Customer customer;
        private DateTime orderDate;
        private string deliveryAddress;
        private bool paymentCompleted;
        private bool dispatched;
        public List<OrderLine> items; //ändra till private

        public Order()
        {
            items = new List<OrderLine>();
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        [Required]
        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        [Required]
        public DateTime OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }

        [Required]
        [StringLength(30, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string DeliveryAddress
        {
            get { return deliveryAddress; }
            set { deliveryAddress = value; }
        }

        [Required]
        public bool PaymentCompleted
        {
            get { return paymentCompleted; }
            set { paymentCompleted = value; }
        }

        [Required]
        public bool Dispatched
        {
            get { return dispatched; }
            set { dispatched = value; }
        }

       // [Required]
        //public List<OrderLine> Items
        //{
        //    get { return items; }
       //     set { items = value; }
       // }






    }
}
