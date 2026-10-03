using OrderService.Application.DTOs;

namespace OrderService.Application.Interfaces
{
    public interface IProductClient
    {
        Task<ProductResponse?> GetProductAsync(Guid productId);
    }
}
