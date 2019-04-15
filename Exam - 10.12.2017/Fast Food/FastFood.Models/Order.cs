using FastFood.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FastFood.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new List<OrderItem>();
        }

        public int Id { get; set; }

        [Required]
        public string Customer { get; set; }

        public DateTime DateTime { get; set; }

        public OrderType Type { get; set; } = OrderType.ForHere;

        [NotMapped]
        public decimal TotalPrice => this.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity);

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
