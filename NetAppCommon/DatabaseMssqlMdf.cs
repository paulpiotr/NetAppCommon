using System.Threading.Tasks;

namespace NetAppCommon
{
    public class DatabaseMssqlMdf : Mssql.DatabaseMssqlMdf
    {
        private static DatabaseMssqlMdf _instance = null;

        public override string ConnectionString { get => base.ConnectionString; set => base.ConnectionString = value; }
        public override string ConnectionStringName { get => base.ConnectionStringName; set => base.ConnectionStringName = value; }
        public override string SettingsJsonFileName { get => base.SettingsJsonFileName; set => base.SettingsJsonFileName = value; }

        public DatabaseMssqlMdf()
        {
        }

        public DatabaseMssqlMdf(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DatabaseMssqlMdf(string connectionStringName, string settingsJsonFileName)
        {
            ConnectionStringName = connectionStringName;
            SettingsJsonFileName = settingsJsonFileName;
        }

        public static DatabaseMssqlMdf GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseMssqlMdf();
            }
            return _instance;
        }

        public static DatabaseMssqlMdf GetInstance(string connectionString)
        {
            if (_instance == null)
            {
                _instance = new DatabaseMssqlMdf(connectionString);
            }
            return _instance;
        }

        public static DatabaseMssqlMdf GetInstance(string connectionStringName, string settingsJsonFileName)
        {
            if (_instance == null)
            {
                _instance = new DatabaseMssqlMdf(connectionStringName, settingsJsonFileName);
            }
            return _instance;
        }

        public async Task<bool> CreateAsync()
        {
            return await Task.Run(() =>
             {
                 return base.Create(ConnectionString, ConnectionStringName, SettingsJsonFileName);
             });
        }

        public bool Create()
        {
            return base.Create(ConnectionString, ConnectionStringName, SettingsJsonFileName);
        }

        public override bool Create(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null)
        {
            return base.Create(connectionString, connectionStringName, settingsJsonFileName);
        }
    }
}
