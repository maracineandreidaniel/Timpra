using System;

#nullable disable

namespace Timpra.DataAccess.Entities
{
    public partial class Order
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Client { get; set; }
        public int Capacity { get; set; }
        public decimal Value { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
