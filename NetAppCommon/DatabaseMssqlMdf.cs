using System.Threading.Tasks;

namespace NetAppCommon
{
    public class DatabaseMssqlMdf : Mssql.DatabaseMssqlMdf
    {
        private static DatabaseMssqlMdf Instance = null;

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
            if (Instance == null)
            {
                Instance = new DatabaseMssqlMdf();
            }
            return Instance;
        }

        public static DatabaseMssqlMdf GetInstance(string connectionString)
        {
            if (Instance == null)
            {
                Instance = new DatabaseMssqlMdf(connectionString);
            }
            return Instance;
        }

        public static DatabaseMssqlMdf GetInstance(string connectionStringName, string settingsJsonFileName)
        {
            if (Instance == null)
            {
                Instance = new DatabaseMssqlMdf(connectionStringName, settingsJsonFileName);
            }
            return Instance;
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
