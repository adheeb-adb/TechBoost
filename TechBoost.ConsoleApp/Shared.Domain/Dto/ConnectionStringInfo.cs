using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Domain.Dto
{
    public class ConnectionStringInfo
    {
        public string StorageConnectionString { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string DataLakeConnectionString { get; set; }
    }
}
