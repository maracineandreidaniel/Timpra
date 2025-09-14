using System;
using System.Collections.Generic;

#nullable disable

namespace Timpra.DataAccess.Entities
{
    public partial class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }

        public Role Role { get; set; }
        public int RoleId { get; set; }

    }
}
