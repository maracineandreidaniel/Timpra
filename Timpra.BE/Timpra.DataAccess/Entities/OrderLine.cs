using System;
using System.Collections.Generic;

#nullable disable

namespace Timpra.DataAccess.Entities
{
    public partial class OrderLine
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}
