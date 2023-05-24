using Microsoft.AspNetCore.Mvc;
using OrderApiC.Commands;
using OrderApiC.Infrastructure;
using OrderApiC.Models.Converters;
using SharedModels;
using SharedModels.EventStoreCQRS;

namespace OrderApiC.Controllers
{
    public class OrderApiCController : Controller
    {
        ICommandHandler<CancelOrder> _cancelOrderCommandHandler;
        ICommandHandler<CompleteOrder> _completeOrderCommandHandler;
        ICommandHandler<CreateOrder> _createOrderCommandHandler;
        ICommandHandler<DeleteOrder> _deleteOrderCommandHandler;
        ICommandHandler<PayforOrder> _payforOrderCommandHandler;
        ICommandHandler<ShipOrder> _shipOrderCommandHandler;
        IServiceGateway<ProductDto> productServiceGateway;
        IServiceGateway<CustomerDto> _customerGateway;
        IMessagePublisher messagePublisher;

        public OrderApiCController(ICommandHandler<CancelOrder> cancelOrderCommandHandler,
            ICommandHandler<CompleteOrder> completeOrderCommandHandler,
            ICommandHandler<CreateOrder> createOrderCommandHandler,
            ICommandHandler<DeleteOrder> deleteOrderCommandHandler,
            ICommandHandler<PayforOrder> payforOrderCommandHandler,
            ICommandHandler<ShipOrder> shipOrderCommandHandler,
            IServiceGateway<ProductDto> productServiceGateway,
            IServiceGateway<CustomerDto> customerGateway,
            IMessagePublisher messagePublisher)
        {
            _cancelOrderCommandHandler = cancelOrderCommandHandler;
            _completeOrderCommandHandler = completeOrderCommandHandler;
            _createOrderCommandHandler = createOrderCommandHandler;
            _deleteOrderCommandHandler = deleteOrderCommandHandler;
            _payforOrderCommandHandler = payforOrderCommandHandler;
            _shipOrderCommandHandler = shipOrderCommandHandler;
            this.productServiceGateway = productServiceGateway;
            _customerGateway = customerGateway;
            this.messagePublisher = messagePublisher;
        }



        // POST CreateOrder
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrder createOrder)
        {
            Console.WriteLine("Order post called");
            var customer = _customerGateway.Get(createOrder.CustomerId);


            if (customer.CustomerId != createOrder.CustomerId || customer.CustomerId == Guid.Empty)
            {
                return BadRequest("The customer  does not exist");
            }

            if (createOrder == null)
            {
                return BadRequest();
            }

            if (!customer.CreditStanding)
            {
                OrderDto orderDto = new OrderDto
                {
                    CustomerId = createOrder.CustomerId,
                    Date = createOrder.Date,
                    Orderlines = createOrder.OrderLines.Select(x => new OrderlineConverter().Convert(x)).ToList(),
                    Status = new OrderStatusConverter().Convert(createOrder.Status),

                };

                // Publish OrderRejectedMessage.
                messagePublisher.PublishOrderRejectedMessage(orderDto, "The customer has outstanding bills");

                return BadRequest("The customer has outstanding bills");
            }

            if (ProductItemsAvailable(createOrder))
            {
                try
                {


                    // Create order.
                    createOrder.Status = OrderStatus.completed;

                    try
                    {
                        if (createOrder.Id == Guid.Empty)
                        {
                            createOrder.Id = Guid.NewGuid();
                        }

                        await _createOrderCommandHandler.HandleAsync(createOrder);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                    }

                    // Publish OrderStatusChangedMessage. If this operation
                    // fails, the order will not be created
                    messagePublisher.PublishOrderStatusChangedMessage(
                        createOrder.CustomerId, createOrder.OrderLines.Select(x => new OrderlineConverter().Convert(x)).ToList(), "completed");

                    return Ok();
                }
                catch
                {
                    OrderDto orderDto = new OrderDto
                    {
                        CustomerId = createOrder.CustomerId,
                        Date = createOrder.Date,
                        Orderlines = createOrder.OrderLines.Select(x => new OrderlineConverter().Convert(x)).ToList(),
                        Status = new OrderStatusConverter().Convert(createOrder.Status),

                    };

                    // Publish OrderRejectedMessage.
                    messagePublisher.PublishOrderRejectedMessage(orderDto, "An error happened. Try again.");
                    return StatusCode(500, "An error happened. Try again.");
                }
            }
            else
            {

                OrderDto orderDto = new OrderDto
                {
                    CustomerId = createOrder.CustomerId,
                    Date = createOrder.Date,
                    Orderlines = createOrder.OrderLines.Select(x => new OrderlineConverter().Convert(x)).ToList(),
                    Status = new OrderStatusConverter().Convert(createOrder.Status),

                };

                // Publish OrderRejectedMessage.
                messagePublisher.PublishOrderRejectedMessage(orderDto, "Not enough items in stock.");

                // If there are not enough product items available.
                return StatusCode(500, "Not enough items in stock.");
            }
        }

        // PUT orderApiC/5/cancelOrder
        // This action method cancels an order and publishes an OrderStatusChangedMessage
        // with topic set to "cancelled".
        [HttpPut("{id}/cancelOrder")]
        public IActionResult Cancel(Guid id, [FromBody] CancelOrder cancelOrder)
        {

            try
            {
                _cancelOrderCommandHandler.HandleAsync(cancelOrder);
                return StatusCode(200, "Order cancelled");



            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }

        }

        // PUT orderApiC/5/shipOrder
        [HttpPut("{id}/shipOrder")]
        public IActionResult Ship(Guid id, [FromBody] ShipOrder shipOrder)
        {
            try
            {
                _shipOrderCommandHandler.HandleAsync(shipOrder);
                return StatusCode(200, "Order shipped");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }
        }

        [HttpPut("{id}/payforOrder")]
        public IActionResult PayForOrder(Guid id, [FromBody] PayforOrder payforOrder)
        {
            try
            {
                _payforOrderCommandHandler.HandleAsync(payforOrder);
                return StatusCode(200, "Order paid for");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }
        }




        private bool ProductItemsAvailable(CreateOrder createOrder)
        {
            foreach (var orderLine in createOrder.OrderLines)
            {
                // Call product service to get the product ordered.
                var orderedProduct = productServiceGateway.Get(orderLine.ProductId);
                if (orderLine.NoOfItems > orderedProduct.ItemsInStock - orderedProduct.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
