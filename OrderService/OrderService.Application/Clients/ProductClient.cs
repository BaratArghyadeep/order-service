using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Clients
{
    public class ProductClient : IProductClient
    {
        private readonly HttpClient _httpClient;

        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductResponse?> GetProductAsync(
            Guid productId)
        {
            return await _httpClient
                .GetFromJsonAsync<ProductResponse>(
                    $"api/products/{productId}");
        }
    }
}
