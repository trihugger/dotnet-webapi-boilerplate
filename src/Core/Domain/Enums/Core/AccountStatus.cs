using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DN.WebApi.Domain.Enums
{
    public enum AccountStatus
    {
        Pending, // All
        Active, // All
        Dormant, // Deposit
        Closed, // All
        ChargedOff, // Loans
        Refinanced // Loans
    }
}
