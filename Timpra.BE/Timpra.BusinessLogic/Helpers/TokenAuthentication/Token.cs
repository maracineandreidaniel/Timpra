
using System;

namespace Timpra.BusinessLogic.Helpers.TokenAuthentication;
public class Token
{
    public string Value { get; set; }
    public DateTime ExpiryDate { get; set; }
}
