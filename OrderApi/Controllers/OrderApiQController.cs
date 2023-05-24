using Microsoft.AspNetCore.Mvc;
using OrderApiQ.Data;
using OrderApiQ.Models.Converters;
using SharedModels;

namespace OrderApiQ.Controllers
{
    [ApiController]
    [Route("OrderApiQ")]
    public class OrderApiQController : ControllerBase
    {
        IOrderRepository repository;
        private readonly IConverter<Order, OrderDto> OrderConverter;

        public OrderApiQController(IOrderRepository repos,
            IConverter<Order, OrderDto> orderconverter
            )
        {
            repository = repos;
            OrderConverter = orderconverter;
        }

        // GET: orders
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var orderDtoList = new List<OrderDto>();
                foreach (var order in repository.GetAll())
                {
                    var orderDto = OrderConverter.Convert(order);
                    orderDtoList.Add(orderDto);
                }

                return new ObjectResult(orderDtoList);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(Guid id)
        {
            Order item;

            try
            {
                item = repository.Get(id);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }

            if (item == null)
            {
                return NotFound();
            }
            var orderDto = OrderConverter.Convert(item);
            return new ObjectResult(orderDto);
        }

        // GET orders/5
        [HttpGet("GetByCustomer/{id}", Name = "GetOrderByCustomer")]
        public IActionResult GetbyCustomer(Guid id)
        {
            var orderDtoList = new List<OrderDto>();

            IEnumerable<Order> orderList;

            try
            {
                orderList = repository.GetByCustomer(id);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }


            if (orderList.Count() <= 0) return NotFound("No matching orders found");

            foreach (var order in orderList)
            {
                var orderDto = OrderConverter.Convert(order);
                orderDtoList.Add(orderDto);
            }
            return Ok(orderDtoList);

        }

        // GET orders/product/5
        // This action method was provided to support request aggregate
        // "Orders by product" in OnlineRetailerApiGateway.
        [HttpGet("product/{id}", Name = "GetOrderByProduct")]
        public IEnumerable<Order> GetByProduct(Guid id)
        {
            List<Order> ordersWithSpecificProduct = new List<Order>();

            foreach (var order in repository.GetAll())
            {
                if (order.Orderlines.Where(o => o.ProductId == id).Any())
                {
                    ordersWithSpecificProduct.Add(order);
                }
            }

            return ordersWithSpecificProduct;
        }
    }

}
