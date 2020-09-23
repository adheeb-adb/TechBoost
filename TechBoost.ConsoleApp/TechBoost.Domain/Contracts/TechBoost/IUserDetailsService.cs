using System;
using System.Collections.Generic;
using System.Text;
using TechBoost.Domain.Models;

namespace TechBoost.Domain.Contracts.TechBoost
{
    public interface IUserDetailsService
    {
        List<User> GetOrderedUsersFromStringList(List<string> users);
    }
}
