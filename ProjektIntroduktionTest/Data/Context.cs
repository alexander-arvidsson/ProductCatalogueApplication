using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIntroduktionTest.Data
{
    public class Context : DbContext
    {

        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Order>().HasMany(c => c.items).WithOne(i => i.Order); //En order har flera items genom Orderlists, varje orderlista är kopplad mot endast en order
            modelBuilder.Entity<Customer>().HasMany(c => c.orders).WithOne(i => i.Customer); //En kund har flera ordrar och varje order är bunden mot en kund
            modelBuilder.Entity<OrderLine>().HasOne(c => c.Product); //En orderline har en produkt

            modelBuilder.Entity<Customer>().HasData(GetCustomerSeededData());
            modelBuilder.Entity<Product>().HasData(GetProductSeededData());
            modelBuilder.Entity<Order>().HasData(GetOrderSeededData());
            modelBuilder.Entity<OrderLine>().HasData(GetOrderLineSeededData());

            base.OnModelCreating(modelBuilder);

        }

        private List<Customer> GetCustomerSeededData()
        {
            List<Customer> customers = new List<Customer> { new Customer() { Id = 1, Name = "Jonatan", Email = "testmail@gmail.com", Phone = "073 766 36 51" } };
            return customers;
        }
        private List<Product> GetProductSeededData()
        {
            DateTime restockDate = new DateTime(2022, 12, 31);
            List<Product> products = new List<Product> { new Product() { Id = 1, Name = "GameBoy Advance SP", Description = "Retro video game console", Price = 2000, RestockingDate = restockDate, Stock=10 } };
            return products;
        }
        private List<Order> GetOrderSeededData()
        {
            DateTime orderDate = DateTime.Now.Date;

            List<Order> orders = new List<Order> { new Order() { Id = 1, CustomerId = GetCustomerSeededData()[0].Id, DeliveryAddress="Studentvägen 8", Dispatched=false, PaymentCompleted=false, OrderDate= orderDate } };
            return orders;
        }
        private List<OrderLine> GetOrderLineSeededData() //tänk på att istället för att lägga in objekt så ska nycklarna matchas
        {
            List<OrderLine> orderLine = new List<OrderLine> { new OrderLine() { Id = 1, OrderId=GetOrderSeededData()[0].Id, ProductId=GetProductSeededData()[0].Id, Quantity=10 } };
            return orderLine;
        }
    }
}
