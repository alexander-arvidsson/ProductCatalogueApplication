using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogueApplication.Data
{
    public class Order
    {
        private int _id;
        private int _customerId;
        private Customer _customer;
        private DateTime _orderDate;
        private string _deliveryAdress;
        private bool _paymentCompleted;
        private bool _dispatched;
        private List<OrderLine> _items;
        public Order()
        {
            Items = new List<OrderLine>();
        }

        /// <summary>
        /// The unique Order ID saved as an int.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [Range(0, int.MaxValue)]
        /// <summary>
        /// The customer ID saved as an int.
        /// </summary>
        public int CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        /// <summary>
        /// A customer saved as a Customer object.
        /// </summary>
        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        /// <summary>
        /// The date the order was made saved as a DateTime data type.
        /// </summary>
        public DateTime OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        [Required(ErrorMessage = "A delivery adress is required")]
        [StringLength(40, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        /// <summary>
        /// The delivery adress of the order saved as a string.
        /// </summary>
        public string DeliveryAdress
        {
            get { return _deliveryAdress; }
            set { _deliveryAdress = value; }
        }

        [Required]
        /// <summary>
        /// A bool that describes whether or not a payment has been completed.
        /// </summary>
        public bool PaymentCompleted
        {
            get { return _paymentCompleted; }
            set { _paymentCompleted = value; }
        }

        [Required]
        /// <summary>
        /// A bool that describes whether or not an order has been dispatched.
        /// </summary>
        public bool Dispatched
        {
            get { return _dispatched; }
            set { _dispatched = value; }
        }

        [Required]
        /// <summary>
        /// The orderline list of items in an order.
        /// </summary>
        public List<OrderLine> Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}
