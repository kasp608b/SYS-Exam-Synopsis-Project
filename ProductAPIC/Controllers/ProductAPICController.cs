using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPIC.Command;
using ProductAPIC.CommandHandlers;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;

namespace ProductAPIC.Controllers
{
    [Route("api/ProductAPIC")]
    [ApiController]
    public class ProductAPICController : ControllerBase
    {
        ICommandHandler<AddItemsToStock> _addItemsToStockCommandHandler;
        ICommandHandler<ChangeProductCategory> _changeProductCategoryCommandHandler;
        ICommandHandler<ShipProduct> _shipProductCommandHandler;
        ICommandHandler<ChangeProductName> _changeProductNameCommandHandler;
        ICommandHandler<ChangeProductPrice> _changeProductPriceCommandHandler;
        ICommandHandler<CreateProduct> _createProductCommandHandler;
        ICommandHandler<DecreaseReservedItems> _decreaseReservedItemsCommandHandler;
        ICommandHandler<DeleteProduct> _deleteProductCommandHandler;
        ICommandHandler<IncreaseReservedItems> _increaseReservedItemsCommandHandler;
        ICommandHandler<RemoveItemsFromStock> _removeItemsFromStockCommandHandler;
        
        public ProductAPICController(
            ICommandHandler<AddItemsToStock> addItemsToStockCommandHandler,
            ICommandHandler<ChangeProductCategory> changeProductCategoryCommandHandler,
            ICommandHandler<ShipProduct> shipProductCommandHandler,
            ICommandHandler<ChangeProductName> changeProductNameCommandHandler,
            ICommandHandler<ChangeProductPrice> changeProductPriceCommandHandler,
            ICommandHandler<CreateProduct> createProductCommandHandler,
            ICommandHandler<DecreaseReservedItems> decreaseReservedItemsCommandHandler,
            ICommandHandler<DeleteProduct> deleteProductCommandHandler,
            ICommandHandler<IncreaseReservedItems> increaseReservedItemsCommandHandler,
            ICommandHandler<RemoveItemsFromStock> removeItemsFromStockCommandHandler
            )
        {
            _addItemsToStockCommandHandler = addItemsToStockCommandHandler;
            _changeProductCategoryCommandHandler = changeProductCategoryCommandHandler;
            _shipProductCommandHandler = shipProductCommandHandler;
            _changeProductNameCommandHandler = changeProductNameCommandHandler;
            _changeProductPriceCommandHandler = changeProductPriceCommandHandler;
            _createProductCommandHandler = createProductCommandHandler;
            _decreaseReservedItemsCommandHandler = decreaseReservedItemsCommandHandler;
            _deleteProductCommandHandler = deleteProductCommandHandler;
            _increaseReservedItemsCommandHandler = increaseReservedItemsCommandHandler;
            _removeItemsFromStockCommandHandler = removeItemsFromStockCommandHandler;

        }

        // POST api/<ProductAPICController>/CreateProduct
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct command)
        {
            try
            {
                if (command.Id == Guid.Empty)
                {
                    command.Id = Guid.NewGuid();
                }
                
                await _createProductCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex) 
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/ChangeProductName
        [HttpPut("ChangeProductName/{id}")]
        public async Task<IActionResult> ChangeProductName(Guid id, [FromBody] ChangeProductName command)
        {
            try
            {
                command.Id = id;
                await _changeProductNameCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/ChangeProductPrice
        [HttpPut("ChangeProductPrice/{id}")]
        public async Task<IActionResult> ChangeProductPrice(Guid id, [FromBody] ChangeProductPrice command)
        {
            try
            {
                command.Id = id;
                await _changeProductPriceCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/ChangeProductCategory
        [HttpPut("ChangeProductCategory/{id}")]
        public async Task<IActionResult> ChangeProductCategory(Guid id, [FromBody] ChangeProductCategory command)
        {
            try
            {
                command.Id = id;
                await _changeProductCategoryCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/ShipProduct
        [HttpPut("ShipProduct/{id}")]
        public async Task<IActionResult> ShipProduct(Guid id, [FromBody] ShipProduct command)
        {
            try
            {
                command.Id = id;
                await _shipProductCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/AddItemsToStock
        [HttpPut("AddItemsToStock/{id}")]
        public async Task<IActionResult> AddItemsToStock(Guid id, [FromBody] AddItemsToStock command)
        {
            try
            {
                command.Id = id;
                await _addItemsToStockCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/RemoveItemsFromStock
        [HttpPut("RemoveItemsFromStock/{id}")]
        public async Task<IActionResult> RemoveItemsFromStock(Guid id, [FromBody] RemoveItemsFromStock command)
        {
            try
            {
                command.Id = id;
                await _removeItemsFromStockCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/IncreaseReservedItems
        [HttpPut("IncreaseReservedItems/{id}")]
        public async Task<IActionResult> IncreaseReservedItems(Guid id, [FromBody] IncreaseReservedItems command)
        {
            try
            {
                command.Id = id;
                await _increaseReservedItemsCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT api/<ProductAPICController>/DecreaseReservedItems
        [HttpPut("DecreaseReservedItems/{id}")]
        public async Task<IActionResult> DecreaseReservedItems(Guid id, [FromBody] DecreaseReservedItems command)
        {
            try
            {
                command.Id = id;
                await _decreaseReservedItemsCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE api/<ProductAPICController>/DeleteProduct
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id, [FromBody] DeleteProduct command)
        {
            try
            {
                command.Id = id;
                await _deleteProductCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

    }
}
