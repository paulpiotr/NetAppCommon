#region using

using System.Threading.Tasks;
using NetAppCommon.AppSettings.Models.Base;

#endregion

namespace NetAppCommon.AppSettings.Repositories.Interface
{
    public interface IAppSettingsRepository<TAppSettings> where TAppSettings : AppSettingsModel
    {
        public TAppSettings Save(TAppSettings appSettings = null);

        public Task<TAppSettings> SaveAsync(TAppSettings appSettings = null);

        public void MergeAndSave(string sourceFilePath, string destFilePath);

        public Task MergeAndSaveAsync(string sourceFilePath, string destFilePath);

        public TAppSettings MergeAndSave(TAppSettings appSettings = null);

        public Task<TAppSettings> MergeAndSaveAsync(TAppSettings appSettings = null);

        public TAppSettings CopyToUserDirectory(TAppSettings appSettings = null);

        public Task<TAppSettings> CopyToUserDirectoryAsync(TAppSettings appSettings = null);

        public TAppSettings MergeAndCopyToUserDirectory(TAppSettings appSettings = null);

        public Task<TAppSettings> MergeAndCopyToUserDirectoryAsync(TAppSettings appSettings = null);

        public TValue GetValue<TValue>(string key);

        public Task<TValue> GetValueAsync<TValue>(string key);

        public TValue GetValue<TValue>(TAppSettings appSettings, string key);

        public Task<TValue> GetValueAsync<TValue>(TAppSettings appSettings, string key);

        public TValue GetValue<TValue>(string filePath, string key);

        public Task<TValue> GetValueAsync<TValue>(string filePath, string key);

        public bool MssqlCanConnect(string connectionString);

        public Task<bool> MssqlCanConnectAsync(string connectionString);

        public bool MssqlCheckConnectionString(string connectionString);

        public Task<bool> MssqlCheckConnectionStringAsync(string connectionString);
    }
}