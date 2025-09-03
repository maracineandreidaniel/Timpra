using System;
using System.ComponentModel.DataAnnotations;

namespace Timpra.API.Test.ContractTests.Common.Models;

internal sealed record OrderDtoTestModel([Required] int Id,
        [Required] string Number,
        [Required] string Client,
        [Required] int Capacity,
        [Required] decimal Value,
        [Required] DateTime DeliveryDate,
        [Required] bool IsActive,
        [Required] bool IsDeleted);
