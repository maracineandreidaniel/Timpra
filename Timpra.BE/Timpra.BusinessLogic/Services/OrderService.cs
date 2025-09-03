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

namespace Timpra.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAll()
        {
            var orders = await _orderRepository.GetAll();
            return orders.Where(order => !order.IsDeleted).ProjectToDto();
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

            return order.MapToDto();
        }

        public async Task<OrderDto> UpdateAsync(OrderDto item, int id, bool applyChanges = true)
        {
            var order = item.MapFromDto();
            await _orderRepository.UpdateAsync(order, id);

            return item;
        }

        public async Task<OrderDto> RemoveAsync(int orderId, bool applyChanges = true)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                order.IsDeleted = true;
                await _orderRepository.UpdateAsync(order, orderId);

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
                .Select(o => new OrderDto()
                {
                    Id = o.Id,
                    Number = o.Number,
                    Client = o.Client,
                    Capacity = o.Capacity,
                    Value = o.Value,
                    DeliveryDate = o.DeliveryDate,
                    IsActive = o.IsActive,
                    IsDeleted = o.IsDeleted
                })
                .ToList();

            return new PaginatedListResponseDto<OrderDto> { RowsCount = itemsCount, Data = items };
        }
    }
}