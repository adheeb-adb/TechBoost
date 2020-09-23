namespace Shared.Domain.Contracts.Factories
{
    public interface IBlobServiceFactory
    {
        IBlobService CreateBlobService(string container, string basePath);

        IBlobService CreateDataLakeService(string container, string basePath);
    }
}
