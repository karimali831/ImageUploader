

using ImageUploader.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using ImageUploader.Helper;

namespace ImageUploader.Repository
{
    public interface IImageUploadRepository
    {
        Task<bool> UploadImageAsync(ImageUpload model);
        Task<ImageUpload> GetImageAsync(Guid Id);
        Task<IEnumerable<ImageUpload>> GetAllImagesAsync();
    }

    public class ImageUploadRepository : IImageUploadRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly string TABLENAME = "ImageUpload";
        private static readonly string[] FIELDS = typeof(ImageUpload).DapperFields();

        public ImageUploadRepository(IConfigHelper config)
        {
            this._dbConnection = new SqlConnection(config.SQLConnectionString);
        }

        public async Task<ImageUpload> GetImageAsync(Guid Id)
        {
            try
            {
                return await _dbConnection.QueryFirstOrDefaultAsync<ImageUpload>($"{DapperHelper.SELECT(TABLENAME, FIELDS)} WHERE Id = @Id", new { Id });
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }

        public async Task<IEnumerable<ImageUpload>> GetAllImagesAsync()
        {
            try
            {
                return await _dbConnection.QueryAsync<ImageUpload>($"{DapperHelper.SELECT(TABLENAME, FIELDS)}");
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }

        public async Task<bool> UploadImageAsync(ImageUpload model)
        {
            try
            {
                await _dbConnection.ExecuteAsync(DapperHelper.INSERT(TABLENAME, FIELDS), model);
                return true;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }
    }
}
