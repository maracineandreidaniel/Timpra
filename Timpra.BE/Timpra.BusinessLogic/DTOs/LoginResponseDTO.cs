using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timpra.BusinessLogic.DTOs
{
    public class LoginResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Token { get; set; }
    }
}
