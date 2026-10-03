using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Service;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.DBContext;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IProductClient _productClient;
        private readonly RabbitMqPublisher _publisher;
        public OrdersController(
            OrderDbContext dbContext,
            IProductClient productClient,
            RabbitMqPublisher publisher)
        {
            _dbContext = dbContext;
            _productClient = productClient;
            _publisher = publisher;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(
            CreateOrderRequest request)
        {
            var product =
                await _productClient.GetProductAsync(
                    request.ProductId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (product.StockQuantity < request.Quantity)
            {
                return BadRequest("Insufficient stock");
            }
            var userId =
             User.FindFirst(
        ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var order = new Order
            {
                Id = Guid.NewGuid(),

                ProductId = request.ProductId,

                UserId = Guid.Parse(userId), // JWT later

                Quantity = request.Quantity,

                UnitPrice = product.Price,

                TotalPrice =
                    product.Price * request.Quantity,

                Status = "Created",

                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Orders.Add(order);

            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(
            new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt
            });

            return Ok(order);
        }
        [HttpGet("test-product/{id}")]
        public async Task<IActionResult> TestProduct(Guid id)
        {
            var product = await _productClient.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
