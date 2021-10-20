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

        [Required]
        [Range(1, int.MaxValue)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        [Required]
        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        [Required]
        public DateTime OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        [Required]
        [StringLength(30, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string DeliveryAdress
        {
            get { return _deliveryAdress; }
            set { _deliveryAdress = value; }
        }

        [Required]
        public bool PaymentCompleted
        {
            get { return _paymentCompleted; }
            set { _paymentCompleted = value; }
        }

        [Required]
        public bool Dispatched
        {
            get { return _dispatched; }
            set { _dispatched = value; }
        }

        [Required]
        public List<OrderLine> Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}
