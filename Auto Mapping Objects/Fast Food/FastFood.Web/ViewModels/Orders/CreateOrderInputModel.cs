using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Orders
{
    public class CreateOrderInputModel
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Customer { get; set; }

        public int ItemId { get; set; }

        public int EmployeeId { get; set; }

        public string OrderType { get; set; }

        [Range(1, 10000)]
        public int Quantity { get; set; }
    }
}
