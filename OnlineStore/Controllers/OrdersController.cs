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
    public class OrdersController : ControllerBase
    {
        private readonly StoreDbContext _dbContext;

        public OrdersController(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Products)
                .Select(order => new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Products = order.Products.Select(p => new ProductsInOrderDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price                    
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Order = await _dbContext.Orders.Include(o => o.Products).FirstOrDefaultAsync(p => p.OrderId==id);

            if (Order == null)
                return NotFound();

            return Ok(Order);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var products = await _dbContext.Products
                .Where(p => dto.ProductIds.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != dto.ProductIds.Count)
                return BadRequest("One or more product IDs are invalid.");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = dto.OrderDate,
                TotalAmount = products.Sum(p => p.Price)
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            foreach (var product in products)
            {
                product.OrderId = order.OrderId;

                if (product.Stock > 0)
                {
                    product.Stock--;
                }
                else
                {
                    return BadRequest($"Product '{product.Name}' is out of stock.");
                }
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Order created successfully", orderId = order.OrderId });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDto dto)
        {
            if (id != dto.OrderId)
                return BadRequest("Invalid order ID.");

            var existingOrder = await _dbContext.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
                return NotFound("Order not found.");

            var products = await _dbContext.Products
                .Where(p => dto.ProductIds.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != dto.ProductIds.Count)
                return BadRequest("One or more product IDs are invalid.");

            foreach (var product in existingOrder.Products)
            {
                product.OrderId = null;
                product.Stock++; 
            }

            existingOrder.CustomerId = dto.CustomerId;
            existingOrder.OrderDate = dto.OrderDate;
            existingOrder.TotalAmount = products.Sum(p => p.Price);

            foreach (var product in products)
            {
                if (product.Stock <= 0)
                    return BadRequest($"Product '{product.Name}' is out of stock.");

                product.OrderId = existingOrder.OrderId;
                product.Stock--;
            }

            await _dbContext.SaveChangesAsync();
            return Ok("Order updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound("Order not found.");

            
            foreach (var product in order.Products)
            {
                product.OrderId = null;
                product.Stock++;
            }

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();

            return Ok("Order deleted successfully.");
        }






    }
}
