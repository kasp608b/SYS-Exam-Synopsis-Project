﻿using CustomerApiC.Commands;
using Microsoft.AspNetCore.Mvc;
using SharedModels.EventStoreCQRS;

namespace CustomerApiC.Controllers
{
    [Route("CustomerApiC")]
    [ApiController]
    public class CustomerApiCController : ControllerBase
    {
        ICommandHandler<CreateCustomer> _createCustomerCommandHandler;
        ICommandHandler<ChangeCustomerInfo> _changeCustomerInfoCommandHandler;
        ICommandHandler<ChangeCustomerCreditStanding> _changeCustomerCreditStandingCommandHandler;
        ICommandHandler<DeleteCustomer> _deleteCustomerCommandHandler;

        public CustomerApiCController(
            ICommandHandler<CreateCustomer> createCustomerCommandHandler,
            ICommandHandler<ChangeCustomerInfo> changeCustomerInfoCommandHandler,
            ICommandHandler<ChangeCustomerCreditStanding> changeCustomerCreditStandingCommandHandler,
            ICommandHandler<DeleteCustomer> deleteCustomerCommandHandler)
        {
            _createCustomerCommandHandler = createCustomerCommandHandler;
            _changeCustomerInfoCommandHandler = changeCustomerInfoCommandHandler;
            _changeCustomerCreditStandingCommandHandler = changeCustomerCreditStandingCommandHandler;
            _deleteCustomerCommandHandler = deleteCustomerCommandHandler;
        }


        //POST api/<CustomerApiCController>/CreateCustomer
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomer command)
        {
            try
            {
                if (command.Id == Guid.Empty)
                {
                    command.Id = Guid.NewGuid();
                }

                await _createCustomerCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (AggregateException ae)
            {
                try
                {
                    ae.Flatten().Handle(e =>
                    {
                        if (e is InvalidOperationException)
                        {
                            throw new InvalidOperationException($" An invalid operation exception was thrown with message: \n {e.Message}");
                        }
                        else if (e is Exception)
                        {
                            throw new Exception($" An exception was thrown with message: \n {e.Message}");
                        }
                        else
                        {
                            return false;
                        }
                    });

                }
                catch (InvalidOperationException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");

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

        //POST api/<CustomerApiCController>/ChangeCustomerInfo
        [HttpPost("ChangeCustomerInfo/{id}")]
        public async Task<IActionResult> ChangeCustomerInfo(Guid id, [FromBody] ChangeCustomerInfo command)
        {
            try
            {
                command.Id = id;
                await _changeCustomerInfoCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (AggregateException ae)
            {
                try
                {
                    ae.Flatten().Handle(e =>
                    {
                        if (e is InvalidOperationException)
                        {
                            throw new InvalidOperationException($" An invalid operation exception was thrown with message: \n {e.Message}");
                        }
                        else if (e is Exception)
                        {
                            throw new Exception($" An exception was thrown with message: \n {e.Message}");
                        }
                        else
                        {
                            return false;
                        }
                    });

                }
                catch (InvalidOperationException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");

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

        //POST api/<CustomerApiCController>/ChangeCustomerCreditStanding
        [HttpPost("ChangeCustomerCreditStanding/{id}")]
        public async Task<IActionResult> ChangeCustomerCreditStanding(Guid id, [FromBody] ChangeCustomerCreditStanding command)
        {
            try
            {
                command.Id = id;
                await _changeCustomerCreditStandingCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (AggregateException ae)
            {
                try
                {
                    ae.Flatten().Handle(e =>
                    {
                        if (e is InvalidOperationException)
                        {
                            throw new InvalidOperationException($" An invalid operation exception was thrown with message: \n {e.Message}");
                        }
                        else if (e is Exception)
                        {
                            throw new Exception($" An exception was thrown with message: \n {e.Message}");
                        }
                        else
                        {
                            return false;
                        }
                    });

                }
                catch (InvalidOperationException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");

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

        //DELETE api/<CustomerApiCController>/DeleteCustomer
        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id, [FromBody] DeleteCustomer command)
        {
            try
            {
                command.Id = id;
                await _deleteCustomerCommandHandler.HandleAsync(command);
                return Ok();
            }
            catch (AggregateException ae)
            {
                try
                {
                    ae.Flatten().Handle(e =>
                    {
                        if (e is InvalidOperationException)
                        {
                            throw new InvalidOperationException($" An invalid operation exception was thrown with message: \n {e.Message}");
                        }
                        else if (e is Exception)
                        {
                            throw new Exception($" An exception was thrown with message: \n {e.Message}");
                        }
                        else
                        {
                            return false;
                        }
                    });

                }
                catch (InvalidOperationException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");

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
