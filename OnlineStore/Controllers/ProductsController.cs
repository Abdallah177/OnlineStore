using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Dtos;
using OnlineStore.Entities;
using OnlineStore.Identity;

namespace OnlineStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreDbContext _dbContext;

        public ProductsController(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Products = await _dbContext.Products
                        .Select(p => new ProductDto
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            Stock = p.Stock,
                            OrderId = p.OrderId
                        })
                        .ToListAsync();

            return Ok(Products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _dbContext.Products
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    OrderId = p.OrderId
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return Ok("Product Added successfully.");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto productDto)
        {
            if (id != productDto.ProductId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ExistingProduct = await _dbContext.Products.FindAsync(id);
            if (ExistingProduct == null)
                return NotFound();

            ExistingProduct.Name = productDto.Name;
            ExistingProduct.Description = productDto.Description;
            ExistingProduct.Price = productDto.Price;
            ExistingProduct.Stock = productDto.Stock;
            
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A concurrency error occurred.");
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);

            if (product == null)
                return NotFound(); 

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return NoContent(); 
        }



    }
}
