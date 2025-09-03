using System.Collections.Generic;

namespace Timpra.BusinessLogic.DTOs
{
    public class PaginatedListResponseDto<T> where T : class
    {
        public int RowsCount { get; set; }
        public List<T> Data { get; set; }
    }
}
