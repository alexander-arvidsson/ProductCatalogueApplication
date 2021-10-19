using System;
using System.Collections.Generic;

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

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public DateTime OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        public string DeliveryAdress
        {
            get { return _deliveryAdress; }
            set { _deliveryAdress = value; }
        }

        public bool PaymentCompleted
        {
            get { return _paymentCompleted; }
            set { _paymentCompleted = value; }
        }

        public bool Dispatched
        {
            get { return _dispatched; }
            set { _dispatched = value; }
        }

        public List<OrderLine> Items
        {
            get { return _items; }
            set { _items = value; }
        }

    }
}
