namespace FastFood.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;

    using Data;
    using ViewModels.Orders;
    using FastFood.Models;
    using AutoMapper.QueryableExtensions;
    using FastFood.Models.Enums;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = this.context.Items
                    .ProjectTo<CreateOrderItemViewModel>(mapper.ConfigurationProvider)
                    .ToList(),
                Employees = this.context.Employees
                    .ProjectTo<CreateOrderEmployeeViewModel>(mapper.ConfigurationProvider)
                    .ToList(),
            };

            return this.View(viewOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = mapper.Map<Order>(model);
            order.DateTime = DateTime.Now;
            OrderType orderType = Enum.Parse<OrderType>(model.OrderType);
            order.Type = orderType;
            context.Orders.Add(order);
            context.SaveChanges();

            order.OrderItems.Add(new OrderItem()
            {
                ItemId = model.ItemId,
                OrderId = order.Id,
                Quantity = model.Quantity
            });
            context.SaveChanges();

            return this.RedirectToAction("All", "Orders");
        }

        public IActionResult All()
        {
            var orders = this.context.Orders
                .ProjectTo<OrderAllViewModel>(mapper.ConfigurationProvider)
                .ToList();

            return View(orders);
        }
    }
}
