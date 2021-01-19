using System.Threading.Tasks;
using NetAppCommon.AppSettings.Models.Base;

namespace NetAppCommon.AppSettings.Repositories.Interface
{
    public interface IAppSettingsRepository<TAppSettings> where TAppSettings : AppSettingsBaseModel
    {
        public TAppSettings Save(TAppSettings appSettings = null);

        public Task<TAppSettings> SaveAsync(TAppSettings appSettings = null);

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
    }
}
