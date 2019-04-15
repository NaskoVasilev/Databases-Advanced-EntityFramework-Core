namespace FastFood.Web.ViewModels.Orders
{
    using System.Collections.Generic;

    public class CreateOrderViewModel
    {
        public List<CreateOrderItemViewModel> Items { get; set; }

        public List<CreateOrderEmployeeViewModel> Employees { get; set; }
    }
}
