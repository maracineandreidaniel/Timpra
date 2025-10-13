using Dashboard.Core.Extensions;
using Timpra.BusinessLogic.DTOs;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.DataAccess.Entities;
using Timpra.DataAccess.Repository.Abstraction;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timpra.BusinessLogic.Mappers;
using System;
using Microsoft.AspNetCore.Http;

namespace Timpra.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly RedisCacheService _redisCacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IRepository<Order> orderRepository, RedisCacheService redisCacheService, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _redisCacheService = redisCacheService;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IEnumerable<OrderDto>> GetAll()
        {
            var orders = await this.GetAllOrders();
            return orders.ToList();
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            var instanceId = GetInstanceId();
            var cacheKey = $"Orders_Cache_{instanceId}";

            var orders = _redisCacheService.GetCachedData<List<Order>>(cacheKey);
            if (orders is null)
            {
                var query = await _orderRepository.GetAll();
                orders = query
                    .Where(o => !o.IsDeleted)
                    .ToList();
                _redisCacheService.SetCachedData(cacheKey, orders, TimeSpan.FromMinutes(3));
            }

            return orders.MapToDto();
        }

        public string GetInstanceId()
        {
            _httpContextAccessor.HttpContext.Session.TryGetValue("InstanceId", out var instanceId);
            if (instanceId is null)
            {
                instanceId = Guid.NewGuid().ToByteArray();
               _httpContextAccessor.HttpContext.Session.Set("InstanceId", instanceId); 

            }

            return Convert.ToBase64String(instanceId);
        }

        public async Task<OrderDto> GetByIdAsync(int id, bool applyChanges = true)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order.MapToDto();
        }

        public async Task<OrderDto> AddAsync(OrderDto item, bool applyChanges = true)
        {
            var order = item.MapFromDto();
            await _orderRepository.AddAsync(order);

            var instanceId = GetInstanceId();
            var cacheKey = $"Orders_Cache_{instanceId}";
            _redisCacheService.RemoveCache(cacheKey);

            return order.MapToDto();
        }

        public async Task<OrderDto> UpdateAsync(OrderDto item, int id, bool applyChanges = true)
        {
            var order = item.MapFromDto();
            await _orderRepository.UpdateAsync(order, id);

            var instanceId = GetInstanceId();
            var cacheKey = $"Orders_Cache_{instanceId}";
            _redisCacheService.RemoveCache(cacheKey);

            return item;
        }

        public async Task<OrderDto> RemoveAsync(int orderId, bool applyChanges = true)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                order.IsDeleted = true;
                await _orderRepository.UpdateAsync(order, orderId);

                var instanceId = GetInstanceId();
                var cacheKey = $"Orders_Cache_{instanceId}";
                _redisCacheService.RemoveCache(cacheKey);

                return order.MapToDto();
            }

            return null;
        }

        public async Task<OrderDto> ArchiveAsync(int orderId, bool applyChanges = true)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                order.IsActive = false;
                await _orderRepository.UpdateAsync(order, orderId);

                var instanceId = GetInstanceId();
                var cacheKey = $"Orders_Cache_{instanceId}";
                _redisCacheService.RemoveCache(cacheKey);

                return order.MapToDto();
            }

            return null;
        }

        public async Task<PaginatedListResponseDto<OrderDto>> GetOrdersPaginatedAsync(int pageIndex, int itemsNumber,
            string sortField, string sortDirection, OrderListFilterDto filter, bool applyChanges = true)
        {
            filter.SearchTerm = filter.SearchTerm?.Trim();
            var baseQuery = await _orderRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                baseQuery = baseQuery.Where(o =>
                    o.Number.Contains(filter.SearchTerm) || o.Client.Contains(filter.SearchTerm));
            }

            baseQuery = baseQuery.Where(o => !o.IsDeleted);
            
            var itemsCount = baseQuery.Count();

            var items = baseQuery
                .OrderByDynamic(sortField, sortDirection)
                .Skip(pageIndex * itemsNumber)
                .Take(itemsNumber)
                .ToList()
                .MapToDto();

            return new PaginatedListResponseDto<OrderDto> { RowsCount = itemsCount, Data = items };
        }
    }
}