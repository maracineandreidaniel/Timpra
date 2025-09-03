using System.Linq;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.DataAccess.Entities;
using Riok.Mapperly.Abstractions;

namespace Timpra.BusinessLogic.Mappers;

[Mapper(UseDeepCloning = true)]
public static partial class UserMapper
{
    public static partial UserDTO MapToDto(this User user);
    public static partial User MapFromDto(this UserDTO user);

    public static partial IQueryable<UserDTO> ProjectToDto(this IQueryable<User> users);
}