using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TechBoost.Domain.Contracts.Application
{
    public interface IApplication
    {
        Task Run();
    }
}
