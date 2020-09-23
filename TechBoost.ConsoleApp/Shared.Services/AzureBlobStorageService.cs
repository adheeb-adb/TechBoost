using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Shared.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class AzureBlobStorageService : IBlobService
    {
        private string _basePath;
        private CloudBlobContainer _blobContainer;
        // private URLInfo _urlInfo;

        public AzureBlobStorageService(string connectionString, string blobContainerName, string basePath)
        {
            _basePath = string.IsNullOrEmpty(basePath) ? string.Empty : basePath + "/";

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            _blobContainer = blobClient.GetContainerReference(blobContainerName);

            // _urlInfo = urlInfo;
        }

        public async Task CreateBlobContainerAsync(bool publicAccessAllowed)
        {
            var accessType = publicAccessAllowed ? BlobContainerPublicAccessType.Blob : BlobContainerPublicAccessType.Off;
            await _blobContainer.CreateIfNotExistsAsync(accessType, null, null);
        }

        /*public BlobSasTokenDto GenerateSasForBlob(string blobId, TimeSpan expiresIn, BlobSasPermissions permission)
        {
            var policy = new SharedAccessBlobPolicy();
            policy.Permissions = (SharedAccessBlobPermissions)permission;
            policy.SharedAccessExpiryTime = DateTime.UtcNow.Add(expiresIn);

            var blob = GetBlobReference(blobId);
            var sasToken = blob.GetSharedAccessSignature(policy);

            return new BlobSasTokenDto
            {
                BlobUri = blob.Uri.ToString().Replace(_urlInfo.AzureStorageBaseUrl, _urlInfo.ApiManagementStorageBaseUrl),
                SasToken = sasToken
            };
        }*/

        public async Task<Stream> OpenWriteBlobAsync(string blobId)
        {
            var blob = GetBlobReference(blobId);
            var stream = await blob.OpenWriteAsync();
            return stream;
        }

        public async Task WriteBlobAsync(string blobId, Stream source)
        {
            var blob = GetBlobReference(blobId);
            var blobIdElements = blobId.Split('.');
            if (blobIdElements[blobIdElements.Length - 1] == "svg")
            {
                blob.Properties.ContentType = "image/svg+xml";
            }

            await blob.UploadFromStreamAsync(source);
        }

        public async Task WriteBlobAsync(string blobId, string text)
        {
            var blob = GetBlobReference(blobId);
            await blob.UploadTextAsync(text);
        }

        public Task WriteBlobAsync(string blobId, Stream source, TimeSpan timeToLive)
        {
            // Azure blob storage does not support expiration yet.
            throw new NotImplementedException();
        }

        public async Task<Stream> OpenReadBlobAsync(string blobId)
        {
            var blob = GetBlobReference(blobId);
            var stream = await blob.OpenReadAsync();
            return stream;
        }

        public async Task ReadBlobAsync(string blobId, Stream target)
        {
            var blob = GetBlobReference(blobId);
            await blob.DownloadToStreamAsync(target);
        }

        public async Task<T> ReadBlobContentAsync<T>(string container, string basePath, string blobId)
        {
            try
            {
                using (var inStreamReader = new StreamReader(await OpenReadBlobAsync(blobId)))
                {
                    string blobContent = inStreamReader.ReadToEnd();
                    var returnObject = JsonConvert.DeserializeObject<T>(blobContent);
                    return returnObject;
                }
            }
            catch (StorageException)
            {
                // If there is no blob for perticular customer
            }

            return default(T);
        }

        public async Task DeleteBlobAsync(string blobId)
        {
            await GetBlobReference(blobId).DeleteIfExistsAsync();
        }

        public async Task<byte[]> ReadBlobAsync(string blobId)
        {
            var blob = GetBlobReference(blobId);

            var bytes = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(bytes, 0);

            return bytes;
        }

        public async Task<bool> BlobExistsAsync(string blobId)
        {
            var blob = GetBlobReference(blobId);
            return await blob.ExistsAsync();
        }

        private CloudBlockBlob GetBlobReference(string blobId)
        {
            return _blobContainer.GetBlockBlobReference($"{_basePath}{blobId}");
        }
    }
}
