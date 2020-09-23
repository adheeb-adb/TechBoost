using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Contracts
{
    public interface IBlobService
    {
        Task CreateBlobContainerAsync(bool publicAccessAllowed);

        Task<Stream> OpenWriteBlobAsync(string blobId);

        Task WriteBlobAsync(string blobId, Stream source);

        Task WriteBlobAsync(string blobId, string text);

        Task<Stream> OpenReadBlobAsync(string blobId);

        Task ReadBlobAsync(string blobId, Stream target);

        Task<T> ReadBlobContentAsync<T>(string container, string basePath, string blobId);

        Task DeleteBlobAsync(string blobId);

        Task<bool> BlobExistsAsync(string blobId);

        // BlobSasTokenDto GenerateSasForBlob(string blobId, TimeSpan expiresIn, BlobSasPermissions permission);
    }
}
