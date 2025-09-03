using System.Linq;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.DataAccess.Entities;
using Riok.Mapperly.Abstractions;

namespace Timpra.BusinessLogic.Mappers;

[Mapper(UseDeepCloning = true)]
public static partial class OrderMapper
{
    public static partial OrderDto MapToDto(this Order order);
    public static partial Order MapFromDto(this OrderDto orderDto);

    public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> orders);
}