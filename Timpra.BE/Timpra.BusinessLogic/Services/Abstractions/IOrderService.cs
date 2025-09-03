using Timpra.BusinessLogic.DTOs;
using Timpra.BusinessLogic.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timpra.BusinessLogic.Services.Abstractions
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderDto>> GetAll();
        public Task<OrderDto> AddAsync(OrderDto item, bool applyChanges = true);
        public Task<OrderDto> UpdateAsync(OrderDto item, int id, bool applyChanges = true);
        public Task<OrderDto> RemoveAsync(int orderId, bool applyChanges = true);
        public Task<OrderDto> GetByIdAsync(int id, bool applyChanges = true);
        public Task<OrderDto> ArchiveAsync(int orderId, bool applyChanges = true);
        public Task<PaginatedListResponseDto<OrderDto>> GetOrdersPaginatedAsync(int pageIndex, int itemsNumber, string sortField, string? sortDirection, OrderListFilterDto filter, bool applyChanges = true);
    }
}
