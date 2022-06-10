using ImageUploader.Helper;
using Microsoft.WindowsAzure.Storage;

namespace ImageUploader.Service
{
    public interface IBlobStorageService
    {
        Task<string> Upload(string fileName, byte[] fileData, string fileMimeType);

    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfigHelper config;


        public BlobStorageService(IConfigHelper config)
        {
            this.config = config;
        }

        public async Task<string> Upload(string fileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(config.AzureConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
     
                var cloudBlobContainer = blobClient.GetContainerReference(config.AzureStorageContainer);

                if (fileName != null && fileData != null)
                {
                    var blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    blockBlob.Properties.ContentType = fileMimeType;

                    await blockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return blockBlob.Uri.AbsoluteUri;
                }
                return null;
            }
            catch 
            {
                return null;
                //throw new Exception(ex.Message);
            }
        }

    }
}
