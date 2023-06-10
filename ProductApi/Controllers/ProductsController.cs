﻿using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("ProductApiQ")]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> repository;
        private IConverter<Product, ProductDto> productConverter;

        public ProductsController(IRepository<Product> repos, IConverter<Product, ProductDto> converter)
        {
            repository = repos;
            productConverter = converter;
        }

        // GET products
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var productDtoList = new List<ProductDto>();
                foreach (var product in repository.GetAll())
                {
                    var productDto = productConverter.Convert(product);
                    productDtoList.Add(productDto);
                }
                return new ObjectResult(productDtoList);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }

        }

        // GET products/5
        [HttpGet("{id}", Name = "GetProduct")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var item = repository.Get(id);
                if (item == null)
                {
                    return NotFound();
                }

                var productDto = productConverter.Convert(item);
                return new ObjectResult(productDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }
        }
/*
        // POST products
        [HttpPost]
        public IActionResult Post([FromBody] ProductDto productDto)
        {
            try
            {
                if (productDto == null)
                {
                    return BadRequest();
                }

                var product = productConverter.Convert(productDto);
                var newProduct = repository.Add(product);

                return CreatedAtRoute("GetProduct", new { id = newProduct.ProductId }, productConverter.Convert(newProduct));

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }

        }
*/
/*

        // PUT products/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] ProductDto productDto)
        {
            try
            {
                if (productDto == null || productDto.ProductId != id)
                {
                    return BadRequest();
                }

                var modifiedProduct = repository.Get(id);

                if (modifiedProduct == null)
                {
                    return NotFound();
                }

                modifiedProduct.Name = productDto.Name;
                modifiedProduct.Category = modifiedProduct.Category;
                modifiedProduct.Price = productDto.Price;
                modifiedProduct.ItemsInStock = productDto.ItemsInStock;
                modifiedProduct.ItemsReserved = productDto.ItemsReserved;


                repository.Edit(modifiedProduct);
                return new NoContentResult();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }

        }
*/
/*
        // DELETE products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                if (repository.Get(id) == null)
                {
                    return NotFound();
                }

                repository.Remove(id);
                return new NoContentResult();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong" + $"{ex.Message}");
            }

        }
*/
    }
}
