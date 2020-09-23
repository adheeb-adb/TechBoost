using System;
using System.Collections.Generic;
using System.Text;

namespace TechBoost.Domain.Models
{
    public class UserDetailsConfiguration
    {
        public string UnsortedDetailsFilePath { get; set; }

        public string SortedDetailsFilePath { get; set; }

        public string RoleSeparatorChar { get; set; }
    }
}
