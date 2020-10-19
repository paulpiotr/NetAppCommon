namespace NetAppCommon.Mssql
{
    public interface IDatabaseMssqlMdf
    {
        string GetCreateScript(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null);
        bool Create(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null);
        bool Drop(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null);
    }
}
