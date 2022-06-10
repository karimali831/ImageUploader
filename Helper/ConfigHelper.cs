namespace ImageUploader.Helper
{
    public interface IConfigHelper
    {
        string SQLConnectionString { get; }
        string AzureStorageSrcFolder { get; }
        string AzureConnectionString { get; }
        string AzureStorageContainer { get; }
        IConfigurationSection GetConfigurationSection(string key);
    }

    public class ConfigHelper : IConfigHelper
    {
        private readonly IConfiguration _configuration;

        public ConfigHelper(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public string SQLConnectionString => _configuration["Configuration:SQLConnectionString"];
        public string AzureConnectionString => _configuration["AzureStorage:ConnectionString"];
        public string AzureStorageSrcFolder => _configuration["AzureStorage:SourceFolder"];
        public string AzureStorageContainer => _configuration["AzureStorage:Container"];

        public IConfigurationSection GetConfigurationSection(string key)
        {
            return _configuration.GetSection(key);
        }
    }
}
