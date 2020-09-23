using Shared.Domain.Contracts;
using Shared.Domain.Contracts.Factories;
using Shared.Domain.Dto;

namespace Shared.Services.Factories
{
    public class BlobServiceFactory : IBlobServiceFactory
    {
        private readonly ConnectionStringInfo _connectionStringInfo;
        // private readonly URLInfo _urlInfo;

        public BlobServiceFactory(ConnectionStringInfo connectionStringInfo)
        {
            _connectionStringInfo = connectionStringInfo;
            // _urlInfo = urlInfo;
        }

        public IBlobService CreateBlobService(string container, string basePath)
        {
            var connectionString = _connectionStringInfo.StorageConnectionString;
            var blobService = new AzureBlobStorageService(connectionString, container, basePath);
            return blobService;
        }

        public IBlobService CreateDataLakeService(string container, string basePath)
        {
            var connectionString = _connectionStringInfo.DataLakeConnectionString;
            var blobService = new AzureBlobStorageService(connectionString, container, basePath);
            return blobService;
        }
    }
}
